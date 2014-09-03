using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;


namespace BinaerSysteme
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private DispatcherTimer DispatcherTimer;       
        private Plotter plotter;        
        private static MainWindow _instance;
        public static DataTable StoreData;

        
        
        public MainWindow()
        {
            this.DispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            this.DispatcherTimer.Tick += new EventHandler(this.e_change_update);
            this.DispatcherTimer.Interval = new TimeSpan(0, 0, 1);


            InitializeComponent();
            _instance = this;
        }

        private void e_change_update(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();
            this.xetResult();
        }

        public static MainWindow GI
        {
            get
            {
                if (_instance == null) _instance = new MainWindow();
                return _instance;
            }
        }

        private void PlotIfChecked()
        {
           if (this.CheckBoxPlotPreviews.IsChecked.Value)
           {
               double _xStreckungsFaktor = Convert.ToDouble(TextBoxXStreckungsfaktor.Text.Replace(".", ","));
               double _yStreckungsFaktor = Convert.ToDouble(TextBoxYStreckungsfaktor.Text.Replace(".", ","));
               this.plotter = new Plotter() { xStreckungsFaktor = _xStreckungsFaktor, yStreckungsFaktor = _yStreckungsFaktor };
               plotter.Show();
               plotter.MainForm(); 
           }
            
        }

        private void PlotCloseTry()
        {
            try 
            { 
                this.plotter.Close(); 
            }
            catch 
            { 
            }
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {            
            ClearTextBoxes();
            CreateComparisonTable();

            PlotCloseTry();                      
            PlotIfChecked(); 
            
                            
        }

        private void ButtonGrapher_Click(object sender, RoutedEventArgs e)
        {
            Grapher grapher = new Grapher();
            grapher.Show();
        }  

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            ClearTextBoxes();

            PlotCloseTry();            
        }

        private void ButtonDefault_Click(object sender, RoutedEventArgs e)
        {
            this.SliderEpsilon.Value = 0.01;
            this.SliderPQ.Value = 0.5;
            this.SliderP2Q2.Value = 0.555;

            this.SliderAnzahlNachkommastellen.Value = 50;
            this.SliderAnzahlDerPunkte.Value = 100;

            this.SliderXStreckungsfaktor.Value = 0.8;
            this.SliderYStreckungsfaktor.Value = 0.8;

            this.TextBoxEpsilonMax.Text = ((0.3).ToString()).Replace(",", ".");

            ClearTextBoxes();
             
        }    

        private void ClearTextBoxes()
        {
            this.TextBoxVergleichstabelle1.Clear();
            this.TextBoxVergleichstabelle2.Clear();
            this.TextBoxVergleichstabelle3.Clear();
            this.TextBoxTotalNumberOfJumps.Clear();
            this.TextBoxNumberOfJumpsPercentage.Clear();
            this.TextBoxSprungstellen.Clear();
        }

        private void ClearJumpTextBoxes()
        {
            this.TextBoxTotalNumberOfJumps.Clear();
            this.TextBoxNumberOfJumpsPercentage.Clear();
            this.TextBoxSprungstellen.Clear();
        }

        private static readonly Random random = new Random();

        private static double RandomNumberBetween(double minValue, double maxValue)
       {
          var next = random.NextDouble();
          return minValue + (next * (maxValue - minValue));
       }

        private string ConvertUnitIntervalDecimalToBinary(double doubleNumber, double pDurchQ)
        {
            double num = doubleNumber;
            string binaryString = "0.";

            double start = pDurchQ;

            int anzahlNachkommastellenBinaer = Convert.ToInt16(this.TextBoxAnzahlNachkommastellenBinaer.Text);

            for (int i = 0; i < anzahlNachkommastellenBinaer; i++)
            {
                //System.Diagnostics.Debug.WriteLine(i + " -> start = " + start);

                if (num == 0)
                {
                    binaryString = binaryString + "0";
                    break;
                }

                if (num < start)
                {
                    binaryString = binaryString + "0";
                    
                }
                else
                {
                    binaryString = binaryString + "1";
                    if (start == num) break;

                    num = num - start;
                }
                //start = start / 2;
                start = start * pDurchQ;

                //num = num - start;               
                
            }

            return binaryString;
        }

        private double ConvertUnitIntervalBinaryToDecimal(string binaryNumber)
        {
            string str = binaryNumber;

            int binaryNumberLength = binaryNumber.Length;

            double decimalNumber = 0;

            for (int i = 2; i < binaryNumberLength; i++)
            {
                if (str.Substring(i, 1) == "1")
                {
                    double P2DurchQ2Inv = 1 / Convert.ToDouble(this.TextBoxP2DurchQ2.Text.Replace(".",","));
                    double addNumber = (1 / (Math.Pow(P2DurchQ2Inv,((double)i - 1))));
                    decimalNumber = decimalNumber + addNumber;
                }
                
            }

            return decimalNumber;

        }       

        private void UpdateJumps()
        {
            string epsilonJumpPositions = string.Empty;


            double pdq = Convert.ToDouble(this.TextBoxPDurchQ.Text.Replace(".", ","));
            double increment = 1 / Convert.ToDouble(this.TextBoxAnzahlDerPunkte.Text.Replace(".", ","));        
            int TotalNumberOfJumps = 0;

            for (double i = 0; i < 1; i += increment)
            {
                double rnd = i;
                double rndPlusIncrement = i + increment;

                string rndConvertedToBinary = ConvertUnitIntervalDecimalToBinary(rnd, pdq);
                double binaryConvertedToDecimal = ConvertUnitIntervalBinaryToDecimal(rndConvertedToBinary);
                string rndPlusIncrementConvertedToBinary = ConvertUnitIntervalDecimalToBinary(rndPlusIncrement, pdq);
                double binaryConvertedToDecimalPlusIncrement = ConvertUnitIntervalBinaryToDecimal(rndPlusIncrementConvertedToBinary);

                double differenceOfBinaryConvertedToDecimalPlusIncrementBinaryConvertedToDecimal = binaryConvertedToDecimalPlusIncrement - binaryConvertedToDecimal;

                if (Math.Abs(differenceOfBinaryConvertedToDecimalPlusIncrementBinaryConvertedToDecimal) > Convert.ToDouble(TextBoxEpsilonJump.Text.Replace(".", ",")))
                {
                    TotalNumberOfJumps = TotalNumberOfJumps + 1;
                    epsilonJumpPositions = epsilonJumpPositions + EpsilonJumpPositions((int)(i * Convert.ToDouble(this.TextBoxAnzahlDerPunkte.Text.Replace(".", ",")) + 1)) + "▲= " + differenceOfBinaryConvertedToDecimalPlusIncrementBinaryConvertedToDecimal + "\n";

                }

                this.TextBoxTotalNumberOfJumps.Text = Convert.ToString(TotalNumberOfJumps);

                double numberOfJumpsPercentage = Convert.ToDouble(TotalNumberOfJumps) / Convert.ToDouble(TextBoxAnzahlDerPunkte.Text);

                this.TextBoxNumberOfJumpsPercentage.Text = Convert.ToString(numberOfJumpsPercentage);

                this.TextBoxSprungstellen.Text = "" + epsilonJumpPositions;

            }

        }
        
        public void CreateComparisonTable()
        {
            string epsilonJumpPositions = string.Empty;

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("RandomDecimalNumber");
            dataTable.Columns.Add("ConvertedBinary");
            dataTable.Columns.Add("BackConvertedDecimalNumber");

            double pdq = Convert.ToDouble(this.TextBoxPDurchQ.Text.Replace(".",","));

            double increment = 1 / Convert.ToDouble(this.TextBoxAnzahlDerPunkte.Text.Replace(".", ","));

            int TotalNumberOfJumps = 0;

            
            for (double i = 0; i < 1; i+=increment)
            {
                //double rnd = RandomNumberBetween(0, 1);
                double rnd = i;


                double rndPlusIncrement = i + increment;                
                

                string rndConvertedToBinary = ConvertUnitIntervalDecimalToBinary(rnd,pdq);
                double binaryConvertedToDecimal = ConvertUnitIntervalBinaryToDecimal(rndConvertedToBinary);

                string rndPlusIncrementConvertedToBinary = ConvertUnitIntervalDecimalToBinary(rndPlusIncrement,pdq);
                double binaryConvertedToDecimalPlusIncrement = ConvertUnitIntervalBinaryToDecimal(rndPlusIncrementConvertedToBinary);

                double differenceOfBinaryConvertedToDecimalPlusIncrementBinaryConvertedToDecimal = binaryConvertedToDecimalPlusIncrement - binaryConvertedToDecimal;

                if (Math.Abs(differenceOfBinaryConvertedToDecimalPlusIncrementBinaryConvertedToDecimal) > Convert.ToDouble(TextBoxEpsilonJump.Text.Replace(".", ",")))
                {
                    TotalNumberOfJumps = TotalNumberOfJumps + 1;
                    epsilonJumpPositions = epsilonJumpPositions + EpsilonJumpPositions((int)(i * Convert.ToDouble(this.TextBoxAnzahlDerPunkte.Text.Replace(".", ",")) + 1)) + "▲= " + differenceOfBinaryConvertedToDecimalPlusIncrementBinaryConvertedToDecimal + "\n";
                } 


                DataRow dataRow = dataTable.NewRow();
                dataRow["RandomDecimalNumber"] = rnd;
                dataRow["ConvertedBinary"] = rndConvertedToBinary; 
                dataRow["BackConvertedDecimalNumber"] = binaryConvertedToDecimal;

                dataTable.Rows.Add(dataRow);

                this.TextBoxVergleichstabelle1.Text = this.TextBoxVergleichstabelle1.Text + (int)(i * Convert.ToDouble(this.TextBoxAnzahlDerPunkte.Text.Replace(".", ",")) + 1) + ") " + (string)dataRow["RandomDecimalNumber"] + "\n";
                this.TextBoxVergleichstabelle2.Text = this.TextBoxVergleichstabelle2.Text + (int)(i * Convert.ToDouble(this.TextBoxAnzahlDerPunkte.Text.Replace(".", ",")) + 1) + ") " + (string)dataRow["ConvertedBinary"] + "\n";
                this.TextBoxVergleichstabelle3.Text = this.TextBoxVergleichstabelle3.Text + (int)(i * Convert.ToDouble(this.TextBoxAnzahlDerPunkte.Text.Replace(".", ",")) + 1) + ") " + (string)dataRow["BackConvertedDecimalNumber"] + "\n";

            }

            this.TextBoxSprungstellen.Text = this.TextBoxSprungstellen.Text + epsilonJumpPositions;

            StoreData = dataTable;

            this.TextBoxTotalNumberOfJumps.Text = this.TextBoxTotalNumberOfJumps.Text + TotalNumberOfJumps;

            double numberOfJumpsPercentage = Convert.ToDouble(TotalNumberOfJumps) / Convert.ToDouble(TextBoxAnzahlDerPunkte.Text);

            this.TextBoxNumberOfJumpsPercentage.Text = this.TextBoxNumberOfJumpsPercentage.Text + numberOfJumpsPercentage;

            
            
        }

        private string EpsilonJumpPositions(int i)
        {
            string position = string.Empty;

            position = i + ")," + (i+1) + ")   ";

            return position;

        }

        private void xetResult()
        {
            ClearTextBoxes();
            CreateComparisonTable();

            PlotCloseTry();
            PlotIfChecked();
            
        }

        private void TextBoxPDurchQ_KeyUp(object sender, KeyEventArgs e)
        {
            this.DispatcherTimer.Start();
        }

        private void TextBoxP2DurchQ2_KeyUp(object sender, KeyEventArgs e)
        {
            this.DispatcherTimer.Start();
        }

        private void SliderP2Q2_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ClearTextBoxes();
            CreateComparisonTable();

            PlotCloseTry();
            PlotIfChecked();
        }

        private void SliderPQ_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ClearTextBoxes();
            CreateComparisonTable();

            PlotCloseTry();
            PlotIfChecked();
        }

        private void SliderEpsilon_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.TextBoxVergleichstabelle1.Text == string.Empty)
            {
                CreateComparisonTable();
            }

            ClearJumpTextBoxes();
            UpdateJumps();

            PlotCloseTry();
            PlotIfChecked();

        }

        private void SliderAnzahlNachkommastellen_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ClearTextBoxes();
            CreateComparisonTable();

            PlotCloseTry();
            PlotIfChecked();
        }

        private void SliderAnzahlDerPunkte_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ClearTextBoxes();
            CreateComparisonTable();

            PlotCloseTry();
            PlotIfChecked();
        }

        private void SliderYStreckungsfaktor_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PlotCloseTry();
            try
            {
                PlotIfChecked();
            }
            catch
            {
                PlotCloseTry();
                MessageBox.Show("Please press Test Button first!"); 
            }
            
        }

        private void SliderXStreckungsfaktor_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {            
            PlotCloseTry();
            try
            {
                PlotIfChecked();
            }
            catch
            {
                PlotCloseTry();
                MessageBox.Show("Please press Test Button first!"); 
            }
            
        }

              

    }
}
