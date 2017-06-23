<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GetConcept.ascx.cs" Inherits="ISTATRegistry.UserControls.GetConcept" %>
<%@ Register Src="SearchBar.ascx" TagName="SearchBar" TagPrefix="uc1" %>

<a href='javascript: openP("df_Concept<%=ControlID %>",<%=PopUpWidth %>)' title="<%= Resources.Messages.lbl_concept %>">
    <img src="./images/GetObject.png" border="0" alt="<%= Resources.Messages.lbl_concept %>" /></a><asp:ImageButton OnClick="btnDeleteText_Click" ID="btnDeleteText" runat="server" Visible="false" ImageUrl="~/images/Delete_mini.png" />

<div id="df_Concept<%=ControlID %>" class="popup_block">

    <asp:Label ID="lblTitle" runat="server" Text="<%# Resources.Messages.lbl_select_concept %>" CssClass="PageTitle"></asp:Label>

    <div id="divBack">
        <asp:ImageButton ID="lbBack" runat="server" OnClick="lbBack_Click" ImageUrl="~/images/back.png" AlternateText="<%# Resources.Messages.lbl_back %>" />
    </div>

    <hr style="width: 100%" />

    <uc1:SearchBar ID="SearchBar1" runat="server" />
    <div style="overflow: auto; width: 100%; max-height: 350px; position: relative">
        <asp:GridView ID="gvConceptScheme" runat="server"
            AllowPaging="True"
            CssClass="Grid"
            OnPageIndexChanging="gvConceptScheme_PageIndexChanging"
            AutoGenerateColumns="False"
            OnRowCommand="gvConceptScheme_RowCommand"
            OnSelectedIndexChanged="gvConceptScheme_SelectedIndexChanged">
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
    <asp:Panel ID="pnlSearchConcept" runat="server" Visible="false">
        <asp:Panel ID="pnlFixed" runat="server">
            <img id="img1" src="./images/Search_off.png" border="0" />
        </asp:Panel>
        <table class="SearchClass">
            <tr>
                <td>
                    <asp:Label ID="lbl_id" runat="server" Text="<%$ Resources: Messages, lbl_id %>"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtSearchID" runat="server" onkeydown="return (event.keyCode!=13);"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbl_name" runat="server" Text="<%$ Resources: Messages, lbl_name %>"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtSearchName" onkeydown="return (event.keyCode!=13);" runat="server"></asp:TextBox>
                </td>
                <td rowspan="2">
                    <asp:Button ID="btnSearchConcept" runat="server"
                        Text="<%$ Resources: Messages, lbl_send %>" OnClick="btnSearchConcept_Click" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <div style="overflow: auto; width: 100%; max-height: 350px; position: relative">
        <asp:GridView
            ID="gvConcepts"
            runat="server"
            AllowPaging="True"
            CssClass="Grid"
            OnPageIndexChanging="gvConcepts_PageIndexChanging"
            AutoGenerateColumns="False"
            Visible="False"
            OnSelectedIndexChanged="gvConcepts_SelectedIndexChanged">
            <Columns>
                <asp:TemplateField HeaderText="ID" SortExpression="ID">
                    <ItemTemplate>
                        <asp:Label ID="lblConceptID" runat="server" Text='<%# Bind("Code") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="200px" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Name" SortExpression="Name">
                    <ItemTemplate>
                        <asp:Label ID="lblName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="400px" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Description" SortExpression="Description" Visible="False">
                    <ItemTemplate>
                        <asp:Label ID="lblParentCode" runat="server" Text='<%# Bind("Description") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="160px" />
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkSelect" runat="server" CausesValidation="False" CommandName="Select" Text="<%# Resources.Messages.lbl_select %>"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <HeaderStyle CssClass="hs" />
            <RowStyle CssClass="rs" />
            <AlternatingRowStyle CssClass="ars" />
            <PagerStyle CssClass="pgr"></PagerStyle>
        </asp:GridView>
    </div>
</div>

<asp:HiddenField ID="hdnViewMode" runat="server" />

