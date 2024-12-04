namespace Base.Models
{
    public class IntakerTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
        public int StatusId { get; set; }
        public string? AssignedTo { get; set; }
    }
}
