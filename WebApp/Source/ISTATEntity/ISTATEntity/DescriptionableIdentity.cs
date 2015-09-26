using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class DescriptionableIdentity : NameableIdentity
    {
        public string Description { get; set; }

        public DescriptionableIdentity(string description, string name, string iD, string agency, string version)
            : base(name,iD,agency,version)
        {
            this.Description = description;
        }
    }
}
