namespace Models;

public class ObjetiveCreateDto
{   
    public int Account_id { get; set; } 
    public string? Name { get; set; }
    public double Target_amount { get; set; }
    public double Current_save  { get; set; } = 0;

    public DateTime Deadline { get; set; }
    public string? Objective_picture_url { get; set; }

}