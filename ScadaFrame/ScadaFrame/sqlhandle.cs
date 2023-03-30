using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ScadaFrame
{
    public class sqlhandle:IDisposable
    {
        /// <summary>
        /// 创建数据库连接对象
        /// </summary>
        /// <param name="server">DataSource 服务器名称</param>
        /// <param name="database">initial Catalog 表名称</param>
        /// <param name="uid">用户名称</param>
        /// <param name="pwd">密码</param>
        public sqlhandle(string server,string database,string uid,string pwd)
        {
            this.Server = server;
            this.DataBase = database;
            this.Uid = uid;
            this.Pwd = pwd;
            this.ConString = $"server={this.Server};database={this.DataBase};uid={this.Uid};pwd={this.Pwd}";
        }
        private string _server;
        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }
        private string _dataBase;
        public string DataBase
        {
            get { return _dataBase; }
            set { _dataBase = value; }
        }
        private string _uid;
        public string Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }
        private string _pwd;
        public string Pwd
        {
            get { return _pwd; }
            set { _pwd = value; }
        }
        private string _conString;
        public string ConString
        {
            get { return _conString; }
            set { _conString = value; }
        }
        private SqlConnection sc = null;
        private SqlCommand cmd = null;
        private SqlDataReader sdr = null;
        
        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <returns>返回数据库连接状态  true已连接   false断开连接</returns>
        public bool connection()
        {
            sc = new SqlConnection(_conString);
            if (sc.State ==  System.Data.ConnectionState.Closed)
            {
                try
                {
                    sc.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    return false;
                }
                return true; 
            }
            return true;
        }
        /// <summary>
        /// 断开数据库连接
        /// </summary>
        /// <returns>返回数据库连接状态  true已连接   false断开连接</returns>
        public bool disconnection()
        {
            if (sc.State == System.Data.ConnectionState.Open)
            {
                sc.Close();
            }
            return false;
        }

        public int excute(string sql)
        {
            cmd = new SqlCommand(sql,sc);
            return cmd.ExecuteNonQuery();
        }

        public SqlDataReader read(string sql)
        {
            cmd = new SqlCommand(sql, sc);
            sdr = cmd.ExecuteReader();
            return sdr;
        }

        public void Dispose()
        {
            if (sdr != null)
            {
                sdr.Close();
            }
            sc.Dispose();
            cmd.Dispose();
        }
    }
}
