namespace Models;

public class JointAccountCreateDto
{
    
    public string Name {get; set;}
    public List<int> Member_Ids {get; set; }

    public JointAccountCreateDto()
    {
        
    }

    
    public JointAccountCreateDto(string name)
    {
        Name = name; 
    }
}