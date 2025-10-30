<%@ Page Title="" Language="C#" MasterPageFile="~/ComplianceM.Master" AutoEventWireup="true" CodeBehind="MasterReports.aspx.cs" Inherits="ComplianceManager.MasterReports" %>
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
    .save-button {background-color: #28a745; color: white;border: none;padding: 4px 4px;border-radius: 4px;cursor: pointer;}/* Green */
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
    <asp:Label ID="Label4" runat="server" Text="Master Reports" />
</div>
<asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" Visible="false" />
    <div style="display: flex; gap: 0px; margin: 5px 0; width: 100%;">
    <!-- First Column (leftmost, Upload and Search) -->
    <div style="flex: 0 0 29%; min-width: 0;">
        <div class="control-div">
            <asp:Label ID="lblMaster" runat="server" Text="Master Name" AssociatedControlID="ddlMaster" CssClass="control-label" style="width: 100px;" />
            <asp:DropDownList ID="ddlMaster" runat="server" style="width: 160px;height:20px; font-size:12px;"></asp:DropDownList>
        </div>
        
    </div>
    <!-- Second Column (middle, Export and Sample) -->
    <div style="flex: 0 0 1%; min-width: 0;">
        <div class="control-div">
            
         </div>
    
        <div style="margin: 10px 0;"></div>
    </div>
        <!-- Third Column (rightmost, Search) -->
        <div style="flex: 0 0 33%; min-width: 0;">
            <div style="margin: 0px 0; height: 100%; display: flex; flex-direction: column; gap: 1px;">
                <div style="flex: 1;">
                    <div style="display: flex; gap: 10px;">
                                <div style="margin: 9px 0;">
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
          <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" Width="100%"
               CssClass="frequency-grid" DataKeyNames="UserName" BorderColor="#E7E7FF" BorderStyle="None" BackColor="White"
               AlternatingRowStyle-BackColor="#f2f2f2" GridLines="Both" HeaderStyle-HorizontalAlign="Center" Visible="true">
      <Columns>
          <asp:TemplateField HeaderText="SNo" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                   <ItemTemplate>
                       <asp:Label ID="lblSNo" runat="server" Text='<%# Container.DataItemIndex + 1 %>' />
                   </ItemTemplate>
               </asp:TemplateField>
          <asp:TemplateField HeaderText="User ID" ItemStyle-Width="150px">
    <ItemTemplate>
        <asp:Label ID="lblUserID" runat="server" Text='<%# Eval("UserName") %>' />
    </ItemTemplate>
</asp:TemplateField>

          <asp:TemplateField HeaderText="User Name" ItemStyle-Width="150px">
              <ItemTemplate>
                  <asp:Label ID="lblUserName" runat="server" Text='<%# Eval("Name") %>' />
              </ItemTemplate>
          </asp:TemplateField>
          <asp:TemplateField HeaderText="Email" ItemStyle-Width="150px">
              <ItemTemplate>
                  <asp:Label ID="lblEmail" runat="server" Text='<%# Eval("email") %>' />
              </ItemTemplate>
              <EditItemTemplate>
                  <asp:TextBox ID="txtEditEmail" runat="server" Text='<%# Bind("email") %>' Width="180px" />
              </EditItemTemplate>
          </asp:TemplateField>

          <asp:TemplateField HeaderText="Mobile" ItemStyle-Width="80px">
              <ItemTemplate>
                  <asp:Label ID="lblMobile" runat="server" Text='<%# Eval("MobileNo") %>' />
              </ItemTemplate>
              <EditItemTemplate>
                  <asp:TextBox ID="txtEditMobile" runat="server" Text='<%# Bind("MobileNo") %>' Width="130px" />
              </EditItemTemplate>
          </asp:TemplateField>

          <asp:TemplateField HeaderText="Department" ItemStyle-Width="140px">
               <ItemTemplate>
                   <asp:Label ID="lblDepartment" runat="server" Text='<%# Eval("DepartmentName") %>' />
               </ItemTemplate>
               <EditItemTemplate>
                   <asp:DropDownList ID="ddlEditDepartment" runat="server" Width="130px" />
                   <asp:HiddenField ID="hfEditDepartmentId" runat="server" Value='<%# Bind("DeptId") %>' />
               </EditItemTemplate>
           </asp:TemplateField>

          <asp:TemplateField HeaderText="Supervisor" ItemStyle-Width="140px">
               <ItemTemplate>
                   <asp:Label ID="lblSupervisor" runat="server" Text='<%# Eval("SupervisorName") %>' />

               </ItemTemplate>
               <EditItemTemplate>
                   <asp:DropDownList ID="ddlEditSupervisor" runat="server" Width="130px" />
                       <asp:HiddenField ID="hfEditSupervisorId" runat="server" Value='<%# Bind("SupervisorId") %>' />
               </EditItemTemplate>
           </asp:TemplateField>
          <asp:TemplateField HeaderText="Approver"  ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
              <ItemTemplate>
                  <asp:CheckBox ID="chkIsApprover" runat="server" Checked='<%# Eval("IsApprover") %>' Enabled="false"/>
              </ItemTemplate>
              <EditItemTemplate>
                  <asp:CheckBox ID="chkEditIsApprover" runat="server" Checked='<%# Bind("IsApprover") %>' />
              </EditItemTemplate>
              <ItemStyle HorizontalAlign="Center" />
          </asp:TemplateField>

          <asp:TemplateField HeaderText="Preparer" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
              <ItemTemplate>
                  <asp:CheckBox ID="chkIsPreparer" runat="server" Checked='<%# Eval("IsPreparer") %>' Enabled="false"/>
              </ItemTemplate>
              <EditItemTemplate>
                  <asp:CheckBox ID="chkEditIsPreparer" runat="server" Checked='<%# Eval("IsPreparer") %>' />
              </EditItemTemplate>
              <ItemStyle HorizontalAlign="Center" />
          </asp:TemplateField>
           <asp:TemplateField HeaderText="Supervisor" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
    <ItemTemplate>
        <asp:CheckBox ID="chkIsSupervisor" runat="server" Checked='<%# Eval("IsSupervisor") %>'  Enabled="false"/>
    </ItemTemplate>
    <EditItemTemplate>
        <asp:CheckBox ID="chkEditIsSupervisor" runat="server" Checked='<%# Eval("IsSupervisor") %>' />
    </EditItemTemplate>
    <ItemStyle HorizontalAlign="Center" />
</asp:TemplateField>
          <asp:TemplateField HeaderText="Active" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
              <ItemTemplate>
                  <asp:CheckBox ID="chkIsActive" runat="server" Checked='<%# Eval("IsActive") %>'  Enabled="false"/>
              </ItemTemplate>
              <EditItemTemplate>
                  <asp:CheckBox ID="chkEditIsActive" runat="server" Checked='<%# Eval("IsActive") %>' />
              </EditItemTemplate>
              <ItemStyle HorizontalAlign="Center" />
          </asp:TemplateField>
      </Columns>
  </asp:GridView>

<asp:GridView ID="gvDepartment" runat="server" AutoGenerateColumns="false" 
        CssClass="frequency-grid"  GridLines="Both" HeaderStyle-HorizontalAlign="Center" AlternatingRowStyle-BackColor="#f2f2f2" 
        BorderColor="#E7E7FF" BorderStyle="None" Width="970px" BackColor="White"  Visible="true">
    <Columns>
        <asp:TemplateField HeaderText="SNo" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Label ID="lblSNo" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Department Name" ItemStyle-Width="150px">
            <ItemTemplate>
                <asp:Label ID="lblDepartmentName" runat="server" Text='<%# Eval("DepartmentName") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtEditDepartmentName" runat="server" Text='<%# Bind("DepartmentName") %>' Width="200px"></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Description" ItemStyle-Width="450px">
            <ItemTemplate>
                <asp:Label ID="lblDescription" runat="server" Text='<%# Bind("Description") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtEditDescription" runat="server" Text='<%# Bind("Description") %>'></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateField>
        
    </Columns>
</asp:GridView>

        <asp:GridView ID="gvFrequencies" runat="server" AutoGenerateColumns="false" CssClass="frequency-grid" Width="700px" GridLines="Both" 
            HeaderStyle-HorizontalAlign="Center"  AlternatingRowStyle-BackColor="#f2f2f2"  BorderColor="#E7E7FF" BorderStyle="None" 
            BackColor="White"  Visible="true">
     <Columns>
         <asp:TemplateField HeaderText="SNo" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
             <ItemTemplate>
                 <asp:Label ID="lblSNo" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
             </ItemTemplate>
         </asp:TemplateField>
         <asp:TemplateField HeaderText="Frequency Name" ItemStyle-Width="250px">
             <ItemTemplate>
                 <asp:Label ID="lblFrequencyName" runat="server" Text='<%# Eval("FrequencyName") %>'></asp:Label>
             </ItemTemplate>
             <EditItemTemplate>
                 <asp:TextBox ID="txtEditFrequencyName" runat="server" Text='<%# Bind("FrequencyName") %>' Width="200px"></asp:TextBox>
             </EditItemTemplate>
         </asp:TemplateField>
        <asp:TemplateField HeaderText="Description" ItemStyle-Width="200px">
             <ItemTemplate>
                 <asp:Label ID="lblOccursEvery" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
             </ItemTemplate>
             <EditItemTemplate>
                 <asp:TextBox ID="txtEditDescription" runat="server" Text='<%# Bind("Description") %>' Width="100px"></asp:TextBox>
             </EditItemTemplate>
         </asp:TemplateField>
         
     </Columns>
  
 </asp:GridView>

        <asp:GridView ID="gvComplianceNature" runat="server" AutoGenerateColumns="false" CssClass="frequency-grid" GridLines="Both" 
        HeaderStyle-HorizontalAlign="Center"  AlternatingRowStyle-BackColor="#f2f2f2"  BorderColor="#E7E7FF"  Visible="true">
    <Columns>
        <asp:TemplateField HeaderText="SNo" ItemStyle-Width="30px" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Label ID="lblSNo" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Nature of Compliance" ItemStyle-Width="150px">
            <ItemTemplate>
                <asp:Label ID="lblComplianceNature" runat="server" Text='<%# Eval("NatureOfCompliance") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtEditComplianceNature" runat="server" Text='<%# Bind("NatureOfCompliance") %>' Width="200px"></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Description" ItemStyle-Width="450px">
            <ItemTemplate>
                <asp:Label ID="lblDescription" runat="server" Text='<%# Bind("Description") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtEditDescription" runat="server" Text='<%# Bind("Description") %>'></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateField>

    </Columns>
</asp:GridView>


        <asp:GridView ID="gvComplianceType" runat="server" AutoGenerateColumns="False" CssClass="frequency-grid"  Visible="true" 
            GridLines="Both" HeaderStyle-HorizontalAlign="Center"  AlternatingRowStyle-BackColor="#f2f2f2"  BorderColor="#E7E7FF" >
    <Columns>
         <asp:TemplateField HeaderText="SNo" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
             <ItemTemplate>
                 <asp:Label ID="lblSNo" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
             </ItemTemplate>
         </asp:TemplateField>
         <asp:TemplateField HeaderText="Compliance Type" ItemStyle-Width="200px">
             <ItemTemplate>
                 <asp:Label ID="lblName" runat="server" Text='<%# Bind("ComplianceTypeName") %>'></asp:Label>
             </ItemTemplate>
             <EditItemTemplate>
                 <asp:TextBox ID="txtName" runat="server" Text='<%# Bind("ComplianceTypeName") %>'></asp:TextBox>
             </EditItemTemplate>
         </asp:TemplateField>
        <asp:TemplateField HeaderText="Description" ItemStyle-Width="450px">
             <ItemTemplate>
                 <asp:Label ID="lblDescription" runat="server" Text='<%# Bind("Description") %>'></asp:Label>
             </ItemTemplate>
             <EditItemTemplate>
                 <asp:TextBox ID="txtEditDescription" runat="server" Text='<%# Bind("Description") %>'></asp:TextBox>
             </EditItemTemplate>
         </asp:TemplateField>
       

        </Columns>
    </asp:GridView>

<asp:GridView ID="gvComplianceStatus" runat="server" AutoGenerateColumns="False" CssClass="frequency-grid" GridLines="Both"  Visible="true"
    HeaderStyle-HorizontalAlign="Center" AlternatingRowStyle-BackColor="#f2f2f2" BorderColor="#E7E7FF" >
    <Columns>
        <asp:TemplateField HeaderText="SNo" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Label ID="lblSNo" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Compliance Status" ItemStyle-Width="500px">
            <ItemTemplate>
                <asp:Label ID="lblComplianceStatus" runat="server" Text='<%# Bind("ComplianceStatusName") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtEditComplianceStatusName" runat="server" Text='<%# Bind("ComplianceStatusName") %>'></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateField>

        
    </Columns>
</asp:GridView>

        <asp:GridView ID="gvLawType" runat="server" AutoGenerateColumns="false" CssClass="frequency-grid"  GridLines="Both" 
            HeaderStyle-HorizontalAlign="Center"  AlternatingRowStyle-BackColor="#f2f2f2"  BorderColor="#E7E7FF"  Visible="true">
    <Columns>
        <asp:TemplateField HeaderText="SNo" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Label ID="lblSNo" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="LawType Name" ItemStyle-Width="150px">
            <ItemTemplate>
                <asp:Label ID="lblLawName" runat="server" Text='<%# Eval("LawName") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtEditLawTypeName" runat="server" Text='<%# Bind("LawName") %>' Width="200px"></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Description" ItemStyle-Width="550px">
            <ItemTemplate>
                <asp:Label ID="lblDescription" runat="server" Text='<%# Bind("Description") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtEditDescription" runat="server" Text='<%# Bind("Description") %>'></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateField>

    </Columns>
</asp:GridView>

        <asp:GridView ID="gvDrivenBy" runat="server" AutoGenerateColumns="false" CssClass="frequency-grid" GridLines="Both" 
            HeaderStyle-HorizontalAlign="Center"  AlternatingRowStyle-BackColor="#f2f2f2"  BorderColor="#E7E7FF" Visible="true"> 
    <Columns>
        <asp:TemplateField HeaderText="SNo" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Label ID="lblSNo" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="DrivenBy Name" ItemStyle-Width="150px">
            <ItemTemplate>
                <asp:Label ID="lblDrivenByName" runat="server" Text='<%# Eval("DrivenName") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtEditDrivenByName" runat="server" Text='<%# Bind("DrivenName") %>' Width="200px"></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Description" ItemStyle-Width="450px">
            <ItemTemplate>
                <asp:Label ID="lblDescription" runat="server" Text='<%# Bind("Description") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtEditDescription" runat="server" Text='<%# Bind("Description") %>'></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateField>

    </Columns>
</asp:GridView>






<asp:GridView ID="gvCompliances" runat="server" AutoGenerateColumns="false" OnPageIndexChanging="gvCompliances_PageIndexChanging" 
     CssClass="frequency-grid" Width="100%" AllowPaging="true" GridLines="Both" PageSize="15" DataKeyNames="ComplianceID" 
    HeaderStyle-HorizontalAlign="Center" BorderColor="#E7E7FF" BorderStyle="None" BackColor="White"
    EmptyDataText="No records for selected Master." ShowHeaderWhenEmpty="true"  Visible="true">
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