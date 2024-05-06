using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboardmmiwpf
{
    public class DataSource
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public List<DataView> dataview { get; set; }
        public string Servername { get; set; }
        public string user { get; set; }
        public string password { get; set; }
        public string titulo { get; set; }

        public List<DataSource> DataTabletoList(DataTable dt)
        {

            List<DataSource> listName = dt.AsEnumerable().Select(m => new DataSource()
            {
                id = m.Field<int>("DataSourceID"),
                nombre = m.Field<string>("DataSourceNombre"),
                Servername = m.Field<string>("ServerName"),
                user = m.Field<string>("DataSourceuser"),
                password = m.Field<string>("DataSourcePassword"),
                titulo = m.Field<string>("DataSourceTitle")

            }).AsParallel().ToList();

            return listName;
        }

        public void DataTabletoListView(DataTable dt)
        {

            List<DataView> listName = dt.AsEnumerable().Select(m => new DataView()
            {
                id = m.Field<int>("DataViewId"),
                nombre = m.Field<string>("DataViewName"),
                DsId = m.Field<int>("DataSourceId"),
                DsNombre = m.Field<string>("DataSourceNombre"),
                Query = m.Field<string>("QueryString")

            }).AsParallel().ToList();

            dataview = listName;
        }

    }
}
