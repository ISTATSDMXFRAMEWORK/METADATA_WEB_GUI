<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="CategorySchemes.aspx.cs" Inherits="ISTATRegistry.categorySchemes" %>

<%@ Register Src="UserControls/FileDownload3.ascx" TagName="FileDownload3" TagPrefix="uc3" %>
<%@ Register src="UserControls/ArtefactDelete.ascx" tagname="ArtefactDelete" tagprefix="uc1" %>
<%@ Register src="UserControls/SearchBar.ascx" tagname="SearchBar" tagprefix="uc2" %>
<%@ Register Namespace= "ISTATRegistry.Classes" TagPrefix="iup" Assembly="IstatRegistry" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h3 class="PageTitle"><%= Resources.Messages.lbl_category_scheme %></h3>

    <hr style="width: 100%" />

    <iup:IstatUpdatePanel ID="UpdatePanel1" runat="server" >
        <ContentTemplate>
            
            <uc2:SearchBar ID="SearchBar1" runat="server" />
            <asp:Label ID="lblNumberOfTotalElements" runat="server" Text=""></asp:Label>
            <asp:GridView 
                ID="gridView" 
                runat="server" 
                CssClass="Grid" 

                AllowPaging="True"
                PagerSettings-Mode="NumericFirstLast"   
                PagerSettings-FirstPageText="<%# Resources.Messages.btn_goto_first %>"
                PagerSettings-LastPageText="<%# Resources.Messages.btn_goto_last %>"

                AllowSorting="True"
                OnSorted="OnSorted"
                OnSorting="OnSorting"
                
                AutoGenerateColumns="False"
                
                DataSourceID="ObjectDataSource1" 
                
                OnPageIndexChanging="OnPageIndexChanging" 
                OnRowCommand="OnRowCommand" 
                OnRowCreated="OnRowCreated"
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

                    <asp:BoundField DataField="LocalID" HeaderText="LocalID" SortExpression="LocalID" Visible="False" />
                    
                    <asp:CommandField EditText="Edit" HeaderText="View/Edit" ShowEditButton="True" Visible="False" />
                    
                    <asp:TemplateField HeaderText="" ShowHeader="False">
                        <ItemTemplate>
                            <uc1:ArtefactDelete 
                            ID="ArtDelete" 
                            runat="server" 
                            ucID='<%# Eval("ID") %>'
                            ucAgency='<%# Eval("Agency") %>' 
                            ucVersion='<%# Eval("Version") %>' 
                            ucArtefactType='CategoryScheme' />
                        </ItemTemplate>
                        <HeaderStyle Width="50px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="" ShowHeader="False">
                        <ItemTemplate>
                            <asp:ImageButton 
                            ID="btmDattail" 
                            runat="server" 
                            CausesValidation="False" 
                            CommandName="Details"
                            CommandArgument="<%# Container.DataItemIndex %>" 
                            ImageUrl="~/images/Details2.png"
                            ToolTip="View details" />
                        </ItemTemplate>
                        <HeaderStyle Width="50px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="">
                        <ItemTemplate>
                            <uc3:FileDownload3 
                            ID="FileDownload3" 
                            runat="server" 
                            ucID='<%# Eval("ID") %>' 
                            ucAgency='<%# Eval("Agency") %>' 
                            ucVersion='<%# Eval("Version") %>' 
                            ucArtefactType='CategoryScheme' />
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

            <asp:Label ID="lblNumberOfRows" runat="server" Text="<%# Resources.Messages.lbl_number_of_rows + ':'%>"></asp:Label>
             <asp:TextBox ID="txtNumberOfRows" runat="server" style="text-align:center" UseSubmitBehavior="false" AutoPostBack="false"
                Width="40px" onkeydown = "return (event.keyCode!=13);"></asp:TextBox>&nbsp;<asp:Button ID="btnChangePaging" runat="server" 
                Text="<%# Resources.Messages.lbl_change_number_of_rows%>" onclick="btnChangePaging_Click" />
            <br /><br />
            <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetCategorySchemeList" TypeName="ISTAT.EntityMapper.EntityMapper">
                <SelectParameters>
                    <asp:Parameter Name="sdmxObjects" Type="Object" />
                </SelectParameters>
            </asp:ObjectDataSource>

            <asp:TreeView ID="mainCategorySchemeThreeView" runat="server">
            </asp:TreeView>

            <asp:Button ID="btnAddCategoryScheme" runat="server" Text="<%# Resources.Messages.btn_new_categoryscheme%>" onclick="btnAddCategoryScheme_Click" />

        </ContentTemplate>
    </iup:IstatUpdatePanel>
</asp:Content>

