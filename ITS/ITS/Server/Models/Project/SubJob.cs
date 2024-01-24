namespace ITS.Server.Models.Project
{
    public class SubJob
    {
        public int Id { get; set; }

        public string Equipment { get; set; } 
        public List<BOM> BillOfMaterials { get; set; }


        public int JobId { get; set; }
        public Job Job { get; set; }

    }
}
