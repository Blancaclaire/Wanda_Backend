namespace Models;

public class AccountsUpdateDTO
{
    public string Name { get; set; }
    public string AccountType { get; set; }
    public double WeeklyBudget { get; set; }
    public double MonthlyBudget { get; set; }
    public string AccountPictureUrl { get; set; }
}