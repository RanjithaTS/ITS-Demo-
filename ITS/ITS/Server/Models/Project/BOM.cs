namespace ITS.Server.Models.Project
{
    public class BOM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } 
        public int LineNum { get; set; } 
        public Material Material { get; set; }


        public int SubJobId { get; set; }
        public SubJob SubJob { get; set; }

    }
}
