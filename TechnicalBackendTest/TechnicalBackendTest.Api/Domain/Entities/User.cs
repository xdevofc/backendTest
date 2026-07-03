public class User
{
    public int Id {get; set;}
    public string Name {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
    public bool IsActive {get; set;} = true;
    public string PasswordHash {get; set;} = string.Empty;

    public ICollection<Address> Addresses {get;set;}  = new List<Address>();
}