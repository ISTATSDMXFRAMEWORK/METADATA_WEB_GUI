<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileDownload3.ascx.cs"
    Inherits="ISTATRegistry.UserControls.FileDownload3" %>

<div id="dialog-form<%=ucID + '_' + ucAgency + '_' + ucVersion.Replace( '.', '_' )%>" class="popup_block">
    
    <asp:Label ID="lblArtefactID" runat="server" Text="<%# Resources.Messages.lbl_artefact+':' %>" CssClass="PageTitle3" EnableViewState="True"></asp:Label>
    <br /><br />
    <hr style="width: 100%" />
    
    <table class="tbNoBorder" cellpadding="6">
        <tr> 
            <td colspan="2">
                <asp:Label ID="lblTitle" Text="<%# Resources.Messages.lbl_download_title %>" runat="server" Visible="true" style="font-weight:bold" />
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblExportType" runat="server" Text="<%# Resources.Messages.lbl_export_type+':' %>"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="cmbDownloadType" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblStub" runat="server" Visible = "false" Text="<%# Resources.Messages.lbl_stub+':' %>"></asp:Label>
            </td>
            <td>
                <asp:CheckBox ID="chkStub" runat="server" Visible = "false"/>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblIncludeCodeListAndConceptScheme" runat="server" Text="<%# Resources.Messages.lbl_include_codelist_and_conceptscheme +':' %>" Visible = "false"></asp:Label>
            </td>
            <td>
                <asp:CheckBox ID="chkExportCodeListAndConcept" runat="server" Checked ="false" Visible = "false" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblSeparator" runat="server" Text="<%# Resources.Messages.lbl_use_this_separator +':' %>"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtSeparator" Text="" Width = "15px" runat="server" />
            </td>
        </tr>
    </table>              
    <hr style="width: 100%" />    
    <br />
    <div style="float: right">
    <center>
        <asp:Button ID="btnDownload" runat="server" OnClick="btnDownload_Click" Text="<%# Resources.Messages.lbl_download %>" />
    </center>
    </div>

</div>

<script>

    $(document).ready(function () {

        $("#<%=lblSeparator.ClientID %>, #<%=txtSeparator.ClientID %>").hide();

        $("#<%=cmbDownloadType.ClientID %>").change(function () {
            var selectedItem = $(this).val();
            if (selectedItem == "CSV") {
                $("#<%=lblSeparator.ClientID %>, #<%=txtSeparator.ClientID %>").show();
            }
            else {
                $("#<%=lblSeparator.ClientID %>, #<%=txtSeparator.ClientID %>").hide();
            }
        });

        $('#<%= txtSeparator.ClientID %>').keydown(function (event) {
            var separator = $(this).val();
            if (separator.length == 1 && event.keyCode != 8) {
                return false;
            }
        });
    });

</script>
<img src="./images/download2.png" id= "imgDownloadButton" onclick="openPopUp('dialog-form<%=ucID + '_' + ucAgency + '_' + ucVersion.Replace( '.', '_' )%>')" style="border:none; cursor:pointer;" alt="<%= Resources.Messages.lbl_download_artefact %>" title="<%= Resources.Messages.lbl_download_artefact %>"/>
 
<asp:Label ID="lblID" Text="" runat="server" Visible="false" />
<asp:Label ID="lblAgency" Text="" runat="server" Visible="false" />
<asp:Label ID="lblVersion" Text="" runat="server" Visible="false" />
<asp:Label ID="lblArtefactType" Text="" runat="server" Visible="false" />

