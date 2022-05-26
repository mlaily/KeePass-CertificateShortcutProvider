using System.Data;
using System.Security.Cryptography;
using KeePass.App;
using KeePass.UI;
using KeePassLib.Utility;

namespace CertificateShortcutProvider;

public partial class KeyCreationAdvancedSettingsForm : Form
{
    public AllowedRSAEncryptionPadding SelectedRSAEncryptionPadding => rsaEncryptionPaddingCombobox.SelectedItem as AllowedRSAEncryptionPadding;

    public KeyCreationAdvancedSettingsForm()
    {
        InitializeComponent();

        rsaEncryptionPaddingCombobox.DisplayMember = nameof(AllowedRSAEncryptionPadding.DisplayName);
        rsaEncryptionPaddingCombobox.Items.AddRange(AllowedRSAEncryptionPadding.List);
        rsaEncryptionPaddingCombobox.SelectedItem = AllowedRSAEncryptionPadding.Default;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        GlobalWindowManager.AddWindow(this);

        Icon = AppIcons.Default;
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        base.OnFormClosed(e);

        GlobalWindowManager.RemoveWindow(this);
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
        }
        base.Dispose(disposing);
    }

    private void okButton_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
    }
}

