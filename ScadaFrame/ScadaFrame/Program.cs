using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScadaFrame
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DialogResult dialogResult = DialogResult.No;
            //while (dialogResult != DialogResult.OK)//如果不正确继续输入
            //{
            //    FrmLogin1 frmLogin1 = new FrmLogin1();
            //    dialogResult = frmLogin1.ShowDialog();
            //    if (dialogResult == DialogResult.OK)
            //    {
            //        Application.Run(new Form1(frmLogin1.userInformation));
            //    }
            //    else if (dialogResult == DialogResult.Cancel)//如果按下退出键就直接跳出
            //    {
            //        break;
            //    }
            //}

            Application.Run(new Form1(new UserInformation()));


        }
    }
}
