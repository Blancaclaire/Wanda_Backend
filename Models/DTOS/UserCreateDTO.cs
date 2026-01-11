namespace Models
{
    public class UserCreateDTO
    {
    
    public string Name {get; set; }

    public string Email {get; set; }

    public string Password {get; set; }


    public UserCreateDTO()
    {
        
    }

    public UserCreateDTO(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }
    }
}