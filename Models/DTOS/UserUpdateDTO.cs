namespace Models
{
    public class UserUpdateDTO
    {
    
    public string Name {get; set; }

    public string Password {get; set; }


    public UserUpdateDTO()
    {
        
    }

    public UserUpdateDTO(string name, string password)
    {
        Name = name;
        Password = password;
    }


    }
}