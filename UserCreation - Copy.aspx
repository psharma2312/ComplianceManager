<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserCreation.aspx.cs" Inherits="ComplianceManager.UserCreation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="server">
    <table  width="100%" >
        <tr><td colspan="14" align="center"><asp:Label ID="lblError" runat="server"></asp:Label></td></tr>
        <tr><td colspan="14" class="heading">&nbsp; Create User</td></tr>
        <tr><td colspan="14" style="height: 20px;">&nbsp;</td></tr>
        <tr>
            <td align="right" width="200px" colspan="2" height="30px">User Name : <span style="color: red">*</span></td>
            <td colspan="2"><asp:TextBox ID="txtusername" runat="server" Width="190px" MaxLength="75"></asp:TextBox></td>
             <td align="right" colspan="2"> Password : <span style="color: red">*</span></td>
            <td colspan="2"><asp:TextBox ID="txtpassword" runat="server" TextMode="Password" Width="190px" MaxLength="50"></asp:TextBox></td>
            <td align="right" colspan="2"> Confirm Password : <span style="color: red">*</span> </td>
            <td colspan="2"><asp:TextBox ID="txtconpassword" runat="server" TextMode="Password" Width="190px" MaxLength="50" /></td>
            <td colspan="2">&nbsp;</td>
        </tr>
       

        <tr>
            <td align="right" style="width: 303px" colspan="2" height="30px">Department : <span style="color: red">*</span></td>
            <td colspan="2"><asp:DropDownList ID="ddlDepartment1" runat="server" Width="192px" height="25px"/></td>
            <td align="right" style="width: 303px" colspan="2">Email : </td>
            <td  colspan="2"style="text-align: left"><asp:TextBox ID="txtemail" runat="server" Width="189px" /></td>
             <td colspan="2" align="right" style="width: 303px">Mobile : </td>
            <td colspan="2"><asp:TextBox ID="txtmobile" runat="server" Width="190px"></asp:TextBox></td>
            <td colspan="2">&nbsp;</td>
        </tr>
    
        <tr>
             <td align="right" style="width: 303px" colspan="2" height="30px">Supervisor : <span style="color: red">*</span></td>
            <td colspan="2"><asp:DropDownList ID="ddlSupervisor" runat="server" Width="192px" height="25px"/></td>
             <td align="right" style="width: 303px" colspan="2" height="30px">Is Approver : </td>
            <td colspan="2"><asp:CheckBox ID="chkApprover" runat="server" /></td>
            <td colspan="6" style="height: 20px;" align="center">
                <asp:Button ID="Button1" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small"
                    Text="Create User" OnClick="btncreate_Click" Style="position: relative" />
            </td>
        </tr>
        <tr>
            <td colspan="14" class="heading">&nbsp;Users</td>
        </tr>
        <tr>
            <td colspan="14">&nbsp;</td>
        </tr>
        <tr>
            <td colspan="14">
                <asp:GridView ID="gvModifyModule" CssClass="gridview" runat="server" AutoGenerateColumns="False" Width="100%"
                    BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" 
                    BorderWidth="1px" CellPadding="5"  GridLines="Both" Font-Size="Small" HeaderStyle-BackColor="#337ab7" HeaderStyle-HorizontalAlign="Center"
                    HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#f2f2f2"  OnRowCancelingEdit="gvModifyModule_RowCancelingEdit" OnRowEditing="gvModifyModule_RowEditing"
                    OnRowUpdating="gvModifyModule_RowUpdating" DataKeyNames="user_name" OnRowDeleting="gvModifyModule_RowDeleting" OnRowDataBound="gvModifyModule_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="User Name">
                            <ItemTemplate>
                                <asp:Label ID="lblUserName" runat="server" Text='<%# Bind("user_name") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Email">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEmail" runat="server" Enabled="False" Text='<%# Bind("email") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderStyle Font-Bold="True" />
                            <ItemTemplate>
                                <asp:Label ID="lblEmail" runat="server" Text='<%# Bind("email") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Mobile">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtMobile" runat="server" Enabled="False" Text='<%# Bind("mobile_no") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderStyle Font-Bold="True" />
                            <ItemTemplate>
                                <asp:Label ID="lblMobile" runat="server" Text='<%# Bind("mobile_no") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="Department"  ItemStyle-Width="80px">
                            <ItemTemplate>
                                <asp:Label ID="lblDepartment" runat="server" Text='<%# Eval("DepartmentName") %>' />

                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlDepartment" runat="server"/>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supervisor"  ItemStyle-Width="80px">
                            <ItemTemplate>
                                <asp:Label ID="lblSupervisor" runat="server" Text='<%# Eval("SupervisorName") %>' />

                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlSupervisor" runat="server"/>
                            </EditItemTemplate>
                        </asp:TemplateField>
                       
                      <%--  <asp:TemplateField HeaderText="Department">
                            <ItemTemplate>
                                <%# Eval("DepartmentName") %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlDepartment" runat="server" DataTextField="DepartmentName" DataValueField="DeptId"></asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>--%>

                        <asp:TemplateField HeaderText="Approver">
                            <ItemTemplate>
                                        <asp:CheckBox ID="chkApprover" runat="server" Checked='<%# Eval("IsApprover") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:CommandField ShowEditButton="true" ControlStyle-ForeColor="Black" />
                        <asp:CommandField ShowDeleteButton="true" ControlStyle-ForeColor="Black" />
                    </Columns>
                     <HeaderStyle HorizontalAlign="Right" Height="40" Font-Size="Small"  />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" Height="25"  />

                    <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
                    <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
                    <AlternatingRowStyle BackColor="#F7F7F7" />
                </asp:GridView>
            </td>
        </tr>
    </table>
</asp:Content>
