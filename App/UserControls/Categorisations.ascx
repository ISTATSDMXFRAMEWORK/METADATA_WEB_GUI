<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Categorisations.ascx.cs" Inherits="ISTATRegistry.UserControls.Categorisations" %>
<%@ Register Namespace="ISTATRegistry.Classes" TagPrefix="iup" Assembly="IstatRegistry" %>
<%@ Register Src="ArtefactDelete.ascx" TagName="ArtefactDelete" TagPrefix="uc1" %>


<asp:Panel ID="pnlAll" runat="server">
    <iup:IstatUpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table width="740px">
                <tr>
                    <td width="50%">
                        <div style="overflow: auto; width: 370px; height: 400px; border: solid 1px #999; -moz-border-radius: 4px 4px 0 0; -webkit-border-radius: 4px 4px 0 0; margin-left: 10px;">
                            <asp:GridView
                                ID="gvCategorisations"
                                runat="server"
                                CssClass="Grid"
                                AllowPaging="True"
                                PagerSettings-Mode="NumericFirstLast"
                                PagerSettings-FirstPageText="<%= Resources.Messages.btn_goto_first %>"
                                PagerSettings-LastPageText="<%= Resources.Messages.btn_goto_last %>"
                                AutoGenerateColumns="False"
                                PagerSettings-Position="TopAndBottom">
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
                                </Columns>

                                <HeaderStyle CssClass="hs" />
                                <RowStyle CssClass="rs" />
                                <AlternatingRowStyle CssClass="ars" />
                                <PagerStyle CssClass="pgr"></PagerStyle>

                            </asp:GridView>
                            <asp:ImageButton CssClass="margin105" ID="btnAddCategorisation" ImageUrl="../images/Add.png" runat="server" AlternateText="<%$ Resources: Messages, lbl_add_categorisation %>" OnClick="btnAddCategorisation_Click" />
                        </div>
                    </td>
                    <td width="50%">
                        <asp:Panel ID="pnlAdd" runat="server" Visible="false">
                            <div style="width: 370px; height: 400px; border: solid 1px #999; -moz-border-radius: 4px 4px 0 0; -webkit-border-radius: 4px 4px 0 0; margin-left: 10px;">
                                <table class="margin105">
                                    <tr>
                                        <td width="45%">
                                            <asp:Label ID="lblCategoryScheme" runat="server" Text="<%$ Resources: Messages, lbl_category_scheme %>"></asp:Label></td>
                                        <td width="55%">
                                            <asp:DropDownList ID="cmbCategorySchemes" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbCategorySchemes_SelectedIndexChanged"></asp:DropDownList></td>
                                    </tr>
                                </table>

                                <div style="padding:10px 5px 10px 5px; overflow: auto; width: 330px; height: 300px; border: solid 1px #999; -moz-border-radius: 4px 4px 0 0; -webkit-border-radius: 4px 4px 0 0; margin-left: 10px;">
                                    <asp:TreeView ID="tvCategory" CssClass="AspNet-TreeView" runat="server" HoverNodeStyle-ForeColor="#FF3300" ImageSet="Arrows" OnSelectedNodeChanged="tvCategory_SelectedNodeChanged">
                                        <HoverNodeStyle ForeColor="#FF3300" />
                                        <NodeStyle ForeColor="Black" />
                                        <SelectedNodeStyle BackColor="#DDDDDD" Font-Bold="True" />
                                    </asp:TreeView>
                                </div>
                                <asp:Button CssClass="margin105" ID="btnSaveCategorisation" runat="server" Text="<%$ Resources: Messages, lbl_add_categorisation %>" OnClick="btnSaveCategorisation_Click" Enabled="false"/>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </iup:IstatUpdatePanel>
</asp:Panel>
<asp:Panel ID="pnlArtefactMustBeSaved" runat="server">
    <asp:Label ID="Label1" runat="server" Text="<%$ Resources: Messages, lbl_save_the_artefact %>"></asp:Label>
</asp:Panel>
