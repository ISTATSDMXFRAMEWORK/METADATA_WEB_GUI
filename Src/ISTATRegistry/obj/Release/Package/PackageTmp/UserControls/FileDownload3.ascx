<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileDownload3.ascx.cs"
    Inherits="ISTATRegistry.UserControls.FileDownload3" %>

<div id="dialog-form<%=ucID.Replace( '@', '_' ) + '_' + ucAgency + '_' + ucVersion.Replace( '.', '_' )%>" class="popup_block">

    <asp:Label ID="lblArtefactID" runat="server" Text="<%# Resources.Messages.lbl_artefact+':' %>" CssClass="PageTitle3" EnableViewState="True"></asp:Label>
    <br />
    <br />
    <hr style="width: 100%" />

    <table class="tbNoBorder" cellpadding="6" style="width: 90%">
        <tr>
            <td colspan="2">
                <asp:Label ID="lblTitle" Text="<%# Resources.Messages.lbl_download_title %>" runat="server" Visible="true" Style="font-weight: bold" />
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblExportType" runat="server" Text="<%# Resources.Messages.lbl_export_type+':' %>"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="cmbDownloadType" runat="server" CssClass="downType">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblStub" runat="server" Visible="false" Text="<%# Resources.Messages.lbl_stub+':' %>"></asp:Label>
            </td>
            <td>
                <asp:CheckBox ID="chkStub" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblIncludeCodeListAndConceptScheme" runat="server" Text="<%# Resources.Messages.lbl_include_codelist_and_conceptscheme +':' %>" Visible="false"></asp:Label>
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
                <asp:Label ID="lblSeparator" runat="server" Text="<%# Resources.Messages.lbl_use_this_separator%>" CssClass="IRCSV"></asp:Label>
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


        <tr>
            <td>
                <asp:Label ID="lblNewID" runat="server" Text="<%$ Resources:Messages,lbl_WSNewID%>" CssClass="WSNewIdentity">
                </asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtNewID" Text="" MaxLength="50" Width="150px" runat="server" CssClass="WSNewIdentity" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblNewAgency" runat="server" Text="<%$ Resources:Messages,lbl_WSNewAgency%>" CssClass="WSNewIdentity"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="cmbNewAgency" runat="server" CssClass="WSNewIdentity">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblNewVersion" runat="server" Text="<%$ Resources:Messages,lbl_WSNewVersion%>" CssClass="WSNewIdentity"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtNewVersion" Text="" MaxLength="50" Width="150px" runat="server" CssClass="WSNewIdentity" />
            </td>
        </tr>

    </table>
    <hr style="width: 100%" />
    <br />
    <div style="float: right">
        <center>
        <asp:Button ID="btnDownload" runat="server" OnClick="btnDownload_Click" OnClientClick="$.unblockUI();" Text="<%# Resources.Messages.lbl_download %>" />
        <asp:Button ID="btnAjaxDownload" runat="server" OnClientClick="return false;" CssClass="WSNewIdentity" OnClick="btnDownload_Click" Text="<%$ Resources:Messages,lbl_upload%>" />
        <asp:HiddenField ID="hdnWsSource" runat="server"></asp:HiddenField>
    </center>
    </div>

</div>


<%--<div id="Div1" class="popup_block">
    <asp:Label ID="Label1" runat="server" Text="<%# Resources.Messages.lbl_artefact+':' %>" CssClass="PageTitle3" EnableViewState="True"></asp:Label>
    <br />
    <br />
    <hr style="width: 100%" />

    <table class="tbNoBorder" cellpadding="6" style="width: 90%">
        <tr>
            <td colspan="2">
                <asp:Label ID="Label2" Text="<%# Resources.Messages.lbl_download_title %>" runat="server" Visible="true" Style="font-weight: bold" />
                <br />
            </td>
        </tr>
     </table>
</div>--%>

