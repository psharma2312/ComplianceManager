<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ModifyUser.aspx.cs" Inherits="ComplianceManager.ModifyUser" %>
<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="server">
         <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <table cellpadding="1" cellspacing="1" width="100%" class="TableBoun">
        <tr>
            <td colspan="5" align="center">
                <asp:Label ID="lblError" runat="server" Width="100%"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="5" class="heading">&nbsp; Modify User
            </td>
        </tr>
        <tr>
            <td colspan="5" style="height: 20px;">&nbsp;
            </td>
        </tr>
        <tr>
            <td align="right" style="width: 100px" colspan="2">
                <asp:Label ID="lblUserID" Font-Size="Small" Text="User ID:" runat="server" SkinID="White"></asp:Label><span
                    style="color: red">*</span>
            </td>
            <td colspan="3">
                <asp:DropDownList ID="ddlUserId" runat="server" Width="195px" OnSelectedIndexChanged="ddlUserId_SelectedIndexChanged"
                    AutoPostBack="True"  DataTextField="UserName" DataValueField="UserId">
                </asp:DropDownList>
            </td>
           <%-- <td align="right" nowrap="nowrap" style="width: 120px">
                <asp:Label ID="lblusername" Font-Size="small" Text="user name:" runat="server" SkinID="white"></asp:Label>
                <span style="color: red">*</span>
            </td>
            <td>
                <asp:TextBox ID="txtusername" runat="server" Width="190px"></asp:TextBox>
            </td>
            <td>
                <asp:RequiredFieldValidator ID="rfvusername" runat="server" ControlToValidate="txtusername"
                    ErrorMessage="user name is required" Font-Names="verdana" Font-Size="x-small"
                    ValidationGroup="pp" Width="157px"></asp:RequiredFieldValidator>
            </td>--%>
        </tr>
        <%--<tr>
            <td align="right" style="width: 100px">
                <asp:Label ID="Label6" Font-Size="Small" Text="Password:" runat="server" SkinID="White"></asp:Label><span
                    style="color: red">*</span>
            </td>
            <td style="text-align: left">
                <asp:TextBox ID="txtpassword" runat="server" Width="195px" TextMode="Password"></asp:TextBox>
            </td>
            <td align="right" nowrap="noWrap" style="width: 120px">
                <asp:Label ID="Label7" Font-Size="Small" Text="Confirm Password:" runat="server"
                    SkinID="White"></asp:Label>
                <span style="color: red">*</span>
            </td>
            <td>
                <asp:TextBox ID="txtconpassword" runat="server" Width="190px" TextMode="Password"></asp:TextBox>
            </td>
            <td>
                <asp:CompareValidator ID="CVPassword" runat="server" ControlToCompare="txtpassword"
                    ControlToValidate="txtconpassword" Display="Dynamic" ErrorMessage="Plz Check Confirm Password"
                    Font-Names="Verdana" Font-Size="X-Small" Width="157px"></asp:CompareValidator>
            </td>
        </tr>--%>
        <tr>
            <td colspan="5" style="height: 20px;">&nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="5">
                <asp:GridView ID="gvModifyModule" CssClass="gridview" runat="server" Width="100%" AutoGenerateColumns="False"
                    OnRowDataBound="gvModifyModule_RowDataBound" BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" 
                    BorderWidth="1px" CellPadding="5"  GridLines="Both" Font-Size="Smaller" HeaderStyle-BackColor="#337ab7" 
                    HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#f2f2f2" DataKeyNames="ObjectId" >
                    <Columns>
                        <asp:TemplateField HeaderText="ID">
                            <ItemTemplate>
                                <asp:Label ID="lbl_ObjectID" runat="server" Text='<%# Bind("ObjectId") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Description">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("ObjectName") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Rights">
                            <ItemTemplate>
                                &nbsp;
                                        <asp:CheckBox ID="Chk_right" runat="server" Checked='<%# Eval("rights") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                     <HeaderStyle HorizontalAlign="Right" Height="40" Font-Size="Small"  />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" Height="25"  />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td colspan="3" align="right">
                <asp:Button ID="btnupdate" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small"
                    Text="Update" OnClick="btnupdate_Click" Style="position: relative; left: 0px; top: 0px;"
                    BackColor="White" BorderColor="Lime" BorderStyle="Solid" ForeColor="Navy" />
            </td>
        </tr>
    </table>
</asp:Content>
