using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class Agency : Item
    {
         public Agency(string id, string name, string description )
            : base(id, name, description )
        {
        }

    }
}
