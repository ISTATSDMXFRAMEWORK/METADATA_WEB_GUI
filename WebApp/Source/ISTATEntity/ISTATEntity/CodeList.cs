using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class CodeList: NameableIdentity
    {

        public CodeList(string name, string iD, string agency, string version)
            : base(name, iD,agency,version)
        {
        }
        public CodeList(string name, string iD, string agency, string version, bool isFinal)
            : base(name, iD, agency, version, isFinal)
        {
        }

    }
}
