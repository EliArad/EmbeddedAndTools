namespace TopazApp.TOPNAT
{
    partial class ViewNatPicturesForm
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
            this.flowLayoutPanelMain1 = new marlie.TumbnailDotnet.ThumbnailFlowLayoutPanel();
            this.flowLayoutPanelMain = new marlie.TumbnailDotnet.ThumbnailFlowLayoutPanel();
            this.SuspendLayout();
            // 
            // flowLayoutPanelMain1
            // 
            this.flowLayoutPanelMain1.AutoScroll = true;
            this.flowLayoutPanelMain1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.flowLayoutPanelMain1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flowLayoutPanelMain1.CausesValidation = false;
            this.flowLayoutPanelMain1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanelMain1.Location = new System.Drawing.Point(0, 392);
            this.flowLayoutPanelMain1.Name = "flowLayoutPanelMain1";
            this.flowLayoutPanelMain1.Size = new System.Drawing.Size(1132, 365);
            this.flowLayoutPanelMain1.TabIndex = 2;
            // 
            // flowLayoutPanelMain
            // 
            this.flowLayoutPanelMain.AutoScroll = true;
            this.flowLayoutPanelMain.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.flowLayoutPanelMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flowLayoutPanelMain.CausesValidation = false;
            this.flowLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanelMain.Name = "flowLayoutPanelMain";
            this.flowLayoutPanelMain.Size = new System.Drawing.Size(1132, 392);
            this.flowLayoutPanelMain.TabIndex = 1;
            this.flowLayoutPanelMain.WrapContents = false;
            // 
            // ViewNatPicturesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1132, 756);
            this.Controls.Add(this.flowLayoutPanelMain1);
            this.Controls.Add(this.flowLayoutPanelMain);
            this.Name = "ViewNatPicturesForm";
            this.Text = "ViewNatPicturesForm";
            this.ResumeLayout(false);

        }

        #endregion

        private marlie.TumbnailDotnet.ThumbnailFlowLayoutPanel flowLayoutPanelMain;
        private marlie.TumbnailDotnet.ThumbnailFlowLayoutPanel flowLayoutPanelMain1;

    }
}