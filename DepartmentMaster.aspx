<%@ Page Title="" Language="C#" MasterPageFile="~/ComplianceM.Master" AutoEventWireup="true" CodeBehind="DepartmentMaster.aspx.cs" Inherits="ComplianceManager.DepartmentMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cntPlaceHolder1" runat="server">
    <link rel="stylesheet" type="text/css" href="Content/common.css" />
     <div>
        <div class="heading">
            <asp:Label ID="Label4" runat="server" Text="Department Master" />
        </div>
     <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" Visible="false"></asp:Label>
     <div style="margin: 20px 0;">
         <asp:Label ID="lblDepartmentName" runat="server" Text="Name" AssociatedControlID="txtDepartmentName" CssClass="control-label" />
         <asp:TextBox ID="txtDepartmentName" runat="server" Style="width: 150px;"></asp:TextBox>
         <asp:Label ID="lblDescription" runat="server" Text="Description" AssociatedControlID="txtDescription" CssClass="control-label" style="margin-left:50px;" />
         <asp:TextBox ID="txtDescription" runat="server" Style="width: 400px; margin-left:20px;"></asp:TextBox>
         <asp:Button ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click" ValidationGroup="add" CssClass="add-button" />
     </div>
        <asp:GridView ID="gvDepartment" runat="server" AutoGenerateColumns="false" 
                OnRowEditing="gvDepartment_RowEditing" OnRowUpdating="gvDepartment_RowUpdating" OnRowCancelingEdit="gvDepartment_RowCancelingEdit" OnRowDeleting="gvDepartment_RowDeleting" 
                CssClass="frequency-grid" DataKeyNames="DeptID" GridLines="Both" HeaderStyle-HorizontalAlign="Center" AlternatingRowStyle-BackColor="#f2f2f2"  BorderColor="#E7E7FF" 
                BorderStyle="None" Width="970px" BackColor="White" >
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
                <asp:TemplateField HeaderText="Action" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
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
                             OnClientClick="return confirm('Are you sure you want to delete this department?');" />
                     </ItemTemplate>
                 </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>