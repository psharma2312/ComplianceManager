<%@ Page Title="" Language="C#" MasterPageFile="~/ComplianceM.Master" AutoEventWireup="true" CodeBehind="DocumentAvailability.aspx.cs" Inherits="ComplianceManager.DocumentAvailability" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cntPlaceHolder1" runat="server">
<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
<link rel="stylesheet" type="text/css" href="Content/common.css" />
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<style type="text/css">
    .error-message {color: #721c24;background-color: #f8d7da;padding: 10px;border: 1px solid #f5c6cb;border-radius: 4px;margin-bottom: 15px;display: block;        }
   
    .compliance-grid {width: 100%;border-collapse: collapse;margin-top: 15px;font-family: Arial, sans-serif;}
    .compliance-grid th {background-color: #0066cc; color: white;padding: 3px;text-align: left;font-size:10px;border-bottom: 1px solid #004d99;}
    .compliance-grid td {padding: 0px;border-bottom: 0px solid #ddd;font-size:9px;}
    .compliance-grid tr:hover {background-color: #f5f5f5; /* Light gray background on hover for better interactivity */
    }
    .upload-button {background-color: #007bff;color: white;border: none;padding: 4px 4px;border-radius: 4px;cursor: pointer;}/* Green */
    .upload-button:hover {background-color: #0069d9;}
    .save-button {background-color: #28a745; color: white;border: none;padding: 4px 10px;border-radius: 4px;cursor: pointer;}/* Green */
    .save-button:hover {background-color: #218838;}
    .grid-button {padding: 5px 10px;border: none;border-radius: 4px;cursor: pointer;}
    
    
    .modal {display: none;position: fixed;top: 0;left: 0;width: 100%;height: 100%;background-color: rgba(0,0,0,0.5);z-index: 1000;}
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
    .modal-close:hover {color: #000;}
    .control-div {
        margin: 10px 0;
    }
    .label-names {
        display: inline-block;
        width: 120px;
        font-size: 12px;
        font-weight: bold;
    }


.details-table { width: 100%; border-collapse: collapse; margin-top: 10px; }
    .details-table td { padding: 5px; vertical-align: top; }
    .details-label { font-weight: bold; font-size: 12px; }
    .details-value { font-size: 12px; }
    </style>
    <div> 

<div class="heading">
    <asp:Label ID="Label4" runat="server" Text="Document Availability Report" />
</div>
<asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" Visible="false" />
        <div style="display: flex; gap: 0px; margin: 5px 0; width: 100%;">
    <!-- First Column (leftmost, Upload and Search) -->
    <div style="flex: 0 0 31%; min-width: 0;">
        <div class="control-div">
            <asp:Label ID="lblDepartmentFunction" runat="server" Text="Department" AssociatedControlID="ddlDepartmentFunction" CssClass="control-label" style="width: 120px;" />
            <asp:DropDownList ID="ddlDepartmentFunction" runat="server" style="width: 180px;height:20px; font-size:12px;"></asp:DropDownList>
        </div>
        <div class="control-div">
            <asp:Label ID="lblNatureOfCompliance" runat="server" Text="Compliance Nature" AssociatedControlID="ddlNatureOfCompliance" CssClass="control-label" style="width: 120px;" />
            <asp:DropDownList ID="ddlNatureOfCompliance" runat="server" style="width: 180px;height:20px; font-size:12px;"></asp:DropDownList>
        </div>

       
    </div>
    <!-- Second Column (middle, Export and Sample) -->
    <div style="flex: 0 0 31%; min-width: 0;">
        <div class="control-div">
             <asp:Label ID="lblTypeOfCompliance" runat="server" Text="Compliance Type" AssociatedControlID="ddlTypeOfCompliance" CssClass="control-label" style="width: 120px;" />
             <asp:DropDownList ID="ddlTypeOfCompliance" runat="server" style="width: 180px;height:20px; font-size:12px;"></asp:DropDownList>
         </div>
        <div  class="control-div">
             <asp:Label ID="lblPriority" runat="server" Text="Priority" AssociatedControlID="ddlPriority" CssClass="control-label" style="width: 120px;" />
             <asp:DropDownList ID="ddlPriority" runat="server" style="width: 180px;height:20px; font-size:12px;"></asp:DropDownList>
         </div>
        <div style="margin: 10px 0;"></div>
    </div>
        <!-- Third Column (rightmost, Search) -->
        <div style="flex: 0 0 33%; min-width: 0;">
            <div style="margin: 10px 0; height: 100%; display: flex; flex-direction: column; gap: 1px;">
                <div style="flex: 1;">
                    <div style="display: flex; gap: 10px;">
                                <div style="margin: 0px 0;">
                                    <asp:Button ID="btnLoad" runat="server" Text="Load Data" OnClick="btnLoad_Click" CssClass="save-button" 
    style="width: 80px;font-size:12px;" />
                                    
                                    <asp:Button ID="btnAll" runat="server" Text="Reset" OnClick="btnAll_Click" CssClass="upload-button" 
    style="width: 70px;margin-left:15px;font-size:12px;" />
                                    <asp:Button ID="btnExport" runat="server" Text="Export Report" OnClick="btnExport_Click" CssClass="save-button" 
    style="width: 100px;margin-left:15px;font-size:12px;" />
                                </div>
                       
                    </div>
                </div>
                <!-- Placeholder to fill height -->
                <div style="flex: 1;"></div>
            </div>
        </div>
    </div>
<div style="text-align: left; margin-top: 0px;"></div>
<asp:GridView ID="gvDocuments" runat="server" AutoGenerateColumns="false" BackColor="White" BorderColor="#E7E7FF" BorderStyle="None"
    BorderWidth="1px" CellPadding="5" GridLines="Both" Font-Size="Smaller" HeaderStyle-BackColor="#337ab7" HeaderStyle-ForeColor="White" 
    DataKeyNames="ComplianceID, DocumentID" CssClass="frequency-grid" OnPageIndexChanging="gvDocuments_PageIndexChanging" PageSize="15"
    AlternatingRowStyle-BackColor="#f2f2f2" ShowHeaderWhenEmpty="true" EmptyDataText="No Documents available." >
    <Columns>
        <asp:TemplateField HeaderText="SNo" ItemStyle-Width="20px" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Label ID="lblSNo" runat="server" Text='<%# Container.DataItemIndex + 1 %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="ComplianceID" HeaderText="CompianceId" ItemStyle-Width="30px" HeaderStyle-HorizontalAlign="Center" />
        <asp:BoundField DataField="ComplianceRef" HeaderText="Compiance Ref" ItemStyle-Width="30px" HeaderStyle-HorizontalAlign="Center" />
        <asp:BoundField DataField="ComplianceType" HeaderText="ComplianceType" ItemStyle-Width="100px" HeaderStyle-HorizontalAlign="Center" />
        <asp:BoundField DataField="ComplianceNature" HeaderText="Compliance Nature" ItemStyle-Width="120px" HeaderStyle-HorizontalAlign="Center" />
        <asp:BoundField DataField="Priority" HeaderText="Priority" ItemStyle-Width="40px" HeaderStyle-HorizontalAlign="Center" />
        <asp:BoundField DataField="Department" HeaderText="Department" ItemStyle-Width="80px" HeaderStyle-HorizontalAlign="Center" />
        <%--<asp:BoundField DataField="DocumentID" HeaderText="ID" ItemStyle-Width="40px" HeaderStyle-HorizontalAlign="Center" />--%>
        <asp:BoundField DataField="DocTypeName" HeaderText="Document Type" ItemStyle-Width="150px" HeaderStyle-HorizontalAlign="Center" />
        <asp:BoundField DataField="FileName" HeaderText="File Name" ItemStyle-Width="170px" ItemStyle-Font-Bold="true" ItemStyle-Font-Size="X-Small" ItemStyle-HorizontalAlign="Left"/>
        <%--<asp:BoundField DataField="UploadedDate" HeaderText="UploadedOn" ItemStyle-Width="120px" ItemStyle-Font-Bold="true" ItemStyle-Font-Size="X-Small" ItemStyle-HorizontalAlign="Left"/>--%>
       <asp:TemplateField HeaderText="UploadedOn" ItemStyle-Width="70px">
    <ItemTemplate>
        <asp:Label ID="lblUploadedDate" runat="server" Text='<%# Eval("UploadedDate", "{0:dd-MM-yyyy}") %>' />
    </ItemTemplate>
  
</asp:TemplateField>
    </Columns>
    <EmptyDataTemplate>
        <div align="center">No documents found.</div>
    </EmptyDataTemplate>
</asp:GridView>


</div>

</asp:Content>
