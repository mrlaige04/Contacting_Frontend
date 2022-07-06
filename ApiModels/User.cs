namespace Contacting_Frontend.ApiModels;

public class User
{
    public User(long tgid, string tgNickname)
    {
        TGID = tgid;
        tg_nickname = tgNickname;
    }

    public User()
    {
          
    }
     
    
    public long TGID { get; set; }
    public string tg_nickname { get; set; }
     
    public string? Name { get; set; }
     
    public Males? Male { get; set; }
     
     
    public int? age { get; set; }
     
    public string? description { get; set; }
    public string? city { get; set; }
    
    public string? photo_path { get; set; }

    public bool IsAllFieldsFillled() => Name != null && Male != null && age != null && description != null && city != null && photo_path != null;
    
}

public enum Males
{
    Male = 0,
    Female
}