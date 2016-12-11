namespace AlgoNature.Components
{
    partial class LeafPalm
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelNature = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelNature
            // 
            this.panelNature.BackColor = System.Drawing.Color.Transparent;
            this.panelNature.Location = new System.Drawing.Point(0, 0);
            this.panelNature.Name = "panelNature";
            this.panelNature.Size = new System.Drawing.Size(500, 500);
            this.panelNature.TabIndex = 0;
            this.panelNature.Paint += new System.Windows.Forms.PaintEventHandler(this.panelLeaf_Paint);
            // 
            // LeafPalm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panelNature);
            this.Name = "LeafPalm";
            this.Size = new System.Drawing.Size(500, 500);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.LeafPalm_Paint);
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.LeafPalm_Layout);
            this.Move += new System.EventHandler(this.LeafPalm_Move);
            this.Resize += new System.EventHandler(this.LeafPalm_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel panelNature;
    }
}
