namespace Models;
public class Transaction
{
    public int Transaction_id { get; set; }
    public int Account_id { get; set; }
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


    public Transaction()
    {

    }

    public Transaction(int transaction_id, int account_id, int user_id, int objective_id, string category, double amount, string transaction_type, string concept, bool isRecurring, string frequency, string split_type, DateTime transaction_date, DateTime end_date, DateTime last_execution_date )
    {
        Transaction_id = transaction_id;
        Account_id = account_id;
        User_id = user_id;
        Objective_id = objective_id;
        Category = category;
        Amount = amount;
        Transaction_type = transaction_type;
        Concept = concept;
        Transaction_date = transaction_date;
        IsRecurring = isRecurring;
        Frequency = frequency;
        End_date = end_date;
        Split_type = split_type;
        Last_execution_date= last_execution_date;
    }
}