<%@ Page Title="" Language="C#" MasterPageFile="~/ComplianceM.Master" AutoEventWireup="true" CodeBehind="UserCreation.aspx.cs" Inherits="ComplianceManager.UserCreation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cntPlaceHolder1" runat="server">
    <link rel="stylesheet" type="text/css" href="Content/common.css" />
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
            <asp:Label ID="Label4" runat="server" Text="Create User" />
        </div>
        <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" Visible="false"></asp:Label>
        <div style="margin: 20px 0;">
            <asp:Label ID="lblUsername" runat="server" Text="Username" AssociatedControlID="txtUsername" CssClass="control-label" />
            <asp:TextBox ID="txtUsername" runat="server" style="width: 100px;"></asp:TextBox>
            
             <asp:Label ID="lblDepartment" runat="server" Text="Department" AssociatedControlID="ddlDepartment"  CssClass="control-label" style="margin-left: 20px;" />
             <asp:DropDownList ID="ddlDepartment" runat="server" style="width: 160px;"></asp:DropDownList>
            
            <asp:Label ID="lblEmail" runat="server" Text="Email" AssociatedControlID="txtEmail"  CssClass="control-label" style="margin-left: 20px;" />
            <asp:TextBox ID="txtEmail" runat="server" style="width: 150px;"></asp:TextBox>

            <asp:CheckBox ID="chkIsApprover" runat="server" CssClass="checkbox-spacing" Text="Is Approver" />
            <asp:CheckBox ID="chkIsSupervisor" runat="server" CssClass="checkbox-spacing" Text="Is Supervisor" />
        </div>
        <div style="margin: 20px 0;">
            <asp:Label ID="lblPassword" runat="server" Text="Password" AssociatedControlID="txtPassword" CssClass="control-label" />
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" style="width: 100px;"></asp:TextBox>
            
            <asp:Label ID="lblSupervisor" runat="server" Text="Supervisor" AssociatedControlID="ddlSupervisor"  CssClass="control-label" style="margin-left: 20px;" />
            <asp:DropDownList ID="ddlSupervisor" runat="server" style="width: 160px;"></asp:DropDownList>
            <asp:Label ID="lblMobile" runat="server" Text="Mobile" AssociatedControlID="txtMobile"  CssClass="control-label" style="margin-left: 20px;" />
            <asp:TextBox ID="txtMobile" runat="server" style="width: 150px;"></asp:TextBox>
            
            <asp:CheckBox ID="chkIsPreparer" runat="server" Text="Is Preparer"  CssClass="checkbox-spacing" />
             <asp:Button ID="btnCreateUser" runat="server" Text="Add" OnClick="btnCreateUser_Click" CssClass="add-button"  style="margin-left: 20px;" />
        </div>

           <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" Width="100%"
                OnRowEditing="gvUsers_RowEditing" OnRowUpdating="gvUsers_RowUpdating" OnRowCancelingEdit="gvUsers_RowCancelingEdit" OnRowDeleting="gvUsers_RowDeleting"
                CssClass="frequency-grid" DataKeyNames="UserName" BorderColor="#E7E7FF" BorderStyle="None" BackColor="White"
                AlternatingRowStyle-BackColor="#f2f2f2" GridLines="Both" HeaderStyle-HorizontalAlign="Center" >
       <Columns>
           <asp:TemplateField HeaderText="SNo" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label ID="lblSNo" runat="server" Text='<%# Container.DataItemIndex + 1 %>' />
                    </ItemTemplate>
                </asp:TemplateField>
           <asp:TemplateField HeaderText="UserId" ItemStyle-Width="150px">
               <ItemTemplate>
                   <asp:Label ID="lblUserId" runat="server" Text='<%# Eval("UserName") %>' />
               </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField HeaderText="User Name" ItemStyle-Width="150px">
    <ItemTemplate>
        <asp:Label ID="lblUserName" runat="server" Text='<%# Eval("Name") %>' />
    </ItemTemplate>
</asp:TemplateField>
           <asp:TemplateField HeaderText="Email" ItemStyle-Width="150px">
               <ItemTemplate>
                   <asp:Label ID="lblEmail" runat="server" Text='<%# Eval("Email") %>' />
               </ItemTemplate>
              <%-- <EditItemTemplate>
                   <asp:TextBox ID="txtEditEmail" runat="server" Text='<%# Bind("email") %>' Width="180px" />
               </EditItemTemplate>--%>
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
                    <asp:HiddenField ID="hfEditDepartmentId" runat="server" Value='<%# Bind("DepartmentId") %>' />
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
            <%--<asp:TemplateField HeaderText="Supervisor" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
     <ItemTemplate>
         <asp:CheckBox ID="chkIsSupervisor" runat="server" Checked='<%# Eval("IsSupervisor") %>'  Enabled="false"/>
     </ItemTemplate>
     <EditItemTemplate>
         <asp:CheckBox ID="chkEditIsSupervisor" runat="server" Checked='<%# Eval("IsSupervisor") %>' />
     </EditItemTemplate>
     <ItemStyle HorizontalAlign="Center" />
 </asp:TemplateField>--%>
           <asp:TemplateField HeaderText="Active" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
               <ItemTemplate>
                   <asp:CheckBox ID="chkIsActive" runat="server" Checked='<%# Eval("IsActive") %>'  Enabled="false"/>
               </ItemTemplate>
               <EditItemTemplate>
                   <asp:CheckBox ID="chkEditIsActive" runat="server" Checked='<%# Eval("IsActive") %>' />
               </EditItemTemplate>
               <ItemStyle HorizontalAlign="Center" />
           </asp:TemplateField>
           <asp:TemplateField HeaderText="Action" ItemStyle-Width="90px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandName="Edit" CssClass="grid-button edit-button" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:Button ID="btnUpdate" runat="server" Text="Update" CommandName="Update" CssClass="grid-button update-button" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CommandName="Cancel" CssClass="grid-button cancel-button" />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Action" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="Delete" CssClass="grid-button delete-button" 
                            OnClientClick="return confirm('Are you sure you want to delete this user?');" />
                    </ItemTemplate>
                </asp:TemplateField>
       </Columns>
   </asp:GridView>
    </div>
</asp:Content>