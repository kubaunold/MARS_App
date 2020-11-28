using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace View
{
    /// <summary>
    /// Interaction logic for BarChart.xaml
    /// </summary>
    public partial class BarChart : UserControl, INotifyPropertyChanged
    {



        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }


        private double maxValue;    // private field
        public double MaxValue      // property (used for gainign access to a `protected` field)
        {
            get { return maxValue; }
            set
            {
                maxValue = value;
                UpdateBarHeight();
                NotifyPropertyChanged("MaxValue");
            }
        }

        private double barHeight;
        public double BarHeight
        {
            get { return barHeight; }
            private set { barHeight = value; NotifyPropertyChanged("BarHeight"); }
        }

        private Brush color;
        public Brush Color
        {
            get { return color; }
            set { color = value; NotifyPropertyChanged("Color"); }
        }

        private void UpdateBarHeight()
        {
            if (maxValue > 0)
            {
                //var percent = (_value * 100) / maxValue;
                var percent = ((double)GetValue(ValueProperty) * 100) / maxValue;
                BarHeight = (percent * this.ActualHeight) / 100;
            }
        }


        //private double _value;  // field
        //public double Value // that's a property
        //{
        //    get { return _value; }  // get method
        //    set                     // set method
        //    {
        //        _value = value;     // blue `value` is a keyword here; it represents the value we assign to the property
        //        UpdateBarHeight();
        //        NotifyPropertyChanged("Value");
        //    }
        //}


        // CLR property that wraps around DependencyProperty
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(BarChart), new PropertyMetadata(7.0));




        public BarChart()
        {
            InitializeComponent();
            this.DataContext = this;
            Color = Brushes.Black;

            DispatcherTimer timer = new DispatcherTimer(TimeSpan.FromSeconds(1.0), DispatcherPriority.Normal, delegate
            {
            double newValue = 0.0;

            if (Value == double.MaxValue)
            {
                newValue = 0.0;
        }
        else
        {
            newValue = Value += 1.0;
        }

    SetValue(ValueProperty, newValue);
}, Dispatcher);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateBarHeight();
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateBarHeight();
        }
    }
}
