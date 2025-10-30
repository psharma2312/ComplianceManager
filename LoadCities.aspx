<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LoadCities.aspx.cs" Inherits="ComplianceManager.LoadCities" %>
<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="server">
    <asp:GridView ID="gvCustomers" DataKeyNames="CustomerId" runat="server" AutoGenerateColumns="false"
        OnRowEditing="EditCustomer" OnRowDataBound="RowDataBound" OnRowUpdating="UpdateCustomer"
        OnRowCancelingEdit="CancelEdit">
        <Columns>
            <asp:BoundField DataField="ContactName" HeaderText="Contact Name" />
            <asp:TemplateField HeaderText="City">
                <ItemTemplate>
                    <asp:Label ID="lblCity" runat="server" Text='<%# Eval("City")%>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlCities" runat="server">
                    </asp:DropDownList>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowEditButton="True" />
        </Columns>
    </asp:GridView>
</asp:Content>
