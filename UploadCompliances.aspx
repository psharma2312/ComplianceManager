<%@ Page Title="" Language="C#" MasterPageFile="~/ComplianceM.Master" AutoEventWireup="true" CodeBehind="UploadCompliances.aspx.cs" Inherits="ComplianceManager.UploadCompliances" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cntPlaceHolder1" runat="server">
     <link rel="stylesheet" type="text/css" href="Content/common.css" />
    <style type="text/css">
        .error-message {color: #721c24;background-color: #f8d7da;padding: 10px;border: 1px solid #f5c6cb;border-radius: 4px;margin-bottom: 15px;display: block;        }
   
      
        .upload-button {background-color: #007bff;color: white;border: none;padding: 2px 7px; font-size:12px;border-radius: 4px;cursor: pointer;}/* Green */
        .upload-button:hover {background-color: #0069d9;}
        .save-button {background-color: #28a745; color: white;border: none;padding: 2px 7px; font-size:12px;border-radius: 4px;cursor: pointer;}/* Green */
        .save-button:hover {background-color: #218838;}
        .grid-button {padding: 5px 10px;border: none;border-radius: 4px;cursor: pointer;}
        .edit-button {background-color: #007bff;color: white;} /* Blue */
        .edit-button:hover {background-color: #0069d9;}
        .update-button {background-color: #28a745; color: white;}/* Green */
        .update-button:hover {background-color: #218838;}
        .cancel-button {background-color: #6c757d; color: white;}/* Gray */
        .cancel-button:hover {background-color: #5a6268;}
        .delete-button {background-color: #dc3545; color: white;}/* Red */
        .delete-button:hover {background-color: #c82333;}
      
        .modal {display: none;position: fixed;top: 0;left: 0;width: 100%;height: 100%;background-color: rgba(0,0,0,0.5);z-index: 1000;}
        .modal-content {background-color: #fff;margin: 15% auto;padding: 20px;border-radius: 5px;width: 80%;max-width: 600px;text-align: center;position: relative;}
        .modal-close {position: absolute;top: 10px;right: 10px;font-size: 18px;cursor: pointer;color: #333;}
        .modal-close:hover {color: #000;}
        .control-div {
            margin: 5px 0;
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
        <asp:Label ID="Label4" runat="server" Text="Upload Compliance" />
    </div>
         <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" Visible="false"></asp:Label>
        <div style="display: flex; gap: 20px; margin: 5px 0; width: 100%;">
            <!-- First Column (leftmost, Upload and Search) -->
            <div style="flex: 0 0 400px; min-width: 0; max-width: 400px;">
                <div style="margin: 10px 0;">
                    <asp:FileUpload ID="FileUpload1" runat="server" style="width: 200px;font-size:12px;" /> &nbsp; 
                    <asp:Button ID="btnUpload" runat="server" Text="Upload File" OnClick="btnUpload_Click" CssClass="upload-button" />
                </div>
            </div>
            <!-- Second Column (middle, Export and Sample) -->
            <div style="flex: 0 0 300px; min-width: 0; max-width: 300px;">
                <div style="margin: 10px 0;">
                    <asp:Button ID="btnExport" runat="server" Text="Excel Export" OnClick="btnExport_Click" CssClass="save-button" 
                        style="width: 90px;" />
                    <asp:Button ID="btnSample" runat="server" Text="Sample File" OnClick="btnSample_Click" CssClass="upload-button" 
    style="width: 90px;" />
                </div>
                <div style="margin: 10px 0;">
                    
                </div>
            </div>
            <!-- Third Column (rightmost, Search) -->
            <div style="flex: 0 0 300px; min-width: 0; max-width: 300px;">
                <div style="margin: 10px 0; height: 100%; display: flex; flex-direction: column; gap: 1px;">
                    <div style="flex: 1;">
                        <div style="display: flex; gap: 10px;">
                            <asp:TextBox ID="txtSKUSearch" runat="server" style="width: 150px; color: Black;"></asp:TextBox>
                            <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" CssClass="upload-button" 
                                style="width: 60px;" />
                            <asp:Button ID="btnAll" runat="server" Text="Show All" OnClick="btnAll_Click" CssClass="upload-button" 
                                style="width: 70px;" />
                        </div>
                    </div>
                    <!-- Placeholder to fill height -->
                    <div style="flex: 1;"></div>
                </div>
            </div>
        </div>
        <div style="text-align: center;">
            <!-- Additional buttons or actions can be added here if needed -->
        </div>
               <asp:GridView ID="gvCompliances" runat="server" AutoGenerateColumns="false" 
                    AllowPaging="true" PageSize="10"   DataKeyNames="ComplianceId"
                    OnPageIndexChanging="gvComplDetails_PageIndexChanging"
                    CssClass="frequency-grid"  Width="100%" 
                    AlternatingRowStyle-BackColor="#f2f2f2" GridLines="Both" HeaderStyle-HorizontalAlign="Center" 
                    BorderColor="#E7E7FF" BorderStyle="None" BackColor="White">
                    <Columns>
                        <asp:TemplateField HeaderText="SNo" ItemStyle-Width="25px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label ID="lblSNo" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Comp. Ref" DataField="ComplianceRef" ItemStyle-Width="30px" ItemStyle-Height="25px"  HeaderStyle-HorizontalAlign="Left"/>
                        <asp:BoundField HeaderText="Compliance Type" DataField="ComplianceTypeName"   ItemStyle-Width="250px" ItemStyle-Height="25px"  HeaderStyle-HorizontalAlign="Left" />
                        <asp:BoundField HeaderText="Act Section" DataField="ActSectionReference"  ItemStyle-Width="480px" ItemStyle-Height="25px" HeaderStyle-HorizontalAlign="Left" />
                        <asp:BoundField HeaderText="Compliance Nature" DataField="NatureOfComplianceName" ItemStyle-Width="200px" ItemStyle-Height="25px" HeaderStyle-HorizontalAlign="Left"/>
                        <%--<asp:BoundField HeaderText="EffectiveFrom" DataField="EffectiveFrom"   ItemStyle-Width="140px" ItemStyle-Height="25px" HeaderStyle-HorizontalAlign="Left" />--%>
                        <%--<asp:BoundField HeaderText="Due Date" DataField="StandardDueDate"  ItemStyle-Width="240px" ItemStyle-Height="25px" HeaderStyle-HorizontalAlign="Left"/>--%>
                        <asp:TemplateField HeaderText="EffectiveFrom" ItemStyle-Width="100px">
    <ItemTemplate>
        <asp:Label ID="lblEffectiveFromDate" runat="server" Text='<%# Eval("EffectiveFrom", "{0:dd-MM-yyyy}") %>' />
    </ItemTemplate>
    <EditItemTemplate>
        <asp:TextBox ID="txtEditEffectiveFromDate" runat="server" Text='<%# Bind("EffectiveFrom", "{0:dd-MM-yyyy}") %>' TextMode="Date" Width="130px" />
    </EditItemTemplate>
</asp:TemplateField>
<asp:TemplateField HeaderText="Effetive Till" ItemStyle-Width="90px">
    <ItemTemplate>
        <asp:Label ID="lblStandardDueDate" runat="server" Text='<%# Eval("StandardDueDate", "{0:dd-MM-yyyy}") %>' />
    </ItemTemplate>
    <EditItemTemplate>
        <asp:TextBox ID="txtEditStandardDueDate" runat="server" Text='<%# Bind("StandardDueDate", "{0:dd-MM-yyyy}") %>' TextMode="Date" Width="130px" />
    </EditItemTemplate>
</asp:TemplateField>
                        <asp:BoundField HeaderText="Frequency" DataField="FrequencyName"  ItemStyle-Width="85px" ItemStyle-Height="25px" HeaderStyle-HorizontalAlign="Left"  />
                        <asp:BoundField HeaderText="Department" DataField="DepartmentName"  ItemStyle-Width="150px" ItemStyle-Height="25px" HeaderStyle-HorizontalAlign="Left" />
                        <%--<asp:BoundField HeaderText="Details" DataField="DetailsOfComplianceRequirements"  ItemStyle-Width="240px" ItemStyle-Height="25px"  HeaderStyle-HorizontalAlign="Left" />--%>
                        
                         <asp:TemplateField HeaderText="Details" ItemStyle-Width="40px">
                             <ItemTemplate>
                                 <div style="display: flex; align-items: center; gap: 5px;">
                                     <asp:Button ID="btnShowDetails" runat="server" Text="More" CssClass="more-button" OnClick="Display" />
                                 </div>
                             </ItemTemplate>
                         </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pager-style" />
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
        </script>
</asp:Content>
