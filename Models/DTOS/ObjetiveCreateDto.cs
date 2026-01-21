namespace DTOs
{
    public class ObjectiveCreateDto
    {
        public string Name { get; set; }
        public double Target_amount { get; set; }
        public DateTime Deadline { get; set; }
        public string Objective_picture_url { get; set; }
    }
}