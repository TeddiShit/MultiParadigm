﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronScheme;
using System.IO;
using IronScheme.Runtime;
using MultiParadigmGrapher.ContentHandling;

namespace MultiParadigmGrapher.GraphFunctions
{
    public static class SchemeMathWrapper
    {
        public static void LoadSchemeFunctions(IContentProvider contentProvider)
        {
            var stream = contentProvider.GetStream("/graphmath.rkt");
            using (var sr = new StreamReader(stream))
            {
                sr.ReadToEnd().Eval();
            }
        }

        public static void VerifyIsProcedure(string f)
        {
            var obj = f.Eval();
            try
            {                
                var proc = (Callable)obj;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException("Input is not a procedure.");
            }
        }

        public static int StepToSamples(double min, double max, double step)
        {
            return "(step->samples {0} {1} {2})".Eval<int>(min, max, step);
        }

        public static double SamplesToStep(double min, double max, int samples)
        {
            var result = "(samples->step {0} {1} {2})".Eval<dynamic>(min, max, samples);

            return (double)result;
        }

        public static IEnumerable<Tuple<double, double>> CalcPlotData(string f, double min, double max, double step)
        {
            var xyCons = "(calc-plot-data {0} {1} {2} {3})".Eval<Cons>(f.Eval(), min, max, step);

            //convert from Cons<Cons> (pseudo) to IEnumerable<Tuple<double, double>>
            return CreateListOfTuples(xyCons);
        }

        public static IEnumerable<Tuple<double, double>> CalcDerivedPlotData(string f, double min, double max, double step)
        {
            var xyCons = "(calc-derived-plot-data {0} {1} {2} {3})".Eval<Cons>(f.Eval(), min, max, step);

            return CreateListOfTuples(xyCons); 
        }

        public static IEnumerable<Tuple<double, double>> CalcMidpointIntegralCoords(string f, double min, double max, int n)
        {
            var xyCons = "(calc-midpoint-integral-coords {0} {1} {2} {3})".Eval<Cons>(f.Eval(), min, max, n);

            return CreateListOfTuples(xyCons);
        }

        public static IEnumerable<Tuple<double, double>> CalcLeftIntegralCoords(string f, double min, double max, int n)
        {
            var xyCons = "(calc-left-integral-coords {0} {1} {2} {3})".Eval<Cons>(f.Eval(), min, max, n);

            return CreateListOfTuples(xyCons);
        }
        public static IEnumerable<Tuple<double, double>> CalcRightIntegralCoords(string f, double min, double max, int n)
        {
            var xyCons = "(calc-right-integral-coords {0} {1} {2} {3})".Eval<Cons>(f.Eval(), min, max, n);

            return CreateListOfTuples(xyCons);
        }

        public static double CalcDefiniteIntegral(double min, double max, int n, IEnumerable<Tuple<double, double>> coords)
        {
            var consDataList = new List<Cons>();
            foreach (var obj in coords)
            {
                consDataList.Add(new Cons(obj.Item1, obj.Item2));
            }
            var consDataCons = Cons.FromList(consDataList);

            var result = "(calc-definite-integral {0} {1} {2} {3})".Eval<double>(min, max, n, consDataCons);

            return result;
        }

        public static double CalcDeltaX(double min, double max, int n)
        {
            return "(calc-delta-x {0} {1} {2})".Eval<double>(min, max, n);
        }

        private static IEnumerable<Tuple<double, double>> CreateListOfTuples(Cons inputCons)
        {
            foreach (var obj in inputCons)
            {
                var pair = (Cons)obj;

                //Unboxing must normally use the exact type.
                //We use dynamic as we don't know the exact type of the boxed value.  
                var x = (dynamic)pair.car;
                var y = (dynamic)pair.cdr;

                //x and y must be castable to double for this to work
                yield return new Tuple<double, double>((double)x, (double)y);
            }
        }
    }
}
