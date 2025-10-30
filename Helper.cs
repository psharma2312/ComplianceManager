using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI.WebControls;
using Image = iTextSharp.text.Image;

namespace ComplianceManager
{
	public static class Helper
	{
        public static Font GetFont(int size, int style = Font.NORMAL)
        {
            return FontFactory.GetFont("Arial", size, style, BaseColor.BLACK);
        }
        public static void AddBlankRow(PdfPTable table, int colspan, int repeat = 1)
        {
            for (int i = 0; i < repeat; i++)
            {
                AddCell(table, "", GetFont(10), colspan);
            }
        }
        public static void AddCell(PdfPTable table, string text, Font font, int colspan = 1, int align = Element.ALIGN_LEFT, bool border = false)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font))
            {
                Colspan = colspan,
                HorizontalAlignment = align,
                Border = border ? Rectangle.BOX : Rectangle.NO_BORDER
            };
            table.AddCell(cell);
        }
        public static void AddCellVertical(PdfPTable table, string text, Font font, int colspan = 1, int align = Element.ALIGN_LEFT, bool border = false, int verticalAlign = Element.ALIGN_MIDDLE)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font))
            {
                Colspan = colspan,
                HorizontalAlignment = align,
                VerticalAlignment = verticalAlign,
                Border = border ? Rectangle.BOX : Rectangle.NO_BORDER
                
            };
            table.AddCell(cell);
        }
        public static void AddRightAlignedCell(PdfPTable table, string text, Font font, int colspan)
        {
            AddCell(table, text, font, colspan, Element.ALIGN_RIGHT);
        }

        public static void AddImageCell(PdfPTable table, Image image, int colspan, int rowspan)
        {
            PdfPCell cell = new PdfPCell
            {
                Colspan = colspan,
                Rowspan = rowspan,
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.AddElement(image);
            table.AddCell(cell);
        }

        public static void AddEmptyCell(PdfPTable table, int colspan, int rowspan)
        {
            PdfPCell cell = new PdfPCell(new Phrase(""))
            {
                Colspan = colspan,
                Rowspan = rowspan,
                Border = Rectangle.NO_BORDER
            };
            table.AddCell(cell);
        }
        public static void AddUnderlineRow(PdfPTable table, int colspan)
        {
            PdfPCell cell = new PdfPCell(new Phrase(""))
            {
                Colspan = colspan,
                Border = Rectangle.BOTTOM_BORDER
            };
            table.AddCell(cell);
        }
        public static void AddHeaderCell(PdfPTable table, string text, Font font, int colspan)
        {
            AddCell(table, text, font, colspan, Element.ALIGN_CENTER, true);
        }

        public static void AddDataCell(PdfPTable table, string text, Font font, int colspan, int align = Element.ALIGN_LEFT)
        {
            AddCell(table, text, font, colspan, align, true);
        }
        public static void SendPdfToBrowser(MemoryStream ms, string filename)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.ContentType = "application/pdf";
            HttpContext.Current.Response.AddHeader("Content-Disposition", $"attachment;filename={filename}");
            using (var output = HttpContext.Current.Response.OutputStream)
            {
                byte[] bytes = ms.ToArray();
                output.Write(bytes, 0, bytes.Length);
                output.Flush();
            }
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        public static DateTime GetLocalTime()
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        }
        public static void ShowMessage(string message, bool isError, Label lblErrorMessage)
        {
            lblErrorMessage.Text = message;
            lblErrorMessage.ForeColor = isError ? System.Drawing.Color.FromArgb(0x72, 0x1C, 0x24) : System.Drawing.Color.Green;
            lblErrorMessage.Visible = true;
        }
        public static void BindDropdown(DropDownList dropdown, object dataSource, string textField, string valueField, string defaultText)
        {
            dropdown.DataSource = dataSource;
            dropdown.DataTextField = textField;
            dropdown.DataValueField = valueField;
            dropdown.DataBind();
            dropdown.Items.Insert(0, new System.Web.UI.WebControls.ListItem(defaultText, "0"));
        }
        public static int dropdownValue(DropDownList ddl)
        {
            int index = 0;
            if (ddl.SelectedIndex != 0)
            {
                index = Convert.ToInt32(ddl.SelectedValue);
            }
            return index;
        }

        
       
    }
}