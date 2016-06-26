<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="StructureSetItemDetails.aspx.cs" Inherits="ISTATRegistry.StructureSetItemDetails"
    EnableSessionState="True" MaintainScrollPositionOnPostback="true" %>

<%@ Register Src="UserControls/FileDownload3.ascx" TagName="FileDownload3" TagPrefix="uc1" %>
<%@ Register Src="UserControls/AddText.ascx" TagName="AddText" TagPrefix="uc2" %>
<%@ Register Src="UserControls/UserPopUp.ascx" TagName="UserPopUp" TagPrefix="uc3" %>
<%@ Register Src="~/UserControls/ControlAnnotations.ascx" TagName="ControlAnnotations"
    TagPrefix="uc4" %>
<%@ Register Src="UserControls/SearchBar.ascx" TagName="SearchBar" TagPrefix="uc5" %>
<%@ Register Src="UserControls/GetDSD.ascx" TagName="GetDSD" TagPrefix="uc6" %>
<%@ Register Src="UserControls/DuplicateArtefact.ascx" TagName="DuplicateArtefact"
    TagPrefix="uc7" %>
<%@ Register Src="UserControls/GetCodeList.ascx" TagName="GetCodeList" TagPrefix="uc8" %>
<%@ Register Namespace= "ISTATRegistry.Classes" TagPrefix="iup" Assembly="IstatRegistry" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%@ register src="UserControls/GetDataFlow.ascx" tagname="GetDataFlow" tagprefix="uc10" %>
    <script type="text/javascript">

        var tabindex = 0;

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

            // ---- Accordion PostBack -----
            BindControlEvents();
            //Re-bind for callbacks
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(function () {
                BindControlEvents();
            });

            BindControlEvents2();
            //Re-bind for callbacks
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(function () {
                BindControlEvents2();
            });

            function BindControlEvents() {
                $("#clmGeneral").accordion({
                    autoHeight: false,
                    header: "h3",

                    active: tabindex,
                    activate: function (event, ui) {
                        var active = jQuery("#clmGeneral").accordion('option', 'active');
                        tabindex = active;
                    }
                });

                $("#<%= btnSaveCLM.ClientID %>").click(function (e) {
                    $("#clmGeneral").accordion('option', 'active', 1);
                    e.preventDefault();
                });                             

            }

            function BindControlEvents2() {
                $("#smGeneral").accordion({
                    autoHeight: false,
                    header: "h3",
                    active: tabindex,
                    activate: function (event, ui) {
                        var active = jQuery("#smGeneral").accordion('option', 'active');
                        tabindex = active;
                    }
                });

                $("#<%= btnSaveSM.ClientID %>").click(function (e) {
                    $("#smGeneral").accordion('option', 'active', 1);
                    e.preventDefault();
                });                
            }
            // -----------------------------
        });

        $(function () {
            $("#clmGeneral").accordion({
                autoHeight: false,
                header: "h3",
                active: tabindex,
                activate: function (event, ui) {
                    var active = jQuery("#clmGeneral").accordion('option', 'active');
                    tabindex = active;
                }
            });
        });

        $(function () {
            $("#smGeneral").accordion({
                autoHeight: false,
                header: "h3",
                active: tabindex,
                activate: function (event, ui) {
                    var active = jQuery("#smGeneral").accordion('option', 'active');
                    tabindex = active;
                }
            });
        });

        function ConfirmDeleteCLM() {
            return confirm("<%=Resources.Messages.lbl_question_delete_clm %>");
        }

        function ConfirmDeleteSM() {
            return confirm("<%=Resources.Messages.lbl_question_delete_sm %>");
        }

        function GetListBoxScrollPosition() {
            var sel = document.getElementById('<%=lbCLMSourceCode.ClientID %>');
            var hdnScrollTop = document.getElementById('<%=hdnCLMSourceCodeScrollTop.ClientID %>');
            hdnScrollTop.value = sel.scrollTop;

            var sel2 = document.getElementById('<%=lbCLMTargetCode.ClientID %>');
            var hdnScrollTop2 = document.getElementById('<%=hdnCLMTargetCodeScrollTop.ClientID %>');
            hdnScrollTop2.value = sel2.scrollTop;

        }

        function SetListBoxScrollPosition() {
            var sel = document.getElementById('<%=lbCLMSourceCode.ClientID %>');
            var hdnScrollTop = document.getElementById('<%=hdnCLMSourceCodeScrollTop.ClientID %>');
            sel.scrollTop = hdnScrollTop.value;

            var sel2 = document.getElementById('<%=lbCLMTargetCode.ClientID %>');
            var hdnScrollTop2 = document.getElementById('<%=hdnCLMTargetCodeScrollTop.ClientID %>');
            sel2.scrollTop = hdnScrollTop2.value;

        } 

    </script>
    <style type="text/css">
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
    <asp:Label ID="lblStructureSetDetail" runat="server" CssClass="PageTitle"><%= Resources.Messages.lbl_structureset %>&#32;<%= Resources.Messages.lbl_item_dettail %></asp:Label>
    <div id="divBack">
        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/StructureSet.aspx?m=y" ImageUrl="~/images/back.png"><%= Resources.Messages.lbl_back %></asp:HyperLink>
    </div>
    <hr style="width: 100%" />
    <div id="tab-container" class='tab-container'>
        <ul class='etabs'>
            <li class='tab'><a href="#general">
                <%= Resources.Messages.lbl_general %></a></li>
            <li class='tab'><a href="#codelistmap">
                <%= Resources.Messages.lbl_codelistmap%></a></li>
            <li class='tab'><a href="#structuremap">
                <%= Resources.Messages.lbl_structuremap%></a></li>
        </ul>
        <div class='panel-container'>
            <div id="general">
                <table class="tableForm">
                    <tr>
                        <td>
                            *<asp:Label ID="lblSSID" runat="server" Text="<%$ Resources:Messages, lbl_id%>" CssClass="tdProperty"></asp:Label>
                        </td>
                        <td width="45%">
                            <asp:TextBox ID="txtSSID" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                        <td>
                            *<asp:Label ID="lblAgency" runat="server" Text="<%$ Resources:Messages, lbl_agency%>"
                                CssClass="tdProperty"></asp:Label>
                        </td>
                        <td width="45%">
                            <asp:DropDownList ID="cmbAgencies" runat="server" Enabled="false">
                            </asp:DropDownList>
                            <asp:TextBox ID="txtAgenciesReadOnly" runat="server" Enabled="false" Visible="false"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            *<asp:Label ID="lblVersion" runat="server" Text="<%$ Resources:Messages, lbl_version%>"
                                CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtVersion" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lblIsFinal" runat="server" Text="<%$ Resources:Messages, lbl_is_final%>"
                                CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkIsFinal" runat="server" Enabled="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblURI" runat="server" Text="<%$ Resources:Messages, lbl_uri%>" CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtURI" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lblURN" runat="server" Text="<%$ Resources:Messages, lbl_urn%>" CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtURN" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblValidFrom" runat="server" Text="<%$ Resources:Messages, lbl_valid_from%>"
                                CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValidFrom" runat="server" Enabled="false" ValidationGroup="dsd"
                                CssClass="datepicker"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lblValidTo" runat="server" Text="<%$ Resources:Messages, lbl_valid_to%>"
                                CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValidTo" runat="server" Enabled="false" ValidationGroup="dsd"
                                CssClass="datepicker"></asp:TextBox>
                        </td>
                    </tr>
                    <%--                    <tr>
                        <td>
                            <asp:Label ID="lblDSD" runat="server" Text="<%# Resources.Messages.lbl_key_family + ':' %>" CssClass="tdProperty"></asp:Label>
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="txtDSD" runat="server" Enabled="false" ValidationGroup="dsd"></asp:TextBox>*<uc6:GetDSD ID="GetDSD1" runat="server" />
                        </td>
                    </tr>--%>
                    <tr>
                        <td>
                            *<asp:Label ID="lblSSNames" runat="server" Text="<%$ Resources:Messages, lbl_name%>"
                                CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:Panel ID="pnlViewName" runat="server" Visible="false">
                                <asp:TextBox ID="txtSSName" runat="server" Enabled="false" TextMode="MultiLine" Rows="5"
                                    ValidationGroup="dsd" CssClass="noScalableTextArea"></asp:TextBox>
                            </asp:Panel>
                            <asp:Panel ID="pnlEditName" runat="server" Visible="false">
                                <uc2:AddText ID="AddTextName" runat="server" />
                            </asp:Panel>
                        </td>
                        <td>
                            <asp:Label ID="lblSSDescriptions" runat="server" Text="<%$ Resources:Messages, lbl_description%>"
                                CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:Panel ID="pnlViewDescription" runat="server" Visible="false">
                                <asp:TextBox ID="txtSSDescription" runat="server" Enabled="false" TextMode="MultiLine"
                                    Rows="5" CssClass="noScalableTextArea"></asp:TextBox>
                            </asp:Panel>
                            <asp:Panel ID="pnlEditDescription" runat="server" Visible="false">
                                <uc2:AddText ID="AddTextDescription" runat="server" />
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lbl_annotation" runat="server" Text="<%$ Resources:Messages, lbl_annotation%>"
                                CssClass="tdProperty"></asp:Label>
                        </td>
                        <td colspan="3">
                            <asp:Panel ID="pnlAnnotation" runat="server" Visible="true">
                                <uc4:ControlAnnotations ID="AnnotationGeneral" runat="server" />
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="codelistmap">
                <iup:IstatUpdatePanel ID="UpdatePanel4" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="gvCodeListMap" runat="server" Width="100%" AllowPaging="True" CssClass="Grid"
                            OnPageIndexChanging="gvCodeListMap_PageIndexChanging" OnRowDeleting="gvCodeListMap_RowDeleting"
                            OnRowCommand="gvCodeListMap_RowCommand" AutoGenerateColumns="False">
                            <Columns>
                                <asp:TemplateField HeaderText="ID" SortExpression="ID">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCLMID" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="30%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="CL Source" SortExpression="CLSource">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCLMCLSource" runat="server" Text='<%# Bind("CLSource") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="30%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="CL Target" SortExpression="CLTarget">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCLMCLTarget" runat="server" Text='<%# Bind("CLTarget") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="30%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ibCLMDelete" runat="server" OnClientClick="return ConfirmDeleteCLM()"
                                            CausesValidation="False" CommandName="Delete" CommandArgument="<%# Container.DataItemIndex %>"
                                            ImageUrl="~/images/Delete2.png" ToolTip="Delete CodeListMap" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="" ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ibCLMEditEdit" runat="server" CausesValidation="False" CommandName="Details"
                                            CommandArgument="<%# Container.DataItemIndex %>" ImageUrl="~/images/Details2.png"
                                            ToolTip="View/Edit Details" />
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
                        <%--                       <div id="df-annattribute" class="popup_block">
                            <asp:Label ID="lblAnnotationAttribute" runat="server" Text="Annotation Manager" CssClass="PageTitle"></asp:Label>
                            <hr style="width: 95%;" />
                            <table class="tableForm">
                                <tr>
                                    <td>
                                        <uc5:ControlAnnotations ID="ucAnnotationAttribute" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </div>--%>
                        <asp:Panel ID="pnlAddCLM" runat="server">
                            <asp:ImageButton ID="btnAddCLM" runat="server" ImageUrl="./images/Add.png" ToolTip="<%$ Resources:Messages, btn_add_codelistmap %>"
                                AlternateText="<%$ Resources:Messages, btn_add_codelistmap %>" OnClick="btnAddCLM_Click" />
                        </asp:Panel>
                        <div id="df-CLM" class="popup_block">
                            <asp:Label ID="Label7" runat="server" Text="<%$ Resources:Messages, lbl_add_edit_codelistmap %>" CssClass="PageTitle"></asp:Label>&nbsp;
                            <asp:HiddenField ID="hdCLMAction" runat="server" />
                            <div id="clmGeneral">
                                <h3>
                                    <%= Resources.Messages.lbl_general %></h3>
                                <div>
                                    <p>
                                        <table>
                                            <tr>
                                                <td width="160px">
                                                    *<asp:Label ID="Label8" runat="server" Text="ID:" CssClass="tdProperty"></asp:Label>
                                                </td>
                                                <td width="370px">
                                                    <asp:TextBox ID="txtCLMID" runat="server" Width="300px" Enabled="false"></asp:TextBox>                                                    
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    *<asp:Label ID="lblCLMNames" runat="server" Text="<%$ Resources:Messages, lbl_name%>"
                                                        CssClass="tdProperty"></asp:Label>
                                                </td>
                                                <td>
                                                    <uc2:AddText ID="AddTextCLMNames" runat="server" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblCLMDescription" runat="server" Text="<%$ Resources:Messages, lbl_description%>"
                                                        CssClass="tdProperty"></asp:Label>
                                                </td>
                                                <td>
                                                    <uc2:AddText ID="AddTextCLMDescription" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblCMLAnnotation" runat="server" Text="<%$ Resources:Messages, lbl_annotation%>"
                                                        CssClass="tdProperty"></asp:Label>
                                                </td>
                                                <td colspan="3">
                                                    <asp:Panel ID="pnlClmAnnotation" runat="server" Visible="true">
                                                        <uc4:ControlAnnotations ID="ControlAnnotationsCLM" runat="server" />
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <br />
                                                    <asp:Button ID="btnSaveCLM" runat="server" Text="<%$ Resources:Messages, btn_next_step%>"
                                                        OnClick="btnSaveCLM_Click" Visible="false"/>
                                                </td>
                                            </tr>
                                        </table>
                                    </p>
                                </div>
                                <h3>
                                    <%= Resources.Messages.lbl_mapping %></h3>
                                <div>
                                    <table>
                                        <tr>
                                            <th width="25%">
                                                <asp:Label ID="Label1" runat="server" Text="<%$ Resources:Messages, lbl_codelist_source%>" CssClass="tdProperty"></asp:Label>
                                            </th>
                                            <th width="25%">
                                                <asp:Label ID="Label5" runat="server" Text="<%$ Resources:Messages, lbl_codelist_target%>" CssClass="tdProperty"></asp:Label>
                                            </th>
                                            <td width="50%" rowspan="4">
                                                <asp:GridView ID="gvCLMMapping" runat="server" Width="100%" AllowPaging="True" CssClass="Grid"
                                                    OnPageIndexChanging="gvCLMMapping_PageIndexChanging" OnRowDeleting="gvCLMMapping_RowDeleting"
                                                    AutoGenerateColumns="False">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="<%$ Resources:Messages, lbl_source_code%>" SortExpression="SourceCode">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblCLMSourceCode" runat="server" Text='<%# Bind("SourceCode") %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <HeaderStyle Width="48%" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="<%$ Resources:Messages, lbl_target_code%>" SortExpression="TargetCode">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblCLMTargetCode" runat="server" Text='<%# Bind("TargetCode") %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <HeaderStyle Width="48%" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="ibCLMDeleteMapping" runat="server" OnClientClick="return ConfirmDeleteMapping()"
                                                                    CausesValidation="False" CommandName="Delete" CommandArgument="<%# Container.DataItemIndex %>"
                                                                    ImageUrl="~/images/Delete2.png" ToolTip="Delete Mapping" />
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
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap>
                                                <asp:TextBox ID="txtCLSource" runat="server" Width="160px" Enabled="false"></asp:TextBox>
                                                <uc8:GetCodeList ID="GetCodeListCLMSource" runat="server"  />
                                            </td>
                                            <td nowrap>
                                                <asp:TextBox ID="txtCLTarget" runat="server" Width="160px" Enabled="false"></asp:TextBox>
                                                <uc8:GetCodeList ID="GetCodeListCLMTarget" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:ListBox ID="lbCLMSourceCode" ClientIDMode="Static" runat="server" Width="180px"
                                                    Height="220px"></asp:ListBox>
                                                <asp:HiddenField ID="hdnCLMSourceCodeScrollTop" runat="server" />
                                            </td>
                                            <td>
                                                <asp:ListBox ID="lbCLMTargetCode" ClientIDMode="Static" runat="server" Width="180px"
                                                    Height="220px"></asp:ListBox>
                                                <asp:HiddenField ID="hdnCLMTargetCodeScrollTop" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Button ID="btnAddCLMMap" runat="server" Text="<%$ Resources:Messages, btn_add_mapping%>" OnClick="btnAddCLMMap_Click" />
                                                <asp:Button ID="btnSaveCLM2" runat="server" Text="<%$ Resources:Messages, btn_save%>" OnClick="btnSaveCLM_Click" Visible="false" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <asp:HiddenField ID="hdnSelectedMappingType" runat="server" />
                    </ContentTemplate>
                </iup:IstatUpdatePanel>
            </div>
            <div id="structuremap">
                <iup:IstatUpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="gvStructureMap" runat="server" Width="100%" AllowPaging="True"
                            CssClass="Grid" OnPageIndexChanging="gvStructureMap_PageIndexChanging" OnRowDeleting="gvStructureMap_RowDeleting"
                            OnRowCommand="gvStructureMap_RowCommand" AutoGenerateColumns="False">
                            <Columns>
                                <asp:TemplateField HeaderText="ID" SortExpression="ID">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSMID" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="30%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<%$ Resources:Messages, lbl_artefact_source%>" SortExpression="ArtefactSource">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSMArtefactSource" runat="server" Text='<%# Bind("ArtefactSource") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="30%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<%$ Resources:Messages, lbl_artefact_target%>" SortExpression="ArtefactTarget">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSMArtefactTarget" runat="server" Text='<%# Bind("ArtefactTarget") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="30%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<%$ Resources:Messages, lbl_artefact_type%>" SortExpression="ArtefactType" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSMArtefactType" runat="server" Text='<%# Bind("ArtefactType") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="1%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ibSMDelete" runat="server" OnClientClick="return ConfirmDeleteSM()"
                                            CausesValidation="False" CommandName="Delete" CommandArgument="<%# Container.DataItemIndex %>"
                                            ImageUrl="~/images/Delete2.png" ToolTip="<%$ Resources:Messages, btn_delete_structuremap%>" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="" ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ibSMEditEdit" runat="server" CausesValidation="False" CommandName="Details"
                                            CommandArgument="<%# Container.DataItemIndex %>" ImageUrl="~/images/Details2.png"
                                            ToolTip="<%$ Resources:Messages, lbl_view_edit_details%>" />
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
                        <asp:Panel ID="pnlAddSM" runat="server">
                            <asp:ImageButton ID="btnAddSM" runat="server" ImageUrl="./images/Add.png" ToolTip="<%$ Resources:Messages, btn_add_structuremap%>"
                                AlternateText="<%$ Resources:Messages, btn_add_structuremap%>" OnClick="btnAddSM_Click" />
                        </asp:Panel>
                        <div id="df-SM" class="popup_block" title="Add StructureMap">
                            <asp:Label ID="Label2" runat="server" Text="<%$ Resources:Messages, lbl_add_edit_structuremap%>" CssClass="PageTitle"></asp:Label>&nbsp;
                            <asp:HiddenField ID="hdSMAction" runat="server" />
                            <div id="smGeneral">
                                <h3>
                                    <%= Resources.Messages.lbl_general %></h3>
                                <div>
                                    <p>
                                        <table>
                                            <tr>
                                                <td width="160px">
                                                    <asp:Label ID="Label3" runat="server" Text="ID:" CssClass="tdProperty"></asp:Label>
                                                </td>
                                                <td width="370px">
                                                    <asp:TextBox ID="txtSMID" runat="server" Width="300px" Enabled="false"></asp:TextBox>                                                    
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblSMNames" runat="server" Text="<%$ Resources:Messages, lbl_name%>"
                                                        CssClass="tdProperty"></asp:Label>
                                                </td>
                                                <td>
                                                    <uc2:AddText ID="AddTextSMNames" runat="server" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblSMDescription" runat="server" Text="<%$ Resources:Messages, lbl_description%>"
                                                        CssClass="tdProperty"></asp:Label>
                                                </td>
                                                <td>
                                                    <uc2:AddText ID="AddTextSMDescription" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblSMAnnotation" runat="server" Text="<%$ Resources:Messages, lbl_annotation%>"
                                                        CssClass="tdProperty"></asp:Label>
                                                </td>
                                                <td colspan="3">
                                                    <asp:Panel ID="pnlSMAnnotation" runat="server" Visible="true">
                                                        <uc4:ControlAnnotations ID="ControlAnnotationsSM" runat="server" />
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <br />
                                                    <asp:Button ID="btnSaveSM" runat="server" Text="<%$ Resources:Messages, btn_next_step%>"
                                                        OnClick="btnSaveSM_Click" Visible="false" />
                                                </td>
                                            </tr>
                                        </table>
                                    </p>
                                </div>
                                <h3>
                                    <%= Resources.Messages.lbl_mapping %></h3>
                                <div>
                                    <table>
                                        <tr>
                                            <td colspan="2">
                                                <asp:Label ID="Label4" runat="server" Text="Artefact Type" CssClass="tdProperty"></asp:Label>
                                                <asp:DropDownList ID="cmbArtefactType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbArtefactType_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th width="25%">
                                                <asp:Label ID="Label10" runat="server" Text="Artefact Source" CssClass="tdProperty"></asp:Label>
                                            </th>
                                            <th width="25%">
                                                <asp:Label ID="Label11" runat="server" Text="Artefact Target" CssClass="tdProperty"></asp:Label>
                                            </th>
                                            <td width="50%" rowspan="4">
                                                <asp:GridView ID="gvSMMapping" runat="server" Width="100%" AllowPaging="True" CssClass="Grid"
                                                    OnPageIndexChanging="gvSMMapping_PageIndexChanging" OnRowDeleting="gvSMMapping_RowDeleting"
                                                    AutoGenerateColumns="False">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Source Dimension" SortExpression="SourceCode">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblSMSourceDimension" runat="server" Text='<%# Bind("SourceCode") %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <HeaderStyle Width="48%" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Target Dimension" SortExpression="TargetCode">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblSMTargetDimension" runat="server" Text='<%# Bind("TargetCode") %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <HeaderStyle Width="48%" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="ibSMDeleteMapping" runat="server" OnClientClick="return ConfirmDeleteMapping()"
                                                                    CausesValidation="False" CommandName="Delete" CommandArgument="<%# Container.DataItemIndex %>"
                                                                    ImageUrl="~/images/Delete2.png" ToolTip="Delete Mapping" />
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
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap>
                                                <asp:TextBox ID="txtArtefactSource" runat="server" Width="160px" Enabled="false"></asp:TextBox>
                                                <uc6:GetDSD ID="GetDSDSource" runat="server" />
                                                <uc10:GetDataFlow ID="GetDataFlowSource" runat="server" Visible="False" />
                                            </td>
                                            <td nowrap>
                                                <asp:TextBox ID="txtArtefactTarget" runat="server" Width="160px" Enabled="false"></asp:TextBox>
                                                <uc6:GetDSD ID="GetDSDTarget" runat="server" />
                                                <uc10:GetDataFlow ID="GetDataFlowTarget" runat="server" Visible="False" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:ListBox ID="lbSMSourceCode" ClientIDMode="Static" runat="server" Width="180px"
                                                    Height="220px"></asp:ListBox>
                                                <asp:HiddenField ID="hdnSMSourceCodeScrollTop" runat="server" />
                                            </td>
                                            <td>
                                                <asp:ListBox ID="lbSMTargetCode" ClientIDMode="Static" runat="server" Width="180px"
                                                    Height="220px"></asp:ListBox>
                                                <asp:HiddenField ID="hdnSMTargetCodeScrollTop" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Button ID="btnAddSMMap" runat="server" Text="Add Mapping" OnClick="btnAddSMMap_Click" />
                                                <asp:Button ID="btnSaveSM2" runat="server" Text="<%$ Resources:Messages, btn_save%>"
                                                        OnClick="btnSaveSM_Click" Visible="false" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </iup:IstatUpdatePanel>
            </div>
        </div>
        <asp:Button ID="btnSaveSS" runat="server" Text="<%$ Resources:Messages, btn_save%>"
            OnClick="btnSaveSS_Click" Visible="false" />
        <div style="float: right">
            <uc1:FileDownload3 ID="FileDownload31" runat="server" ucArtefactType="StructureSet" />
        </div>
        <uc7:DuplicateArtefact ID="DuplicateArtefact1" runat="server" Visible="false" />
        <br />
</asp:Content>
