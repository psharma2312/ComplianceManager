<%@ Page Title="" Language="C#" MasterPageFile="~/ComplianceM.Master" AutoEventWireup="true" CodeBehind="Notification.aspx.cs" Inherits="ComplianceManager.Notification1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cntPlaceHolder1" runat="server">
     <link rel="stylesheet" type="text/css" href="Content/common.css" />
    <style>
                                                            /* Heading Styling */
.heading {
    background-color: #f1f8ff; /* Light blue background matching dashboard theme */
    padding: 7px;
    border-bottom: 2px solid #0066cc; /* Darker blue border */
    margin-bottom: 10px;
}

    </style>
    <div id="inner_page">
        <div id="inner_page_main">
                   <div class="heading">
    <asp:Label ID="Label4" runat="server" Text="Notifications" />
</div>
            <div>
                <asp:Label ID="labelError" runat="server" Text="" BackColor="Yellow" /></div>
            <div id="blankspace"></div>
            <div id="inner_page_top">
                <table with="100%">


                    <tr>
                        <td colspan="20" style="height: 20px;">
                            <asp:Label ID="lblStatus" runat="server" Text="" Visible="false" BackColor="Yellow" />
                        </td>
                    </tr>

                </table>

            </div>

            <div id="inner_page_bottom">
                <table width="100%">
                    <tr>
                        <td>
                           <asp:GridView ID="GridView1"  CssClass="frequency-grid" runat="server" Width="100%" AutoGenerateColumns="False"
                            GridLines="Both" Font-Size="Smaller" HeaderStyle-BackColor="#337ab7" HeaderStyle-ForeColor="White"
                            AlternatingRowStyle-BackColor="#f2f2f2" DataKeyNames="NotificationId" EmptyDataText="No Notifications."
                            ShowHeaderWhenEmpty="true" OnRowCommand="GridView1_RowCommand"  OnPageIndexChanging="OnPageIndexChanging" 
                    PageSize="10" AllowPaging="true" >
                                <AlternatingRowStyle BackColor="#DCDCDC" />
                                <Columns>
                                    <asp:BoundField DataField="NotificationID" HeaderText="ID" ItemStyle-Width="30" />
                                    <asp:BoundField DataField="ComplianceRef" HeaderText="Compliance Ref" ItemStyle-Width="80" />
                                    <asp:BoundField DataField="NotificationMessage" HeaderText="NotificationMessage" ItemStyle-Width="280" />
                                    <asp:BoundField DataField="NotificationType" HeaderText="NotificationType" ItemStyle-Width="100" />
                                    <asp:BoundField DataField="NotificationDate" HeaderText="NotificationDate" ItemStyle-Width="90" />
                                    <asp:TemplateField HeaderText="Actions" ItemStyle-Width="80">
                                        <ItemTemplate>
                                            <asp:Button ID="btnMarkAsRead" runat="server" Text="Mark as Read" CommandName="MarkAsRead" CommandArgument='<%# Eval("NotificationID") %>'  CssClass="grid-button edit-button" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <div>
                            </div>
                        </td>
                    </tr>

                </table>
            </div>
        </div>
    </div>
</asp:Content>
