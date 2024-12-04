namespace API.Models
{
    public class AddTask
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int StatusId { get; set; }
        public string? AssignedTo { get; set; }
    }
}
