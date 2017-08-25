using System;
using System.Windows.Forms;

namespace JH.Calculations
{
    public partial class FormGeneralComplex : Form
    {
        Random random = new Random();

        int P;
        int N;

        public FormGeneralComplex()
        {
            InitializeComponent();
        }

        private void FormComplex_Load(object sender, EventArgs e)
        {
            N = 3;
            P = 100;
            double[] result;

            ILeastSquare ls = LeastSquare.Create(LSType.GeneralComplex);

            ls.Ac = A;
            ls.Dc = D;

            ls.Solve(N, P, out result);

            this.Close();
        }

        private Complex A(int k, int p)
        {
            return exp(k, p);
        }

        private Complex D(int p)
        {
            return (1- 0.8*exp(1, p)) * (1 + 0.1 * exp(1, p)) * (1 + 0.1 * (random.NextDouble() - 0.5)); // 1 - 0.7 * z_1 - 0.08 + z_2
        }

        private Complex exp(int k, int p)
        {
            double phi = (double)p / P * Math.PI * k;
            return new Complex(Math.Cos(phi), -Math.Sin(phi));
        }


    }
}

