using Microsoft.EntityFrameworkCore;

namespace ITS.Server.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ProjectDbContext dbContext;

        public InventoryService(ProjectDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //Get BOM BySubJob Number
        public async Task<List<BOM>> GetBOMBySubJobNumberAsync(int subJobNumber)
        {
            var boms = await dbContext.BillOfMaterials
            .Where(bom => bom.SubJobId == subJobNumber)
            .ToListAsync();

            return boms;

        }

        //Get Jobs By ProjectNumber
        public async Task<List<Job>> GetJobsByProjectNumberAsync(int projectNumber)
        {
            var jobs = await dbContext.Jobs
           .Where(j => j.ProjectId == projectNumber)
           .ToListAsync();

            return jobs;

        }

        //Get ProjectData (using Eager loading)
        public async Task<Project> GetProjectDataAsync(int projectNumber)
        {
            var project = await dbContext.Projects
        .Include(p => p.Jobs)
            .ThenInclude(j => j.SubJobs)
                .ThenInclude(sj => sj.BillOfMaterials)
                    .ThenInclude(bom => bom.Material)
        .SingleOrDefaultAsync(p => p.Id == projectNumber);

            return project;
        }

        // Get Sub Jobs By JobNumber
        public async Task<List<SubJob>> GetSubJobsByJobNumberAsync(int jobNumber)
        {
            var subJobs = await dbContext.SubJobs
            .Where(sj => sj.JobId == jobNumber)
            .ToListAsync();

            return subJobs;
        }

        // Search By Project Number
        public async Task<Project?> SearchByProjectNumberAsync(int projectNumber)
        {
            Project project = dbContext.Projects.FirstOrDefault(p => p.Id == projectNumber);

            return project;
        }
    }
}
