<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="UploadStructure.aspx.cs" Inherits="ISTATRegistry.UploadStructure" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        #loading
        {
            width: 280px;
            height: 280px;
            background-color: #c0c0c0;
            position: absolute;
            left: 50%;
            top: 50%;
            margin-top: -140px;
            margin-left: -140px;
            text-align: center;
        }
    </style>
    <script language="javascript" type="text/javascript">
        function showWait() {
            if ($("#<%=uploadedFiles.ClientID %>").val() != "") {
                $.blockUI();
            }
        }

        var itemsAreSelected = false;

        function SelectDeselectAll() {
            if (itemsAreSelected) {
                $("#<%=gridView.ClientID %> input[type=checkbox]:not(:disabled)").attr('checked', false);
                itemsAreSelected = false;
            }
            else {
                $("#<%=gridView.ClientID %> input[type=checkbox]:not(:disabled)").attr('checked', true);
                itemsAreSelected = true;
            }
        }

        function BlockScreenForImport() {
            var numberOfSelectedItems = $("#<%=gridView.ClientID %> input[type=checkbox]:checked").length;
            if (numberOfSelectedItems > 0) {
                var myDiv = $("#importedItemsGridDiv");
                myDiv.hide();
                $.blockUI();
            }
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnUploadFile" />
        </Triggers>
        <ContentTemplate>
            <h3 class="PageTitle"><%= Resources.Messages.lbl_upload_structure %></h3>
            <hr style="width: 100%" />
            <div id="fUpload">
            <br />
            <p style="width: 100%">
                <asp:Label runat="server" ID="lblLoadFileAsmx" Text="<%# Resources.Messages.lbl_select_file_sdmxml + ':'%>" CssClass="PageTitle2" />             
                <asp:FileUpload runat="server" ID="uploadedFiles" CssClass="inputUploader" Visible="true" EnableTheming="true" />
                </p>
                <br /><br /><br />
                <asp:Button ID="btnUploadFile" runat="server" Text="<%# Resources.Messages.btn_upload_file %>" UseSubmitBehavior="true"
                    OnClick="btnUploadFile_Click" OnClientClick="javascript:showWait();"/>
            </div>
            <div id="dialog-form" class="popup_block" title="Upload-Info">
                <asp:Label runat="server" ID="lbl" Text="<%# Resources.Messages.lbl_importated_aterfacts %>" CssClass="PageTitle" />
                <hr style="width: 100%" />
                <div style="height: 200px; overflow:auto">
                    <asp:Label runat="server" ID="lblInfo" Text="" CssClass="PageTitle2" />
                </div>
            </div>
            <div id= "importedItemsGridDiv" class="popup_block" >
            <input id="chbSelectAll" type="checkbox" onchange= "SelectDeselectAll()"/>&nbsp;<asp:Label runat="server" ID="lblSelectAll" Text="<%# Resources.Messages.lbl_select_all_items %>"/> 
            <br /><br />           
            <center>          
            <div id= "importGridDiv" style="overflow: auto">   
                <asp:GridView 
                    ID="gridView" 
                    runat="server"
                    CssClass="Grid" 
                    AutoGenerateColumns="False"    
                    AllowPaging="false"              
                    PagerSettings-Mode="NumericFirstLast"   
                    PagerSettings-FirstPageText="<%= Resources.Messages.btn_goto_first %>"
                    PagerSettings-LastPageText="<%= Resources.Messages.btn_goto_last %>"
                    AllowSorting="True"
                    OnPageIndexChanging="OnPageIndexChanging" 
                    PagerSettings-Position="TopAndBottom">
            
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

                        <asp:TemplateField HeaderText="TYPE" SortExpression="Version">
                            <ItemTemplate>
                                <asp:Label ID="lblType" runat="server" Text='<%# Bind("_type") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="50px" />
                        </asp:TemplateField>

                         <asp:TemplateField HeaderText="Import" ItemStyle-HorizontalAlign ="Center" >
                            <ItemTemplate>
                                <asp:CheckBox ID="chkImportThisItem" runat="server" Enabled='<%# Bind("_isOk") %>' Checked="false" />
                            </ItemTemplate>
                            <HeaderStyle Width="50px" />
                        </asp:TemplateField>

                    </Columns>

                    <HeaderStyle CssClass="hs" />
                    <RowStyle CssClass="rs" />
                    <AlternatingRowStyle CssClass="ars" />
                    <PagerStyle CssClass="pgr"></PagerStyle>

                </asp:GridView>
                <asp:Label ID="lblNoItemsAllowed" runat="server" Visible="false" Text="<%# Resources.Messages.lbl_no_items_allowed %>"></asp:Label>
                </div>
                <br />

                <asp:Button ID="btnConfirmImport" runat="server" Text="<%# Resources.Messages.btn_confirm_import %>" OnClientClick = "BlockScreenForImport(); return true;" 
                    onClick="btnConfirmImport_Click" Visible="false"/>
                </center>
                
                </div>
            <asp:Label ID="lblWait" runat="server" BackColor="#507CD1" Font-Bold="True" ForeColor="White" Text="<%# Resources.Messages.lbl_please_wait %>" style="display:none"></asp:Label>
       
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
