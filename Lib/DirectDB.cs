using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib
{
    public class DirectDB
    {
        static string cnnStr = Devmasters.Core.Util.Config.GetConfigValue("CnnString");

        public static void NoResult(string sql, params IDataParameter[] param)
        {
            NoResult(sql, CommandType.Text, param);
        }

        public static void NoResult(string sql, System.Data.CommandType type, params IDataParameter[] param)
        {
            using (var p = new Devmasters.Core.PersistLib())
            {
                try
                {
                    p.ExecuteNonQuery(cnnStr, type, sql, param);

                }
                catch (Exception e)
                {
                    HlidacStatu.Util.Consts.Logger.Error("SQL error:" + sql, e);
                    throw;
                }
            }
        }


        public static string GetRawSql(CommandType typ, string text, IDataParameter[] pars)
        {
            // vytahne spoj a inicializuje
            string _connStr = cnnStr;
            var conn = new SqlConnection(_connStr);

            // nastaveni prikazu
            IDbCommand _comm = new SqlCommand();
            _comm.CommandTimeout = 120;
            _comm.CommandText = text;
            _comm.CommandType = typ;
            _comm.Connection = conn;

            // jestlize zadany parametry, pak pripoj
            AttachParameters(_comm, pars);

            string query = _comm.CommandText;

            foreach (SqlParameter p in _comm.Parameters)
            {
                query = query.Replace("@" + p.ParameterName, ParameterValueForSQL(p));
            }
            return query;
        }
        /// <summary>
        /// pridej parametry do prikazu
        /// </summary>
        /// <param name="comm">prikaz</param>
        /// <param name="pars">sada parametru</param>
        private static void AttachParameters(IDbCommand comm, IDataParameter[] pars)
        {
            // ohlidani vstupu
            if (comm == null)
                throw new ArgumentNullException("command");
            if (pars == null)
                return;

            // ve smycce pridej vsechny parametry
            foreach (IDataParameter p in pars)
            {
                // osetreni db. nuly
                if (p.Value == null)
                    p.Value = DBNull.Value;

                comm.Parameters.Add(p);
            }
        }

        private static String ParameterValueForSQL(SqlParameter sp)
        {
            String retval = "";
            if (Devmasters.Core.PersistLib.IsNull(sp.Value))
                return "null";

            switch (sp.SqlDbType)
            {
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.Time:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                    retval = "N'" + sp.Value.ToString().Replace("'", "''") + "'";
                    break;

                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                    retval = "'" + ((DateTime)sp.Value).ToString("yyy-MM-dd hh:mm:ss").Replace("'", "''") + "'";
                    break;


                case SqlDbType.Bit:
                    retval = ((bool)sp.Value) ? "1" : "0";
                    break;

                case SqlDbType.DateTimeOffset:
                default:
                    retval = sp.Value.ToString().Replace("N'", "''");
                    break;
            }

            return retval;
        }
    }
}
