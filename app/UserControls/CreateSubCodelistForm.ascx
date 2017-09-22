<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CreateSubCodelistForm.ascx.cs" Inherits="ISTATRegistry.UserControls.CreateSubCodelistForm" %>


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
                <asp:TextBox ID="txtSearchValue" runat="server" onkeydown = "return (event.keyCode!=13);"></asp:TextBox>
            </td>
            <td>    
                <asp:DropDownList ID="ddlSearchField" runat="server">
                    <asp:ListItem Text="Cod" Value="0" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Name" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Parent Code" Value="3"></asp:ListItem>
                </asp:DropDownList>                       
            </td>
            <td><asp:DropDownList ID="ddlSearchType" runat="server">
                <asp:ListItem Text="Equals" Value="0" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Like" Value="1"></asp:ListItem>
                </asp:DropDownList></td>
            <td style="align-content:flex-end">
                <asp:Button ID="btnSearch" runat="server" Text="<%# Resources.Messages.lbl_send %>" OnClick="btnSearch_Click" />
            </td>
        </tr>
    </table>
</div>
<br />
<div id="MainForm">
    <table>
        <tr>
            <td><div><asp:CheckBox ID="chkParentCode" runat="server" Text=" Import Parent Code not selected"/></div></td>
        </tr>
        <tr>
            <td>
                <div style="height: 340px;width:340px; overflow-x:scroll;"><asp:ListBox ID="lbCodesLeft" runat="server" SelectionMode="Multiple" Height="340px" Width="330px"></asp:ListBox></div>
            </td>
            <td>
                <asp:Button ID="btnAddCodes" runat="server" Text="Add" OnClick="btnAddCodes_Click"/><br />
                <asp:Button ID="btnRemoveCodes" runat="server" Text="Remove" OnClick="btnRemoveCodes_Click"/>
            </td>
            <td>
                <div style="height: 340px; width:340px; overflow-x: scroll;"><asp:ListBox ID="lbCodesSelected" runat="server" SelectionMode="Multiple" Height="340px" Width="330px"></asp:ListBox></div>
            </td>
        </tr>
        </table>
    </div>