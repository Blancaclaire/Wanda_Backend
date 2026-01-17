namespace Models;

public class User
{
    public int User_id { get; set; }
    public string Name {get; set; }

    public string Email {get; set; }

    public string Password {get; set; }


    public User()
    {
        
    }

    public User(int user_id, string name, string email, string password)
    {
        User_id = user_id;
        Name = name;
        Email = email;
        Password = password;
    }
    
    
}