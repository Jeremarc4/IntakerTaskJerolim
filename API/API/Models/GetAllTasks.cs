namespace API.Models
{
    public class GetAllTasks
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public BaseHelper Status { get; set; }
        public string? AssignedTo { get; set; }
    }
}
