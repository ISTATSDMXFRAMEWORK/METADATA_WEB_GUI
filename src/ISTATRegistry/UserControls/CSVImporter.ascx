<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CSVImporter.ascx.cs" Inherits="ISTATRegistry.UserControls.CSVImporter" %>
<asp:ImageButton ID="imgImportCsv" Visible="true" runat="server"
    ToolTip="<%# Resources.Messages.btn_import_csv_file %>"
    AlternateText="<%# Resources.Messages.btn_import_csv_file %>"
    ImageUrl="~/images/csvImport.png" OnClientClick="javascript: openP('importCsv',500); return false;" />
<div id="importCsv" class="popup_block">
    <asp:Label ID="lblImportCsvTitle" runat="server" Text="<%$ Resources:Messages,lbl_import_csv %>" CssClass="PageTitle"></asp:Label>
    <br />
    <br />
    <div style="padding: 10px; overflow: auto; text-align: left; width: 450px; height: 250px; border: solid 1px #999; -moz-border-radius: 4px 4px 0 0; -webkit-border-radius: 4px 4px 0 0; margin-left: 10px;">
        <table class="tblForm">
            <tr>
                <td>
                    <asp:Label ID="lblCsvLanguage" runat="server" Text="<%$ Resources:Messages,lbl_language%>"></asp:Label></td>
                <td>
                    <asp:DropDownList ID="cmbLanguageForCsv" runat="server" AutoPostBack="False"></asp:DropDownList></td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblcsvFile" runat="server" Text="<%$ Resources:Messages,lbl_csv_file%>"></asp:Label></td>
                <td>
                    <asp:Panel ID="pnlFileUpload" runat="server">
                        <asp:FileUpload ID="csvFile" runat="server" />
                    </asp:Panel>
                    <asp:Panel ID="pnlTextUpload" runat="server" Visible="false">
                        <asp:Label ID="lblCsvFileName" runat="server" Text=""></asp:Label><asp:ImageButton ID="ibDeleteFileSelection" runat="server" ImageUrl="~/images/Delete_mini.png" OnClick="ibDeleteFileSelection_Click" />
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblHeaderRow" runat="server" Text="<%$ Resources:Messages, lbl_is_header_row%>"></asp:Label></td>
                <td>
                    <asp:CheckBox ID="chkHeaderRow" runat="server" Checked="true" /></td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblSeparator" runat="server" Text="<%$ Resources:Messages,lbl_used_separator%>"></asp:Label></td>
                <td>
                    <asp:TextBox ID="txtSeparator" Text="" runat="server" Width="15px" /></td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblTextDelimiter" runat="server" Text="<%$ Resources:Messages, lbl_text_delimiter%>"></asp:Label></td>
                <td>
                    <asp:TextBox ID="txtTextDelimiter" Text="" MaxLength="1" runat="server" Width="15px" /></td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblExtraFields" runat="server" Text="<%$ Resources:Messages, lbl_extra_fields%>"></asp:Label>
                </td>
                <td>
                    <asp:ListBox ID="lbExtraFields" runat="server" SelectionMode="Multiple" Height="50px" Width="150px">
                        <asp:ListItem Value="Description" Text="<%$ Resources:Messages,lbl_description%>"></asp:ListItem>
                        <asp:ListItem Value="Parent" Text="<%$ Resources:Messages,lbl_parent%>"></asp:ListItem>
                    </asp:ListBox><img alt="Clear" id="imgClear" visible="false" src="./images/Delete_mini.png" style="cursor: pointer; display: none" onclick="$('#<%=lbExtraFields.ClientID%>').val([]); $('#imgClear').hide(); " />
                </td>
            </tr>
        </table>
    </div>
    <br />
    <center>
        <asp:Button ID="btnPreview" runat="server" Text="<%$ Resources:Messages,btn_preview %>" OnClick="btnPreview_Click" />&nbsp;
        <asp:Button ID="btnImportFromCsv" runat="server" Text="<%$ Resources:Messages,btn_import_csv %>" OnClick="btnImportFromCsv_Click" />
    </center>
</div>

<div id="df-csvPreview" class="popup_block" style="text-align: left">
    <asp:Label ID="Label1" runat="server" Text="<%$ Resources:Messages,lbl_csv_preview %>" CssClass="PageTitle"></asp:Label>
    <br />
    <hr style="width: 100%;" />
    <br />
    <div style="overflow: auto; height: 380px;">
        <asp:GridView ID="gvCsvPreview" runat="server" CssClass="Grid"></asp:GridView>
    </div>
</div>

<div id="importCsvErrors" class="popup_block">
    <asp:Label ID="lblImportCsvErrorsTitle" runat="server" Text="IMPORT ERRORS" CssClass="PageTitle"></asp:Label>
    <hr style="width: 95%;" />
    <center>
        <div style="height: 100px; overflow: auto">
            <br />
            <asp:Label ID="lblImportCsvErrors" runat="server" Text=""></asp:Label>
        </div>
    </center>
    <span style="text-align: left"><%= Resources.Messages.lbl_wrong_lines %></span>
    <div style="height: 100px; width: 100%;">
        <asp:TextBox ID="lblImportCsvWrongLines" runat="server" Text="" TextMode="MultiLine" CssClass="noScalableTextArea" Width="100%" Height="80%"></asp:TextBox>
    </div>
</div>

<script>

    $('#<%= txtSeparator.ClientID %>').keydown(function (event) {
        var separator = $(this).val();
        if (separator.length == 1 && event.keyCode != 8) {
            return false;
        }
    });

    $('#<%=lbExtraFields.ClientID%>').click(function () {
        $('#imgClear').show();
    });

</script>
