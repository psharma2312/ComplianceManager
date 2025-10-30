<%@ Page Title="" Language="C#" MasterPageFile="~/Home.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ComplianceManager.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cntPlaceHolder1" runat="server">
    <style type="text/css">
        .logo-container {
            text-align: center;
            margin-bottom: 20px;
        }
        .login-logo {
            max-width: 100px; /* Adjust size as needed */
            height: auto;
        }
        .button-container {
            display: flex;
            justify-content: space-between;
            margin-top: 20px;
        }
        .login-button {
            background-color: #0066cc;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 4px;
            cursor: pointer;
            width: 48%; /* Slightly less than half to account for spacing */
        }
        .login-button:hover {
            background-color: #0052a3;
        }
        .forgot-button {
            background-color: #6c757d; /* Gray */
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 4px;
            cursor: pointer;
            width: 48%; /* Slightly less than half to account for spacing */
        }
        .forgot-button:hover {
            background-color: #5a6268;
        }
        .twofa-container {
            margin-top: 20px;
            display: none; /* Controlled server-side */
        }
        .twofa-code {
            width: 100%;
            padding: 10px;
            margin-top: 5px;
            border: 1px solid #ccc;
            border-radius: 4px;
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
            padding: 20px;
            border-radius: 5px;
            width: 80%;
            max-width: 400px;
            text-align: center;
            position: relative;
        }
        .modal-close {
            position: absolute;
            top: 10px;
            right: 10px;
            font-size: 18px;
            cursor: pointer;
            color: #333;
        }
        .modal-close:hover {
            color: #000;
        }
    </style>
    <script type="text/javascript">
        function showModal() {
            document.getElementById('forgotPasswordModal').style.display = 'block';
        }
        function hideModal() {
            document.getElementById('forgotPasswordModal').style.display = 'none';
        }
        function showwrongModal() {
            document.getElementById('wrongPasswordModal').style.display = 'block';
        }
        function hidewrongModal() {
            document.getElementById('wrongPasswordModal').style.display = 'none';
        }
    </script>
    <div class="login-box">
        <div class="logo-container">
            <asp:Image ID="imgLoginLogo" runat="server" ImageUrl="Image/Logo/delhi-golf-course-logo.png" AlternateText="Login Logo" CssClass="login-logo" />
        </div>
        <h2>DGC Compliance Tool</h2>
        <div class="form-group">
            <asp:Label ID="lblUsername" runat="server" Text="User Name" style="font-size:12px;font-weight:bold;" AssociatedControlID="txtUsername"></asp:Label>
            <asp:TextBox ID="txtUsername" runat="server"></asp:TextBox>
        </div>
        <div class="form-group">
            <asp:Label ID="lblPassword" runat="server" Text="Password" style="font-size:12px;font-weight:bold;" AssociatedControlID="txtPassword"></asp:Label>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
        </div>
        <div class="button-container">
            <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="LoginButton_Click" CssClass="login-button" />
            <asp:Button ID="btnForgotPassword" runat="server" Text="Forgot Password" OnClick="btnForgotPassword_Click" CssClass="forgot-button" />
        </div>
         <!-- // -----------------UNCOMMENT IF 2FA IS IMPLEMENED -->
<%-- <div id="twofaContainer" class="twofa-container" runat="server">
     <div class="form-group">
         <asp:Label ID="lblTwoFACode" runat="server" Text="Enter 2FA Code"  style="font-size:12px;font-weight:bold;"></asp:Label>
         <asp:TextBox ID="txtTwoFACode" runat="server" CssClass="twofa-code"></asp:TextBox>
     </div>
     <div class="button-container">
         <asp:Button ID="btnVerifyTwoFA" runat="server" Text="Verify" OnClick="btnVerifyTwoFA_Click" CssClass="login-button" />
         <asp:Button ID="btnCancelTwoFA" runat="server" Text="Cancel" OnClick="btnCancelTwoFA_Click" CssClass="forgot-button" />
     </div>
 </div>--%>
        <%-- // -----------------UNCOMMENT IF 2FA IS IMPLEMENED --%>
        </div>
       
    <!-- Modal for Forgot Password -->
    <div id="forgotPasswordModal" class="modal">
        <div class="modal-content">
            <span class="modal-close" onclick="hideModal()">&times;</span>
            <p>Contact your administrator to reset your password.</p>
        </div>
    </div>
    <!-- Modal for Wrong Password -->
<div id="wrongPasswordModal" class="modal">
    <div class="modal-content">
        <span class="modal-close" onclick="hidewrongModal()">&times;</span>
        <p>Invalid User Id /Password.</p>
    </div>
</div>
</asp:Content>

