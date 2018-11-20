using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Net;
using System.Web.Http;
using Newtonsoft.Json;
using TVMazeScapper.Models;

namespace TVMazeScapper.Controllers
{
    public class TvMazeController : ApiController
    {
        public object GetInfo(int page)
        {
            var ShowsAndCast = new List<ComboModel>();
            ShowsAndCast = GetShowsInfo(page);

            return JsonConvert.SerializeObject(ShowsAndCast);
        }

        private List<ComboModel> GetShowsInfo(int page)
        {
            var ShowsAndCast = new List<ComboModel>();

            string PathAndFile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TvMazeDatabase.sqlite";

            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + PathAndFile + ";Version=3;");
            m_dbConnection.Open();

            int OffsetCount = (page - 1) * 10;
            string sql = "select * from shows Limit 10 offset " + OffsetCount + "";

            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var showItem = new ComboModel();

                showItem.Id = Convert.ToInt32(reader["id"]);
                showItem.Name = reader["name"].ToString();

                showItem.Cast = new List<CastDetail>();
                showItem.Cast = GetShowCast(showItem.Id);

                ShowsAndCast.Add(showItem);
            }

            m_dbConnection.Close();

            return ShowsAndCast;
        }

        private List<CastDetail> GetShowCast(int ShowId)
        {
            var ShowCast = new List<CastDetail>();

            string PathAndFile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TvMazeDatabase.sqlite";

            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + PathAndFile + ";Version=3;");
            m_dbConnection.Open();

            string sql = "select * from cast where showid=" + ShowId + " ORDER BY birthday DESC";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                var CastItem = new CastDetail();

                CastItem.Id = Convert.ToInt32(reader["id"]);
                CastItem.Name = reader["name"].ToString();
                CastItem.Birthday = reader["birthday"].ToString();

                ShowCast.Add(CastItem);
            }

            m_dbConnection.Close();

            return ShowCast;
        }
    }
}
