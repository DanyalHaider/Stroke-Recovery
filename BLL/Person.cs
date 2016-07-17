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

    public class Person
    {
        public int PID;
        public string PName;
        public string UName;
        public string PPassword;
        public string PGender;
        public string PAge;
//****************************************************************************************
// Person Sigup
        public int CreateAccount(Person per)
        {
            DataAccessLayer da = new DataAccessLayer();
            SqlParameter[] parameter = { new SqlParameter("@Per_Name", per.PName), new SqlParameter("@User_Name", per.UName), new SqlParameter("@Per_Password", per.PPassword), new SqlParameter("@Per_Gender", per.PGender), new SqlParameter("@Per_Age", per.PAge) };
            int validity = da.ExecuteNonQuery("SPRegisterUser", CommandType.StoredProcedure, parameter);
            return validity;
        }
//****************************************************************************************
// Person Login
        public DataTable Login(Person per)
        {
            DataAccessLayer da = new DataAccessLayer();
            DataTable dt = new DataTable();
            SqlParameter[] parameter = { new SqlParameter("@User_Name", per.UName), new SqlParameter("@Per_Password", per.PPassword) };
            dt = da.ExecuteQuery("SPPatientLogin", CommandType.StoredProcedure, parameter);

            return dt;
        }
        /* public DataTable ViewReport(Person per)
         {
             DataTable dt = new DataTable();
             DataAccessLayer da = new DataAccessLayer();
             SqlParameter[] parameter = { new SqlParameter("@StampTime", ), new SqlParameter("@StampTime", ) };
             dt = da.ExecuteQuery("SPViewViaStampDate", CommandType.StoredProcedure, parameter);
             return dt;
         }*/
 //****************************************************************************************
// Person Update profile
        public int UpdateProfile(Person per)
        {
            SqlParameter[] parameter = { new SqlParameter("@Person_Name", per.PName), new SqlParameter("@LastName", per.UName), new SqlParameter("@Up_Adress", per.PPassword) };
            DataAccessLayer da = new DataAccessLayer();
            return da.ExecuteNonQuery("SpPatientUpdateProfile", CommandType.StoredProcedure, parameter);
        }
//****************************************************************************************
// Person Sigup
        public void DeleteAcc(Person per)
        {

            SqlParameter[] parameter = { new SqlParameter("@PersonID", per.PID) };
            DataAccessLayer DA = new DataAccessLayer();
            int validity = DA.ExecuteNonQuery("SP_DELETE_PERSON_ACC", CommandType.StoredProcedure, parameter);
            if (validity == 1)
            {
                throw new Exception("Failed to delete a Record");
            }
        }
    }
}
