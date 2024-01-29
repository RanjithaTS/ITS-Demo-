namespace ITS.Server.Contracts
{
    public interface IInventoryService
    {
        Task<Project> SearchByProjectNumberAsync(int projectNumber);

        Task<List<Job>> GetJobsByProjectNumberAsync(int projectNumber);

        Task<List<SubJob>> GetSubJobsByJobNumberAsync(int jobNumber);

        Task<Project> GetProjectDataAsync(int projectNumber);

        Task<List<BOM>> GetBOMBySubJobNumberAsync(int subJobNumber);
    }
}
