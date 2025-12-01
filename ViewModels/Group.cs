using System;
using System.Collections.Generic;
using System.Text;

namespace OMPS.ViewModels
{
    public class User
    {
        public Guid EmployeeID { get; set; } = Guid.Empty;
        public uint EmpNbr { get; set; } = 0;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => FullName + " " + LastName;
        public string UserInit => FullName[0..1] + LastName[0..1];
        public string Email { get; set; } = string.Empty;
        public AccessLevel LevelOfAccess { get; set; } = AccessLevel.None;
        public User()
        {
            
        }
    }

    public enum AccessLevel
    {
        None = -1,
        Minimal,
        Base,
        Elevated,
        Super,
        Admin,
        IT,
        Dev
    }

    public class Permission
    {
        public Guid PermID = Guid.Empty;
        public string Name { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public Permission(string scope, string name)
        {
            this.Name = name;
            this.Scope = scope == "" ? "global" : scope.ToLower();
            this.PermID = new();
        }
    };

    public class Foo
    {
        public List<Permission> Permissions = new List<Permission>()
        {
            new("EngOrder", "canModifyItemLine"),
            new("EngOrder", "canModifyHeaderGeneral"),
            new("EngOrder", "canModifyHeaderOther"),
            new("EngOrder", "canDeleteItemLine"),
        };
    }

    public class Group
    {
        public Guid GroupID { get; set; } = Guid.Empty;
        public string Name { get; set; } = string.Empty;
        public User[] Users { get; set; } = [];
    }

    public class ListView
    {
        public bool IsDefault = false;
        public Guid ListViewID { get; set; } = Guid.Empty;
        public User[] Users { get; set; } = [];
        public User[] Groups { get; set; } = [];
    }

    public class DetailView
    {
        public bool IsDefault = false;
        public Guid DetailViewID { get; set; } = Guid.Empty;
        public User[] Users { get; set; } = [];
        public User[] Groups { get; set; } = [];
    }

}
