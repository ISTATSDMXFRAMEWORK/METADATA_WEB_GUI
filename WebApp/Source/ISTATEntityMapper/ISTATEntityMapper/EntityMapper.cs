using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISTAT.Entity;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using ISTATUtils;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Registry;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Mapping;

namespace ISTAT.EntityMapper
{
    public class EntityMapper
    {
        #region Props

        private const string _defaultLanguage = "en";
        private LocalizedUtils _localizedUtils;

        private string _language;
        public string Language { get { return _language; } }

        #endregion

        #region Constructors

        public EntityMapper()
        {
            _language = _defaultLanguage;
            _localizedUtils = new LocalizedUtils(new System.Globalization.CultureInfo(_language));
        }

        public EntityMapper(string localizedLanguage)
        {
            _language = localizedLanguage;
            _localizedUtils = new LocalizedUtils(new System.Globalization.CultureInfo(_language));
        }

        #endregion

        #region Artefact Method

        public List<CodeList> GetCodeListList(ISdmxObjects sdmxObjects)
        {
            return GetCodeListList(sdmxObjects, _language);
        }

        public List<CodeList> GetCodeListList(ISdmxObjects sdmxObjects, string localization)
        {

            if (sdmxObjects == null || sdmxObjects.Codelists == null)
                return null;

            List<CodeList> lCodeList = new List<CodeList>();

            foreach (ICodelistObject codelist in sdmxObjects.Codelists)
            {
                lCodeList.Add(new CodeList(_localizedUtils.GetNameableName(codelist), codelist.Id, codelist.AgencyId, codelist.Version, codelist.IsFinal.IsTrue));
            }

            return lCodeList;
        }

        public List<ConceptScheme> GetConceptSchemeList(ISdmxObjects sdmxObjects)
        {
            return GetConceptSchemeList(sdmxObjects, _language);
        }

        public List<ConceptScheme> GetConceptSchemeList(ISdmxObjects sdmxObjects, string localization)
        {


            if (sdmxObjects == null || sdmxObjects.ConceptSchemes == null)
                return null;

            List<ConceptScheme> lCS = new List<ConceptScheme>();

            foreach (IConceptSchemeObject cs in sdmxObjects.ConceptSchemes)
            {
                lCS.Add(new ConceptScheme(_localizedUtils.GetNameableName(cs), cs.Id, cs.AgencyId, cs.Version,cs.IsFinal.IsTrue));
            }

            return lCS;
        }

        public List<AgencyScheme> GetAgencySchemeList(ISdmxObjects sdmxObjects)
        {
            return GetAgencySchemeList(sdmxObjects, _language);
        }

        public List<AgencyScheme> GetAgencySchemeList(ISdmxObjects sdmxObjects, string localization)
        {
            if (sdmxObjects == null || sdmxObjects.AgenciesSchemes == null)
                return null;

            List<AgencyScheme> lAS = new List<AgencyScheme>();

            foreach (IAgencyScheme agency in sdmxObjects.AgenciesSchemes)
            {
                lAS.Add(new AgencyScheme(_localizedUtils.GetNameableName(agency), agency.Id, agency.AgencyId, agency.Version, agency.IsFinal.IsTrue));
            }

            return lAS;
        }

        public List<DataProviderScheme> GetDataProviderSchemeList(ISdmxObjects sdmxObjects)
        {
            return GetDataProviderSchemeList(sdmxObjects, _language);
        }

        public List<DataProviderScheme> GetDataProviderSchemeList(ISdmxObjects sdmxObjects, string localization)
        {
            if (sdmxObjects == null || sdmxObjects.AgenciesSchemes == null)
                return null;

            List<DataProviderScheme> lDPS = new List<DataProviderScheme>();

            foreach (IDataProviderScheme dataProviderScheme in sdmxObjects.DataProviderSchemes)
            {
                lDPS.Add(new DataProviderScheme(_localizedUtils.GetNameableName(dataProviderScheme), dataProviderScheme.Id, dataProviderScheme.AgencyId, dataProviderScheme.Version, dataProviderScheme.IsFinal.IsTrue));
            }

            return lDPS;
        }

        public List<OrganizationUnitScheme> GetOrganizationUnitSchemeList(ISdmxObjects sdmxObjects)
        {
            return GetOrganizationUnitSchemeList(sdmxObjects, _language);
        }

