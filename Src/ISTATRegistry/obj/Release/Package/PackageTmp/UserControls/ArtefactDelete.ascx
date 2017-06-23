<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ArtefactDelete.ascx.cs"
    Inherits="ISTATRegistry.UserControls.ArtefactDelete" %>
<div id="delete-form<%=ReplInvChar(ucID) + '_' + ReplInvChar(ucAgency) + '_' + ucVersion.Replace( '.', '_' )%>" class="popup_block">
<asp:Label ID="lbl_title" Text="<%# Resources.Messages.lbl_delete_question %>" runat="server" Visible="true" style="font-weight:bold" />
    <hr style="width: 100%" />
    
    <table class="tbNoBorder">
        <tr>
            <td style="padding-bottom: 10px">
                <asp:Label ID="lbl_type" runat="server" Text="<%# Resources.Messages.lbl_artefact_type+':' %>" CssClass="tdProperty"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblArtefactType" Text="" runat="server"/>
            </td>
        </tr>
        <tr>
            <td style="padding-bottom: 10px">
                <asp:Label ID="lbl_id" runat="server" Text="<%# Resources.Messages.lbl_id+':' %>" CssClass="tdProperty"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblID" Text="" runat="server"/>
            </td>
        </tr>
        <tr>
            <td style="padding-bottom: 10px">
                <asp:Label ID="lbl_agency" runat="server" Text="<%# Resources.Messages.lbl_agency+':' %>" CssClass="tdProperty"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblAgency" Text="" runat="server"/>
            </td>
        </tr>
        <tr>
            <td style="padding-bottom: 10px">
                <asp:Label ID="lbl_version" runat="server" Text="<%# Resources.Messages.lbl_version+':' %>" CssClass="tdProperty"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblVersion" Text="" runat="server"/>
            </td>
        </tr>
    </table>
    
    <hr style="width: 100%" />
    
    <script>
        function SwitchDeleteButtonStatus(disabled) {
            $("#<%=ucButtonClientId%>").prop("disabled", disabled);
        }
    </script>
    <div style="float: right">
        <asp:Button ID="btnDelete" runat="server" UseSubmitBehavior="false" Text= "<%# Resources.Messages.btn_delete %>" OnClick="btnDelete_Click"  />
    </div>

</div>

<a ID= "<%=ucID + "_" + ucAgency + "_" + ucVersion + "_link" %>" href="#?w=500" rel="delete-form<%=ucID + "_" + ucAgency + "_" + ucVersion.Replace( '.', '_' )%>" class="poplight" title="<%= Resources.Messages.btn_delete+':' %>" onclick=" if ( <%= ucCanDeleteThis %> ) { openPopUp('delete-form<%=ReplInvChar(ucID) + '_' + ReplInvChar(ucAgency) + '_' + ucVersion.Replace( '.', '_' )%>') } else { ShowDialog( '<%= Resources.Messages.msg_error_deleting%>', 400, '<%= Resources.Messages.msg_generic_error_messagge%>' ); }">
    <img src="./images/delete2.png" style="border:none" alt="<%= Resources.Messages.btn_delete+':' %>" title="Delete Artefact"/>
</a>





