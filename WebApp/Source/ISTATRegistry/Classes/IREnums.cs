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
        KEY_FAMILY,
        CATEGORIZATION,
        AGENCY_SCHEME,
        DATA_PROVIDER_SCHEME,
        DATA_CONSUMER_SCHEME,
        ORGANISATION_UNIT_SCHEME,
        CONTENT_CONSTRAINT,
        STRUCTURE_SET
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

}