<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ControlAnnotations.ascx.cs"
    Inherits="ISTATRegistry.UserControls.ControlAnnotations" %>
<%@ Register Src="AddText.ascx" TagName="AddText" TagPrefix="uc2" %>
<!-- Start Annotation container -->
<div id="control-annotation">
    <asp:GridView ID="gridView" runat="server" AutoGenerateColumns="False" CssClass="Grid"
        AllowPaging="True" PagerSettings-Mode="NumericFirstLast" PagerSettings-FirstPageText="<%# Resources.Messages.btn_goto_first %>"
        PagerSettings-LastPageText="<%# Resources.Messages.btn_goto_last %>" OnPageIndexChanging="OnPageIndexChanging"
        OnRowCommand="OnRowCommand" OnRowUpdating="gridView_RowUpdating" OnRowDeleting="gridView_RowDeleting"
        OnRowDataBound="gridView_RowDataBound">
        <Columns>
            <%--No.--%>
            <asp:TemplateField HeaderText="No.">
                <ItemTemplate>
                    <%# Container.DataItemIndex + 1 %>
                </ItemTemplate>
            </asp:TemplateField>
            <%--ID--%>
            <asp:TemplateField HeaderText="ID" SortExpression="ID">
                <ItemTemplate>
                    <asp:Label ID="lbl_id" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Width="200px" />
            </asp:TemplateField>
            <%--Name--%>
            <asp:TemplateField HeaderText="Title" SortExpression="Title">
                <ItemTemplate>
                    <asp:Label ID="lbl_name" runat="server" Text='<%# Bind("Title") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Width="400px" />
            </asp:TemplateField>
            <%--Type--%>
            <asp:TemplateField HeaderText="Type" SortExpression="Type">
                <ItemTemplate>
                    <asp:Label ID="lbl_type" runat="server" Text='<%# Bind("Type") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Width="160px" />
            </asp:TemplateField>
            <%--Url--%>
            <asp:TemplateField HeaderText="URI" SortExpression="URI">
                <ItemTemplate>
                    <asp:Label ID="lbl_url" runat="server" Text='<%# Bind("Url") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Width="160px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="" ShowHeader="False">
                <ItemTemplate>
                    <asp:ImageButton ID="img_update" runat="server" CausesValidation="False" CommandName="UPDATE"
                        CommandArgument="<%# Container.DataItemIndex %>" ImageUrl='<%#"~/images/"+_EditImage %>'
                        ToolTip="VIEW/UPDATE" />
                </ItemTemplate>
                <HeaderStyle Width="50px" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="" ShowHeader="False">
                <ItemTemplate>
                    <asp:ImageButton ID="img_delete" runat="server" CausesValidation="False" CommandName="DELETE"
                        CommandArgument="<%# Container.DataItemIndex %>" ImageUrl="~/images/Delete_mini.png"
                        ToolTip="DELETE" />
                </ItemTemplate>
                <HeaderStyle Width="50px" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
        </Columns>
        <HeaderStyle CssClass="ui-widget-header" />
        <PagerStyle CssClass="pgr"></PagerStyle>
    </asp:GridView>
    <asp:Panel ID="pnlNewAnnotation" runat="server">
        <asp:ImageButton ID="btnNewAnnotation" runat="server" title="<%# Resources.Messages.lbl_add_annotation %>"
            ImageUrl="../images/Add.png" OnClick="btnNewAnnotation_Click" />
    </asp:Panel>
    <script type="text/javascript">
        $('#btmNewAnnotation').click(function () {
            $('#<%= txt_id_annotation.ClientID %>').val("");
            $('#<%= txt_title_annotation.ClientID %>').val("");
            $('#<%= txt_type_annotation.ClientID %>').val("");
            $('#<%= txt_url_annotation.ClientID %>').val("");
        });
    </script>
    <div id="<%=_dfAnnotation%><%=_sessionName %>" class="popup_block">
        <div style="height: 300px; overflow-y: scroll;">
            <asp:Label ID="lblTitle" runat="server" Text="<%# Resources.Messages.lbl_annotation %>"
                CssClass="PageTitle"></asp:Label>
            <hr style="width: 100%;" />
            <table class="tableForm">
                <tr>
                    <td width="20%">
                        <asp:Label ID="lbl_id_annotation" Font-Size="Small" runat="server" Text="<%# '*' + Resources.Messages.lbl_id + ':' %>"
                            CssClass="tdProperty"></asp:Label>
                    </td>
                    <td width="80%">
                        <asp:TextBox ID="txt_id_annotation" runat="server" Enabled="true" Width="300px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td width="20%">
                        <asp:Label ID="lbl_title_annotation" Font-Size="Small" runat="server" Text="<%# Resources.Messages.lbl_title + ':' %>"
                            CssClass="tdProperty"></asp:Label>
                    </td>
                    <td width="80%">
                        <asp:TextBox ID="txt_title_annotation" runat="server" Enabled="true" Width="300px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td width="20%">
                        <asp:Label ID="lbl_type_annotation" Font-Size="Small" runat="server" Text="<%# Resources.Messages.lbl_type + ':' %>"
                            CssClass="tdProperty"></asp:Label>
                    </td>
                    <td width="80%">
                        <asp:DropDownList ID="cmbAnnotationType" Visible="false" runat="server" Width="300px"
                            Style="margin-bottom: 5px" OnSelectedIndexChanged="cmbAnnotationType_SelectedIndexChanged"
                            AutoPostBack="True">
                        </asp:DropDownList>
                        <asp:TextBox ID="txt_type_annotation" runat="server" Enabled="true" Width="300px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td width="20%">
                        <asp:Label ID="lbl_url_annotation" Font-Size="Small" runat="server" Text="<%# Resources.Messages.lbl_uri + ':' %>"
                            CssClass="tdProperty"></asp:Label>
                    </td>
                    <td width="80%">
                        <asp:TextBox ID="txt_url_annotation" runat="server" Enabled="true" Width="300px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lbl_value_annotation" Font-Size="Small" runat="server" Text="<%# '*' + Resources.Messages.lbl_value + ':' %>"
                            CssClass="tdProperty"></asp:Label>
                    </td>
                    <td>
                        <uc2:AddText ID="AddText_value_annotation" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="btnAddNewAnnotation" Font-Size="Small" OnClick="OnNewClick" runat="server"
                            Text="<%# Resources.Messages.btn_add_annotation %>" />
                        <asp:Button ID="btnUpdateAnnotation" Font-Size="Small" OnClick="OnUpdateClick" runat="server"
                            Text="<%# Resources.Messages.btn_edit_annotation %>" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>
<!-- End Annotation container -->
<script type="text/javascript">

    $("#<%= cmbAnnotationType.ClientID %>").change(function (e) {
        window.onbeforeunload = null;
    });

</script>