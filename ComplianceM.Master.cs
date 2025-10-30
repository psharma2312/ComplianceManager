using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class ComplianceM : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["IsAuthenticated"] == null || !(bool)Session["IsAuthenticated"])
            {
                Response.Redirect("~/Login.aspx");
            }
            else
            {
                lblUsername.Text = "Welcome, " + (Session["Name"]?.ToString() ?? "Guest");
                BindMenu();
                UpdateNotificationCount();
            }
         
            if (Request["__EVENTTARGET"] == "ctl00$SignOutLink")
            {
                SignOut();
            }
        }
        private void BindMenu()
        {
            int userId = Convert.ToInt32(Session["UserId"]?.ToString() ?? "0");
            DAL dal = new DAL();
            var sideMenu = dal.GenerateSideMenu(userId);
            StringBuilder menuHtml = new StringBuilder();
            BuildMenuRecursive(sideMenu, null, menuHtml);
            phMenu.Controls.Add(new Literal { Text = menuHtml.ToString() });
        }
        private void BuildMenuRecursive(List<SideMenu> menuItems, int? parentId, StringBuilder menuHtml)
        {
            // Filter items based on ParentMenuId (null for top-level, specific ID for submenus)
            var items = menuItems.Where(m => m.ParentMenuId == parentId).ToArray();
            if (items.Length > 0)
            {
                menuHtml.Append($"<ul class=\"{(parentId.HasValue ? "submenu" : "menu")}\" {(parentId.HasValue ? $"id=\"submenu_{parentId}\"" : "")}>");
                foreach (var item in items)
                {
                    menuHtml.Append("<li>");
                    string menuText = item.MenuText;
                    string menuUrl = item.MenuUrl;
                    int menuId = item.MenuId;

                    if (!parentId.HasValue) // Top-level menu
                    {
                        if (menuText.Equals("LogOut", StringComparison.OrdinalIgnoreCase))
                        {
                            // LogOut: No arrow, triggers sign-out
                            menuHtml.Append($"<a href=\"{ResolveUrl(menuUrl)}\" onclick=\"return signOut();\">{menuText}</a>");
                        }
                        else
                        {
                            // Other top-level items: Expandable with arrow
                            menuHtml.Append($"<a href=\"javascript:;\" onclick=\"toggleSubmenu({menuId})\" class=\"expandable\">{menuText}</a>");
                        }
                    }
                    else // Submenu
                    {
                        menuHtml.Append($"<a href=\"{ResolveUrl(menuUrl)}\">{menuText}</a>");
                    }
                    // Recursively build submenus
                    var childItems = menuItems.Where(m => m.ParentMenuId == item.MenuId).ToArray();
                    if (childItems.Length > 0)
                    {
                        BuildMenuRecursive(menuItems, item.MenuId, menuHtml);
                    }
                    menuHtml.Append("</li>");
                }
                menuHtml.Append("</ul>");
            }
        }

        // Sign-out method
        protected void SignOut()
        {
            Session["IsAuthenticated"] = false;
            Session["Name"] = null;
            Session["UserName"] = null;
            Session["UserId"] = null;
            Response.Redirect("~/Login.aspx");
        }
        protected void lnkMail_Click(object sender, EventArgs e)
        {
            LoadUserControl("~/Mail.ascx");
        }
        protected void lnkNotifications_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Notification.aspx");
        }
        protected void lnkUser_Click(object sender, EventArgs e)
        {
            LoadUserControl("~/Controls/UserProfile.ascx");
        }

        private void LoadUserControl(string controlPath, bool passUsername = false)
        {
            Control contentPlaceHolder = FindControl("cntPlaceHolder1");
            contentPlaceHolder.Controls.Clear();

            try
            {
                Control userControl = LoadControl(controlPath);
                if (userControl != null)
                {
                    contentPlaceHolder.Controls.Add(userControl);
                }
                else
                {
                    lblErrorMessage.Text = $"Failed to load {controlPath}";
                    lblErrorMessage.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = $"Error loading {controlPath}: {ex.Message}";
                lblErrorMessage.Visible = true;
            }
        }

        public void UpdateNotificationCount()
        {
            int userId = Convert.ToInt32(Session["UserId"]?.ToString() ?? "0");
            DAL dal = new DAL();
            var notificationCount = dal.GetNotificationCount(userId);
            lblNotificationCount.Text = notificationCount.ToString();
            lblNotificationCount.Visible = notificationCount > 0; // Hide if no notifications
            
        }
    }
}