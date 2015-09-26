<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="Oops.aspx.cs" Inherits="ISTATRegistry.ErrorPages.Oops" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="padding-left: 25px; padding-right: 25px; padding-top: 35px; font-weight: bold;
        font-size: small">
        <p>
            <p>
                An Error Has Occurred</p>
            <br />
            An expected error has occurred on our website!
            <br />
            <br />
            <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/default.aspx">Return to the Home Page</asp:HyperLink>
        </p>
    </div>
</asp:Content>
