using System;
using System.Windows.Forms;

namespace JH.Calculations
{
    public partial class FormGeneralReal : Form
    {
        Random random = new Random();

        int N;
        int P;
        int L;


        public FormGeneralReal()
        {
            InitializeComponent();
        }

        private void FormReal_Load(object sender, EventArgs e)
        {
            N = 5;
            P = 1000000;
            L = 1024;

            double[] result;

            ILeastSquare ls = LeastSquare.Create(LSType.GeneralReal);
            for (int n = 2; n < 8; n++)
            {
                N = n;

                ls.Ad = A;
                ls.Dd = D;

                ls.Solve(N, P, out result);

                for (int i = 0; i < result.Length;i++ )
                    Console.Write("{0,15:0.0000000000}",result[i]);
                Console.WriteLine();
            }
            this.Close();
        }

        private double A(int k, int p)
        {
            double x = X(p);

            if (k == 0)
            {
                return Sinx_x(x);
            }
            else
            {

                return 0.5 * (Sinx_x((x - k)) + Sinx_x((x + k)));
            }
        }

        private double D(int p)
        {
            double x = X(p);

            if (x == 0)
                return 1;
            else
                return 0;
        }

        private double X(int p)
        {
            double p1 = p - P / 2;
            int sgn = Math.Sign(p1);

            if (sgn == 0)
                return 0;
            else
                return (double)sgn * ((Math.Abs(p1) - 1) / (P / 2 - 1) * (L / 2 - N) + N);
        }

        private double Sinx_x(double x)
        {
            if (x == 0)
                return 1;

            else
                return Math.Sin(x * Math.PI) / Math.Sin(x / L * Math.PI) / L;

        }


    }
}

