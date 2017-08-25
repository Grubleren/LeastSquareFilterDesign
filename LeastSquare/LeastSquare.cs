using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.Calculations
{
    public abstract class ILeastSquare
    {
        public delegate double ADEventHandler(int k, int p);
        public delegate double DDEventHandler(int p);
        public delegate Complex ACEventHandler(int k, int j);
        public delegate Complex DCEventHandler(int j);


        public ADEventHandler Ad;
        public DDEventHandler Dd;

        public ACEventHandler Ac;
        public DCEventHandler Dc;

        public virtual void Solve(int N, int P, out double[] result) { result = null; }
        public virtual void Solve(int N, Complex[] Exp, Complex[] D, double[] W, out double[] result) { result = null; }
        public virtual void Solve(int N, Complex[] Exp, Complex[] D, double[] W, Complex[] H, out double[] result) { result = null; }
        public virtual void Solve(int N, int P, Complex[] Exp, Complex[] D, double[] W, Complex[] H, Complex[] A, out double[] result) { result = null; }

    }

    public enum LSType
    {
        GeneralReal,
        GeneralComplex,
        FirComplex,
        FirPowerApprox,
        IirComplexApprox,
        IirPowerApprox
    }

    public class LeastSquare
    {
        public static ILeastSquare Create(LSType type)
        {
            switch(type)
            {
                case LSType.GeneralReal:
                    return new GeneralReal();
                case LSType.GeneralComplex:
                    return new GeneralComplex();
                case LSType.FirComplex:
                    return new FirComplex();
                case LSType.FirPowerApprox:
                    return new FirPowerApprox();
                case LSType.IirComplexApprox:
                    return new IirComplexApprox();
                case LSType.IirPowerApprox:
                    return new IirPowerApprox();
            }
            return null;
        }
    }
}
