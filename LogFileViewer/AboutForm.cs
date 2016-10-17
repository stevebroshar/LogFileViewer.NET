using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace Scb
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            appNameLabel.Text = fileVersionInfo.FileDescription;
            versionLabel.Text = "Version " + fileVersionInfo.FileVersion;
            copyrightLabel.Text = fileVersionInfo.LegalCopyright;
        }
    }
}
