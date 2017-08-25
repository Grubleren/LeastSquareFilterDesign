using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using JH.Applications;

namespace JH.Calculations
{
    public partial class FormIir : Form
    {
        double[] dataR;
        double[] dataP;

        GraphicsDisplay graphicsDisplay;

        public FormIir()
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

        private void FormIir_Load(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(IirTest));
            thread.Start();
        }

        private void IirTest()
        {
            double fs = 4000;
            int N = 1000;
            int numeratorOrder = 4;
            int denominatorOrder = 4;
            double[] result;
            double[] om = new double[N];
            Complex[] Exp = new Complex[N];
            Complex[] D = new Complex[N];
            double[] W = new double[N];
            Complex[] A = new Complex[N];
            Complex[] B = new Complex[N];
            Complex[] H = new Complex[N];
            double[] a = new double[denominatorOrder + 1];
            double[] b = new double[numeratorOrder + 1];

            double f1 = fs / 8;
            double f2 = 3 * f1;

            for (int i = 0; i < N; i++)
            {
                if (i < N / 2)
                {
                    om[i] = f1 / (N / 2) * i * 2 * Math.PI / fs;
                    Exp[i] = new Complex(Math.Cos(om[i]), Math.Sin(-om[i]));
                    D[i] = new Complex(1, 0);// *Exp[i] * Exp[i] * Exp[i];// *Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i] * Exp[i];
                    W[i] = 1;
                }
                else
                {
                    om[i] = ((fs / 2 - f2) / (N / 2) * (i - N / 2) + f2) * 2 * Math.PI / fs;
                    Exp[i] = new Complex(Math.Cos(om[i]), Math.Sin(-om[i]));
                    D[i] = new Complex(0, 0);
                    W[i] = 1;
                }
            }

            a[0] = 1;
            A = FirResponse(om, a);
            LeastSquare.Create(LSType.FirComplex).Solve(numeratorOrder + 1, Exp, Vector.Mul(D, A), W, out b);
            DisplayFirResult(b, f1, fs);

            H = FirResponse(om, b);
            double sum = 0;
            for (int j = 0; j < N; j++)
                sum += W[j] * Math.Pow((Complex.AbsSqr(H[j]) - Complex.AbsSqr(D[j])), 2);
            Console.WriteLine("Error1  {0}", Math.Sqrt(sum / N));

            for (int j = 0; j < N; j++)
                W[j] *= 1;// W[j];
            for (int k1 = 0; k1 < 5000; k1++)
            {
                LeastSquare.Create(LSType.IirPowerApprox).Solve(numeratorOrder + 1 + denominatorOrder, numeratorOrder + 1, Exp, D, W, H, A, out result);
                for (int i = 0; i < numeratorOrder + 1; i++)
                    b[i] += 0.5 * result[i];
                for (int i = 0; i < denominatorOrder; i++)
                    a[i + 1] += 0.5 * result[i + numeratorOrder + 1];

                PolynomialRootFinder.PolynomialRootFinder poly = new PolynomialRootFinder.PolynomialRootFinder();
                Complex[] roots = new Complex[denominatorOrder];
                int numberRoots;
                poly.FindRootsReverse(a, roots, out numberRoots);
                for (int i = 0; i < numberRoots; i++)
                {
                    if (Complex.AbsSqr(roots[i]) > 1)
                        roots[i] = 1 / roots[i];
                    if (Complex.AbsSqr(roots[i]) > 0.81)
                        roots[i] /= Math.Sqrt(Complex.AbsSqr(roots[i])) * 1.1;
                    Console.WriteLine(Math.Sqrt(Complex.AbsSqr(roots[i])));
                }

                a = poly.BuildPolynomial(roots);
                double suma = 0;
                for (int i = 0; i < numeratorOrder + 1; i++)
                {
                    suma += a[i];
                }

                double sumb = 0;
                for (int i = 0; i < numeratorOrder + 1; i++)
                {
                    sumb += b[i];
                }

                for (int i = 0; i < numeratorOrder + 1; i++)
                {
                    b[i] *= suma / sumb;
                }

                A = FirResponse(om, a);
                for (int j = 0; j < N; j++)
                    if (Complex.AbsSqr(A[j]) < 0.1)
                        A[j] /= Math.Sqrt(Complex.AbsSqr(A[j])) * 10;

                H = Vector.Div(FirResponse(om, b), A);
                //for (int j = 0; j < N; j++)
                //    if (Complex.AbsSqr(H[j]) > 1.1)
                //        H[j] /= Math.Sqrt(Complex.AbsSqr(H[j]));

                //for (int j = 0; j < N; j++)
                //       D[j] = Math.Sqrt(Complex.AbsSqr(D[j]/H[j])) * H[j];
                sum = 0;
                for (int j = 0; j < N; j++)
                    sum += W[j] * Math.Pow((Complex.AbsSqr(H[j]) - Complex.AbsSqr(D[j])), 2);
                Console.WriteLine("Error2  {0}", Math.Sqrt(sum / N));

                DisplayIirResult(a, b, f1, fs);
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
                if (j < f1 / fs * N * 2)
                    dataP[j] *= 20;
            }

            graphicsDisplay.Invalidate();
        }


        private void DisplayIirResult(double[] a, double[] b, double f1, double fs)
        {
            int N = graphicsDisplay.Width;
            double[] om = new double[N];
            Complex[] H = new Complex[N];

            for (int j = 0; j < N; j++)
                om[j] = Math.PI / N * j;

            H = Vector.Div(FirResponse(om, b), FirResponse(om, a));

            for (int j = 0; j < N; j++)
            {
                dataP[j] = 10 * Math.Log10(Complex.AbsSqr(H[j]));
                if (j < f1 / fs * N * 2)
                    dataP[j] *= 100;
            }

            graphicsDisplay.Invalidate();
            // Thread.Sleep(100);
        }



    }
}