<script>

    function openAndReset(pName) {
        $(".downType").val(0).change();
        $(".popup_block").css("z-index", 19999);
        if ("<%=ucArtefactType%>".toUpperCase() == "DATAPROVIDERSCHEME" ||
            "<%=ucArtefactType%>".toUpperCase() == "DATACONSUMERSCHEME") {
            $("#<%=txtNewID.ClientID %>").prop("readonly", true)
            $("#<%=txtNewVersion.ClientID %>").prop("readonly", true)
        }
        openPopUp(pName);
    }

    $(document).ready(function () {
        $('.IRCSV').hide();
        $('.WSExport').hide();
        $('.WSAutentication').hide();
        $('.WSNewIdentity').hide();
        $('#<%=btnAjaxDownload.ClientID %>').hide();

        $('#<%=btnAjaxDownload.ClientID %>').unbind('click').click(function () {

            var jsonData = JSON.stringify({
                sourceEndPoint: $("#<%=hdnWsSource.ClientID %>").val(),
                targetEndPoint: $("#<%=cmbWSExport.ClientID %>").val(),
                artefactType: "<%=ucArtefactType%>",
                artID: "<%=ucID%>",
                artAgency: "<%=ucAgency%>",
                artVersion: "<%=ucVersion%>",
                newArtID: $("#<%=txtNewID.ClientID %>").val(),
                newArtAgency: $("#<%=cmbNewAgency.ClientID %>").val(),
                newArtVersion: $("#<%=txtNewVersion.ClientID %>").val(),
                exportWithRef: $("#<%=chkExportCodeListAndConcept.ClientID %>").is(":checked")
            });
            $.ajax({
                type: "POST",
                url: "WebServices/IRWebService.asmx/ExportArtefact",
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
                msg += "<br>" + item.substring(item.lastIndexOf(".", item.indexOf("=", 0))+1);   //Codelist=ESTAT:CL_INSTR_ASSET(1.0)
            });

            msg = '<div style="overflow: auto; width: 100%; height: 380px;">' + head + msg + "</div>";

            ShowDialog(msg, 800, 'IR Message');

        }
        function OnExportError(response) {
            jsonValue = jQuery.parseJSON(response.responseText);
            ShowDialog(jsonValue.Message, 300, 'IR Error');
        }

        $("#<%=btnDownload.ClientID %>").click(function () {

            $(".popup_block").css("z-index", 99999);
            //closePopup();
            //$.unblockUI();
        });

        function GetAgencies(selectedItem) {
            var jsonData = JSON.stringify({ endPointName: selectedItem });
            $.ajax({
                type: "POST",
                url: "WebServices/IRWebService.asmx/GetAgencies",
                data: jsonData,
                contentType: "application/json; charset=utf-8",
                dataType: "json", // dataType is json format
                success: OnGetAgenciesSuccess,
                error: OnGetAgenciesErrorCall
            });
        }

        function OnGetAgenciesSuccess(data) {
            $("#<%=cmbNewAgency.ClientID %> option").remove();

            console.log(data);
            console.log(data.d);

            var items = JSON.parse(data.d);

            $.each(items, function (index, item) {
                $('#<%=cmbNewAgency.ClientID %>').append($('<option>', {
                    value: item.id,
                    text: item.id
                }));
            });

            if ($("#<%=cmbNewAgency.ClientID %> option[value='<%=ucAgency%>']").length > 0)
                $('#<%=cmbNewAgency.ClientID %>').val("<%=ucAgency%>");
            else {
                $('#<%=cmbNewAgency.ClientID %>').prepend($('<option>', {
                    value: "",
                    text: ""
                }));
                $('#<%=cmbNewAgency.ClientID %>').val("");
            }
        }

        function OnGetAgenciesErrorCall(xhr) {
            var err = eval("(" + xhr.responseText + ")");
            ShowDialog(err.Message, 300, 'IR Error');
        }

        $("#<%=cmbWSExport.ClientID %>").change(function () {
            // effettuo una chiamata Ajax per vedere se l'utente è autenticato all'export nel WS di destinazione

            //$.blockUI();

            var selectedItem = $(this).val();
            var cmb = $(this).children("option").first();

            if (cmb.val() == "")
                cmb.remove();

            //$('#<%=btnAjaxDownload.ClientID %>').show();

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
                    $('.WSNewIdentity').hide();
                    $("#<%=txtWSUserName.ClientID %>").val("");
                    $("#<%=txtWSPassword.ClientID %>").val("");
                }
                else {
                    GetAgencies(selectedItem);
                    $('.WSAutentication').hide();
                    $('.WSNewIdentity').show();
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
                    $('.WSNewIdentity').show();
                    GetAgencies($("#<%=cmbWSExport.ClientID %>").val());
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
                //$('#<%=btnAjaxDownload.ClientID %>').hide();
                $('#<%=btnDownload.ClientID %>').hide();

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
                $('#<%=btnDownload.ClientID %>').show();
            }

        });

        $('#<%= txtSeparator.ClientID %>').keydown(function (event) {
            var separator = $(this).val();
            if (separator.length == 1 && event.keyCode != 8) {
                return false;
            }
        });
    });

    var prm = Sys.WebForms.PageRequestManager.getInstance();

    prm.add_endRequest(function () {

        // E' NECESSARIO RIPETERE IL CODICE JS QUI DENTRO

        $('.IRCSV').hide();
        $('.WSExport').hide();
        $('.WSAutentication').hide();
        $('.WSNewIdentity').hide();
        $('#<%=btnAjaxDownload.ClientID %>').hide();

        $('#<%=btnAjaxDownload.ClientID %>').unbind('click').click(function () {

            var jsonData = JSON.stringify({
                sourceEndPoint: $("#<%=hdnWsSource.ClientID %>").val(),
                targetEndPoint: $("#<%=cmbWSExport.ClientID %>").val(),
                artefactType: "<%=ucArtefactType%>",
                artID: "<%=ucID%>",
                artAgency: "<%=ucAgency%>",
                artVersion: "<%=ucVersion%>",
                newArtID: $("#<%=txtNewID.ClientID %>").val(),
                newArtAgency: $("#<%=cmbNewAgency.ClientID %>").val(),
                newArtVersion: $("#<%=txtNewVersion.ClientID %>").val(),
                exportWithRef: $("#<%=chkExportCodeListAndConcept.ClientID %>").is(":checked")
            });
            $.ajax({
                type: "POST",
                url: "WebServices/IRWebService.asmx/ExportArtefact",
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
                msg += "<br>" + item.substring(item.lastIndexOf(".", item.indexOf("=", 0))+1);   //Codelist=ESTAT:CL_INSTR_ASSET(1.0)
            });

            msg = '<div style="overflow: auto; width: 100%; height: 380px;">' + head + msg + "</div>";

            ShowDialog(msg, 800, 'IR Message');

        }
        function OnExportError(response) {
            jsonValue = jQuery.parseJSON(response.responseText);
            ShowDialog(jsonValue.Message, 300, 'IR Error');
        }

        $("#<%=btnDownload.ClientID %>").click(function () {

            $(".popup_block").css("z-index", 99999);
            //closePopup();
            //$.unblockUI();
        });

        function GetAgencies(selectedItem) {
            var jsonData = JSON.stringify({ endPointName: selectedItem });
            $.ajax({
                type: "POST",
                url: "WebServices/IRWebService.asmx/GetAgencies",
                data: jsonData,
                contentType: "application/json; charset=utf-8",
                dataType: "json", // dataType is json format
                success: OnGetAgenciesSuccess,
                error: OnGetAgenciesErrorCall
            });
        }

        function OnGetAgenciesSuccess(data) {
            $("#<%=cmbNewAgency.ClientID %> option").remove();

            console.log(data);
            console.log(data.d);

            var items = JSON.parse(data.d);

            $.each(items, function (index, item) {
                $('#<%=cmbNewAgency.ClientID %>').append($('<option>', {
                    value: item.id,
                    text: item.id
                }));
            });

            if ($("#<%=cmbNewAgency.ClientID %> option[value='<%=ucAgency%>']").length > 0)
                $('#<%=cmbNewAgency.ClientID %>').val("<%=ucAgency%>");
            else {
                $('#<%=cmbNewAgency.ClientID %>').prepend($('<option>', {
                    value: "",
                    text: ""
                }));
                $('#<%=cmbNewAgency.ClientID %>').val("");
            }
        }

        function OnGetAgenciesErrorCall(xhr) {
            var err = eval("(" + xhr.responseText + ")");
            ShowDialog(err.Message, 300, 'IR Error');
        }

        $("#<%=cmbWSExport.ClientID %>").change(function () {
            // effettuo una chiamata Ajax per vedere se l'utente è autenticato all'export nel WS di destinazione

            //$.blockUI();

            var selectedItem = $(this).val();
            var cmb = $(this).children("option").first();

            if (cmb.val() == "")
                cmb.remove();

            //$('#<%=btnAjaxDownload.ClientID %>').show();

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
                    $('.WSNewIdentity').hide();
                    $("#<%=txtWSUserName.ClientID %>").val("");
                    $("#<%=txtWSPassword.ClientID %>").val("");
                }
                else {
                    GetAgencies(selectedItem);
                    $('.WSAutentication').hide();
                    $('.WSNewIdentity').show();
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
                    $('.WSNewIdentity').show();
                    GetAgencies($("#<%=cmbWSExport.ClientID %>").val());
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
                //$('#<%=btnAjaxDownload.ClientID %>').hide();
                $('#<%=btnDownload.ClientID %>').hide();

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
                $('#<%=btnDownload.ClientID %>').show();
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
<asp:Panel ID="pnlDownload" runat="server">
    <img src="./images/download2.png" id="imgDownloadButton" onclick="openAndReset('dialog-form<%=ucID.Replace( '@', '_' ) + '_' + ucAgency + '_' + ucVersion.Replace( '.', '_' )%>')" style="border: none; cursor: pointer;" alt="<%= Resources.Messages.lbl_download_artefact %>" title="<%= Resources.Messages.lbl_download_artefact %>" />
</asp:Panel>
<asp:Label ID="lblID" Text="" runat="server" Visible="false" />
<asp:Label ID="lblAgency" Text="" runat="server" Visible="false" />
<asp:Label ID="lblVersion" Text="" runat="server" Visible="false" />
<asp:Label ID="lblArtefactType" Text="" runat="server" Visible="false" />



