<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CategorySchemeGraph.aspx.cs" Inherits="ISTATRegistry.CategorySchemeGraph" %>

<%@ Register Src="~/UserControls/FileDownload3.ascx" TagPrefix="uc1" TagName="FileDownload3" %>


<%@ Register Namespace="ISTATRegistry.Classes" TagPrefix="iup" Assembly="IstatRegistry" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <%--    <link href="css/jquery.msg.css" rel="stylesheet" />--%>
    <link href="css/popup.css" rel="stylesheet" />

    <script src="js/jquery-1.7.1.min.js" type="text/javascript"></script>

    <script src="js/PopUp.js" type="text/javascript"></script>
    <script type="text/javascript">
        //$(document).ready(function () {
        //    //$('#tab-container').easytabs();

        //    $("#FileDownload3_cmbDownloadType").change(function () {
        //        alert("changebefore");
        //    });

        //});
        
        //var prm = Sys.WebForms.PageRequestManager.getInstance();

        //prm.add_endRequest(function () {
        //    $("#FileDownload3_cmbDownloadType").change(function () {
        //        alert("changeafter");
        //    });
        //});

        function ShowDialog(text) {
            alert(text);
        }

    </script>
    <link href="css/tabs.css" rel="stylesheet" />

    <style>
        a:link {
            text-decoration: underline;
            color: black;
        }

        a:hover {
            color: purple;
            font-weight: bold;
        }

        a:active {
            color: purple;
        }

        .AspNet-TreeView {
            width: 200px;
            /*border-top: solid 1px #DDD;*/
        }

        .title {
            font-weight: bold;
            font-size: medium;
        }

        .label {
            font-weight: bold;
        }

        .tableDetails {
            border-collapse: collapse;
            font-size: 10pt;
            margin-top: 15px;
            margin-left: 15px;
            width: 820px;
        }

            .tableDetails th {
                background-color: #bababa;
            }

        /*
.AspNet-TreeView ul {
    list-style: none; 
}
 
.AspNet-TreeView-Leaf {
    border-bottom: solid 1px #DDD;
    background: url(../../images/structure/node-dot.gif) 8px 9px no-repeat;  
}
 
.AspNet-TreeView-Root {
    border-bottom: solid 1px #DDD; 
}
 
.AspNet-TreeView-Root a {
    display: block;
    width: 170px;
    margin-left: 20px;
    padding: 5px 5px 5px 5px; 
}
 
.AspNet-TreeView-Selected {
    background: #F6F6F6 url(../../images/structure/arrow-right.gif) 8px 9px no-repeat;
}
 
.AspNet-TreeView-Expand {
    display: block;
    float: left;
    margin: 9px 0px 0px 8px;
    padding: 6px 4px 5px 4px;
    height: 0px !important;
    background: url(../../images/structure/node-plus.gif) 0px 0px no-repeat;
    cursor: pointer;
}
 
.AspNet-TreeView-Collapse {
    display: block;
    float: left;
    margin: 9px 0px 0px 8px;
    padding: 6px 4px 5px 4px;
    height: 0px !important;
    background: url(../../images/structure/node-minus.gif) 0px 0px no-repeat;
    cursor: pointer;
}
 
.AspNet-TreeView-Show li {
      border-top: solid 1px #DDD;
      background-position: 28px 9px;
}
 
.AspNet-TreeView-Hide {
    display: none;
}
 
.AspNet-TreeView ul li ul li {
    text-indent: 20px;
    border-bottom: none;
    font-size: 11px;
}
     */
    </style>


</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
        <div style="float: left; width: 100%">
            <img src="./images/SdmxLogoBrowser.png" alt="Sdmx Logo" />
            <br />
        </div>

        <div id="tab-container" class='tab-container'>
            <ul class='etabs'>
                <li class='tab'><b>&nbsp;<%= Resources.Messages.lbl_categorisation_detail %>&nbsp;</b></li>
            </ul>
            <div class='panel-container' style="width: 1280px">
                <div id="general">
                    <%--<asp:UpdatePanel ID="UpdatePanel1" runat="server">--%>

                        <iup:IstatUpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <asp:UpdateProgress ID="updateProgress" runat="server">
                                <ProgressTemplate>
                                    <div style="position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; z-index: 9999999; background-color: #000000; opacity: 0.8;">
                                        <span style="border-width: 0px; position: fixed; padding: 10px 100px 10px 100px; background-color: #FFFFFF; font-size: 24px; left: 40%; top: 40%;">Loading ...</span>
                                    </div>
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                            <table width="980px">
                                <tr>
                                    <td colspan="2">
                                        <div style="background-color: ActiveCaption; width: 100%; height: 40px; border: solid 1px #999; -moz-border-radius: 4px 4px 0 0; -webkit-border-radius: 4px 4px 0 0; margin-left: 10px;">
                                            <div style="float: right; padding-right: 8px; padding-top: 5px">
                                                <asp:DropDownList ID="cmbLanguage" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbLanguage_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label2" runat="server" Text="<%$ Resources:Messages, lbl_ab_CategoryScheme%>"
                                                            CssClass="tdProperty"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="cmbCategorySchemes" runat="server" Width="200px">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox ID="chkDataflow" runat="server" Text="<%$ Resources:Messages, lbl_ab_Dataflow%>" />&nbsp;
                                                        <asp:CheckBox ID="chkCodelist" runat="server" Text="<%$ Resources:Messages, lbl_ab_Codelist%>" />&nbsp;
                                                        <asp:CheckBox ID="chkConcSchema" runat="server" Text="<%$ Resources:Messages, lbl_ab_ConceptSchema%>" />&nbsp;
                                                        <asp:CheckBox ID="chkDSD" runat="server" Text="<%$ Resources:Messages, lbl_ab_Dsdef%>" />&nbsp;
                                                    </td>
                                                    <td>
                                                        <asp:Button ID="btnView" runat="server" Text="<%$ Resources:Messages, lbl_ab_View%>" OnClick="btnView_Click" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td width="20%">
                                        <div style="overflow: auto; width: 330px; height: 500px; border: solid 1px #999; -moz-border-radius: 4px 4px 0 0; -webkit-border-radius: 4px 4px 0 0; margin-left: 10px;">
                                            <asp:TreeView ID="tvCategory" CssClass="AspNet-TreeView" runat="server" OnSelectedNodeChanged="tvCategory_SelectedNodeChanged" HoverNodeStyle-ForeColor="#FF3300">
                                                <SelectedNodeStyle BackColor="#DDDDDD" Font-Bold="True" />
                                            </asp:TreeView>
                                        </div>

                                    </td>
                                    <td width="80%">
                                        <div style="overflow: auto; width: 870px; height: 500px; border: solid 1px #999; -moz-border-radius: 4px 4px 0 0; -webkit-border-radius: 4px 4px 0 0; margin-left: 10px;">
                                            <span style="font-size:small;">
                                                <uc1:FileDownload3 runat="server" ID="FileDownload3" Visible="true" />
                                            </span>
                                            <asp:Panel ID="pnlResult" runat="server">
                                            </asp:Panel>
                                            <br />
                                            <br />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                        </iup:IstatUpdatePanel>
                <%--    </asp:UpdatePanel>--%>
                </div>
            </div>
        </div>

    </form>
</body>
</html>

