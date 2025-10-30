<%@ Page Title="" Language="C#" MasterPageFile="~/ComplianceM.Master" AutoEventWireup="true" CodeBehind="MyCompliances.aspx.cs" Inherits="ComplianceManager.MyCompliances" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cntPlaceHolder1" runat="server">
<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
<link rel="stylesheet" type="text/css" href="Content/myCompliance.css" />
<script type="text/javascript">
    function openModal() {
        $('[id*=myModal]').modal('show');
    }
</script>
        <style type="text/css">
        .error-message {
            color: #721c24;
            background-color: #f8d7da;
            padding: 10px;
            border: 1px solid #f5c6cb;
            border-radius: 4px;
            margin-bottom: 15px;
            display: block;
        }

        .compliance-grid {
            width: 100%;
            border-collapse: collapse;
            margin-top: 15px;
            font-family: Arial, sans-serif;
        }

            .compliance-grid th {
                background-color: #0066cc;
                color: white;
                padding: 3px;
                text-align: left;
                font-size: 10px;
                border-bottom: 1px solid #004d99;
            }

            .compliance-grid td {
                padding: 0px;
                border-bottom: 0px solid #ddd;
                font-size: 9px;
            }

            .compliance-grid tr:hover {
                background-color: #f5f5f5; /* Light gray background on hover for better interactivity */
            }

        .upload-button {
            background-color: #007bff;
            color: white;
            border: none;
            padding: 4px 4px;
            border-radius: 4px;
            cursor: pointer;
        }
            /* Green */
            .upload-button:hover {
                background-color: #0069d9;
            }

        .save-button {
            background-color: #28a745;
            color: white;
            border: none;
            padding: 4px 4px;
            border-radius: 4px;
            cursor: pointer;
        }
            /* Green */
            .save-button:hover {
                background-color: #218838;
            }

        .grid-button {
            padding: 5px 10px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
        }


        .modal {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0,0,0,0.5);
            z-index: 1000;
        }

        .modal-content {
            background-color: #fff;
            margin: 15% auto;
            padding: 8px;
            border-radius: 5px;
            width: 80%;
            max-width: 600px;
            text-align: left;
            font-size: 14px;
            position: relative;
            min-height: 200px;
            box-shadow: 0 4px 8px rgba(0,0,0,0.1); /* Subtle shadow for depth */
        }

        .modal-close {
            position: absolute;
            top: 10px;
            right: 15px;
            font-size: 18px;
            cursor: pointer;
            color: #333;
        }

        .modal-header {
            min-height: 16.43px;
            padding: 10px;
            border-bottom: 2px solid #e5e5e5;
        }

        .modal-title {
            margin: 0;
            line-height: 1.42857143;
            color: #0066cc;
            font-family: Verdana;
            font-size: 14px;
            font-weight: bold;
        }

        .modal-body {
            position: relative;
            padding: 15px;
        }

        .modal-close:hover {
            color: #000;
        }

        .control-div {
            margin: 0px 0;
        }

        .label-names {
            display: inline-block;
            width: 120px;
            font-size: 12px;
            font-weight: bold;
        }

        .details-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 10px;
        }

            .details-table td {
                padding: 5px;
                vertical-align: top;
            }

        .details-label {
            font-weight: bold;
            font-size: 12px;
        }

        .details-value {
            font-size: 12px;
        }
    </style>
