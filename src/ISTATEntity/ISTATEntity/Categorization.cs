using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class Categorization: NameableIdentity
    {
        public Categorization(string name, string iD, string agency, string version, string categorySchemeReference, string categoryReference, string structureReference )
            : base(name, iD,agency,version)
        {
            this._categorySchemeReference = categorySchemeReference;
            this._categoryReference = categoryReference;
            this._structureReference = structureReference;
        }

        public Categorization(string name, string iD, string agency, string version, bool isFinal, string categorySchemeReference, string categoryReference, string structureReference)
            : base(name, iD, agency, version, isFinal)
        {
            this._categorySchemeReference = categorySchemeReference;
            this._categoryReference = categoryReference;
            this._structureReference = structureReference;
        }

        public string _categorySchemeReference
        {
            get
            {
                return this.categorySchemeReference;
            }

            set
            {
                this.categorySchemeReference = value;
            }
        }

        public string _categoryReference
        {
            get
            {
                return this.categoryReference;
            }

            set
            {
                this.categoryReference = value;
            }
        }

        public string _structureReference
        {
            get
            {
                return this.structureReference;
            }

            set
            {
                this.structureReference = value;
            }
        }

        private string structureReference;
        private string categoryReference;
        private string categorySchemeReference;
    }
}
