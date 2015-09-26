using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class DataProviderScheme : NameableIdentity
    {

        public DataProviderScheme(string name, string ID, string agency, string version, bool isFinal = false)
            : base(name, ID, agency, version, isFinal)
        {
        }

    }
}
