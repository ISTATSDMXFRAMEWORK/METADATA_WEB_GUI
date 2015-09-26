<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddText.ascx.cs" Inherits="ISTATRegistry.UserControls.AddText" %>
<asp:Panel ID="pnlEditText" runat="server">
    <asp:GridView ID="gvText" runat="server" AutoGenerateColumns="False" OnRowDataBound="gvText_RowDataBound"
        OnRowCancelingEdit="gvText_RowCancelingEdit" OnRowEditing="gvText_RowEditing"
        OnRowDeleting="gvText_RowDeleting" CssClass="Grid">
        <Columns>
            <asp:TemplateField HeaderStyle-Font-Size="Small" HeaderText="<%$ Resources:Messages, lbl_add_text_first_column %>">
                <ItemTemplate>
                    <asp:Label ID="lblLanguage" runat="server" Font-Size="Small" Text='<%# Bind("Locale") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Width="15%" />
            </asp:TemplateField>
            <asp:TemplateField HeaderStyle-Font-Size="Small" HeaderText="<%$ Resources:Messages, lbl_add_text_second_column %>">
                <ItemTemplate>
                    <asp:Label ID="lblText" CssClass="textBlockWrap" Font-Size="Small" runat="server"
                        Text='<%# Bind("Value") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:ImageButton ID="ibDelete" runat="server" CausesValidation="False" CommandName="Delete"
                        Text="<%# Resources.Messages.lbl_delete %>" ImageUrl="../images/Delete_mini.png"
                        ToolTip="Delete" />
                </ItemTemplate>
                <HeaderStyle Width="5px" />
            </asp:TemplateField>
        </Columns>
        <HeaderStyle CssClass="ui-widget-header" />
    </asp:GridView>
    <a href='javascript: openP("dialog-form<%=_sessionName %>",900)' title="<%= Resources.Messages.lbl_add_text %>">
        <img src="./images/Add.png" border="0" alt="<%= Resources.Messages.lbl_add_text %>" />
    </a>
</asp:Panel>
<asp:Panel ID="pnlViewText" runat="server" Visible="false">
    <asp:TextBox ID="txtViewText" CssClass="noScalableTextArea" runat="server" TextMode="MultiLine"
        Height="70px" Enabled="false"></asp:TextBox>
</asp:Panel>
<div id="dialog-form<%=_sessionName %>" class="popup_block">
    <asp:Label ID="lblTitle" runat="server" Text="<%= Resources.Messages.lbl_add_text %>"
        CssClass="PageTitle"></asp:Label>
    <hr style="width: 100%;" />
    <table style="width: 100%">
        <tr>
            <td style="font-size: 'small'">
                <asp:Label ID="lblLanguage" runat="server" Font-Size="Small" Text="<%# '*' + Resources.Messages.lbl_lang+':' %>"></asp:Label>
            </td>
            <td>
                <asp:DropDownList Font-Size="Small" ID="cmbLanguage" runat="server" Width="200px">
                </asp:DropDownList>
                <asp:DropDownList ID="cmbAnnotationValue" Visible="false" runat="server" Width="300px"
                    Style="margin-left: 10px" AutoPostBack="True" 
                    onselectedindexchanged="cmbAnnotationValue_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblText" runat="server" Font-Size="Small" Text="<%# Resources.Messages.lbl_text+':' %>"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtText" Font-Size="Small" runat="server" TextMode="MultiLine" CssClass="noScalableTextArea"
                    Height="300"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="btnAdd" runat="server" Font-Size="Small" Text="<%# Resources.Messages.lbl_add %>"
                    ToolTip="<%# Resources.Messages.lbl_add_text %>" OnClick="btnAdd_Click" />
            </td>
        </tr>
    </table>
</div>
<script type="text/javascript">

    $("#<%= cmbAnnotationValue.ClientID %>").change(function (e) {
        window.onbeforeunload = null;
    });

</script>