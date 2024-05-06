using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoScreenSharing
{
    public partial class Secundarios : Form
    {
        public Secundarios()
        {
            InitializeComponent();
        }

        private void Secundarios_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = (this.Location.X).ToString();
            label2.Text = this.Location.Y.ToString();
            label3.Text = (this.Location.X).ToString();
            label4.Text = (this.Location.Y + this.Height).ToString();
            label5.Text = (this.Location.X + this.Width).ToString();
            label6.Text = this.Location.Y.ToString();
            label7.Text = (this.Location.X + this.Width).ToString();
            label8.Text = (this.Location.Y + this.Height).ToString();

        }
    }
}
