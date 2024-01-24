namespace ITS.Server.Models.Project
{
    public class Job
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        public List<SubJob> SubJobs { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }



    }
}
