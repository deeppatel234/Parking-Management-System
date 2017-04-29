using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data.SqlClient;
using System.Windows;

namespace DEFinal
{
    class PDF
    {
        public void printGateData(SqlDataReader dr)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "PDF file (*.pdf)|*.pdf";
                if (saveFileDialog.ShowDialog() == true)
                {
                    System.IO.FileStream fs = new System.IO.FileStream(saveFileDialog.FileName, System.IO.FileMode.Create);

                    Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);

                    document.Open();

                    PdfPTable table = new PdfPTable(5);
                    PdfPCell cell = new PdfPCell(new Phrase("Gate Usage Details"));
                    cell.Colspan = 5;
                    cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    table.AddCell(cell);

                    var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);


                    table.AddCell(new iTextSharp.text.Paragraph("No.", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("Gate Type", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("Start Date and Time", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("End Date and Time", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("Mode", boldFont));

                    int i = 1;
                    while (dr.Read())
                    {
                        table.AddCell(i + "");
                        table.AddCell(dr[1].ToString());
                        table.AddCell(dr[2].ToString());
                        table.AddCell(dr[3].ToString());
                        table.AddCell(dr[4].ToString());
                        i++;
                    }

                    document.Add(table);
                    document.Close();
                    writer.Close();
                    fs.Close();
                    dr.Close();
                }
            }
            catch
            {
                MessageBox.Show("Something Went Wrong");
            }
        }

        public void getAllGateDetails(SqlDataReader dr)
        {
            printGateData(dr);
        }

        public void getbyDate(string startdate,string enddate,SqlDataReader dr)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "PDF file (*.pdf)|*.pdf";
                if (saveFileDialog.ShowDialog() == true)
                {
                    System.IO.FileStream fs = new System.IO.FileStream(saveFileDialog.FileName, System.IO.FileMode.Create);

                    Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);

                    document.Open();

                    PdfPTable table = new PdfPTable(5);
                    PdfPCell cell = new PdfPCell(new Phrase("Gate Usage Details"));
                    cell.Colspan = 5;
                    cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    table.AddCell(cell);

                    var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);


                    table.AddCell(new iTextSharp.text.Paragraph("No.", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("Gate Type", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("Start Date and Time", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("End Date and Time", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("Mode", boldFont));

                    int i = 1;
                    string[] splitStartDate;
                    string[] splitEndDate;

                    while (dr.Read())
                    {
                        splitStartDate = dr.GetValue(2).ToString().Split(' ');
                        splitEndDate = dr.GetValue(3).ToString().Split(' ');

                        if (DateTime.Parse(splitStartDate[0]) >= DateTime.Parse(startdate) && DateTime.Parse(splitEndDate[0]) <= DateTime.Parse(enddate))
                        {
                            table.AddCell(i + "");
                            table.AddCell(dr[1].ToString());
                            table.AddCell(dr[2].ToString());
                            table.AddCell(dr[3].ToString());
                            table.AddCell(dr[4].ToString());
                            i++;
                        }
                    }

                    document.Add(table);
                    document.Close();
                    writer.Close();
                    fs.Close();
                    dr.Close();
                }
            }
            catch
            {
                MessageBox.Show("Something Went Wrong");
            }
        }




        public void printCarUsageData(SqlDataReader dr,string type)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "PDF file (*.pdf)|*.pdf";
                if (saveFileDialog.ShowDialog() == true)
                {
                    System.IO.FileStream fs = new System.IO.FileStream(saveFileDialog.FileName, System.IO.FileMode.Create);

                    Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);

                    document.Open();

                    PdfPTable table = new PdfPTable(4);
                    PdfPCell cell = new PdfPCell(new Phrase(type + " Usage Details"));
                    cell.Colspan =4;
                    cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    table.AddCell(cell);

                    var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);


                    table.AddCell(new iTextSharp.text.Paragraph("No.", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("Slot NO.", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("Start Date and Time", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("End Date and Time", boldFont));
                    

                    int i = 1;
                    while (dr.Read())
                    {
                        table.AddCell(i + "");
                        table.AddCell(dr[1].ToString());
                        table.AddCell(dr[2].ToString());
                        table.AddCell(dr[3].ToString());
                        i++;
                    }

                    document.Add(table);
                    document.Close();
                    writer.Close();
                    fs.Close();
                    dr.Close();
                }
            }
            catch
            {
                MessageBox.Show("Something Went Wrong");
            }
        }


        public void printCarDateDetails(SqlDataReader dr, string type,string startdate,string enddate)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "PDF file (*.pdf)|*.pdf";
                if (saveFileDialog.ShowDialog() == true)
                {
                    System.IO.FileStream fs = new System.IO.FileStream(saveFileDialog.FileName, System.IO.FileMode.Create);

                    Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);

                    document.Open();

                    PdfPTable table = new PdfPTable(4);
                    PdfPCell cell = new PdfPCell(new Phrase(type + " Usage Details"));
                    cell.Colspan = 4;
                    cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    table.AddCell(cell);

                    var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);


                    table.AddCell(new iTextSharp.text.Paragraph("No.", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("Slot NO.", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("Start Date and Time", boldFont));
                    table.AddCell(new iTextSharp.text.Paragraph("End Date and Time", boldFont));


                    int i = 1;
                    string[] splitStartDate;
                    string[] splitEndDate;

                    while (dr.Read())
                    {
                        splitStartDate = dr.GetValue(2).ToString().Split(' ');
                        splitEndDate = dr.GetValue(3).ToString().Split(' ');

                        if (DateTime.Parse(splitStartDate[0]) >= DateTime.Parse(startdate) && DateTime.Parse(splitEndDate[0]) <= DateTime.Parse(enddate))
                        {
                            table.AddCell(i + "");
                            table.AddCell(dr[1].ToString());
                            table.AddCell(dr[2].ToString());
                            table.AddCell(dr[3].ToString());
                            i++;
                        }
                    }

                    document.Add(table);
                    document.Close();
                    writer.Close();
                    fs.Close();
                    dr.Close();
                }
            }
            catch
            {
                MessageBox.Show("Something Went Wrong");
            }
        }

    }
}
