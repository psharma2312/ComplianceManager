<%@ Page Title="Change Password" Language="C#" MasterPageFile="~/ComplianceM.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="ComplianceManager.ChangePassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cntPlaceHolder1" runat="server">
<link rel="stylesheet" type="text/css" href="Content/common.css" />
<div>
    <div class="heading">
        <asp:Label ID="Label4" runat="server" Text="Change Password" />
    </div>
    <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" Visible="false"></asp:Label>
    <div class="form-group">
        <asp:Label ID="lblUsername" runat="server" Text="Username" AssociatedControlID="ddlUsername"  CssClass="control-label"  style="width:110px;"/>
        <asp:DropDownList ID="ddlUsername" runat="server" style="width: 150px;" Visible="false"></asp:DropDownList>
        <asp:TextBox ID="txtUserName" runat="server" Text="" style="width: 150px;"  Visible="true" Enabled="false"/>
    </div>
    <div class="form-group">
        <asp:Label ID="lblNewPassword" runat="server" Text="New Password" AssociatedControlID="txtNewPassword"  CssClass="control-label"  style="width:110px;"/>
        <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" style="width: 150px;" />
    </div>
    <div class="form-group">
        <asp:Label ID="lblConfirmPassword" runat="server" Text="Confirm Password" AssociatedControlID="txtConfirmPassword"  CssClass="control-label" style="width:110px;"/>
        <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" style="width: 150px;" />
    </div>
    <div class=".button-container">
        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="add-button" style="margin-left:0px;" />
    </div>
</div>
</asp:Content>