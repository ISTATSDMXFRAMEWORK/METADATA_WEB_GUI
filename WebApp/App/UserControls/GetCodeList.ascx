<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GetCodeList.ascx.cs" Inherits="ISTATRegistry.UserControls.GetCodeList" %>
<%@ Register Src="SearchBar.ascx" TagName="SearchBar" TagPrefix="uc1" %>

<a href='javascript: openP("df_CodeList<%=ControlID %>",<%=PopUpWidth %>)' title="<%= Resources.Messages.lbl_codelist %>">
    <img src="./images/<%=IconFileName %>" border="0" alt="<%= Resources.Messages.lbl_codelist %>" />
</a>

<div id="df_CodeList<%=ControlID %>" class="popup_block">
    
    <asp:Label ID="lblTitle" runat="server" Text="<%# Resources.Messages.lbl_select_codelist %>" CssClass="PageTitle"></asp:Label>
    
    <hr style="width:100%;" />
    
    <uc1:SearchBar ID="SearchBar1" runat="server" />

    <asp:GridView 
        ID="gvCodelists" 
        runat="server"
        AllowPaging="True" 
        CssClass="Grid"
        OnPageIndexChanging="gvCodelists_PageIndexChanging" 
        AutoGenerateColumns="False"
        DataSourceID="ObjectDataSource1" 
        OnRowCommand="gvCodelists_RowCommand" 
        onselectedindexchanged="gvCodelists_SelectedIndexChanged">
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
            <asp:BoundField DataField="LocalID" HeaderText="LocalID" SortExpression="LocalID" Visible="False" />
            <asp:CommandField EditText="Edit" HeaderText="View/Edit" ShowEditButton="True" Visible="False" />
            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Select" Text="<%# Resources.Messages.lbl_select %>"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <HeaderStyle CssClass="hs" />
        <RowStyle CssClass="rs" />
        <AlternatingRowStyle CssClass="ars" />
        <PagerStyle CssClass="pgr"></PagerStyle>
    </asp:GridView>

</div>

<asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetCodeListList"
    TypeName="ISTAT.EntityMapper.EntityMapper">
    <SelectParameters>
        <asp:Parameter Name="sdmxObjects" Type="Object" />
    </SelectParameters>
</asp:ObjectDataSource>
