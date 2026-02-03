namespace Models;

public class TransactionCreateDTO
{

    public int User_id { get; set; }
    public int Objective_id { get; set; }
    public string Category { get; set; }
    public double Amount { get; set; }
    public string Transaction_type { get; set; } 
    public string Concept { get; set; }
    public DateTime Transaction_date { get; set; }
    public bool IsRecurring { get; set; }
    public string? Frequency { get; set; }        
    public DateTime? End_date { get; set; }
    public string Split_type { get; set; } 
    public DateTime? Last_execution_date { get; set; } 

    public List<TransactionSplitDetailDTO>? CustomSplits { get; set; }     
    
}
 