        public List<OrganizationUnitScheme> GetOrganizationUnitSchemeList(ISdmxObjects sdmxObjects, string localization)
        {
            if (sdmxObjects == null || sdmxObjects.AgenciesSchemes == null)
                return null;

            List<OrganizationUnitScheme> lOUS = new List<OrganizationUnitScheme>();

            foreach ( IOrganisationUnitSchemeObject organizationUnitScheme in sdmxObjects.OrganisationUnitSchemes)
            {
                lOUS.Add(new OrganizationUnitScheme(_localizedUtils.GetNameableName(organizationUnitScheme), organizationUnitScheme.Id, organizationUnitScheme.AgencyId, organizationUnitScheme.Version, organizationUnitScheme.IsFinal.IsTrue));
            }

            return lOUS;
        }

        public List<DataConsumerScheme> GetDataConsumerSchemeList(ISdmxObjects sdmxObjects)
        {
            return GetDataConsumerSchemeList(sdmxObjects, _language);
        }

        public List<DataConsumerScheme> GetDataConsumerSchemeList(ISdmxObjects sdmxObjects, string localization)
        {
            if (sdmxObjects == null || sdmxObjects.DataConsumerSchemes == null)
                return null;

            List<DataConsumerScheme> lDCS = new List<DataConsumerScheme>();

            foreach (IDataConsumerScheme dataConsumerScheme in sdmxObjects.DataConsumerSchemes)
            {
                lDCS.Add(new DataConsumerScheme(_localizedUtils.GetNameableName(dataConsumerScheme), dataConsumerScheme.Id, dataConsumerScheme.AgencyId, dataConsumerScheme.Version, dataConsumerScheme.IsFinal.IsTrue));
            }

            return lDCS;
        }

        public List<CategoryScheme> GetCategorySchemeList(ISdmxObjects sdmxObjects)
        {
            return GetCategorySchemeList(sdmxObjects, _language);
        }

        public List<CategoryScheme> GetCategorySchemeList(ISdmxObjects sdmxObjects, string localization)
        {


            if (sdmxObjects == null || sdmxObjects.CategorySchemes == null)
                return null;

            List<CategoryScheme> lCS = new List<CategoryScheme>();

            foreach (ICategorySchemeObject cs in sdmxObjects.CategorySchemes)
            {
                lCS.Add(new CategoryScheme(_localizedUtils.GetNameableName(cs), cs.Id, cs.AgencyId, cs.Version,cs.IsFinal.IsTrue));
            }

            return lCS;
        }

        public List<DataFlow> GetDataFlowList(ISdmxObjects sdmxObjects)
        {
            return GetDataFlowList(sdmxObjects, _language);
        }

        public List<DataFlow> GetDataFlowList(ISdmxObjects sdmxObjects, string localization)
        {

            if (sdmxObjects == null || sdmxObjects.Dataflows == null)
                return null;

            List<DataFlow> lDF = new List<DataFlow>();

            foreach (IDataflowObject df in sdmxObjects.Dataflows)
            {
                lDF.Add(new DataFlow(_localizedUtils.GetNameableName(df), df.Id, df.AgencyId, df.Version,df.IsFinal.IsTrue));
            }

            return lDF;
        }

        public List<KeyFamily> GetKeyFamilyList(ISdmxObjects sdmxObjects)
        {
            return GetKeyFamilyList(sdmxObjects, _language);
        }

        public List<KeyFamily> GetKeyFamilyList(ISdmxObjects sdmxObjects, string localization)
        {
            if (sdmxObjects == null || sdmxObjects.DataStructures == null)
                return null;

            List<KeyFamily> lKF = new List<KeyFamily>();

            foreach (IDataStructureObject kf in sdmxObjects.DataStructures)
            {
                lKF.Add(new KeyFamily(_localizedUtils.GetNameableName(kf), kf.Id, kf.AgencyId, kf.Version,kf.IsFinal.IsTrue));
            }

            return lKF;
        }

        public List<ContentConstraint> GetContentConstraintList(ISdmxObjects sdmxObjects, string localization)
        {
            if (sdmxObjects == null || sdmxObjects.ContentConstraintObjects == null)
                return null;

            List<ContentConstraint> lCCS = new List<ContentConstraint>();

            foreach (IContentConstraintObject ccs in sdmxObjects.ContentConstraintObjects)
            {
                lCCS.Add(new ContentConstraint(_localizedUtils.GetNameableName(ccs), ccs.Id, ccs.AgencyId, ccs.Version, ccs.IsFinal.IsTrue));
            }

            return lCCS;
        }

