namespace Models;

public class AccountUsers
{

    public int User_id { get; set; }
    public int Account_id { get; set; }
    public string Role { get; set; }

    public AccountUsers()
    {
        
    }

    public AccountUsers(int user_id, int account_id, string role)
    {
        User_id = user_id;
        Account_id = account_id;
        Role = role;
    }

}