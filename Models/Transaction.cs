namespace Models;

public class Transaction
{
    public enum ETransaction_type { income, expense, saving }
    public enum Split_type { individual, contribution, divided }
    public enum EFrequency{mouthly, weekly, annual}
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
    public EFrequency Frequency { get; set; }
    public DateTime End_date { get; set; }
    public Split_type Splittype { get; set; }

    public Transaction()
    {

    }

    public Transaction(int transaction_id, int account_id, int user_id, int objective_id, string category, double amount, ETransaction_type transaction_type, string concept, bool isRecurring, EFrequency frequency, Split_type split_type, DateTime transaction_date, DateTime end_date)
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
        Splittype = split_type;
    }
}