<div id="inner_page">
    <div id="inner_page_main">
        <div class="heading">
            <asp:Label ID="Label4" runat="server" Text="My Compliances" />
        </div>
            <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" Visible="false"  />
            <div style="display: flex; gap: 0px; margin: 5px 0; width: 100%;">
            <!-- First Column (leftmost, Upload and Search) -->
            <div style="flex: 0 0 30%; min-width: 0;">
                <div class="control-div">
                    <asp:Label ID="lblCompStatus" runat="server" Text="Compliance Status" AssociatedControlID="ddlStatus" CssClass="control-label" Style="width: 120px;" />
                    <asp:DropDownList ID="ddlStatus" runat="server" DataTextField="ComplianceStatusName" DataValueField="ComplianceStatusId" Width="180px" Height="22px" AutoPostBack="true" />
                </div>

                
            </div>
            <!-- Second Column (middle, Export and Sample) -->
            <div style="flex: 0 0 14%; min-width: 0;">
                <div class="control-div">
                    <asp:Label ID="lblMonth" runat="server" Text="Month" AssociatedControlID="ddlMonth" CssClass="control-label" Style="width: 50px;" />
                    <asp:DropDownList ID="ddlMonth" runat="server" Style="width: 70px; height: 20px; font-size: 12px;"></asp:DropDownList>
                </div>
                
            </div>
            <!-- Third Column (rightmost, Search) -->
            <div style="flex: 0 0 33%; min-width: 0;">
                <div class="control-div">
                    <asp:Label ID="lblYear" runat="server" Text="Year" AssociatedControlID="ddlYear" CssClass="control-label" Style="width: 50px;" />
                    <asp:DropDownList ID="ddlYear" runat="server" Style="margin-right:10px; width: 70px; height: 20px; font-size: 12px;"></asp:DropDownList>
                        <asp:Button ID="btnLoad" runat="server" Text="Load" OnClick="btnLoad_Click" CssClass="save-button"
                            Style="width: 60px; font-size: 12px;" />
                    <!-- Placeholder to fill height -->
                    <div style="flex: 1;"></div>
                </div>
            </div>
        </div>
        <div style="text-align: left; margin-top: 0px;"></div>

      <%--  <div class="status-message" style="height: 20px;">
            <asp:Label ID="lblStatus" runat="server" Text="" Visible="false" BackColor="Yellow" />
        </div>--%>

        <div id="inner_page_bottom">
            <div class="grid-container">
                <asp:GridView ID="GridView1" CssClass="frequency-grid" runat="server" Width="100%" AutoGenerateColumns="False" GridLines="Both"  
                    DataKeyNames="ComplianceID,ComplianceDetailID"  BorderColor="#E7E7FF"
                    OnSelectedIndexChanged="OnSelectedIndexChanged" OnRowDataBound="GridView1_RowDataBound" OnPageIndexChanging="OnPageIndexChanging" 
                    PageSize="4" AllowPaging="true" EmptyDataText="No Compliances assigned." ShowHeaderWhenEmpty="true"  BorderStyle="None"  BackColor="White">
                    <Columns>
                    <asp:BoundField DataField="ComplianceDetailID" HeaderText="Id" ItemStyle-Width="30" HeaderStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="ComplianceRef" HeaderText="Compliance Ref" ItemStyle-Width="70" />
                        <asp:BoundField DataField="ComplianceTypeName" HeaderText="Type" ItemStyle-Width="100" />
                        <asp:BoundField DataField="ActSectionReference" HeaderText="Act/Section" ItemStyle-Width="360" />
                        <asp:BoundField DataField="EffectiveFrom" HeaderText="EffectiveFrom" ItemStyle-Width="85" DataFormatString="{0:dd-MMM-yyyy}" />
                        <asp:BoundField DataField="StandardDueDate" HeaderText="Due Date" ItemStyle-Width="90" DataFormatString="{0:dd-MMM-yyyy}" />
                        <asp:BoundField DataField="ComplianceStatusName" HeaderText="Status" ItemStyle-Width="80" />
                        <asp:BoundField DataField="AssignedDate" HeaderText="AssignedOn" ItemStyle-Width="90"  DataFormatString="{0:dd-MMM-yyyy}" />
                        <asp:BoundField DataField="PriorityName" HeaderText="Priority" ItemStyle-Width="50" />
                        <asp:BoundField DataField="ApprovedByName" HeaderText="Appprover" ItemStyle-Width="120" />
                         <asp:TemplateField HeaderText="Details" ItemStyle-Width="240px">
                             <ItemTemplate>
                                 <span><%# GetTruncatedDetails(Eval("DetailsOfComplianceRequirements")) %></span>
                                 <asp:LinkButton ID="lnkBtnEdit" runat="server" Text="..." style="padding:0px 4px;font-size:8px;color: #fff;
                                         background-color: #5bc0de;vertical-align: middle; font-weight: 400;border: 1px solid transparent; 
                                         border-color: #46b8da;border-radius: 4px;" OnClick="Display"></asp:LinkButton>
                             </ItemTemplate>
                         </asp:TemplateField>
                        <asp:CommandField ShowSelectButton="True" ItemStyle-Font-Bold="true" ItemStyle-HorizontalAlign="Center" />
                    </Columns>
                    <RowStyle BackColor="#FFFFFF" /> 
                    <AlternatingRowStyle BackColor="#F0F0F0" /> 
                    <SelectedRowStyle BackColor="#FFFF00" /> 
                    <PagerStyle CssClass="pager-style" />
                    <EmptyDataTemplate>
                        <div align="center">No Compliances assigned.</div>
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
                                                <asp:Label ID="lblComplianceAreaLabel" runat="server" Text="Compliance Nature:" CssClass="details-label" />
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
                    <asp:Label width="100%" ID="lblDocUpload" runat="server" style="background-color: #f1f8ff;padding: 3px;border-bottom: 2px solid #0066cc; margin-bottom: 0px;" 
                        Font-Bold="True" Font-Size="Large" Text="Compliance Details" Visible="false" />

                    </td>
                </tr>
                 
                <tr>
                    <td colspan="10" style="text-align: right; height: 5px;">
                        <asp:Label ID="label1" runat="server" Text=" Compliance Area :" BackColor="Yellow" Visible="false" />
                        <asp:TextBox ID="txtDetailsComplianceID" runat="server" Width="180px" Height="5px" Visible="false" />
                        <asp:Label ID="label2" runat="server" Text="Compliance Details:" BackColor="Yellow" Visible="false" />
                        <asp:TextBox ID="txtDetailsComplianceDetailID" runat="server" Width="180px" Height="5px" Visible="false" />
                        <asp:TextBox ID="txtComplianceRef" runat="server" Width="20px" Height="5px" Visible="false" />
                        </td>
                    <td colspan="1" style="width: 1%; text-align: right; height: 5px;">&nbsp; </td>
                    
                </tr>
                <tr>
                    <td style="width: 11%; text-align: right;" colspan="2">
                        <asp:Label ID="lblUpdatedComments" runat="server" Text="Preparer Comments"  CssClass="control-label" Visible="false" /></td>
                    <td colspan="8" style="vertical-align: bottom;">
                        <asp:TextBox ID="txtUpdatedComments" TextMode="multiline" Columns="70" Rows="3" BackColor="#dddddd" ReadOnly="true" runat="server" Visible="false" /></td>
                    <td colspan="1" style="text-align: right; height: 30px;">&nbsp; </td>
                    <td style="vertical-align: top; width: 38%; text-align: left; padding:0px 5px 5px 1px" colspan="9" rowspan="5">
                        <table width="100%">
                            <tr><td>
                                <asp:Label ID="lblUploadedDocuments" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="X-Small" ForeColor="Navy" Text="Uploaded Master Documents" Visible="false" />
                                <asp:GridView ID="GridView3" runat="server"  CssClass="frequency-grid" AutoGenerateColumns="false" EmptyDataText="No files uploaded"
                                    Width="90%" BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" DataKeyNames="ComplianceID, DocumentID"
                                    BorderWidth="1px" CellPadding="5" GridLines="Both" Font-Size="Smaller" HeaderStyle-BackColor="#337ab7"
                                    style="margin-top:0px;" HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#f2f2f2" ShowHeaderWhenEmpty="true">
                                     <Columns>
                                         <asp:BoundField DataField="DocumentID" HeaderText="ID" ItemStyle-Width="40px" HeaderStyle-HorizontalAlign="Center" />
                                         <asp:BoundField DataField="DocTypeName" HeaderText="Type" ItemStyle-Width="300px" HeaderStyle-HorizontalAlign="Center" />
                                         <asp:BoundField DataField="FileName" HeaderText="File Name" ItemStyle-Width="350px" ItemStyle-Font-Bold="true" ItemStyle-Font-Size="X-Small" ItemStyle-HorizontalAlign="Left"/>
                                         <asp:BoundField DataField="UploadedDate" HeaderText="UploadedOn" ItemStyle-Width="150px" ItemStyle-Font-Bold="true" ItemStyle-Font-Size="X-Small" ItemStyle-HorizontalAlign="Left"/>
                                         <asp:TemplateField HeaderText="Action"  ItemStyle-Width="60px" HeaderStyle-HorizontalAlign="Center">
                                             <ItemTemplate>
                                                  <asp:LinkButton ID="lnkDownload" Text="Download" CommandArgument='<%# Eval("DownloadPath") %>' 
                                                          OnClientClick='<%# "window.location.href = \"FileDownloadHandler.ashx?fileName=" + Eval("DownloadPath") + "\"; return false;" %>' 
                                                          runat="server" />
                                             </ItemTemplate>
                                         </asp:TemplateField>
                                        
                                     </Columns>
                                    <EmptyDataTemplate>
                                        <div align="center">No documents for compliance master found.</div>
                                    </EmptyDataTemplate>
                                </asp:GridView>

                                </td></tr>
                            <tr><td>
                                <asp:Label ID="lblUploadedDocuments1" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="X-Small" ForeColor="Navy" Text="Uploaded User Documents" Visible="false" />
                                <asp:GridView ID="GridView2" runat="server"  CssClass="frequency-grid" AutoGenerateColumns="false" EmptyDataText="No files uploaded"
                                    Width="90%" BackColor="White" BorderColor="#E7E7FF" BorderStyle="None"
                                    BorderWidth="1px" CellPadding="5" GridLines="Both" Font-Size="Smaller" HeaderStyle-BackColor="#337ab7"
                                    style="margin-top:0px;" HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#f2f2f2" ShowHeaderWhenEmpty="true">
                                    <Columns>
                                        <asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="40px" HeaderStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="ComplianceId" HeaderText="Compliance ID" />
                                        <asp:BoundField DataField="FileName" HeaderText="File Name" ItemStyle-Font-Bold="true" ItemStyle-Font-Size="Smaller" ItemStyle-HorizontalAlign="Left" ItemStyle-Font-Names="Verdana"/>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkDownload" Text="Download" CommandArgument='<%# Eval("DownloadPath") %>' 
                                                        OnClientClick='<%# "window.location.href = \"FileDownloadHandler.ashx?fileName=" + Eval("DownloadPath") + "\"; return false;" %>' 
                                                        runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                    <asp:LinkButton ID="lnkDelete" runat="server" Text="Delete" 
                                                        OnClientClick='<%# "deleteFile(\"" + Eval("DownloadPath") + "\", \"" + Eval("ComplianceId") + "\", \"" + Eval("ID") + "\"); return false;" %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <div align="center">No documents found.</div>
                                    </EmptyDataTemplate>
                                </asp:GridView>
                                </td></tr>
                        </table>
                       
                    </td>
                </tr>
                <tr>
                    <td style="width: 11%; text-align: right;" colspan="2">
                        <asp:Label ID="lblApproverComments" runat="server" Text="Approver Comments"  CssClass="control-label" Visible="false" /></td>
                    <td colspan="8" style="vertical-align: bottom;">
                        <asp:TextBox ID="txtApproverComments" TextMode="multiline" Columns="70" Rows="3" BackColor="#dddddd" ReadOnly="true" runat="server" Visible="false" /></td>
                        <td colspan="1" style="text-align: right; height: 30px;">&nbsp; </td>
   
                </tr>
                <tr>
                    <td style="width: 11%; text-align: right;" colspan="2">
                        <asp:Label ID="lblFileUpload" runat="server" Text="File Upload"  CssClass="control-label" Visible="false" />
                    </td>
                    <td style="width: 180px;" colspan="2">
                        <asp:FileUpload ID="FileUpload1" runat="server" multiple="multiple" Visible="false"/>
                    </td>
                    <td colspan="1">&nbsp;</td>
                    <td style="width: 300px;" colspan="2">
                        <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click" CssClass="load-button" Visible="false" />
                    </td>
                    <td style="width: 1%;text-align: right; align-content: flex-start; height: 30px;" colspan="13">&nbsp;</td>
                    </tr>
                <tr>
                    <td colspan="2" style="width: 11%; text-align: right;" height: "30px;">
                        <asp:Label ID="lblNewUserComments" runat="server"  CssClass="control-label" Text="New Comments:" Visible="false" /> </td>
                    <td  colspan="8">
                        <asp:TextBox ID="txtNewComments" runat="server" Width="530px" Visible="false" /></td>
                    <td colspan="1" >&nbsp; </td>
                </tr>
                <tr> 
                    <td colspan="2" style="width: 11%; text-align: right;" height: "30px;"></td>
                    <td colspan="9" style="width: 600px; text-align: left; height: 30px;"> 
                        <asp:Button ID="btnupdate" runat="server"  CssClass="add-button"
                            Text="Update Comments" OnClick="btnupdate_Click"   Height="22px" Visible="false" /> &nbsp;&nbsp;&nbsp;&nbsp;  
                        <asp:Button ID="btnSubmitForApproval" runat="server"  CssClass="add-button"
                            Text="Submit for Approval" OnClick="btnSubmitForApproval_Click"   Height="22px" Visible="false" />
                        <asp:TextBox ID="txtEmail" runat="server" Visible="false" />
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
    function deleteFile(fileName, complianceId, complianceDetailId) {
        $.ajax({
            type: "GET",
            //url: "FileDeleteHandler.ashx?fileName=" + encodeURIComponent(fileName) + "&complianceId=" + complianceId,
            url: "FileDeleteHandler.ashx?fileName=" + encodeURIComponent(fileName) + "&complianceId=" + complianceId + "&documentID=-1",
            success: function (response) {
                if (response === "Success") {
                    reloadGrid(complianceDetailId, complianceId); // Pass parameters to reload
                } else {
                    alert("Failed to delete the file.");
                }
            },
            error: function (xhr, status, error) {
                alert("An error occurred while deleting the file.");
            }
        });
    }
    function reloadGrid(complianceDetailId, complianceId) {
        if (typeof updatePanelClientId !== 'undefined') {
            __doPostBack(updatePanelClientId, 'Reload|' + complianceDetailId + '|' + complianceId);
        } else {
            console.error('UpdatePanel client ID is not defined.');
        }
    }
</script>
</asp:Content>