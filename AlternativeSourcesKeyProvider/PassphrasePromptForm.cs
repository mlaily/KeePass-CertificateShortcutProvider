using KeePass.UI;
using KeePassLib.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlternativeSourcesKeyProvider
{
    public partial class PassphrasePromptForm : Form
    {
        public SecureTextBoxEx textBox;
        public PassphrasePromptForm()
        {
            InitializeComponent();

            textBox = new SecureTextBoxEx() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { Close(); };
            Controls.Add(textBox);
            Controls.Add(confirmation);
            //Controls.Add(textLabel);
            AcceptButton = confirmation;
        }

        public static ProtectedString ShowDialogPrompt()
        {
            ProtectedString protectedResult;
            var prompt = new PassphrasePromptForm();
            if (prompt.ShowDialog() == DialogResult.OK)
            {
                protectedResult = prompt.textBox.TextEx;
            }
            else
            {
                protectedResult = new ProtectedString(true, "");
            }

            return protectedResult;
        }
    }
}
