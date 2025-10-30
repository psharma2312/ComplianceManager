<%@ Page Title="" Language="C#" MasterPageFile="~/ComplianceM.Master" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="ComplianceManager.Test" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cntPlaceHolder1" runat="server">
     <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
        <div style="margin: 15px 0;">
        <asp:Label ID="lblEffectiveFromDate" runat="server" Text="Effective From" AssociatedControlID="txtEffectiveFromDate" 
            style="display: inline-block; width: 140px;"></asp:Label>
        <asp:TextBox ID="txtEffectiveFromDate" runat="server" />
<asp:ImageButton ID="imgbtnCalendar" runat="server" ImageUrl="~/image/logo/calendar.png" />
<cc1:CalendarExtender ID="calEffective"   Format="dd/MM/yyyy" TargetControlID="txtEffectiveFromDate" PopupButtonID="imgbtnCalendar" runat="server" />
    </div>

      <div style="text-align: left; margin-top: 0px;">
      <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="save-button" />
  </div>

</asp:Content>
