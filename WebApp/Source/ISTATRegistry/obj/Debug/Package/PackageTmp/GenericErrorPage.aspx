<%@ Page Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="GenericErrorPage.aspx.cs"
    Inherits="ISTATRegistry.GenericErrorPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .werror
        {
            width: 100%;
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <br />
        <p style="Font-Size:medium;font-weight:bold"><b>An expected error has occurred on our website!</p>
        <br />
        <div id="accordion" class="werror">
            <h5>
                See Details</h5>
            <div>
                <asp:Panel ID="pnlInnerException" runat="server" Visible="false">
                    <p>
                        Inner Error Message:<br />
                        <asp:Label ID="innerMessage" runat="server" Font-Bold="true" Font-Size="small" /><br />
                    </p>
                    <pre>
                <asp:Label ID="innerTrace" runat="server" />
                </asp:Panel>
                </pre>
                <p>
                    Error Message:<br />
                    <asp:Label ID="exMessage" runat="server" Font-Bold="true" Font-Size="small" />
                </p>
                <pre>
                <asp:Label ID="exTrace" runat="server" Visible="true" Font-Size="small" />
            </pre>
            </div>
        </div>
        <br />
        Return to the <a href='Default.aspx'>Default Page</a>
    </div>
</asp:Content>
