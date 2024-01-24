namespace ITS.Server.Models.Project
{
    public class Project
    {
        public int Id { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public string ProductName { get; set; }

        public List<Job> Jobs { get; set; }
    }
}
