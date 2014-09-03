using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using Microsoft.Research.DynamicDataDisplay;
using System.Windows.Threading;

namespace BinaerSysteme
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class Grapher : Window
    {
        private int p1, q1, p2, q2;
        private double m1, m2;
        private double convergency_m1, convergency_m2;

        private double Accuracity;
        private double JumpHeight;

        private Point P0;
        private Point P1;

        private List<Point> Graphpoints_DIA;
        private List<Point> Graphpoints_BOW;
        private List<Point> Graphpoints_RJF;
        private List<Point> Graphpoints_JMP_down;
        private List<Point> Graphpoints_JMP_up;

        private List<double> X_AxisParallels;
        private List<double> Y_AxisParallels;

        private SolidColorBrush CurrGraphColor;

        private DispatcherTimer DispatcherTimerP1;
        private DispatcherTimer DispatcherTimerQ1;
        private DispatcherTimer DispatcherTimerPQ1;

        private DispatcherTimer DispatcherTimerP2;
        private DispatcherTimer DispatcherTimerQ2;
        private DispatcherTimer DispatcherTimerPQ2;

        public Grapher()
        {
            InitializeComponent();

            this.InitDispatchers();

            this.p1 = 100;
            this.q1 = 200;

            this.p2 = 100;
            this.q2 = 200;

            this.m1 = 0.5;
            this.m2 = 0.5;

            this.convergency_m1 = (double)1;
            this.convergency_m2 = (double)1;

            this.Accuracity = 0.001;
            this.JumpHeight = 0.002;
            

            this.P0 = new Point(0, 0);
            this.P1 = new Point(1, 1);

            this.Graphpoints_DIA = new List<Point>();
            this.Graphpoints_BOW = new List<Point>();
            this.Graphpoints_RJF = new List<Point>();
            this.Graphpoints_JMP_down = new List<Point>();
            this.Graphpoints_JMP_up = new List<Point>();

            this.X_AxisParallels = new List<double>();
            this.Y_AxisParallels = new List<double>();

            this.CurrGraphColor = Brushes.Black;

            this.UpdatePQ();

            this.Create();
            this.UpdateDraw();
        }

        private void DrawSystem()
        {
            Line l;
            l = new Line();
            l.X1 = P0.X;
            l.Y1 = P0.Y;
            l.X2 = P1.X;
            l.Y2 = P0.Y;
            l.Stroke = Brushes.Black;
            l.StrokeThickness = 1;

            this.canvas.Children.Add(l);

            l = new Line();
            l.X1 = P0.X;
            l.Y1 = P0.Y;
            l.X2 = P0.X;
            l.Y2 = P1.Y;
            l.Stroke = Brushes.Black;
            l.StrokeThickness = 1;

            this.canvas.Children.Add(l);
        }

        private void Create()
        {
            this.Graphpoints_DIA.Clear();
            this.Graphpoints_BOW.Clear();
            this.Graphpoints_RJF.Clear();

            this.X_AxisParallels.Clear();
            this.Y_AxisParallels.Clear();

            JumpHeight = this.sliderJH.Value;

            double last_y = 0;
            int jump_counter = 0;

            for (double x = 0; x <= 1; x += this.Accuracity)
            {
                double tmp_y = f(x);
                this.Graphpoints_DIA.Add(new Point() { X = x, Y = tmp_y });
                this.Graphpoints_BOW.Add(new Point() { X = x, Y = tmp_y - x + 0.5 });
                //if (tmp_y < last_y)
                //{
                //    jump_counter++;
                //    this.X_AxisParallels.Add(last_y);
                //    this.X_AxisParallels.Add(tmp_y);
                //    this.Y_AxisParallels.Add(x);
                //}

                if (last_y - tmp_y > JumpHeight)
                {
                    jump_counter++;
                    this.X_AxisParallels.Add(last_y);
                    this.X_AxisParallels.Add(tmp_y);
                    this.Y_AxisParallels.Add(x);
                }

                if (tmp_y - last_y > JumpHeight)
                {
                    jump_counter++;
                    this.X_AxisParallels.Add(last_y);
                    this.X_AxisParallels.Add(tmp_y);
                    this.Y_AxisParallels.Add(x);
                }

                this.Graphpoints_RJF.Add(new Point { X = x, Y = (double)jump_counter });
                last_y = tmp_y;
            }

            if (jump_counter > 0)
            {
                List<Point> tmp_list = new List<Point>();
                foreach (Point p in this.Graphpoints_RJF)
                {
                    tmp_list.Add(new Point() { X = p.X, Y = p.Y / (double)jump_counter });
                }
                this.Graphpoints_RJF.Clear();
                foreach (Point p in tmp_list)
                {
                    this.Graphpoints_RJF.Add(new Point() { X = p.X, Y = p.Y - p.X + 0.5 });
                }
            }
        }

        private double f(double x)
        {
            String bin_string = this.DoubleToBinString(x, (double)this.m1);

            return this.BinStringToDouble(bin_string, (double)this.m2);
        }

        private String DoubleToBinString(double x, double m) { return DoubleToBinString(x, m, 1000); }

        private String DoubleToBinString(double x, double m, int digits)
        {
            if (x >= 1) return "1";
            x = x * this.convergency_m1;
            String ret = "0.";
            double val = 1;

            for (int d = 0; d < digits; d++)
            {
                val *= m;
                if (x >= val)
                {
                    ret += "1";
                    x -= val;
                }
                else
                {
                    ret += "0";
                }
            }
            System.Diagnostics.Debug.Write("BIN: " + ret + " ");
            return ret;
        }

        private double BinStringToDouble(String str, double m)
        {
            var bin_string = str.Split('.')[1];
            double ret = 0;
            double val = 1;
            foreach (Char ch in bin_string)
            {
                val *= m;
                if (ch == '1') ret += val;
            }
            System.Diagnostics.Debug.Write("VAL: " + ret + Environment.NewLine);
            return ret / this.convergency_m2;
        }

        private void DrawGraph()
        {
            this.stpLegendCol.Children.Clear();
            this.stpLegendName.Children.Clear();

            if (this.Y_AxisParallels.Count > 0)
            {
                this.CurrGraphColor = Brushes.Gray;
                this.stpLegendCol.Children.Add(new TextBlock() { Text = "f(x)", Background = this.CurrGraphColor });
                this.stpLegendName.Children.Add(new TextBlock() { Text = "MonoticityBreakings" });
                foreach (double d in this.X_AxisParallels) this.Add_X_parallels(d);
                foreach (double d in this.Y_AxisParallels) this.Add_Y_parallels(d);
            }

            this.CurrGraphColor = Brushes.Black;

            double y = 0.5;
            double canvas_y = P0.Y - y * (P0.Y - P1.Y);

            Line l = new Line();
            l.X1 = P0.X;
            l.Y1 = canvas_y;
            l.X2 = P1.X;
            l.Y2 = canvas_y;
            l.Stroke = this.CurrGraphColor;
            l.StrokeThickness = 1;
            l.Opacity = 1;

            this.canvas.Children.Add(l);

            this.CurrGraphColor = Brushes.Blue;
            this.stpLegendCol.Children.Add(new TextBlock() { Text = "f(x)", Background = this.CurrGraphColor });
            this.stpLegendName.Children.Add(new TextBlock() { Text = "MainGraph" });
            foreach (Point p in this.Graphpoints_DIA) this.AddPoint(p);

            this.CurrGraphColor = Brushes.Red;
            this.stpLegendCol.Children.Add(new TextBlock() { Text = "f(x)", Background = this.CurrGraphColor });
            this.stpLegendName.Children.Add(new TextBlock() { Text = "MainGraph - x + 0.5" });
            foreach (Point p in this.Graphpoints_BOW) this.AddPoint(p);

            //this.CurrGraphColor = Brushes.SeaGreen;
            //this.stpLegendCol.Children.Add(new TextBlock() { Text = "f(x)", Background = this.CurrGraphColor });
            //this.stpLegendName.Children.Add(new TextBlock() { Text = "RelativeJumpFrequency" });
            //foreach (Point p in this.Graphpoints_RJF) this.AddPoint(p);


            this.CurrGraphColor = Brushes.DarkSlateGray;
            this.stpLegendCol.Children.Add(new TextBlock() { Text = "f(x)", Background = this.CurrGraphColor });
            this.stpLegendName.Children.Add(new TextBlock() { Text = "JumpHeights" });
            Point last_point = new Point() { X = 0, Y = 0.00 };
            foreach (Point p in this.Graphpoints_DIA)
            {
                double diff = p.Y - last_point.Y;
                if (diff >= this.JumpHeight || diff <= -this.JumpHeight) { this.AddJump(p.X, diff); }
                last_point = p;
            }
        }

        private void AddJump(double x, double y)
        {
            double canvas_x1 = P0.X + x * (P1.X - P0.X);
            double canvas_y1 = P0.Y - (y + 0.5) * (P0.Y - P1.Y);
            double canvas_x2 = P0.X + x * (P1.X - P0.X);
            double canvas_y2 = P0.Y - 0.5 * (P0.Y - P1.Y);

            Line l = new Line();
            l.X1 = canvas_x1;
            l.Y1 = canvas_y1;
            l.X2 = canvas_x2;
            l.Y2 = canvas_y2;
            l.Stroke = this.CurrGraphColor;
            l.StrokeThickness = 1;

            this.canvas.Children.Add(l);
        }

        private void AddPoint(Point p) { this.AddPoint(p.X, p.Y); }

        private void AddPoint(double x, double y)
        {
            double canvas_x = P0.X + x * (P1.X - P0.X);
            double canvas_y = P0.Y - y * (P0.Y - P1.Y);

            Line l = new Line();
            l.X1 = canvas_x;
            l.Y1 = canvas_y;
            l.X2 = canvas_x + 1;
            l.Y2 = canvas_y + 1;
            l.Stroke = this.CurrGraphColor;
            l.StrokeThickness = 1;

            this.canvas.Children.Add(l);
        }

        private void Add_X_parallels(double y)
        {
            if (y > 1) return;

            double canvas_y = P0.Y - y * (P0.Y - P1.Y);

            Line l = new Line();
            l.X1 = P0.X;
            l.Y1 = canvas_y;
            l.X2 = P1.X;
            l.Y2 = canvas_y;
            l.Stroke = this.CurrGraphColor;
            l.StrokeThickness = 1;
            l.Opacity = 0.2;

            this.canvas.Children.Add(l);
        }

        private void Add_Y_parallels(double x)
        {
            if (x > 1) return;

            double canvas_x = P0.X + x * (P1.X - P0.X);

            Line l = new Line();
            l.X1 = canvas_x;
            l.Y1 = P0.Y;
            l.X2 = canvas_x;
            l.Y2 = P1.Y;
            l.Stroke = this.CurrGraphColor;
            l.StrokeThickness = 1;
            l.Opacity = 0.2;

            this.canvas.Children.Add(l);
        }

        private void UpdateDraw()
        {
            this.canvas.Children.Clear();

            this.DrawSystem();
            this.DrawGraph();
        }

        ///// Helpers \\\\\
        private bool IsExspectedCharVal(Char ch)
        {
            switch (ch)
            {
                case '0': return true;
                case '1': return true;
                case '2': return true;
                case '3': return true;
                case '4': return true;
                case '5': return true;
                case '6': return true;
                case '7': return true;
                case '8': return true;
                case '9': return true;
            }
            return false;
        }

        private void UpdatePQ()
        {
            if (this.q1 == 0 || this.q2 == 0)
            {
                MessageBox.Show("Divisor is 0");
                return;
            }
            this.m1 = (double)this.p1 / (double)this.q1;
            this.m2 = (double)this.p2 / (double)this.q2;

            System.Diagnostics.Debug.WriteLine("M1: " + this.m1);
            System.Diagnostics.Debug.WriteLine("M2: " + this.m2);

            this.convergency_m1 = (1 / (1 - this.m1)) - 1;
            this.convergency_m2 = (1 / (1 - this.m2)) - 1;

            System.Diagnostics.Debug.WriteLine("M1 CON: " + this.convergency_m1);
            System.Diagnostics.Debug.WriteLine("M2 CON: " + this.convergency_m2);
        }

        private void InitDispatchers()
        {
            this.DispatcherTimerP1 = new System.Windows.Threading.DispatcherTimer();
            this.DispatcherTimerQ1 = new System.Windows.Threading.DispatcherTimer();
            this.DispatcherTimerPQ1 = new System.Windows.Threading.DispatcherTimer();

            this.DispatcherTimerP2 = new System.Windows.Threading.DispatcherTimer();
            this.DispatcherTimerQ2 = new System.Windows.Threading.DispatcherTimer();
            this.DispatcherTimerPQ2 = new System.Windows.Threading.DispatcherTimer();

            this.DispatcherTimerP1.Interval = new TimeSpan(0, 0, 0, 0, 500);
            this.DispatcherTimerQ1.Interval = new TimeSpan(0, 0, 0, 0, 500);
            this.DispatcherTimerPQ1.Interval = new TimeSpan(0, 0, 0, 0, 500);

            this.DispatcherTimerP2.Interval = new TimeSpan(0, 0, 0, 0, 500);
            this.DispatcherTimerQ2.Interval = new TimeSpan(0, 0, 0, 0, 500);
            this.DispatcherTimerPQ2.Interval = new TimeSpan(0, 0, 0, 0, 500);

            this.DispatcherTimerP1.Tick += new EventHandler(this.e_change_P1_update);
            this.DispatcherTimerQ1.Tick += new EventHandler(this.e_change_Q1_update);
            this.DispatcherTimerPQ1.Tick += new EventHandler(this.e_change_PQ1_update);

            this.DispatcherTimerP2.Tick += new EventHandler(this.e_change_P2_update);
            this.DispatcherTimerQ2.Tick += new EventHandler(this.e_change_Q2_update);
            this.DispatcherTimerPQ2.Tick += new EventHandler(this.e_change_PQ2_update);
        }

        ///// Event handling \\\\\
        private void e_p1_key_up(object sender, KeyEventArgs e) { this.DispatcherTimerP1.Start(); }
        private void e_q1_key_up(object sender, KeyEventArgs e) { this.DispatcherTimerQ1.Start(); }
        private void e_pq1_key_up(object sender, KeyEventArgs e) { this.DispatcherTimerPQ1.Start(); }
        private void e_slider1_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.tbP1.Text = "niu";
            this.tbQ1.Text = "niu";
            this.tbPQ1.Text = this.slider1.Value.ToString();
            this.m1 = this.slider1.Value;
            this.convergency_m1 = (1 / (1 - this.m1)) - 1;
        }

        private void e_p2_key_up(object sender, KeyEventArgs e) { this.DispatcherTimerP2.Start(); }
        private void e_q2_key_up(object sender, KeyEventArgs e) { this.DispatcherTimerQ2.Start(); }
        private void e_pq2_key_up(object sender, KeyEventArgs e) { this.DispatcherTimerPQ2.Start(); }
        private void e_slider2_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.tbP2.Text = "niu";
            this.tbQ2.Text = "niu";
            this.tbPQ2.Text = this.slider2.Value.ToString();
            this.m2 = this.slider2.Value;
            this.convergency_m2 = (1 / (1 - this.m2)) - 1;
        }

        private void e_change_P1_update(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();

            var txt = this.tbP1.Text;
            var proofed_txt = "";
            foreach (Char ch in txt) if (this.IsExspectedCharVal(ch)) proofed_txt += ch;
            this.p1 = int.Parse(proofed_txt);
            this.UpdatePQ();
            this.slider1.Value = this.m1;
            this.tbPQ1.Text = this.m1.ToString();
            this.Create();
            this.UpdateDraw();
        }

        private void e_change_Q1_update(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();

            var txt = this.tbQ1.Text;
            var proofed_txt = "";
            foreach (Char ch in txt) if (this.IsExspectedCharVal(ch)) proofed_txt += ch;
            this.q1 = int.Parse(proofed_txt);
            this.UpdatePQ();
            this.slider1.Value = this.m1;
            this.tbPQ1.Text = this.m1.ToString();
            this.Create();
            this.UpdateDraw();
        }

        private void e_change_PQ1_update(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();

            var txt = this.tbPQ1.Text;
            this.m1 = double.Parse(txt);
            this.slider1.Value = this.m1;
            this.tbP1.Text = "niu";
            this.tbQ1.Text = "niu";
            this.Create();
            this.UpdateDraw();
        }

        private void e_slider1_mouse_up(object sender, MouseButtonEventArgs e)
        {
            this.Create();
            this.UpdateDraw();
        }

        private void e_change_P2_update(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();

            var txt = this.tbP2.Text;
            var proofed_txt = "";
            foreach (Char ch in txt) if (this.IsExspectedCharVal(ch)) proofed_txt += ch;
            this.p2 = int.Parse(proofed_txt);
            this.UpdatePQ();
            this.tbPQ2.Text = this.m2.ToString();
            this.slider2.Value = this.m2;
            this.Create();
            this.UpdateDraw();
        }

        private void e_change_Q2_update(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();

            var txt = this.tbQ2.Text;
            var proofed_txt = "";
            foreach (Char ch in txt) if (this.IsExspectedCharVal(ch)) proofed_txt += ch;
            this.q2 = int.Parse(proofed_txt);
            this.UpdatePQ();
            this.slider2.Value = this.m2;
            this.tbPQ2.Text = this.m2.ToString();
            this.Create();
            this.UpdateDraw();
        }

        private void e_change_PQ2_update(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();

            var txt = this.tbPQ2.Text;
            this.m2 = double.Parse(txt);
            this.slider2.Value = this.m2;
            this.tbP2.Text = "niu";
            this.tbQ2.Text = "niu";
            this.Create();
            this.UpdateDraw();
        }

        private void e_slider2_mouse_up(object sender, MouseButtonEventArgs e)
        {
            this.Create();
            this.UpdateDraw();
        }

        private void e_size_changed(object sender, SizeChangedEventArgs e)
        {
            this.P0.X = 10;
            this.P0.Y = this.canvas.ActualHeight - 10;
            this.P1.X = this.canvas.ActualWidth - 10;
            this.P1.Y = 10;

            this.UpdateDraw();
        }

        private void e_mouse_move(object sender, MouseEventArgs e)
        {
            Point mouse_pos = Mouse.GetPosition(this.canvas);

            double rel_X = (mouse_pos.X - 10) / (this.canvas.ActualWidth - 20);
            double rel_Y = (this.canvas.ActualHeight - mouse_pos.Y - 10) / (this.canvas.ActualHeight - 20);

            this.txtMousePos.Text = "REL( " + Math.Round(rel_X, 3) + " / " + Math.Round(rel_Y, 3) + " )";
            this.txtMousePos.Text += "   ABS( " + Math.Round(rel_X * this.convergency_m1, 3) + " / " + Math.Round(rel_Y * this.convergency_m2, 3) + " )";
        }

        private void e_mouse_leave(object sender, MouseEventArgs e)
        {
            this.txtMousePos.Text = "Mouse out of system";
        }

        private void sliderJH_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Create();
            this.UpdateDraw();
        }
    }
}
