<%@ Page Title="" Language="C#" MasterPageFile="~/ComplianceM.Master" AutoEventWireup="true" CodeBehind="Frequency.aspx.cs" Inherits="ComplianceManager.Frequency1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cntPlaceHolder1" runat="server">
    <link rel="stylesheet" type="text/css" href="Content/common.css" />
    <div>
         <div class="heading">
             <asp:Label ID="Label4" runat="server" Text="Frequency Master" />
         </div>
        <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" Visible="false"></asp:Label>
        <div style="margin: 20px 0;">
            <asp:Label ID="lblFrequencyName" runat="server" Text="Name" AssociatedControlID="txtFrequencyName"  CssClass="control-label" />
            <asp:TextBox ID="txtFrequencyName" runat="server" style="width: 150px;"></asp:TextBox>
            <asp:Label ID="lblOccursEvery" runat="server" Text="Description" AssociatedControlID="txtDescription"  CssClass="control-label" style="margin-left:50px;" />
            <asp:TextBox ID="txtDescription" runat="server" style="width: 150px;margin-left:20px;"></asp:TextBox>
            <asp:Button ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click" ValidationGroup="add" CssClass="add-button"/>
        </div>
       <asp:GridView ID="gvFrequencies" runat="server" AutoGenerateColumns="false" 
            OnRowEditing="gvFrequencies_RowEditing"
           OnRowUpdating="gvFrequencies_RowUpdating"
           OnRowCancelingEdit="gvFrequencies_RowCancelingEdit"
           OnRowDeleting="gvFrequencies_RowDeleting" 
            CssClass="frequency-grid" DataKeyNames="FrequencyId" Width="700px"
           GridLines="Both" HeaderStyle-HorizontalAlign="Center"  AlternatingRowStyle-BackColor="#f2f2f2"  BorderColor="#E7E7FF" 
           BorderStyle="None" BackColor="White" >
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
                <asp:TemplateField HeaderText="Action" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandName="Edit" CssClass="grid-button edit-button" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:Button ID="btnUpdate" runat="server" Text="Update" CommandName="Update" CssClass="grid-button update-button" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CommandName="Cancel" CssClass="grid-button cancel-button" />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Action" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="Delete" CssClass="grid-button delete-button" 
                            OnClientClick="return confirm('Are you sure you want to delete this frequency?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
         
        </asp:GridView>
    </div>
</asp:Content>