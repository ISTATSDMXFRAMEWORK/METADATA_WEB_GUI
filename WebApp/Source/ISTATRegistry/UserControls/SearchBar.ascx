<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchBar.ascx.cs" Inherits="ISTATRegistry.UserControls.SearchBar" %>

<asp:Panel ID="pnlSearchButton" runat="server">
    <a id = "searchLink" onclick =  "$('#SearchPanel').toggle('slow', function(){ if ( $('#SearchPanel').css('display') == 'block') { $( '#imgSearch' ).attr( 'src', './images/Search_on.png' ); } else { $( '#imgSearch' ).attr( 'src', './images/Search_off.png' ); }}); return false;">
        <img id= "imgSearch" src="./images/Search_off.png" border="0" />
    </a>
</asp:Panel>

<asp:Panel ID="pnlFixed" runat="server" Visible="false">
    <img id= "img1" src="./images/Search_off.png" border="0" />
</asp:Panel>

<div id="SearchPanel">
    <table class="SearchClass">
        <tr>
            <td>
                <asp:Label ID="lbl_id" runat="server" Text="<%# Resources.Messages.lbl_id %>"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtSearchID" runat="server" onkeydown = "return (event.keyCode!=13);"></asp:TextBox>
            </td>
            <td>
                <asp:Label ID="lbl_agency" runat="server" Text="<%# Resources.Messages.lbl_agency %>"></asp:Label>
            </td>
            <td>
<%--                <asp:DropDownList ID="cmbAgencies" runat="server" Enabled="true" Visible="true">
                </asp:DropDownList>     --%>                              
                <asp:TextBox ID="txtSearchAgency" runat="server" Visible="true"></asp:TextBox>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lbl_version" runat="server" Text="<%# Resources.Messages.lbl_version %>"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtSearchVersion" onkeydown = "return (event.keyCode!=13);" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:Label ID="lbl_name" runat="server" Text="<%# Resources.Messages.lbl_name %>"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtSearchName" onkeydown = "return (event.keyCode!=13);" runat="server"></asp:TextBox>
            </td>
            <td rowspan="2">
                <asp:Button ID="btnSearch" runat="server" 
                    Text="<%# Resources.Messages.lbl_send %>" onclick="btnSearch_Click" />
            </td>
        </tr>
    </table>
</div>

