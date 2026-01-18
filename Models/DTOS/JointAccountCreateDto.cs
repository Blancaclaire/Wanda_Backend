namespace Models;

public class JointAccountCreateDto
{
    
    public string Name {get; set;}
    public List<int> Member_Ids {get; set; }

    public int OwnerId { get; set; }

    public JointAccountCreateDto()
    {
        
    }

    
    public JointAccountCreateDto(string name, List<int> member_Ids, int ownerId)
    {
        Name = name; 
        Member_Ids = member_Ids;
        OwnerId = ownerId;
    }
}