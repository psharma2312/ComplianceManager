<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyTeam.aspx.cs" Inherits="ComplianceManager.MyTeam" %>
<asp:Content ID="Content1" ContentPlaceHolderID="mainContent" runat="server">
          <div id="inner_page">
<div id="inner_page_main">
    <div class="heading">
            <asp:Label ID="Label4" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="My Compliances" />
</div>
     <div><asp:Label ID="labelError" runat="server" Text="" BackColor="Yellow" /></div>
<div id="blankspace"></div>
<div id="inner_page_top">
    <table with="100%">

<tr>
            <td colspan="2" style="width: 20%; text-align: right; height: 30px;">
                <asp:Label ID="lblFilter" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Smaller" ForeColor="Navy" Text="Filter By Supervisee :" />
             </td>
            <td colspan="18">
                <asp:DropDownList ID="ddlStatus" runat="server" DataTextField="UserName" DataValueField="UserId" Width="180px" Height="22px"
                    AutoPostBack="true" />
                <asp:Button ID="btnLoad" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Smaller"
                              Height="22px" style="text-align:center;" Text="Load" OnClick="btnLoad_Click"  ForeColor="Navy"/></td>
        </tr>
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
                <asp:GridView ID="GridView1" CssClass="gridview" runat="server" Width="100%" AutoGenerateColumns="False"
                    GridLines="Both" 
                    HeaderStyle-BackColor="#337ab7" HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#f2f2f2" 
                    DataKeyNames="ComplianceID,ComplianceDetailID" OnSelectedIndexChanged="OnSelectedIndexChanged" OnRowDataBound="GridView1_RowDataBound">
                    <AlternatingRowStyle BackColor="#DCDCDC" />
                    <Columns>
                        <asp:BoundField DataField="ComplianceArea" HeaderText="Compliance Area" ItemStyle-Width="160"  />
                        <asp:BoundField DataField="ComplianceTypeName" HeaderText="Compliance Type" ItemStyle-Width="160"/>
                        <asp:BoundField DataField="GovernmentLegislation" HeaderText="Governing Legislation" ItemStyle-Width="160" />
                        <asp:BoundField DataField="ActSectionReference" HeaderText="Act/ Section" ItemStyle-Width="145"  />
                        <asp:BoundField DataField="NatureOfComplianceName" HeaderText="Compliance Nature" ItemStyle-Width="145"  />
                        <asp:BoundField DataField="EffectiveFrom" HeaderText="EffectiveFrom" ItemStyle-Width="90" DataFormatString="{0:dd-MMM-yyyy}"  />
                        <asp:BoundField DataField="StandardDueDate" HeaderText="Due Date" ItemStyle-Width="85" DataFormatString="{0:dd-MMM-yyyy}"  />
                        <asp:BoundField DataField="ComplianceStatusName" HeaderText="Status" ItemStyle-Width="80"  />
                        <asp:BoundField DataField="AssignedToName" HeaderText="AssignedTo" ItemStyle-Width="100"  />
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
            </td>
        </tr>
        <tr>
            <td colspan="20">
                <table class="style1" width="100%">
                    <tr><td colspan="20" style="border-bottom: thin solid #008080;">
                        <asp:Label ID="Label3" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Compliance Details" />

                        </td></tr>
                 
                    <tr>
                        <td colspan="10" style="text-align: right; height: 30px;">
                            <asp:Label ID="label1" runat="server" Text=" Compliance Area :" BackColor="Yellow" Visible="false" />
                            <asp:TextBox ID="txtDetailsComplianceID" runat="server" Width="180px" Height="25px" Visible="false" />
                            <asp:Label ID="label2" runat="server" Text="Compliance Details:" BackColor="Yellow" Visible="false" />
                            <asp:TextBox ID="txtDetailsComplianceDetailID" runat="server" Width="180px" Height="25px" Visible="false" />
                            <asp:TextBox ID="txtDetailsComplArea" runat="server" Width="180px" Height="25px" Visible="false" />
                            <asp:TextBox ID="txtDetailsDepartmentName" runat="server" Visible="false" />
                            </td>
                         <td colspan="1" style="width: 1%; text-align: right; height: 30px;">&nbsp; </td>
                        <td colspan="9" style="text-align:center; height: 30px;">
                            <asp:Label ID="lblUploadedDocuments" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Small" ForeColor="Navy" Text="Uploaded Documents" Visible="false" />
                           </td>
                    </tr>
                   
                    <tr>
                        <!--Commenting the Textboxes as information is already in Grid above, if needed we can uncomment to show data-->
                        <%--<td style="width: 16%; text-align: right; height: 30px;" colspan="2">Compliance Area :</td>
                        <td style="width: 14%;" colspan="2">
                            <asp:TextBox ID="txtDetailsComplArea" runat="server" Width="180px" Height="25px" Enabled="false" /></td>
                        <td style="width: 2%;" colspan="1">&nbsp;</td>
                        <td style="width: 14%; text-align: right; height: 30px;" colspan="2">Type of Compliance :</td>
                        <td style="width: 14%;" colspan="2">
                            <asp:TextBox ID="txtDetailsComplianceType" runat="server" Width="180px" Height="25px" Enabled="false" /></td>
                        <td style="width: 2%;" colspan="1">&nbsp;</td>--%>
                        
                         <td style="width: 15%; text-align: right;" colspan="2">
                             <asp:Label ID="lblCompReqDetails" runat="server" Text="Details of Compliance Requirements :" Font-Bold="True" Font-Names="Verdana" Font-Size="Smaller" 
                                 ForeColor="Navy" Visible="false" />
                         </td>
                        <td colspan="8" style="vertical-align: bottom;">
                            <asp:TextBox ID="txtDetails" TextMode="multiline" Columns="75" BackColor="#dddddd" Rows="4" runat="server" ReadOnly="true" Visible="false" /></td>

                        <td colspan="1" style="width: 1%;text-align: right; height: 30px;">&nbsp; </td>
                        
                        <td style="vertical-align: top; width: 38%; text-align: center; padding:0px 5px 5px 1px" colspan="9" rowspan="3">
                            
                            <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="false" EmptyDataText="No files uploaded"
                                Width="90%" BackColor="White" BorderColor="#E7E7FF" BorderStyle="None"
                                BorderWidth="1px" CellPadding="5" GridLines="Both" Font-Size="Smaller" HeaderStyle-BackColor="#337ab7"
                                HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#f2f2f2" ShowHeaderWhenEmpty="true">
                                <Columns>
                                    <asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="40px" HeaderStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="FileName" HeaderText="File Name" ItemStyle-Font-Bold="true" ItemStyle-Font-Size="Smaller" ItemStyle-HorizontalAlign="Left" ItemStyle-Font-Names="Verdana"/>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDownload" Text="Download" CommandArgument='<%# Eval("FileName") %>' runat="server" OnClick="DownloadFile"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDelete" Text="Delete" CommandArgument='<%# Eval("FileName") %>' runat="server" OnClick="DeleteFile" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div align="center">No documents found.</div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </td>
                    </tr>
                    <!--Commenting the Textboxes as information is already in Grid above, if needed we can uncomment to show data-->
                    <%--<tr>
                        <td style="width: 16%; text-align: right; height: 30px;" colspan="2">Governing Legislation :</td>
                        <td style="width: 14%;" colspan="2">
                            <asp:TextBox ID="txtDetailsGovLegislation" runat="server" Width="180px" Height="25px" Enabled="false" /></td>
                        <td style="width: 2%;" colspan="1">&nbsp;</td>
                        <td style="width: 14%; text-align: right; height: 30px;" colspan="2">&nbsp;Act/ Section Ref. :</td>
                        <td style="width: 14%;" colspan="2">
                            <asp:TextBox ID="txtDetailsActSection" runat="server" Width="180px" Height="25px" Enabled="false" /></td>
                        <td style="width: 2%;" colspan="1">&nbsp;</td>
                        <td style="width: 38%; text-align: right; height: 30px;" colspan="11">&nbsp;</td>

                    </tr>
                    <tr>

                        <td style="width: 16%; text-align: right; height: 30px;" colspan="2">Nature of Compliance :</td>
                        <td style="width: 14%;" colspan="2">
                            <asp:TextBox ID="txtDetailsNatureOfCompliance" runat="server" Enabled="false" />
                        <td style="width: 2%;" colspan="1">&nbsp;</td>
                        <td style="width: 14%; text-align: right; height: 30px;" colspan="2">Effective from Date : </td>
                        <td style="width: 14%;" colspan="2">
                            <asp:TextBox ID="txtDetailsEffectiveDate" runat="server" Enabled="false" /></td>
                        <td style="width: 2%;" colspan="1">&nbsp;</td>
                        <td style="width: 38%; text-align: right; height: 30px;" colspan="11">&nbsp;</td>
                    </tr>
                    <tr>
                        <td style="width: 16%; text-align: right; height: 30px;" colspan="2">Frequency :</td>
                        <td style="width: 14%;" colspan="2">
                            <asp:TextBox ID="txtDetailsFrequencyName" runat="server" Enabled="false" /></td>
                        <td style="width: 2%;" colspan="1">&nbsp;</td>
                        <td style="width: 14%; text-align: right; height: 30px;" colspan="2">Standard Due Date :</td>
                        <td style="width: 14%;" colspan="2">
                            <asp:TextBox ID="txtDetailsStandardDate" runat="server" Enabled="false" /></td>
                        <td style="width: 2%;" colspan="1">&nbsp;</td>
                        <td style="width: 38%; text-align: right; height: 30px;" colspan="11">&nbsp;</td>
                    </tr>
                    <tr>
                        <td style="width: 16%; text-align: right; height: 30px;" colspan="2">Department / Function :</td>
                        <td style="width: 14%;" colspan="2">
                            <asp:TextBox ID="txtDetailsDepartmentName" runat="server" Enabled="false" />
                        <td style="width: 2%;" colspan="1">&nbsp;</td>
                        <td style="width: 14%; text-align: right; height: 30px;" colspan="2">&nbsp;</td>
                        <td style="width: 14%;" colspan="2">&nbsp;</td>
                        <td style="width: 2%;" colspan="1">&nbsp;</td>
                        <td style="width: 38%; text-align: right; height: 30px;" colspan="11">&nbsp;</td>
                    </tr>--%>

                   
                    <tr>
                        <td style="width: 15%; text-align: right;" colspan="2">
                            <asp:Label ID="lblFileUpload" runat="server" Text="File Upload :" Font-Bold="True" Font-Names="Verdana" Font-Size="Smaller" 
                                 ForeColor="Navy" Visible="false" />
                            </td>
                        <td style="width: 180px;" colspan="2">
                            <asp:FileUpload ID="FileUpload1" runat="server" multiple="multiple" Visible="false"/>
                        </td>
                        <td colspan="1">&nbsp;</td>
                        <td style="width: 180px;" colspan="2">
                            <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click" Height="22px"
                                Font-Bold="True" Font-Names="Verdana" Font-Size="Smaller" ForeColor="Navy" Visible="false" Enabled="false"/>
                        </td>
                        <td style="width: 1%;text-align: right; align-content: flex-start; height: 30px;" colspan="13">&nbsp;</td>
                    </tr>
                    <tr>
                        <td style="width: 15%; text-align: right;" colspan="2">
                            <asp:Label ID="lblUpdatedComments" runat="server" Text="Comments History:" Font-Bold="True" Font-Names="Verdana" Font-Size="Smaller" 
                                 ForeColor="Navy" Visible="false" /></td>
                        <td colspan="8" style="vertical-align: bottom;">
                            <asp:TextBox ID="txtUpdatedComments" TextMode="multiline" Columns="75" Rows="4" BackColor="#dddddd" ReadOnly="true" runat="server" Visible="false" /></td>
                         <td colspan="1" style="text-align: right; height: 30px;">&nbsp; </td>
                       
                    </tr>
                    <tr>
                        
                         <td colspan="2" style="width: 15%; text-align: right;" height: "30px;">
                             <asp:Label ID="lblNewUserComments" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Smaller" 
                                 ForeColor="Navy" Text="New Comments:" Visible="false" /> </td>
                       <td style="vertical-align: top; text-align: center;"  colspan="8">
                            <asp:TextBox ID="txtNewComments" runat="server" Width="100%" Visible="false" Enabled="false" /></td>
                        <td colspan="1" style="text-align: right; height: 30px;">&nbsp; </td>
                        
                    </tr>
                    <tr> <td colspan="5" style="width: 380px; text-align: right; height: 30px;"> 
                            <asp:Button ID="btnupdate" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Smaller" ForeColor="Navy"
                                Text="Update Comments" OnClick="btnupdate_Click"   Height="22px"
                                 Visible="false" Enabled="false" /></td>
                        <td colspan="5" style="width: 380px; text-align: right; height: 30px;"> 
                            <asp:Button ID="btnSubmitForApproval" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="Smaller" ForeColor="Navy"
                                Text="Submit for Approval" OnClick="btnSubmitForApproval_Click"   Height="22px"
                                 Visible="false" Enabled="false" /></td>
                    </tr>
                </table>
            </td>
        </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
