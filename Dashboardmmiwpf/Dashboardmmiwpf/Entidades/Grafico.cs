using Microsoft.Kinect.Wpf.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Dashboardmmiwpf
{
    public class Grafico
    {
        public double posicionX { get; set; }
        public double posicionY { get; set; }
        public int posX { get; set; }
        public int posY { get; set; }
        public double ancho { get; set; }
        public double alto { get; set; }
        public string ejex { get; set; }
        public string ejey { get; set; }
        public int tipoGrafico { get; set; }
        public int dashboardid { get; set; }
        public int GraficoId { get; set; }
        public string query { get; set;}

        public List<Grafico> DataTabletoList(DataTable dt)
        {

            List<Grafico> listName = dt.AsEnumerable().Select(m => new Grafico()
            {
                GraficoId = m.Field<int>("id"),
                query    = m.Field<string>("query"),
                tipoGrafico = m.Field<int>("tipoGrafico"),
                posicionX = Convert.ToDouble(m.Field<decimal>("posx")),
                posicionY = Convert.ToDouble(m.Field<decimal>("posy")),
                ancho =     Convert.ToDouble(m.Field<decimal>("ancho")),
                alto =      Convert.ToDouble(m.Field<decimal>("alto")),
                ejex = m.Field<string>("ejex"),
                ejey = m.Field<string>("ejey"),
                dashboardid = m.Field<int>("dashboardId")

            }).AsParallel().ToList();

            return listName;
        }
    }

}



//public Grafico(DragDropElement c, string r)
//{
//    Canvas d = new Canvas();
//    ChartControl chart = new ChartControl();
//    chart.Height = 200;
//    chart.Width = 200;
//    switch (r)
//    {
//        case "BARRAS":

//            // Create a diagram.
//            Diagram2D diagram = new XYDiagram2D();
//            chart.Diagram = diagram;


//            // Create a bar series.
//            BarSideBySideSeries2D series = new BarSideBySideSeries2D();
//            diagram.Series.Add(series);

//            // Add points to the series.
//            series.Points.Add(new SeriesPoint("A", 1));
//            series.Points.Add(new SeriesPoint("B", 3));
//            series.Points.Add(new SeriesPoint("C", 5));
//            series.Points.Add(new SeriesPoint("D", 2));
//            series.Points.Add(new SeriesPoint("E", 7));


//            break;
//        case "EMBUDO":

//            SimpleDiagram2D funnel = new SimpleDiagram2D();
//            // Create a funnel series.
//            chart.Diagram = funnel;
//            Series serie1 = new FunnelSeries2D();
//            serie1.LegendTextPattern = "{}{A}: {VP: ##.##%}";

//            // Add points to the series.
//            serie1.Points.Add(new SeriesPoint("A", 48.5));
//            serie1.Points.Add(new SeriesPoint("B", 29.6));
//            serie1.Points.Add(new SeriesPoint("C", 17.1));
//            serie1.Points.Add(new SeriesPoint("D", 13.3));
//            serie1.Points.Add(new SeriesPoint("E", 11.6));
//            funnel.Series.Add(serie1);

//            // Add the series to the chart.


//            // Add the chart to the form.

//            break;
//        case "ÁREA":

//            Diagram2D area = new XYDiagram2D();
//            chart.Diagram = area;


//            // Create a bar series.
//            AreaSeries2D areaserie = new AreaSeries2D();
//            area.Series.Add(areaserie);

//            // Add points to the series.
//            areaserie.Points.Add(new SeriesPoint("A", 1));
//            areaserie.Points.Add(new SeriesPoint("B", 3));
//            areaserie.Points.Add(new SeriesPoint("C", 5));
//            areaserie.Points.Add(new SeriesPoint("D", 2));
//            areaserie.Points.Add(new SeriesPoint("E", 7));

//            // Add points to the series.

//            break;

//        case "CIRCULAR":

//            SimpleDiagram2D pie = new SimpleDiagram2D();
//            // Create a funnel series.
//            chart.Diagram = pie;
//            Series serpie = new PieSeries2D();
//            serpie.LegendTextPattern = "{}{A}: {VP: ##.##%}";

//            // Add points to the series.
//            serpie.Points.Add(new SeriesPoint("A", 48.5));
//            serpie.Points.Add(new SeriesPoint("B", 29.6));
//            serpie.Points.Add(new SeriesPoint("C", 17.1));
//            serpie.Points.Add(new SeriesPoint("D", 13.3));
//            serpie.Points.Add(new SeriesPoint("E", 11.6));


//            pie.Series.Add(serpie);
//            break;
//        case "DONA":

//            SimpleDiagram2D dona = new SimpleDiagram2D();
//            // Create a funnel series.
//            chart.Diagram = dona;
//            Series sieredona = new PieSeries2D();

//            sieredona.LegendTextPattern = "{}{A}: {VP: ##.##%}";

//            // Add points to the series.
//            sieredona.Points.Add(new SeriesPoint("A", 48.5));
//            sieredona.Points.Add(new SeriesPoint("B", 29.6));
//            sieredona.Points.Add(new SeriesPoint("C", 17.1));
//            sieredona.Points.Add(new SeriesPoint("D", 13.3));
//            sieredona.Points.Add(new SeriesPoint("E", 11.6));


//            dona.Series.Add(sieredona);
//            break;
//        case "APILADAS":

//            Diagram2D apiladas = new XYDiagram2D();
//            chart.Diagram = apiladas;


//            // Create a bar series.
//            BarFullStackedSeries2D seriea1 = new BarFullStackedSeries2D();
//            apiladas.Series.Add(seriea1);

//            // Add points to the series.
//            seriea1.Points.Add(new SeriesPoint("A", 2));
//            seriea1.Points.Add(new SeriesPoint("B", 2));
//            seriea1.Points.Add(new SeriesPoint("C", 1));
//            seriea1.Points.Add(new SeriesPoint("D", 6));
//            BarFullStackedSeries2D seriea2 = new BarFullStackedSeries2D();
//            apiladas.Series.Add(seriea2);

//            // Add points to the series.
//            seriea2.Points.Add(new SeriesPoint("A", 4));
//            seriea2.Points.Add(new SeriesPoint("B", 3));
//            seriea2.Points.Add(new SeriesPoint("C", 2));
//            seriea2.Points.Add(new SeriesPoint("D", 1));

//            break;
//    }


//    d.Background = Brushes.Black;
//    d.Width = 200;
//    d.Height = 600;

//    c.Child = chart;

//}