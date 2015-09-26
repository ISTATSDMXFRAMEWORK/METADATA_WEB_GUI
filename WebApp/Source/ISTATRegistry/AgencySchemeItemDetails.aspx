<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="AgencySchemeItemDetails.aspx.cs" Inherits="ISTATRegistry.agencyschemeItemDetails" 
    EnableSessionState="True" %>
<%@ Register src="UserControls/FileDownload3.ascx" tagname="FileDownload3" tagprefix="uc1" %>
<%@ Register src="UserControls/AddText.ascx" tagname="AddText" tagprefix="uc2" %>
<%@ Register src="UserControls/UserPopUp.ascx" tagname="UserPopUp" tagprefix="uc3" %>
<%@ Register src="UserControls/ControlAnnotations.ascx" tagname="ControlAnnotations" tagprefix="uc4" %>
<%@ Register src="UserControls/DuplicateArtefact.ascx" tagname="DuplicateArtefact" tagprefix="uc5" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#tab-container').easytabs();

            $(".datepicker").datepicker({
                changeMonth: true,
                changeYear: true,
                dateFormat: 'dd/mm/yy'
            });
            $(".datepicker").datepicker("option", "showAnim", "drop");

            jQuery(function ($) {
                var form = $('form'), oldSubmit = form[0].onsubmit;
                form[0].onsubmit = null;

                if(!<%=AspConfirmationExit%>)
                    window.onbeforeunload = null;

                $('form').submit(function () {
                    // reset the onbeforeunload
                    window.onbeforeunload = null;

                    // run what actually was on
                    if (oldSubmit)
                        oldSubmit.call(this);
                });
            });

            $(window).bind('beforeunload', function (e) {
                var confirmationMessage = "<%=Resources.Messages.lbl_question_exit_page %>";  // a space
                (e || window.event).returnValue = confirmationMessage;
                $.unblockUI();
                return confirmationMessage;
            });

            $("#<%= gvAgencyschemesItem.ClientID %> .pgr a").click(function (e) {
                window.onbeforeunload = null;
            });

            $('#<%= txtSeparator.ClientID %>').keydown(function (event) {
                var separator = $(this).val();
                if (separator.length == 1 && event.keyCode != 8) {
                    return false;
                }
            });
        });
    </script>
    <style>
        .etabs { margin: 0; padding: 0; }
        .tab { display: inline-block; zoom:1; *display:inline; background: #eee; border: solid 1px #999; border-bottom: none; -moz-border-radius: 4px 4px 0 0; -webkit-border-radius: 4px 4px 0 0; }
        
        .tab a { font-size: 14px; line-height: 2em; display: block; padding: 0 10px; outline: none; color: #000000; text-decoration: none}
        .tab a:hover { color: #ff0000; text-decoration: none }
        .tab.active { background: #fff; padding-top: 6px; position: relative; top: 1px; border-color: #666; color: #000000; text-decoration: none}
        .tab a.active { font-weight: bold; color: #000000; text-decoration: none}
        
        .tab-container .panel-container { background: #fff; border: solid #666 1px; padding: 10px; -moz-border-radius: 0 4px 4px 4px; -webkit-border-radius: 0 4px 4px 4px; }
        
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Label ID="lblAgencySchemeDetail" runat="server" CssClass="PageTitle"><%= Resources.Messages.lbl_agency_scheme %>&#32;<%= Resources.Messages.lbl_item_dettail %></asp:Label>
    <div id="divBack">
        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/agencyschemes.aspx?m=y" ImageUrl="~/images/back.png"><%= Resources.Messages.lbl_back %></asp:HyperLink></div>
                           
    <hr style="width:100%" />

    <!-- Form di CRUD codelist ------- Fabrizio Alonzi -->
    <div id="tab-container" class='tab-container'>
                <ul class='etabs'>
                    <li class='tab'><a href="#general"><%= Resources.Messages.lbl_general %></a></li>
                    <li class='tab'><a href="#agencies"><%= Resources.Messages.lbl_agencies %></a></li>
                </ul>
                <div class='panel-container'>
                    <div id="general">
                        <table class="tableForm">
                            <tr>
                                <td>
                                    <asp:Label ID="lblDSDID" runat="server" Text="<%# '*' + Resources.Messages.lbl_id + ':' %>" CssClass="tdProperty"></asp:Label>
                                </td>
                                <td width="45%">
                                    <asp:TextBox ID="txtDSDID" runat="server" Enabled="false" ValidationGroup="dsd" Text="AGENCIES"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDSDID"
                                        ErrorMessage="The DSD ID is Mandatory" ValidationGroup="dsd">*</asp:RequiredFieldValidator>
                                </td>
                                <td>
                                    <asp:Label ID="lblAgency" runat="server" Text="<%# '*' + Resources.Messages.lbl_agency + ':' %>" CssClass="tdProperty"></asp:Label>
                                </td>
                                <td width="45%">
                                    <asp:DropDownList ID="cmbAgencies" runat="server" Enabled="False" Visible="true">
                                    </asp:DropDownList>
                                    <asp:TextBox ID="txtAgenciesReadOnly" runat="server" Enabled="false" Visible="false"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblVersion" runat="server" Text="<%# '*' + Resources.Messages.lbl_version + ':' %>" CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtVersion" runat="server" Enabled="false" ValidationGroup="dsd" Text="1.0"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtVersion"
                                        ErrorMessage="The DSD Version  is Mandatory" ValidationGroup="dsd">*</asp:RequiredFieldValidator>
                                </td>
                                <td>
                                    <asp:Label ID="lblIsFinal" runat="server" Text="<%# Resources.Messages.lbl_is_final + ':' %>" CssClass="tdProperty" Enabled="false"></asp:Label>
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkIsFinal" runat="server" Enabled="false"/>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblDSDURI" runat="server" Text="<%# Resources.Messages.lbl_uri + ':' %>" CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtDSDURI" runat="server" Enabled="false"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Label ID="lblDSDURN" runat="server" Text="<%# Resources.Messages.lbl_urn + ':' %>" CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtDSDURN" runat="server" Enabled="false"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblValidFrom" runat="server" Text="<%# Resources.Messages.lbl_valid_from + ':' %>" CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtValidFrom" runat="server" Enabled="false" ValidationGroup="dsd" CssClass="datepicker"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Label ID="lblValidTo" runat="server" Text="<%# Resources.Messages.lbl_valid_to + ':' %>" CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtValidTo" runat="server" Enabled="false" ValidationGroup="dsd" CssClass="datepicker"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblDSDName" runat="server" Text="<%# '*' + Resources.Messages.lbl_name + ':' %>" CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:Panel ID="pnlViewName" runat="server" Visible="false">
                                        <asp:TextBox ID="txtDSDName" runat="server" Enabled="false" TextMode="MultiLine"
                                            Rows="5" ValidationGroup="dsd"></asp:TextBox>
<%--                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtDSDName"
                                            ErrorMessage="The DSD Name is Mandatory" ValidationGroup="dsd">*</asp:RequiredFieldValidator>
--%>                                    </asp:Panel>
                                    <asp:Panel ID="pnlEditName" runat="server" Visible="false">
                                        <uc2:AddText ID="AddTextName" runat="server" />
                                    </asp:Panel>
                                    <asp:TextBox ID="txtAllNames" runat="server" visible="false" enabled= "false" />
                                </td>
                                <td>
                                    <asp:Label ID="lblDSDDescription" runat="server" Text="<%# Resources.Messages.lbl_description + ':' %>" CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:Panel ID="pnlViewDescription" runat="server" Visible="false">
                                        <asp:TextBox ID="txtDSDDescription" runat="server" Enabled="false" TextMode="MultiLine"
                                            Rows="5"></asp:TextBox>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlEditDescription" runat="server" Visible="false">
                                        <uc2:AddText ID="AddTextDescription" runat="server" />
                                    </asp:Panel>
                                    <asp:TextBox ID="txtAllDescriptions" runat="server" visible="false" enabled= "false" />
                                </td>
                            </tr>
                             <tr>              
                                <td>
                                    <asp:Label ID="lbl_annotation" runat="server" Text="<%# Resources.Messages.lbl_annotation+ ':' %>" CssClass="tdProperty"></asp:Label>
                                </td>
                                <td colspan="3">
                                    <asp:Panel ID="pnlAnnotation" runat="server" Visible="true">
                                        <uc4:ControlAnnotations ID="AnnotationGeneralControl" runat="server" />
                                    </asp:Panel>
                                </td>
                            </tr>

                        </table>
                        <br />
                    </div>
             <div id= "agencies">
             <asp:Label ID="lblNoItemsPresent" runat="server" Text="<%# Resources.Messages.lbl_no_items_in_grid %>"></asp:Label>
             <br />
             <asp:Label ID="lblNumberOfTotalElements" runat="server" Text=""></asp:Label>
                     <asp:GridView ID="gvAgencyschemesItem" runat="server" width="730px"
            AllowPaging="True" 
            CssClass="Grid"
            onpageindexchanging="gvAgencyschemesItem_PageIndexChanging" 
            OnRowCommand = "gvAgencyschemesItem_RowCommand"
            AutoGenerateColumns="False" onrowupdating="gvAgencyschemesItem_RowUpdating" 
                     onrowdeleting="gvAgencyschemesItem_RowDeleting" 
                     onrowdatabound="gvAgencyschemesItem_RowDataBound">
        <Columns>
            <asp:TemplateField HeaderText="No." ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="30px">
                 <ItemTemplate>
                    <%# Container.DataItemIndex + 1 %>
                 </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="ID" SortExpression="ID">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("code") %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblID" runat="server" Text='<%# Bind("code") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Width="200px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Name" SortExpression="Name">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("name") %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblName" runat="server" Text='<%# Bind("name") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Width="400px" />
            </asp:TemplateField>
            <%-- <asp:CommandField EditText="Edit" HeaderText="View/Edit" ShowEditButton="True" Visible="False" /> --%>
             <asp:TemplateField HeaderText="" ShowHeader="False">
             <ItemTemplate>
               <asp:ImageButton ID="ImageButton2" runat="server" CausesValidation="False" CommandName="UPDATE"
                                CommandArgument="<%# Container.DataItemIndex %>" ImageUrl="~/images/Details2.png"
                                ToolTip="UPDATE"/>
             </ItemTemplate>
             <HeaderStyle Width="50px" HorizontalAlign="Center" />
             <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="" ShowHeader="False">
                        <ItemTemplate>
                          <asp:ImageButton ID="ImageButton3" runat="server" CausesValidation="False" CommandName="DELETE"
                                CommandArgument="<%# Container.DataItemIndex %>" ImageUrl="~/images/Delete2.png"
                                ToolTip="DELETE"/> 
                        </ItemTemplate>
                        <HeaderStyle Width="50px" HorizontalAlign="Center" CssClass="HiddenCol" />
                        <ItemStyle HorizontalAlign="Center" CssClass="HiddenCol" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="" ShowHeader="False">
                            <ItemTemplate> 
                                <asp:Label ID="lblNumberOfAnnotation" runat="server" Text="Label"></asp:Label>          
                                <asp:ImageButton 
                                    ID="img_annotation" 
                                    runat="server" 
                                    CausesValidation="False" 
                                    CommandName="ANNOTATION"
                                    CommandArgument="<%# Container.DataItemIndex %>" 
                                    ImageUrl="~/images/Annotation.png"
                                    ToolTip="ANNOTATION"/> 
                            </ItemTemplate>
                            <HeaderStyle Width="50px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
        </Columns>
        <HeaderStyle CssClass="hs" />
        <RowStyle CssClass="rs" />
        <AlternatingRowStyle CssClass="ars" />
        <PagerStyle CssClass="pgr"></PagerStyle>
    </asp:GridView>
    <!-- <input type= "button" id="newCode" onclick='javascript: ;' value="Add code" />  -->
    <asp:Label ID="lblNumberOfRows" runat="server" Text="<%# Resources.Messages.lbl_number_of_rows + ':'%>"></asp:Label>
        <asp:TextBox ID="txtNumberOfRows" runat="server" style="text-align:center" onkeydown = "return (event.keyCode!=13);"
        ontextchanged="txtNumberOfRows_TextChanged" Width="40px"></asp:TextBox>&nbsp;<asp:Button ID="btnChangePaging" runat="server" 
                Text="<%# Resources.Messages.lbl_change_number_of_rows%>" onclick="btnChangePaging_Click" />
    <br /><br />
    <table width= "100%">
        <tr>
            <td align= "left">
                <asp:Button ID= "btnAddNewAgency" text="<%# Resources.Messages.lbl_add_agency%>" onClientClick= "javascript: openP('df-AddAgency',600); return false;" runat="server" onclick="btnAddNewAgency_Click" /> 
            </td>
            <td align= "right">
                <asp:ImageButton ID="imgImportCsv" runat="server" ToolTip="<%# Resources.Messages.btn_import_csv_file %>" AlternateText="<%# Resources.Messages.btn_import_csv_file %>" ImageUrl="~/images/csvImport.png" onClientClick= "javascript: openP('importCsv',450); return false;"/>
            </td>
        </tr>
    </table>
              
    <div id="importCsv" class="popup_block">
        <asp:Label ID="lblImportCsvTitle" runat="server" Text="<%# Resources.Messages.lbl_import_csv %>" CssClass="PageTitle"></asp:Label> 
        <br /><br />
        <asp:Label ID="lblCsvLanguage" runat="server" Text="<%# Resources.Messages.lbl_language + ':' %>"></asp:Label>
        <asp:DropDownList ID="cmbLanguageForCsv" runat="server" AutoPostBack="False"></asp:DropDownList>
        <br /><br />
        <asp:Label ID="lblcsvFile" runat="server" Text="<%# Resources.Messages.lbl_csv_file + ':'%>"></asp:Label>
        <asp:FileUpload ID="csvFile" runat="server" />
        <br /><br />
        <asp:Label ID="lblSeparator" runat="server" Text="<%# Resources.Messages.lbl_used_separator +':' %>"></asp:Label>
        <asp:TextBox ID="txtSeparator" Text="" runat="server" Width= "15px"/>
        <hr />
        <center>
        <asp:Button ID="btnImportFromCsv" runat="server" Text="<%# Resources.Messages.btn_import_csv %>" onclick="btnImportFromCsv_Click" />  
        </center>
    </div>

    <div id="df-AddAgency" class="popup_block">
        
        <asp:Label ID="lbl_title_new" runat="server" Text="<%# Resources.Messages.lbl_add_agency%>" CssClass="PageTitle"></asp:Label>

        <hr style="width: 100%;" />

        <table class="tableForm">
            <tr>
                <td width="20%">
                    <asp:Label ID="lbl_id_new" runat="server" Text="<%# '*' + Resources.Messages.lbl_id + ':' %>" CssClass="tdProperty"></asp:Label>
                </td>
                <td width="80%">
                    <asp:TextBox ID="txt_id_new" runat="server" Enabled="true" ></asp:TextBox>
                </td>
            </tr>
       <tr>
        <td>
         <asp:Label ID="lbl_name_new" runat="server" Text="<%# '*' + Resources.Messages.lbl_name + ':' %>" CssClass="tdProperty"></asp:Label>
        </td>
        <td>
         <uc2:AddText ID="AddTextName_new" runat="server" />
        </td>
       </tr>
       <tr>
        <td>
         <asp:Label ID="lbl_description_new" runat="server" Text="<%# Resources.Messages.lbl_description + ':' %>" CssClass="tdProperty"></asp:Label>
        </td>
        <td>
         <uc2:AddText ID="AddTextDescription_new" runat="server" />
        </td>
       </tr>
       <tr>
                <td colspan = "2">
                    <center>
                       <asp:Label ID="lblErrorOnNewInsert" runat="server" Text="" ForeColor="Red"></asp:Label><br /><br />  
                       <asp:Button OnClick="btnAddNewAgency_Click" ID="btnNewAgency" runat="server" Text="<%# Resources.Messages.btn_add_agency %>"/>
                       <asp:Button ID="btnClearFields" runat="server" Text="<%# Resources.Messages.btn_cancel_operation %>" onclick="btnClearFields_Click"/>          
                    </center>
                </td>
            </tr>
        </table>
    </div>

    </div> 
    </div>
    </div>  

    <asp:Button ID="btnSaveMemoryAgencyScheme" runat="server" Text="<%# Resources.Messages.btn_save%>" onclick="btnSaveMemoryAgencyScheme_Click" /> 
    
    <div style="float:right"><uc1:FileDownload3 ID="FileDownload31" runat="server"/></div>

    <div id="df-Agency-update" class="popup_block">
        <asp:Label ID="lbl_title_update" runat="server" Text="<%# Resources.Messages.lbl_update_agency%>" CssClass="PageTitle"></asp:Label>

        <hr style="width: 100%;" />
      
        <table class="tableForm">
        <tr>
        <td>
            <asp:Label ID="lbl_id_update" runat="server" Text="<%# '*' + Resources.Messages.lbl_id + ':' %>" CssClass="tdProperty"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="txt_id_update" runat="server" Enabled="false"></asp:TextBox>
        </td>
        </tr>
        <tr>
        <td>
            <asp:Label ID="lbl_name_update" runat="server" Text="<%# '*' + Resources.Messages.lbl_name + ':' %>" CssClass="tdProperty"></asp:Label>
        </td>
        <td>
            <uc2:AddText ID="AddTextName_Update" runat="server" />
        </td>
        </tr>
        <tr>
        <td>
            <asp:Label ID="lbl_description_update" runat="server" Text="<%# Resources.Messages.lbl_description + ':' %>" CssClass="tdProperty"></asp:Label>
        </td>
        <td>
            <uc2:AddText ID="AddTextDescription_Update" runat="server" />
        </td>
        </tr>
        <tr>
            <td colspan = "2">
                <center>
                    <asp:Label ID="lblErrorOnUpdate" runat="server" Text="" ForeColor="Red"></asp:Label><br /><br />   
                    <asp:Button OnClick="btnUpdateAgency_Click" ID="btnUpdateAgency" runat="server" Text="<%# Resources.Messages.btn_update_agency %>"/>
                    <asp:Button ID="btnClearFieldForUpdate" runat="server" 
                        Text="<%# Resources.Messages.btn_cancel_operation %>" 
                        onclick="btnClearFieldsForUpdate_Click"/>                            
                    <uc3:UserPopUp ID="UserPopUp2" runat="server" />
                </center>
            </td>
        </tr>
      
        </table> 

    </div>

    <div id="agency_annotation" class="popup_block">
    <asp:Label ID="lblAnnotationAttribute" runat="server" Text="Annotation Manager" CssClass="PageTitle"></asp:Label>
        <hr style="width: 95%;" />
        <table class="tableForm">
            <tr>
                <td>
                    <uc4:ControlAnnotations ID="ctr_annotation_update" runat="server" PopUpContainer="agency_annotation" />
                </td>
            </tr>
        </table>       
        <asp:Button ID="btnSaveAnnotationCode" runat="server" Text="<%# Resources.Messages.btn_save %>" onclick="btnSaveAnnotationAgency_Click" /> 
    </div>

     <div id= "importCsvErrors" class="popup_block" >
        <asp:Label ID="lblImportCsvErrorsTitle" runat="server" Text="IMPORT ERRORS" CssClass="PageTitle"></asp:Label>
        <hr style="width: 95%;" />
        <center>  
            <div style="height: 100px; overflow: auto">
                <br />
                <asp:Label ID="lblImportCsvErrors" runat="server" Text=""></asp:Label>                
            </div>
        </center>
            <span style="text-align:left"><%= Resources.Messages.lbl_wrong_lines %></span>
            <div style="height: 100px; width: 100%;">                
                <asp:TextBox ID="lblImportCsvWrongLines" runat="server" Text="" TextMode="MultiLine" CssClass="noScalableTextArea" Width= "100%" Height= "80%"></asp:TextBox>
            </div>     
    </div>

    <uc3:UserPopUp ID="UserPopUp1" runat="server" />
    <uc5:DuplicateArtefact ID="DuplicateArtefact1" runat="server" visible="false"/>
</asp:Content>
