namespace Models;

public class AccountUsers
{

    public enum UserRole { member, admin }

    public int User_id { get; set; }
    public int Account_id { get; set; }
    public UserRole Role { get; set; }

    public AccountUsers()
    {
        
    }

    public AccountUsers(int user_id, int account_id, UserRole role)
    {
        User_id = user_id;
        Account_id = account_id;
        Role = role;
    }

}