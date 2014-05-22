using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using IronScheme;
using IronScheme.Runtime;
using System;
using System.Windows;
using System.Linq;
using IronScheme.Scripting;
using MultiParadigmGrapher.IronSchemeBridging;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Xml;
using MultiParadigmGrapher.GraphFunctions;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections;


namespace MultiParadigmGrapher.ViewModel
{

    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private const double TEN = 10;
        private const double TWO = 2;
        private const double EULER = 2.71828182845904523536;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Functions = new ObservableCollection<GraphFunction>();
            Functions.Add(new GraphFunction());

            ApplyFunctionCommand = new RelayCommand(ApplyFunctionExecute, ApplyFunctionCanExecute);
            AddFunctionCommand = new RelayCommand(() => Functions.Add(new GraphFunction()), () => Functions.Count < 30);
            DeleteFunctionCommand = new RelayCommand<IList>(deleteFunctionExecute, deleteFunctionCanExecute);

            InitPlotModel();

            LogarithmicBases = new List<double> { TEN, TWO, EULER };
            XLogarithmicBase = YLogarithmicBase = TEN;
            
            

            LoadSyntaxHighlighting();
            IronSchemeBridge.ResetEnvironment();
        }        

        //backing
        private TextDocument codeDocument = new TextDocument();
        private GraphFunction selectedFunction;
        private bool isXLogarithmic = false;
        private bool isXLinear = true;
        private bool isYLogarithmic = false;
        private bool isYLinear = true;
        private double xMin = -20;
        private double xMax = 20;
        private double yMin = -20;
        private double yMax = 20;

        //properties
        public IList<double> LogarithmicBases { get; private set; }
        public bool IsXLogarithmic 
        {
            get { return isXLogarithmic; }
            set 
            {
                isXLogarithmic = value;
                RaisePropertyChanged();
            }
        }
        public bool IsXLinear
        {
            get { return isXLinear; }
            set
            {
                isXLinear = value;
                RaisePropertyChanged();
            }
        }
        public bool IsYLogarithmic
        {
            get { return isYLogarithmic; }
            set
            {
                isYLogarithmic = value;
                RaisePropertyChanged();
            }
        }
        public bool IsYLinear
        {
            get { return isYLinear; }
            set
            {
                isYLinear = value;
                RaisePropertyChanged();
            }
        }
        public double XLogarithmicBase { get; set; }
        public double YLogarithmicBase { get; set; }
        public double XMin
        {
            get { return xMin; }
            set
            {
                if (value < XMax)
                {
                    xMin = value;
                    RaisePropertyChanged();
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Minimum x value must be less than maximum x value.");
                }
            }
        }
        public double XMax
        {
            get { return yMax; }
            set
            {
                if (value > XMin)
                {
                    xMax = value;
                    RaisePropertyChanged();
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Maximum x value must be greater than minimum x value.");
                }
            }
        }        
        public double YMin
        {
            get { return yMin; }
            set
            {
                if (value < YMax)
                {
                    yMin = value;
                    RaisePropertyChanged();
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Minimum y value must be less than maximum y value.");
                }
            }
        }
        public double YMax 
        {
            get { return yMax; }
            set 
            {
                if (value > YMin)
                {
                    yMax = value;
                    RaisePropertyChanged();
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Maximum y value must be greater than minimum y value.");
                }
            }
        }

        public RelayCommand ApplyFunctionCommand { get; private set; }
        public RelayCommand AddFunctionCommand { get; private set; }
        public RelayCommand<IList> DeleteFunctionCommand { get; private set; }

        public PlotModel PlotModel { get; private set; }

        public IHighlightingDefinition SyntaxHighlighting { get; private set; }
        public TextDocument CodeDocument { 
            get { return codeDocument; }
            set 
            {
                codeDocument = value;
                RaisePropertyChanged();
            }
        }

        public GraphFunction SelectedFunction
        {
            get { return selectedFunction; }
            set 
            {
                if (selectedFunction != value && value != null)
                {
                    SetCodeString(value);

                    selectedFunction = value;
                    RaisePropertyChanged();
                }
            }
        }
         
        public ObservableCollection<GraphFunction> Functions { get; set; }

        private void InitPlotModel()
        {
            PlotModel = new PlotModel();
            var linearAxis1 = new LinearAxis() { Position = AxisPosition.Bottom };
            PlotModel.Axes.Add(linearAxis1);
            var linearAxis2 = new LinearAxis() { Position = AxisPosition.Left };
            PlotModel.Axes.Add(linearAxis2);

            //set legend
            PlotModel.LegendBackground = OxyColor.FromArgb(200, 255, 255, 255);
            PlotModel.LegendBorder = OxyColors.Black;
        }

        private bool deleteFunctionCanExecute(IList functions)
        {
            if (functions != null)
            {
                return functions.Count > 0;
            }
            return false;
        }

        private void deleteFunctionExecute(IList obj)
        {
            
        }

        private bool ApplyFunctionCanExecute()
        {
            return true;
        }

        private void ApplyFunctionExecute()
        {
            /*
             * 
             */

            try
            {   
                var SchemeFunction = CodeDocument.Text.Eval<Callable>();
                var xdata = "(range {0} {1} {2})".Eval<Cons>(0, 30, 0.1);
                var ydata = "(map {0} {1})".Eval<Cons>(SchemeFunction, xdata);

                var xlist = xdata.ToList<double>();
                var ylist = ydata.ToList<double>();

                var xydata = xlist.Zip(ylist, (x,y) => new DataPoint(x,y));

                var lineSeries = new LineSeries() { Title = "test" };
                lineSeries.Points.AddRange(xydata);
                PlotModel.Series.Add(lineSeries);

                App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    PlotModel.InvalidatePlot(true)));

            }
            catch (SyntaxErrorException e)
            {
                MessageBox.Show("BITCHES SAYS: " + e.Message);
            }
            catch (SchemeException e)
            {
                MessageBox.Show("MORE BITCHES " + e.Message);
            }
        }

        private void LoadSyntaxHighlighting()
        {
            var uri = new Uri("/SchemeSyntax.xshd", UriKind.Relative);
            var info = Application.GetContentStream(uri);

            XshdSyntaxDefinition xshd;
            using (XmlTextReader reader = new XmlTextReader(info.Stream))
            {
                xshd = HighlightingLoader.LoadXshd(reader);
            }

            SyntaxHighlighting = HighlightingLoader.Load(xshd, HighlightingManager.Instance);
        }

        private void SetCodeString(GraphFunction value)
        {
            if (selectedFunction != null)
                selectedFunction.Code = CodeDocument.Text;

            CodeDocument.Text = value.Code;
        }
    }
}