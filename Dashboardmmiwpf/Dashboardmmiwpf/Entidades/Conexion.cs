using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboardmmiwpf
{
    public class Conexion
    {
        public Conexion()
        {

        }

        public DataTable GetDataViews()
        {
            SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

            thisConnection.Open();

            string Get_Data = "SELECT * FROM DataView";

            SqlCommand cmd = thisConnection.CreateCommand();
            cmd.CommandText = Get_Data;

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("emp");
            sda.Fill(dt);

            thisConnection.Close();
            return dt;
        }

        public DataTable GetDataViews(int datasourceId)
        {
            SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

            thisConnection.Open();

            string Get_Data = "SELECT * FROM DataView WHERe DataSourceId = " + datasourceId.ToString();

            SqlCommand cmd = thisConnection.CreateCommand();
            cmd.CommandText = Get_Data;

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("emp");
            sda.Fill(dt);

            thisConnection.Close();
            return dt;
        }

        public DataTable GetDataViewsIndex(int dataViewId)
        {
            SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

            thisConnection.Open();

            string Get_Data = "SELECT * FROM DataView WHERe DataViewId = " + dataViewId.ToString();

            SqlCommand cmd = thisConnection.CreateCommand();
            cmd.CommandText = Get_Data;

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("emp");
            sda.Fill(dt);

            thisConnection.Close();
            return dt;
        }

        public DataTable GetGraficoIndex(int GraficoID)
        {
            SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

            thisConnection.Open();

            string Get_Data = "SELECT * FROM Grafico WHERE id = " + GraficoID.ToString();

            SqlCommand cmd = thisConnection.CreateCommand();
            cmd.CommandText = Get_Data;

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("emp");
            sda.Fill(dt);

            thisConnection.Close();
            return dt;
        }

        public DataTable GetDashboardIndex(int index)
        {
            SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

            thisConnection.Open();

            string Get_Data = "SELECT * FROM Dashboard WHERe DashboardID = " + index.ToString();

            SqlCommand cmd = thisConnection.CreateCommand();
            cmd.CommandText = Get_Data;

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("emp");
            sda.Fill(dt);

            thisConnection.Close();
            return dt;
        }

        public DataTable GetProject(int index)
        {
            SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

            thisConnection.Open();

            string Get_Data = "SELECT * FROM Projects WHERe ProjectID = " + index.ToString();

            SqlCommand cmd = thisConnection.CreateCommand();
            cmd.CommandText = Get_Data;

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("emp");
            sda.Fill(dt);

            thisConnection.Close();
            return dt;
        }

        public DataTable GetDBs()
        {
            SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=master;Integrated Security=True");

            thisConnection.Open();

            string Get_Data = "SELECT name, database_id, create_date, state FROM sys.databases";

            SqlCommand cmd = thisConnection.CreateCommand();
            cmd.CommandText = Get_Data;

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("emp");
            sda.Fill(dt);

            thisConnection.Close();
            return dt;
        }


        public DataTable GetGraphics(int DashboardId)
        {
            SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

            thisConnection.Open();

            string Get_Data = "SELECT * FROM Grafico WHERE dashboardId = " + DashboardId.ToString();

            SqlCommand cmd = thisConnection.CreateCommand();
            cmd.CommandText = Get_Data;

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("emp");
            sda.Fill(dt);

            thisConnection.Close();
            return dt;
        }



        public DataTable getTables(String nombre)
        {
            try
            {
                SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=" + nombre.ToString() + ";Integrated Security=True");

                thisConnection.Open();

                string Get_Data = @"SELECT TABLE_NAME, TABLE_TYPE 
                                    FROM INFORMATION_SCHEMA.TABLES 
                                    WHERE TABLE_CATALOG = '" + nombre.ToString() + "'";
                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("emp");
                sda.Fill(dt);

                thisConnection.Close();
                return dt;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public void updatedatasource(int id,string titulo,string username, string password)
        {
                
            try
            {
                SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

                thisConnection.Open();

                string Get_Data = @"UPDATE DataSource SET DataSourceTitle = '"+ titulo +"', DataSourceUser = '"+ username +"', DataSourcePassword = '"+ password  +"' WHERE DataSourceID = " + id.ToString();

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                cmd.ExecuteNonQuery();

                thisConnection.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public void updateDataView(String Query, String DataSourceNombre, String DataViewName, int DataSourceId, int DataViewId)
        {

            try
            {
                SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

                thisConnection.Open();

                string Get_Data = @"UPDATE DataView SET DataSourceId = '" + DataSourceId + "', QueryString = '" + Query + "', DataSourceNombre = '" + DataViewName + "', DataViewName = '" + DataSourceNombre + "' WHERE DataViewId = " + DataViewId.ToString();
                
                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                cmd.ExecuteNonQuery();

                thisConnection.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public void updateGraphic(String Query, String ejex, String ejey, int GraficoId)
        {

            try
            {
                Query = Query.Replace("*",ejex + ", " + ejey);

                SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

                thisConnection.Open();

                string Get_Data = @"UPDATE Grafico SET query = '" + Query + "', ejex = '" + ejex + "', ejey = '" + ejey + "' WHERE id = " + GraficoId.ToString();

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                cmd.ExecuteNonQuery();

                thisConnection.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public void updategraphicPosition(double posx, double posy, double ancho, double alto, int GraficoId)
        {

            try
            {

                SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

                thisConnection.Open();

                string Get_Data = @"UPDATE Grafico SET posx = '" + posx.ToString() + "', posy = '" + posy.ToString() + "', ancho = '" + ancho.ToString() + "', alto = '" + alto.ToString() + "' WHERE id = " + GraficoId.ToString();

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                cmd.ExecuteNonQuery();

                thisConnection.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public void updateDashboard(int id, String titulo)
        {

            try
            {
                SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

                thisConnection.Open();

                string Get_Data = @"UPDATE Dashboard SET Nombre = '" + titulo + "' WHERE DashboardID = " + id.ToString();

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                cmd.ExecuteNonQuery();

                thisConnection.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public void updateProject(String Nombre, int ID)
        {

            try
            {
                SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

                thisConnection.Open();

                string Get_Data = @"UPDATE Projects SET Nombre = '" + Nombre + "' where ProjectID = " + ID.ToString();

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                cmd.ExecuteNonQuery();

                thisConnection.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public void deleteDatasource(int id)
        {
            
            try
            {
                SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

                thisConnection.Open();

                string Get_Data = @"DELETE FROM DataSource WHERE DataSourceID = " + id.ToString();
                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("emp");
                sda.Fill(dt);

                thisConnection.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public void deleteDataview(int id)
        {

            try
            {
                SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

                thisConnection.Open();

                string Get_Data = @"DELETE FROM DataView WHERE DataViewId = " + id.ToString();
                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("emp");
                sda.Fill(dt);

                thisConnection.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public void deleteDashboard(int id)
        {

            try
            {
                SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

                thisConnection.Open();

                string Get_Data = @"DELETE FROM Dashboard WHERE DashboardID = " + id.ToString();
                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("emp");
                sda.Fill(dt);

                thisConnection.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public void deleteGrupo(int id)
        {

            try
            {
                SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

                thisConnection.Open();

                string Get_Data = @"DELETE FROM Projects WHERE ProjectID = " + id.ToString();
                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("emp");
                sda.Fill(dt);

                thisConnection.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public void deleteGrafico(int id)
        {

            try
            {
                SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

                thisConnection.Open();

                string Get_Data = @"DELETE FROM Grafico WHERE id = " + id.ToString();
                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("emp");
                sda.Fill(dt);

                thisConnection.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

        }


        public DataTable getRegistros(String nombre, String db,ref bool error)
        {
            error = false;

            SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=" + db.ToString() + ";Integrated Security=True");

            thisConnection.Open();

            string Get_Data = nombre;

            SqlCommand cmd = thisConnection.CreateCommand();
            cmd.CommandText = Get_Data;
            DataTable dt = new DataTable();

            try
            {
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                dt = new DataTable("emp");
                sda.Fill(dt);
            }
            catch (Exception ex)
            {
                error = true;
                
            }
            finally
            {
                thisConnection.Close();
               
            }

            return dt;
        }

        public void GuardarDataSource(String Title, String Password, String User, String db, String Serverame)
        {
            SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

            string query = "INSERT INTO dbo.DataSource (DataSourceNombre, ServerName, DataSourceuser, DataSourcePassword, DataSourceTitle) " +
                   "VALUES (@nombre, @server, @user, @pass, @title) ";

            using (SqlCommand cmd = new SqlCommand(query, thisConnection))
            {
                // define parameters and their values
                cmd.Parameters.Add("@nombre", SqlDbType.VarChar, 50).Value = db;    
                cmd.Parameters.Add("@server", SqlDbType.VarChar, 50).Value = Serverame;
                cmd.Parameters.Add("@user", SqlDbType.VarChar, 50).Value = User;
                cmd.Parameters.Add("@pass", SqlDbType.VarChar, 50).Value = Password;
                cmd.Parameters.Add("@title", SqlDbType.VarChar,50).Value = Title;

                // open connection, execute INSERT, close connection
                thisConnection.Open();
                cmd.ExecuteNonQuery();
                thisConnection.Close(); 
            }
        }

        public void GuardarDataView(String Query, String DataSourceNombre, String DataViewName, int DataSourceId)
        {
            SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

            string query = "INSERT INTO dbo.DataView (DataSourceId, DataSourceNombre, DataViewName, QueryString) " +
                   "VALUES (@dsid, @dsnombre, @dvname, @query) ";

            using (SqlCommand cmd = new SqlCommand(query, thisConnection))
            {
                // define parameters and their values
                cmd.Parameters.Add("@dsid", SqlDbType.Int).Value = DataSourceId;
                cmd.Parameters.Add("@dsnombre", SqlDbType.VarChar, 50).Value = DataViewName;
                cmd.Parameters.Add("@dvname", SqlDbType.VarChar, 50).Value = DataSourceNombre;
                cmd.Parameters.Add("@query", SqlDbType.VarChar, -1).Value = Query;

                // open connection, execute INSERT, close connection
                thisConnection.Open();
                cmd.ExecuteNonQuery();
                thisConnection.Close();
            }
        }

        public DataTable GetDataSource()
        {

            SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

            thisConnection.Open();

            string Get_Data = "SELECT * FROM DataSource";

            SqlCommand cmd = thisConnection.CreateCommand();
            cmd.CommandText = Get_Data;

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("emp");
            sda.Fill(dt);

            thisConnection.Close();
            return dt;

        }

        public void GuardarProject(string nombre)
        {
            SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

            string query = "INSERT INTO dbo.Projects (Nombre)" +
                   "VALUES (@nombre) ";

            using (SqlCommand cmd = new SqlCommand(query, thisConnection))
            {
                // define parameters and their values
                cmd.Parameters.Add("@nombre", SqlDbType.VarChar, 50).Value = nombre;

                // open connection, execute INSERT, close connection
                thisConnection.Open();
                cmd.ExecuteNonQuery();
                thisConnection.Close();
            }
        }

        public DataTable GetProjects()
        {
            SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

            thisConnection.Open();

            string Get_Data = "SELECT * FROM Projects";

            SqlCommand cmd = thisConnection.CreateCommand();
            cmd.CommandText = Get_Data;

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("emp");
            sda.Fill(dt);

            thisConnection.Close();
            return dt;
        }

       public DataTable GetDashboards(int id)
        {
            SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

            thisConnection.Open();

            string Get_Data = "SELECT * FROM Dashboard WHERE ProjectID = " + id.ToString();

            SqlCommand cmd = thisConnection.CreateCommand();
            cmd.CommandText = Get_Data;

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("emp");
            sda.Fill(dt);

            thisConnection.Close();
            return dt;
        }

        public DataTable GetDashboardsIndex(int id)
        {
            SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

            thisConnection.Open();

            string Get_Data = "SELECT * FROM Dashboard WHERE DashboardID = " + id.ToString();

            SqlCommand cmd = thisConnection.CreateCommand();
            cmd.CommandText = Get_Data;

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("emp");
            sda.Fill(dt);

            thisConnection.Close();
            return dt;
        }

        public void GuardarDashboard(string nombre, int Project,int DsId)
        {
            SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

            string query = "INSERT INTO dbo.Dashboard (Nombre,ProjectID,DataSourceId)" +
                   "VALUES (@nombre,@projectID,@DsId) ";

            using (SqlCommand cmd = new SqlCommand(query, thisConnection))
            {
                // define parameters and their values
                cmd.Parameters.Add("@nombre", SqlDbType.VarChar, 50).Value = nombre;
                cmd.Parameters.Add("@projectID", SqlDbType.Int).Value = Project;
                cmd.Parameters.Add("@DsId", SqlDbType.Int).Value = DsId;

                // open connection, execute INSERT, close connection
                thisConnection.Open();
                cmd.ExecuteNonQuery();
                thisConnection.Close();
            }
        }

        public void GuardarGrafico(int DashboardId, int tipoGrafico, double posx, double posy)
        {
            SqlConnection thisConnection = new SqlConnection(@"Data Source=localhost;Initial Catalog=MMIDashBoard;Integrated Security=True");

            string query = "INSERT INTO dbo.Grafico (query,tipoGrafico,posx,posy,ancho,alto,dashboardId,ejex,ejey)" +
                   "VALUES (@query,@tipoGrafico,@posx,@posy,@ancho,@alto,@dashboardId,@ejex,@ejey) ";

            using (SqlCommand cmd = new SqlCommand(query, thisConnection))
            {
                // define parameters and their values
                cmd.Parameters.Add("@query", SqlDbType.Text).Value = "";
                cmd.Parameters.Add("@tipoGrafico", SqlDbType.Int).Value = tipoGrafico;
                cmd.Parameters.Add("@posx", SqlDbType.Decimal).Value = Convert.ToDecimal(posx);
                cmd.Parameters.Add("@posy", SqlDbType.Decimal).Value = Convert.ToDecimal(posy);
                cmd.Parameters.Add("@ancho", SqlDbType.Decimal).Value = 250;
                cmd.Parameters.Add("@alto", SqlDbType.Decimal).Value = 250;
                cmd.Parameters.Add("@dashboardId", SqlDbType.Int).Value = DashboardId;
                cmd.Parameters.Add("@ejex", SqlDbType.Text).Value = "";
                cmd.Parameters.Add("@ejey", SqlDbType.Text).Value = "";

                // open connection, execute INSERT, close connection
                thisConnection.Open();
                cmd.ExecuteNonQuery();
                thisConnection.Close();
            }
        }


        public bool testconnection(string databasename,string server)
        {
            try
            {
                SqlConnection thisConnection = new SqlConnection(@"Data Source="+ server + ";Initial Catalog=" + databasename + ";Integrated Security=True");

                thisConnection.Open();

                string Get_Data = "SELECT 1";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("emp");
                sda.Fill(dt);

                thisConnection.Close();
                return true;

            }
            catch(Exception e)
            {
                return false;
            }
            
        }
    }
}
