using System;
using System.Threading;

using JH.Applications;

namespace JH.Calculations
{
    public class IirComplexApprox : ILeastSquare
    {
        public override void Solve(int N, int P, Complex[] Exp, Complex[] D, double[] W, Complex[] H, Complex[] A, out double[] result)
        {
            double[] vector = new double[N];
            double[,] matrix = new double[N, N];
            Complex[] fr = new Complex[D.Length];
            Complex[] fc = new Complex[D.Length];

            for (int j = 0; j < D.Length; j++)
                fr[j] = 1 / A[j];

            for (int r = 0; r < N; r++)
            {
                for (int j = 0; j < D.Length; j++)
                {
                    vector[r] += (W[j] * Complex.Conjugate(D[j] - H[j]) * fr[j]).re;
                    fc[j] = 1 / A[j];
                    for (int c = 0; c < N; c++)
                    {
                        matrix[r, c] += (W[j] * Complex.Conjugate(fc[j]) * fr[j]).re;
                        if (c != P - 1)
                            fc[j] *= Exp[j];
                        else
                            fc[j] = -H[j] / A[j] * Exp[j];
                    }
                    if (r != P - 1)
                        fr[j] *= Exp[j];
                    else
                        fr[j] = -H[j] / A[j] * Exp[j];
                }
            }

            SolveNbyN solveNbyN = new SolveNbyN();
            solveNbyN.Solve(N, matrix, vector, out result);

        }
    }
}
