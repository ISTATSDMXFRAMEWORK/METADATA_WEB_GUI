using System.Configuration;

namespace ISTAT.EXPORT.Settings
{
    public class ContactSettingsHandler : System.Configuration.ConfigurationSection
    {
        [ConfigurationProperty("Name", DefaultValue = "", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 0, MaxLength = 60)]
        public string Name
        {
            get
            { return (string)this["Name"]; }
            set
            { this["Name"] = value; }
        }

        [ConfigurationProperty("Direction", DefaultValue = "", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 0, MaxLength = 255)]
        public string Direction
        {
            get
            { return (string)this["Direction"]; }
            set
            { this["Direction"] = value; }
        }

        [ConfigurationProperty("Email", DefaultValue = "", IsRequired = true)]
        public string Email
        {
            get
            { return (string)this["Email"]; }
            set
            { this["Email"] = value; }
        }

    }
    public class SecuritySettingsHandler : System.Configuration.ConfigurationSection {

        [ConfigurationProperty("UserGroup", DefaultValue = "", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 0, MaxLength = 60)]
        public string UserGroup
        {
            get
            { return (string)this["UserGroup"]; }
            set
            { this["UserGroup"] = value; }
        }

        [ConfigurationProperty("Domain", DefaultValue = "", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 0, MaxLength = 60)]
        public string Domain
        {
            get
            { return (string)this["Domain"]; }
            set
            { this["Domain"] = value; }
        }
    }

    public class Languages : System.Configuration.ConfigurationSection
    {

        [ConfigurationProperty("Values", DefaultValue = "", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 0, MaxLength = 60)]
        public string Values
        {
            get
            { return (string)this["Values"]; }
            set
            { this["Values"] = value; }
        }
    }

    
}
