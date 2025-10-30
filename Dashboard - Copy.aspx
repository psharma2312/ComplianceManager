<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="ComplianceManager.Dashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script> <!-- Include Chart.js -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script> <!-- jQuery for AJAX -->

       <div id="inner_page">
 <div id="inner_page_main">
     <div class="heading">&nbsp;Dashboard</div>
     <div>
         <asp:Label ID="labelError" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Medium"></asp:Label>
     </div>
     <div id="blankspace"></div>
     <div id="inner_page_top">
         <table with="100%">
      <tr>
            <td width="1%">&nbsp;</td>
            <td colspan="4"  style="text-align:center; width:24%; height:100px; font-family:Verdana; font-size:15px; font-weight:900; background-color:#17A2B8" >
                <asp:Label ID="lblTotalAssigned" runat="server" Text="150" ForeColor="White" /><br />
                <asp:LinkButton  ID="lnkTab1" runat="server" OnClick="lnkTab1_Click" style="color:#fff;">Total Compliance This Month</asp:LinkButton>
            </td>
            <td width="1%">&nbsp;</td>
            <td colspan="4" style="text-align:center; width:24%; height:100px; font-family:Verdana; font-size:15px; font-weight:900; background-color:#28A745">
                 <asp:Label ID="lblComplianceDue" runat="server" Text="150" ForeColor="White" /><br />
                <asp:LinkButton ID="lnkTab2" runat="server" OnClick="lnkTab2_Click" style="color:#fff;">Due This Month</asp:LinkButton>
            </td>
            <td width="1%">&nbsp;</td>
            <td colspan="4" style="text-align:center; width:24%; height:100px; font-family:Verdana; font-size:15px; font-weight:900; background-color:#FFC107">
                 <asp:Label ID="lblPendingApproval" runat="server" Text="150" ForeColor="White" /><br />
                <asp:LinkButton ID="lnkTab3" runat="server" OnClick="lnkTab3_Click" style="color:#fff;">Pending Approval</asp:LinkButton>
            </td>
            <td width="1%">&nbsp;</td>
            <td colspan="4" style="text-align:center; width:24%; height:100px; font-family:Verdana; font-size:15px; font-weight:900; background-color:#DC3545">
                 <asp:Label ID="lblDelayed" runat="server" Text="150" ForeColor="White" /><br />
                <asp:LinkButton ID="lnkTab4" runat="server" OnClick="lnkTab4_Click" style="color:#fff;">Delayed Compliances</asp:LinkButton>
            </td>
            <td width="1%">&nbsp;</td>
        </tr>
                </table>

        </div>

