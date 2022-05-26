using KeePass.UI;

namespace CertificateShortcutProvider
{
    partial class KeyCreationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeyCreationForm));
            this.secureTextBox = new KeePass.UI.SecureTextBoxEx();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.hidePasswordCheckBox = new System.Windows.Forms.CheckBox();
            this.saveFileButton = new System.Windows.Forms.Button();
            this.browseCertStoreButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.certNameTextBox = new System.Windows.Forms.TextBox();
            this.keyFileLocationTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.browseButton = new System.Windows.Forms.Button();
            this.advancedSettingsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // secureTextBox
            // 
            this.secureTextBox.Location = new System.Drawing.Point(15, 196);
            this.secureTextBox.Name = "secureTextBox";
            this.secureTextBox.Size = new System.Drawing.Size(419, 20);
            this.secureTextBox.TabIndex = 0;
            this.secureTextBox.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(403, 117);
            this.label1.TabIndex = 1;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 180);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Enter your master password:";
            // 
            // hidePasswordCheckBox
            // 
            this.hidePasswordCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.hidePasswordCheckBox.Checked = true;
            this.hidePasswordCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hidePasswordCheckBox.Location = new System.Drawing.Point(440, 194);
            this.hidePasswordCheckBox.Name = "hidePasswordCheckBox";
            this.hidePasswordCheckBox.Size = new System.Drawing.Size(32, 23);
            this.hidePasswordCheckBox.TabIndex = 3;
            this.hidePasswordCheckBox.Text = "***";
            this.hidePasswordCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.hidePasswordCheckBox.UseVisualStyleBackColor = true;
            this.hidePasswordCheckBox.CheckedChanged += new System.EventHandler(this.hidePasswordCheckBox_CheckedChanged);
            // 
            // saveFileButton
            // 
            this.saveFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveFileButton.Location = new System.Drawing.Point(271, 363);
            this.saveFileButton.Name = "saveFileButton";
            this.saveFileButton.Size = new System.Drawing.Size(204, 23);
            this.saveFileButton.TabIndex = 8;
            this.saveFileButton.Text = "Save the key file and close";
            this.saveFileButton.UseVisualStyleBackColor = true;
            this.saveFileButton.Click += new System.EventHandler(this.saveFileButton_Click);
            // 
            // browseCertStoreButton
            // 
            this.browseCertStoreButton.Location = new System.Drawing.Point(326, 242);
            this.browseCertStoreButton.Name = "browseCertStoreButton";
            this.browseCertStoreButton.Size = new System.Drawing.Size(146, 23);
            this.browseCertStoreButton.TabIndex = 11;
            this.browseCertStoreButton.Text = "Browse certificate store...";
            this.browseCertStoreButton.UseVisualStyleBackColor = true;
            this.browseCertStoreButton.Click += new System.EventHandler(this.browseCertStoreButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 228);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Choose a certificate:";
            // 
            // certNameTextBox
            // 
            this.certNameTextBox.Location = new System.Drawing.Point(15, 244);
            this.certNameTextBox.Name = "certNameTextBox";
            this.certNameTextBox.ReadOnly = true;
            this.certNameTextBox.Size = new System.Drawing.Size(305, 20);
            this.certNameTextBox.TabIndex = 13;
            // 
            // keyFileLocationTextBox
            // 
            this.keyFileLocationTextBox.Location = new System.Drawing.Point(15, 296);
            this.keyFileLocationTextBox.Name = "keyFileLocationTextBox";
            this.keyFileLocationTextBox.Size = new System.Drawing.Size(379, 20);
            this.keyFileLocationTextBox.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 280);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(375, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Choose the new key file location (default location will be loaded automatically):" +
    "";
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(400, 294);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 16;
            this.browseButton.Text = "Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // advancedSettingsButton
            // 
            this.advancedSettingsButton.Location = new System.Drawing.Point(15, 333);
            this.advancedSettingsButton.Name = "advancedSettingsButton";
            this.advancedSettingsButton.Size = new System.Drawing.Size(124, 23);
            this.advancedSettingsButton.TabIndex = 17;
            this.advancedSettingsButton.Text = "Advanced settings...";
            this.advancedSettingsButton.UseVisualStyleBackColor = true;
            this.advancedSettingsButton.Click += new System.EventHandler(this.advancedSettingsButton_Click);
            // 
            // KeyCreationForm
            // 
            this.AcceptButton = this.saveFileButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 398);
            this.Controls.Add(this.advancedSettingsButton);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.keyFileLocationTextBox);
            this.Controls.Add(this.certNameTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.browseCertStoreButton);
            this.Controls.Add(this.saveFileButton);
            this.Controls.Add(this.hidePasswordCheckBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.secureTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "KeyCreationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Certificate Shortcut Provider - Key Creation";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SecureTextBoxEx secureTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox hidePasswordCheckBox;
        private System.Windows.Forms.Button saveFileButton;
        private System.Windows.Forms.Button browseCertStoreButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox certNameTextBox;
        private System.Windows.Forms.TextBox keyFileLocationTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button browseButton;
        private Button advancedSettingsButton;
    }
}
