using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboardmmiwpf
{
    public class Dashboard
    {
        public int DashboardId { get; set; }
        public string Nombre { get; set; }
        public int DataSourceId { get; set; }
        public int ProjectId { get; set; }
        public List<DragDropElement> ElementosArrastrables { get; set; }
        public List<Grafico> Grafics { get; set; }

        public void graficoToDragoDrop()
        {
            ElementosArrastrables = new List<DragDropElement>();
            foreach (Grafico c in Grafics)
            {
                DragDropElement aux = new DragDropElement();
                //aux.Child = c.chart;
                //ElementosArrastrables.Add(aux);
            }
        }
    }
}
