using RateController.Classes;
using RateController.Printing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmJobReport : Form
    {
        private string DateFormat = "yyyy-MM-ddTHH:mm";
        private Job JB;
        private int MaxLineLength = 45;
        private bool UseBold = false;
        private bool UseUnderline = false;

        public frmJobReport(Job JobToReport)
        {
            InitializeComponent();
            JB = JobToReport;
            ShowReport();
        }

        private void Add(string line, bool Underline = false, bool Bold = false, int Pad = 0, int TitleLength = 0, int LineFeed = 1)
        {
            try
            {
                if (line.Length > MaxLineLength)
                {
                    List<string> Lines = SplitString(line, MaxLineLength);

                    bool FirstLine = true;
                    foreach (string SubLine in Lines)
                    {
                        if (FirstLine)
                        {
                            // adjust padding for title
                            FirstLine = false;
                            if (Pad - TitleLength > 0)
                            {
                                if (UseUnderline) ShowUnderline(false);
                                if (UseBold) ShowBold(false);
                                rtb.AppendText(string.Empty.PadRight(Pad - TitleLength));
                            }
                        }
                        else if (Pad > 0)
                        {
                            if (UseUnderline) ShowUnderline(false);
                            if (UseBold) ShowBold(false);
                            rtb.AppendText(string.Empty.PadRight(Pad));
                        }

                        if (UseUnderline != Underline) ShowUnderline(Underline);
                        if (UseBold != Bold) ShowBold(Bold);
                        rtb.AppendText(SubLine);

                        if (LineFeed > 0 && LineFeed < 20)
                        {
                            for (int i = 0; i < LineFeed; i++)
                            {
                                rtb.AppendText(Environment.NewLine);
                            }
                        }
                    }
                }
                else
                {
                    if (Pad - TitleLength > 0)
                    {
                        if (UseUnderline) ShowUnderline(false);
                        if (UseBold) ShowBold(false);
                        rtb.AppendText(string.Empty.PadRight(Pad - TitleLength));
                    }

                    if (UseUnderline != Underline) ShowUnderline(Underline);
                    if (UseBold != Bold) ShowBold(Bold);
                    rtb.AppendText(line);

                    if (LineFeed > 0 && LineFeed < 20)
                    {
                        for (int i = 0; i < LineFeed; i++)
                        {
                            rtb.AppendText(Environment.NewLine);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmJobReport/Add: " + ex.Message);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            RichTextBoxPrinterRTF printer = new RichTextBoxPrinterRTF(rtb);
            printer.Print();
        }

        private void frmJobReport_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmJobReport_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
        }

        private void ShowBold(bool Enabled = true)
        {
            UseBold = Enabled;
            Font currentFont = rtb.SelectionFont ?? rtb.Font;
            FontStyle newStyle = currentFont.Style;
            if (UseBold)
            {
                newStyle |= FontStyle.Bold;
            }
            else
            {
                newStyle &= ~FontStyle.Bold;
            }
            rtb.SelectionFont = new Font(currentFont, newStyle);
        }

        private void ShowReport()
        {
            try
            {
                int Pad1 = 10;
                int PadUsed = 0;

                string line = "Job Report";
                Add(line, true, true, 25, 0, 2);

                line = "Name:";
                Add(line, false, false, 0, 0, 0);
                PadUsed = line.Length;

                line = JB.Name;
                Add(line, false, false, Pad1, PadUsed);

                line = "Date:";
                Add(line, false, false, 0, 0, 0);
                PadUsed = line.Length;

                line = JB.Date.ToString("dd-MMM-yyyy");
                Add(line, false, false, Pad1, PadUsed);

                line = "Field:";
                Add(line, false, false, 0, 0, 0);
                PadUsed = line.Length;

                line = ParcelManager.SearchParcel(JB.FieldID)?.Name ?? "Unknown Field";
                Add(line, false, false, Pad1, PadUsed);

                line = "Notes:";
                Add(line, false, false, 0, 0, 0);
                PadUsed = line.Length;

                line = JB.Notes;
                Add(line, false, false, Pad1, PadUsed, 1);
                Add("");

                // products
                JobProductData[] Data = Props.JobCollector.JobData(JB);
                if (Data.Length > 0)
                {
                    int Pad2 = 12;
                    int Pad3 = 24;
                    int Pad4 = 36;
                    int Pad5 = 48;

                    // Fixed column widths for right alignment of numeric columns
                    int QtyColWidth = 10;     // adjust as needed
                    int AreaColWidth = 10;    // adjust as needed

                    line = "Product";
                    Add(line, true, true, 0, 0, 0);
                    PadUsed = line.Length;

                    line = "Start";
                    Add(line, true, true, Pad2, PadUsed, 0);
                    PadUsed = line.Length + Pad2;

                    line = "Finish";
                    Add(line, true, true, Pad3, PadUsed, 0);
                    PadUsed = line.Length + Pad3;

                    line = "Quantity";
                    int qtyExtraPad = Math.Max(0, QtyColWidth - line.Length);
                    int qtyStartPad = Pad4 + qtyExtraPad;
                    Add(line, true, true, qtyStartPad, PadUsed, 0);
                    PadUsed = qtyStartPad + line.Length;

                    if (Props.UseMetric)
                    {
                        line = "Hectares";
                    }
                    else
                    {
                        line = "Acres";
                    }
                    int areaExtraPad = Math.Max(0, AreaColWidth - line.Length);
                    int areaStartPad = Pad5 + areaExtraPad;
                    Add(line, true, true, areaStartPad, PadUsed);

                    foreach (JobProductData PrdData in Data)
                    {
                        if (PrdData.Hectares > 0)
                        {
                            line = Props.MainForm.Products.Item(PrdData.ProductID).ProductName;
                            Add(line, false, false, 0, 0, 0);
                            PadUsed = line.Length;

                            if (PrdData.StartTime == DateTime.MinValue)
                            {
                                line = "-";
                            }
                            else
                            {
                                line = PrdData.StartTime.ToString(DateFormat);
                            }
                            Add(line, false, false, Pad2, PadUsed, 0);
                            PadUsed = line.Length + Pad2;

                            // End time
                            if (PrdData.EndTime == DateTime.MinValue)
                            {
                                line = "-";
                            }
                            else
                            {
                                line = PrdData.EndTime.ToString(DateFormat);
                            }
                            Add(line, false, false, Pad3, PadUsed, 0);
                            PadUsed = line.Length + Pad3;

                            // Quantity (right-aligned within QtyColWidth starting at Pad4)
                            line = PrdData.Quantity.ToString("N2");
                            qtyExtraPad = Math.Max(0, QtyColWidth - line.Length);
                            qtyStartPad = Pad4 + qtyExtraPad;
                            Add(line, false, false, qtyStartPad, PadUsed, 0);
                            PadUsed = qtyStartPad + line.Length;

                            // Area (Hectares/Acres) right-aligned within AreaColWidth starting at Pad5
                            if (Props.UseMetric)
                            {
                                line = PrdData.Hectares.ToString("N2");
                            }
                            else
                            {
                                line = (PrdData.Hectares * 2.47).ToString("N2");
                            }
                            areaExtraPad = Math.Max(0, AreaColWidth - line.Length);
                            areaStartPad = Pad5 + areaExtraPad;
                            Add(line, false, false, areaStartPad, PadUsed);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmJobReport/ShowReport: " + ex.Message);
            }
        }

        private void ShowUnderline(bool Enabled = true)
        {
            UseUnderline = Enabled;
            Font currentFont = rtb.SelectionFont ?? rtb.Font;
            FontStyle newStyle = currentFont.Style;
            if (UseUnderline)
            {
                newStyle |= FontStyle.Underline;
            }
            else
            {
                newStyle &= ~FontStyle.Underline;
            }
            rtb.SelectionFont = new Font(currentFont, newStyle);
        }

        private List<string> SplitString(string input, int maxLength)
        {
            List<string> result = new List<string>();

            for (int i = 0; i < input.Length; i += maxLength)
            {
                int length = Math.Min(maxLength, input.Length - i);
                result.Add(input.Substring(i, length));
            }

            return result;
        }
    }
}