        public List<StructureSet> GetStructureSetList(ISdmxObjects sdmxObjects, string localization)
        {
            if (sdmxObjects == null || sdmxObjects.StructureSets == null)
                return null;

            List<StructureSet> lSS = new List<StructureSet>();

            foreach (IStructureSetObject ss in sdmxObjects.StructureSets)
            {
                lSS.Add(new StructureSet(_localizedUtils.GetNameableName(ss), ss.Id, ss.AgencyId, ss.Version, ss.IsFinal.IsTrue));
            }

            return lSS;
        }

        public List<HierarchicalCodelist> GetHclList(ISdmxObjects sdmxObjects, string localization)
        {
            if (sdmxObjects == null || sdmxObjects.HierarchicalCodelists == null)
                return null;

            List<HierarchicalCodelist> lSS = new List<HierarchicalCodelist>();

            foreach (IHierarchicalCodelistObject hcl in sdmxObjects.HierarchicalCodelists)
            {
                lSS.Add(new HierarchicalCodelist(_localizedUtils.GetNameableName(hcl), hcl.Id, hcl.AgencyId, hcl.Version, hcl.IsFinal.IsTrue));
            }

            return lSS;
        }

        #endregion

        #region Item Method

        public List<CodeItem> GetCodeItemList(ISdmxObjects sdmxObjects)
        {
            return GetCodeItemList(sdmxObjects, _language);
        }

        public List<CodeItem> GetCodeItemList(ISdmxObjects sdmxObjects, string localization)
        {
            List<CodeItem> lCodeListItem = new List<CodeItem>();

            foreach (ICodelistObject codelist in sdmxObjects.Codelists)
            {
                foreach (ICode code in codelist.Items)
                {
                    lCodeListItem.Add(new CodeItem(code.Id, _localizedUtils.GetNameableName(code), _localizedUtils.GetNameableDescription(code), code.ParentCode));
                }
                break;
            }
            
            return lCodeListItem;
        }

        public List<Concept> GetConceptList(ISdmxObjects sdmxObjects)
        {
            return GetConceptList(sdmxObjects, _language);
        }

        public List<Concept> GetConceptList(ISdmxObjects sdmxObjects, string localization)
        {
            List<Concept> lConcept = new List<Concept>();

            foreach (IConceptSchemeObject item in sdmxObjects.ConceptSchemes)
            {
                foreach (IConceptObject code in item.Items)
                {
                    lConcept.Add(new Concept(code.Id, _localizedUtils.GetNameableName(code), _localizedUtils.GetNameableDescription(code), code.ParentConcept));
                }
                break;
            }
            return lConcept;
        }

        public List<Category> GetCategoryList(ISdmxObjects sdmxObjects)
        {
            return GetCategoryList(sdmxObjects, _language);
        }

        public List<Category> GetCategoryList(ISdmxObjects sdmxObjects, string localization)
        {
            List<Category> lCategory = new List<Category>();

            foreach (ICategorySchemeObject categoryScheme in sdmxObjects.CategorySchemes)
            {
                foreach (ICategoryObject category in categoryScheme.Items)
                {
                    lCategory.Add(new Category(category.Id, _localizedUtils.GetNameableName(category), _localizedUtils.GetNameableDescription(category), ""));
                    GetCategoryParent(ref lCategory, category);
                }
                break;
            }
            return lCategory;
        }


        // CATEGORIZATION

        public List<Categorization> GetCategorizationList(ISdmxObjects sdmxObjects)
        {
            return GetCategorizationList(sdmxObjects, _language);
        }

        public List<Categorization> GetCategorizationList(ISdmxObjects sdmxObjects, string localization)
        {
            List<Categorization> lCategorization = new List<Categorization>();
          
            if ( sdmxObjects == null ) 
            {
                return null;
            }

            foreach (ICategorisationObject categorization in sdmxObjects.Categorisations)
            {
                lCategorization.Add(new Categorization(_localizedUtils.GetNameableName(categorization), categorization.Id, categorization.AgencyId, categorization.Version,categorization.IsFinal.IsTrue, string.Format( "{0} - {1} - {2}", categorization.CategoryReference.MaintainableId, categorization.CategoryReference.AgencyId, categorization.CategoryReference.Version ), categorization.CategoryReference.IdentifiableIds[0].ToString(),  string.Format( "{0} - {1} - {2}", categorization.StructureReference.MaintainableId, categorization.StructureReference.AgencyId, categorization.StructureReference.Version ) ));
            }
            return lCategorization;
        }

