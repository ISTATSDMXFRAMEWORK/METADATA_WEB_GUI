<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="404.aspx.cs" Inherits="ISTATRegistry.ErrorPages._404" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="padding-left: 25px; padding-right: 25px; padding-top: 35px; font-weight: bold;
        font-size: small">
        <p>
            <p>
                Page Not Found - Error 404</p>
            <br />
            The request page not exist on our website!
            <br />
            <br />
            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/default.aspx">Return to the Home Page</asp:HyperLink>
        </p>
    </div>
</asp:Content>
