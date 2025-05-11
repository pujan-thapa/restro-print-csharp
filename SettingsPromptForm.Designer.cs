namespace RestroPrint
{
    partial class SettingsPromptForm
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
            this.txtAppKey = new System.Windows.Forms.TextBox();
            this.txtCluster = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.labelAppKey = new System.Windows.Forms.Label();
            this.labelCluster = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtAppKey
            // 
            this.txtAppKey.Location = new System.Drawing.Point(120, 30);
            this.txtAppKey.Name = "txtAppKey";
            this.txtAppKey.Size = new System.Drawing.Size(200, 22);
            this.txtAppKey.TabIndex = 0;
            // 
            // txtCluster
            // 
            this.txtCluster.Location = new System.Drawing.Point(120, 70);
            this.txtCluster.Name = "txtCluster";
            this.txtCluster.Size = new System.Drawing.Size(200, 22);
            this.txtCluster.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(120, 110);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 30);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(245, 110);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // labelAppKey
            // 
            this.labelAppKey.AutoSize = true;
            this.labelAppKey.Location = new System.Drawing.Point(40, 33);
            this.labelAppKey.Name = "labelAppKey";
            this.labelAppKey.Size = new System.Drawing.Size(55, 17);
            this.labelAppKey.TabIndex = 4;
            this.labelAppKey.Text = "App Key:";
            // 
            // labelCluster
            // 
            this.labelCluster.AutoSize = true;
            this.labelCluster.Location = new System.Drawing.Point(40, 73);
            this.labelCluster.Name = "labelCluster";
            this.labelCluster.Size = new System.Drawing.Size(54, 17);
            this.labelCluster.TabIndex = 5;
            this.labelCluster.Text = "Cluster:";
            // 
            // SettingsPromptForm
            // 
            this.ClientSize = new System.Drawing.Size(400, 160);
            this.Controls.Add(this.labelCluster);
            this.Controls.Add(this.labelAppKey);
            this.Controls.Add(this.txtAppKey);
            this.Controls.Add(this.txtCluster);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Name = "SettingsPromptForm";
            this.Text = "Enter Pusher Settings";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtAppKey;
        private System.Windows.Forms.TextBox txtCluster;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label labelAppKey;
        private System.Windows.Forms.Label labelCluster;
    }
}
