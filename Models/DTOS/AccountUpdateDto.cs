namespace Models;

public class AccountUpdateDto
{
    public string Name { get; set; }

    public double Amount { get; set; }

    public double Weekly_budget { get; set; }

    public double Monthly_budget { get; set; }
    public string Account_picture_url { get; set; }


    public AccountUpdateDto()
    {
        
    }

    public AccountUpdateDto(string name, double amount, double weekly_budget, double monthly_budget, string account_picture_url)
    {
        Name = name;
        Amount = amount;
        Weekly_budget = weekly_budget;
        Monthly_budget = monthly_budget;
        Account_picture_url = account_picture_url;
    }
}