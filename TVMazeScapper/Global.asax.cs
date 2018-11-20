using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TVMazeScapper.Models;

namespace TVMazeScapper
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            CreateDatabase();
            GetShows();
        }

        private void CreateDatabase()
        {
            string PathAndFile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TvMazeDatabase.sqlite";
            if (!File.Exists(PathAndFile)) { SQLiteConnection.CreateFile(PathAndFile); }
            
            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + PathAndFile + ";Version=3;");
            m_dbConnection.Open();

            string sql = "create table shows (id int, name varchar(500))";

            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();

            sql = "create table cast (showid int , id int, name varchar(500), birthday varchar(500))";

            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();

            m_dbConnection.Close();
        }

        private void GetShows()
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                json_data = w.DownloadString("http://api.tvmaze.com/shows");

                var Shows = !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<List<ShowModel>>(json_data) : new List<ShowModel>();

                string PathAndFile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TvMazeDatabase.sqlite";

                SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + PathAndFile + ";Version=3;");
                m_dbConnection.Open();

                foreach(ShowModel show in Shows)
                {
                    string sql = "insert into shows (id, name) values (" + show.Id + ", '" + show.Name.Replace("'","''") + "')";

                    SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();
                }

                GetShowCast(Shows);

                m_dbConnection.Close();
            }
        }

        private void GetShowCast(List<ShowModel> shows)
        {
            foreach (ShowModel show in shows)
            {
                using (var w = new WebClient())
                {
                    var json_data = string.Empty;
                    try
                    {
                        json_data = w.DownloadString("http://api.tvmaze.com/shows/" + show.Id + "/cast");
                    }
                    catch (WebException)
                    {
                        Thread.Sleep(10000);
                        json_data = w.DownloadString("http://api.tvmaze.com/shows/" + show.Id + "/cast");
                    }
                    var ShowCast = !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<List<CastModel>>(json_data) : new List<CastModel>();

                    string PathAndFile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TvMazeDatabase.sqlite";

                    SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + PathAndFile + ";Version=3;");
                    m_dbConnection.Open();

                    foreach (CastModel castmember in ShowCast)
                    {
                        string sql = "insert into cast (showid, id, name, birthday) values (" + show.Id + ", " + castmember.Person.Id + ", '" + castmember.Person.Name.Replace("'", "''") + "', '" + castmember.Person.Birthday + "')";

                        SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                        command.ExecuteNonQuery();
                    }

                    m_dbConnection.Close();
                }
            }
        }
    }
}
