<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MultipleDownload.ascx.cs" Inherits="ISTATRegistry.UserControls.MultipleDownload" %>
<asp:Label ID="lblArtefactType" Text="" runat="server" Visible="false" />

<asp:Panel ID="pnlDownload" runat="server">
    <img src="./images/download2.png" id="imgDownloadButton" onclick="openAndReset('dfMultipleDownload')" style="border: none; cursor: pointer;" alt="<%= Resources.Messages.lbl_download_multiple_artefact %>" title="<%= Resources.Messages.lbl_download_multiple_artefact %>" />
</asp:Panel>

<div id="dfMultipleDownload" class="popup_block">

    <hr style="width: 100%" />

    <table class="tbNoBorder" cellpadding="6" style="width: 90%">
        <tr>
            <td colspan="2">
                <asp:Label ID="lblTitle" Text="<%$ Resources: Messages, lbl_download_title %>" runat="server" Visible="true" Style="font-weight: bold" />
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblExportType" runat="server" Text="<%$ Resources: Messages, lbl_export_type %>"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="cmbDownloadType" runat="server" CssClass="downType">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblStub" runat="server" Visible="false" Text="<%$ Resources: Messages, lbl_stub%>"></asp:Label>
            </td>
            <td>
                <asp:CheckBox ID="chkStub" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblIncludeCodeListAndConceptScheme" runat="server" Text="<%$ Resources: Messages, lbl_include_codelist_and_conceptscheme%>" Visible="false"></asp:Label>
            </td>
            <td>
                <asp:CheckBox ID="chkExportCodeListAndConcept" runat="server" Checked="false" Visible="false" />
            </td>
        </tr>
        <!-- DOTSTAT -->
        <tr>
            <td>
                <asp:Label ID="lblServer" runat="server" Text="<%$ Resources:Messages,lbl_server%>" Style="display: none" CssClass="IRDotStat"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtServer" Text="" MaxLength="50" Width="150px" runat="server" Style="display: none" CssClass="IRDotStat" />
            </td>
        </tr>

        <tr>
            <td>
                <asp:Label ID="lblDirectory" runat="server" Text="<%$ Resources:Messages,lbl_directory%>" Style="display: none" CssClass="IRDotStat"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtDirectory" Text="" MaxLength="50" Width="150px" runat="server" Style="display: none" CssClass="IRDotStat" />
            </td>
        </tr>

        <tr>
            <td>
                <asp:Label ID="lblTheme" runat="server" Text="<%$ Resources:Messages,lbl_theme%>" Style="display: none" CssClass="IRDotStat"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtTheme" Text="" MaxLength="50" Width="150px" runat="server" Style="display: none" CssClass="IRDotStat" />
            </td>
        </tr>

        <tr>
            <td>
                <asp:Label ID="lblContactName" runat="server" Text="<%$ Resources:Messages,lbl_contactname%>" Style="display: none" CssClass="IRDotStat"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtContactName" Text="" MaxLength="50" Width="150px" runat="server" Style="display: none" CssClass="IRDotStat" />
            </td>
        </tr>

        <tr>
            <td>
                <asp:Label ID="lblContactDirection" runat="server" Text="<%$ Resources:Messages,lbl_contactdirection%>" Style="display: none" CssClass="IRDotStat"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtContactDirection" Text="" MaxLength="200" Width="150px" runat="server" Style="display: none" CssClass="IRDotStat" />
            </td>
        </tr>

        <tr>
            <td>
                <asp:Label ID="lblContactEMail" runat="server" Text="<%$ Resources:Messages,lbl_contactemail%>" Style="display: none" CssClass="IRDotStat"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtContactEMail" Text="" MaxLength="100" Width="150px" runat="server" Style="display: none" CssClass="IRDotStat" />
            </td>
        </tr>

        <tr>
            <td>
                <asp:Label ID="lblSecurityUser" runat="server" Text="<%$ Resources:Messages,lbl_securityuser%>" Style="display: none" CssClass="IRDotStat"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtSecurityUser" Text="" MaxLength="100" Width="150px" runat="server" Style="display: none" CssClass="IRDotStat" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblSecurityDomain" runat="server" Text="<%$ Resources:Messages,lbl_securitydomain%>" Style="display: none" CssClass="IRDotStat"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtSecurityDomain" Text="" MaxLength="100" Width="150px" runat="server" Style="display: none" CssClass="IRDotStat" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblSeparator" runat="server" Text="<%$ Resources: Messages, lbl_use_this_separator%>" CssClass="IRCSV"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtSeparator" Text="" MaxLength="1" Width="15px" runat="server" CssClass="IRCSV" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblDelimiter" runat="server" Text="<%$ Resources:Messages,lbl_text_delimiter%>" CssClass="IRCSV"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtDelimiter" Text="" MaxLength="1" Width="15px" runat="server" CssClass="IRCSV" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblWSExport" runat="server" Text="<%$ Resources:Messages,lbl_wsExport%>" CssClass="WSExport"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="cmbWSExport" runat="server" CssClass="WSExport">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblWSUserName" runat="server" Text="<%$ Resources:Messages,lbl_WSUserName%>" CssClass="WSAutentication"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtWSUserName" Text="" MaxLength="50" Width="150px" runat="server" CssClass="WSAutentication" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblWSPassword" runat="server" Text="<%$ Resources:Messages,lbl_WSPassword%>" CssClass="WSAutentication"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtWSPassword" Text="" MaxLength="50" Width="150px" runat="server" CssClass="WSAutentication" TextMode="Password" />
            </td>
        </tr>

        <tr>
            <td></td>
            <td>
                <asp:Button ID="btnAuthentication" runat="server" CssClass="WSAutentication" OnClientClick="return false;" OnClick="btnDownload_Click" Text="<%$ Resources:Messages,btn_authentication%>" />
            </td>
        </tr>

    </table>
    <hr style="width: 100%" />
    <br />
    <div style="float: right">
        <center>
        <%--<asp:Button ID="btnDownload" runat="server" OnClick="btnDownload_Click" OnClientClick="$.unblockUI();" Text="<%# Resources.Messages.lbl_download %>" />--%>
        <asp:Button ID="btnAjaxDownload" runat="server" OnClientClick="return false;" CssClass="WSNewIdentity" OnClick="btnDownload_Click" Text="<%$ Resources:Messages,lbl_upload%>" />
        <asp:Button ID="btnMultipleDownload" runat="server" Text="<%$ Resources: Messages, btn_download%>" onclick="btnMultipleDownload_Click" />
        <asp:HiddenField ID="hdnWsSource" runat="server"></asp:HiddenField>

    </center>
    </div>

