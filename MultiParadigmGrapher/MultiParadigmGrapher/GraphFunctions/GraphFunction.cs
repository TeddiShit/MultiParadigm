using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MultiParadigmGrapher.GraphFunctions
{
    public class GraphFunction : INotifyPropertyChanged
    {
        static int FunctionCounter = 0;

        public GraphFunction()
        {
            FunctionCounter++;
            Name = "Function " + FunctionCounter;

            Step = 0.2;
            Code = "(lambda (x) x)"; //solid start!

            ShowDerivative = false;
            ShowIntegral = false;
            IsEnabled = true;

            Color = Brushes.White;
        }

        //backing
        private double step;
        //private int samples;

        public double Step { get; set; }
        //public int Samples { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }

        public Brush Color { get; set; }

        public bool ShowDerivative { get; set; }
        public bool ShowIntegral { get; set; }
        public bool IsEnabled { get; set; }
    
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }
}
