using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultiParadigmGrapher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void logView_Loaded(object sender, RoutedEventArgs args)
        {
            var itemsControl = sender as ItemsControl;
            var scrollViewer = VisualTreeHelper.GetChild(itemsControl, 0) as ScrollViewer;

            var obscollection = itemsControl.ItemsSource as INotifyCollectionChanged;

            obscollection.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    scrollViewer.ScrollToBottom();
                }
            };

        }
    }
}
