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

        /// <summary>
        /// 儲存最後一次執行的失敗訊息
        /// </summary>
        public string ErrorMessage { get; private set; }

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
                ErrorMessage = "";
            }
            catch (Exception ex)
            {
                CloseConn();
                ErrorMessage = ex.Message;
                return false;
            }
            finally
            {
                cmd.Parameters.Clear();
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
                ErrorMessage = "";
            }
            catch (Exception ex)
            {
                CloseConn();
                ErrorMessage = ex.Message;
                return false;
            }
            finally
            {
                cmd.Parameters.Clear();
            }
            return true;
        }

        /// <summary>
        /// 新增SQL參數
        /// </summary>
        /// <param name="parameterName">參數名稱</param>
        /// <param name="value">參數值</param>
        public void AddParameter(string parameterName, object value)
        {
            cmd.Parameters.AddWithValue(parameterName, value);
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
            catch (Exception ex)
            {
                cmd.Parameters.Clear();
                ErrorMessage = ex.Message;
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
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
            return true;
        }

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    conn.Close();
                    adapter.Dispose();
                    cmd.Dispose();
                    conn.Dispose();
                }
                disposedValue = true;
            }
        }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
