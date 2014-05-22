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
        private double integralMax;
        private double integralMin;
        private int integralRes;
      
        private bool isEnabled;
        private bool showDerivative;
        private bool showIntegral;

        private Brush color;

        public double Step 
        {
            get { return step; }
            set 
            {
                if (value > 0)
                {
                    step = value;
                    OnPropertyChanged();
                }
                else
                    throw new ArgumentOutOfRangeException("Step must be positive.");
            }
        }
        public int Samples
        {
            get { return samples; }
            set
            {
                if (value > 0)
                {
                    samples = value;
                    OnPropertyChanged();
                }
                else
                    throw new ArgumentOutOfRangeException("Samples must be positive.");
            }
        }

        public double IntegralMin
        {
            get { return integralMin; }
            set
            {
                if (value < IntegralMax)
                {
                    integralMin = value;
                    OnPropertyChanged();
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Integral minimum must be less than integral maximum.");
                }
            }
        }
        public double IntegralMax
        {
            get { return integralMax; }
            set
            {
                if (value > IntegralMin)
                {
                    integralMax = value;
                    OnPropertyChanged();
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Integral maximum must be greater than integral minimum.");
                }
            }
        }
        public int IntegralRes 
        {
            get { return integralRes; }
            set 
            {
                if (value > 0)
                {
                    integralRes = value;
                    OnPropertyChanged();
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Integral resolution must be positive.");
                }
            }
        }

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
