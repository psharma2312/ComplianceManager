<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ImageUpload.aspx.cs" Inherits="ComplianceManager.ImageUpload" %>
<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="server">
    <table class="style1">
    <tr>
            <td colspan="20" style="border-bottom: thin solid #008080; font-weight: 700; text-align: left;">&nbsp; Assign Compliances</td>
        </tr>
        <tr>
            <td width="200px" colspan="20">
                <asp:Label ID="labelError" runat="server" Text="" BackColor="Yellow" />&nbsp;</td>
        </tr>
        <tr>
            <td colspan="3" style="width: 200px; text-align: right; height: 30px;">Department :</td>
            <td colspan="3">
                <asp:DropDownList ID="ddlDepartment" runat="server" DataTextField="DepartmentName" DataValueField="DeptId" Width="180px" Height="25px" 
                   OnSelectedIndexChanged="ddlDepartment_SelectedIndexChanged" AutoPostBack="true" /></td>
            <td colspan="1" width="25px">&nbsp;</td>
            <td colspan="3" style="text-align: right; height: 30px;">&nbsp;</td>
            <td colspan="4">
              &nbsp;</td>
            <td colspan="2" style="width: 120px; text-align: right; height: 30px;">&nbsp;</td>
            <td colspan="4" style="width: 120px; text-align: right; height: 30px;">&nbsp;</td>

        </tr>
         <tr>
            <td colspan="20" style="height: 20px;">&nbsp;
            </td>
        </tr>
          <tr>
            <td colspan="20">
                
           
            
    <asp:GridView ID="GridView1" runat="server" Width="100%" AutoGenerateColumns="False"
                    OnRowDataBound="GridView1_RowDataBound" BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" 
                    BorderWidth="1px" CellPadding="5"  GridLines="Both" Font-Size="Smaller" HeaderStyle-BackColor="#337ab7" 
                    HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#f2f2f2" DataKeyNames="ComplianceID" >  
            <AlternatingRowStyle BackColor="#DCDCDC" />  
            <Columns>  
                <asp:TemplateField HeaderText="Compliance ID">  
                    <EditItemTemplate>  
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("ComplianceID") %>'></asp:TextBox>  
                    </EditItemTemplate>  
                    <ItemTemplate>  
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("ComplianceID") %>'></asp:Label>  
                    </ItemTemplate>  
                </asp:TemplateField>  
                <asp:TemplateField HeaderText="Compliance Area">  
                    <EditItemTemplate>  
                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("ComplianceArea") %>'></asp:TextBox>  
                    </EditItemTemplate>  
                    <ItemTemplate>  
                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("ComplianceArea") %>'></asp:Label>  
                    </ItemTemplate>  
                </asp:TemplateField> 
                 <asp:TemplateField HeaderText="Compliance Type">  
                            <EditItemTemplate>  
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("ComplianceTypeName") %>'></asp:TextBox>  
                            </EditItemTemplate>  
                            <ItemTemplate>  
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("ComplianceTypeName") %>'></asp:Label>  
                            </ItemTemplate>  
                        </asp:TemplateField>
                 <asp:TemplateField HeaderText="Governing Legislation">  
                            <EditItemTemplate>  
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("GovernmentLegislation") %>'></asp:TextBox>  
                            </EditItemTemplate>  
                            <ItemTemplate>  
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("GovernmentLegislation") %>'></asp:Label>  
                            </ItemTemplate>  
                        </asp:TemplateField>
                <asp:TemplateField HeaderText="Act/ Section">  
                            <EditItemTemplate>  
                                <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("ActSectionReference") %>'></asp:TextBox>  
                            </EditItemTemplate>  
                            <ItemTemplate>  
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("ActSectionReference") %>'></asp:Label>  
                            </ItemTemplate>  
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="Compliance Nature">  
                            <EditItemTemplate>  
                                <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("NatureOfComplianceName") %>'></asp:TextBox>  
                            </EditItemTemplate>  
                            <ItemTemplate>  
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("NatureOfComplianceName") %>'></asp:Label>  
                            </ItemTemplate>  
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="EffectiveFrom">  
                            <EditItemTemplate>  
                                <asp:TextBox ID="TextBox7" runat="server" Text='<%# Bind("EffectiveFrom" , "{0:dd-MM-yyyy}") %>'></asp:TextBox>  
                            </EditItemTemplate>  
                            <ItemTemplate>  
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("EffectiveFrom" , "{0:dd-MM-yyyy}") %>'></asp:Label>  
                            </ItemTemplate>  
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Due Date">  
                            <EditItemTemplate>  
                                <asp:TextBox ID="TextBox8" runat="server" Text='<%# Bind("StandardDueDate" , "{0:dd-MM-yyyy}") %>'></asp:TextBox>  
                            </EditItemTemplate>  
                            <ItemTemplate>  
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("StandardDueDate" , "{0:dd-MM-yyyy}") %>'></asp:Label>  
                            </ItemTemplate>  
                        </asp:TemplateField>
                <asp:TemplateField HeaderText="Qualification">  
                    <EditItemTemplate>  
                        <asp:TextBox ID="TextBox3" runat="server"></asp:TextBox>  
                    </EditItemTemplate>  
                    <ItemTemplate>  
                        <asp:DropDownList ID="DropDownList1" runat="server">  
                        </asp:DropDownList>  
                    </ItemTemplate>  
                </asp:TemplateField>  
             
            </Columns>  
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />  
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />  
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />  
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />  
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />  
            <SortedAscendingCellStyle BackColor="#F1F1F1" />  
            <SortedAscendingHeaderStyle BackColor="#0000A9" />  
            <SortedDescendingCellStyle BackColor="#CAC9C9" />  
            <SortedDescendingHeaderStyle BackColor="#000065" />  
        </asp:GridView>  
       </tr>
         <tr>
            <td colspan="3" align="right">
                <asp:Button ID="btnupdate" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small"
                    Text="Update" OnClick="btnupdate_Click" Style="position: relative; left: 0px; top: 0px;"
                    BackColor="White" BorderColor="Lime" BorderStyle="Solid" ForeColor="Navy" Visible="false" />
            </td>
        </tr>
    </table>
</asp:Content>
