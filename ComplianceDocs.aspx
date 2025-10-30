<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ComplianceDocs.aspx.cs" Inherits="ComplianceManager.ComplianceDocs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="server">
      <table width="100%" border="0">
        <tr><td colspan="8" class="heading"> &nbsp;&nbsp;Upload Compliance Docs for each Compliance</td></tr>
        <tr>
            <td style="width: 10px;" height="20px"></td>
            <td align="right" style="width: 100px"></td>
            <td width="200px"><asp:Label ID="labelError" runat="server" Text=""></asp:Label></td>
            <td style="width: 100px; color: #000; font-weight: bold;"></td>
            <td style="width: 10px;" colspan="4"></td>
        </tr>
        <tr>
            <td align="right" style="width: 80px" colspan="2">
                <asp:Label ID="Label2" Font-Size="Small" Text="Compliance Name:" runat="server" SkinID="White" ></asp:Label>
            </td>
            <td><asp:TextBox ID="txtName" runat="server"></asp:TextBox></td>

            <td align="right" style="width: 100px"><asp:Label ID="lblFileType" Font-Size="Small" Text="File Type:" runat="server" SkinID="White"></asp:Label></td>
            <td width="200px">
                <asp:DropDownList ID="ddlFileType" runat="server" Width="180px" OnSelectedIndexChanged="ddlFileType_SelectedIndexChanged"
                    AutoPostBack="false" DataTextField="Code" DataValueField="ID"></asp:DropDownList></td>
            <td style="width: 90px; color: #000; font-weight: bold;"><asp:FileUpload ID="FileUpload1" runat="server" /></td>
            <td style="width: 90px; color: #000;" colspan="2"><asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click"
                    Style="color: #000;" /><asp:Label ID="Label1" runat="server" Text=""></asp:Label></td>
            
        </tr>
        <tr><td style="width: 10px"  colspan="8">&nbsp;</td></tr>
       
        <tr><td colspan="8" style="height: 20px;"></td></tr>
        <tr>
        
            <td colspan="8">
                <asp:GridView ID="gvCompliance" runat="server" BackColor="White" AutoGenerateColumns="false"
                    BorderColor="#E7E7FF" BorderStyle="None" 
                    BorderWidth="1px" CellPadding="5"  GridLines="Both" Font-Size="Smaller" HeaderStyle-BackColor="#337ab7" HeaderStyle-HorizontalAlign="Center"
                    HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#f2f2f2" >
                    <Columns>
                        <asp:BoundField DataField="Code" HeaderText="SKU Code" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="left"
                            HeaderStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="Brand" HeaderText="Brand" ItemStyle-Width="100px" Visible="true"  HeaderStyle-HorizontalAlign="Center"/>
                        <asp:ImageField HeaderText="Logo Image" DataImageUrlField="Category" Visible="false"
                            ItemStyle-HorizontalAlign="center" HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="50px"
                            ItemStyle-Height="25px" ControlStyle-Width="50px" ControlStyle-Height="25px" />
                        <asp:BoundField DataField="QTY" HeaderText="Quantity" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="left"
                            HeaderStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Batch" HeaderText="Batch" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="left"
                            HeaderStyle-HorizontalAlign="Center" />
                        <asp:BoundField HeaderText="MRP" DataField="MRP" Visible="true"
                            ItemStyle-Width="50px"  HeaderStyle-HorizontalAlign="left"  ItemStyle-HorizontalAlign="left"/>
                        <asp:BoundField HeaderText="Date" DataField="DateAdded" ItemStyle-Width="150px" ItemStyle-Height="25px"
                            ControlStyle-Width="50px" ControlStyle-Height="25px" ItemStyle-HorizontalAlign="left"
                            HeaderStyle-HorizontalAlign="Center" />
                    </Columns>
                    <RowStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
                    <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
                    <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
                    <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
                    <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" />
                    <AlternatingRowStyle BackColor="#F7F7F7" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
        <td width="10px">&nbsp;</td>
            <td colspan="8"><asp:Button ID="btnPrintLabel" runat="server" Text="Print Labels" OnClick="btnPrintLabel_Click" /></td>
        </tr>
        <tr><td width="10px">&nbsp;</td></tr>
          
    </table>

</asp:Content>
