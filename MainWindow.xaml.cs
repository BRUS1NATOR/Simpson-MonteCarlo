using org.mariuszgromada.math.mxparser;
using OxyPlot;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using Expression = org.mariuszgromada.math.mxparser.Expression;

namespace Simpson
{
    public partial class MainWindow : Window
    {
        List<Point> res = new List<Point>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double a = Convert.ToDouble(textBoxA.Text);
            double b = Convert.ToDouble(textBoxB.Text);
            int n = Convert.ToInt32(textBoxN.Text);

            Argument x = new Argument("x");
            x.setArgumentValue(1);
            Expression expression = new Expression(textBox1.Text, x);

            if (expression != null)
            {
                if ((bool)ControlAccuracy.IsChecked == true)
                {
                    double accuracy = Convert.ToDouble(textBoxAccuracy.Text);
                    n = 2;
                    while (true)
                    {
                        double sim1 = SimpsonMethod(expression, b, a, n);
                        double sim2 = SimpsonMethod(expression, b, a, n * 2);
                        if (Runge(sim1, sim2) <= accuracy || n > 16384)
                        {
                            labelSimpson.Content = $"Simpson({n}):\n" + sim1;
                            break;
                        }

                        n *= 2;
                    }
                }
                else
                {
                    double simpson = SimpsonMethod(expression, b, a, n);
                    labelSimpson.Content = "Simpson:\n" + simpson;

                    double runge = Runge(SimpsonMethod(expression, b, a, 2 * n), simpson);
                    labelRunge.Content = "Runge:\n" + runge;
                }

                expression = new Expression($"int({textBox1.Text},x,{a.ToString(new CultureInfo("en-US"))},{b.ToString(new CultureInfo("en-US"))})");
                labelExpect.Content = "Expected:\n" + expression.calculate();

                CreateMonteCarlo();
            }
        }

        public double SimpsonMethod(Expression f, double b, double a, int n)
        {
            res.Clear();
            double sum = 0;
            double h = (b - a) / n;     //b-a = величина отрезка
            string stringFunction = f.getExpressionString();

            for (int i = 0; i < n; i++)
            {
                Argument x = new Argument("x");
                x.setArgumentValue(a + h * i);
                double exp1 = new Expression(stringFunction, x).calculate();

                x.setArgumentValue(a + h * (i + 0.5f));
                double exp2 = new Expression(stringFunction, x).calculate();

                x.setArgumentValue(a + h * (i + 1));
                double exp3 = new Expression(stringFunction, x).calculate();

                sum += (exp1 + 4 * exp2 + exp3);

                res.Add(new Point(a + h * i, exp1));
            }

            return sum * h / 6f;
        }


        public void CreateMonteCarlo()
        {
            (this.DataContext as MainViewModel).SetPoints(res);
            var stream = new MemoryStream();
            var pngExporter = new PngExporter { Width = 512, Height = 512, Background = OxyColors.White };
            pngExporter.Export((this.DataContext as MainViewModel).MyModel, stream);

            double axisLength = (this.DataContext as MainViewModel).MyModel.DefaultXAxis.ActualMaximum - (this.DataContext as MainViewModel).MyModel.DefaultXAxis.ActualMinimum;
            System.Drawing.Bitmap myBitmap = new System.Drawing.Bitmap(stream);

            ImageWindow window = new ImageWindow();
            labelMonte.Content = "Monte Carlo: " + window.MonteCarlo(myBitmap, axisLength);
            window.Show();
        }

        public double Runge(double fun1, double fun2)
        {
            return 1 / 15f * (Math.Abs(fun1 - fun2));
        }

        private void ControlAccuracy_Checked(object sender, RoutedEventArgs e)
        {
            textBoxAccuracy.IsEnabled = true;
            textBoxN.IsEnabled = false;
        }

        private void ControlAccuracy_Unchecked(object sender, RoutedEventArgs e)
        {
            textBoxAccuracy.IsEnabled = false;
            textBoxN.IsEnabled = true;
        }
    }
}
