using System;

using JH.Applications;

namespace JH.Calculations
{
    public class GeneralComplex : ILeastSquare
    {
        public override void Solve(int N, int P, out double[] result)
        {
            if (Ac == null || Dc == null)
                throw new InvalidOperationException("Missing event handlers");

            double[,] matrix = new double[N, N];
            double[] vector = new double[N];

            for (int r = 0; r < N; r++)
            {
                for (int j = 0; j < P; j++)
                {
                    Complex Arj = Ac(r, j);
                    for (int c = 0; c < N; c++)
                    {
                        matrix[r, c] += (Arj * Complex.Conjugate(Ac(c, j))).re;
                    }
                    vector[r] += (Arj * Complex.Conjugate(Dc(j))).re;
                }
            }

            SolveNbyN solve = new SolveNbyN();
            solve.Solve(N, matrix, vector, out result);
        }

    }
}
