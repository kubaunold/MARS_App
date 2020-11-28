using System;
using System.Collections.Generic;
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
using System.Windows.Threading;
//using MARS_App.ViewModel;

using LiveCharts;
using LiveCharts.Wpf;

namespace View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();



            //DispatcherTimer timer = new DispatcherTimer(TimeSpan.FromSeconds(1.0), DispatcherPriority.Normal, delegate
            //{
            //    double newValue = 0.0;

            //                //double currentValue = (double)GetValue(ValueProperty);

            //                if (BarChart == double.MaxValue)
            //    {
            //        newValue = 0.0;
            //    }
            //    else
            //    {
            //        newValue = BarChart.ValueProperty += 1.0;
            //    }

            //    SetValue(ValueProperty, newValue);
            //}, Dispatcher);




            DataContext = new ViewModel.ViewModel();
        }



        //public Func<ChartPoint, string> PointLabel { get; set; }

        //private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        //{
        //    var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

        //    //clear selected slice.
        //    foreach (PieSeries series in chart.Series)
        //        series.PushOut = 0;

        //    var selectedSeries = (PieSeries)chartpoint.SeriesView;
        //    selectedSeries.PushOut = 8;
        //}

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{

        //}
    }



}
