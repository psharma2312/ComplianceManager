<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyCompliance.aspx.cs" Inherits="ComplianceManager.MyCompliance" %>
<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="server">
     <tr>
            <%--<td colspan="3" style="text-align: right; height: 30px;">Department / Function :</td>
            <td colspan="3"><asp:DropDownList ID="ddlDepartment" runat="server" AutoPostBack="True" DataTextField="DepartmentName" DataValueField="DeptId" Width="180px" Height="25px" /></td>
            <td colspan="1">&nbsp;</td>--%>
            <td colspan="3" style="text-align: right; height: 30px;">Status :</td>
            <td colspan="4">
                 <asp:DropDownList ID="ddlDue" runat="server" CssClass="DropDown" AutoPostBack="True" Width="150px" Height="25px">
                    <asp:ListItem Value="1">Pending</asp:ListItem>
                    <asp:ListItem Value="2">Done</asp:ListItem>
                    <asp:ListItem Value="3">Approved</asp:ListItem>
                    <asp:ListItem Value="3">Closed</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td colspan="5" style="width: 100px; text-align: right; height: 30px;">&nbsp;</td>
            <td colspan="8" style="width: 100px; text-align: right; height: 30px;">&nbsp;</td>
        </tr>
    <tr>
            <td colspan="20">
                <asp:GridView ID="gvComplDetails" runat="server" AutoGenerateColumns="False" DataKeyNames="ComplianceID" AllowPaging="true" PageSize="20" BackColor="White" BorderColor="#E7E7FF" BorderStyle="None"
                    BorderWidth="1px" CellPadding="5" Width="90%" GridLines="Both" Font-Size="Smaller" HeaderStyle-BackColor="#337ab7"
                    HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#f2f2f2" HorizontalAlign="Center">
                    <Columns>
                        <asp:BoundField DataField="ComplianceID" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"
                            ControlStyle-Width="100" HeaderText="Compliance ID" ReadOnly="true" SortExpression="ComplianceID" ItemStyle-Width="90px" />
                        <asp:BoundField HeaderText="Compliance Area" DataField="ComplianceArea" HeaderStyle-Width="1" ItemStyle-Width="40px" />
                        <asp:BoundField DataField="ComplianceType" ControlStyle-Width="300" HeaderText="Compliance Type" ReadOnly="true" SortExpression="ComplianceType" ItemStyle-Width="250px" />
                        <asp:BoundField DataField="GovernmentLegislation" ControlStyle-Width="100" HeaderText="Government Legislation" ReadOnly="true" SortExpression="GovernmentLegislation" ItemStyle-Width="50px" />
                        <asp:BoundField DataField="ActSectionReference" ControlStyle-Width="100" HeaderText="Act / SectionReference" ReadOnly="true" SortExpression="ActSectionReference" ItemStyle-Width="50px" />
                        <asp:BoundField DataField="NatureOfCompliance" ControlStyle-Width="100" HeaderText="Nature Of Compliance" ReadOnly="true" ItemStyle-Width="50px" SortExpression="NatureOfCompliance" />
                        <asp:BoundField DataField="EffectiveFrom" ControlStyle-Width="100" HeaderText="EffectiveFrom Date" ReadOnly="true" SortExpression="EffectiveFrom" ItemStyle-Width="50px" />
                        <asp:BoundField DataField="StandardDueDate" ControlStyle-Width="100" HeaderText="Standard Due Date" ReadOnly="true" SortExpression="StandardDueDate" ItemStyle-Width="50px" />
                        <asp:BoundField DataField="Frequency" ControlStyle-Width="100" HeaderText="Frequency" ReadOnly="true" SortExpression="Frequency" ItemStyle-Width="50px" />
                        <asp:BoundField DataField="DepartmentFunction" ControlStyle-Width="100" HeaderText="Department /Function" ReadOnly="true" SortExpression="DepartmentFunction" ItemStyle-Width="50px" />
                        <asp:BoundField DataField="CreatedByName" ControlStyle-Width="100" HeaderText="Created By" ReadOnly="true" SortExpression="CreatedByName" ItemStyle-Width="50px" />
                        <asp:BoundField DataField="ApprovedByName" ControlStyle-Width="100" HeaderText="Approved By" ReadOnly="true" SortExpression="ApprovedByName" ItemStyle-Width="50px" />
                        <asp:BoundField DataField="DetailsOfComplianceRequirements" ControlStyle-Width="100" HeaderText="Details Of Compliance Requirements" ReadOnly="true" SortExpression="DetailsOfComplianceRequirements" ItemStyle-Width="50px" />
                        <asp:CommandField ShowDeleteButton="True" ButtonType="Button" ItemStyle-Width="50px" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
</asp:Content>
