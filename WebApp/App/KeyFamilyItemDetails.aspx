<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="KeyFamilyItemDetails.aspx.cs" Inherits="ISTATRegistry.keyFamilyItemDetails"
    EnableSessionState="True" %>

<%@ Register Src="UserControls/FileDownload3.ascx" TagName="FileDownload3" TagPrefix="uc1" %>
<%@ Register Src="UserControls/AddText.ascx" TagName="AddText" TagPrefix="uc2" %>
<%@ Register Src="UserControls/GetCodeList.ascx" TagName="GetCodeList" TagPrefix="uc3" %>
<%@ Register Src="UserControls/GetConcept.ascx" TagName="GetConcept" TagPrefix="uc4" %>
<%@ Register Src="UserControls/UserPopUp.ascx" TagName="UserPopUp" TagPrefix="uc5" %>
<%@ Register Src="UserControls/ControlAnnotations.ascx" TagName="ControlAnnotations"
    TagPrefix="uc5" %>
<%@ Register Src="UserControls/DuplicateArtefact.ascx" TagName="DuplicateArtefact"
    TagPrefix="uc6" %>
<%@ Register Namespace="ISTATRegistry.Classes" TagPrefix="iup" Assembly="IstatRegistry" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function ConfirmDeleteDimension() {
            return confirm("<%=Resources.Messages.lbl_question_delete_dimension %>");
        }

        function ConfirmDeleteAttribute() {
            return confirm("<%=Resources.Messages.lbl_question_delete_attribute %>");
        }

        function ConfirmDeleteGroup() {
            return confirm("<%=Resources.Messages.lbl_question_delete_group %>");
        }

        $(document).ready(function () {
            $('#tab-container').easytabs();

            $(".datepicker").datepicker({
                changeMonth: true,
                changeYear: true,
                dateFormat: 'dd/mm/yy'
            });
            $(".datepicker").datepicker("option", "showAnim", "drop");

            jQuery(function ($) {
                var form = $('form'), oldSubmit = form[0].onsubmit;
                form[0].onsubmit = null;

                if(!<%=AspConfirmationExit%>)
                    window.onbeforeunload = null;

                $('form').submit(function () {
                    // reset the onbeforeunload
                    window.onbeforeunload = null;

                    // run what actually was on
                    if (oldSubmit)
                        oldSubmit.call(this);
                });
            });

            $(window).bind('beforeunload', function (e) {
                var confirmationMessage = "<%=Resources.Messages.lbl_question_exit_page %>";  // a space
                (e || window.event).returnValue = confirmationMessage;
                $.unblockUI();
                return confirmationMessage;
            });

        });           

    </script>
    <style>
        .etabs { margin: 0; padding: 0; }
        .tab { display: inline-block; zoom:1; *display:inline; background: #eee; border: solid 1px #999; border-bottom: none; -moz-border-radius: 4px 4px 0 0; -webkit-border-radius: 4px 4px 0 0; }
        
        .tab a { font-size: 14px; line-height: 2em; display: block; padding: 0 10px; outline: none; color: #000000; text-decoration: none}
        .tab a:hover { color: #ff0000; text-decoration: none }
        .tab.active { background: #fff; padding-top: 6px; position: relative; top: 1px; border-color: #666; color: #000000; text-decoration: none}
        .tab a.active { font-weight: bold; color: #000000; text-decoration: none}
        
        .tab-container .panel-container { background: #fff; border: solid #666 1px; padding: 10px; -moz-border-radius: 0 4px 4px 4px; -webkit-border-radius: 0 4px 4px 4px; }
        
        /*
        a:link {color: #000000; text-decoration: none}
        a:active {color: #000000; text-decoration: none}
        a:visited {color: #000000; text-decoration: none}
        a:hover {color: #ff0000; text-decoration: none}
        */
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblDetail" runat="server" CssClass="PageTitle"><%= Resources.Messages.lbl_key_family %>&#32;<%= Resources.Messages.lbl_item_dettail %></asp:Label>
    <div id="divBack">
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/KeyFamily.aspx?m=y"
            ImageUrl="~/images/back.png"><%= Resources.Messages.lbl_back %></asp:HyperLink>
    </div>
    <hr style="width: 100%" />
    <div id="tab-container" class='tab-container'>
        <ul class='etabs'>
            <li class='tab'><a href="#general">
                <%= Resources.Messages.lbl_general %></a></li>
            <li class='tab'><a href="#PrimaryMeasure">
                <%= Resources.Messages.lbl_primary_measure %></a></li>
            <li class='tab'><a href="#Dimensions">
                <%= Resources.Messages.lbl_dimension %></a></li>
            <li class='tab'><a href="#Groups">
                <%= Resources.Messages.lbl_group %></a></li>
            <li class='tab'><a href="#Attributes">
                <%= Resources.Messages.lbl_attribute %></a></li>
            <%--            <li class='tab'><a href="#Annotations"><%= Resources.Messages.lbl_annotation%></a></li>--%>
        </ul>
        <div class='panel-container'>
            <div id="general">
                <iup:IstatUpdatePanel ID="IstatUpdatePanel1" runat="server">
                    <ContentTemplate>
                        <table class="tableForm">
                            <tr>
                                <td>
                                    *<asp:Label ID="lblDSDID" runat="server" Text="<%# Resources.Messages.lbl_id +':'%>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td width="45%">
                                    <asp:TextBox ID="txtDSDID" runat="server" Enabled="false"></asp:TextBox>
                                </td>
                                <td>
                                    *<asp:Label ID="lblAgency" runat="server" Text="<%# Resources.Messages.lbl_agency +':'%>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td width="45%">
                                    <asp:DropDownList ID="cmbAgencies" runat="server" Enabled="False">
                                    </asp:DropDownList>
                                    <asp:TextBox ID="txtAgenciesReadOnly" runat="server" Enabled="false" Visible="false"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    *<asp:Label ID="lblVersion" runat="server" Text="<%# Resources.Messages.lbl_version +':'%>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtVersion" runat="server" Enabled="false"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Label ID="lblIsFinal" runat="server" Text="<%# Resources.Messages.lbl_is_final +':'%>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkIsFinal" runat="server" Enabled="false" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblDSDURI" runat="server" Text="<%# Resources.Messages.lbl_uri +':'%>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtDSDURI" runat="server" Enabled="false"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Label ID="lblDSDURN" runat="server" Text="<%# Resources.Messages.lbl_urn +':'%>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtDSDURN" runat="server" Enabled="false"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblValidFrom" runat="server" Text="<%# Resources.Messages.lbl_valid_from +':'%>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtValidFrom" runat="server" Enabled="false" CssClass="datepicker"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Label ID="lblValidTo" runat="server" Text="<%# Resources.Messages.lbl_valid_to +':'%>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtValidTo" runat="server" Enabled="false" CssClass="datepicker"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    *<asp:Label ID="lblDSDName" runat="server" Text="<%# Resources.Messages.lbl_name +':'%>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:Panel ID="pnlViewName" runat="server" Visible="false">
                                        <asp:TextBox ID="txtDSDName" runat="server" Enabled="false" TextMode="MultiLine"
                                            Rows="5" CssClass="noScalableTextArea"></asp:TextBox>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlEditName" runat="server" Visible="false">
                                        <uc2:AddText ID="AddTextName" runat="server" />
                                    </asp:Panel>
                                </td>
                                <td>
                                    <asp:Label ID="lblDSDDescription" runat="server" Text="<%# Resources.Messages.lbl_description +':'%>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:Panel ID="pnlViewDescription" runat="server" Visible="false">
                                        <asp:TextBox ID="txtDSDDescription" runat="server" Enabled="false" TextMode="MultiLine"
                                            Rows="5" CssClass="noScalableTextArea"></asp:TextBox>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlEditDescription" runat="server" Visible="false">
                                        <uc2:AddText ID="AddTextDescription" runat="server" />
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lbl_annotation" runat="server" Text="<%# Resources.Messages.lbl_annotation+ ':' %>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td colspan="3">
                                    <asp:Panel ID="pnlAnnotation" runat="server" Visible="true">
                                        <uc5:ControlAnnotations ID="AnnotationGeneral" runat="server" />
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </iup:IstatUpdatePanel>
            </div>
            <div id="PrimaryMeasure">
                <iup:IstatUpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <table class="tableForm">
                            <tr>
                                <td width="10%">
                                    *<asp:Label ID="lblPMID" runat="server" Text="<%# Resources.Messages.lbl_id +':'%>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td width="90%">
                                    <asp:TextBox ID="txtPMID" runat="server" Enabled="false" Width="300px">OBS_VALUE</asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    *<asp:Label ID="lblConcept" runat="server" Text="<%# Resources.Messages.lbl_concept +':'%>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtConcept" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                    <uc4:GetConcept ID="GetConcept1" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblCodelist" runat="server" Text="<%# Resources.Messages.lbl_codelsit +':'%>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtCodelist" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                    <uc3:GetCodeList ID="GetCodeList1" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblAnnotaionPM" runat="server" Text="<%# Resources.Messages.lbl_annotation+ ':' %>"
                                        CssClass="tdProperty"></asp:Label>
                                </td>
                                <td>
                                    <uc5:ControlAnnotations ID="AnnotationPrimaryMeasure" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </iup:IstatUpdatePanel>
            </div>
            <div id="Dimensions">
                <iup:IstatUpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="gvDimension" runat="server" Width="100%" AllowPaging="True" CssClass="Grid"
                            OnPageIndexChanging="gvDimension_PageIndexChanging" AutoGenerateColumns="False"
                            OnRowDeleting="gvDimension_RowDeleting" OnRowCommand="gvDimension_RowCommand"
                            OnRowDataBound="gvDimension_RowDataBound">
                            <Columns>
                                <asp:TemplateField HeaderText="ID" SortExpression="Type">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDimID" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="25%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Concept" SortExpression="Concept">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDimConcept" runat="server" Text='<%# Bind("Concept") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="25%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Coded Representation" SortExpression="CodeList">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCodelist2" runat="server" Text='<%# Bind("CodeList") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="25%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="TextFormat" SortExpression="TextFormat" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTextFormat2" runat="server" Text='<%# Bind("TextFormat") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="25%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Role" SortExpression="Role">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDimRole" runat="server" Text='<%# Bind("Role") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="18%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="" ShowHeader="False" Visible="False">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ibDimUpdate" runat="server" CausesValidation="False" CommandName="Details"
                                            CommandArgument="<%# Container.DataItemIndex %>" ImageUrl="~/images/Details2.png"
                                            ToolTip="View details" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ibDimDelete" runat="server" OnClientClick="return ConfirmDeleteDimension()"
                                            CausesValidation="False" CommandName="Delete" CommandArgument="<%# Container.DataItemIndex %>"
                                            ImageUrl="~/images/Delete2.png" ToolTip="Delete Dimension" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="" ShowHeader="False" Visible="True">
                                    <ItemTemplate>
                                        <asp:Label ID="lblNumberOfAnnotationDimensions" runat="server" Text="Label"></asp:Label>
                                        <asp:ImageButton ID="ibDimAnnotation" runat="server" CausesValidation="False" CommandName="Annotation"
                                            CommandArgument="<%# Container.DataItemIndex %>" ImageUrl="~/images/Annotation.png"
                                            ToolTip="View/Edit annotations" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="7%" HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle CssClass="hs" />
                            <RowStyle CssClass="rs" />
                            <AlternatingRowStyle CssClass="ars" />
                            <PagerStyle CssClass="pgr"></PagerStyle>
                        </asp:GridView>
                        <div id="df-anndimension" class="popup_block">
                            <asp:Label ID="lblAnnotationDimension" runat="server" Text="Annotation Manager" CssClass="PageTitle"></asp:Label>
                            <hr style="width: 95%;" />
                            <table class="tableForm">
                                <tr>
                                    <td>
                                        <uc5:ControlAnnotations ID="ucAnnotationDimension" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <asp:Panel ID="pnlAddDimension" runat="server">
                            <a href='javascript: openP("df-Dimension",600)' title="<%# Resources.Messages.lbl_add_dimension%>">
                                <img src="./images/Add.png" border="0" alt="<%# Resources.Messages.lbl_add_dimension%>" /></a>
                        </asp:Panel>
                        <div id="df-Dimension" class="popup_block">
                            <asp:Label ID="lblTitle" runat="server" Text="<%# Resources.Messages.lbl_add_dimension%>"
                                CssClass="PageTitle"></asp:Label>
                            <hr style="width: 95%;" />
                            <table class="tableForm">
                                <tr>
                                    <td>
                                        <asp:Label ID="lblDimType" runat="server" Text="<%# Resources.Messages.lbl_dimension_type +':'%>"
                                            CssClass="tdProperty"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="cmbDimType" runat="server" OnSelectedIndexChanged="cmbDimType_SelectedIndexChanged"
                                            AutoPostBack="True">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td width="35%">
                                        *<asp:Label ID="lblDimID" runat="server" Text="<%# Resources.Messages.lbl_id +':'%>"
                                            CssClass="tdProperty"></asp:Label>
                                    </td>
                                    <td width="65%">
                                        <asp:TextBox ID="txtDimID" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblDimConceptReference" runat="server" Text="<%# Resources.Messages.lbl_concept_reference +':'%>"
                                            CssClass="underline"></asp:Label>
                                        <br />
                                        *<asp:Label ID="lblDimConcept" runat="server" Text="<%# Resources.Messages.lbl_concept +':'%>"
                                            CssClass="tdProperty"></asp:Label>
                                    </td>
                                    <td>
                                        <br />
                                        <asp:TextBox ID="txtDimConcept" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                        <uc4:GetConcept ID="GetDimConcept" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Panel ID="pnlCodedRepresentationLabel" runat="server">
                                            <asp:Label ID="lblCodeCodeRappresentation" runat="server" Text="<%# Resources.Messages.lbl_code_rappresentation +':'%>"
                                                CssClass="underline"></asp:Label>
                                            <br />
                                            <asp:Label ID="lblCodeCodelist" runat="server" Text="<%# Resources.Messages.lbl_concept +':'%>"
                                                CssClass="tdProperty"></asp:Label>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlConceptSchemeLabel" runat="server" Visible="false">
                                            <asp:Label ID="lblCodeConceptScheme" runat="server" Text="<%# Resources.Messages.lbl_concept_scheme +':'%>"
                                                CssClass="tdProperty"></asp:Label>
                                        </asp:Panel>
                                    </td>
                                    <td>
                                        <asp:Panel ID="pnlCodedRepresentationValue" runat="server">
                                            <br />
                                            <asp:TextBox ID="txtDimCodelist" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                            <uc3:GetCodeList ID="GetDimCodeList" runat="server" />
                                        </asp:Panel>
                                        <asp:Panel ID="pnlConceptSchemeValue" runat="server" Visible="false">
                                            <asp:TextBox ID="txtDimConceptScheme" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                            <uc4:GetConcept ID="GetDimConceptScheme" runat="server" />
                                            *
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnAddDimension" runat="server" Text="<%# Resources.Messages.lbl_add +':'%>"
                                            OnClick="btnAddDimension_Click" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </iup:IstatUpdatePanel>
            </div>
            <div id="Groups">
                <iup:IstatUpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="gvGroups" runat="server" Width="100%" AllowPaging="True" CssClass="Grid"
                            OnPageIndexChanging="gvGroups_PageIndexChanging" AutoGenerateColumns="False"
                            OnRowDataBound="gvGroups_RowDataBound" OnRowDeleting="gvGroups_RowDeleting" OnRowCommand="gvGroup_RowCommand">
                            <Columns>
                                <asp:TemplateField HeaderText="ID" SortExpression="ID">
                                    <ItemTemplate>
                                        <asp:Label ID="lblGroupKeyID" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="45%" />
                                    <ItemStyle VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="DimensionList" SortExpression="DimensionList">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDimensionList" runat="server" Text='<%# Bind("DimensionList") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="45%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ibGroupDelete" runat="server" OnClientClick="return ConfirmDeleteGroup()"
                                            CausesValidation="False" CommandName="Delete" CommandArgument="<%# Container.DataItemIndex %>"
                                            ImageUrl="~/images/Delete2.png" ToolTip="Delete Group" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="" ShowHeader="False" Visible="True">
                                    <ItemTemplate>
                                        <asp:Label ID="lblNumberOfAnnotationGroups" runat="server" Text="Label"></asp:Label>
                                        <asp:ImageButton ID="ibDimGroup" runat="server" CausesValidation="False" CommandName="Annotation"
                                            CommandArgument="<%# Container.DataItemIndex %>" ImageUrl="~/images/Annotation.png"
                                            ToolTip="View/Edit annotations" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="7%" HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle CssClass="hs" />
                            <RowStyle CssClass="rs" />
                            <AlternatingRowStyle CssClass="ars" />
                            <PagerStyle CssClass="pgr"></PagerStyle>
                        </asp:GridView>
                        <div id="df-anngroup" class="popup_block">
                            <asp:Label ID="lblAnnotationGroup" runat="server" Text="Annotation Manager" CssClass="PageTitle"></asp:Label>
                            <hr style="width: 95%;" />
                            <table class="tableForm">
                                <tr>
                                    <td>
                                        <uc5:ControlAnnotations ID="ucAnnotationGroup" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <asp:Panel ID="pnlAddGroup" runat="server">
                            <a href='javascript: openP("df-Group",600)' title="<%= Resources.Messages.lbl_add_group %>">
                                <img src="./images/Add.png" border="0" alt="<%= Resources.Messages.lbl_add_group %>" /></a>
                        </asp:Panel>
                        <div id="df-Group" class="popup_block">
                            <asp:Label ID="lblAddGroupTitle" runat="server" Text="<%# Resources.Messages.lbl_add_group %>"
                                CssClass="PageTitle"></asp:Label>
                            <hr style="width: 100%;" />
                            <table style="width: 470px">
                                <tr>
                                    <td width="95px">
                                        <asp:Label ID="lblGroupID" runat="server" Text="<%# Resources.Messages.lbl_id %>"
                                            CssClass="tdProperty"></asp:Label>
                                    </td>
                                    <td width="370px">
                                        <asp:TextBox ID="txtGroupID" runat="server" Width="100%"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblGroupDimension" runat="server" Text="<%# Resources.Messages.lbl_dimension +':'%>"
                                            CssClass="tdProperty"></asp:Label>
                                    </td>
                                    <td style="vertical-align: middle; width: 210px;">
                                        <asp:ListBox ID="lbGroupDimension" runat="server" Width="210px" Height="100px" SelectionMode="Multiple">
                                        </asp:ListBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <br />
                                        <asp:Button ID="btnAddGroup" runat="server" Text="<%# Resources.Messages.lbl_add +':'%>"
                                            OnClick="btnAddGroup_Click" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </iup:IstatUpdatePanel>
            </div>
            <div id="Attributes">
                <iup:IstatUpdatePanel ID="UpdatePanel4" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="gvAttribute" runat="server" Width="100%" AllowPaging="True" CssClass="Grid"
                            OnPageIndexChanging="gvAttribute_PageIndexChanging" OnRowDeleting="gvAttribute_RowDeleting"
                            OnRowCommand="gvAttribute_RowCommand" AutoGenerateColumns="False" OnRowDataBound="gvAttribute_RowDataBound">
                            <Columns>
                                <asp:TemplateField HeaderText="ID" SortExpression="Type">
                                    <ItemTemplate>
                                        <asp:Label ID="lblGVAttributeID" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="20%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Concept" SortExpression="Concept">
                                    <ItemTemplate>
                                        <asp:Label ID="lblAttrConcept" runat="server" Text='<%# Bind("Concept") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="20%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Coded Representation" SortExpression="CodeList">
                                    <ItemTemplate>
                                        <asp:Label ID="lblAttrCodelist" runat="server" Text='<%# Bind("CodeList") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="20%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="TextFormat" SortExpression="TextFormat" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTextFormat2" runat="server" Text='<%# Bind("TextFormat") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="20%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="AssStatus" SortExpression="AssStatus">
                                    <ItemTemplate>
                                        <asp:Label ID="lblAssStatus" runat="server" Text='<%# Bind("AssStatus") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="15%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="AttachLevel" SortExpression="AttachLevel">
                                    <ItemTemplate>
                                        <asp:Label ID="lblAttachLevel" runat="server" Text='<%# Bind("AttachLevel") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="15%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ibAttrDelete" runat="server" OnClientClick="return ConfirmDeleteAttribute()"
                                            CausesValidation="False" CommandName="Delete" CommandArgument="<%# Container.DataItemIndex %>"
                                            ImageUrl="~/images/Delete2.png" ToolTip="Delete Attribute" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="" ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ibAttrEdit" runat="server" CausesValidation="False" CommandName="Details"
                                            CommandArgument="<%# Container.DataItemIndex %>" ImageUrl="~/images/Details2.png"
                                            ToolTip="View/Edit Details" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="" ShowHeader="False" Visible="True">
                                    <ItemTemplate>
                                        <asp:Label ID="lblNumberOfAnnotationAttributes" runat="server" Text="Label"></asp:Label>
                                        <asp:ImageButton ID="ibDimAttribute" runat="server" CausesValidation="False" CommandName="Annotation"
                                            CommandArgument="<%# Container.DataItemIndex %>" ImageUrl="~/images/Annotation.png"
                                            ToolTip="View/Edit annotations" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="7%" HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle CssClass="hs" />
                            <RowStyle CssClass="rs" />
                            <AlternatingRowStyle CssClass="ars" />
                            <PagerStyle CssClass="pgr"></PagerStyle>
                        </asp:GridView>
                        <div id="df-annattribute" class="popup_block">
                            <asp:Label ID="lblAnnotationAttribute" runat="server" Text="Annotation Manager" CssClass="PageTitle"></asp:Label>
                            <hr style="width: 95%;" />
                            <table class="tableForm">
                                <tr>
                                    <td>
                                        <uc5:ControlAnnotations ID="ucAnnotationAttribute" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <asp:Panel ID="pnlAddAttribute" runat="server">
                            <asp:ImageButton ID="btnOpenAttributePopUp" runat="server" ImageUrl="./images/Add.png"
                                ToolTip="Add Attribute" AlternateText="Add Attribute" OnClick="btnOpenAttributePopUp_Click" />
                            <%--                            <a href='javascript: OpenAttributePopUp();' title="Add Attribute">
                                <img src="./images/Add.png" border="0" alt="Add Attribute" /></a>--%>
                        </asp:Panel>
                        <div id="df-Attribute" class="popup_block" title="Add Attribute">
                            <asp:Label ID="Label7" runat="server" Text="Add/Edit Attribute" CssClass="PageTitle"></asp:Label>
                            <hr style="width: 95%;" />
                            <table>
                                <tr>
                                    <td width="160px">
                                        *<asp:Label ID="Label8" runat="server" Text="Atttribute ID:" CssClass="tdProperty"></asp:Label>
                                    </td>
                                    <td width="370px">
                                        <asp:TextBox ID="txtAttributeID" runat="server" Width="300px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        *<asp:Label ID="Label11" runat="server" Text="Concept:" CssClass="tdProperty"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAttributeConcept" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                        <uc4:GetConcept ID="GetConceptAttribute" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label13" runat="server" Text="Codelist:" CssClass="tdProperty"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAttributeCodelist" runat="server" Enabled="false" Width="300px"></asp:TextBox>
                                        <uc3:GetCodeList ID="GetCodeListAttribute" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        *<asp:Label ID="Label10" runat="server" Text="Assignment Status:" CssClass="tdProperty"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="cmbAssignmentStatus" runat="server" Width="210px">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        *<asp:Label ID="Label12" runat="server" Text="Attachment Level:" CssClass="tdProperty"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="cmbAttachLevel" runat="server" OnSelectedIndexChanged="cmbAttachLevel_SelectedIndexChanged"
                                            AutoPostBack="True" Width="210px">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <asp:Panel ID="pnlAttachmentDimension" runat="server" Visible="false">
                                <table>
                                    <tr>
                                        <td width="160px">
                                            <asp:Label ID="Label14" runat="server" Text="Attachment Dimension:" CssClass="tdProperty"></asp:Label>
                                        </td>
                                        <td style="vertical-align: middle; width: 210px;">
                                            <asp:ListBox ID="lbAttachmentDimension" runat="server" Width="210px" Height="100px"
                                                SelectionMode="Multiple"></asp:ListBox>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="pnlAttachedGroupID" runat="server" Visible="false">
                                <table>
                                    <tr>
                                        <td width="160px">
                                            <asp:Label ID="Label15" runat="server" Text="Attached Group ID:" CssClass="tdProperty"></asp:Label>
                                        </td>
                                        <td style="vertical-align: middle; width: 210px;">
                                            <asp:DropDownList ID="cmbAttachedGroupID" runat="server" Width="210px">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <br />
                            <asp:Button ID="btnAddAttribute" runat="server" Text="Save" OnClick="btnAddAttribute_Click" />
                            <asp:HiddenField ID="hdnEditAttribute" runat="server" />
                        </div>
                    </ContentTemplate>
                </iup:IstatUpdatePanel>
            </div>
            <%--            <div id="Annotations">
                <asp:Label ID="lbl_annotation" runat="server" Text="<%# Resources.Messages.lbl_annotation %>" CssClass="tdProperty"></asp:Label>
                <asp:Panel ID="pnlAnnotation" runat="server" Visible="true">
                    <uc5:ControlAnnotations ID="AnnotationGeneralControl" runat="server" />
                </asp:Panel>
            </div>--%>
        </div>
    </div>
    <asp:Button ID="btnSaveDSD" runat="server" OnClick="btnSaveDSD_Click" Text="<%# Resources.Messages.btn_save %>"
        Visible="False" />
    <uc6:DuplicateArtefact ID="DuplicateArtefact1" runat="server" Visible="false" />
    <div style="float: right">
        <uc1:FileDownload3 ID="FileDownload31" runat="server" Visible="False" />
    </div>
</asp:Content>
