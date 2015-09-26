using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.MetadataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Registry;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Registry;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Mapping;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Mapping;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Codelist;

namespace ISTATRegistry
{
    public class SDMXUtils
    {
        public SDMXUtils()
        {
        }

        public IDataStructureMutableObject buildDataStructure(string id, string agency, string version)
        {
            IDataStructureMutableObject dsd = new DataStructureMutableCore();
            dsd.Id = id;
            dsd.AgencyId = agency;
            dsd.Version = version;

            return dsd;
        }

        public IDataflowMutableObject buildDataFlow(string id, string agency, string version)
        {
            IDataflowMutableObject df = new DataflowMutableCore();
            df.Id = id;
            df.AgencyId = agency;
            df.Version = version;

            return df;
        }

        public IContentConstraintMutableObject buildContentConstraint(string id, string agency, string version)
        {
            IContentConstraintMutableObject ccs = new ContentConstraintMutableCore();
            ccs.Id = id;
            ccs.AgencyId = agency;
            ccs.Version = version;

            return ccs;
        }

        public IStructureSetMutableObject buildStructureSet(string id, string agency, string version)
        {
            IStructureSetMutableObject ss = new StructureSetMutableCore();
            ss.Id = id;
            ss.AgencyId = agency;
            ss.Version = version;

            return ss;
        }


        public IHierarchicalCodelistMutableObject buildHcl(string id, string agency, string version)
        {
            IHierarchicalCodelistMutableObject hcl = new HierarchicalCodelistMutableCore();
            hcl.Id = id;
            hcl.AgencyId = agency;
            hcl.Version = version;

            return hcl;
        }
    }
}