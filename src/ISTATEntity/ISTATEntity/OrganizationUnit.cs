using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class OrganizationUnit: ParentableItem
    {
        public OrganizationUnit(string code, string name, string description, string parentOrganizationUnit)
            : base( code, name, description, parentOrganizationUnit)
        {
            // NULL
        }
    }
}
