using OxyPlot.Series;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace MultiParadigmGrapher.GraphFunctions
{
    public enum LogType
    {
        StdOut,
        StdErr,
        SyntaxErr,
        SchemeError,
        GeneralError
    }

    public class LogEntry
    {
        public LogEntry(string message, LogType type)
        {
            Type = type;
            Message = message;
        }

        public LogType Type { get; private set; }
        public string Message { get; private set; }
    }

    public class GraphFunction : INotifyPropertyChanged
    {
        //for automatic default name
        static int FunctionCounter = 0;

        //default constructor setting default values
        public GraphFunction()
        {
            FunctionCounter++;
            Name = "Function " + FunctionCounter;

            Step = 0.2;
            Samples = 50;
            Code = 
@";cos(3*x)+sin(x)
(lambda (x)
  (+ (cos (* 3 x))
	 (sin x)))"; 

            IntegralMax = 20;
            IntegralMin = -20;
            IntegralRes = 10;

            IsMiddleIntegral = true;

            ShowDerivative = false;
            ShowIntegral = false;
            IsEnabled = true;

            Color = Brushes.Gray;

            Log = new ObservableCollection<LogEntry>();
        }

        #region Backing fields
        private double step;
        private int samples;

        private double integralMax;
        private double integralMin;
        private int integralRes;
      
        private bool isEnabled;
        private bool showDerivative;
        private bool showIntegral;

        private bool isLeftIntegral;
        private bool isMiddleIntegral;
        private bool isRightIntegral;

        private Brush color;
        private string name;

        #endregion

        #region Properties
        //resolution
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

        //integral settings
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
        public bool IsLeftIntegral
        {
            get { return isLeftIntegral; }
            set
            {
                isLeftIntegral = value;
                OnPropertyChanged();
            }
        }
        public bool IsMiddleIntegral
        {
            get { return isMiddleIntegral; }
            set
            {
                isMiddleIntegral = value;
                OnPropertyChanged();
            }
        }
        public bool IsRightIntegral
        {
            get { return isRightIntegral; }
            set
            {
                isRightIntegral = value;
                OnPropertyChanged();
            }
        }

        //visibility properties
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

        public string Code { get; set; }
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
        public string Name
        {
            get { return name; }
            set 
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }
        
        //Properties for related OxyPlot series
        public LineSeries PlotSeries { get; set; }
        public LineSeries DerivedSeries { get; set; }
        public AreaSeries IntegralSeries { get; set; }

        public ObservableCollection<LogEntry> Log { get; private set; }
        #endregion

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
