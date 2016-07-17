using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using DAL;
namespace BLL
{
    class PlayVideo
    {
      public  int VideoID;
        public DataTable Login(PlayVideo pv)
        {
            DataAccessLayer da = new DataAccessLayer();
            DataTable dt = new DataTable();
            SqlParameter[] parameter = { new SqlParameter("@VideoID",pv.VideoID) };
            dt = da.ExecuteQuery("SPViewVideo", CommandType.StoredProcedure, parameter);

            return dt;
        }
    }
}
