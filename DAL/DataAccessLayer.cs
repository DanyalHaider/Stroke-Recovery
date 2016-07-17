using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace DAL
{
   public class DataAccessLayer
    {

        SqlConnection conn;
        SqlCommand cmd;
        SqlDataAdapter adp;
        string ConString;
        public DataAccessLayer()
        {
            // ConString = System.Configuration.ConfigurationManager.AppSettings["ConString"].ToString();

            ConString = "Data Source=danyalhaider-pc\\sqlexpress;Initial Catalog=StrokeRecovery;Integrated Security=True";
        }


        private void CreateCommand(string query, CommandType cmdType, SqlParameter[] param = null)
        {
            conn = new SqlConnection(ConString);
            cmd = new SqlCommand(query, conn);
            adp = new SqlDataAdapter(cmd);
            if (param != null)
                cmd.Parameters.AddRange(param);

            if (cmdType == CommandType.StoredProcedure)
                cmd.CommandType = CommandType.StoredProcedure;
            else
                cmd.CommandType = CommandType.Text;
        }
        public DataTable ExecuteQuery(string query, CommandType cmdType, SqlParameter[] param = null)
        {

            try
            {
                DataTable dt = new DataTable();
                CreateCommand(query, cmdType, param);
                conn.Open();
                adp.Fill(dt);
                return dt;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to connect with Database");
            }

            finally
            {
                conn.Close();
            }

        }

        public DataRow ExecuteDataRow(string query, CommandType cmdType, SqlParameter[] param = null)
        {

            try
            {
                DataTable dt = new DataTable();
                CreateCommand(query, cmdType, param);
                conn.Open();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                    return dt.Rows[0];
                else
                    return null;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to connect with Database");
            }

        }

        public int ExecuteNonQuery(string query, CommandType cmdType, SqlParameter[] parameter = null)
        {
            try
            {
                CreateCommand(query, cmdType, parameter);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception("Unable to connect with Database" + e.Message);
            }

        }

        public DataTable ExecuteDataTable(string p, CommandType commandType, SqlParameter[] param)
        {

            throw new NotImplementedException();
        }
    }
}
