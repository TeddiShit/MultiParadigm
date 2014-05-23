using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using oxy = OxyPlot.Wpf;
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
using System.Windows.Media;


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
        private const double EULER = 2.71828;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Functions = new ObservableCollection<GraphFunction>();

            AddFunction();

            ApplyFunctionCommand = new RelayCommand(ApplyFunctionExecute, ApplyFunctionCanExecute);
            AddFunctionCommand = new RelayCommand(AddFunction, () => Functions.Count < 12);
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
        private double xLogarithmicBase;
        private double yLogarithmicBase;
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

                if (value)
                    currentXAxis = xLogarithmicAxis;
                else
                    currentXAxis = xLinearAxis;

                currentXAxis.AbsoluteMaximum = currentXAxis.Maximum = XMax;
                currentXAxis.AbsoluteMinimum = currentXAxis.Minimum = XMin;

                if (currentXAxis == xLogarithmicAxis)
                {
                    XMin = 0;
                    YMax = Math.Abs(XMax);
                }

                PlotModel.ResetAllAxes();
                PlotModel.InvalidatePlot(false);

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
                if (value)
                    currentYAxis = yLogarithmicAxis;
                else
                    currentYAxis = yLinearAxis;

                currentYAxis.AbsoluteMaximum = currentYAxis.Maximum = YMax;
                currentYAxis.AbsoluteMinimum = currentYAxis.Minimum = YMin;

                if (currentYAxis == yLogarithmicAxis)
                {
                    YMin = 0;
                    YMax = Math.Abs(YMax);
                }

                PlotModel.ResetAllAxes();
                PlotModel.InvalidatePlot(false);
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
        public double XLogarithmicBase
        {
            get { return xLogarithmicBase; }
            set 
            {
                xLogarithmicBase = value;
                xLogarithmicAxis.Base = xLogarithmicBase;
                PlotModel.ResetAllAxes();
                PlotModel.InvalidatePlot(false);
                RaisePropertyChanged();
            }
        }
        public double YLogarithmicBase
        {
            get { return yLogarithmicBase; }
            set
            {
                yLogarithmicBase = value;
                yLogarithmicAxis.Base = yLogarithmicBase;
                PlotModel.ResetAllAxes();
                PlotModel.InvalidatePlot(false);
                RaisePropertyChanged();
            }
        }
        public double XMin
        {
            get { return xMin; }
            set
            {
                if (value < XMax)
                {
                    var old = xMin;
                    xMin = value;
                    if (xMin < old)
                        applyAllFunctions();

                    xMin = value;
                    currentXAxis.AbsoluteMinimum = currentXAxis.Minimum = value;
                    PlotModel.ResetAllAxes();
                    PlotModel.InvalidatePlot(true);
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
            get { return xMax; }
            set
            {
                if (value > XMin)
                {
                    var old = xMax;
                    xMax = value;
                    if (xMax > old)
                        applyAllFunctions();

                    
                    currentXAxis.AbsoluteMaximum = currentXAxis.Maximum = value;
                    PlotModel.ResetAllAxes();
                    PlotModel.InvalidatePlot(true);
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
                    currentYAxis.AbsoluteMinimum = currentYAxis.Minimum = value;
                    PlotModel.ResetAllAxes();
                    PlotModel.InvalidatePlot(false);
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
                    currentYAxis.AbsoluteMaximum = currentYAxis.Maximum = value;
                    PlotModel.ResetAllAxes();
                    PlotModel.InvalidatePlot(false);
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
                if (selectedFunction != value)
                {
                    if (value != null)
                        SetCodeString(value);
                    else
                        CodeDocument.Text = string.Empty;

                    selectedFunction = value;
                    RaisePropertyChanged();
                }
            }
        }
         
        public ObservableCollection<GraphFunction> Functions { get; set; }

        LinearAxis xLinearAxis = new LinearAxis() { Position = AxisPosition.Bottom  /*, PositionAtZeroCrossing = true */ };
        LinearAxis yLinearAxis = new LinearAxis() { Position = AxisPosition.Left /*,  PositionAtZeroCrossing = true*/ };
        LogarithmicAxis xLogarithmicAxis = new LogarithmicAxis() { Position = AxisPosition.Bottom};
        LogarithmicAxis yLogarithmicAxis = new LogarithmicAxis() { Position = AxisPosition.Left};

        private Axis currentXAxis 
        {
            get 
            {
                return PlotModel.Axes[0];
            }
            set 
            {
                PlotModel.Axes[0] = value;
            }
        }
        private Axis currentYAxis
        {
            get
            {
                return PlotModel.Axes[1];
            }
            set
            {
                PlotModel.Axes[1] = value;
            }
        }

        private void InitPlotModel()
        {
            PlotModel = new PlotModel();
            xLinearAxis.AbsoluteMaximum = xLinearAxis.Maximum = XMax;
            xLinearAxis.AbsoluteMinimum = xLinearAxis.Minimum = XMin;
            PlotModel.Axes.Add(xLinearAxis);

            yLinearAxis.AbsoluteMaximum = yLinearAxis.Maximum = YMax;
            yLinearAxis.AbsoluteMinimum = yLinearAxis.Minimum = YMin;

            PlotModel.Axes.Add(yLinearAxis);

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

        private void deleteFunctionExecute(IList functions)
        {
            var toBeDeleted = new List<GraphFunction>(functions.Cast<GraphFunction>());

            foreach (var func in toBeDeleted)
            {
                var function = func as GraphFunction;
                if (function != null)
                {
                    RemoveFunction(function);
                }
            }
        }

        private bool suppressStepPropertyChanges = false;
        private bool suppressSamplePropertyChanges = false;

        private void function_propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var graphFunc = sender as GraphFunction;

            if (graphFunc != null)
            {
                if (!suppressStepPropertyChanges && e.PropertyName == "Step")
                {
                    suppressSamplePropertyChanges = true;
                    graphFunc.Samples = SchemeMathWrapper.StepToSamples(XMin, XMax, graphFunc.Step);
                    suppressSamplePropertyChanges = false;                    
                }
                else if (!suppressSamplePropertyChanges && e.PropertyName == "Samples")
                {
                    suppressStepPropertyChanges = true;
                    graphFunc.Step = SchemeMathWrapper.SamplesToStep(XMin, XMax, graphFunc.Samples);
                    suppressStepPropertyChanges = false;
                }
                else if (e.PropertyName == "IsEnabled")
                {
                    SetFunctionVisibility(graphFunc);
                    PlotModel.InvalidatePlot(false);
                }
            }
        }

        private bool ApplyFunctionCanExecute()
        {
            return SelectedFunction != null;
        }

        private void ApplyFunctionExecute()
        {
            if (ApplyFunctionCanExecute())
            {
                SetCodeString(SelectedFunction);
                applyFunction(SelectedFunction);
                PlotModel.InvalidatePlot(true);
            }
        }

        private void applyAllFunctions()
        {
            foreach (var func in Functions)
            {
                applyFunction(func);
            }
        }        

        private void applyFunction(GraphFunction function)
        {
            try
            {
                InitFunctionSeries(function);

                var xydata = SchemeMathWrapper.CalcPlotData(function.Code, XMin, XMax, function.Step);
                var xydatapoints = xydata.Select( (xy) => new DataPoint(xy.Item1, xy.Item2));
                function.PlotSeries.Points.Clear();
                function.PlotSeries.Points.AddRange(xydatapoints);
                function.PlotSeries.Title = function.Name;
                
                if (function.ShowDerivative)
                {
                    var dxydata = SchemeMathWrapper.CalcDerivedPlotData(function.Code, XMin, XMax, function.Step);
                    var dxydatapoints = dxydata.Select((xy) => new DataPoint(xy.Item1, xy.Item2));
                    function.DerivedSeries.Points.Clear();
                    function.DerivedSeries.Points.AddRange(dxydatapoints);
                    function.DerivedSeries.Title = "'" + function.Name;
                                        
                }

                if (function.ShowIntegral)
                {
                    //TODO: RIGHT, MIDDLE, LEFT!

                    var coords = SchemeMathWrapper.CalcMidpointIntegralCoords(function.Code, function.IntegralMin,
                        function.IntegralMax, function.IntegralRes);

                    var deltaX = SchemeMathWrapper.CalcDeltaX(function.IntegralMin,
                        function.IntegralMax, function.IntegralRes);

                    function.IntegralSeries.Points.Clear();
                    function.IntegralSeries.Points2.Clear();
                    foreach (var coord in coords)
                    {
                        function.IntegralSeries.Points.Add(new DataPoint(coord.Item1, coord.Item2));
                        function.IntegralSeries.Points.Add(new DataPoint(coord.Item1 + deltaX, coord.Item2));                        
                    }

                    var bottomLineSeries = new LineSeries();
                    function.IntegralSeries.Points2.Add(new DataPoint(function.IntegralMin, 0));
                    function.IntegralSeries.Points2.Add(new DataPoint(function.IntegralMax, 0));

                    var area = SchemeMathWrapper.CalcDefiniteIntegral(function.IntegralMin,
                        function.IntegralMax, function.IntegralRes, coords);

                    function.IntegralSeries.Title = function.Name + "(A: " + area.ToString("G5") + ")";
                }

                SetFunctionVisibility(function);
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

        private static void SetFunctionVisibility(GraphFunction function)
        {
            function.PlotSeries.IsVisible = function.IsEnabled;
            function.DerivedSeries.IsVisible = function.IsEnabled && function.ShowDerivative;
            function.IntegralSeries.IsVisible = function.IsEnabled && function.ShowIntegral;
        }

        int nextColor = 0;

        private OxyColor GetNextColor()
        {
            var color = PlotModel.DefaultColors[nextColor];

            nextColor = ++nextColor % PlotModel.DefaultColors.Count;

            return color;
        }

        private void InitFunctionSeries(GraphFunction function)
        {
            if (function.PlotSeries == null)
            {
                function.PlotSeries = new LineSeries();
                function.PlotSeries.Color = GetNextColor();                
                PlotModel.Series.Add(function.PlotSeries);

                function.Color = new SolidColorBrush(oxy.ConverterExtensions.ToColor(function.PlotSeries.ActualColor));
            }
            if (function.DerivedSeries == null)
            {
                function.DerivedSeries = new LineSeries();
                function.DerivedSeries.Color = function.PlotSeries.ActualColor;
                function.DerivedSeries.LineStyle = LineStyle.Dash;
                PlotModel.Series.Add(function.DerivedSeries);
            }
            if (function.IntegralSeries == null)
            {
                function.IntegralSeries = new AreaSeries();
                function.IntegralSeries.Color = function.PlotSeries.ActualColor;
                PlotModel.Series.Add(function.IntegralSeries);
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

        private void RemoveFunction(GraphFunction function)
        {
            Functions.Remove(function);
            if (function.PlotSeries != null)
                PlotModel.Series.Remove(function.PlotSeries);
            if (function.DerivedSeries != null)
                PlotModel.Series.Remove(function.DerivedSeries);
            if (function.IntegralSeries != null)
                PlotModel.Series.Remove(function.IntegralSeries);

            PlotModel.InvalidatePlot(true);
        }

        private void AddFunction()
        {
            var func = new GraphFunction();
            func.PropertyChanged += function_propertyChanged;
            Functions.Add(func);
        }
    }
}