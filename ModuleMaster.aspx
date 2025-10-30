<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ModuleMaster.aspx.cs" Inherits="ComplianceManager.ModuleMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="server">
    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="View1" runat="server">
            <table cellpadding="1" cellspacing="1" class="TableBoun" width="100%">
                <tr>
                    <td colspan="5" class="heading">&nbsp;Module-Role Mapping</td>
                </tr>
                <tr>
                    <td></td>
                    <td class="calibri" align="right">Role ID&nbsp; &nbsp; &nbsp;<span style="color: #ff0000">&nbsp; </span>
                    </td>
                    <td class="calibri">:
                    </td>
                    <td style="color: #ff0000; height: 28px; text-align: left">
                        <asp:DropDownList ID="ddlRole" runat="server" AutoPostBack="True" Width="192px">
                        </asp:DropDownList></td>
                    <td style="width: 100%; color: #ff0000; height: 28px; text-align: left">
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="ddlRole"
                            Display="Dynamic" ErrorMessage="Please Select Role" InitialValue="0" Width="175px" ForeColor="WhiteSmoke">*</asp:RequiredFieldValidator></td>
                </tr>
                <tr style="color: #006400">
                    <td style="width: 1px; height: 22px"></td>
                    <td class="calibri" align="right">Module ID &nbsp;</td>
                    <td class="calibri">:
                    </td>
                    <td style="color: #006400; height: 22px; text-align: left">
                        <asp:DropDownList ID="ddlModule" runat="server" Width="192px">
                        </asp:DropDownList></td>
                    <td style="width: 100%; color: #006400; height: 22px; text-align: left">
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddlModule"
                            Display="Dynamic" ErrorMessage="Please Select Module" InitialValue="0" Width="175px" ForeColor="WhiteSmoke">*</asp:RequiredFieldValidator></td>
                </tr>
                <tr style="color: #ff0000">
                    <td style="width: 1px; height: 22px"></td>
                    <td style="width: 48%; height: 22px; text-align: right"></td>
                    <td style="color: #006400; height: 22px"></td>
                    <td align="left" style="color: #006400; height: 22px">
                        <asp:Button ID="btnModule" runat="server" OnClick="btnModule_Click" Text="Save" CssClass="clsButton" />
                    </td>
                    <td style="width: 100%; color: #006400; height: 22px; text-align: left">
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
                            ShowSummary="False" />

                    </td>
                </tr>
                <tr>
                    <td width="100%" colspan="5" class="heading">&nbsp;
                     <asp:Label ID="Label1" runat="server" Text="Display  Mapping"></asp:Label>
                        <asp:GridView ID="gvMapping" runat="server" AutoGenerateColumns="False" DataKeyNames="Role_ID"
                            OnRowCancelingEdit="gvMapping_RowCancelingEdit" OnRowEditing="gvMapping_RowEditing1"
                            OnRowUpdating="gvMapping_RowUpdating" Width="100%" OnRowDataBound="gvMapping_RowDataBound">
                            <Columns>
                                <asp:TemplateField HeaderText="Role ID">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtRoleId" runat="server" Enabled="False" Text='<%# Bind("Role_Id") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <HeaderStyle Font-Bold="True" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblRoleID" runat="server" Text='<%# Bind("Role_name") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Module ID">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtObjectID" runat="server" Text='<%# Bind("object_Name") %>' Enabled="False" Width="109px"></asp:TextBox>
                                        <asp:Label ID="lblRoleID" runat="server" Text='<%# Bind("object_id") %>' Visible="False"></asp:Label>
                                    </EditItemTemplate>
                                    <HeaderStyle Font-Bold="True" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblRoleName" runat="server" Text='<%# Bind("object_name") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Rights">
                                    <EditItemTemplate>
                                        <asp:CheckBox ID="chkRights" runat="server" Checked="True" />
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="ChkCheck" runat="server" Checked='<%# Bind("rights") %>' Enabled="False" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField ShowHeader="False">
                                    <EditItemTemplate>
                                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Update"
                                            Text="Update"></asp:LinkButton>
                                        <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel"
                                            Text="Cancel"></asp:LinkButton>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit"
                                            Text="Edit"></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblError" runat="server" CssClass="header" Font-Bold="True" Font-Names="Verdana"
                            Font-Size="Small" ForeColor="White" Height="23px" Width="100%"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="5" width="100%">
                        <asp:Label ID="Label3" runat="server" CssClass="header" Font-Bold="True" Font-Names="Verdana"
                            Font-Size="Small" ForeColor="White" Height="23px" Width="100%"></asp:Label></td>
                </tr>
            </table>
        </asp:View>
    </asp:MultiView>
</asp:Content>
