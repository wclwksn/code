using ChromeFormsWebBrowserExperiment;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChromeFormsWebBrowserExperiment
{
    public partial class ApplicationControl : UserControl
    {
        public ApplicationControl()
        {
            this.InitializeComponent();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            Console.WriteLine("Navigate!");

            this.webBrowser1.Navigate("http://example.com");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.webBrowser1.Navigate("http://192.168.1.100:27463/");

        }

    }
}