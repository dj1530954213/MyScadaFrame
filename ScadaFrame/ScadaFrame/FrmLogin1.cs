using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CCWin;
using Sunny.UI;

namespace ScadaFrame
{
    public struct UserInformation
    {
        internal string userName;
        internal int authorityLevel;
    }
    public partial class FrmLogin1 : CCWin.CCSkinMain
    {
        #region 窗体边框阴影效果变量申明
        const int CS_DropSHADOW = 0x20000;
        const int GCL_STYLE = (-26);
        //声明Win32 API
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetClassLong(IntPtr hwnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassLong(IntPtr hwnd, int nIndex);
        #endregion
        public FrmLogin1()
        {
            InitializeComponent();
            //SetClassLong(this.Handle, GCL_STYLE, GetClassLong(this.Handle, GCL_STYLE) | CS_DropSHADOW); //API函数加载，实现窗体边框阴影效果
        }
        private sqlhandle userSqlHandle;
        internal UserInformation userInformation = new UserInformation();//保存用户名和权限
        //userInformation
        
        private void btn_Close_Click(object sender, EventArgs e)
        {

            DialogResult = DialogResult.Cancel;//证明账户名密码正确，通过这个来传递给主窗体使其显示
            this.Close();
        }

        private void btn_Login_Click(object sender, EventArgs e)
        {
            userSqlHandle = new sqlhandle("DESKTOP-46E3VFR", "SCADA", "SA", "1");
            userSqlHandle.connection();
            string sql = $"select count(*) from UserManagement where userName = '{uiTextBoxUserName.Text}' and passWard = '{uiTextBoxPassWord.Text}'";
            IDataReader idr = userSqlHandle.read(sql);
            int count;
            if (idr.Read())
            {
                count = (Int32)idr[0];//判断账号是否存在
            }
            else
            {
                count = 0;
            }
            idr.Close();
            idr.Dispose();
            if (count == 1)
            {
                sql = $"select userName,authorityLevel from UserManagement where userName = '{uiTextBoxUserName.Text}' and passWard = '{uiTextBoxPassWord.Text}'";
                idr = userSqlHandle.read(sql);
                if (idr.Read())
                {
                    userInformation.userName = idr[0].ToString();
                    userInformation.authorityLevel = (Int32)idr[1];
                }
                idr.Close();
                idr.Dispose();
                DialogResult = DialogResult.OK;//证明账户名密码正确，通过这个来传递给主窗体使其显示
            }
            else
            {
                DialogResult = DialogResult.No;
                Sunny.UI.UIPage uIPage = new UIPage();
                uIPage.BackColor = Color.Orchid;
                uIPage.ShowErrorDialog("用户名或密码错误");
            }
            userSqlHandle.disconnection();
        }

        private void FrmLogin1_Shown(object sender, EventArgs e)
        {
            uiTextBoxUserName.Focus();//获得焦点
        }

        private void FrmLogin1_Load(object sender, EventArgs e)
        {

        }
    }
}
