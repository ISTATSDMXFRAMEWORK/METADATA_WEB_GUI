<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HttpErrorPage.aspx.cs"
    Inherits="ISTATRegistry.HttpErrorPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ISTAT Registry Http Error Page</title>
    <link rel="stylesheet" href="./jquery/jquery-ui.css" />
    <script src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script src="jquery/jquery-ui.min.js" type="text/javascript"></script>
    <style>
        .wrapper
        {
            width: 1024px;
            font-size: small;
        }
    </style>
    <script>
        $(function () {
            $("#accordion").accordion({
                collapsible: true,
                active: false
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h2>
            ISTAT Registry Http Error Page</h2>
        <p><asp:Label ID="innerMessage" runat="server" Font-Size="medium" /></p>
        <div id="accordion" class="wrapper">
            <h5>
                Details</h5>
            <div>
                <asp:Panel ID="InnerErrorPanel" runat="server" Visible="false">
                    <pre>
                    <asp:Label ID="innerTrace" runat="server" />
                   </pre>
                </asp:Panel>
                            Error Message:<br />
                            <asp:Label ID="exMessage" runat="server" Font-Bold="true" Font-Size="small" />
                            <pre>
                  <asp:Label ID="exTrace" runat="server"/>
                </pre>
            </div>
        </div>
        <br />
        Return to the <a href='Default.aspx'>Default Page</a>
    </div>
    </form>
</body>
</html>
