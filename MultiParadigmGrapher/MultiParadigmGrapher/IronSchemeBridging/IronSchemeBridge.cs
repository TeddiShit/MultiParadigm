using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronScheme;
using IronScheme.Runtime;
using System.IO;

namespace MultiParadigmGrapher.IronSchemeBridging
{
    public static class IronSchemeBridge
    {
        private static Func<TextWriter, object> SetStdOut { get; set; }
        private static Func<TextWriter, object> SetErrOut { get; set; }
        public static Func<TextReader, object> SetStdIn { get; private set; }

        private static Func<object> resetEnvironment;

        public static event EventHandler<EventTextWriter.StringEventArgs> StdOutOutput;
        public static event EventHandler<EventTextWriter.StringEventArgs> StdErrOutput;

        private static EventTextWriter stdOutWriter;
        private static EventTextWriter stdErrWriter;

        static IronSchemeBridge()
        {
            extractSchemeProcedures();
            ResetEnvironment();

            stdOutWriter = new EventTextWriter();
            stdErrWriter = new EventTextWriter();

            stdOutWriter.StringWritten += (o, a) => onStdOutOutput(o, a);
            stdErrWriter.StringWritten += (o, a) => onStdErrOutput(o, a);

            SetStdOut(stdOutWriter);
            SetErrOut(stdErrWriter);
        }        

        private static void extractSchemeProcedures()
        {
            SetStdOut = "current-output-port".Eval<Callable>().Call;
            SetErrOut = "current-error-port".Eval<Callable>().Call;
            SetStdIn = "current-input-port".Eval<Callable>().Call;
            resetEnvironment = "(lambda () (interaction-environment (new-interaction-environment)))".Eval<Callable>().Call;
        }

        #region defineSchemeProcedure

        public static void DefineSchemeProcedure(this Delegate del, string name)
        {
            //workaround for defining a procedure - provided by leppie(IronScheme author) on #IronScheme @ Freenode 
            String.Format("(define {0} #f)", name).Eval();
            var expr = String.Format("(set! {0} ", name) + "{0})";
            expr.Eval(del.ToSchemeProcedure());
        }

        public static void DefineSchemeProcedure(this Callable proc, string name)
        {
            //workaround for defining a procedure - provided by leppie(IronScheme author) on #IronScheme @ Freenode 
            String.Format("(define {0} #f)", name).Eval();
            var expr = String.Format("(set! {0} ", name) + "{0})";
            expr.Eval(proc);
        }

        #endregion

        public static void ResetEnvironment()
        {
            resetEnvironment();
            defineDefaultProcedures();
        }

        public static IList<T> ToList<T>(this Cons cons)
        {
            var list = new List<T>(cons.Count());

            foreach(var consElement in cons)
            {
                //Unboxing must normally use the exact type.
                //We use dynamic as we don't know the exact type of the boxed value.                
                var dconsElement = (dynamic)consElement;

                //boxed value must be castable to type T for this to work.
                list.Add((T)dconsElement);                               
            }

            return list;
        }

        private static void defineDefaultProcedures()
        { 
        }

        private static void onStdOutOutput(object sender, EventTextWriter.StringEventArgs args)
        {
            var handler = StdOutOutput;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        private static void onStdErrOutput(object sender, EventTextWriter.StringEventArgs args)
        {
            var handler = StdErrOutput;
            if (handler != null)
            {
                handler(sender, args);
            }
        }
    }
}
