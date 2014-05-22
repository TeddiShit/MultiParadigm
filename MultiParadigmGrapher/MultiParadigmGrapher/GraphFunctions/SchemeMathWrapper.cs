using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronScheme;
using System.IO;
using IronScheme.Runtime;

namespace MultiParadigmGrapher.GraphFunctions
{
    public class SchemeMathWrapper
    {
        static SchemeMathWrapper()
        {
            var uri = new Uri("/graphmath.rkt", UriKind.Relative);
            var info = App.GetContentStream(uri);

            using (var sr = new StreamReader(info.Stream))
            {
                sr.ReadToEnd().Eval();
            }
        }

        // random test
        public static void SchemeTest()
        {
            int result = "(+ 1 2)".Eval<int>();
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

        public static List<Tuple<double, double>> CalcPlotData(string f, double min, double max, int step)
        {
            var inputCons = "(calc-plot-data {0} {1} {2} {3})".Eval<Cons>(f.Eval<Callable>(), min, max, step);

            return CreateListOfTuples(inputCons);
        }

        public static List<Tuple<double, double>> CalcDerivedPlotData(string f, double min, double max, int step)
        {
            var inputCons = "(calc-derived-plot-data {0} {1} {2} {3})".Eval<Cons>(f.Eval<Callable>(), min, max, step);

            return CreateListOfTuples(inputCons);
        }

        public static List<Tuple<double, double>> CalcMidpointIntegralCoords(string f, double min, double max, int n)
        {
            var inputCons = "(calc-midpoint-integral-coords {0} {1} {2} {3})".Eval<Cons>(f.Eval<Callable>(), min, max, n);

            return CreateListOfTuples(inputCons);
        }

        public static List<Tuple<double, double>> CalcLeftIntegralCoords(string f, double min, double max, int n)
        {
            var inputCons = "(calc-left-integral-coords {0} {1} {2} {3})".Eval<Cons>(f.Eval<Callable>(), min, max, n);

            return CreateListOfTuples(inputCons);
        }
        public static List<Tuple<double, double>> CalcRightIntegralCoords(string f, double min, double max, int n)
        {
            var inputCons = "(calc-right-integral-coords {0} {1} {2} {3})".Eval<Cons>(f.Eval<Callable>(), min, max, n);

            return CreateListOfTuples(inputCons);
        }

        public static double CalcDefiniteIntegral(double min, double max, int n, List<Tuple<double, double>> coords)
        {
            var ConsDataList = new List<Cons>();
            foreach (var obj in coords)
            {
                ConsDataList.Add(new Cons(obj.Item1, obj.Item2));
            }
            var ConsDataCons = Cons.FromList(ConsDataList);


            var result = "(calc-definite-integral {0} {1} {2} {3})".Eval<double>(min, max, n, ConsDataCons);

            return result;
        }

        private static List<Tuple<double, double>> CreateListOfTuples(Cons inputCons)
        {
            List<Tuple<double, double>> PlotDataList = new List<Tuple<double, double>>();

            foreach (var obj in inputCons)
            {
                var plot = (Cons)obj;

                var x = (dynamic)plot.car;
                var y = (dynamic)plot.cdr;

                PlotDataList.Add(new Tuple<double, double>((double)x, (double)y));
            }
            return PlotDataList;
        }
    }
}
