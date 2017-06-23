<%@ Page Language="VB" AutoEventWireup="false" ValidateRequest="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .style1
        {
            width: 132px;
        }
        .style2
        {
            width: 109px;
        }
        .style3
        {
            width: 166px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%;">
            <tr>
                <td>
                    <img alt="Istat SDMX Registry" src="img/istat.gif" height="35" width="150" border="0" />
                </td>
                <td>
                    <asp:Label ID="lblTitolo" runat="server" Font-Size="XX-Large" Text="SDMX Registry"></asp:Label>
                </td>
            </tr>
        </table>
        &nbsp;
        <table style="width: 100%;">
            <tr>
                <td class="style3" width="90%"><asp:Label ID="lblError" runat="server" Text=""></asp:Label></td>
                <td width="10%"><asp:DropDownList ID="ddlLang" runat="server" style="margin-left: 583px">
                    <asp:ListItem Value="it">it-Italian</asp:ListItem>
                    <asp:ListItem Value="en">en-English</asp:ListItem>
                    </asp:DropDownList></td>
            </tr>
        </table>
        &nbsp;
        <table style="width: 100%;">
            <tr>
                <td class="style1" style="border-style: solid; border-width: 1px" align="right">
                    <table border="1" bgcolor="#990000">
                        <tr>
                            <td colspan="2"><asp:Label ID="lblAuth" runat="server" Text="Autenticazione" 
                                    ForeColor="White"></asp:Label></td>
                        </tr>
                        <tr>
                            <td><asp:Label ID="lblUid" runat="server" Text="Utente" ForeColor="White"></asp:Label></td>
                            <td class="style2"><asp:TextBox ID="txtUid" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td><asp:Label ID="lblPwd" runat="server" Text="Password" ForeColor="White"></asp:Label></td>
                            <td class="style2"><asp:TextBox ID="txtPwd" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center"><asp:Button ID="btnSend" runat="server" Text="Invia" /></td>
                        </tr>
                    </table>
                </td>
                <td style="border-style: solid; border-width: 1px">
                    
                                <asp:TextBox ID="txtResponse" runat="server" Columns="90" Height="103px" 
                                    TextMode="MultiLine" Width="633px"></asp:TextBox>
                    
                </td>
            </tr>
            <tr>
                <td class="style1" style="border-style: solid; border-width: 1px" valign="top">
                    <table border="1">
                        <tr>
                            <td><asp:LinkButton ID="lnkBtnConceptScheme" runat="server">Concept Schemes</asp:LinkButton></td>
                        </tr>
                        <tr>
                            <td><asp:LinkButton ID="lnkBtnCodeLists" runat="server">Codelists</asp:LinkButton></td>
                        </tr>
                    </table>
                </td>
                <td style="border-style: solid; border-width: 1px">
                                <asp:GridView ID="grdCodeLists" runat="server" 
                        OnRowCommand="selectCL" BackColor="White" 
                                    BorderColor="#CC9966" BorderWidth="1px" CellPadding="4" 
                                    AutoGenerateColumns="False" BorderStyle="None" 
                        AllowPaging="True">
                                    <RowStyle BackColor="White" ForeColor="#330099" />
                                    <Columns>
                                        <asp:BoundField DataField="artefact" HeaderText="artefact" Visible="False" />
                                        <asp:BoundField DataField="id" HeaderText="id" />
                                        <asp:BoundField DataField="name" HeaderText="name" />
                                        <asp:BoundField DataField="agencyID" HeaderText="agency" />
                                        <asp:BoundField DataField="version" HeaderText="version" />
                                        <asp:ButtonField Text="SDMX" ButtonType="Button" CommandName="SDMX" />
                                    </Columns>
                                    <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
                                    <PagerStyle BackColor="#FFFFCC" ForeColor="#330099" 
                                        HorizontalAlign="Center" />
                                    <SelectedRowStyle BackColor="#FFCC66" ForeColor="#663399" Font-Bold="True" />
                                    <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="#FFFFCC" />
                                </asp:GridView>
                            </td>
            </tr>
        </table>
        
    </div>
    </form>
</body>
</html>
