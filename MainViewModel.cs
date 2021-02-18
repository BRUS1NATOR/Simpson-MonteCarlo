using System.Collections.Generic;
using System.Windows;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Simpson
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            Create();
        }

        public void SetPoints(List<Point> points)
        {
            PointsPlus.Clear();
            PointsMinus.Clear();
            foreach (Point p in points)
            {
                if (p.Y == 0)
                {
                    this.PointsPlus.Add(new DataPoint(p.X, p.Y));
                    this.PointsMinus.Add(new DataPoint(p.X, p.Y));
                }
                else if (p.Y > 0)
                {
                    this.PointsPlus.Add(new DataPoint(p.X, p.Y));
                }
                else
                {
                    this.PointsMinus.Add(new DataPoint(p.X, p.Y));
                }
            }
            this.MyModel.ResetAllAxes();
            this.MyModel.InvalidatePlot(true);

        }

        private void Create()
        {
            this.MyModel = new PlotModel { Title="CHART", PlotType = PlotType.Cartesian };
            this.PointsPlus = new List<DataPoint>();
            this.PointsMinus = new List<DataPoint>();
            MyModel.Series.Add(new AreaSeries
            {
                Color = OxyColors.Green,
                ItemsSource = PointsPlus,
                LineStyle = LineStyle.None,
                StrokeThickness = 0.1f
            }) ;

            MyModel.Series.Add(new AreaSeries
            {
                Color = OxyColors.Blue,
                ItemsSource = PointsMinus,
                LineStyle = LineStyle.None,
                StrokeThickness = 0.1f      
            }) ;
        }

        public IList<DataPoint> PointsPlus { get; private set; }
        public IList<DataPoint> PointsMinus { get; private set; }

        public PlotModel MyModel { get; private set; }
    }
}
