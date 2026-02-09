namespace RateController.Menu
{
    partial class frmMenuCalibrate
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lbPWM = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.lbPWMData = new System.Windows.Forms.Label();
            this.lbSpeed = new System.Windows.Forms.Label();
            this.tbSpeed = new System.Windows.Forms.TextBox();
            this.lbCalSpeed = new System.Windows.Forms.Label();
            this.btnCalStop = new System.Windows.Forms.Button();
            this.btnCalStart = new System.Windows.Forms.Button();
            this.lbBaseRate = new System.Windows.Forms.Label();
            this.lbMeterSet = new System.Windows.Forms.Label();
            this.lbMeasured = new System.Windows.Forms.Label();
            this.lbExpected = new System.Windows.Forms.Label();
            this.lbCalFactor = new System.Windows.Forms.Label();
            this.lbPulses = new System.Windows.Forms.Label();
            this.pbRunning = new System.Windows.Forms.ProgressBar();
            this.btnLocked = new System.Windows.Forms.Button();
            this.tbMeasured = new System.Windows.Forms.TextBox();
            this.lbExpectedData = new System.Windows.Forms.Label();
            this.tbMeterCal = new System.Windows.Forms.TextBox();
            this.tbBaseRate = new System.Windows.Forms.TextBox();
            this.lbPulsesData = new System.Windows.Forms.Label();
            this.btnPower = new System.Windows.Forms.Button();
            this.lbName = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lbPWM
            // 
            this.lbPWM.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPWM.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lbPWM.Location = new System.Drawing.Point(159, 536);
            this.lbPWM.Name = "lbPWM";
            this.lbPWM.Size = new System.Drawing.Size(110, 24);
            this.lbPWM.TabIndex = 433;
            this.lbPWM.Text = "PWM";
            this.lbPWM.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.Transparent;
            this.button5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button5.Enabled = false;
            this.button5.FlatAppearance.BorderSize = 0;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.button5.Image = global::RateController.Properties.Resources.number_circle_five;
            this.button5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button5.Location = new System.Drawing.Point(386, 12);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(70, 63);
            this.button5.TabIndex = 432;
            this.button5.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.Transparent;
            this.button4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button4.Enabled = false;
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.button4.Image = global::RateController.Properties.Resources.number_circle_four;
            this.button4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button4.Location = new System.Drawing.Point(301, 12);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(70, 63);
            this.button4.TabIndex = 431;
            this.button4.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.Transparent;
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button3.Enabled = false;
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.button3.Image = global::RateController.Properties.Resources.number_circle_three;
            this.button3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button3.Location = new System.Drawing.Point(216, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(70, 63);
            this.button3.TabIndex = 430;
            this.button3.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Transparent;
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button2.Enabled = false;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.button2.Image = global::RateController.Properties.Resources.number_circle_two;
            this.button2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button2.Location = new System.Drawing.Point(131, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(70, 63);
            this.button2.TabIndex = 429;
            this.button2.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button1.Enabled = false;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.button1.Image = global::RateController.Properties.Resources.number_circle_one;
            this.button1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button1.Location = new System.Drawing.Point(46, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(70, 63);
            this.button1.TabIndex = 428;
            this.button1.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lbPWMData
            // 
            this.lbPWMData.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPWMData.Location = new System.Drawing.Point(325, 533);
            this.lbPWMData.Name = "lbPWMData";
            this.lbPWMData.Size = new System.Drawing.Size(88, 30);
            this.lbPWMData.TabIndex = 427;
            this.lbPWMData.Text = "0";
            this.lbPWMData.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbPWMData.Visible = false;
            // 
            // lbSpeed
            // 
            this.lbSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSpeed.ForeColor = System.Drawing.Color.DarkGreen;
            this.lbSpeed.Location = new System.Drawing.Point(120, 639);
            this.lbSpeed.Name = "lbSpeed";
            this.lbSpeed.Size = new System.Drawing.Size(54, 24);
            this.lbSpeed.TabIndex = 407;
            this.lbSpeed.Text = "mph";
            this.lbSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbSpeed
            // 
            this.tbSpeed.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSpeed.Location = new System.Drawing.Point(49, 636);
            this.tbSpeed.MaxLength = 8;
            this.tbSpeed.Name = "tbSpeed";
            this.tbSpeed.Size = new System.Drawing.Size(65, 30);
            this.tbSpeed.TabIndex = 426;
            this.tbSpeed.TabStop = false;
            this.tbSpeed.Text = "5.0";
            this.tbSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSpeed.TextChanged += new System.EventHandler(this.tbSpeed_TextChanged);
            this.tbSpeed.Enter += new System.EventHandler(this.tbSpeed_Enter);
            this.tbSpeed.Validating += new System.ComponentModel.CancelEventHandler(this.tbSpeed_Validating);
            // 
            // lbCalSpeed
            // 
            this.lbCalSpeed.AutoSize = true;
            this.lbCalSpeed.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCalSpeed.Location = new System.Drawing.Point(17, 594);
            this.lbCalSpeed.Name = "lbCalSpeed";
            this.lbCalSpeed.Size = new System.Drawing.Size(157, 23);
            this.lbCalSpeed.TabIndex = 411;
            this.lbCalSpeed.Text = "Calibration Speed";
            // 
            // btnCalStop
            // 
            this.btnCalStop.BackColor = System.Drawing.Color.Transparent;
            this.btnCalStop.FlatAppearance.BorderSize = 0;
            this.btnCalStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCalStop.Image = global::RateController.Properties.Resources.Stop;
            this.btnCalStop.Location = new System.Drawing.Point(297, 603);
            this.btnCalStop.Name = "btnCalStop";
            this.btnCalStop.Size = new System.Drawing.Size(70, 63);
            this.btnCalStop.TabIndex = 409;
            this.btnCalStop.UseVisualStyleBackColor = false;
            this.btnCalStop.Click += new System.EventHandler(this.btnCalStop_Click);
            // 
            // btnCalStart
            // 
            this.btnCalStart.BackColor = System.Drawing.Color.Transparent;
            this.btnCalStart.FlatAppearance.BorderSize = 0;
            this.btnCalStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCalStart.Image = global::RateController.Properties.Resources.Start;
            this.btnCalStart.Location = new System.Drawing.Point(219, 603);
            this.btnCalStart.Name = "btnCalStart";
            this.btnCalStart.Size = new System.Drawing.Size(70, 63);
            this.btnCalStart.TabIndex = 408;
            this.btnCalStart.UseVisualStyleBackColor = false;
            this.btnCalStart.Click += new System.EventHandler(this.btnCalStart_Click);
            // 
            // lbBaseRate
            // 
            this.lbBaseRate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbBaseRate.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lbBaseRate.Location = new System.Drawing.Point(153, 219);
            this.lbBaseRate.Name = "lbBaseRate";
            this.lbBaseRate.Size = new System.Drawing.Size(166, 24);
            this.lbBaseRate.TabIndex = 425;
            this.lbBaseRate.Text = "Base Rate";
            this.lbBaseRate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbMeterSet
            // 
            this.lbMeterSet.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMeterSet.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lbMeterSet.Location = new System.Drawing.Point(153, 460);
            this.lbMeterSet.Name = "lbMeterSet";
            this.lbMeterSet.Size = new System.Drawing.Size(110, 24);
            this.lbMeterSet.TabIndex = 424;
            this.lbMeterSet.Text = "Meter Set";
            this.lbMeterSet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbMeasured
            // 
            this.lbMeasured.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMeasured.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lbMeasured.Location = new System.Drawing.Point(153, 329);
            this.lbMeasured.Name = "lbMeasured";
            this.lbMeasured.Size = new System.Drawing.Size(165, 24);
            this.lbMeasured.TabIndex = 423;
            this.lbMeasured.Text = "Measured Amount";
            this.lbMeasured.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbExpected
            // 
            this.lbExpected.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbExpected.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lbExpected.Location = new System.Drawing.Point(153, 384);
            this.lbExpected.Name = "lbExpected";
            this.lbExpected.Size = new System.Drawing.Size(153, 24);
            this.lbExpected.TabIndex = 422;
            this.lbExpected.Text = "Expected Amount";
            this.lbExpected.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbCalFactor
            // 
            this.lbCalFactor.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCalFactor.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lbCalFactor.Location = new System.Drawing.Point(153, 274);
            this.lbCalFactor.Name = "lbCalFactor";
            this.lbCalFactor.Size = new System.Drawing.Size(110, 24);
            this.lbCalFactor.TabIndex = 421;
            this.lbCalFactor.Text = "Meter Cal";
            this.lbCalFactor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbPulses
            // 
            this.lbPulses.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPulses.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lbPulses.Location = new System.Drawing.Point(153, 164);
            this.lbPulses.Name = "lbPulses";
            this.lbPulses.Size = new System.Drawing.Size(110, 24);
            this.lbPulses.TabIndex = 420;
            this.lbPulses.Text = "Pulses";
            this.lbPulses.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pbRunning
            // 
            this.pbRunning.Location = new System.Drawing.Point(353, 461);
            this.pbRunning.MarqueeAnimationSpeed = 50;
            this.pbRunning.Name = "pbRunning";
            this.pbRunning.Size = new System.Drawing.Size(72, 23);
            this.pbRunning.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbRunning.TabIndex = 419;
            // 
            // btnLocked
            // 
            this.btnLocked.BackColor = System.Drawing.Color.Transparent;
            this.btnLocked.FlatAppearance.BorderSize = 0;
            this.btnLocked.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLocked.Image = global::RateController.Properties.Resources.ColorUnlocked;
            this.btnLocked.Location = new System.Drawing.Point(353, 436);
            this.btnLocked.Name = "btnLocked";
            this.btnLocked.Size = new System.Drawing.Size(72, 72);
            this.btnLocked.TabIndex = 418;
            this.btnLocked.UseVisualStyleBackColor = false;
            this.btnLocked.Visible = false;
            this.btnLocked.Click += new System.EventHandler(this.btnLocked_Click);
            // 
            // tbMeasured
            // 
            this.tbMeasured.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMeasured.Location = new System.Drawing.Point(325, 326);
            this.tbMeasured.MaxLength = 8;
            this.tbMeasured.Name = "tbMeasured";
            this.tbMeasured.Size = new System.Drawing.Size(88, 30);
            this.tbMeasured.TabIndex = 417;
            this.tbMeasured.TabStop = false;
            this.tbMeasured.Text = "100.5";
            this.tbMeasured.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbMeasured.TextChanged += new System.EventHandler(this.tbBaseRate_TextChanged);
            this.tbMeasured.Enter += new System.EventHandler(this.tbMeasured_Enter);
            this.tbMeasured.Validating += new System.ComponentModel.CancelEventHandler(this.tbMeasured_Validating);
            // 
            // lbExpectedData
            // 
            this.lbExpectedData.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbExpectedData.Location = new System.Drawing.Point(325, 381);
            this.lbExpectedData.Name = "lbExpectedData";
            this.lbExpectedData.Size = new System.Drawing.Size(88, 30);
            this.lbExpectedData.TabIndex = 416;
            this.lbExpectedData.Text = "0";
            this.lbExpectedData.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbMeterCal
            // 
            this.tbMeterCal.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMeterCal.Location = new System.Drawing.Point(325, 271);
            this.tbMeterCal.MaxLength = 8;
            this.tbMeterCal.Name = "tbMeterCal";
            this.tbMeterCal.Size = new System.Drawing.Size(88, 30);
            this.tbMeterCal.TabIndex = 415;
            this.tbMeterCal.TabStop = false;
            this.tbMeterCal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbMeterCal.TextChanged += new System.EventHandler(this.tbBaseRate_TextChanged);
            this.tbMeterCal.Enter += new System.EventHandler(this.tbMeterCal_Enter);
            this.tbMeterCal.Validating += new System.ComponentModel.CancelEventHandler(this.tbMeterCal_Validating);
            // 
            // tbBaseRate
            // 
            this.tbBaseRate.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbBaseRate.Location = new System.Drawing.Point(325, 216);
            this.tbBaseRate.MaxLength = 8;
            this.tbBaseRate.Name = "tbBaseRate";
            this.tbBaseRate.Size = new System.Drawing.Size(88, 30);
            this.tbBaseRate.TabIndex = 414;
            this.tbBaseRate.TabStop = false;
            this.tbBaseRate.Text = "99,999";
            this.tbBaseRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbBaseRate.TextChanged += new System.EventHandler(this.tbBaseRate_TextChanged);
            this.tbBaseRate.Enter += new System.EventHandler(this.tbBaseRate_Enter);
            this.tbBaseRate.Validating += new System.ComponentModel.CancelEventHandler(this.tbBaseRate_Validating);
            // 
            // lbPulsesData
            // 
            this.lbPulsesData.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPulsesData.Location = new System.Drawing.Point(325, 161);
            this.lbPulsesData.Name = "lbPulsesData";
            this.lbPulsesData.Size = new System.Drawing.Size(88, 30);
            this.lbPulsesData.TabIndex = 406;
            this.lbPulsesData.Text = "0";
            this.lbPulsesData.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnPower
            // 
            this.btnPower.BackColor = System.Drawing.Color.Transparent;
            this.btnPower.FlatAppearance.BorderSize = 0;
            this.btnPower.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPower.Image = global::RateController.Properties.Resources.FanOff;
            this.btnPower.Location = new System.Drawing.Point(46, 91);
            this.btnPower.Name = "btnPower";
            this.btnPower.Size = new System.Drawing.Size(72, 72);
            this.btnPower.TabIndex = 405;
            this.btnPower.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnPower.UseVisualStyleBackColor = false;
            this.btnPower.Click += new System.EventHandler(this.btnPower_Click);
            // 
            // lbName
            // 
            this.lbName.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbName.Location = new System.Drawing.Point(127, 109);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(299, 37);
            this.lbName.TabIndex = 413;
            this.lbName.Text = "1. Wheat - Standard Valve\r\n";
            this.lbName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCancel.Enabled = false;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnCancel.Image = global::RateController.Properties.Resources.Cancel64;
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(375, 603);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 63);
            this.btnCancel.TabIndex = 410;
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.Transparent;
            this.btnOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnOK.Enabled = false;
            this.btnOK.FlatAppearance.BorderSize = 0;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnOK.Image = global::RateController.Properties.Resources.Save;
            this.btnOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnOK.Location = new System.Drawing.Point(453, 603);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 63);
            this.btnOK.TabIndex = 412;
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmMenuCalibrate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 678);
            this.Controls.Add(this.lbPWM);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lbPWMData);
            this.Controls.Add(this.lbSpeed);
            this.Controls.Add(this.tbSpeed);
            this.Controls.Add(this.lbCalSpeed);
            this.Controls.Add(this.btnCalStop);
            this.Controls.Add(this.btnCalStart);
            this.Controls.Add(this.lbBaseRate);
            this.Controls.Add(this.lbMeterSet);
            this.Controls.Add(this.lbMeasured);
            this.Controls.Add(this.lbExpected);
            this.Controls.Add(this.lbCalFactor);
            this.Controls.Add(this.lbPulses);
            this.Controls.Add(this.pbRunning);
            this.Controls.Add(this.btnLocked);
            this.Controls.Add(this.tbMeasured);
            this.Controls.Add(this.lbExpectedData);
            this.Controls.Add(this.tbMeterCal);
            this.Controls.Add(this.tbBaseRate);
            this.Controls.Add(this.lbPulsesData);
            this.Controls.Add(this.btnPower);
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuCalibrate";
            this.ShowInTaskbar = false;
            this.Text = "frmMenuCalibrate";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMenuCalibrate_FormClosing);
            this.Load += new System.EventHandler(this.frmMenuCalibrate_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbPWM;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lbPWMData;
        private System.Windows.Forms.Label lbSpeed;
        private System.Windows.Forms.TextBox tbSpeed;
        private System.Windows.Forms.Label lbCalSpeed;
        private System.Windows.Forms.Button btnCalStop;
        private System.Windows.Forms.Button btnCalStart;
        private System.Windows.Forms.Label lbBaseRate;
        private System.Windows.Forms.Label lbMeterSet;
        private System.Windows.Forms.Label lbMeasured;
        private System.Windows.Forms.Label lbExpected;
        private System.Windows.Forms.Label lbCalFactor;
        private System.Windows.Forms.Label lbPulses;
        private System.Windows.Forms.ProgressBar pbRunning;
        private System.Windows.Forms.Button btnLocked;
        private System.Windows.Forms.TextBox tbMeasured;
        private System.Windows.Forms.Label lbExpectedData;
        private System.Windows.Forms.TextBox tbMeterCal;
        private System.Windows.Forms.TextBox tbBaseRate;
        private System.Windows.Forms.Label lbPulsesData;
        private System.Windows.Forms.Button btnPower;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Timer timer1;
    }
}