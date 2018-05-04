namespace TestForm
{
    partial class Form1
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
            this.schedulerControl1 = new HotelSchedulerControl.Chart.SchedulerControl();
            this.SuspendLayout();
            // 
            // schedulerControl1
            // 
            this.schedulerControl1.BarHeight = 0;
            this.schedulerControl1.BarSpacing = 0;
            this.schedulerControl1.HeaderFormat = null;
            this.schedulerControl1.HeaderOneHeight = 0;
            this.schedulerControl1.HeaderTwoHeight = 0;
            this.schedulerControl1.Location = new System.Drawing.Point(13, 13);
            this.schedulerControl1.MajorWidth = 0;
            this.schedulerControl1.MinorWidth = 0;
            this.schedulerControl1.Name = "schedulerControl1";
            this.schedulerControl1.ShowTaskLabels = false;
            this.schedulerControl1.Size = new System.Drawing.Size(150, 150);
            this.schedulerControl1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.schedulerControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private HotelSchedulerControl.Chart.SchedulerControl schedulerControl1;
    }
}

