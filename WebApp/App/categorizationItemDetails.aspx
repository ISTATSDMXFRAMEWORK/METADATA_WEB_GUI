<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" CodeBehind="categorizationItemDetails.aspx.cs" Inherits="ISTATRegistry.categorizationItemDetails" 
    AutoEventWireup="True" 
    EnableSessionState="True" %>
<%@ Register src="UserControls/FileDownload3.ascx" tagname="FileDownload3" tagprefix="uc1" %>
<%@ Register src="UserControls/AddText.ascx" tagname="AddText" tagprefix="uc2" %>
<%@ Register src="UserControls/UserPopUp.ascx" tagname="UserPopUp" tagprefix="uc3" %> 
<%@ Register src="UserControls/ControlAnnotations.ascx" tagname="ControlAnnotations" tagprefix="uc4" %>

<%@ Register src="UserControls/DuplicateArtefact.ascx" tagname="DuplicateArtefact" tagprefix="uc5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
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

            $("#<%= ddlCategorySchemeList.ClientID %>, #<%= ddlAvailableStructures.ClientID %>").change(function () {
                window.onbeforeunload = null;
            });

            $("#<%= TreeView1.ClientID %>, #<%= structuresGrid.ClientID %>").click(function () {
                window.onbeforeunload = null;
            });

            $(window).bind('beforeunload', function (e) {
                var confirmationMessage = "<%=Resources.Messages.lbl_question_exit_page %>";  // a space
                (e || window.event).returnValue = confirmationMessage;
                $.unblockUI();
                return confirmationMessage;
            });   
        });
    </script>
    <style type="text/css">
        
        .etabs { margin: 0; padding: 0; }
        .tab { display: inline-block; zoom:1; *display:inline; background: #eee; border: solid 1px #999; border-bottom: none; -moz-border-radius: 4px 4px 0 0; -webkit-border-radius: 4px 4px 0 0; }
        
        .tab a { font-size: 14px; line-height: 2em; display: block; padding: 0 10px; outline: none; color: #000000; text-decoration: none}
        .tab a:hover { color: #ff0000; text-decoration: none }
        .tab.active { background: #fff; padding-top: 6px; position: relative; top: 1px; border-color: #666; color: #000000; text-decoration: none}
        .tab a.active { font-weight: bold; color: #000000; text-decoration: none}
        
        .tab-container .panel-container { background: #fff; border: solid #666 1px; padding: 10px; -moz-border-radius: 0 4px 4px 4px; -webkit-border-radius: 0 4px 4px 4px; }
        
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Label ID="lblCategorizationDetail" runat="server" CssClass="PageTitle"><%= Resources.Messages.lbl_categorization%>&#32;<%= Resources.Messages.lbl_item_dettail %></asp:Label>

    <div id="divBack">
        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/categorization.aspx?m=y" ImageUrl="~/images/back.png"><%= Resources.Messages.lbl_back %></asp:HyperLink>
    </div> 
                     
    <hr style="width:100%" />

    <!-- Form di CRUD codelist ------- Fabrizio Alonzi -->
    <div id="tab-container" class='tab-container'>
        <ul class='etabs'>
            <li class='tab'><a href="#general"><%= Resources.Messages.lbl_general %></a></li>
            <li class='tab'><a href="#structure"><%= Resources.Messages.lbl_structure %></a></li>
        </ul>
        <div class='panel-container'>
            <div id="general">
                <table class="tableForm">

                    <tr>
                        <td>
                            <asp:Label ID="lbl_id" runat="server" Text="<%# '*' + Resources.Messages.lbl_id+ ':' %>" CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txt_id" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lbl_agency" runat="server" Text="<%# '*' + Resources.Messages.lbl_agency+ ':' %>" CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="cmb_agencies" runat="server" Enabled="False">
                            </asp:DropDownList>
                            <asp:TextBox ID="txtAgenciesReadOnly" runat="server" Enabled="false" Visible="false"></asp:TextBox>   
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lbl_version" runat="server" Text="<%# '*' + Resources.Messages.lbl_version+ ':' %>" CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txt_version" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lbl_isFinal" runat="server" Text="<%# Resources.Messages.lbl_is_final+ ':' %>" CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:CheckBox ID="chk_isFinal" runat="server" Enabled="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lbl_uri" runat="server" Text="<%# Resources.Messages.lbl_uri+ ':' %>" CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txt_uri" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lbl_urn" runat="server" Text="<%# Resources.Messages.lbl_urn+ ':' %>" CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txt_urn" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lbl_valid_from" runat="server" Text="<%# Resources.Messages.lbl_valid_from+ ':' %>" CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txt_valid_from" runat="server" Enabled="false" CssClass="datepicker"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lbl_valid_to" runat="server" Text="<%# Resources.Messages.lbl_valid_to+ ':' %>" CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txt_valid_to" runat="server" Enabled="false" CssClass="datepicker"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lbl_name" runat="server" Text="<%# '*' + Resources.Messages.lbl_name+ ':' %>" CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:Panel ID="pnlViewName" runat="server" Visible="false">
                                <asp:TextBox ID="txt_name_locale" runat="server" Enabled="false" TextMode="MultiLine" Rows="5" ValidationGroup="dsd"></asp:TextBox>
                            </asp:Panel>
                            <asp:Panel ID="pnlEditName" runat="server" Visible="false">
                                <uc2:AddText ID="AddTextName" runat="server" />
                            </asp:Panel>
                            <asp:TextBox ID="txt_all_names" runat="server" visible="false" ReadOnly= "true" />
                        </td>
                        <td>
                            <asp:Label ID="lbl_description" runat="server" Text="<%# Resources.Messages.lbl_description+ ':' %>" CssClass="tdProperty"></asp:Label>
                        </td>
                        <td>
                            <asp:Panel ID="pnlViewDescription" runat="server" Visible="false">
                                <asp:TextBox ID="txt_description_locale" runat="server" Enabled="false" TextMode="MultiLine" Rows="5"></asp:TextBox>
                            </asp:Panel>
                            <asp:Panel ID="pnlEditDescription" runat="server" Visible="false">
                                <uc2:AddText ID="AddTextDescription" runat="server" />
                            </asp:Panel>
                            <asp:TextBox ID="txt_all_description" runat="server" visible="false" ReadOnly= "true" />
                        </td>
                    </tr>
                    
                    <tr>              
                        <td>
                            <asp:Label ID="lbl_annotation" runat="server" Text="<%# Resources.Messages.lbl_annotation+ ':' %>" CssClass="tdProperty"></asp:Label>
                        </td>
                        <td colspan="3">
                            <asp:Panel ID="pnlAnnotation" runat="server" Visible="true">
                                <uc4:ControlAnnotations ID="AnnotationGeneralControl" runat="server" />
                            </asp:Panel>
                        </td>
                    </tr>

                </table>
            </div>
            <div id= "structure">
                <asp:Label ID="lblCategorySchemeList" runat="server" Text="<%# '*' + Resources.Messages.lbl_category_scheme_list + ':'%>"></asp:Label>&nbsp;&nbsp;
                <asp:DropDownList ID="ddlCategorySchemeList" runat="server" AutoPostBack="true"
                    onselectedindexchanged="ddlCategorySchemeList_SelectedIndexChanged">
                </asp:DropDownList>
                <br /><br />
                <asp:Button ID="btnOpenTreeDiv" runat="server" 
                    Text="<%# Resources.Messages.btn_open_tree_div %>" 
                    onclick="btnOpenTreeDiv_Click"></asp:Button>
                <br /><br />
                <asp:Label ID="lblSelectedCategory" runat="server" Visible="false" Text="<%# Resources.Messages.lbl_selected_category + ':'%>"></asp:Label>&nbsp;&nbsp;
                <asp:Label ID="txtSelectedCategory" runat="server" Visible="false" ></asp:Label>
                <br /><br />
                <asp:Label ID="lblAvailableStructures" runat="server" Text="<%# '*' + Resources.Messages.lbl_available_structures_list + ':'%>"></asp:Label>&nbsp;&nbsp;
                <asp:DropDownList ID="ddlAvailableStructures" runat="server" AutoPostBack="true"
                    onselectedindexchanged="ddlAvailableStructures_SelectedIndexChanged">
                </asp:DropDownList>
                <br /><br />                
                <asp:Button ID="btnOpenGridDiv" runat="server" 
                    Text="<%# Resources.Messages.btn_open_grid_div %>" 
                    onclick="btnOpenGridDiv_Click"></asp:Button>
                <br /><br />
                <asp:Label ID="lblSelectedItem" runat="server" Visible="false" Text="<%# Resources.Messages.lbl_selected_item + ':'%>"></asp:Label>&nbsp;&nbsp;
                <asp:Label ID="txtSelectedItem" runat="server" Visible="false" ></asp:Label>
                <br /><br />
            </div> 
        </div>
    </div>

    <asp:Button ID="btnSaveMemoryCategorization" runat="server" Text="<%# Resources.Messages.btn_save %>" onclick="btnSaveMemoryCategorization_Click" /> 
    
    <div style="float:right">
        <uc1:FileDownload3 ID="FileDownload31" runat="server"/>
    </div>
    
    <uc3:UserPopUp ID="UserPopUp1" runat="server" />

    <div id= "treeViewDiv" class="popup_block" >
        <div style="overflow:auto; height:300px;">
          <asp:TreeView ID="TreeView1" runat="server" 
                            onselectednodechanged="TreeView1_SelectedNodeChanged"
                            BorderStyle="Solid" 
                            BorderWidth="0px" 
                            ForeColor="Black" 
                            ShowLines="True" 
                            Width="100%"
                            CssClass="CatTreeView"
                            ExpandDepth="1">
                            <NodeStyle NodeSpacing="2px" />
                            <SelectedNodeStyle BackColor="#CCD0D3" Font-Bold="True" />
                        </asp:TreeView>
        </div>
    </div>

    <div id= "gridDiv" class="popup_block" >
        <div style="overflow:auto; height:400px;">
        <asp:GridView 
                ID="structuresGrid" 
                runat="server"
                CssClass="Grid" 
                AllowPaging="True"
                PagerSettings-Mode="NumericFirstLast"   
                PagerSettings-FirstPageText="<%= Resources.Messages.btn_goto_first %>"
                PagerSettings-LastPageText="<%= Resources.Messages.btn_goto_last %>"          
                AutoGenerateColumns="False"               
                DataSourceID="ObjectDataSource1"                
                PagerSettings-Position="TopAndBottom" 
                onpageindexchanged="structuresGrid_PageIndexChanged" 
                onpageindexchanging="structuresGrid_PageIndexChanging"
                PageSize="12" onrowcommand="structuresGrid_RowCommand">         
                <Columns>
                    <asp:TemplateField HeaderText="ID" SortExpression="ID">
                        <ItemTemplate>
                            <asp:Label ID="lblID" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="190px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Agency" SortExpression="Agency">
                        <ItemTemplate>
                            <asp:Label ID="lblAgency" runat="server" Text='<%# Bind("AgencyId") %>'></asp:Label>
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
                    <asp:TemplateField ShowHeader="False">
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Select" CommandArgument="<%# Container.DataItemIndex %>" Text="<%# Resources.Messages.lbl_select %>"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="LocalID" HeaderText="LocalID" SortExpression="LocalID" Visible="False" />                  
                    <asp:CommandField EditText="Edit" HeaderText="View/Edit" ShowEditButton="True" Visible="False" />                  
                </Columns>
                <HeaderStyle CssClass="hs" />
                <RowStyle CssClass="rs" />
                <AlternatingRowStyle CssClass="ars" />
                <PagerStyle CssClass="pgr"></PagerStyle>
            </asp:GridView>
             <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetCodeListList" TypeName="ISTAT.EntityMapper.EntityMapper">
                <SelectParameters>
                    <asp:Parameter Name="sdmxObjects" Type="Object" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </div>
    </div>

    <uc5:DuplicateArtefact ID="DuplicateArtefact1" runat="server" Visible="false"/>

</asp:Content>
