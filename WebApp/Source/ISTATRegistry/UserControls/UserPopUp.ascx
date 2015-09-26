<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserPopUp.ascx.cs" Inherits="ISTATRegistry.UserControls.UserPopUp" %>
<style type="text/css">
.upCenterTitle
{
    display:block;
    width: 100%;
    height: 100%;
    margin: 0 auto;
    text-align:center;
    font-size:medium;
    font-weight: bold;
}
</style>

<div id="up<%=upID%>" class="popup_block">

    <asp:Label ID="lblUpTitle" Text="" runat="server" CssClass="PageTitle3" Visible="true" Style="font-weight: bold" />
    
    <hr style="width: 100%" />

    <asp:Literal ID="ltUpText" runat="server"></asp:Literal>

    <asp:Panel ID="pnlConfirm" runat="server" Visible="false">

        <hr style="width: 100%" />

        <div style="float: right">
            <asp:Button ID="btnCancel" runat="server" onclick="btnCancel_Click" Text="<%# Resources.Messages.btn_cancel %>" />
            <asp:Button ID="btnOk" runat="server" onclick="btnOk_Click" Text="<%# Resources.Messages.btn_ok %>" />
        </div>

    </asp:Panel>

</div>
