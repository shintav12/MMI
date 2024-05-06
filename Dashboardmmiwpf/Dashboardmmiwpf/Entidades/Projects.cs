using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboardmmiwpf
{
    public class Projects
    {

        public int ProjectId { get; set; }
        public string Nombre { get; set; }

        public List<Dashboard> lstDashboard { get; set; }


        public List<Projects> DataTabletoList(DataTable dt)
        {

            List<Projects> listName = dt.AsEnumerable().Select(m => new Projects()
            {
                ProjectId = m.Field<int>("ProjectID"),
                Nombre = m.Field<string>("Nombre"),

            }).AsParallel().ToList();

            return listName;
        }

        public void DataTabletoListView(DataTable dt)
        {

            List<Dashboard> listName = dt.AsEnumerable().Select(m => new Dashboard()
            {
                DashboardId = m.Field<int>("DashboardID"),
                ProjectId = m.Field<int>("ProjectID"),
                Nombre = m.Field<string>("Nombre"),
                DataSourceId = m.Field<int>("DataSourceId")        
            }).AsParallel().ToList();

            lstDashboard = listName;
        }
    }
}
