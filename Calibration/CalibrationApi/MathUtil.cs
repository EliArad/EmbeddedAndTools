using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetMatrix;

namespace MathUtilLib
{
    public static class MathUtil
    {
        /// <summary>
        /// Function : GetStandardDeviation
        /// Calculates the Standard Deviation
        /// Formula: SD = Sqrt (Summation(x-x~)^2)/N-1
        /// x~= Summation(x)/N
        /// 
        /// </summary>
        /// <returns></returns>
        public static double GetStandardDeviation(List<double> x)
        {
            double x_mean = 0d;
            double sumNumeratorSquare = 0d;
            double SD = 0d;

            x_mean = x.Sum() / x.Count;
            x.ForEach(i => sumNumeratorSquare += (x_mean - i) * (x_mean - i));
            SD = Math.Sqrt(sumNumeratorSquare / (x.Count - 1));

            //Currently rounding off to 2 decimal places. 
            //This can be configurable
            return SD;// Math.Round(SD, 2);
        }

        /// <summary>
        /// Function : MultiLinearRegression
        /// Purpose : Implements  Multiple Regression  
        /// Formula: W = Inverse(At.A).At.B
        /// A - > X matrix , B - > Y matrix , At - > Transpose of A matrix/X matrix
        /// 
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public static double[][] MultiLinearRegression(double[][] X, double[] Y)
        {
            GeneralMatrix A = new GeneralMatrix(X);
            GeneralMatrix B = new GeneralMatrix(Y, 1);

            //Find A transpose
            GeneralMatrix AT = A.Transpose();

            //Multiply A and At
            GeneralMatrix AT_mul_A = A.Multiply(AT);

            //Get the inverse of Multiplication of A and At(AT_mul_A)
            GeneralMatrix At_Inv_A = AT_mul_A.Inverse();

            //Get the Multiplication of B and At
            GeneralMatrix At_mul_Y = B.Multiply(AT);

            //Multiply AT_mul_A and At_mul_Y
            GeneralMatrix W = At_mul_Y.Multiply(At_Inv_A);

            double[][] Result = W.Array;
            return Result;
        }

        /// <summary>
        /// Function: SimpleLR
        /// Purpose: Computes the simple linear regression
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public static Dictionary<string, double> SimpleLR(List<double> X, List<double> Y)
        {
            ///Variable declarations            
            int num = 0; //use for List count
            double sumX = 0; //summation of x[i]
            double sumY = 0; //summation of y[i]
            double sum2X = 0; // summation of x[i]*x[i]
            double sum2Y = 0; // summation  of y[i]*y[i]
            double sumXY = 0; // summation of x[i] * y[i]  
            double denX = 0;
            double denY = 0;
            double top = 0;
            double corelation = 0; // holds Corelation
            double slope = 0; // holds slope(beta)
            double y_intercept = 0; //holds y-intercept (alpha)

            //Standard error variables
            double sum_res = 0.0;
            double yhat = 0;
            double res = 0;
            double standardError = 0; //
            int n = 0;
            //End standard variable declaration
            Dictionary<string, double> result
                = new Dictionary<string, double>(); //Stores the final result
            //End variable declaration

            #region Computation begins

            num = X.Count;  //Since the X and Y list are of same length, so 
            // we can take the count of any one list 
            sumX = X.Sum();  //Get Sum of X list
            sumY = Y.Sum(); //Get Sum of Y list           
            X.ForEach(i => { sum2X += i * i; }); //Get sum of x[i]*x[i]           
            Y.ForEach(i => { sum2Y += i * i; }); //Get sum of y[i]*y[i]            
            sumXY = Enumerable.Range(0, num).Select(i => X[i] * Y[i]).Sum();//Get Summation of x[i] * y[i]

            //Find denx, deny,top
            denX = num * sum2X - sumX * sumX;
            denY = num * sum2Y - sumY * sumY;
            top = num * sumXY - sumX * sumY;

            //Find corelation, slope and y-intercept
            corelation = top / Math.Sqrt(denX * denY);
            slope = top / denX;
            y_intercept = (sumY - sumX * slope) / num;


            //Implementation of Standard Error
            sum_res = Enumerable.Range(0, num).Aggregate(0.0, (sum, i) =>
            {
                yhat = y_intercept + (slope * X[i]);
                res = yhat - Y[i];
                n++;
                return sum + res * res;
            });

            if (n > 2)
            {
                standardError = sum_res / (1.0 * n - 2.0);
                standardError = Math.Pow(standardError, 0.5);
            }
            else standardError = 0;

            #endregion

            //Add the computed value to the resultant dictionary
            result.Add("Beta", slope);
            result.Add("Alpha", y_intercept);
            result.Add("Corelation", corelation);
            result.Add("StandardError", standardError);
            return result;
        }
        /// <summary>
        /// Function : BetaExposure
        /// Purpose: Calculates the beta exposure
        /// </summary>
        /// <param name="x"></param>
        /// <param name="beta"></param>
        /// <param name="standardError"></param>
        /// <returns></returns>
        public static double BetaExposure(List<double> X, List<double> Y)
        {
            Dictionary<string, double> SimpleLRValues = SimpleLR(X, Y);
            double sx = GetStandardDeviation(X);

            double sb = SimpleLRValues["StandardError"] / sx / Math.Pow(59, 0.5);
            double varb = sb * sb;
            double beta_adj = ((SimpleLRValues["Beta"] / 4.0) + varb) / (0.25 + varb);
            return (beta_adj - 1.0256) / 0.4677;
        }
    }
}
