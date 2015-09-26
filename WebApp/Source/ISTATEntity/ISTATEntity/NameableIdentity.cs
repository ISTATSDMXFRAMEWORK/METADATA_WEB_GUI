using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class NameableIdentity : ArtefactIdentity
    {
        public string Name { get; set; }

        public NameableIdentity(string name, string iD, string agency, string version)
            : base(iD,agency,version)
        {
            this.Name = name;
        }
        public NameableIdentity(string name, string iD, string agency, string version,bool isFinal)
            : base(iD, agency, version, isFinal)
        {
            this.Name = name;
        }
    }
}