<div id="inner_page_bottom">
    <table width="100%">
        <tr>
            <td>
                 <div style="width: 50%; margin: auto; text-align: center;">
            <h2>Compliance Summary</h2>
            
            <!-- Pie Chart Canvas -->
            <canvas id="complianceChart" width="400" height="400"></canvas>

            <!-- GridView for Details -->
            <asp:GridView ID="gvComplianceDetails" runat="server" AutoGenerateColumns="true" CssClass="table" Visible="false"></asp:GridView>
        </div>
                <script>
            var complianceData = {}; // Stores compliance summary data

            function loadChart(data) {
                complianceData = data; // Store data for click events
                
                var ctx = document.getElementById('complianceChart').getContext('2d');
                new Chart(ctx, {
                    type: 'pie',
                    data: {
                        labels: ["Due This Month", "Pending Approval", "Delayed", "Total Assigned"],
                        datasets: [{
                            label: "Compliance Summary",
                            data: [data.due, data.pending, data.delayed, data.total],
                            backgroundColor: ["blue", "orange", "red", "green"]
                        }]
                    },
                    options: {
                        responsive: true,
                        onClick: function (event, elements) {
                            if (elements.length > 0) {
                                var index = elements[0].index;
                                var status = ["due", "pending", "delayed", "total"][index];
                                loadGridView(status);
                            }
                        }
                    }
                });
            }

            function loadGridView(status) {
                $.ajax({
                    type: "POST",
                    url: "Dashboard.aspx/GetComplianceDetails",
                    data: JSON.stringify({ status: status }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    xhrFields: { withCredentials: true }, // Ensures session authentication
                    success: function (response) {
                        var grid = document.getElementById('<%= gvComplianceDetails.ClientID %>');
                        var data = response.d;
                        var html = "<table border='1'><tr><th>Compliance Name</th><th>Due Date</th><th>Status</th></tr>";

                        data.forEach(function (item) {
                            html += "<tr><td>" + item.ComplianceName + "</td><td>" + item.DueDate + "</td><td>" + item.Status + "</td></tr>";
                        });

                        html += "</table>";
                        grid.innerHTML = html;
                        grid.style.display = "block";
                    },
                    error: function (xhr, status, error) {
                        console.log("Error:", xhr.responseText);
                        if (xhr.status === 401) {
                            alert("Session expired. Please log in again.");
                            window.location.href = "Login.aspx";
                        }
                    }
                });
            }
        </script>
               <%-- <asp:MultiView ID="MultiView1" runat="server">
                    <table width="10%" cellpadding="2" cellspacing="5">
                        <tr>
                            <td>
                                <asp:View ID="View1" runat="server">
                                    <asp:GridView ID="gvDueThisMonth"  CssClass="gridview" runat="server" Width="100%" AutoGenerateColumns="False"
                    BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" HeaderStyle-Height="50px"
                    BorderWidth="1px" CellPadding="5" GridLines="Both" Font-Size="Smaller" HeaderStyle-BackColor="#337ab7" HeaderStyle-HorizontalAlign="Center"
                    HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#f2f2f2" DataKeyNames="ComplianceID,ComplianceDetailID">
                   
                    <AlternatingRowStyle BackColor="#DCDCDC" />
                    <Columns>
                        <asp:BoundField DataField="ComplianceArea" HeaderText="Compliance Area" ItemStyle-Width="160" ItemStyle-Height="25px" HeaderStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="ComplianceTypeName" HeaderText="Compliance Type" ItemStyle-Width="160" HeaderStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="EffectiveFrom" HeaderText="EffectiveFrom" ItemStyle-Width="90" DataFormatString="{0:dd-MMM-yyyy}" HeaderStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="StandardDueDate" HeaderText="Due Date" ItemStyle-Width="85" DataFormatString="{0:dd-MMM-yyyy}" HeaderStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="ComplianceStatusName" HeaderText="Status" ItemStyle-Width="80" HeaderStyle-HorizontalAlign="Center" />
                        <asp:CommandField ShowSelectButton="True" ItemStyle-Font-Bold="true" ItemStyle-HorizontalAlign="Center" />
                    </Columns>
                    <HeaderStyle HorizontalAlign="Right" Height="40" Font-Size="Small"  />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" Height="25"  />
                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                    <SortedAscendingCellStyle BackColor="#F1F1F1" />
                    <SortedAscendingHeaderStyle BackColor="#0000A9" />
                    <SortedDescendingCellStyle BackColor="#CAC9C9" />
                    <SortedDescendingHeaderStyle BackColor="#000065" />
                </asp:GridView>
                                </asp:View>
                            </td>
                            <td>
                                <asp:View ID="View2" runat="server">
                                    <asp:GridView ID="gvSubmitted" runat="server" Width="100%" AutoGenerateColumns="False"
                    BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" HeaderStyle-Height="50px"
                    BorderWidth="1px" CellPadding="5" GridLines="Both" Font-Size="Smaller" HeaderStyle-BackColor="#337ab7" HeaderStyle-HorizontalAlign="Center"
                    HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#f2f2f2" DataKeyNames="ComplianceID,ComplianceDetailID">
                   
                    <AlternatingRowStyle BackColor="#DCDCDC" />
                    <Columns>
                        <asp:BoundField DataField="ComplianceArea" HeaderText="Compliance Area" ItemStyle-Width="160" HeaderStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="ComplianceTypeName" HeaderText="Compliance Type" ItemStyle-Width="160" HeaderStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="EffectiveFrom" HeaderText="EffectiveFrom" ItemStyle-Width="90" DataFormatString="{0:dd-MMM-yyyy}" HeaderStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="StandardDueDate" HeaderText="Due Date" ItemStyle-Width="85" DataFormatString="{0:dd-MMM-yyyy}" HeaderStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="ComplianceStatusName" HeaderText="Status" ItemStyle-Width="80" HeaderStyle-HorizontalAlign="Center" />
                        <asp:CommandField ShowSelectButton="True" ItemStyle-Font-Bold="true" ItemStyle-HorizontalAlign="Center" />
                    </Columns>
                    <HeaderStyle HorizontalAlign="Center" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                    <SortedAscendingCellStyle BackColor="#F1F1F1" />
                    <SortedAscendingHeaderStyle BackColor="#0000A9" />
                    <SortedDescendingCellStyle BackColor="#CAC9C9" />
                    <SortedDescendingHeaderStyle BackColor="#000065" />
                </asp:GridView>
                                </asp:View>
                            </td>
                            <td>
                                <asp:View ID="View3" runat="server">
                                    <asp:GridView ID="gvNotSubmitted" runat="server" Width="100%" AutoGenerateColumns="False"
                    BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" HeaderStyle-Height="50px"
                    BorderWidth="1px" CellPadding="5" GridLines="Both" Font-Size="Smaller" HeaderStyle-BackColor="#337ab7" HeaderStyle-HorizontalAlign="Center"
                    HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#f2f2f2" DataKeyNames="ComplianceID,ComplianceDetailID">
                    
                    <AlternatingRowStyle BackColor="#DCDCDC" />
                    <Columns>
                        <asp:BoundField DataField="ComplianceArea" HeaderText="Compliance Area" ItemStyle-Width="160" HeaderStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="ComplianceTypeName" HeaderText="Compliance Type" ItemStyle-Width="160" HeaderStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="EffectiveFrom" HeaderText="EffectiveFrom" ItemStyle-Width="90" DataFormatString="{0:dd-MMM-yyyy}" HeaderStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="StandardDueDate" HeaderText="Due Date" ItemStyle-Width="85" DataFormatString="{0:dd-MMM-yyyy}" HeaderStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="ComplianceStatusName" HeaderText="Status" ItemStyle-Width="80" HeaderStyle-HorizontalAlign="Center" />
                        <asp:CommandField ShowSelectButton="True" ItemStyle-Font-Bold="true" ItemStyle-HorizontalAlign="Center" />
                    </Columns>
                    <HeaderStyle HorizontalAlign="Center" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                    <SortedAscendingCellStyle BackColor="#F1F1F1" />
                    <SortedAscendingHeaderStyle BackColor="#0000A9" />
                    <SortedDescendingCellStyle BackColor="#CAC9C9" />
                    <SortedDescendingHeaderStyle BackColor="#000065" />
                </asp:GridView>
                                </asp:View>
                            </td>
                            <td>
                                <asp:View ID="View4" runat="server">
                                   <asp:GridView ID="gvPending" runat="server" Width="100%" AutoGenerateColumns="False"
                    BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" HeaderStyle-Height="50px"
                    BorderWidth="1px" CellPadding="5" GridLines="Both" Font-Size="Smaller" HeaderStyle-BackColor="#337ab7" HeaderStyle-HorizontalAlign="Center"
                    HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#f2f2f2" DataKeyNames="ComplianceID,ComplianceDetailID">
                    
                    <AlternatingRowStyle BackColor="#DCDCDC" />
                    <Columns>
                        <asp:BoundField DataField="ComplianceArea" HeaderText="Compliance Area" ItemStyle-Width="160" HeaderStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="ComplianceTypeName" HeaderText="Compliance Type" ItemStyle-Width="160" HeaderStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="EffectiveFrom" HeaderText="EffectiveFrom" ItemStyle-Width="90" DataFormatString="{0:dd-MMM-yyyy}" HeaderStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="StandardDueDate" HeaderText="Due Date" ItemStyle-Width="85" DataFormatString="{0:dd-MMM-yyyy}" HeaderStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="ComplianceStatusName" HeaderText="Status" ItemStyle-Width="80" HeaderStyle-HorizontalAlign="Center" />
                        <asp:CommandField ShowSelectButton="True" ItemStyle-Font-Bold="true" ItemStyle-HorizontalAlign="Center" />
                    </Columns>
                    <HeaderStyle HorizontalAlign="Center" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                    <SortedAscendingCellStyle BackColor="#F1F1F1" />
                    <SortedAscendingHeaderStyle BackColor="#0000A9" />
                    <SortedDescendingCellStyle BackColor="#CAC9C9" />
                    <SortedDescendingHeaderStyle BackColor="#000065" />
                </asp:GridView>
                                </asp:View>
                            </td>
                        </tr>
                    </table>
                </asp:MultiView>--%>
            </td>
            <td width="1%">&nbsp;</td>
                            </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
