<%@ Page Language="VB" ValidateRequest="false" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table style="width: 100%;">
                <tr>
                    <td>
                        <asp:Label ID="lblError" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
            &nbsp;&nbsp;&nbsp;
            <table style="width: 100%;">
                <tr>
                    <td>
                        <asp:TextBox ID="txtRequest" runat="server" Columns="100" Height="170px" 
                            TextMode="MultiLine" Width="656px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox ID="txtResponse" runat="server" Columns="100" Height="201px" 
                            TextMode="MultiLine" Width="655px"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Button ID="btnSendQuery" runat="server" Text="Send Query" />
    </form>
</body>
</html>
