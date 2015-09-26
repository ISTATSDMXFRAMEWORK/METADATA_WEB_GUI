<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="ContentConstraint.aspx.cs" Inherits="ISTATRegistry.ContentConstraint" %>

<%@ Register Src="UserControls/FileDownload3.ascx" TagName="FileDownload3" TagPrefix="uc3" %>
<%@ Register Src="UserControls/ArtefactDelete.ascx" TagName="ArtefactDelete" TagPrefix="uc1" %>
<%@ Register Src="UserControls/SearchBar.ascx" TagName="SearchBar" TagPrefix="uc2" %>
<%@ Register Namespace= "ISTATRegistry.Classes" TagPrefix="iup" Assembly="IstatRegistry" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h3 class="PageTitle">
        <%= Resources.Messages.lbl_content_constraint%></h3>
    <hr style="width: 100%" />
    <iup:IstatUpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc2:SearchBar ID="SearchBar1" runat="server" />
            <asp:GridView ID="gridView" runat="server" CssClass="Grid" AllowPaging="True" PagerSettings-Mode="NumericFirstLast"
                PagerSettings-FirstPageText="<%# Resources.Messages.btn_goto_first %>" PagerSettings-LastPageText="<%# Resources.Messages.btn_goto_last %>"
                AllowSorting="True" OnSorted="OnSorted" OnSorting="OnSorting" AutoGenerateColumns="False"
                OnPageIndexChanging="OnPageIndexChanging" OnRowCommand="OnRowCommand" OnRowCreated="OnRowCreated"
                PagerSettings-Position="TopAndBottom" 
                onrowdatabound="gridView_RowDataBound">
                <Columns>
                    <asp:TemplateField HeaderText="ID" SortExpression="ID">
                        <ItemTemplate>
                            <asp:Label ID="lblID" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="190px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Agency" SortExpression="Agency">
                        <ItemTemplate>
                            <asp:Label ID="lblAgency" runat="server" Text='<%# Bind("Agency") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="70px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Version" SortExpression="Version">
                        <ItemTemplate>
                            <asp:Label ID="lblVersion" runat="server" Text='<%# Bind("Version") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="50px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Name" SortExpression="Name">
                        <ItemTemplate>
                            <asp:Label ID="lblName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="310px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="IsFinal" SortExpression="IsFinal">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkIsFinal" runat="server" Enabled="false" Checked='<%# Bind("IsFinal") %>' />
                        </ItemTemplate>
                        <HeaderStyle Width="50px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="LocalID" HeaderText="LocalID" SortExpression="LocalID"
                        Visible="False" />
                    <asp:CommandField EditText="Edit" HeaderText="View/Edit" ShowEditButton="True" Visible="False" />
                    <asp:TemplateField HeaderText="" ShowHeader="False">
                        <ItemTemplate>
                            <uc1:ArtefactDelete ID="ArtDelete" runat="server" ucID='<%# Eval("ID") %>' ucAgency='<%# Eval("Agency") %>'
                                ucVersion='<%# Eval("Version") %>' ucArtefactType='ContentConstraint' />
                        </ItemTemplate>
                        <HeaderStyle Width="50px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="" ShowHeader="False">
                        <ItemTemplate>
                            <asp:ImageButton ID="btmDattail" runat="server" CausesValidation="False" CommandName="Details"
                                CommandArgument="<%# Container.DataItemIndex %>" ImageUrl="~/images/Details2.png"
                                ToolTip="View details" />
                        </ItemTemplate>
                        <HeaderStyle Width="50px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="">
                        <ItemTemplate>
                            <uc3:FileDownload3 ID="FileDownload3" runat="server" ucID='<%# Eval("ID") %>' ucAgency='<%# Eval("Agency") %>'
                                ucVersion='<%# Eval("Version") %>' ucArtefactType='ContentConstraint' />
                        </ItemTemplate>
                        <HeaderStyle Width="50px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="hs" />
                <RowStyle CssClass="rs" />
                <AlternatingRowStyle CssClass="ars" />
                <PagerStyle CssClass="pgr"></PagerStyle>
            </asp:GridView>
            <asp:Label ID="lblNumberOfRows" runat="server" Text="<%$ Resources:Messages, lbl_number_of_rows %>"></asp:Label>
            <asp:TextBox ID="txtNumberOfRows" runat="server" Style="text-align: center" onkeydown="return (event.keyCode!=13);"
                OnTextChanged="txtNumberOfRows_TextChanged" Width="40px"></asp:TextBox>&nbsp;<asp:Button
                    ID="btnChangePaging" runat="server" Text="<%$ Resources:Messages, lbl_change_number_of_rows %>"
                    OnClick="btnChangePaging_Click" />
            <br />
            <br />
            <asp:Button ID="btnAdd" runat="server" OnClick="btnAdd_Click" Text="<%$ Resources:Messages, btn_new_contentconstraint %>" />
        </ContentTemplate>
    </iup:IstatUpdatePanel>
</asp:Content>
