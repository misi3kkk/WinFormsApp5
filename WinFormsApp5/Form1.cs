using System;
using System.Windows.Forms;
using AngouriMath;
using AngouriMath.Extensions;
using static AngouriMath.MathS;
using MathNet.Numerics;
using MathNet.Symbolics;
using ScottPlot;

namespace WinFormsApp5
{
    public partial class Form1 : Form
    {
        private ScottPlot.FormsPlot scottPlot;
        
        public Form1()
        {
            InitializeComponent();
            scottPlot = new ScottPlot.FormsPlot();
            scottPlot.Dock = DockStyle.Right;
            scottPlot.Width = 400;
            scottPlot.Height = 400;
            Controls.Add(scottPlot);

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
        
            try
            {

                string functionExpression = functionTextBox.Text;


                SymbolicExpression function = SymbolicExpression.Parse(functionExpression);
                var variableValues = new Dictionary<string, FloatingPoint>
                {
                    { "x", 0.0 }
                };

                double lowerLimit = double.Parse(lowerBoundTextBox.Text);
                double upperLimit = double.Parse(textBox3.Text);
                double stlower = -1;
                double stupper = -1;

                double nki_n = double.Parse(textBox1.Text);
                int numPoints = (int)nki_n;

              
               


                Func<double, double> functionDelegate = x =>
                {
                    variableValues["x"] = x;
                    return function.Evaluate(variableValues).RealValue;
                };


                double result = GaussLegendreIntegrationMethod(lowerLimit, upperLimit, nki_n, functionDelegate);

                ScaleIntegrationLimits(ref lowerLimit, ref upperLimit, stupper, stlower);


                resultLabel.Text = $"Wynik: {result:F6}";
                PlotFunction(functionDelegate, -1, 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wyst¹pi³ b³¹d: {ex.Message}", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void PlotFunction(Func<double, double> function, double lower, double upper)
        {

            var plt = scottPlot.Plot;
            plt.Clear();


            double[] xValues = ScottPlot.DataGen.Range(lower, upper, 0.01);
            double[] yValues = xValues.Select(function).ToArray();


            plt.PlotScatter(xValues, yValues, label: "Funkcja");


            plt.PlotVLine(lower, color: System.Drawing.Color.Red, lineStyle: LineStyle.Dash, label: "Granice");
            plt.PlotVLine(upper, color: System.Drawing.Color.Red, lineStyle: LineStyle.Dash);

            scottPlot.Refresh();

            plt.Legend();


            plt.Render();
        }
        static void ScaleIntegrationLimits(ref double a, ref double b, double newA, double newB)
        {
            
            double scaleFactor = (newB - newA) / (b - a);

            a = newA + scaleFactor * (a - a);
            b = newA + scaleFactor * (b - a);
        }
        static double ScaleLimits(double y, double a, double b)
        {
            return (2 * y - a - b) / (b - a);

        }
        static double GaussLegendreIntegrationMethod(double a, double b, double numPoints, Func<double, double> function)
        {
        
            double[] weights = {
        0.236926885056189087514264040719917362643260002212,
        0.478628670499366468041291514835638192912295368357,
        0.568888888888888888888888888888888888888888888889,
        0.478628670499366468041291514835638192912295368357,
        0.236926885056189087514264040719917362643260002212
    };

            double[] points = {
         -0.906179845938663992797626878299047367066831846353,
        -0.538469310105683091036314420700208804967286606054,
        0.000000000000000000000000000000000000000000000000,
        0.538469310105683091036314420700208804967286606054,
        0.906179845938663992797626878299047367066831846353
    };      
            double result = 0;
                
          
            for (int i = 0; i < numPoints; i++)
            {
                double x = ((0.5*(b - a)) * points[i] + (0.5*(b + a)));
                result += weights[i] * function(x);
            }


            result *= 0.5 * (b - a);

            return result;
        }



    }

}


