using System.Collections.Generic;

public class ManageUserRolesViewModel
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public List<string> AvailableRoles { get; set; } = new List<string>();
    public List<string> AllRoles { get; set; }
    public IList<string> UserRoles { get; set; } = new List<string>();
    public List<string> SelectedRoles { get; set; } = new List<string>();
}
