using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.ConceptScheme;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Codelist;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ISTATRegistry.UserControls
{
    public delegate void OperationCompleteEventHandler(object sender, object e);

    public partial class CSVImporter : System.Web.UI.UserControl
    {
        public ICodelistMutableObject ucCodelist { get; set; }
        public IConceptSchemeMutableObject ucConceptScheme { get; set; }
        public ICategorySchemeMutableObject ucCategoryScheme { get; set; }
        public event OperationCompleteEventHandler OperationComplete;

        private SdmxStructureEnumType _structureType;

        public struct csvCode
        {
            public string code;
            public string name;
            public string description;
            public string parentCode;

            public csvCode(string code, string name, string description, string parentCode)
            {
                this.code = code;
                this.name = name;
                this.description = description;
                this.parentCode = parentCode;
            }
        }

        protected virtual void OnValueChanged(object e)
        {
            if (OperationComplete != null)
                OperationComplete(this, e);
        }

        #region EVENTS

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ucCodelist != null)
                _structureType = SdmxStructureEnumType.CodeList;
            else if (ucConceptScheme != null)
                _structureType = SdmxStructureEnumType.ConceptScheme;
            else if (ucCategoryScheme != null)
                _structureType = SdmxStructureEnumType.CategoryScheme;
            else
                return;

            if (!IsPostBack)
            {
                Utils.PopulateCmbLanguages(cmbLanguageForCsv, AVAILABLE_MODES.MODE_FOR_ADD_TEXT);
                cmbLanguageForCsv.SelectedValue = Session[SESSION_KEYS.KEY_LANG].ToString();
            }
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            // Click su preview senza che sia stato selezionato un file
            if (csvFile.Visible && !csvFile.HasFile)
            {
                Session["FileUpload1"] = null;
                OpenCsvImportPopUp();
                Utils.ShowDialog(Resources.Messages.err_no_file_uploaded);
                return;
            }

            // Prima volta che si chiede la preview
            if (csvFile.Visible && csvFile.HasFile)
            {
                Session["FileUpload1"] = csvFile;
                lblCsvFileName.Text = csvFile.FileName;
                csvFile.Visible = false;
                lblCsvFileName.Visible = true;
                ibDeleteFileSelection.Visible = true;
                SaveCsvFile();
            }
            else if (!csvFile.Visible && Session["FileUpload1"] != null)
            {
                csvFile = (FileUpload)Session["FileUpload1"];
            }

            DataTable dt = Utils.ConvertCSVtoDataTable(GetCsvFilePath(),
                                                (txtSeparator.Text != String.Empty ? Char.Parse(txtSeparator.Text) : ';'),
                                                txtTextDelimiter.Text, chkHeaderRow.Checked, 50);
            gvCsvPreview.DataSource = dt;
            gvCsvPreview.DataBind();

            OpenCsvImportPopUp();
            OpenCsvPreview();
        }

        protected void ibDeleteFileSelection_Click(object sender, ImageClickEventArgs e)
        {
            Session["FileUpload1"] = null;

            ibDeleteFileSelection.Visible = false;
            csvFile.Visible = true;
            lblCsvFileName.Visible = false;

            OpenCsvImportPopUp();
        }
        //protected void btnImportFromCsv_Click(object sender, EventArgs e)
        //{
        //}

        protected void btnImportFromCsv_Click(object sender, EventArgs e)
        {
            if (csvFile.Visible && !csvFile.HasFile)
            {
                Session["FileUpload1"] = null;
                OpenCsvImportPopUp();
                Utils.ShowDialog(Resources.Messages.err_no_file_uploaded);
                return;
            }

            if (!csvFile.Visible && Session["FileUpload1"] != null)
            {
                csvFile = (FileUpload)Session["FileUpload1"];
                //Session["FileUpload1"] = null;
                ibDeleteFileSelection.Visible = false;
                lblCsvFileName.Visible = false;
                csvFile.Visible = true;
            }

            //ICodelistMutableObject cl = GetCodeListFromSession();
            //if (cl == null) cl = GetCodelistForm();
            //else cl = GetCodelistForm(cl);



            csvCode cCode = new csvCode();
            List<csvCode> codes = new List<csvCode>();
            bool errorInUploading = false;
            bool haveDescription, haveParent;
            StreamReader reader = null;
            string wrongRowsMessage = string.Empty;
            string wrongRowsMessageForUser = string.Empty;
            string wrongFileLines = string.Empty;

            haveDescription = lbExtraFields.Items[0].Selected;
            haveParent = lbExtraFields.Items[1].Selected;

            try
            {

                if (Session["FileUpload1"] == null)
                    SaveCsvFile();

                reader = GetCsvReader();

                if (chkHeaderRow.Checked)
                    reader.ReadLine();

                int currentRow = 1;
                int extraFieldCount = lbExtraFields.GetSelectedIndices().Count();

                char separator = txtSeparator.Text.Trim().Equals(string.Empty) ? ';' : txtSeparator.Text.Trim().ElementAt(0);

                while (!reader.EndOfStream)
                {
                    string currentFileLine = reader.ReadLine();

                    string[] fields;

                    if (txtTextDelimiter.Text != String.Empty)
                        fields = Utils.CsvParser(currentFileLine, Char.Parse(txtTextDelimiter.Text), separator);
                    else
                        fields = currentFileLine.Split(separator);

                    if (fields.Length != 2 + extraFieldCount)
                    {
                        errorInUploading = true;
                        wrongRowsMessage += string.Format(Resources.Messages.err_csv_import_line_bad_format, currentRow + 1);
                        wrongRowsMessageForUser += string.Format(Resources.Messages.err_csv_import_line_bad_format_gui, currentRow + 1);
                        wrongFileLines += string.Format("{0}\n", currentFileLine);
                        currentRow++;
                        continue;
                    }
                    if (fields[0].Trim().Equals("\"\"") || fields[0].Trim().Equals(string.Empty))
                    {
                        errorInUploading = true;
                        wrongRowsMessage += string.Format(Resources.Messages.err_csv_import_id_missing, currentRow + 1);
                        wrongRowsMessageForUser += string.Format(Resources.Messages.err_csv_import_id_missing_gui, currentRow + 1);
                        wrongFileLines += string.Format("{0}\n", currentFileLine);
                        currentRow++;
                        continue;
                    }
                    if (fields[1].Trim().Equals("\"\"") || fields[1].Trim().Equals(string.Empty))
                    {
                        errorInUploading = true;
                        wrongRowsMessage += string.Format(Resources.Messages.err_csv_import_name_missing, currentRow + 1);
                        wrongRowsMessageForUser += string.Format(Resources.Messages.err_csv_import_name_missing_gui, currentRow + 1);
                        wrongFileLines += string.Format("{0}\n", currentFileLine);
                        currentRow++;
                        continue;
                    }

                    cCode.code = fields[0].ToString();
                    cCode.name = fields[1].ToString();
                    cCode.description = haveDescription ? fields[2].ToString() : "";
                    cCode.parentCode = haveParent ? fields[1 + (haveDescription ? 2 : 1)].ToString() : "";

                    codes.Add(cCode);

                    currentRow++;
                }

            }
            catch (Exception ex)
            {
                Utils.AppendScript(string.Format("Upload status: The file could not be uploaded. The following error occured: {0}", ex.Message));
            }

            foreach (csvCode code in codes)
            {
                if (!code.parentCode.Trim().Equals(string.Empty))
                {
                    int cont = 0;

                    switch (_structureType)
                    {
                        case SdmxStructureEnumType.CategoryScheme:
                            break;
                        case SdmxStructureEnumType.CodeList:
                            cont = (from myCode in ucCodelist.Items
                                    where myCode.Id.Equals(code.parentCode)
                                    select myCode).Count();
                            break;
                        case SdmxStructureEnumType.ConceptScheme:
                            break;
                    }
                    
                    if (cont == 0)
                    {
                        errorInUploading = true;
                        wrongRowsMessageForUser += string.Format(Resources.Messages.err_csv_import_parent_code_error, code.parentCode, code.code, code.code);
                        continue;
                    }
                }

                ManageCode(code);

            }

            if (errorInUploading)
            {
                lblImportCsvErrors.Text = wrongRowsMessageForUser;
                lblImportCsvWrongLines.Text = wrongFileLines;
                csvFile.Visible = true;
                Utils.AppendScript("openP('importCsvErrors',500);");
            }


            reader.Close();

            switch (_structureType)
            {
                case SdmxStructureEnumType.CategoryScheme:
                    break;
                case SdmxStructureEnumType.CodeList:
                    OnValueChanged(ucCodelist);
                    break;
                case SdmxStructureEnumType.ConceptScheme:
                    break;
            }

            

            //if (!SaveInMemory(cl)) return;

            //BindData();
            //if (!errorInUploading)
            //{
            //    Utils.ShowDialog(Resources.Messages.succ_operation);
            //}
            //Utils.AppendScript("location.href='#codes'");
        }

        private void ManageCode(csvCode code)
        {
            switch (_structureType)
            {
                case SdmxStructureEnumType.CategoryScheme:
                    break;
                case SdmxStructureEnumType.CodeList:
                    ICodeMutableObject tmpClCode = ucCodelist.GetCodeById(code.code);

                    if (tmpClCode == null)
                    {
                        tmpClCode = new CodeMutableCore();
                        tmpClCode.Id = code.code;
                        tmpClCode.ParentCode = code.parentCode;
                        tmpClCode.AddName(cmbLanguageForCsv.SelectedValue.ToString(), code.name);
                        tmpClCode.AddDescription(cmbLanguageForCsv.SelectedValue.ToString(), code.description);
                        ucCodelist.AddItem(tmpClCode);
                    }
                    else
                    {
                        tmpClCode.Id = code.code;
                        tmpClCode.ParentCode = code.parentCode;
                        tmpClCode.AddName(cmbLanguageForCsv.SelectedValue.ToString(), code.name);
                        tmpClCode.AddDescription(cmbLanguageForCsv.SelectedValue.ToString(), code.description);
                    }

                    break;
                case SdmxStructureEnumType.ConceptScheme:
                    break;
            }
        }

        #endregion 

        #region METHODS

        private string GetCsvFilePath()
        {
            return Server.MapPath("~/csv_codelists_files/") + csvFile.FileName;
        }

        private void SaveCsvFile()
        {
            try
            {
                csvFile.SaveAs(GetCsvFilePath());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private StreamReader GetCsvReader()
        {
            return new StreamReader(GetCsvFilePath());
        }

        private void OpenCsvImportPopUp()
        {
            Utils.AppendScript("location.href='#codes';");
            Utils.AppendScript("openP('importCsv',550);");
        }

        private void OpenCsvPreview()
        {
            Utils.AppendScript("openP('df-csvPreview',550);");
        }


        #endregion

    }
}