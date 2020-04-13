using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OracleClient;
using System.Collections;
using System.Configuration;

namespace DataOperate
{
    public abstract class OracleHelper
    {
        public static readonly string ConnectionStringLocalTransaction = ConfigurationManager.AppSettings["DBConn"].ToString();
        //public static readonly string ConnectionStringLocalTransaction = System.Configuration.ConfigurationSettings.AppSettings["DBConn"].ToString();
        //public static readonly string WareHouseID = System.Configuration.ConfigurationSettings.AppSettings["WareHouseID"].ToString();

        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        public static int ExecuteNonQuery(CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            //OracleCommand cmd = new OracleCommand();
            using (OracleConnection connection = new OracleConnection(ConnectionStringLocalTransaction))
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static OracleDataReader ExecuteReader(CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = new OracleConnection(ConnectionStringLocalTransaction);
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                OracleDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                return null;
            }
        }

        public static DataSet ExecuteDataSet(CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            //OracleCommand cmd = new OracleCommand();
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionStringLocalTransaction))
                {
                    OracleCommand cmd = new OracleCommand();
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                    OracleDataAdapter oda = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    oda.Fill(ds);
                    cmd.Parameters.Clear();
                    return ds;
                }
            }
            catch(Exception)
            {
                return null;
            }
        }

        public static string[] ExecuteProcedure(CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = new OracleConnection(ConnectionStringLocalTransaction);
            string[] returnv = new string[commandParameters.Length];
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                cmd.ExecuteNonQuery();
                for (int i = 0; i < returnv.Length; i++)
                {
                    returnv[i] = cmd.Parameters[i].Value.ToString();
                }
                return returnv;
            }
            catch
            {
                conn.Close();
                return returnv;
            }
        }

        public static object ExecuteScalar(CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();

            using (OracleConnection conn = new OracleConnection(ConnectionStringLocalTransaction))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static void CacheParameters(string cacheKey, params OracleParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        public static OracleParameter[] GetCachedParameters(string cacheKey)
        {
            OracleParameter[] cachedParms = (OracleParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;
            OracleParameter[] clonedParms = new OracleParameter[cachedParms.Length];
            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (OracleParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }

        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, CommandType cmdType, string cmdText, OracleParameter[] commandParameters)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;

            if (trans != null)
                cmd.Transaction = trans;
            if (commandParameters != null)
            {
                foreach (OracleParameter parm in commandParameters)
                    cmd.Parameters.Add(parm);
            }
        }
    }
}
