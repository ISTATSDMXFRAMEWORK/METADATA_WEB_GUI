<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="ContentConstraintItemDetails.aspx.cs" Inherits="ISTATRegistry.ContentConstraintItemDetails"
    EnableSessionState="True" %>

<%@ Register Src="UserControls/FileDownload3.ascx" TagName="FileDownload3" TagPrefix="uc1" %>
<%@ Register Src="UserControls/AddText.ascx" TagName="AddText" TagPrefix="uc2" %>
<%@ Register Src="UserControls/UserPopUp.ascx" TagName="UserPopUp" TagPrefix="uc3" %>
<%@ Register Src="~/UserControls/ControlAnnotations.ascx" TagName="ControlAnnotations"
    TagPrefix="uc4" %>
<%@ Register Src="UserControls/SearchBar.ascx" TagName="SearchBar" TagPrefix="uc5" %>
<%@ Register Src="UserControls/GetDSD.ascx" TagName="GetDSD" TagPrefix="uc6" %>
<%@ Register Src="UserControls/DuplicateArtefact.ascx" TagName="DuplicateArtefact"
    TagPrefix="uc7" %>
<%@ Register Src="UserControls/GetDataFlow.ascx" TagName="GetDataFlow" TagPrefix="uc8" %>
<%@ Register Namespace= "ISTATRegistry.Classes" TagPrefix="iup" Assembly="IstatRegistry" %>
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
        });

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
    <asp:Label ID="lblContConstrDetail" runat="server" CssClass="PageTitle"><%= Resources.Messages.lbl_content_constraint%>&#32;<%= Resources.Messages.lbl_item_dettail %></asp:Label>
    <div id="divBack">
        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/ContentConstraint.aspx?m=y"
            ImageUrl="~/images/back.png"><%= Resources.Messages.lbl_back %></asp:HyperLink>
    </div>
    <hr style="width: 100%" />
    <div id="tab-container" class='tab-container'>
        <ul class='etabs'>
            <li class='tab'><a href="#general">
                <%= Resources.Messages.lbl_general %></a></li>
            <li class='tab'><a href="#conselements">
                <%= Resources.Messages.lbl_constraintselements %></a></li>
            <li class='tab'><a href="#releasecalendar">Release Calendar</a></li>
        </ul>
        <div class='panel-container'>
            <div id="general">
                <table class="tableForm">
                    <tr>
                        <td>
                            *<asp:Label ID="lblCCID" runat="server" Text="<%$ Resources:Messages, lbl_id %>" CssClass="tdProperty"></asp:Label>
                        </td>
                        <td width="45%">
                            <asp:TextBox ID="txtCCID" runat="server" Enabled="false" ValidationGroup="dsd"></asp:TextBox>
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
                            <asp:TextBox ID="txtVersion" runat="server" Enabled="false" ValidationGroup="dsd"></asp:TextBox>
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
                    <tr>
                        <td>
                            *<asp:Label ID="lblCCNames" runat="server" Text="<%$ Resources:Messages, lbl_name%>"
                                CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:Panel ID="pnlViewName" runat="server" Visible="false">
                                <asp:TextBox ID="txtCCName" runat="server" Enabled="false" TextMode="MultiLine" Rows="5"
                                    ValidationGroup="dsd" CssClass="noScalableTextArea"></asp:TextBox>
                            </asp:Panel>
                            <asp:Panel ID="pnlEditName" runat="server" Visible="false">
                                <uc2:AddText ID="AddTextName" runat="server" />
                            </asp:Panel>
                        </td>
                        <td>
                            <asp:Label ID="lblCCDescriptions" runat="server" Text="<%$ Resources:Messages, lbl_description%>"
                                CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:Panel ID="pnlViewDescription" runat="server" Visible="false">
                                <asp:TextBox ID="txtCCDescription" runat="server" Enabled="false" TextMode="MultiLine"
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
            <div id="conselements">
                <iup:IstatUpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <fieldset>
                            <legend>
                                *<asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Messages, lbl_artefact_type%>" /></legend>
                            <table class="tableForm">
                                <tr>
                                    <td colspan="2">
                                        <asp:DropDownList ID="cmbArtefactType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbArtefactType_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:GridView ID="gvIncludedArtefact" runat="server" CssClass="Grid" AllowPaging="True"
                                            PagerSettings-Mode="NumericFirstLast" PagerSettings-FirstPageText="<%# Resources.Messages.btn_goto_first %>"
                                            PagerSettings-LastPageText="<%# Resources.Messages.btn_goto_last %>" AutoGenerateColumns="False"
                                            PagerSettings-Position="TopAndBottom" OnRowCommand="gvIncludedArtefact_RowCommand"
                                            OnRowDeleting="gvIncludedArtefact_RowDeleting">
                                            <Columns>
                                                <asp:TemplateField HeaderText="ID" SortExpression="ID">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblID" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="190px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Agency" SortExpression="Agency">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAgency" runat="server" Text='<%# Bind("Agency") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="70px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Version" SortExpression="Version">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblVersion" runat="server" Text='<%# Bind("Version") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="50px" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="LocalID" HeaderText="LocalID" SortExpression="LocalID"
                                                    Visible="False" />
                                                <asp:TemplateField HeaderText="" ShowHeader="False">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                                            CommandArgument="<%# Container.DataItemIndex %>" ImageUrl="~/images/delete2.png"
                                                            ToolTip="Delete" />
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
                                    <td colspan="2">
                                        <uc6:GetDSD ID="GetDSD1" runat="server" />
                                        <uc8:GetDataFlow ID="GetDataFlow1" runat="server" Visible="False" />
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                        <fieldset>
                            <legend>*Constraint Components</legend>
                            <table>
                                <tr>
                                    <td style="width: 50%">
                                        <asp:GridView ID="gvDimension" runat="server" CssClass="Grid" AllowPaging="True"
                                            PagerSettings-Mode="NumericFirstLast" PagerSettings-FirstPageText="<%# Resources.Messages.btn_goto_first %>"
                                            PagerSettings-LastPageText="<%# Resources.Messages.btn_goto_last %>" AutoGenerateColumns="False"
                                            PagerSettings-Position="Bottom" OnRowCommand="gvDimension_RowCommand" 
                                            OnPageIndexChanging="gvDimension_PageIndexChanging" 
                                            onrowdatabound="gvDimension_RowDataBound">
                                            <Columns>
                                                <asp:TemplateField HeaderText="ID" SortExpression="ID">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblID" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="190px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Codelist" SortExpression="Codelist">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCodelist" runat="server" Text='<%# Bind("Codelist") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="150px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="ComponentType" SortExpression="ComponentType" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblComponentType" runat="server" Text='<%# Bind("ComponentType") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ShowHeader="False">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDimNumber" runat="server" Text=""></asp:Label>
                                                        <asp:ImageButton ID="btnViewItemDimension" runat="server" CausesValidation="False"
                                                            CommandName="View" CommandArgument="<%# Container.DataItemIndex %>" ImageUrl="~/images/details2.png"
                                                            ToolTip="Delete" />
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Right" />
                                                </asp:TemplateField>
                                            </Columns>
                                            <HeaderStyle CssClass="hs" />
                                            <RowStyle CssClass="rs" />
                                            <AlternatingRowStyle CssClass="ars" />
                                            <PagerStyle CssClass="pgr"></PagerStyle>
                                        </asp:GridView>
                                    </td>
                                    <td style="width: 50%">
                                        <table>
                                            <tr>
                                                <th colspan="3">
                                                    <asp:Label ID="lblComponentSelected" runat="server" Text="Selected Component:" Visible="false"
                                                        CssClass="PageTitle"></asp:Label>
                                                    <asp:Label ID="lblComponentSelectedID" runat="server" Text="" CssClass="PageTitle"></asp:Label>
                                                    <asp:HiddenField ID="hdnCLSelected" runat="server" />
                                                    <asp:HiddenField ID="hdnSelectedComponentType" runat="server" />
                                                </th>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:ListBox ID="lbSource" runat="server" Width="160px" Height="180px" SelectionMode="Multiple"></asp:ListBox>
                                                </td>
                                                <td>
                                                    <asp:Button ID="btnAdd" runat="server" Text=" > " OnClick="btnAdd_Click" />
                                                    <br />
                                                    <asp:Button ID="btnRemove" runat="server" Text=" < " OnClick="btnRemove_Click" />
                                                    <br />
                                                    <asp:Button ID="btnRemoveAll" runat="server" Text="<<" OnClick="btnRemoveAll_Click" />
                                                </td>
                                                <td>
                                                    <asp:ListBox ID="lbTarget" runat="server" Width="160px" Height="180px" SelectionMode="Multiple"></asp:ListBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="3">
                                                    <asp:Button ID="btnSaveComponents" runat="server" Text="<%$ Resources:Messages, btn_save%>"
                                                        OnClick="btnSaveComponents_Click" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </ContentTemplate>
                </iup:IstatUpdatePanel>
            </div>
            <div id="releasecalendar">
                <iup:IstatUpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <table class="tableForm">
                            <tr>
                                <td>
                                    *<asp:Label ID="Label1" runat="server" Text="<%$ Resources:Messages, lbl_periodicity %>" CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPeriodicity" runat="server" Enabled="false"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:DropDownList ID="cmbPeriodicity" runat="server" Enabled="false">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    *<asp:Label ID="Label2" runat="server" Text="<%$ Resources:Messages, lbl_offset %>" CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtOffset" runat="server" Enabled="false"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:DropDownList ID="cmbOffset" runat="server" Enabled="false">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    *<asp:Label ID="Label3" runat="server" Text="<%$ Resources:Messages, lbl_tolerance %>" CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtTolerance" runat="server" Enabled="false"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:DropDownList ID="cmbTolerance" runat="server" Enabled="false">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </iup:IstatUpdatePanel>
                </div>
            </div>
            <asp:Button ID="btnSaveCC" runat="server" Text="<%$ Resources:Messages, btn_save%>"
                OnClick="btnSaveCC_Click" Visible="false" />
            <div style="float: right">
                <uc1:FileDownload3 ID="FileDownload31" runat="server" />
            </div>
            <uc7:DuplicateArtefact ID="DuplicateArtefact1" runat="server" Visible="false"/>
            <br />
</asp:Content>
