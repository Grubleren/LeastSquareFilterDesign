using System;
using System.Threading;

using JH.Applications;

namespace JH.Calculations
{
    public class FirPowerApprox : ILeastSquare
    {
        public override void Solve(int N, Complex[] Exp, Complex[] D, double[] W, Complex[] H, out double[] result)
        {
            double[] vector = new double[N];
            double[,] matrix = new double[N, N];
            Complex[] fr = new Complex[D.Length];
            Complex[] fc = new Complex[D.Length];

            for (int j = 0; j < D.Length; j++)
                fr[j] = new Complex(1, 0);

            for (int r = 0; r < N; r++)
            {
                for (int j = 0; j < D.Length; j++)
                {
                    vector[r] += (W[j] * (Complex.AbsSqr(D[j]) - Complex.AbsSqr(H[j])) * Complex.Conjugate(H[j]) * fr[j]).re;
                    fc[j] = new Complex(1, 0);
                    for (int c = 0; c < N; c++)
                    {
                        matrix[r, c] += (W[j] * (Complex.Conjugate(H[j] * H[j]) * fc[j] + (2 * Complex.AbsSqr(H[j]) - Complex.AbsSqr(D[j])) * Complex.Conjugate(fc[j])) * fr[j]).re;
                        fc[j] *= Exp[j];
                    }
                    fr[j] *= Exp[j];
                }
            }

            SolveNbyN solveNbyN = new SolveNbyN();
            solveNbyN.Solve(N, matrix, vector, out result);

        }
    }
}
