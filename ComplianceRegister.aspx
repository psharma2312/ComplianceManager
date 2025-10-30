<%@ Page Title="" Language="C#" MasterPageFile="~/ComplianceM.Master" AutoEventWireup="true" CodeBehind="ComplianceRegister.aspx.cs" Inherits="ComplianceManager.ComplianceRegister" %>
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
    .upload-button {background-color: #007bff;color: white;border: none;padding: 4px 10px;border-radius: 4px;cursor: pointer;}/* Green */
    .upload-button:hover {background-color: #0069d9;}
    .save-button {background-color: #28a745; color: white;border: none;padding: 4px 10px;border-radius: 4px;cursor: pointer;}/* Green */
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
    <asp:Label ID="Label4" runat="server" Text="Compliance Register" />
</div>
<asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" Visible="false" />
<div style="display: flex; gap: 0px; margin: 5px 0; width: 100%;">
    <!-- First Column (leftmost, Upload and Search) -->
    <div style="flex: 0 0 33%; min-width: 0;">
        <div class="control-div">
            <asp:Label ID="lblDepartmentFunction" runat="server" Text="Department" AssociatedControlID="ddlDepartmentFunction" CssClass="control-label" style="width: 120px;" />
            <asp:DropDownList ID="ddlDepartmentFunction" runat="server" style="width: 180px;height:20px; font-size:12px;"></asp:DropDownList>
        </div>
        <div class="control-div">
            <asp:Label ID="lblNatureOfCompliance" runat="server" Text="Compliance Nature" AssociatedControlID="ddlNatureOfCompliance" CssClass="control-label" style="width: 120px;" />
            <asp:DropDownList ID="ddlNatureOfCompliance" runat="server" style="width: 180px;height:20px; font-size:12px;"></asp:DropDownList>
        </div>

       
    </div>
    <!-- Second Column (middle, Export and Sample) -->
    <div style="flex: 0 0 33%; min-width: 0;">
        <div class="control-div">
             <asp:Label ID="lblTypeOfCompliance" runat="server" Text="Compliance Type" AssociatedControlID="ddlTypeOfCompliance" CssClass="control-label" style="width: 120px;" />
             <asp:DropDownList ID="ddlTypeOfCompliance" runat="server" style="width: 180px;height:20px; font-size:12px;"></asp:DropDownList>
         </div>
        <div  class="control-div">
             <asp:Label ID="lblPriority" runat="server" Text="Priority" AssociatedControlID="ddlPriority" CssClass="control-label" style="width: 120px;" />
             <asp:DropDownList ID="ddlPriority" runat="server" style="width: 180px;height:20px; font-size:12px;"></asp:DropDownList>
         </div>
        <div style="margin: 10px 0;"></div>
    </div>
        <!-- Third Column (rightmost, Search) -->
        <div style="flex: 0 0 33%; min-width: 0;">
            <div style="margin: 10px 0; height: 100%; display: flex; flex-direction: column; gap: 1px;">
                <div style="flex: 1;">
                    <div style="display: flex; gap: 10px;">
                        <div style="margin: 10px 0;">
                            <asp:Button ID="btnLoad" runat="server" Text="Load Data" OnClick="btnLoad_Click" CssClass="save-button" 
style="width: 80px; font-size:12px;" />
                                
                            <asp:Button ID="btnAll" runat="server" Text="Reset" OnClick="btnAll_Click" CssClass="upload-button" 
style="width: 70px;margin-left:15px; font-size:12px;" />
                            <asp:Button ID="btnExport" runat="server" Text="Export Report" OnClick="btnExport_Click" CssClass="save-button" 
style="width: 100px;margin-left:15px; font-size:12px;" />
                        </div>
                     
                    </div>
                </div>
                <!-- Placeholder to fill height -->
                <div style="flex: 1;"></div>
            </div>
        </div>
    </div>
<div style="text-align: left; margin-top: 0px;"></div>

<asp:GridView ID="gvCompliances" runat="server" AutoGenerateColumns="false" OnPageIndexChanging="gvCompliances_PageIndexChanging" 
     CssClass="frequency-grid" DataKeyNames="ComplianceId" Width="100%" AllowPaging="true" GridLines="Both" PageSize="15"
    HeaderStyle-HorizontalAlign="Center" BorderColor="#E7E7FF" BorderStyle="None" BackColor="White"
    EmptyDataText="No records for selected criteria." ShowHeaderWhenEmpty="true">
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
            <EditItemTemplate>
                <asp:TextBox ID="txtEditBusinessUnit" runat="server" Text='<%# Bind("BusinessUnitName") %>' Width="130px" />
            </EditItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Department" ItemStyle-Width="100px">
                <ItemTemplate>
                    <asp:Label ID="lblDepartmentFunction" runat="server" Text='<%# Eval("DepartmentName") %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlEditDepartmentFunction" runat="server" Width="130px">
                    </asp:DropDownList>
                    <asp:HiddenField ID="hfEditDepartmentFunction" runat="server" Value='<%# Bind("DepartmentName") %>' />
                </EditItemTemplate>
            </asp:TemplateField>
        <asp:TemplateField HeaderText="Compliance Nature" ItemStyle-Width="100px">
                <ItemTemplate>
                    <asp:Label ID="lblNatureOfCompliance" runat="server" Text='<%# Eval("NatureOfComplianceName") %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlEditNatureOfCompliance" runat="server" Width="130px"></asp:DropDownList>
                    <asp:HiddenField ID="hfEditNatureOfCompliance" runat="server" Value='<%# Bind("NatureOfComplianceName") %>' />
                </EditItemTemplate>
            </asp:TemplateField>
        <asp:TemplateField HeaderText="Compliance Type" ItemStyle-Width="160px">
                <ItemTemplate>
                    <asp:Label ID="lblTypeOfCompliance" runat="server" Text='<%# Eval("ComplianceTypeName") %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlEditTypeOfCompliance" runat="server" Width="160px">
                    </asp:DropDownList>
                    <asp:HiddenField ID="hfEditTypeOfCompliance" runat="server" Value='<%# Bind("ComplianceTypeName") %>' />
                </EditItemTemplate>
            </asp:TemplateField>
        <asp:TemplateField HeaderText="Frequency" ItemStyle-Width="70px">
                <ItemTemplate>
                    <asp:Label ID="lblFrequency" runat="server" Text='<%# Eval("FrequencyName") %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlEditFrequency" runat="server" Width="130px" />
                    <asp:HiddenField ID="hfEditFrequency" runat="server" Value='<%# Bind("FrequencyName") %>' />
                </EditItemTemplate>
            </asp:TemplateField>
        <asp:TemplateField HeaderText="Effective From" ItemStyle-Width="75px">
                <ItemTemplate>
                    <asp:Label ID="lblEffectiveFromDate" runat="server" Text='<%# Eval("EffectiveFrom", "{0:dd-MM-yyyy}") %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtEditEffectiveFromDate" runat="server" Text='<%# Bind("EffectiveFrom", "{0:dd-MM-yyyy}") %>' TextMode="Date" Width="130px" />
                </EditItemTemplate>
            </asp:TemplateField>
        <asp:TemplateField HeaderText="Effetive Till" ItemStyle-Width="75px">
                <ItemTemplate>
                    <asp:Label ID="lblStandardDueDate" runat="server" Text='<%# Eval("StandardDueDate", "{0:dd-MM-yyyy}") %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtEditStandardDueDate" runat="server" Text='<%# Bind("StandardDueDate", "{0:dd-MM-yyyy}") %>' TextMode="Date" Width="130px" />
                </EditItemTemplate>
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
    </Columns>
    <RowStyle BackColor="#FFFFFF" /> 
    <AlternatingRowStyle BackColor="#F0F0F0" /> 
    <SelectedRowStyle BackColor="#FFFF00" /> 
    <PagerStyle CssClass="pager-style" />
    <EmptyDataTemplate>
    <div align="center">No records for selected criteria.</div>
</EmptyDataTemplate>
</asp:GridView>

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
