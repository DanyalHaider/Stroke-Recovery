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
    class ViewImages
    {
        public int ImageID;
        public DataTable Login(ViewImages vi)
        {
            DataAccessLayer da = new DataAccessLayer();
            DataTable dt = new DataTable();
            SqlParameter[] parameter = { new SqlParameter("@ImageID", vi.ImageID) };
            dt = da.ExecuteQuery("SPViewImages", CommandType.StoredProcedure, parameter);

            return dt;
        }
    }
}
