using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISTATRegistry
{
    public enum DotStatExportType
    {
        DSD,
        CODELIST,
        ALL
    }

    public enum TextType
    {
        NAME,
        DESCRIPTION
    }

    public enum Action
    {
        INSERT,
        UPDATE,
        DELETE,
        VIEW
    }

    // Strutture disponibili
    public enum AvailableStructures
    {
        CODELIST,
        CONCEPT_SCHEME,
        CATEGORY_SCHEME,
        DATAFLOW,
        DSD,
        CATEGORIZATION,
        AGENCY_SCHEME,
        DATA_PROVIDER_SCHEME,
        DATA_CONSUMER_SCHEME,
        ORGANISATION_UNIT_SCHEME,
        CONTENT_CONSTRAINT,
        STRUCTURE_SET,
        HIERARCHICAL_CODELIST
    }

    public enum AddIconType
    {
        pencil,
        cross
    }

    public enum ReleaseCalendar
    {
        DAYS,
        WEEK,
        MONTH,
        YEAR
    }

    public enum SdmxStructure
    {
        AgencyScheme = 33,
        Categorisation = 47,
        CategoryScheme = 48,
        CodeList = 41,
        ConceptScheme = 52,
        ContentConstraint = 83,
        DataConsumerScheme = 37,
        Dataflow = 57,
        DataProviderScheme = 35,
        Dsd = 54,
        HierarchicalCodelist = 43,
        OrganisationUnitScheme = 39,
        StructureSet = 84
    }

}