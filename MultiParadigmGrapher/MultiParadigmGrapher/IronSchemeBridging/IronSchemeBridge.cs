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
        public static Func<TextWriter, object> SetStdOut { get; private set; }
        public static Func<TextWriter, object> SetErrOut { get; private set; }
        public static Func<TextReader, object> SetStdIn { get; private set; }

        private static Func<object> resetEnvironment;

        static IronSchemeBridge()
        {
            extractSchemeProcedures();
            ResetEnvironment();
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
            @"
              (define range
                (lambda (n m step)
                    (cond
                        ((>= (+ n step) m) (list n))
                        (else (cons n 
                                    (range ((if (< n m) + -) n step) m step))))))
            ".Eval();
        }
    }
}
