<%@ Page Title="" Language="C#" MasterPageFile="~/ComplianceM.Master" AutoEventWireup="true" CodeBehind="ComplianceStatus.aspx.cs" Inherits="ComplianceManager.ComplianceStatus1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cntPlaceHolder1" runat="server">
  <link rel="stylesheet" type="text/css" href="Content/common.css" />
    <div>
        <div class="heading">
            <asp:Label ID="Label4" runat="server" Text="Compliance Status" />
        </div>
        <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" Visible="false"></asp:Label>
        <div style="margin: 20px 0;">
            <asp:Label ID="lblComplianceStatus" runat="server" Text="Name" AssociatedControlID="txtComplianceStatusName" CssClass="control-label" />
            <asp:TextBox ID="txtComplianceStatusName" runat="server" Style="width: 200px;"></asp:TextBox>
            <asp:Button ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click" ValidationGroup="add" CssClass="add-button" />
        </div>

        <asp:GridView ID="gvComplianceStatus" runat="server" AutoGenerateColumns="False"
            OnRowCancelingEdit="gvComplianceStatus_RowCancelingEdit" OnRowEditing="gvComplianceStatus_RowEditing" OnRowUpdating="gvComplianceStatus_RowUpdating"
            OnRowDeleting="gvComplianceStatus_RowDeleting" CssClass="frequency-grid" GridLines="Both" HeaderStyle-HorizontalAlign="Center" AlternatingRowStyle-BackColor="#f2f2f2" 
            BorderColor="#E7E7FF"  DataKeyNames="ComplianceStatusID">
            <Columns>
                <asp:TemplateField HeaderText="SNo" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label ID="lblSNo" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Compliance Type" ItemStyle-Width="500px">
                    <ItemTemplate>
                        <asp:Label ID="lblComplianceStatus" runat="server" Text='<%# Bind("ComplianceStatusName") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEditComplianceStatusName" runat="server" Text='<%# Bind("ComplianceStatusName") %>'></asp:TextBox>
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
                <asp:TemplateField HeaderText="Action" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="Delete" CssClass="grid-button delete-button"
                            OnClientClick="return confirm('Are you sure you want to delete this compliance status?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>