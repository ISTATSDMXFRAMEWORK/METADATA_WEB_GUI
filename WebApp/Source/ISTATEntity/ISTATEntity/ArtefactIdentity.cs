using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class ArtefactIdentity
    {
        public string ID { get; set; }
        public string Agency { get; set; }
        public string Version { get; set; }
        public bool? IsFinal { get; set; }

        public ArtefactIdentity()
        {
        }

        public ArtefactIdentity(string ID, string agency, string version)
        {
            this.ID = ID;
            this.Agency = agency;
            this.Version = version;
        }

        public ArtefactIdentity(string ID, string agency, string version, bool? isFinal)
        {
            this.ID = ID;
            this.Agency = agency;
            this.Version = version;
            this.IsFinal = isFinal;
        }

        public override string ToString()
        {
            string ret;
            ret = String.Format("{0},{1},{2}", ID, Agency, Version);

            if (IsFinal != null)
                ret += String.Format(",{0}", IsFinal);

            return ret;

        }

    }
}
