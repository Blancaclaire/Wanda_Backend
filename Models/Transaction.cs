namespace Models;

public class Transaction
{   
    public enum Transaction_type{ income, expense, saving }
    public enum Splits_type{ individual, contribution, divided }
    public enum EFrecuency{mouthly, weekly, annual}
    public int Transaction_id { get; set; }
    public int User_id { get; set; }
    public int Objective_id { get; set; }
    public Transaction_type Transactions_Type { get; set; }
    public Splits_type Splitstype { get; set; }
    public EFrecuency Frecuency { get; set; }
    public string Category { get; set; }
    public double Amount { get; set; }
    public string Concept{ get; set; }
    public bool  IsRecurring { get; set; }
    public DateTime Transactions_date { get; set; }
    public DateTime End_date { get; set; }

public Transaction(Transaction_type transaction_Type, Splits_type splits_type, EFrecuency frecuency, int transactions_id, int user_id, int objective_id, double amount, string concept, bool isRecurring, string category, DateTime transactions_date, DateTime end_date)
{       
        Transactions_Type = transaction_Type;
        Splitstype = splits_type;
        Frecuency = frecuency;
        Category = category;
        Transaction_id = transactions_id;
        User_id = user_id;
        Objective_id = objective_id;
        Amount = amount;
        Concept = concept;
        IsRecurring = isRecurring;
        Transactions_date = transactions_date;
        End_date = end_date;
}
}