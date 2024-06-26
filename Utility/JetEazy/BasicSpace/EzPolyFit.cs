using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using System.Linq;
//using MathNet.Numerics.LinearAlgebra;

namespace JetEazy.BasicSpace
{
    public class EzPolyFit
    {
        public EzPolyFit() { }
        public double FitA { get; set; } = 0;
        public double FitB { get; set; } = 0;
        public double FitC { get; set; } = 0;
        public double[] Fit(double[] yData, double[] xData, int order = 2)
        {
            var matrixX = createDesignMatrix(xData, order);
            var matrixY = createResponseMatrix(yData);
            var coefficients = solve(matrixX, matrixY);
            var coefs = coefficients.ToArray();
            FitA = coefs[2];
            FitB = coefs[1];
            FitC = coefs[0];
            return coefficients.ToArray();
        }

        #region PRIVATE_FUNCTIONS
        Matrix<double> createDesignMatrix(double[] xData, int order)
        {
            int n = xData.Length;
            var matrixX = Matrix<double>.Build.Dense(n, order + 1);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j <= order; j++)
                {
                    matrixX[i, j] = System.Math.Pow(xData[i], j);
                }
            }

            return matrixX;
        }
        Vector<double> createResponseMatrix(double[] yData)
        {
            return Vector<double>.Build.DenseOfArray(yData);
        }
        List<double> solve(Matrix<double> matrixX, Vector<double> matrixY)
        {
            var qr = matrixX.QR();
            var coefficients = qr.Solve(matrixY);

            return coefficients.ToList();
        }
        #endregion
    }
}
