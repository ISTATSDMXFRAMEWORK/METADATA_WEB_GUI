using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace ISTAT.WSDAL
{
    public class WSConstants
    {

        public enum wsOperation
        {
            GetAgencyScheme,
            GetCategorisation,
            GetCategoryScheme,
            GetCodelist,
            GetConceptScheme,
            GetContentConstraint,
            GetDataConsumerScheme,
            GetDataflow,
            GetDataProviderScheme,
            GetDataStructure,
            GetDataStructureWithRef,
            GetHierarchicalCodelist,
            GetOrganisationUnitScheme,
            GetStructureSet,
            GetStructures,
            SubmitStructure
        }

        // Template utilizzato per caricare l'xml restituito dal WS in un SDMXObjects
        public static string xmlTemplate = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                "<mes:Structure " +
                                "xmlns:mes=\"http://www.sdmx.org/resources/sdmxml/schemas/v2_1/message\">" +
                                "  <mes:Header>" +
                                "    <mes:ID>ISTATRegistryRetrieveTemplate</mes:ID> " +
                                "    <mes:Test>false</mes:Test>" +
                                "    <mes:Prepared>2014-05-06T21:53:11.874Z</mes:Prepared>" +
                                "    <mes:Sender id=\"MG\"/>" +
                                "    <mes:Receiver id=\"unknown\"/>" +
                                "  </mes:Header> " +
                                "</mes:Structure>";

        public static int MaxOutputFileLength
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["MaxOutputFileLength"]);
            }
        }

        public static string wsEndPoint
        {
            get
            {
                return ConfigurationManager.AppSettings["WSEndPoint"];
            }
        }
    
    
    }

}