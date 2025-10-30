<%@ Page Title="" Language="C#" MasterPageFile="~/ComplianceM.Master" AutoEventWireup="true" CodeBehind="LawTypes.aspx.cs" Inherits="ComplianceManager.LawTypes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cntPlaceHolder1" runat="server">
    <link rel="stylesheet" type="text/css" href="Content/common.css" />
     <div>
     <div class="heading">
        <asp:Label ID="Label4" runat="server" Text="Law Types" />
    </div>
     <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" Visible="false"></asp:Label>

     <div style="margin: 20px 0;">
         <asp:Label ID="lblLawName" runat="server" Text="Name" AssociatedControlID="txtLawTypeName" CssClass="control-label" />
         <asp:TextBox ID="txtLawTypeName" runat="server" Style="width: 200px;"></asp:TextBox>
         <asp:Label ID="lblDescription" runat="server" Text="Description" AssociatedControlID="txtDescription" CssClass="control-label"  style="margin-left: 50px;" />
         <asp:TextBox ID="txtDescription" runat="server" Style="width: 420px;"></asp:TextBox>
         <asp:Button ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click" ValidationGroup="add" CssClass="add-button" />
     </div>
        <asp:GridView ID="gvLawType" runat="server" AutoGenerateColumns="false" 
                OnRowEditing="gvLawType_RowEditing" OnRowUpdating="gvLawType_RowUpdating" OnRowCancelingEdit="gvLawType_RowCancelingEdit" OnRowDeleting="gvLawType_RowDeleting" 
                CssClass="frequency-grid" DataKeyNames="LawId" GridLines="Both" HeaderStyle-HorizontalAlign="Center"  AlternatingRowStyle-BackColor="#f2f2f2"  BorderColor="#E7E7FF" >
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
                             OnClientClick="return confirm('Are you sure you want to delete this Law Type?');" />
                     </ItemTemplate>
                 </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>