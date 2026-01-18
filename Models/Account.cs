namespace Models;

public class Account
{

    public enum AccountType { personal, joint }
    public int Account_id { get; set; }
    public string Name { get; set; }
    public AccountType Account_Type { get; set; }
    public double Amount { get; set; }
    public double Weekly_budget { get; set; }
    public double Monthly_budget { get; set; }
    public string Account_picture_url { get; set; }

    public Account()
    {

    }

    public Account(int account_id, string name, AccountType account_type, double amount, double weekly_budget, double monthly_budget, string account_picture_url)
    {
        Account_id = account_id;
        Name = name;
        Account_Type = account_type;
        Amount=amount;
        Weekly_budget = weekly_budget;
        Monthly_budget = monthly_budget;
        Account_picture_url = account_picture_url;
        
    }

}