</div>
<script>

    $("#<%=btnMultipleDownload.ClientID %>").click(function () {

        $(".popup_block").css("z-index", 99999);
        //closePopup();
        //$.unblockUI();
    });


    function openAndReset(pName) {
        $(".downType").val(0).change();
        $(".popup_block").css("z-index", 19999);
        if ("<%=ucArtefactType%>".toUpperCase() == "DATAPROVIDERSCHEME" ||
            "<%=ucArtefactType%>".toUpperCase() == "DATACONSUMERSCHEME") {
        }
        $('#<%=btnAjaxDownload.ClientID %>').hide();

        openPopUp(pName);


    }

    $(document).ready(function () {
        $('.IRCSV').hide();
        $('.WSExport').hide();
        $('.WSAutentication').hide();
        $('.WSNewIdentity').hide();
        $('#<%=btnAjaxDownload.ClientID %>').hide();


        $('#<%=btnAjaxDownload.ClientID %>').unbind('click').click(function () {

            var items = [];
            $(".rs, .ars").each(function () {

                if($(this).find(".chkDown").children().is(':checked'))
                {
                    var item = {};
                    item.ID = $(this).find(".aiID").text();
                    item.Agency = $(this).find(".aiAgency").text();
                    item.Version = $(this).find(".aiVersion").text();
                    items.push(item);
                }
            });

            var jsonData = JSON.stringify({
                sourceEndPoint: $("#<%=hdnWsSource.ClientID %>").val(),
                            targetEndPoint: $("#<%=cmbWSExport.ClientID %>").val(),
                            artefactType: "<%=ucArtefactType%>",
                            lArtefacts: items
                        });
            $.ajax({
                type: "POST",
                url: "WebServices/IRWebService.asmx/ExportMultipleArtefact",
                data: jsonData,
                contentType: "application/json; charset=utf-8",
                dataType: "json", // dataType is json format
                success: OnExportSuccess,
                error: OnExportError
            });

        
        });

        function OnExportSuccess(data) {

            var items = JSON.parse(data.d);
            var msg = "";
            var head = "<span class='PageTitle'><%=GetGlobalResourceObject("Messages", "lbl_importedArtefact") %></span><br>";


            $.each(items, function (index, item) {
                msg += "<br>" + item.substring(item.lastIndexOf(".", item.indexOf("=", 0)) + 1);   //Codelist=ESTAT:CL_INSTR_ASSET(1.0)
            });

            msg = '<div style="overflow: auto; width: 100%; height: 380px;">' + head + msg + "</div>";

            ShowDialog(msg, 800, 'IR Message');

        }
        function OnExportError(response) {
            jsonValue = jQuery.parseJSON(response.responseText);
            ShowDialog(jsonValue.Message, 300, 'IR Error');
        }

        $("#<%=cmbWSExport.ClientID %>").change(function () {
            // effettuo una chiamata Ajax per vedere se l'utente è autenticato all'export nel WS di destinazione

            //$.blockUI();

            var selectedItem = $(this).val();
            var cmb = $(this).children("option").first();

            if (cmb.val() == "")
                cmb.remove();

            $('#<%=btnAjaxDownload.ClientID %>').show();

            var jsonData = JSON.stringify({ endPointName: selectedItem });
            $.ajax({
                type: "POST",
                url: "WebServices/IRWebService.asmx/ISWSExportAuth",
                data: jsonData,
                contentType: "application/json; charset=utf-8",
                dataType: "json", // dataType is json format
                success: OnSuccess,
                error: OnErrorCall
            });

            function OnSuccess(response) {
                if (!$.parseJSON(response.d)) {
                    $('.WSAutentication').show();
                    $("#<%=txtWSUserName.ClientID %>").val("");
                    $("#<%=txtWSPassword.ClientID %>").val("");
                    $("#<%=btnAjaxDownload.ClientID %>").hide();
                }
                else {
                    $("#<%=btnAjaxDownload.ClientID %>").show();
                    $('.WSAutentication').hide();
                }
            }
            function OnErrorCall(xhr) {
                var err = eval("(" + xhr.responseText + ")");
                ShowDialog(err, 300, 'IR Error');
                uscita = 1;
            }
        });


        $("#<%=btnAuthentication.ClientID %>").click(function () {
            //1. eseguo una chiamata ajax per vedere se le credenziali dell'utente sono corrette
            //      se sono corrette nascondo il pannello di autenticazione e visualizzo il pannello con new id, agency ecc..
            //      se non sono corrette visualizzo un messaggio di errore


            var vuserName = $("#<%=txtWSUserName.ClientID %>").val();
            var vpassword = $("#<%=txtWSPassword.ClientID %>").val();
            var vendpoint = $("#<%=cmbWSExport.ClientID %>").val();

            var jsonData = JSON.stringify({
                userName: vuserName,
                password: vpassword,
                endpoint: vendpoint
            });
            $.ajax({
                type: "POST",
                url: "WebServices/IRWebService.asmx/WSAutenticateUser",
                data: jsonData,
                contentType: "application/json; charset=utf-8",
                dataType: "json", // dataType is json format
                success: OnAuthSuccess,
                error: OnAuthErrorCall
            });

            function OnAuthSuccess(response) {
                if ($.parseJSON(response.d)) {
                    $('.WSAutentication').hide();
                    $("#<%=btnAjaxDownload.ClientID %>").show();

                }
                else {
                    ShowDialog("Credenziali non corrette", 300, 'IR Error');
                }
            }

            function OnAuthErrorCall(response) {
                jsonValue = jQuery.parseJSON(response.responseText);
                ShowDialog(jsonValue.Message, 300, 'IR Error');
            }

            //AutenticateUser(userName, password, endpoint)

        });


        $("#<%=cmbDownloadType.ClientID %>").change(function () {
            var selectedItem = $(this).val();
            $('.WSNewIdentity').hide();

            if (selectedItem == "CSV") {
                $('.IRCSV').show();
            }
            else {
                $('.IRCSV').hide();
            }

            if (selectedItem == ".STAT_DSD" || selectedItem == ".STAT_All") {
                $("#<%=lblIncludeCodeListAndConceptScheme.ClientID %>, #<%=chkExportCodeListAndConcept.ClientID %>").hide();
                $('.IRDotStat').show();
            }
            else
                $('.IRDotStat').hide();

            if (selectedItem == "SDMX20" || selectedItem == "SDMX21" || selectedItem == "RTF") {
                if (selectedItem == "RTF")
                    $("#<%=lblIncludeCodeListAndConceptScheme.ClientID %>, #<%=chkExportCodeListAndConcept.ClientID %>").hide();
                else
                    $("#<%=lblIncludeCodeListAndConceptScheme.ClientID %>, #<%=chkExportCodeListAndConcept.ClientID %>").show();
            }

            if (selectedItem == "Web Service") {

                $(document).ajaxStart($.blockUI).ajaxStop($.unblockUI);

                $("#<%=lblIncludeCodeListAndConceptScheme.ClientID %>, #<%=chkExportCodeListAndConcept.ClientID %>").hide();
                $('.WSExport').show();
                $('#<%=btnMultipleDownload.ClientID %>').hide();

                if ($("#<%=cmbWSExport.ClientID %>").children("option").first().val() != "") {
                    $("#<%=cmbWSExport.ClientID %>").prepend($('<option>', { value: '', text: '' }));
                    $("#<%=cmbWSExport.ClientID %>").val("0");
                }

                if ('<%=ucArtefactType%>'.toUpperCase() == 'KEYFAMILY')
                    $("#<%=lblIncludeCodeListAndConceptScheme.ClientID %>, #<%=chkExportCodeListAndConcept.ClientID %>").show();
            }
            else {
                $('.WSExport').hide();
                $('.WSAutentication').hide();
                $('#<%=btnAjaxDownload.ClientID %>').hide();
                $('#<%=btnMultipleDownload.ClientID %>').show();

            }

        });

        $('#<%= txtSeparator.ClientID %>').keydown(function (event) {
            var separator = $(this).val();
            if (separator.length == 1 && event.keyCode != 8) {
                return false;
            }
        });
    });








</script>