        public List<DataFlowItem> GetDataFlowItemList(ISdmxObjects catObjects, ISdmxObjects dataFlowObjects)
        {
            return GetDataFlowItemList(catObjects, dataFlowObjects, _language);
        }

        public List<DataFlowItem> GetDataFlowItemList(ISdmxObjects catObjects, ISdmxObjects dataFlowObjects, string localization)
        {
            List<DataFlowItem> lDataFlow = new List<DataFlowItem>();

            foreach (IDataflowObject dataFlow in dataFlowObjects.Dataflows)
            {
                lDataFlow.Add(new DataFlowItem("DataflowID", dataFlow.Id));
                lDataFlow.Add(new DataFlowItem("AgencyID", dataFlow.AgencyId));
                lDataFlow.Add(new DataFlowItem("Version", dataFlow.Version));
                lDataFlow.Add(new DataFlowItem("Name", _localizedUtils.GetNameableName(dataFlow)));
                lDataFlow.Add(new DataFlowItem("KeyFamilyID", dataFlow.DataStructureRef.MaintainableId));
                lDataFlow.Add(new DataFlowItem("KeyFamilyAgencyID", dataFlow.DataStructureRef.AgencyId));
                lDataFlow.Add(new DataFlowItem("KeyFamilyVersion", dataFlow.DataStructureRef.Version));

                if (catObjects != null)
                {
                    foreach (ICategorisationObject cat in catObjects.Categorisations)
                    {
                        if (cat.StructureReference.MaintainableId == dataFlow.Id &&
                            cat.StructureReference.AgencyId == dataFlow.AgencyId &&
                            cat.StructureReference.Version == dataFlow.Version)
                        {
                            lDataFlow.Add(new DataFlowItem("CategorySchemeID", cat.CategoryReference.MaintainableId));
                            lDataFlow.Add(new DataFlowItem("CategorySchemeAgencyID", cat.CategoryReference.AgencyId));
                            lDataFlow.Add(new DataFlowItem("CategorySchemeVersion", cat.CategoryReference.Version));
                            lDataFlow.Add(new DataFlowItem("CategoryID", cat.CategoryReference.FullId));
                        }
                    }
                }
                break;
            }

            return lDataFlow;
        }


        public List<Dimension> GetDimensionList(IDataStructureObject dimObjects)
        {
            return GetDimensionList(dimObjects, _language);
        }

        private List<Dimension> GetDimensionList(IDataStructureObject dimObjects, string localization)
        {
            List<Dimension> lDimension = new List<Dimension>();

            if (dimObjects.DimensionList == null)
                return null;

            foreach (IDimension dimension in dimObjects.DimensionList.Dimensions)
            {
                string TextFormat = String.Empty;
                string CodeList = String.Empty;

                if (dimension.Representation != null && dimension.Representation.Representation != null)
                {
                    CodeList = dimension.Representation.Representation.MaintainableId;

                    if (dimension.Representation.TextFormat != null)
                        TextFormat = dimension.Representation.TextFormat.TextType.EnumType.ToString();
                }

                lDimension.Add(new Dimension(dimension.Id, 
                    dimension.ConceptRef.MaintainableId + "," + dimension.ConceptRef.AgencyId + "," + dimension.ConceptRef.Version +" - "+ dimension.ConceptRef.FullId,
                    CodeList, TextFormat, GetDimensionRole(dimension)));
            }

            return lDimension;
        }



        public List<Entity.Attribute> GetAttributeList(IDataStructureObject attObject)
        {
            return GetAttributeList(attObject, _language);
        }

