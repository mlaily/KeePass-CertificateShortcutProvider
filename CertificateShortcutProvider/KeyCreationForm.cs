using KeePass.App;
using KeePass.UI;
using KeePassLib.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CertificateShortcutProvider
{
    public partial class KeyCreationForm : Form
    {
        private bool _initializing = true;
        private X509Certificate2 _selectedCertificate;

        public KeyCreationForm(string keyFileDefaultLocation)
        {
            InitializeComponent();

            keyFileLocationTextBox.Text = keyFileDefaultLocation;
        }

        protected override void OnLoad(EventArgs e)
        {
            _initializing = true;

            base.OnLoad(e);

            GlobalWindowManager.AddWindow(this);

            Icon = AppIcons.Default;

            _initializing = false;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (secureTextBox.CanFocus) UIUtil.ResetFocus(secureTextBox, this, true);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            GlobalWindowManager.RemoveWindow(this);
        }

        private void hidePasswordCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool hide = hidePasswordCheckBox.Checked;
            if (!hide && !AppPolicy.Try(AppPolicyId.UnhidePasswords))
            {
                hidePasswordCheckBox.Checked = true;
                return;
            }

            secureTextBox.EnableProtection(hide);

            if (!_initializing) UIUtil.SetFocus(secureTextBox, this);
        }

        private void browseCertStoreButton_Click(object sender, EventArgs e)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            using (store)
            {
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                var selection = X509Certificate2UI.SelectFromCollection(
                    store.Certificates,
                    "Select a certificate",
                    "Select the certificate you want to use as your authentication method.",
                    X509SelectionFlag.SingleSelection)
                    .Cast<X509Certificate2>().SingleOrDefault();

                _selectedCertificate = selection;
                certNameTextBox.Text = selection?.GetNameInfo(X509NameType.SimpleName, forIssuer: false);
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialogEx("Select a location for your new Certificate Shortcut Provider Key file.");
            sfd.Filter = $"Certificate Shortcut Provider Key files (*{CertificateShortcutProvider.DefaultKeyExtension})|*{CertificateShortcutProvider.DefaultKeyExtension}|All files (*.*)|*.*";
            sfd.FileName = keyFileLocationTextBox.Text;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                keyFileLocationTextBox.Text = sfd.FileName;
            }
        }

        private void saveFileButton_Click(object sender, EventArgs e)
        {
            if (secureTextBox.TextLength <= 0 || _selectedCertificate == null || string.IsNullOrWhiteSpace(keyFileLocationTextBox.Text))
            {
                MessageService.ShowWarning("All the fields are required.");
                return;
            }

            bool overwrite = false;
            if (File.Exists(keyFileLocationTextBox.Text))
            {
                overwrite = MessageService.AskYesNo($"The file '{keyFileLocationTextBox.Text}' already exists. Overwrite?", "Warning");
                if (overwrite != true)
                {
                    return;
                }
            }

            var cspKey = CryptoHelpers.EncryptPassphrase(_selectedCertificate, secureTextBox.TextEx);

            using (var fs = new FileStream(keyFileLocationTextBox.Text, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.Read))
            {
                XmlUtilEx.Serialize(fs, cspKey);
            }

            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                _selectedCertificate?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

