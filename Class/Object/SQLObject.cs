using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public class SQLobject : IDisposable
    {
        private MySqlConnection conn;
        private MySqlCommand cmd;
        private MySqlDataAdapter adapter;

        public SQLobject(string conningString)
        {
            conn = new MySqlConnection(conningString);
            cmd = new MySqlCommand() { Connection = conn };
            adapter = new MySqlDataAdapter(cmd);
        }

        /// <summary>
        /// 執行SQL指令碼
        /// </summary>
        /// <returns></returns>
        public bool Run(string commandText)
        {
            if (!OpenConn()) return false;

            cmd.CommandText = commandText;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {
                CloseConn();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 執行SQL指令碼
        /// </summary>
        /// <returns></returns>
        public bool Run(out DataTable result, string commandText)
        {
            result = null;
            if (!OpenConn()) return false;

            cmd.CommandText = commandText;
            try
            {
                DataTable fillData = new DataTable();
                adapter.Fill(fillData);
                result = fillData;
            }
            catch
            {
                CloseConn();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 開啟SQL連結
        /// </summary>
        /// <returns></returns>
        public bool OpenConn()
        {
            if (conn.State != System.Data.ConnectionState.Closed) return true;
            try
            {
                conn.Open();
            }
            catch
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 關閉SQL連結
        /// </summary>
        /// <returns></returns>
        public bool CloseConn()
        {
            if (conn.State == System.Data.ConnectionState.Closed) return true;
            try
            {
                conn.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void Dispose()
        {

        }
    }
}
