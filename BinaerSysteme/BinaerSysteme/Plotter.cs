using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BinaerSysteme
{
    public partial class Plotter : Form
    {
        public Plotter()
        {
            InitializeComponent();
        }

        public double xStreckungsFaktor;
        public double yStreckungsFaktor;


        //private void CenterPictureBox(PictureBox picBox, Bitmap picImage)
        //{
        //    picBox.Image = picImage;
        //    picBox.Location = new Point((picBox.Parent.ClientSize.Width / 2) - (picImage.Width / 2),
        //                                (picBox.Parent.ClientSize.Height / 2) - (picImage.Height / 2));
        //    picBox.Refresh();
        //}


        public void MainForm()
        {
            DataTable dt = MainWindow.StoreData;

            // just make the window big enough to fit this graph...
            this.Width = 700; // 1200;
            this.Height = 350; // 800;

            // add 5 so the bars fit properly
            int x = 3; // the position of the X axis (..0)
            int y = 245; // the position of the Y axis (..240)            

            Bitmap bmp = new Bitmap(800, 800);
            
            //Bitmap bmp = new Bitmap(360, 290);

            

            MainWindow mainWindow = new MainWindow();

            Graphics g = Graphics.FromImage(bmp);


            //g.DrawLine(new Pen(Color.Red, 2), 5, 5, 5, 1000);
            g.DrawLine(new Pen(Color.Yellow, 2), 5, 5, 5, 250);

            //g.DrawLine(new Pen(Color.Red, 2), 5, 1000, 300, 1000);
            g.DrawLine(new Pen(Color.Yellow, 2), 5, 250, Convert.ToInt16(250 * xStreckungsFaktor) + 5, 250);

            g.DrawLine(new Pen(Color.Blue, 2), Convert.ToInt16(250 * xStreckungsFaktor) + 5, 250, 600, 250);            


            //for (int i = 0; i < 100; i++)
            foreach(DataRow dtRow in dt.Rows)                        
            {                          

                // let's draw a coordinate equivalent to (20,30) (20 up, 30 across)
                

                double xAdd = Convert.ToDouble(dtRow["RandomDecimalNumber"]) * 250 * xStreckungsFaktor;
                double yAdd = Convert.ToDouble(dtRow["BackConvertedDecimalNumber"]) * 250 * yStreckungsFaktor;

                //double xAdd = Convert.ToDouble(dtRow["RandomDecimalNumber"])*500;
                //double yAdd = Convert.ToDouble(dtRow["BackConvertedDecimalNumber"])*125;
                                
                int _xAdd = Convert.ToInt32(xAdd);
                int _yAdd = Convert.ToInt32(yAdd);

                g.DrawString("·", new Font("Calibri", 6), new SolidBrush(Color.Black), x + _xAdd, y - _yAdd);
                //g.DrawString("·", new Font("Calibri", 6), new SolidBrush(Color.Red), y - _yAdd, x + _xAdd);

                //-125 + 0.9*_xAdd
                

                //g.DrawString("·", new Font("Calibri", 12), new SolidBrush(Color.Black), y + _yAdd, x - _xAdd); //ALT250; ASCII für Punkt in der Mitte;
            }


 

            PictureBox display = new PictureBox();

            display.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            //display.SizeMode = PictureBoxSizeMode.AutoSize;
            //display.Anchor = AnchorStyles.None;
            //this.Controls.Add(display);
            //CenterPictureBox(display, bmp);

            //display.Dock = DockStyle.Fill;

            display.Width = 1200; // 360;
            display.Height = 900; // 290;
            this.Controls.Add(display);
            display.Image = bmp;


            //stretch!
            using (bmp)
            {
                var bmp2 = new Bitmap(display.Width, display.Height);
                using (g = Graphics.FromImage(bmp2))
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(bmp, new Rectangle(Point.Empty, bmp2.Size));
                    display.Image = bmp2;
                }
            }

            
            

            

        }

     
    }
}