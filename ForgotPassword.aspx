<%@ Page Title="Forgot Password" Language="C#" MasterPageFile="~/Home.Master" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="ComplianceManager.ForgotPassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cntPlaceHolder1" runat="server">
    <div class="login-box">
        <div class="logo-container">
            <asp:Image ID="imgLoginLogo" runat="server" ImageUrl="~/Images/login-logo.png" AlternateText="Login Logo" CssClass="login-logo" />
        </div>
        <h2>Forgot Password</h2>
        <div class="form-group">
            <asp:Label ID="lblUsername" runat="server" Text="Username" AssociatedControlID="txtUsername"></asp:Label>
            <asp:TextBox ID="txtUsername" runat="server"></asp:TextBox>
        </div>
        <div class="button-container">
            <asp:Button ID="btnRetrieve" runat="server" Text="Retrieve Password" OnClick="btnRetrieve_Click" CssClass="retrieve-button" />
            <asp:Button ID="btnBackToLogin" runat="server" Text="Back to Login" OnClick="btnBackToLogin_Click" CssClass="back-button" />
        </div>
        <div style="margin-top: 15px;">
            <asp:Label ID="lblResult" runat="server" Visible="false"></asp:Label>
        </div>
    </div>

    <style type="text/css">
        .logo-container {
            text-align: center;
            margin-bottom: 20px;
        }
        .login-logo {
            max-width: 150px;
            height: auto;
        }
        .button-container {
            display: flex;
            justify-content: space-between;
            margin-top: 20px;
        }
        .retrieve-button {
            background-color: #0066cc; /* Blue */
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 4px;
            cursor: pointer;
            width: 48%;
        }
        .retrieve-button:hover {
            background-color: #0052a3;
        }
        .back-button {
            background-color: #6c757d; /* Gray */
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 4px;
            cursor: pointer;
            width: 48%;
        }
        .back-button:hover {
            background-color: #5a6268;
        }
    </style>
</asp:Content>
