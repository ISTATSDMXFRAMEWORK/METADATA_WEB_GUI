<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="Categorization.aspx.cs" Inherits="ISTATRegistry.categorization" EnableEventValidation="true" %>

<%@ Register Src="UserControls/FileDownload3.ascx" TagName="FileDownload3" TagPrefix="uc3" %>
<%@ Register Src="UserControls/ArtefactDelete.ascx" TagName="ArtefactDelete" TagPrefix="uc1" %>
<%@ Register Src="UserControls/SearchBar.ascx" TagName="SearchBar" TagPrefix="uc2" %>
<%@ Register Namespace= "ISTATRegistry.Classes" TagPrefix="iup" Assembly="IstatRegistry" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h3 class="PageTitle"><%= Resources.Messages.lbl_categorization %></h3>

    <hr style="width: 100%" />

    <iup:IstatUpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>            
            <uc2:SearchBar ID="SearchBar1" runat="server" />
            <div style= "overflow: auto">
                <asp:Label ID="lblNumberOfTotalElements" runat="server" Text=""></asp:Label>
                <asp:GridView 
                    ID="gridView" 
                    runat="server"
                    CssClass="Grid" 
                    AllowPaging="True"
              
                    PagerSettings-Mode="NumericFirstLast"   
                    PagerSettings-FirstPageText="<%= Resources.Messages.btn_goto_first %>"
                    PagerSettings-LastPageText="<%= Resources.Messages.btn_goto_last %>"

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

                        <asp:TemplateField HeaderText="Ref. Category Scheme" SortExpression="ID">
                            <ItemTemplate>
                                <asp:Label ID="lblCategorySchemeReference" runat="server" Text='<%# Bind("_categorySchemeReference") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="190px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ref. Category" SortExpression="Agency">
                            <ItemTemplate>
                                <asp:Label ID="lblCategory" runat="server" Text='<%# Bind("_categoryReference") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="70px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ref. Structure" SortExpression="Version">
                            <ItemTemplate>
                                <asp:Label ID="lblStructure" runat="server" Text='<%# Bind("_structureReference") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="50px" />
                        </asp:TemplateField>
                  
                        <asp:TemplateField HeaderText="IsFinal" SortExpression="IsFinal" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkIsFinal" runat="server" Enabled="false" Checked='<%# Bind("IsFinal") %>' />
                            </ItemTemplate>
                            <HeaderStyle Width="50px" />
                        </asp:TemplateField>

                        <%-- <asp:BoundField DataField="LocalID" HeaderText="LocalID" SortExpression="LocalID" Visible="False" /> --%>
                    
                        <%-- <asp:CommandField EditText="Edit" HeaderText="View/Edit" ShowEditButton="True" Visible="False" /> --%>
                    
                        <asp:TemplateField HeaderText="" ShowHeader="False">
                            <ItemTemplate>
                                <uc1:ArtefactDelete 
                                ID="ArtDelete" 
                                runat="server" 
                                ucID='<%# Eval("ID") %>'
                                ucAgency='<%# Eval("Agency") %>' 
                                ucVersion='<%# Eval("Version") %>' 
                                ucArtefactType='Categorization' />
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
                                ucArtefactType='Categorization' />
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
            </div>
            <asp:Label ID="lblNumberOfRows" runat="server" Text="<%# Resources.Messages.lbl_number_of_rows + ':'%>"></asp:Label>
             <asp:TextBox ID="txtNumberOfRows" runat="server" style="text-align:center"
                ontextchanged="txtNumberOfRows_TextChanged" onkeydown = "return (event.keyCode!=13);" 
                Width="40px"></asp:TextBox>&nbsp;<asp:Button ID="btnChangePaging" runat="server" 
                Text="<%# Resources.Messages.lbl_change_number_of_rows%>" onclick="btnChangePaging_Click" />
            <br /><br />
            <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetCategorizationList" TypeName="ISTAT.EntityMapper.EntityMapper">
                <SelectParameters>
                    <asp:Parameter Name="sdmxObjects" Type="Object" />
                </SelectParameters>
            </asp:ObjectDataSource>

            <asp:Button ID="btnAddCategorization" runat="server" Text="<%# Resources.Messages.btn_new_categorization %>" onclick="btnAddCategorization_Click" />

        </ContentTemplate>
    </iup:IstatUpdatePanel>
</asp:Content>
