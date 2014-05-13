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
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
            }

            ApplyFunctionCommand = new RelayCommand(ApplyFunctionExecute, ApplyFunctionCanExecute);

            Functions = new ObservableCollection<GraphFunction>();
            Functions.Add(new GraphFunction());
            Functions.Add(new GraphFunction());
            Functions.Add(new GraphFunction());
            Functions.Add(new GraphFunction());
            Functions.Add(new GraphFunction());
            Functions.Add(new GraphFunction());

            InitPlotModel();
            LoadSyntaxHighlighting();
            IronSchemeBridge.ResetEnvironment();
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

        private void InitPlotModel()
        {
            PlotModel = new PlotModel();
            var linearAxis1 = new LinearAxis();
            linearAxis1.Position = AxisPosition.Bottom;
            PlotModel.Axes.Add(linearAxis1);
            var linearAxis2 = new LinearAxis();
            PlotModel.Axes.Add(linearAxis2);
            PlotModel.LegendBackground = OxyColor.FromArgb(200, 255, 255, 255);
            PlotModel.LegendBorder = OxyColors.Black;
        }

        public RelayCommand ApplyFunctionCommand { get; private set; }

        public PlotModel PlotModel { get; private set; }

        public IHighlightingDefinition SyntaxHighlighting { get; private set; }

        private TextDocument codeDocument = new TextDocument();
        public TextDocument CodeDocument { 
            get { return codeDocument; }
            set 
            {
                codeDocument = value;
                RaisePropertyChanged();
            }
        }

        private GraphFunction selectedFunction;
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

        private void SetCodeString(GraphFunction value)
        {
            if (selectedFunction != null)
                selectedFunction.Code = CodeDocument.Text;

            CodeDocument.Text = value.Code;
        }

        public ObservableCollection<GraphFunction> Functions { get; set; }

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
    }
}