namespace Models;

public class AccountPercentajeDto
{
    public string Name { get; set; }
    public int ObjectiveId { get; set; }
    public int Percentage { get; set; }       
    public double AmountRemaining { get; set; } 
    public bool IsCompleted { get; set; }

    public AccountPercentajeDto()
    {
        
    }

}