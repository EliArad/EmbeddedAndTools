namespace TopazApp.Forms.DebugForms
{
    partial class EqualEnergyGraphForm
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
            this.zg3 = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // zg3
            // 
            this.zg3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zg3.EditButtons = System.Windows.Forms.MouseButtons.Left;
            this.zg3.Location = new System.Drawing.Point(0, 0);
            this.zg3.Name = "zg3";
            this.zg3.PanModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)));
            this.zg3.ScrollGrace = 0D;
            this.zg3.ScrollMaxX = 0D;
            this.zg3.ScrollMaxY = 0D;
            this.zg3.ScrollMaxY2 = 0D;
            this.zg3.ScrollMinX = 0D;
            this.zg3.ScrollMinY = 0D;
            this.zg3.ScrollMinY2 = 0D;
            this.zg3.Size = new System.Drawing.Size(706, 237);
            this.zg3.TabIndex = 111;
            // 
            // EqualEnergyGraphForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 237);
            this.Controls.Add(this.zg3);
            this.Name = "EqualEnergyGraphForm";
            this.Text = "Equal Energy Graph Form";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EqualEnergyGraphForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl zg3;
    }
}