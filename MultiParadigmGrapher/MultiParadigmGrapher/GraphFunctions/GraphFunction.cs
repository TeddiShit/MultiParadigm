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
            Samples = 50;
            Code = "(lambda (x) x)"; //solid start!

            IntegralMax = 20;
            IntegralMin = -20;
            IntegralRes = 10;

            ShowDerivative = false;
            ShowIntegral = false;
            IsEnabled = true;

            Color = Brushes.White;
        }

        //backing
        private double step;
        private int samples;
      
        private bool isEnabled;
        private bool showDerivative;
        private bool showIntegral;

        private Brush color;

        public double Step { get; set; }
        public int Samples { get; set; }

        public double IntegralMin { get; set; }
        public double IntegralMax { get; set; }
        public int IntegralRes { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }

        public Brush Color 
        { 
            get { return color; } 
            set 
            {
                if (value != color)
                {
                    color = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowDerivative 
        {
            get { return showDerivative; }
            set 
            {
                if (value != showDerivative)
                {
                    showDerivative = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool ShowIntegral
        {
            get { return showIntegral; }
            set
            {
                if (value != showIntegral)
                {
                    showIntegral = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (value != isEnabled)
                {
                    isEnabled = value;
                    OnPropertyChanged();
                }
            }
        }
    
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }
}
