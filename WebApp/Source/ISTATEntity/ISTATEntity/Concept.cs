using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class Concept: ParentableItem
    {

        public Concept(string code, string name, string description, string parentConcept)
            : base(code,name, description, parentConcept)
        {
        }

    }
}
