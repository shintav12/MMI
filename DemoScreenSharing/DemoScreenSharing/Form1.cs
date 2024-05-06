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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<Secundarios> pantallasSecundarias;
        Cuadrado circulo;
        bool mover = false;
        List<int> limitesX;
        List<int> limitesY;
        List<Point> limite;
        private void Form1_Load(object sender, EventArgs e)
        {
            pantallasSecundarias  = new List<Secundarios>();
            List<Screen> pantallas = Screen.AllScreens.ToList();
            this.Text = "Principal";
            for (int i = 1; i < pantallas.Count; i++)
            {
                Secundarios pantalla = new Secundarios();
                pantalla.StartPosition = FormStartPosition.Manual;
                pantalla.FormBorderStyle = FormBorderStyle.None;
                pantalla.DesktopLocation = pantallas[i].WorkingArea.Location;
                pantalla.WindowState = FormWindowState.Maximized;
                pantalla.Tag = (i).ToString();
                pantalla.Text = "Pantalla Secundaria: " + (i).ToString();
                if (i != 0)
                {
                    //pantalla..X = pantallasSecundarias[i - 1].Location.X + pantallasSecundarias[i - 1].Width + 10;
                    //pantalla.Location.Y = pantallasSecundarias[i - 1].Location.Y + pantallasSecundarias[i - 1].Width + 10;
                }
                pantallasSecundarias.Add(pantalla);
                
            }

             limite = new List<Point>();

             limitesX = new List<int>();
             limitesY = new List<int>();

            limitesX.Add(pantallasSecundarias[1].Location.X);
            limitesX.Add(pantallasSecundarias[0].Location.X + pantallasSecundarias[1].Width + 2880);
            limitesX.Add(pantallasSecundarias[1].Location.X);
            limitesX.Add(pantallasSecundarias[0].Location.X + pantallasSecundarias[1].Width + 2880);

            limitesY.Add(pantallasSecundarias[0].Location.Y);
            limitesY.Add(pantallasSecundarias[0].Location.Y + pantallasSecundarias[0].Height);
            limitesY.Add(pantallasSecundarias[1].Location.Y);
            limitesY.Add(pantallasSecundarias[1].Location.Y + pantallasSecundarias[1].Height);

            limite.Add(new Point(limitesX[0], limitesY[0]));
            limite.Add(new Point(limitesX[1], limitesY[1]));
            limite.Add(new Point(limitesX[2], limitesY[2]));
            limite.Add(new Point(limitesX[3], limitesY[3]));

            circulo = new Cuadrado(limite);

            foreach (Secundarios c in pantallasSecundarias)
            {
                c.Show(); 
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            limitesX[0] = pantallasSecundarias[1].Location.X;
            limitesX[2] = pantallasSecundarias[1].Location.X;
            limitesX[1] = pantallasSecundarias[0].Location.X + 2880 + pantallasSecundarias[0].Width;
            limitesX[3] = pantallasSecundarias[0].Location.X + 2880 + pantallasSecundarias[0].Width;

            limitesY[0]=pantallasSecundarias[0].Location.Y;
            limitesY[1]=pantallasSecundarias[0].Location.Y;
            limitesY[2]=pantallasSecundarias[1].Location.Y + pantallasSecundarias[1].Height;
            limitesY[3]=pantallasSecundarias[1].Location.Y + pantallasSecundarias[1].Height;

            limite[0]=new Point(limitesX[0], limitesY[0]);
            limite[1]=new Point(limitesX[1], limitesY[1]);
            limite[2]=new Point(limitesX[2], limitesY[2]);
            limite[3]=new Point(limitesX[3], limitesY[3]);

            circulo.limites = limite;

            label1.Text = circulo.x.ToString();
            label2.Text = circulo.y.ToString();

            if (mover)
            {
                circulo.mover();
                foreach (Secundarios c in pantallasSecundarias)
                {
                    circulo.pintar(c);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mover = true;
        }
    }
}
