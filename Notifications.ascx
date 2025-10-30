<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Notifications.ascx.cs" Inherits="ComplianceManager.Notifications" %>
    <style>
                                                            /* Heading Styling */
.heading {
    background-color: #f1f8ff; /* Light blue background matching dashboard theme */
    padding: 7px;
    border-bottom: 2px solid #0066cc; /* Darker blue border */
    margin-bottom: 10px;
}
.frequency-grid {
    width: 100%;
    border-collapse: collapse;
    margin-top: 20px;
}

    .frequency-grid th {
        background-color: #0066cc;
        color: white;
        padding: 5px;
        text-align: left;
        font-size: 12px;
    }

    .frequency-grid td {
        padding: 3px;
        border-bottom: 1px solid #ddd;
        font-size: 12px;
    }

.grid-button {
    padding: 2px 5px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 12px;
}

.edit-button {
    background-color: #007bff;
    color: white;
}

    .edit-button:hover {
        background-color: #0069d9;
    }

.update-button {
    background-color: #28a745;
    color: white;
}

    .update-button:hover {
        background-color: #218838;
    }

.cancel-button {
    background-color: #6c757d;
    color: white;
}

    .cancel-button:hover {
        background-color: #5a6268;
    }

.delete-button {
    background-color: #dc3545;
    color: white;
}

    .delete-button:hover {
        background-color: #c82333;
    }
    </style>
    <div id="inner_page">
        <div id="inner_page_main">
            <div class="heading">
                <asp:Label ID="Label4" runat="server" Text="Notifications" />
            </div>
            <div>
             <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" Visible="false"  />
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
                                ShowHeaderWhenEmpty="true" OnRowCommand="GridView1_RowCommand" OnPreRender="GridView1_PreRender">
                                <AlternatingRowStyle BackColor="#DCDCDC" />
                                <Columns>
                                    <asp:BoundField DataField="NotificationID" HeaderText="ID" ItemStyle-Width="160" />
                                    <asp:BoundField DataField="NotificationMessage" HeaderText="NotificationMessage" ItemStyle-Width="80" />
                                    <asp:BoundField DataField="NotificationType" HeaderText="NotificationType" ItemStyle-Width="190" />
                                   <asp:BoundField DataField="NotificationDate" HeaderText="Date" DataFormatString="{0:dd-MM-yyyy HH:mm}" />
                                    <asp:TemplateField HeaderText="Actions" ItemStyle-Width="150">
                                        <ItemTemplate>
                                            <asp:Button ID="btnMarkAsRead" runat="server" Text="Mark as Read" CommandName="MarkAsRead" 
                                                        CommandArgument='<%# Eval("NotificationID") %>' CssClass="btn btn-success" />
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
    </div>