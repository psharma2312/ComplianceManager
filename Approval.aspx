<%@ Page Title="" Language="C#" MasterPageFile="~/ComplianceM.Master" AutoEventWireup="true" CodeBehind="Approval.aspx.cs" Inherits="ComplianceManager.Approval" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cntPlaceHolder1" runat="server">
<link rel="stylesheet" href="Content/custom.css" />
<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <link rel="stylesheet" type="text/css" href="Content/Approval.css" />
<script type="text/javascript">
    function openModal() {
        $('[id*=myModal]').modal('show');
    }
</script>
        <style type="text/css">
    .error-message {color: #721c24;background-color: #f8d7da;padding: 10px;border: 1px solid #f5c6cb;border-radius: 4px;margin-bottom: 15px;display: block;        }
   .details-table { width: 100%; border-collapse: collapse; margin-top: 10px; }
    .details-table td { padding: 5px; vertical-align: top; }
    .details-label { font-weight: bold; font-size: 12px; }
    .details-value { font-size: 12px; }
    </style>
<div id="inner_page">
    <div id="inner_page_main">
       <div class="heading">
     <asp:Label ID="Label4" runat="server" Text="Approval" />
 </div>
         <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" Visible="false"  />
        <div id="blankspace"></div>
        <div id="inner_page_top" class="filter-container">
            <div class="filter-label" style="width: 150px; text-align: left; height: 20px;">
               <asp:Label ID="lblFilter" runat="server" Text="Approval Status" AssociatedControlID="ddlStatus" CssClass="control-label" />
            </div>
            <div class="filter-controls" style="flex: 1;">
                <asp:DropDownList ID="ddlStatus" runat="server" DataTextField="ComplianceStatusName" DataValueField="ComplianceStatusId" Width="180px" Height="20px"  AutoPostBack="true" />
                 <asp:Button ID="btnLoad" runat="server" Text="Load" OnClick="btnLoad_Click" CssClass="load-button" />
            </div>
        </div>
        <div class="status-message" style="height: 20px;">
            <asp:Label ID="lblStatus" runat="server" Text="" Visible="false" BackColor="Yellow" />
        </div>
        <div id="inner_page_bottom">
            <div class="grid-container">
                <asp:GridView ID="GridView1" CssClass="frequency-grid" runat="server" Width="100%" AutoGenerateColumns="False"
                    BorderColor="#E7E7FF" BorderStyle="None"
                    BorderWidth="1px" GridLines="Both" Font-Size="Smaller" HeaderStyle-BackColor="#337ab7"
                    HeaderStyle-ForeColor="White" DataKeyNames="ComplianceID,ComplianceDetailID"
                    OnSelectedIndexChanged="OnSelectedIndexChanged"
                    OnPageIndexChanging="OnPageIndexChanging" PageSize="5" AllowPaging="true"
                    EmptyDataText="No Compliances for Approval." ShowHeaderWhenEmpty="true">
                    <Columns>
                        <asp:BoundField DataField="ComplianceDetailID" HeaderText="Id" ItemStyle-Width="40" HeaderStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="ComplianceRef" HeaderText="Compliance Ref" ItemStyle-Width="80" HeaderStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="ComplianceTypeName" HeaderText="Type" ItemStyle-Width="100" HeaderStyle-HorizontalAlign="Right" />
                        <%--<asp:BoundField DataField="GovernmentLegislation" HeaderText="Governing Legislation" ItemStyle-Width="350" HeaderStyle-HorizontalAlign="Right" />--%>
                        <asp:BoundField DataField="ActSectionReference" HeaderText="Act/Section" ItemStyle-Width="380" HeaderStyle-HorizontalAlign="Right" />
                        <%--<asp:BoundField DataField="NatureOfComplianceName" HeaderText="Compliance Nature" ItemStyle-Width="120" HeaderStyle-HorizontalAlign="Right" />--%>
                        <asp:BoundField DataField="EffectiveFrom" HeaderText="EffectiveFrom" DataFormatString="{0:dd-MMM-yyyy}" ItemStyle-Width="85" HeaderStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="StandardDueDate" HeaderText="Due Date" DataFormatString="{0:dd-MMM-yyyy}" ItemStyle-Width="90" HeaderStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="ComplianceStatusName" HeaderText="Status" ItemStyle-Width="120" />
                          <asp:TemplateField HeaderText="Details" ItemStyle-Width="240px">
                              <ItemTemplate>
                                  <span><%# GetTruncatedDetails(Eval("DetailsOfComplianceRequirements")) %></span>
                                  <asp:LinkButton ID="lnkBtnEdit" runat="server" Text="..." style="padding:0px 4px;font-size:8px;color: #fff;
                                          background-color: #5bc0de;vertical-align: middle; font-weight: 400;border: 1px solid transparent; 
                                          border-color: #46b8da;border-radius: 4px;" OnClick="Display"></asp:LinkButton>
                              </ItemTemplate>
                          </asp:TemplateField>
                        <asp:CommandField ShowSelectButton="True" ItemStyle-Font-Bold="true" ItemStyle-HorizontalAlign="Center" />
                    </Columns>
                    <RowStyle BackColor="#FFFFFF" /> 
                    <AlternatingRowStyle BackColor="#F0F0F0" /> 
                    <SelectedRowStyle BackColor="#FFFF00" /> 
                    <PagerStyle CssClass="pager-style" />
                    <EmptyDataTemplate>
                        <div align="center">No Compliances for Approval.</div>
                    </EmptyDataTemplate>
                </asp:GridView>
                               <div>
                    <div id="forgotPasswordModal" class="modal" role="dialog">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title">Details</h4>
                                    <span class="modal-close" onclick="hideModal()">×</span>
                                </div>
                                <div class="modal-body">
                                    <table class="details-table">
                                        <tr>
                                            <td  style="text-align:left;">
                                                <asp:Label ID="lblComplianceRefLabel" runat="server" Text="Compliance Ref:" CssClass="details-label" />
                                                <asp:Label ID="lblComplianceRef" runat="server" Text="" CssClass="details-value" />
                                            </td>
                                            <td style="text-align:left;">
                                                <asp:Label ID="lblComplianceAreaLabel" runat="server" Text="Compliance Nature:" CssClass="details-label" />
                                                <asp:Label ID="lblComplianceAreaValue" runat="server" Text="" CssClass="details-value" />
                                            </td>
                                        </tr>
                                         <tr>
                                            <td  style="text-align:left;">
                                                 <asp:Label ID="lblNonCompLabel" runat="server" Text="Non Compliance Penalty:" CssClass="details-label" />
                                                 <asp:Label ID="lblNonCompliance" runat="server" Text="" CssClass="details-value" />
                                             </td>
                                              <td colspan="4"  style="text-align:left;">
                                                 <asp:Label ID="lblActionLabel" runat="server" Text="Action to be taken:" CssClass="details-label" />
                                                 <asp:Label ID="lblActionToBeTaken" runat="server" Text="" CssClass="details-value" />
                                             </td>
                                         </tr>
   
 
                                        <tr>
                                            <td style="text-align:left;">
                                                 <asp:Label ID="lblActSecLabel" runat="server" Text="Act Section:" CssClass="details-label" />
                                                 <asp:Label ID="lblActSec" runat="server" Text="" CssClass="details-value" />
                                             </td>
                                            <td style="text-align:left;">
                                                <asp:Label ID="lblPrioritLabel" runat="server" Text="Priority:" CssClass="details-label" />
                                                <asp:Label ID="lblPrio" runat="server" Text="" CssClass="details-value" />
                                            </td>
                                        </tr>
                                        <tr>
                                           <td style="text-align:left;">
                                                <asp:Label ID="lblLawtLabel" runat="server" Text="LawType:" CssClass="details-label" />
                                                <asp:Label ID="lblLawt" runat="server" Text="" CssClass="details-value" />
                                            </td>
                                           <td style="text-align:left;">
                                               <asp:Label ID="lblDrivenBLabel" runat="server" Text="Driven By:" CssClass="details-label" />
                                               <asp:Label ID="lblDriv" runat="server" Text="" CssClass="details-value" />
                                           </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4" style="text-align:left;">
                                                <asp:Label ID="lblDetailsLabel" runat="server" Text="Details:" CssClass="details-label" />
                                                <asp:Label ID="lblDetails" runat="server" Text="" CssClass="details-value" />
                                            </td>
                                        </tr>
   
                                    </table>
                                   <%-- <div class="col-lg-12 col-sm-12 col-md-12 col-xs-12">
                                        <p style="margin: 0 0 10px;"><asp:Label ID="dscpp" runat="server" Text=""></asp:Label></p>
                                    </div>--%>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <table width="100%">
                <tr>
                    <td colspan="20" width="100%">
                    <asp:Label width="100%" ID="lblDocUpload" runat="server" style="background-color: #f1f8ff;padding: 4px;border-bottom: 2px solid #0066cc; margin-bottom: 8px;" 
                        Font-Bold="True" Font-Size="Large" Text="Compliance Details" Visible="false" />
                    </td>
                </tr>
                <tr>
                    <td colspan="10" style="text-align: right; height: 5px;">
                         <asp:Label ID="label1" runat="server" Text="Compliance Area :" BackColor="Yellow" Visible="false" />
                         <asp:TextBox ID="txtDetailsComplianceID" runat="server" Width="180px" Height="25px" Visible="false" />
                         <asp:Label ID="label2" runat="server" Text="Compliance Details:" BackColor="Yellow" Visible="false" />
                         <asp:TextBox ID="txtDetailsComplianceDetailID" runat="server" Width="180px" Height="25px" Visible="false" />
                        <asp:TextBox ID="txtComplianceRef" runat="server" Width="20px" Height="5px" Visible="false" />
                    </td>
                     <td colspan="1" style="width: 1%; text-align: right; height: 5px;">&nbsp; </td>
                     <td colspan="9" style="text-align:center; height: 5px;">
                        
                     </td>
                </tr>
                <tr>
                    <td style="width: 11%; text-align: right;" colspan="2">
                        <asp:Label ID="lblUserComments" runat="server" Text="Preparer Comments"  CssClass="control-label" Visible="false" /></td>
                    <td colspan="8" style="vertical-align: bottom;">
                        <asp:TextBox ID="txtUserComments" TextMode="multiline" Columns="70" Rows="3" BackColor="#dddddd" ReadOnly="true" runat="server" Visible="false" /></td>
                    <td colspan="1" style="text-align: right; height: 30px;">&nbsp; </td>
                    <td style="vertical-align: top; width: 38%; text-align: center; padding:0px 5px 5px 1px" colspan="9" rowspan="3">
                        <table width="100%">
    <tr><td>
        <asp:Label ID="lblUploadedDocuments" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Uploaded Master Documents" Visible="false" />
        <asp:GridView ID="GridView3" runat="server"  CssClass="frequency-grid" AutoGenerateColumns="false" EmptyDataText="No files uploaded"
            Width="90%" BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" DataKeyNames="ComplianceID, DocumentID"
            BorderWidth="1px" CellPadding="5" GridLines="Both" Font-Size="Smaller" HeaderStyle-BackColor="#337ab7"
            style="margin-top:0px;" HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#f2f2f2" ShowHeaderWhenEmpty="true">
             <Columns>
                 <asp:BoundField DataField="DocumentID" HeaderText="ID" ItemStyle-Width="40px" HeaderStyle-HorizontalAlign="Center" />
                 <asp:BoundField DataField="DocTypeName" HeaderText="Type" ItemStyle-Width="300px" HeaderStyle-HorizontalAlign="Center" />
                 <asp:BoundField DataField="FileName" HeaderText="File Name" ItemStyle-Width="350px" ItemStyle-Font-Bold="true" ItemStyle-Font-Size="X-Small" ItemStyle-HorizontalAlign="Left"/>
                 <asp:BoundField DataField="UploadedDate" HeaderText="UploadedOn" ItemStyle-Width="150px" ItemStyle-Font-Bold="true" ItemStyle-Font-Size="X-Small" ItemStyle-HorizontalAlign="Left"/>
                 <asp:TemplateField HeaderText="Action"  ItemStyle-Width="60px" HeaderStyle-HorizontalAlign="Center">
                     <ItemTemplate>
                          <asp:LinkButton ID="lnkDownload" Text="Download" CommandArgument='<%# Eval("DownloadPath") %>' 
                                  OnClientClick='<%# "window.location.href = \"FileDownloadHandler.ashx?fileName=" + Eval("DownloadPath") + "\"; return false;" %>' 
                                  runat="server" />
                     </ItemTemplate>
                 </asp:TemplateField>
                
             </Columns>
            <EmptyDataTemplate>
                <div align="center">No documents for compliance master found.</div>
            </EmptyDataTemplate>
        </asp:GridView>

        </td></tr>
    <tr><td>
        <asp:Label ID="lblUploadedDocuments1" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Uploaded User Documents" Visible="false" />
                                
                         <asp:GridView ID="GridView2" runat="server"  CssClass="frequency-grid" AutoGenerateColumns="false" EmptyDataText="No files uploaded" OnRowCommand="GridView2_RowCommand"
                         Width="90%" BackColor="White" BorderColor="#E7E7FF" BorderStyle="None"
                         BorderWidth="1px" CellPadding="5" GridLines="Both" Font-Size="Smaller" HeaderStyle-BackColor="#337ab7"
                         style="margin-top:0px;" HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#f2f2f2" ShowHeaderWhenEmpty="true">
                        <Columns>
                            <asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="40px" HeaderStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="FileName" HeaderText="File Name" ItemStyle-Font-Bold="true" ItemStyle-Font-Size="Smaller" ItemStyle-HorizontalAlign="Left" ItemStyle-Font-Names="Verdana"/>
                            <asp:BoundField DataField="ComplianceId" HeaderText="Compliance ID" ItemStyle-Width="100px"   />
                            <asp:TemplateField>
                                <ItemTemplate>
                                     <asp:LinkButton ID="lnkDownload" Text="Download" 
                                            CommandArgument='<%# Eval("DownloadPath") %>' 
                                            OnClientClick='<%# "window.location.href = \"FileDownloadHandler.ashx?fileName=" + Eval("DownloadPath") + "\"; return false;" %>' 
                                            runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--<asp:TemplateField>
                                <ItemTemplate>
                                   <asp:LinkButton ID="lnkDelete" runat="server" Text="Delete" 
                                        OnClientClick='<%# "deleteFile(\"" + Eval("DownloadPath") + "\", \"" + Eval("ComplianceId") + "\", \"" + Eval("ID") + "\"); return false;" %>' />
                                </ItemTemplate>
                            </asp:TemplateField>--%>
                        </Columns>
                        <EmptyDataTemplate>
                            <div align="center">No documents found.</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                        </td></tr>
</table>

                    </td>
                </tr>
                <tr>
 <td style="width: 15%; text-align: right;" colspan="2">
      <asp:Label ID="lblUpdatedComments" runat="server" Text="Approver Comments"  CssClass="control-label" Visible="false" /></td>
       <td colspan="8" style="vertical-align: bottom;">
           <asp:TextBox ID="txtUpdatedComments" TextMode="multiline" BackColor="#dddddd" Columns="70" Rows="3" ReadOnly="true" runat="server" Visible="false" />
           </td>
                     <td colspan="1" style="text-align: right; height: 30px;">&nbsp; </td>

                </tr>
                <tr>
                    <td colspan="2" style="width: 15%; text-align: right;" height: "30px;">
    <asp:Label ID="lblNewUserComments" runat="server"  CssClass="control-label" Text="New Comments:" Visible="false" /> </td>
<td  colspan="8">
    <asp:TextBox ID="txtNewComments" runat="server" Width="530px" Visible="false" /></td>
<td colspan="1" >&nbsp; </td>
                </tr>
                <tr>
                    <td colspan="2" style="width: 15%; text-align: right;" height: "30px;"></td>
                    <td colspan="9" style="width: 600px; text-align: left; height: 30px;"> 
                        <asp:Button ID="btnApprove" runat="server"  CssClass="add-button" Text="Approve" OnClick="btnApprove_Click" Visible="false" />  
                        <asp:Button ID="btnReject" runat="server"  CssClass="reject-button" Text="Reject" OnClick="btnReject_Click" Visible="false" />
                   <asp:TextBox ID="txtEmail" runat="server" Visible="false" />
                        </td>
                </tr>
            </table>
        </div>
    </div>
</div>

 <script type="text/javascript">
     function showModal() {
         document.getElementById('forgotPasswordModal').style.display = 'block';
     }
     function hideModal() {
         document.getElementById('forgotPasswordModal').style.display = 'none';
     }
     function deleteFile(fileName, complianceId, complianceDetailId) {
         console.log("deleteFile called with fileName: " + fileName + ", complianceId: " + complianceId + ", complianceDetailId: " + complianceDetailId);
         $.ajax({
             type: "GET",
             //url: "FileDeleteHandler.ashx?fileName=" + encodeURIComponent(fileName) + "&complianceId=" + complianceId,
             url: "FileDeleteHandler.ashx?fileName=" + encodeURIComponent(fileName) + "&complianceId=" + complianceId + "&documentID=-1",
             success: function (response) {
                 console.log("AJAX success: " + response);
                 if (response === "Success") {
                     console.log("Deletion successful, reloading grid...");
                     reloadGrid(complianceDetailId, complianceId); // Pass parameters to reload
                 } else {
                     console.log("Deletion failed: " + response);
                     alert("Failed to delete the file.");
                 }
             },
             error: function (xhr, status, error) {
                 console.log("AJAX error: Status = " + status + ", Error = " + error + ", Response = " + xhr.responseText);
                 console.log("AJAX error: " + error);
                 alert("An error occurred while deleting the file.");
             }
         });
     }
     function reloadGrid(complianceDetailId, complianceId) {
         if (typeof updatePanelClientId !== 'undefined') {
             console.log("Triggering postback with args: Reload|" + complianceDetailId + "|" + complianceId);
             __doPostBack(updatePanelClientId, 'Reload|' + complianceDetailId + '|' + complianceId);
         } else {
             console.error('UpdatePanel client ID is not defined.');
         }
     }
 </script>
</asp:Content>