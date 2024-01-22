using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


ConfigurationManager configuration = builder.Configuration;

#region Database Configuration
builder.Services.AddDbContext<IdentityDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("IdentityConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(15),
            errorNumbersToAdd: null);
        });
});

builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), new string[] { "IdentityAPI" })
                .AddCheck("IdentityDB-check", new SqlConnectionHealthCheck(
                            configuration.GetConnectionString("IdentityConnection")),
                            HealthStatus.Unhealthy, new string[] { "IdentityDB" });
#endregion  Database Configuration

#region Identity Configuration JwtIssuer

builder.Services.AddIdentityServices(configuration); // extension method: ../Extensions/IdentityServicesExtensions.cs

#endregion  Identity Configuration JwtIssuer

#region Other Services

builder.Services.AddScoped<ITokenService, TokenService>();

// Ref https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.httpservicecollectionextensions?view=aspnetcore-7.0
builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Identity.API", Version = "v1" });
});


builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin()
        );
});

builder.Services.AddTransient<SeedIdentity>();

#endregion


var app = builder.Build();

await SeedIdentityData(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity.API v1"));
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

async Task SeedIdentityData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        try
        {
            var context = scope.ServiceProvider.GetService<IdentityDbContext>();
            await context.Database.MigrateAsync();


            var userManager = scope.ServiceProvider.GetService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

            var service = scope.ServiceProvider.GetService<SeedIdentity>();
            await service.SeedUsersAndRolesAsync(userManager, roleManager, configuration);
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding initial identity data");
        }
    }
}
