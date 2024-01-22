namespace ITS.Server.Contracts
{
    public interface IAuthenticationService
    { 
        Task<LoginResult> Login(LoginModel loginModel);
        Task Logout();
    }
}
