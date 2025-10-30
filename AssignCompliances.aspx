<%@ Page Title="" Language="C#" MasterPageFile="~/ComplianceM.Master" AutoEventWireup="true" CodeBehind="AssignCompliances.aspx.cs" Inherits="ComplianceManager.AssignCompliances" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cntPlaceHolder1" runat="server">
<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <link rel="stylesheet" href="Content/AssignCompliance.css" />

    <script type="text/javascript">
        function openModal() {
            $('[id*=myModal]').modal('show');
        }
    </script>
     <style type="text/css">
 .error-message {color: #721c24;background-color: #f8d7da;padding: 10px;border: 1px solid #f5c6cb;border-radius: 4px;margin-bottom: 15px;display: block;        }
.details-table { width: 100%; border-collapse: collapse; margin-top: 10px; }
 .details-table td { padding: 5px; vertical-align: top; }
 .details-label { font-weight: bold; font-size: 12px; }
 .details-value { font-size: 12px; }
 </style>
    
<div id="inner_page">
    <div id="inner_page_main">
        <div class="heading">
             <asp:Label ID="Label4" runat="server" Text="Assign Compliances" />
        </div>
       <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" Visible="false"  />
            <div id="blankspace"></div>
            <div id="inner_page_top">
                <table with="100%" border="0">
                    <tr>
                        <td colspan="6" style="text-align: right; height: 30px;">
                            <asp:Label ID="lblDepartment" runat="server" Text="Department" AssociatedControlID="ddlDeparmentLoad" CssClass="control-label" />
                            <asp:DropDownList ID="ddlDeparmentLoad" runat="server" DataTextField="DepartmentName" DataValueField="DeptId"
                                OnSelectedIndexChanged="ddlDepartmentLoad_SelectedIndexChanged" AutoPostBack="true" Width="160px" Height="22px" /></td>
                        <td colspan="6" style="text-align: right; height: 30px;">
                            <asp:Label ID="lblCompType" runat="server" Text="Compliance Type" AssociatedControlID="ddlComplianceType" CssClass="control-label" />
                             &nbsp;&nbsp;
                            <asp:DropDownList ID="ddlComplianceType" runat="server" DataTextField="ComplianceTypeName" DataValueField="ComplianceTypeId" Width="180px" Height="22px" /></td>
                        <td colspan="6" style="text-align: right; height: 30px;">
                            <asp:Label ID="lblCompArea" runat="server" Text="Compliance Area" AssociatedControlID="ddlComplianceAreaLoad" CssClass="control-label" />
                             &nbsp;&nbsp;
                            <asp:DropDownList ID="ddlComplianceAreaLoad" runat="server" DataTextField="ComplianceAreaName" DataValueField="ComplianceAreaName" Width="180px" Height="22px" />&nbsp;&nbsp;
                         <asp:Button ID="btnLoad" runat="server"  CssClass="load-button" Text="Load" OnClick="btnLoad_Click" />
                        </td>
                        <td colspan="2" style="text-align: right; height: 30px;"></td>
                    </tr>
                 
                    <tr>
                        <td colspan="20" style="height: 20px;">
                            <asp:Label ID="lblStatus" runat="server" Text="" Visible="false" BackColor="Yellow" />
                        </td>
                    </tr>
                </table>

            </div>

            <div id="inner_page_bottom">
                <div class="grid-container">
                    <asp:GridView ID="GridView2"  CssClass="frequency-grid"  runat="server" Width="100%" AutoGenerateColumns="False"
                                BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" AllowPaging="true" PageSize="8"
                                OnPageIndexChanging="OnPageIndexChanging" OnSelectedIndexChanged="OnSelectedIndexChanged"
                                BorderWidth="1px" CellPadding="5" GridLines="Both" Font-Size="Smaller" HeaderStyle-BackColor="#337ab7" HeaderStyle-HorizontalAlign="Center"
                                HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#f2f2f2" DataKeyNames="ComplianceID,ComplianceDetailID, DeptId "
                                  EmptyDataText="No Compliances assigned to the department." ShowHeaderWhenEmpty="true">
                                <AlternatingRowStyle BackColor="#DCDCDC" />
                                <Columns>
                                    <asp:BoundField DataField="ComplianceID" HeaderText="ID" ItemStyle-Width="40" ReadOnly="true" HeaderStyle-HorizontalAlign="Left"  />
                                    <asp:BoundField DataField="ComplianceArea" HeaderText="Compliance Area" ItemStyle-Width="160" ReadOnly="true" HeaderStyle-HorizontalAlign="Left"  />
                                    <asp:BoundField DataField="ComplianceTypeName" HeaderText="Compliance Type" ItemStyle-Width="160" ReadOnly="true" HeaderStyle-HorizontalAlign="Left"  />
                                    <asp:BoundField DataField="DepartmentName" HeaderText="Department" ItemStyle-Width="160" ReadOnly="true" HeaderStyle-HorizontalAlign="Left"  />
                                    <asp:BoundField DataField="ActSectionReference" HeaderText="Act Section" ItemStyle-Width="100" ReadOnly="true"  HeaderStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="NatureOfComplianceName" HeaderText="Compliance Nature" ItemStyle-Width="140"  ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="StandardDueDate" HeaderText="Due Date" ItemStyle-Width="85" DataFormatString="{0:dd-MMM-yyyy}"  ReadOnly="true"  HeaderStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="FrequencyName" HeaderText="Frequency" ItemStyle-Width="85" DataFormatString="{0:dd-MMM-yyyy}"  ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                    <%--<asp:BoundField DataField="DeptId" Visible="false" HeaderText="Dept" ItemStyle-Width="85" ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />--%>

                                    <asp:TemplateField HeaderText="Details" ItemStyle-Width="420px">
                                        <ItemTemplate>
                                            <span><%# string.Join(" ", ((string)Eval("DetailsOfComplianceRequirements")).Split(' ').Take(10)) + "..." %></span>
                                            <asp:LinkButton ID="lnkBtnEdit" runat="server" Text="..." style="padding:0px 6px;font-size:8px;color: #fff;
                                                    background-color: #5bc0de;vertical-align: middle; font-weight: 400;border: 1px solid transparent; 
                                                    border-color: #46b8da;border-radius: 4px;" OnClick="Display"></asp:LinkButton>
                                        </ItemTemplate>
                                       
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Assign To" ItemStyle-Width="160px">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPreparer" runat="server" Text='<%# Bind("PreparerName") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                  <asp:CommandField ShowSelectButton="True" ItemStyle-Font-Bold="true" ItemStyle-HorizontalAlign="Center" />
                                </Columns>
                            
                               <RowStyle BackColor="#FFFFFF" /> 
                                <AlternatingRowStyle BackColor="#F0F0F0" /> 
                                <SelectedRowStyle BackColor="#FFFF00" /> 
                                <PagerStyle CssClass="pager-style" />
                                <EmptyDataTemplate>
                                    <div align="center">No Compliances for the department.</div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                                   <div>
    <div id="forgotPasswordModal" class="modal" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Details</h4>
                    <span class="modal-close" onclick="hideModal()">×</span>
                </div>
                <div class="modal-body">
                    <table class="details-table">
                        <tr>
                            <td  style="text-align:left;">
                                <asp:Label ID="lblComplianceRefLabel" runat="server" Text="Compliance Ref:" CssClass="details-label" />
                                <asp:Label ID="lblComplianceRef" runat="server" Text="" CssClass="details-value" />
                            </td>
                            <td style="text-align:left;">
                                <asp:Label ID="lblComplianceAreaLabel" runat="server" Text="Compliance Area:" CssClass="details-label" />
                                <asp:Label ID="lblComplianceAreaValue" runat="server" Text="" CssClass="details-value" />
                            </td>
                        </tr>
                         <tr>
                            <td  style="text-align:left;">
                                 <asp:Label ID="lblNonCompLabel" runat="server" Text="Non Compliance Penalty:" CssClass="details-label" />
                                 <asp:Label ID="lblNonCompliance" runat="server" Text="" CssClass="details-value" />
                             </td>
                              <td colspan="4"  style="text-align:left;">
                                 <asp:Label ID="lblActionLabel" runat="server" Text="Action to be taken:" CssClass="details-label" />
                                 <asp:Label ID="lblActionToBeTaken" runat="server" Text="" CssClass="details-value" />
                             </td>
                         </tr>
   
 
                        <tr>
                            <td style="text-align:left;">
                                 <asp:Label ID="lblActSecLabel" runat="server" Text="Act Section:" CssClass="details-label" />
                                 <asp:Label ID="lblActSec" runat="server" Text="" CssClass="details-value" />
                             </td>
                            <td style="text-align:left;">
                                <asp:Label ID="lblPrioritLabel" runat="server" Text="Priority:" CssClass="details-label" />
                                <asp:Label ID="lblPrio" runat="server" Text="" CssClass="details-value" />
                            </td>
                        </tr>
                        <tr>
                           <td style="text-align:left;">
                                <asp:Label ID="lblLawtLabel" runat="server" Text="LawType:" CssClass="details-label" />
                                <asp:Label ID="lblLawt" runat="server" Text="" CssClass="details-value" />
                            </td>
                           <td style="text-align:left;">
                               <asp:Label ID="lblDrivenBLabel" runat="server" Text="Driven By:" CssClass="details-label" />
                               <asp:Label ID="lblDriv" runat="server" Text="" CssClass="details-value" />
                           </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="text-align:left;">
                                <asp:Label ID="lblDetailsLabel" runat="server" Text="Details:" CssClass="details-label" />
                                <asp:Label ID="lblDetails" runat="server" Text="" CssClass="details-value" />
                            </td>
                        </tr>
   
                    </table>
                   <%-- <div class="col-lg-12 col-sm-12 col-md-12 col-xs-12">
                        <p style="margin: 0 0 10px;"><asp:Label ID="dscpp" runat="server" Text=""></asp:Label></p>
                    </div>--%>
                </div>
            </div>
        </div>
    </div>
</div>
                </div>
             <table width="100%">
                <tr>
                    <td colspan="20" width="100%">
                    <asp:Label width="100%" ID="lblDocUpload" runat="server" style="background-color: #f1f8ff;padding: 7px;border-bottom: 2px solid #0066cc; margin-bottom: 10px;" 
                        Font-Bold="True" Font-Size="Large" Text="Assign Compliance" Visible="false" />

                    </td>
                </tr>
                  <tr>
                       <td colspan="20" style="width: 550px; text-align: left; height: 22px;">
                            <asp:Label ID="lblAssignTo" runat="server" CssClass="control-label-assign" Text="Assign To :" Visible="false"/>&nbsp;&nbsp;
                            <asp:Label ID="lblCompDetailId" runat="server" Text="" Visible="false"/>&nbsp;&nbsp;
                            <asp:DropDownList ID="ddlAssignTo" runat="server" AppendDataBoundItems="true" DataTextField="UserName"
                                    DataValueField="UserId" Width="180px" Height="22px" Visible="false" />&nbsp;&nbsp;
                           <asp:Button ID="btnSave" runat="server" Text="Assign" OnClick="btnSave_Click" Visible="false"  CssClass="add-button"/>
                        </td>
                       
                      </tr>
                 </table>
            </div>
        </div>
    </div>
<script type="text/javascript">
    function showModal() {
        document.getElementById('forgotPasswordModal').style.display = 'block';
    }
    function hideModal() {
        document.getElementById('forgotPasswordModal').style.display = 'none';
    }
</script>
</asp:Content>
