using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using JH.Applications;

namespace JH.Calculations
{
    public partial class FormFirComplex : Form
    {
        double[] dataR;
        double[] dataP;

        GraphicsDisplay graphicsDisplay;

        public FormFirComplex()
        {
            InitializeComponent();
            int N = 1000;
            dataR = new double[N];
            dataP = new double[N];
            graphicsDisplay = new GraphicsDisplay(dataR, dataP);
            graphicsDisplay.Location = new Point(25, 25);
            graphicsDisplay.Size = new Size(1000, 400);
            Controls.Add(graphicsDisplay);
        }

        private void FormFir_Load(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(FirTest));
            thread.Start();
        }

        private void FirTest()
        {
            double fs = 48000;
            int N = 1000;
            int length = 51;
            double[] result1, result2;
            double[] om = new double[N];
            Complex[] Exp = new Complex[N];
            Complex[] D = new Complex[N];
            double[] W = new double[N];
            Complex[] H = new Complex[N];

            double f1 = fs / 2.56 / 2;
            double f2 = 1.28 * f1;

            for (int i = 0; i < N; i++)
            {
                if (i < N / 2)
                {
                    om[i] = f1 / (N / 2) * i * 2 * Math.PI / fs;
                    Exp[i] = new Complex(Math.Cos(om[i]), Math.Sin(-om[i]));
                    D[i] = new Complex(1, 0) * Exp[i] * Exp[i];// *Exp[i];// *Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i];
                    W[i] = 0.000001;
                }
                else
                {
                    om[i] = ((fs / 2 - f2) / (N / 2) * (i - N / 2) + f2) * 2 * Math.PI / fs;
                    Exp[i] = new Complex(Math.Cos(om[i]), Math.Sin(-om[i]));
                    D[i] = new Complex(0, 0);
                    W[i] = 1;
                }
            }

            LeastSquare.Create(LSType.FirComplex).Solve(length, Exp, D, W, out result1);
            H = FirResponse(om, result1);
            DisplayFirResult(result1, f1, fs);

            double sum = 0;
            for (int j = 0; j < N; j++)
                sum += W[j] * Complex.AbsSqr(H[j] - D[j]);
            Console.WriteLine("Error1  {0}", Math.Sqrt(sum / N));

            for (int j = 0; j < N; j++)
                W[j] *= W[j];

            for (int k1 = 0; k1 < 100; k1++)
            {
                LeastSquare.Create(LSType.FirPowerApprox).Solve(length, Exp, D, W, H, out result2);
                for (int i = 0; i < length; i++)
                    result1[i] += 0.5 * result2[i];

                H = FirResponse(om, result1);

                sum = 0;
                for (int j = 0; j < N; j++)
                    sum += W[j] * Math.Pow((Complex.AbsSqr(H[j]) - Complex.AbsSqr(D[j])), 2);
                Console.WriteLine("Error2  {0}", Math.Sqrt(sum / N));

                DisplayFirResult(result1, f1, fs);

            }
        }

        private Complex[] FirResponse(double[] om, double[] c)
        {
            Complex[] result = new Complex[om.Length];

            for (int j = 0; j < om.Length; j++)
            {
                result[j] = new Complex(0, 0);
                for (int k = 0; k < c.Length; k++)
                    result[j] += c[k] * new Complex(Math.Cos(om[j] * k), Math.Sin(-om[j] * k));
            }

            return result;
        }

        private void DisplayFirResult(double[] result, double f1, double fs)
        {
            int N = graphicsDisplay.Width;
            double[] om = new double[N];
            Complex[] H = new Complex[N];

            for (int j = 0; j < N; j++)
                om[j] = Math.PI / N * j;

            H = FirResponse(om, result);

            for (int j = 0; j < N; j++)
            {
                dataP[j] = 10 * Math.Log10(Complex.AbsSqr(H[j]));
                if (j <f1 / fs * N * 2)
                    dataP[j] *= 20;
            }

            graphicsDisplay.Invalidate();
        }


    }
}
