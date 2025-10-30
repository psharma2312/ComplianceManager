<%@ Page Title="" Language="C#" MasterPageFile="~/ComplianceM.Master" AutoEventWireup="true" CodeBehind="InvalidComplianceMasster.aspx.cs" Inherits="ComplianceManager.InvalidComplianceMasster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cntPlaceHolder1" runat="server">
<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
<link rel="stylesheet" type="text/css" href="Content/common.css" />
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<style type="text/css">
    .error-message {color: #721c24;background-color: #f8d7da;padding: 10px;border: 1px solid #f5c6cb;border-radius: 4px;margin-bottom: 15px;display: block;        }
   
    .compliance-grid {width: 100%;border-collapse: collapse;margin-top: 15px;font-family: Arial, sans-serif;}
    .compliance-grid th {background-color: #0066cc; color: white;padding: 3px;text-align: left;font-size:10px;border-bottom: 1px solid #004d99;}
    .compliance-grid td {padding: 0px;border-bottom: 0px solid #ddd;font-size:9px;}
    .compliance-grid tr:hover {background-color: #f5f5f5; /* Light gray background on hover for better interactivity */
    }
    .upload-button {background-color: #007bff;color: white;border: none;padding: 2px 10px;border-radius: 4px;cursor: pointer;}/* Green */
    .upload-button:hover {background-color: #0069d9;}
    .save-button {background-color: #28a745; color: white;border: none;padding: 2px 10px;border-radius: 4px;cursor: pointer;}/* Green */
    .save-button:hover {background-color: #218838;}
    .grid-button {padding: 5px 10px;border: none;border-radius: 4px;cursor: pointer;}
    
    
    .modal {display: none;position: fixed;top: 0;left: 0;width: 100%;height: 100%;background-color: rgba(0,0,0,0.5);z-index: 1000;}
    .modal-content {
    background-color: #fff;
    margin: 15% auto;
    padding: 8px;
    border-radius: 5px;
    width: 80%;
    max-width: 600px;
    text-align: left;
    font-size: 14px;
    position: relative;
    min-height: 200px;
    box-shadow: 0 4px 8px rgba(0,0,0,0.1); /* Subtle shadow for depth */
}
    .modal-close {
    position: absolute;
    top: 10px;
    right: 15px;
    font-size: 18px;
    cursor: pointer;
    color: #333;
}
    .modal-header {
    min-height: 16.43px;
    padding: 10px;
    border-bottom: 2px solid #e5e5e5;
}
    .modal-title {
    margin: 0;
    line-height: 1.42857143;
    color: #0066cc;
    font-family: Verdana;
    font-size: 14px;
    font-weight: bold;
}
.modal-body {
    position: relative;
    padding: 15px;
}
    .modal-close:hover {color: #000;}
    .control-div {
        margin: 10px 0;
    }
    .label-names {
        display: inline-block;
        width: 120px;
        font-size: 12px;
        font-weight: bold;
    }

.details-table { width: 100%; border-collapse: collapse; margin-top: 10px; }
    .details-table td { padding: 5px; vertical-align: top; }
    .details-label { font-weight: bold; font-size: 12px; }
    .details-value { font-size: 12px; }
    </style>
<div> 

<div class="heading">
    <asp:Label ID="Label4" runat="server" Text="Compliance Master" />
</div>
<asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" Visible="false" />

<div style="text-align: left; margin-top: 0px;"></div>

<asp:GridView ID="gvCompliances" runat="server" AutoGenerateColumns="false" OnPageIndexChanging="gvCompliances_PageIndexChanging" 
    OnSelectedIndexChanged="OnSelectedIndexChanged" CssClass="frequency-grid" DataKeyNames="ComplianceId" Width="100%" AllowPaging="true" GridLines="Both"
    HeaderStyle-HorizontalAlign="Center" BorderColor="#E7E7FF" BorderStyle="None" BackColor="White">
    <Columns>
        <asp:TemplateField HeaderText="SNo" ItemStyle-Width="20px" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Label ID="lblSNo" runat="server" Text='<%# Container.DataItemIndex + 1 %>' />
            </ItemTemplate>
        </asp:TemplateField>
         <asp:TemplateField HeaderText="Comp Ref" ItemStyle-Width="30px">
     <ItemTemplate>
         <asp:Label ID="lblComplianceArea" runat="server" Text='<%# Eval("ComplianceRef") %>' />
     </ItemTemplate>
   
 </asp:TemplateField>
        <asp:TemplateField HeaderText="BU" ItemStyle-Width="50px">
            <ItemTemplate>
                <asp:Label ID="lblBusinessUnit" runat="server" Text='<%# Eval("BusinessUnitName") %>' />
            </ItemTemplate>
           
        </asp:TemplateField>
            <asp:TemplateField HeaderText="Department" ItemStyle-Width="100px">
                <ItemTemplate>
                    <asp:Label ID="lblDepartmentFunction" runat="server" Text='<%# Eval("DepartmentName") %>' />
                </ItemTemplate>
             
            </asp:TemplateField>
           
            <asp:TemplateField HeaderText="Compliance Nature" ItemStyle-Width="100px">
                <ItemTemplate>
                    <asp:Label ID="lblNatureOfCompliance" runat="server" Text='<%# Eval("NatureOfComplianceName") %>' />
                </ItemTemplate>
              
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Compliance Type" ItemStyle-Width="160px">
                <ItemTemplate>
                    <asp:Label ID="lblTypeOfCompliance" runat="server" Text='<%# Eval("ComplianceTypeName") %>' />
                </ItemTemplate>
            
            </asp:TemplateField>
         
            <asp:TemplateField HeaderText="Frequency" ItemStyle-Width="70px">
                <ItemTemplate>
                    <asp:Label ID="lblFrequency" runat="server" Text='<%# Eval("FrequencyName") %>' />
                </ItemTemplate>
          
            </asp:TemplateField>
  
            <asp:TemplateField HeaderText="Effective From" ItemStyle-Width="75px">
                <ItemTemplate>
                    <asp:Label ID="lblEffectiveFromDate" runat="server" Text='<%# Eval("EffectiveFrom", "{0:dd-MM-yyyy}") %>' />
                </ItemTemplate>
          
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Effetive Till" ItemStyle-Width="75px">
                <ItemTemplate>
                    <asp:Label ID="lblStandardDueDate" runat="server" Text='<%# Eval("StandardDueDate", "{0:dd-MM-yyyy}") %>' />
                </ItemTemplate>
            
            </asp:TemplateField>
            <asp:TemplateField HeaderText="First DueOn" ItemStyle-Width="75px">
                <ItemTemplate>
                    <asp:Label ID="lblFirstDueOn" runat="server" Text='<%# Eval("FirstDueDate", "{0:dd-MM-yyyy}") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Initiator" ItemStyle-Width="100px">
                <ItemTemplate>
                    <asp:Label ID="lblInitiator" runat="server" Text='<%# Eval("InitiatorName") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Approver" ItemStyle-Width="100px">
                <ItemTemplate>
                    <asp:Label ID="lblApproverName" runat="server" Text='<%# Eval("ApproverName") %>' />
                </ItemTemplate>
            </asp:TemplateField>

        <asp:TemplateField HeaderText="Details" ItemStyle-Width="30px">
            <ItemTemplate>
                <div style="display: flex; align-items: center; gap: 5px;">
                    <asp:Button ID="btnShowDetails" runat="server" Text="More" CssClass="more-button" OnClick="Display" />
                </div>
            </ItemTemplate>
        </asp:TemplateField>    
        <asp:CommandField ControlStyle-Font-Size="11px" HeaderText="Action" ItemStyle-Width="30px" ShowSelectButton="True" ItemStyle-Font-Bold="true" ItemStyle-HorizontalAlign="Center" />
</Columns>
    <RowStyle BackColor="#FFFFFF" /> 
    <AlternatingRowStyle BackColor="#F0F0F0" /> 
    <SelectedRowStyle BackColor="#FFFF00" /> 
    <PagerStyle CssClass="pager-style" />
</asp:GridView>

<div>
    <table width="100%">
<tr>
    <td width="100%">
        <asp:Label width="100%" ID="lblDocUpload" runat="server" style="background-color: #f1f8ff;padding: 7px;border-bottom: 2px solid #0066cc; margin-bottom: 10px;" 
            Font-Bold="True" Font-Size="Large" Text="Document Upload" Visible="false" />
    </td>
</tr>
                 
<tr>
    <td style="width: 150px; text-align: left;" colspan ="4">
        <asp:Label ID="lblDocType" runat="server" CssClass="control-label" style="width:30px;" Text="Type" visible="false"/>&nbsp;&nbsp;&nbsp;
        <asp:DropDownList ID="ddlDocType" runat="server" DataTextField="DocumentTypeName" DataValueField="DocumentTypeId" Width="200px" Height="20px" Visible="false" />&nbsp;&nbsp;&nbsp;
        <asp:Label ID="lblFileUpload" runat="server" Text="" style="width:15px;" CssClass="control-label" Visible="false" />
        &nbsp;&nbsp;&nbsp;<asp:FileUpload ID="FileUpload1" runat="server" multiple="multiple" Visible="false"/>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Label ID="hdnComplianceID" runat="server" Text="" style="display:none;" />
    </td>
</tr>

<tr>
    <td style="vertical-align: top; width: 400px; text-align: left; padding:0px 5px 5px 1px;">
        <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="false" BackColor="White" BorderColor="#E7E7FF" BorderStyle="None"
            BorderWidth="1px" CellPadding="5" GridLines="Both" Font-Size="Smaller" HeaderStyle-BackColor="#337ab7" HeaderStyle-ForeColor="White" 
            DataKeyNames="ComplianceID, DocumentID" CssClass="frequency-grid"
            AlternatingRowStyle-BackColor="#f2f2f2" ShowHeaderWhenEmpty="true" EmptyDataText="No Documents available." >
            <Columns>
                <asp:TemplateField HeaderText="SNo" ItemStyle-Width="20px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label ID="lblSNo" runat="server" Text='<%# Container.DataItemIndex + 1 %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="DocumentID" HeaderText="ID" ItemStyle-Width="40px" HeaderStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="DocTypeName" HeaderText="Type" ItemStyle-Width="300px" HeaderStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="FileName" HeaderText="File Name" ItemStyle-Width="350px" ItemStyle-Font-Bold="true" ItemStyle-Font-Size="X-Small" ItemStyle-HorizontalAlign="Left"/>
                <asp:BoundField DataField="UploadedDate" HeaderText="UploadedOn" ItemStyle-Width="150px" ItemStyle-Font-Bold="true" ItemStyle-Font-Size="X-Small" ItemStyle-HorizontalAlign="Left"/>
                <asp:TemplateField HeaderText="Action"  ItemStyle-Width="60px" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%--<asp:LinkButton ID="lnkDownload" Text="Download" CommandArgument='<%# Eval("FileName") %>' runat="server" OnClick="DownloadFile"></asp:LinkButton>--%>
                         <asp:LinkButton ID="lnkDownload" Text="Download" CommandArgument='<%# Eval("DownloadPath") %>' 
                                 OnClientClick='<%# "window.location.href = \"FileDownloadHandler.ashx?fileName=" + Eval("DownloadPath") + "\"; return false;" %>' 
                                 runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Action"  ItemStyle-Width="60px" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkDelete" runat="server" Text="Delete" 
                            OnClientClick='<%# "deleteFile(\"" + Eval("DownloadPath") + "\", \"" + Eval("ComplianceId") + "\", \"" + Eval("DocumentID") + "\"); return false;" %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <div align="center">No documents found.</div>
            </EmptyDataTemplate>
        </asp:GridView>
        <Triggers>
</Triggers>
    </td>
</tr>
</table>
    </div>
    </div>
<div>
    <div id="forgotPasswordModal" class="modal" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Compliance Details</h4>
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
                </div>
            </div>
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
        function deleteFile(fileName, complianceId, DocumentID) {
            $.ajax({
                type: "GET",
                url: "FileDeleteHandler.ashx?fileName=" + encodeURIComponent(fileName) + "&complianceId=" + complianceId + "&documentID=" + DocumentID,
                success: function (response) {
                    if (response === "Success") {
                        reloadGrid(complianceId); // Pass parameters to reload
                    } else {
                        alert("Failed to delete the file.");
                    }
                },
                error: function (xhr, status, error) {
                    alert("An error occurred while deleting the file.");
                }
            });
        }
        function reloadGrid(complianceId) {
            if (typeof updatePanelClientId !== 'undefined') {
                __doPostBack(updatePanelClientId, 'Reload|' + complianceId);
            } else {
                console.error('UpdatePanel client ID is not defined.');
            }
        }
    </script>
</asp:Content>
