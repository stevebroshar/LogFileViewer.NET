using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace LogFileViewer
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
