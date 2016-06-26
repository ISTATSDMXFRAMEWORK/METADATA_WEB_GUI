using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class AssociatedAgency : Agency
    {
        public AssociatedAgency( string id, string name, string description, bool isOk ) : base( id, name, description )
        {
            this._isOk = isOk;
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

        private bool isOk;
    }
}