        public List<Entity.Attribute> GetAttributeList(IDataStructureObject attObject, string localization)
        {
            List<Entity.Attribute> lAttribute = new List<Entity.Attribute>();

            if (attObject.AttributeList == null)
                return null;

            foreach (IAttributeObject attribute in attObject.AttributeList.Attributes)
            {
                string TextFormat = String.Empty;
                string CodeList = String.Empty;

                if (attribute.Representation != null && attribute.Representation.Representation != null)
                {
                    ICrossReference rep = attribute.Representation.Representation;
                    CodeList = rep.MaintainableId + ","+ rep.AgencyId +","+ rep.Version;

                    if (attribute.Representation.TextFormat != null)
                        TextFormat = attribute.Representation.TextFormat.TextType.EnumType.ToString();
                }

                lAttribute.Add(new Entity.Attribute(attribute.Id, attribute.ConceptRef.MaintainableId + "," + attribute.ConceptRef.AgencyId + "," + attribute.ConceptRef.Version + " - " + attribute.ConceptRef.FullId, CodeList, TextFormat, attribute.AssignmentStatus, attribute.AttachmentLevel.ToString()));
            }

            return lAttribute;
        }


        public List<Group> GetGroupList(IDataStructureObject groupObject)
        {
            List<Entity.Group> lGroup = new List<Entity.Group>();

            if (groupObject.Groups != null && groupObject.Groups.Count <= 0)
                return null;

            foreach (IGroup group in groupObject.Groups)
            {
                string DimensionList = String.Empty;

                if (group.DimensionRefs != null)
                {
                    foreach(string dim in group.DimensionRefs)
                        DimensionList += dim +",";
                }

                if(DimensionList != String.Empty)
                    DimensionList = DimensionList.Substring(0,DimensionList.Length -1);

                lGroup.Add(new Entity.Group(group.Id, DimensionList));
            }

            return lGroup;
        }


        public List<CodeListMap> GetCodeListMapList(IStructureSetObject sso)
        {
            List<Entity.CodeListMap> lCLM = new List<Entity.CodeListMap>();

            if ((sso.CodelistMapList != null && sso.CodelistMapList.Count <= 0) || sso.CodelistMapList == null)
                return null;

            string clSourceID,clTargetID;

            foreach (ICodelistMapObject clm in sso.CodelistMapList)
            {
                clSourceID = String.Empty;
                clTargetID = String.Empty;

                if (clm.SourceRef != null)
                    clSourceID = clm.SourceRef.MaintainableId + ", " + clm.SourceRef.AgencyId + ", " + clm.SourceRef.Version;

                if (clm.TargetRef != null)
                    clTargetID = clm.TargetRef.MaintainableId + ", " + clm.TargetRef.AgencyId + ", " + clm.TargetRef.Version;


                lCLM.Add(new CodeListMap(clm.Id, clSourceID, clTargetID));
            }

            return lCLM;
        }

        public List<StructureMap> GetStructureMapList(IStructureSetObject sso)
        {
            List<Entity.StructureMap> lSM = new List<Entity.StructureMap>();

            if ((sso.StructureMapList != null && sso.StructureMapList.Count <= 0) || sso.StructureMapList == null)
                return null;

            string SourceID, TargetID, ArtefactType;

            foreach (IStructureMapObject sm in sso.StructureMapList)
            {
                SourceID = String.Empty;
                TargetID = String.Empty;
                ArtefactType = String.Empty;

                if (sm.SourceRef != null)
                    SourceID = sm.SourceRef.MaintainableId + ", " + sm.SourceRef.AgencyId + ", " + sm.SourceRef.Version;

                if (sm.TargetRef != null)
                    TargetID = sm.TargetRef.MaintainableId + ", " + sm.TargetRef.AgencyId + ", " + sm.TargetRef.Version;

                ArtefactType = sm.SourceRef.TargetReference.StructureType;

                lSM.Add(new StructureMap(sm.Id, SourceID, TargetID, ArtefactType));
            }

            return lSM;
        }




        #endregion

        #region Private Methods


        private void GetCategoryParent(ref List<Category> lCategory, ICategoryObject categoryObject)
        {
            foreach (ICategoryObject category in categoryObject.Items)
            {
                lCategory.Add(new Category(category.Id, category.Name, category.Description, category.IdentifiableParent.Id));
                GetCategoryParent(ref lCategory, category);
            }
        }

        private string GetDimensionRole(IDimension dimension)
        {
            string ret = "Dimension";

            if (dimension.TimeDimension)
                ret = "TimeDimension";

            if (dimension.FrequencyDimension)
                ret = "FrequencyDimension";

            if (dimension.MeasureDimension)
                ret = "MeasureDimension";

            return ret;
        }


        #endregion



    }
}
