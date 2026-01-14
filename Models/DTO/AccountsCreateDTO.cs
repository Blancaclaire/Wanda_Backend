namespace Models;

public class CreateAccountDto
{
    public string Name { get; set; }
    public string AccountType { get; set; }
    public double Balance { get; set; }
    public double WeeklyBudget { get; set; }
    public double MonthlyBudget { get; set; }
    public string AccountPictureUrl { get; set; }
    public string Password { get; set; }
}