using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class StructureMap
    {
        public string Id { get; set; }
        public string ArtefactSource { get; set; }
        public string ArtefactTarget { get; set; }
        public string ArtefactType { get; set; }

        public StructureMap()
        {
        }

        public StructureMap(string id, string artefactSource, string artefactTarget, string artefactType)
        {
            this.Id = id;
            this.ArtefactSource = artefactSource;
            this.ArtefactTarget = artefactTarget;
            this.ArtefactType = artefactType;
        }

    }
}
