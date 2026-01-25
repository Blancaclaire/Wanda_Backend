namespace Models;

public class AccountUsers
{

    public enum UserRole { member, admin }

    public int User_id { get; set; }
    public int Account_id { get; set; }
    public DateTime Joined_at { get; set; }

    public AccountUsers()
    {
        
    }

    public AccountUsers(int user_id, int account_id, DateTime joined_at )
    {
        User_id = user_id;
        Account_id = account_id;
        Joined_at = joined_at;
    }

}