namespace Models;

public class TransactionCreateDTO
{
    public enum ETransaction_type { income, expense, saving }
    public enum Split_type { individual, contribution, divided }
    public enum EFrecuency{mouthly, weekly, annual}
    public int Transaction_id { get; set; }
    public int Account_id { get; set; }
    public int User_id { get; set; }
    public int Objective_id { get; set; }
    public string Category { get; set; }
    public double Amount { get; set; }
    public ETransaction_type Transaction_type { get; set; }
    public string Concept { get; set; }
    public DateTime Transaction_date { get; set; }
    public bool IsRecurring { get; set; }
    public EFrecuency Frecuency { get; set; }
    public DateTime End_date { get; set; }
    public Split_type Splittype { get; set; }

}