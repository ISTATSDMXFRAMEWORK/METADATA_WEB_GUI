<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="admin.aspx.cs"
    Inherits="ISTATRegistry.admin" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>
        <%= Resources.Messages.lbl_administration_page %></title>
    <!--[if lt IE 9]><script src="http://html5shiv.googlecode.com/svn/trunk/html5.js"></script><![endif]-->
    <link href="css/style.css" rel="stylesheet" />
    <link href="css/gridview.css" rel="stylesheet" />
    <link href="css/popup.css" rel="stylesheet" />
    <link rel="icon" href="images/favicon.ico" type="image/x-icon" />
    <link rel="shortcut icon" href="images/favicon.ico" type="image/x-icon" />
    <link rel="stylesheet" href="./jquery/jquery-ui.css" />
    <link href="css/jquery.msg.css" rel="stylesheet" />
    <script src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script src="js/PopUp.js" type="text/javascript"></script>
    <script src="js/jquery.blockUI.js" type="text/javascript"></script>
    <script src="jquery/jquery-ui.min.js" type="text/javascript"></script>
    <script src="js/jquery.ba-hashchange.js" type="text/javascript"></script>
    <script src="js/jquery.easytabs.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="js/jquery.center.min.js"></script>
    <script src="js/jquery.msg.min.js" type="text/javascript"></script>
    <script src="js/animatedcollapse.js" type="text/javascript"></script>
    <script language="jscript" type="text/javascript">

        var waitReload = true;

        $(document).ready(function () {
            $(document).submit(function (e) {
                $.blockUI();
            });

            $(window).bind("beforeunload", function (e) {
                $.blockUI();
            });
        });

        function getInternetExplorerVersion() {
            var ua = window.navigator.userAgent;
            var msie = ua.indexOf("MSIE ");
            var rv = -1;

            if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./))      // If Internet Explorer, return version number
            {

                if (isNaN(parseInt(ua.substring(msie + 5, ua.indexOf(".", msie))))) {
                    //For IE 11 >
                    if (navigator.appName == 'Netscape') {
                        var ua = navigator.userAgent;
                        var re = new RegExp("Trident/.*rv:([0-9]{1,}[\.0-9]{0,})");
                        if (re.exec(ua) != null) {
                            rv = parseFloat(RegExp.$1);
                            return rv;
                        }
                    }
                    else {
                        return 'otherbrowser';
                    }
                }
                else {
                    //For < IE11
                    return parseInt(ua.substring(msie + 5, ua.indexOf(".", msie)));
                }
                return false;
            }
        }

        function PrepareDefaultDiv(divId, text, title) {
            var myDiv = document.getElementById(divId);
            if (myDiv == null) {
                var myDiv = document.createElement('div');
                myDiv.id = divId;
                myDiv.className = 'popup_block';
                if (title == null || title.trim() == '') {
                    title = "<%= Resources.Messages.lbl_message_title %>";
                }
                title = "<span class= 'PageTitle3'>" + title + "</span><br /><hr/><br />";
                myDiv.innerHTML = title + text;
                document.body.appendChild(myDiv);
            }
            else {
                myDiv.id = divId;
                myDiv.className = 'popup_block';
                myDiv.innerHTML = text;
            }
        }

        function ShowDialog(text, popWidth, title) {
            if (popWidth === undefined) popWidth = 300;

            var currentDate = new Date();
            var newIdName = currentDate.getDay().toString() + currentDate.getMonth().toString() + currentDate.getYear().toString() + currentDate.getHours().toString() + currentDate.getMinutes().toString() + currentDate.getSeconds().toString() + currentDate.getMilliseconds().toString();
            PrepareDefaultDiv(newIdName, text, title);
            openP(newIdName, popWidth, false, null);
        }

        function ForceBlackClosing() {
            $("#fade").fadeOut();
        }

        function animatedInit(SearchBarName) {
            try {
                animatedcollapse.addDiv(SearchBarName, 'fade=0,speed=400,group=sb,hide=1');
                animatedcollapse.init();
            } catch (e) { }
        }

        function ShowSearchBar(SearchBarName) {
            try {
                animatedcollapse.show(SearchBarName);
            } catch (e) {

            }
        }

        function HideSearchBar(SearchBarName) {
            try {
                animatedcollapse.hide(SearchBarName);
            } catch (e) { }
        }

        function openP(pname, popWidth, usingCallback, callback) {
            if (popWidth === undefined) popWidth = 350;
            openPopUp(pname, popWidth, usingCallback, callback);
            //return false;
        }

        function openPopulatedP(pname, popWidth, usingCallback, callback) {
            openP(pname, popWidth, usingCallback, callback);
            //return false;
        }

        function ResetBeforeUnload() {
            window.onbeforeunload = null;
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="wrapper">
        <header class="header">
        <div style="float: left">
		    <img src="images/SdmxLogo.png" alt="Sdmx Logo"/>
        </div>
        <br /><br /><br /><br />
	    </header>
        <!-- .header-->
        <div class="middle">
            <div class="container">
                <div id="adminLogin">
                    <asp:Panel ID="pnlAdministrationLogin" runat="server">
                        <div id="loginAdministrator">
                            <h1>
                                <%= Resources.Messages.lblUserCredentials %></h1>
                            <asp:Label ID="lblEndPoint" runat="server" Text="EndPoint"></asp:Label>
                            <asp:DropDownList ID="cmbEPoints" CssClass="loginUserNameText loginUserNameAdminText"
                                runat="server" AutoPostBack="false" OnSelectedIndexChanged="cmbEPoints_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:Label ID="lblLoginUserName" runat="server" Text="Username"></asp:Label>
                            <asp:TextBox ID="txtLoginUserName" CssClass="loginUserNameText loginUserNameAdminText"
                                runat="server"></asp:TextBox>
                            <br />
                            <asp:Label ID="lblLoginPassword" runat="server" Text="Password"></asp:Label>
                            <asp:TextBox ID="txtLoginPassword" CssClass="loginPasswordText loginPasswordAdminText"
                                runat="server" TextMode="Password"></asp:TextBox>
                            <br />
                            <asp:Button ID="btnLoginSubmit" runat="server" Text="Login" CssClass="loginSubmit loginSubmitAdmin"
                                OnClick="btnLoginSubmit_Click"></asp:Button>
                        </div>
                    </asp:Panel>
                </div>
                <asp:Panel ID="pnlTools" runat="server" Visible="false">
                    <div id="newUser">
                        <table class="newUserPanel">
                            <tr>
                                <td>
                                    <asp:Label ID="lblNewName" runat="server" Text="<%# Resources.Messages.lbl_new_user_name %>"></asp:Label>:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNewName" runat="server" CssClass="newUserPanelTextBoxes"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Label ID="lblNewSurname" runat="server" Text="<%# Resources.Messages.lbl_new_user_surname %>"></asp:Label>:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNewSurName" runat="server" CssClass="newUserPanelTextBoxes"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblNewUserName" runat="server" Text="<%# Resources.Messages.lbl_new_user_username %>"></asp:Label>:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNewUserName" runat="server" CssClass="newUserPanelTextBoxes"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Label ID="lblNewPassword" runat="server" Text="<%# Resources.Messages.lbl_new_user_password %>"></asp:Label>:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNewPassword" runat="server" CssClass="newUserPanelTextBoxes"
                                        TextMode="Password"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblAdmin" runat="server" Text="<%$ Resources: Messages, lbl_admin %>"></asp:Label>
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkAdmin" runat="server" />
                                </td>
                                <td colspan="2">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4">
                                    <asp:Button ID="btnSaveNewUser" runat="server" Text="<%# Resources.Messages.btn_save_new_user %>"
                                        OnClick="btnSaveNewUser_Click" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <br />
                    <asp:GridView ID="grdUsers" runat="server" BorderStyle="Solid" BorderWidth="1" HeaderStyle-BackColor="#CC0000"
                        SortedAscendingHeaderStyle-VerticalAlign="NotSet" HeaderStyle-ForeColor="White"
                        BorderColor="Black" RowStyle-BorderStyle="Solid" RowStyle-BorderColor="Black"
                        RowStyle-BorderWidth="1" AutoGenerateColumns="false" OnRowCommand="grdUsers_RowCommand"
                        OnRowDeleting="grdUsers_RowDeleting" OnRowUpdating="grdUsers_RowUpdating" Width="50%">
                        <Columns>
                            <asp:BoundField DataField="id" HeaderText="ID" />
                            <asp:BoundField DataField="name" HeaderText="NAME" />
                            <asp:BoundField DataField="surname" HeaderText="SURNAME" />
                            <asp:BoundField DataField="username" HeaderText="USERNAME" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Button ID="btnUpdateUser" runat="server" Text='<%$ Resources: Messages, btn_update_user_button %>'
                                        CommandName="UPDATE" CommandArgument="<%# Container.DataItemIndex %>" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Button ID="btnSetAgencies" runat="server" Text='<%$ Resources: Messages, btn_set_agencies_for_user %>'
                                        CommandName="SET_AGENCIES" CommandArgument="<%# Container.DataItemIndex %>" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Button ID="btnDeleteUser" runat="server" Text='<%$ Resources: Messages, btn_delete_user_button %>'
                                        CommandName="DELETE" CommandArgument="<%# Container.DataItemIndex %>" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </asp:Panel>
            </div>
        </div>
        <!-- .middle-->
    </div>
    <!-- .wrapper -->
    <div id="df-User-update" class="popup_block">
        <asp:Label ID="lbl_title_update" runat="server" Text="<%$  Resources: Messages, lbl_update_user %>"
            CssClass="PageTitle"></asp:Label>
        <hr style="width: 100%;" />
        <asp:HiddenField ID="hiddenUserId" runat="server" />
        <table class="tableForm">
            <tr>
                <td>
                    <asp:Label ID="lbl_name_update" runat="server" Text="<%$  Resources: Messages, lbl_new_user_name %>"
                        CssClass="tdProperty"></asp:Label>:
                </td>
                <td>
                    <asp:TextBox ID="txt_name_update" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbl_surname_update" runat="server" Text="<%$  Resources: Messages, lbl_new_user_surname %>"
                        CssClass="tdProperty"></asp:Label>:
                </td>
                <td>
                    <asp:TextBox ID="txt_surname_update" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbl_username_update" runat="server" Text="<%$  Resources: Messages, lbl_new_user_username %>"
                        CssClass="tdProperty"></asp:Label>:
                </td>
                <td>
                    <asp:TextBox ID="txt_username_update" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbl_password_update" runat="server" Text="<%$  Resources: Messages, lbl_new_user_password %>"
                        CssClass="tdProperty"></asp:Label>:
                </td>
                <td>
                    <asp:TextBox ID="txt_password_update" TextMode="Password" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbl_admin_update" runat="server" Text="<%$ Resources: Messages, lbl_admin %>"></asp:Label>
                </td>
                <td>
                    <asp:CheckBox ID="chkAdminUpdate" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <center>
                        <asp:Label ID="lblErrorOnUpdate" runat="server" Text="" ForeColor="Red"></asp:Label><br />
                        <br />
                        <asp:Button ID="btnUpdateUser" runat="server" Text="<%$ Resources: Messages, btn_update_user_button %>"
                            OnClick="btnUpdateUser_Click" />
                    </center>
                </td>
            </tr>
        </table>
    </div>
    <div id="setAgencyDiv" class="popup_block">
        <asp:Label runat="server" ID="lblSelectAgencyScheme" Text="AgencyScheme:" />
        <asp:DropDownList ID="cmbAgencies" runat="server" Enabled="true" Visible="true" AutoPostBack="true"
            OnSelectedIndexChanged="cmbAgencies_SelectedIndexChanged">
        </asp:DropDownList>
        <br />
        <br />
        <center>
            <div id="setAgencyGridDiv" style="overflow: auto">
                <asp:HiddenField ID="selectedUserId" runat="server" />
                <asp:GridView ID="gridView" runat="server" AutoGenerateColumns="False" AllowPaging="false"
                    AllowSorting="True" BorderStyle="Solid" BorderWidth="1" HeaderStyle-BackColor="#CC0000"
                    SortedAscendingHeaderStyle-VerticalAlign="NotSet" HeaderStyle-ForeColor="White"
                    BorderColor="Black" RowStyle-BorderStyle="Solid" RowStyle-BorderColor="Black"
                    RowStyle-BorderWidth="1" Width="80%">
                    <Columns>
                        <asp:TemplateField HeaderText="ID">
                            <ItemTemplate>
                                <asp:Label ID="lblId" runat="server" Text='<%# Bind("Code") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="50px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="NAME">
                            <ItemTemplate>
                                <asp:Label ID="lblName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="50px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="DESCRIPTION">
                            <ItemTemplate>
                                <asp:Label ID="lblDescription" runat="server" Text='<%# Bind("Description") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="50px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Set" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkImportThisItem" runat="server" Enabled='<%# Bind("_isOk") %>'
                                    Checked="false" />
                            </ItemTemplate>
                            <HeaderStyle Width="50px" />
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="hs" />
                    <RowStyle CssClass="rs" />
                    <AlternatingRowStyle CssClass="ars" />
                </asp:GridView>
                <asp:Label ID="lblNoItemsAllowed" runat="server" Visible="false" Text="<%# Resources.Messages.lbl_no_items_allowed %>"></asp:Label>
            </div>
            <br />
            <asp:Button ID="btnSet" runat="server" Text="<%$ Resources: Messages, btn_set_agency %>"
                OnClientClick="BlockScreenForImport(); return true;" Visible="true" OnClick="btnSet_Click" />
            <asp:Button ID="btnConfirmSet" runat="server" Text="<%$ Resources: Messages, btn_confirm_set_agency %>"
                OnClientClick="BlockScreenForImport(); return true;" Visible="true" OnClick="btnConfirmSet_Click" />
            <asp:Button ID="btnCancelOperation" runat="server" Text="<%$ Resources: Messages, btn_cancel_set_agency %>"
                OnClientClick="BlockScreenForImport(); return true;" Visible="true" OnClick="btnCancelOperation_Click" />
        </center>
    </div>
    <footer>
        <hr />
	    <h1><%= Resources.Messages.lbl_developer %></h1>
        <h2><%= Resources.Messages.lbl_developer_address%></h2>
    </footer>
    <!-- .footer -->
    </form>
</body>
</html>
</html> 