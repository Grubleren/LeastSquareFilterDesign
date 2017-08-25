using JH.Applications;
using System;

namespace JH.Calculations
{
    public class GeneralReal : ILeastSquare
    {
        public override void Solve(int N, int P, out double[] result)
        {
            if (Ad == null || Dd == null)
                throw new InvalidOperationException("Missing event handlers");

            double[,] matrix = new double[N, N];
            double[] vector = new double[N];

            for (int p = 0; p < P; p++)
            {
                double Dp = Dd(p);
                for (int r = 0; r < N; r++)
                {
                    double Arp = Ad(r, p);
                    for (int c = 0; c < N; c++)
                    {
                        matrix[r, c] += Ad(c, p) * Arp;
                    }
                    vector[r] += Dp * Arp;
                }
            }

            SolveNbyN solve = new SolveNbyN();
            solve.Solve(N, matrix, vector, out result);
        }

    }
}
