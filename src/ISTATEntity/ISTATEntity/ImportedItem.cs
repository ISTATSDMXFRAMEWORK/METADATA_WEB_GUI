using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class ImportedItem: NameableIdentity
    {

        public ImportedItem(string name, string iD, string agency, string version, string type, bool isOk)
            : base(name, iD,agency,version)
        {
            this._type = type;
            this._isOk = isOk;
        }

        public ImportedItem(string name, string iD, string agency, string version, bool isFinal, string type, bool isOk)
            : base(name, iD, agency, version, isFinal)
        {
            this._type = type;
            this._isOk = isOk;
        }

        public string _type
        {
            get 
            {
                return this.importedType;
            }

            set
            {
                this.importedType = value;
            }
        }

        public bool _isOk
        {
            get 
            {
                return this.isOk;
            }

            set
            {
                this.isOk = value;
            }
        }

        private string importedType;
        private bool isOk;

    }
}
