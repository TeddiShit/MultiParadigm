using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronScheme;
using System.IO;

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

        public static int StepToSamples(int min, int max, int step)
        {
            return "(step->samples {0} {1} {2})".Eval<int>(min, max, step);
        }

        public static double SamplesToStep(int min, int max, int samples)
        {
            var result = "(samples->step {0} {1} {2})".Eval<dynamic>(min, max, samples);

            return (double)result;
        }
    }
}
