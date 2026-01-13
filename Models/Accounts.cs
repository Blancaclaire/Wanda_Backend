namespace Models;

public class Account{
    public int Account_id { get; set; }
    public string Name {get; set; }
    public string Account_Type {get; set; }
    public double  Account{get; set; }
    public double   Paweekly_budget {get; set; }
    public double  Monthly_budget {get; set; }
    public double  Account_picture_url {get; set;}

    public Account()
    {
        
    }

    public Account(int account_id , string name, string account_type, double  Account, double  paweekly_budget, double  monthly_budget, double  account_picture_url)
    {
        Account_id = account_id;
        Name = name;
        Account_Type = account_type;
        Paweekly_budget = paweekly_budget;
        Monthly_budget = monthly_budget;
        Account_picture_url =  account_picture_url;
        Password = password;
    }
    
}