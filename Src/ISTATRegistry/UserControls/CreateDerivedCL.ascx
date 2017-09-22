<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CreateDerivedCL.ascx.cs" Inherits="ISTATRegistry.UserControls.CreateDerivedCL" %>

<asp:Panel ID="pnlSearchButton" runat="server">
    <a id = "searchLink" onclick =  "$('#SearchPanel').toggle('slow', function(){ if ( $('#SearchPanel').css('display') == 'block') { $( '#imgSearch' ).attr( 'src', './images/Search_on.png' ); } else { $( '#imgSearch' ).attr( 'src', './images/Search_off.png' ); }}); return false;">
        <img id= "imgSearch" src="./images/Search_off.png" border="0" />
    </a>
</asp:Panel>

<asp:Panel ID="pnlFixed" runat="server" Visible="false">
    <img id= "img1" src="./images/Search_off.png" border="0" />
</asp:Panel>

<div id="SearchPanel">
    <table class="SearchClass">
        <tr>
            <td>
                <asp:Label ID="lbl_id" runat="server" Text="<%# Resources.Messages.lbl_id %>"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtSearchID" runat="server" onkeydown = "return (event.keyCode!=13);"></asp:TextBox>
            </td>
            <td>
                <asp:Label ID="lbl_name" runat="server" Text="<%# Resources.Messages.lbl_name %>"></asp:Label>
            </td>
            <td>                           
                <asp:TextBox ID="txtSearchName" onkeydown = "return (event.keyCode!=13);" runat="server" Visible="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lbl_parentcode" runat="server" Text="<%# Resources.Messages.lbl_parent_code %>"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtSearchParentCode" onkeydown = "return (event.keyCode!=13);" runat="server"></asp:TextBox>
            </td>
            <td></td>
            <td style="align-content:flex-end">
                <asp:Button ID="btnSearch" runat="server" Text="<%# Resources.Messages.lbl_send %>" onclick="btnSearch_Click" />
            </td>
        </tr>
    </table>
</div>
<br />

<asp:GridView ID="gvDerivedCL" runat="server"
                            Width="730px"
                            AllowSorting="False"
                            AllowPaging="True"
                            CssClass="Grid"
                            PagerSettings-Mode="NumericFirstLast"
                            OnPageIndexChanging="gvDerivedCL_PageIndexChanging"
                            PagerSettings-FirstPageText="<%# Resources.Messages.btn_goto_first %>"
                            PagerSettings-LastPageText="<%# Resources.Messages.btn_goto_last %>"
                            AutoGenerateColumns="False">
                            <Columns>
                                <asp:TemplateField HeaderText="No.">
                                    <ItemTemplate>
                                        <%# Container.DataItemIndex + 1 %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="ID" SortExpression="ID">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCLID" runat="server" Text='<%# Bind("Code") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="200px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCLName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="400px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Parent Code" SortExpression="ParentCode">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCLParentCode" runat="server" Text='<%# Bind("ParentCode") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="160px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="" ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:CheckBox
                                            ID="img_update"
                                            runat="server"
                                            CommandArgument="<%# Container.DataItemIndex %>"
                                             />
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

<asp:ImageButton ID="btnAddCodes" runat="server" ImageUrl="../images/Add.png" 
                ToolTip="<%# Resources.Messages.lbl_add_text %>" 
                Text="<%# Resources.Messages.lbl_add_text %>"
                OnClick="btnAddCodes_Click"/>
<br />
Codici selezionati
<br />
<asp:GridView ID="gvFiltered" runat="server"
                            Width="730px"
                            AllowSorting="False"
                            AllowPaging="True"
                            CssClass="Grid"
                            PagerSettings-Mode="NumericFirstLast"
                            OnPageIndexChanging="gvFiltered_PageIndexChanging"
                            PagerSettings-FirstPageText="<%# Resources.Messages.btn_goto_first %>"
                            PagerSettings-LastPageText="<%# Resources.Messages.btn_goto_last %>"
                            AutoGenerateColumns="False">
                            <Columns>
                                <asp:TemplateField HeaderText="No.">
                                    <ItemTemplate>
                                        <%# Container.DataItemIndex + 1 %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="ID">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCLID2" runat="server" Text='<%# Bind("Code") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="200px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCLName2" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="400px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Parent Code" SortExpression="ParentCode">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCLParentCode2" runat="server" Text='<%# Bind("ParentCode") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="160px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="" ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:ImageButton
                                            ID="img_cancel"
                                            runat="server"
                                            CommandArgument="<%# Container.DataItemIndex %>"
                                             />
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