<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="CategorySchemeItemDetails.aspx.cs" Inherits="ISTATRegistry.categorySchemeItemDetails" 
    EnableSessionState="True" %>
<%@ Register src="UserControls/FileDownload3.ascx" tagname="FileDownload3" tagprefix="uc1" %>
<%@ Register src="UserControls/AddText.ascx" tagname="AddText" tagprefix="uc2" %>
<%@ Register src="UserControls/UserPopUp.ascx" tagname="UserPopUp" tagprefix="uc3" %> 
<%@ Register src="~/UserControls/ControlAnnotations.ascx" tagname="ControlAnnotations" tagprefix="uc4" %>

<%@ Register src="UserControls/DuplicateArtefact.ascx" tagname="DuplicateArtefact" tagprefix="uc5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#tab-container').easytabs().bind('easytabs:before', function () {
                var txtCategorySchemeIdText = $('#<%= txtDSDID.ClientID %>').val().toString().trim();
                var txtCategorySchemeVersionText = $('#<%= txtVersion.ClientID %>').val().toString().trim();
                var addTextNameElementCount = $('#<%= pnlEditName.ClientID %> .textBlockWrap').length;

                //                if (txtCategorySchemeIdText == "" || txtCategorySchemeVersionText == "" || addTextNameElementCount == 0) {
                //                    alert("<%= Resources.Messages.missing_fields_for_category_scheme_message %>");
                //                    return true;
                //                }

            });

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

            $("#<%= TreeView1.ClientID %>").click(function () {
                window.onbeforeunload = null;
            });

            $(window).bind('beforeunload', function (e) {
                var confirmationMessage = "<%=Resources.Messages.lbl_question_exit_page %>";  // a space
                (e || window.event).returnValue = confirmationMessage;
                $.unblockUI();
                return confirmationMessage;
            });
        });

        $('#<%= txtSeparator.ClientID %>').keydown(function (event) {
            var separator = $(this).val();
            if (separator.length == 1 && event.keyCode != 8) {
                return false;
            }
        });

    </script>
    <style type="text/css">
        .etabs { margin: 0; padding: 0; }
        .tab { display: inline-block; zoom:1; *display:inline; background: #eee; border: solid 1px #999; border-bottom: none; -moz-border-radius: 4px 4px 0 0; -webkit-border-radius: 4px 4px 0 0; }
        
        .tab a { font-size: 14px; line-height: 2em; display: block; padding: 0 10px; outline: none; color: #000000; text-decoration: none}
        .tab a:hover { color: #ff0000; text-decoration: none }
        .tab.active { background: #fff; padding-top: 6px; position: relative; top: 1px; border-color: #666; color: #000000; text-decoration: none}
        .tab a.active { font-weight: bold; color: #000000; text-decoration: none}
        
        .tab-container .panel-container { background: #fff; border: solid #666 1px; padding: 10px; -moz-border-radius: 0 4px 4px 4px; -webkit-border-radius: 0 4px 4px 4px; }
        
        /*
        a:link {color: #000000; text-decoration: none}
        a:active {color: #000000; text-decoration: none}
        a:visited {color: #000000; text-decoration: none}
        a:hover {color: #ff0000; text-decoration: none}
        */
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Label ID="lblCategorySchemeDetail" runat="server" CssClass="PageTitle"><%= Resources.Messages.lbl_category_scheme %>&#32;<%= Resources.Messages.lbl_item_dettail %></asp:Label>
    <div id="divBack">
        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/CategorySchemes.aspx?m=y" ImageUrl="~/images/back.png"><%= Resources.Messages.lbl_back %></asp:HyperLink>
    </div>
                           
    <hr style="width:100%" />

    <!-- Form di CRUD codelist ------- Fabrizio Alonzi -->
    <div id="tab-container" class='tab-container'>
        <ul class='etabs'>
            <li class='tab'><a href="#general"><%= Resources.Messages.lbl_general %></a></li>
            <li class='tab'><a href="#categories"><%= Resources.Messages.lbl_categories %></a></li>
        </ul>
        <div class='panel-container'>
            <div id="general">
                <table class="tableForm">
                    <tr>
                        <td>
                            <asp:Label ID="lblDSDID" runat="server" Text="<%# '*' + Resources.Messages.lbl_id + ':' %>" CssClass="tdProperty"></asp:Label>
                        </td>
                        <td width="45%">
                            <asp:TextBox ID="txtDSDID" runat="server" Enabled="false" ValidationGroup="dsd"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lblAgency" runat="server" Text="<%# '*' + Resources.Messages.lbl_agency + ':' %>" CssClass="tdProperty"></asp:Label>
                        </td>
                        <td width="45%">
                            <asp:DropDownList ID="cmbAgencies" runat="server" Enabled="False">
                            </asp:DropDownList>
                            <asp:TextBox ID="txtAgenciesReadOnly" runat="server" Enabled="false" Visible="false"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblVersion" runat="server" Text="<%# '*' + Resources.Messages.lbl_version + ':' %>" CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtVersion" runat="server" Enabled="false" ValidationGroup="dsd"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lblIsFinal" runat="server" Text="<%# Resources.Messages.lbl_is_final + ':' %>" CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkIsFinal" runat="server" Enabled="false" />
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
                                <asp:TextBox ID="txtDSDName" runat="server" Enabled="false" TextMode="MultiLine" Rows="5" ValidationGroup="dsd"></asp:TextBox>
                            </asp:Panel>
                            <asp:Panel ID="pnlEditName" runat="server" Visible="false">
                                <uc2:AddText ID="AddTextName" runat="server" />
                            </asp:Panel>
                            <asp:TextBox ID="txtAllNames" runat="server" visible="false" Enabled= "false" />
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
                            <asp:TextBox ID="txtAllDescriptions" runat="server" visible="false" Enabled= "false" />
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
            </div>
            <div id= "categories">                       
                <asp:TreeView ID="TreeView1" runat="server" 
                    onselectednodechanged="TreeView1_SelectedNodeChanged" 
                    ontreenodecheckchanged="TreeView1_TreeNodeCheckChanged" 
                    BorderStyle="Solid" 
                    BorderWidth="1px" 
                    ForeColor="Black" 
                    ShowLines="True" 
                    Width="100%"
                    CssClass="CatTreeView"
                    ExpandDepth="1" onprerender="TreeView1_PreRender">
                    <NodeStyle NodeSpacing="2px" />
                    <SelectedNodeStyle BackColor="#CCD0D3" Font-Bold="True" />
                </asp:TreeView>

                <table width= "100%">
                    <tr>
                        <td align= "left">
                             <asp:ImageButton ID= "btnAddNewCategory" Width= "25px" Height= "25px" onClientClick= "javascript: openP('df-Dimension',600); return false;" runat="server" onclick="btnAddNewCategory_Click" ImageUrl="~/images/Plus-50.png" ToolTip= "<%# Resources.Messages.lbl_add_category %>" /> 
                            &nbsp;&nbsp;&nbsp;
                            <asp:ImageButton ID= "btnUpdateSelectedCategory" Width= "25px" Height= "25px"  Enabled = "false" onClientClick= "javascript: openP('df-Dimension-update',600); return false;" runat="server" ImageUrl="~/images/Edit-50 - disable.png" ToolTip= "<%# Resources.Messages.lbl_update_category %>"/>
                            &nbsp;&nbsp;&nbsp;
                            <asp:ImageButton OnClick="btnDeleteCategory_Click" Width= "25px" Height= "25px"  ID="btnDeleteCategory" runat="server" Enabled = "false" ImageUrl="~/images/Trash-50 - disable.png"  ToolTip= "<%# Resources.Messages.lbl_delete_category %>"  />
                            &nbsp;&nbsp;&nbsp;
                            <asp:Label ID="lblCategoryOrder" runat="server" Text="<%# Resources.Messages.lbl_category_order %>"></asp:Label>                            
                            <asp:TextBox ID="txtCategoryOrder" runat="server" Width="50px" Enabled = "false"></asp:TextBox>
                            &nbsp;&nbsp;&nbsp;
                            <asp:ImageButton OnClick="btnMoveCategory_Click" Width= "25px" Height= "25px"  ID="btnMoveCategory" runat="server" Enabled = "false" ImageUrl="~/images/Cut-50 - disable.png"  ToolTip= "<%# Resources.Messages.lbl_init_move_category %>"  />
                            &nbsp;&nbsp;&nbsp;
                            <asp:ImageButton OnClick="btnCancelMovingCategory_Click" Width= "25px" Height= "25px"  ID="btnCancelMoveCategory" Enabled = "false" runat="server" ToolTip="<%# Resources.Messages.lbl_cancel_move_category %>" ImageUrl="~/images/Cancel File-50 - disable.png"/>
                            &nbsp;&nbsp;&nbsp;
                            <asp:ImageButton OnClick="btnDeselectCategory_Click" Width= "25px" Height= "25px"  ID="btnDeselectCategory" Enabled = "false" runat="server" ToolTip="<%# Resources.Messages.lbl_deselect_category %>" ImageUrl="~/images/Cancel-50 - disable.png"/>
                            <br /><br />
                            <asp:Label ID="lblMoveInstructions" runat="server" Text="<%# Resources.Messages.txt_move_instructions_first %>"></asp:Label>
                        </td>
                        <td align= "right">
                            <asp:ImageButton ID="imgImportCsv" runat="server" ToolTip="Import CSV file" AlternateText="Import CSV file" ImageUrl="~/images/csvImport.png" onClientClick= "javascript: openP('importCsv',450); return false;"/>
                        </td>
                    </tr>
                </table>               
               <div id="importCsv" class="popup_block">
                    <asp:Label ID="lblImportCsvTitle" runat="server" Text="<%# Resources.Messages.lbl_import_csv %>" CssClass="PageTitle"></asp:Label> 
                    <asp:Label ID="lblCsvLanguage" runat="server" Text="<%# Resources.Messages.lbl_language + ':' %>"></asp:Label>
                    <asp:DropDownList ID="cmbLanguageForCsv" runat="server" AutoPostBack="False"></asp:DropDownList>
                    <asp:Label ID="lblcsvFile" runat="server" Text="<%# Resources.Messages.lbl_csv_file %>"></asp:Label>
                    <asp:FileUpload ID="csvFile" runat="server" />
                    <br /><br />
                    <asp:Label ID="lblSeparator" runat="server" Text="<%# Resources.Messages.lbl_used_separator +':' %>"></asp:Label>
                    <asp:TextBox ID="txtSeparator" Text="" runat="server" Width= "15px"/>
                    <hr />
                    <asp:Button ID="btnImportFromCsv" runat="server" Text="<%# Resources.Messages.btn_import_csv %>" onclick="btnImportFromCsv_Click" />  
                </div>
            </div> 
        </div>
    </div>

    <asp:Button ID="btnSaveMemoryCategoryScheme" runat="server" Text="<%# Resources.Messages.btn_save%>" onclick="btnSaveMemoryCategoryScheme_Click" /> 
    
    <div style="float:right"><uc1:FileDownload3 ID="FileDownload31" runat="server" ucArtefactType="CategoryScheme"/></div>

    <div id="df-Dimension-update" class="popup_block" title="Update Dimension">
        <asp:Label ID="lblTitleUpdateCategory" runat="server" Text="<%# Resources.Messages.lbl_update_category%>" CssClass="PageTitle"></asp:Label>
      
        <hr style="width: 100%;" />
      
        <table class="tableForm">
            <tr>
                <td width="20%">
                    <asp:Label ID="lblUpdateCategoryID" runat="server" Text="<%# '*' + Resources.Messages.lbl_id + ':' %>" CssClass="tdProperty"></asp:Label>
                </td>
                <td width="80%">
                    <asp:TextBox ID="txtUpdateCategoryID" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblUpdateCategoryName" runat="server" Text="<%# '*' + Resources.Messages.lbl_name + ':' %>" CssClass="tdProperty"></asp:Label>
                </td>
                <td>
                    <uc2:AddText ID="AddText1" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblUpdateCategoryDescription" runat="server" Text="<%# Resources.Messages.lbl_description + ':' %>" CssClass="tdProperty"></asp:Label>
                </td>
                <td>
                    <uc2:AddText ID="AddText2" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblUpdateAnnotation" runat="server" Text="<%# Resources.Messages.lbl_annotation+ ':' %>" CssClass="tdProperty"></asp:Label>
                </td>
                <td>
                    <asp:Panel ID="pnlAnnotationUpdateControl" runat="server" Visible="true">
                        <uc4:ControlAnnotations ID="AnnotationUpdateControl" runat="server" />
                    </asp:Panel>
                </td>
             </tr>
              <tr>
                <td colspan = "2"> 
                    <center>
                        <asp:Label ID="lblErrorOnUpdate" runat="server" Text="" ForeColor="Red"></asp:Label><br /><br />  
                        <asp:Button OnClick="btnUpdateCategory_Click" ID="btnUpdateCategory" runat="server" Text="<%# Resources.Messages.btn_update_category %>"/>
                        <asp:Button ID="btnClearFieldForUpdate" runat="server" 
                            Text="<%# Resources.Messages.btn_cancel_operation %>" 
                            onclick="btnClearFieldsForUpdate_Click"/>   
                        </center>
                </td>
            </tr>
        </table>

    </div>

    <div id="df-Dimension" class="popup_block" title="<%= Resources.Messages.lbl_add_category%>">
        <asp:Label ID="lblTitle" runat="server" Text="<%# Resources.Messages.lbl_add_category%>" CssClass="PageTitle"></asp:Label>
      
        <hr style="width: 100%;" />
      
        <table class="tableForm">
            <tr>
                <td width="20%">
                    <asp:Label ID="lblNewCategoryId" runat="server" Text="<%# '*' + Resources.Messages.lbl_id + ':' %>" CssClass="tdProperty"></asp:Label>
                </td>
                <td width="80%">
                    <asp:TextBox ID="txtNewCategoryId" runat="server" Enabled="true" Width="300px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblNewCategoryName" runat="server" Text="<%# '*' + Resources.Messages.lbl_name + ':' %>" CssClass="tdProperty"></asp:Label>
                </td>
                <td>
                    <uc2:AddText ID="NewCategoryAddTextName" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblNewCategoryDescription" runat="server" Text="<%# Resources.Messages.lbl_description + ':' %>" CssClass="tdProperty"></asp:Label>
                </td>
                <td>
                    <uc2:AddText ID="NewCategoryAddTextDescription" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblNewAnnotation" runat="server" Text="<%# Resources.Messages.lbl_annotation+ ':' %>" CssClass="tdProperty"></asp:Label>
                </td>      
                <td>
                <uc4:ControlAnnotations ID="AnnotationNewControl" runat="server" />
                    <asp:Panel ID="pnlAnnotationNewControl" runat="server" Visible="true">
                        
                    </asp:Panel>
                </td>
            </tr>

            <tr>
                <td colspan = "2">
                   <center>
                    <asp:Label ID="lblErrorOnNewInsert" runat="server" Text="" ForeColor="Red"></asp:Label><br /><br /> 
                    <asp:Button OnClick="btnAddNewCategory_Click" ID="btnNewCategory" runat="server" Text="<%# Resources.Messages.btn_add_category %>"/>
                    <asp:Button ID="btnClearFields" runat="server" Text="<%# Resources.Messages.btn_cancel_operation %>" onclick="btnClearFields_Click"/>          
                   </center>
                </td>
            </tr>
        </table>
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