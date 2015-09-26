<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="DataFlowItemDetails.aspx.cs" Inherits="ISTATRegistry.dataFlowItemDetails"
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
    <asp:Label ID="lblDataFlowDetail" runat="server" CssClass="PageTitle"><%= Resources.Messages.lbl_data_flow %>&#32;<%= Resources.Messages.lbl_item_dettail %></asp:Label>
    <div id="divBack">
        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/DataFlow.aspx?m=y" ImageUrl="~/images/back.png"><%= Resources.Messages.lbl_back %></asp:HyperLink>
    </div>
    <hr style="width: 100%" />
    <div id="tab-container" class='tab-container'>
        <ul class='etabs'>
            <li class='tab'><a href="#general">
                <%= Resources.Messages.lbl_general %></a></li>
        </ul>
        <div class='panel-container'>
            <div id="general">
                <iup:IstatUpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <table class="tableForm">
                            <tr>
                                <td>
                                    <asp:Label ID="lblDFID" runat="server" Text="<%# Resources.Messages.lbl_id + ':' %>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td width="45%">
                                    <asp:TextBox ID="txtDFID" runat="server" Enabled="false" ValidationGroup="dsd"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Label ID="lblAgency" runat="server" Text="<%# Resources.Messages.lbl_agency + ':' %>"
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
                                    <asp:Label ID="lblVersion" runat="server" Text="<%# Resources.Messages.lbl_version + ':' %>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtVersion" runat="server" Enabled="false" ValidationGroup="dsd"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Label ID="lblIsFinal" runat="server" Text="<%# Resources.Messages.lbl_is_final + ':' %>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkIsFinal" runat="server" Enabled="false" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblURI" runat="server" Text="<%# Resources.Messages.lbl_uri + ':' %>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtURI" runat="server" Enabled="false"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Label ID="lblURN" runat="server" Text="<%# Resources.Messages.lbl_urn + ':' %>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtURN" runat="server" Enabled="false"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblValidFrom" runat="server" Text="<%# Resources.Messages.lbl_valid_from + ':' %>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtValidFrom" runat="server" Enabled="false" ValidationGroup="dsd"
                                        CssClass="datepicker"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Label ID="lblValidTo" runat="server" Text="<%# Resources.Messages.lbl_valid_to + ':' %>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtValidTo" runat="server" Enabled="false" ValidationGroup="dsd"
                                        CssClass="datepicker"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblDSD" runat="server" Text="<%# Resources.Messages.lbl_key_family + ':' %>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td colspan="3">
                                    <asp:TextBox ID="txtDSD" runat="server" Enabled="false" ValidationGroup="dsd"></asp:TextBox>*<uc6:GetDSD
                                        ID="GetDSD1" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblDFNames" runat="server" Text="<%# Resources.Messages.lbl_name + ':' %>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:Panel ID="pnlViewName" runat="server" Visible="false">
                                        <asp:TextBox ID="txtDFName" runat="server" Enabled="false" TextMode="MultiLine" Rows="5"
                                            ValidationGroup="dsd" CssClass="noScalableTextArea"></asp:TextBox>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlEditName" runat="server" Visible="false">
                                        <uc2:AddText ID="AddTextName" runat="server" />
                                    </asp:Panel>
                                </td>
                                <td>
                                    <asp:Label ID="lblDFDescriptions" runat="server" Text="<%# Resources.Messages.lbl_description + ':' %>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:Panel ID="pnlViewDescription" runat="server" Visible="false">
                                        <asp:TextBox ID="txtDFDescription" runat="server" Enabled="false" TextMode="MultiLine"
                                            Rows="5" CssClass="noScalableTextArea"></asp:TextBox>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlEditDescription" runat="server" Visible="false">
                                        <uc2:AddText ID="AddTextDescription" runat="server" />
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lbl_annotation" runat="server" Text="<%# Resources.Messages.lbl_annotation+ ':' %>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td colspan="3">
                                    <asp:Panel ID="pnlAnnotation" runat="server" Visible="true">
                                        <uc4:ControlAnnotations ID="AnnotationGeneral" runat="server" />
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </iup:IstatUpdatePanel>
            </div>
        </div>
        <asp:Button ID="btnSaveDF" runat="server" Text="<%# Resources.Messages.btn_save_dataflow%>"
            OnClick="btnSaveDF_Click" Visible="false" />
        <div style="float: right">
            <uc1:FileDownload3 ID="FileDownload31" runat="server" />
        </div>
        <uc7:DuplicateArtefact ID="DuplicateArtefact1" runat="server" Visible="false"/>
        <br />
</asp:Content>
