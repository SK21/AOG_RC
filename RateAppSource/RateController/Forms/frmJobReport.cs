using RateController.Classes;
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
        private int MaxLineLength = 40;
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
                int Pad1 = 15;
                int PadUsed = 0;

                string line = "Job Report - " + JB.Name;
                Add(line, true, true, 20, 0, 2);

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
                    Add(line, true, true, Pad4, PadUsed, 0);
                    PadUsed = line.Length + Pad4;

                    if (Props.UseMetric)
                    {
                        line = "Hectares";
                    }
                    else
                    {
                        line = "Acres";
                    }
                    Add(line, true, true, Pad5, PadUsed);

                    foreach (JobProductData PrdData in Data)
                    {
                        //if (PrdData.Hectares > 0)
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

                            line = PrdData.Quantity.ToString("N2");
                            Add(line, false, false, Pad4, PadUsed, 0);
                            PadUsed = line.Length + Pad4;

                            if (Props.UseMetric)
                            {
                                line = PrdData.Hectares.ToString("N2");
                            }
                            else
                            {
                                line = (PrdData.Hectares * 2.47).ToString("N2");
                            }
                            Add(line, false, false, Pad5, PadUsed);
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