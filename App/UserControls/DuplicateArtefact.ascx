<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DuplicateArtefact.ascx.cs" Inherits="ISTATRegistry.UserControls.DuplicateArtefact" %>

<input id="btnOpenPopUp" type="button" value="<%=Resources.Messages.btn_Duplicate_Artefact %>" onclick='javascript: openP("dialog-form<%=this.ClientID %>")' />

<div id="dialog-form<%=this.ClientID %>" class="popup_block" >
    
    <asp:Label ID="lblTitle" runat="server" 
        Text="<%$ Resources:Messages, btn_Duplicate_Artefact %>" CssClass="PageTitle"></asp:Label>
    
    <hr style="width: 100%;" />
    
    <table style="width: 100%;">
        <tr>
            <td colspan="2" style="width: 100%;">
                <asp:Label ID="lblLanguage" runat="server" Text="<%$ Resources:Messages, lbl_source %>"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="width: 90px;">
                <asp:Label ID="lblArtType_E" runat="server" Text="<%$ Resources:Messages, lbl_artefact_type %>"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblArtType" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblArtID_E" runat="server" Text="<%$ Resources:Messages, lbl_id %>"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblArtID" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblArtAgency_E" runat="server" Text="<%$ Resources:Messages, lbl_agency %>"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblArtAgency" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblArtVersion_E" runat="server" Text="<%$ Resources:Messages, lbl_version %>"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblArtVersion" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <hr style="width: 100%;" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="Label1" runat="server" Text="<%$ Resources:Messages, lbl_target %>"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                *<asp:Label ID="lblDestID" runat="server" Text="<%$ Resources:Messages, lbl_id %>"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtDSDID" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                *<asp:Label ID="lblDestAgency" runat="server" Text="<%$ Resources:Messages, lbl_agency %>"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="cmbAgencies" runat="server"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                *<asp:Label ID="lblDestVersion" runat="server" Text="<%$ Resources:Messages, lbl_version %>"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtVersion" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <hr style="width: 100%;" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="btnDuplicate" runat="server" Text="<%$ Resources:Messages, btn_duplicate %>" OnClientClick = "return ConfirmBeforeDuplicate()"
                    ToolTip="<%$ Resources:Messages, btn_duplicate %>" onclick="btnDuplicate_Click"/>
            </td>
        </tr>
    </table>
    <script>
        function ConfirmBeforeDuplicate() {
            return confirm('<%= Resources.Messages.msg_save_before_duplicate %>');
        }
    </script>
</div>