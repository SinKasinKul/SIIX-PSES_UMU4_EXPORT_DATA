using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSES_UMU4_EXPORT_DATA
{
    class ConnectDB
    {
        public string Error;
        public SqlConnection ConnectionDB()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string Server = appSettings.Get("Server");
                string DataBase = appSettings.Get("DBNane");
                string User = appSettings.Get("User");
                string PW = appSettings.Get("PW");
                string PoolSize = appSettings.Get("PoolSize");
                string Timeout = appSettings.Get("Timeout");
                string connetionString;
                SqlConnection cnn;
                connetionString = @"Data Source=" + Server + ";"
                                + "Initial Catalog=" + DataBase + ";"
                                + "User ID=" + User + ";"
                                + "Password=" + PW + ";"
                                + "Max Pool Size=" + PoolSize + ";"
                                + "Connect Timeout=" + Timeout + ";";
                cnn = new SqlConnection(connetionString);
                return cnn;
            }
            catch (SqlException e)
            {
                SqlConnection cnn;
                cnn = new SqlConnection();
                Error = (e.ToString());
                return cnn;
            }
        }
        public DataTable STBL_GET_ALL_DATA_EXPORT(string SERIAL)
        {
            SqlConnection cnn = ConnectionDB();
            SqlCommand objCmd = new SqlCommand();
            SqlDataAdapter dtAdapter = new SqlDataAdapter();

            string strStored;

            using (cnn)
            {
                strStored = "STBL_GET_ALL_DATA_EXPORT";
                objCmd.Parameters.Add(new SqlParameter("@pBarcode", SERIAL));

                objCmd.Connection = cnn;
                objCmd.CommandText = strStored;
                objCmd.CommandType = CommandType.StoredProcedure;

                dtAdapter.SelectCommand = objCmd;

                DataTable dtRecord = new DataTable();
                dtAdapter.Fill(dtRecord);
                return dtRecord;
            }
        }
    }
}
