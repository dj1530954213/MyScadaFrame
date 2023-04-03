using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslCommunication;
using HslCommunication.LogNet;
using HslCommunication.Profinet.AllenBradley;

namespace ABCommunicationDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public AllenBradleyNet allenBradleyNet = new AllenBradleyNet("");
        private void BT_Connect_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBoxPortName.Text, out int port))//端口
            {
                MessageBox.Show("端口输入错误");
                return;
            }
            if (!byte.TryParse(textBoxSlotNum.Text, out byte slot))//槽号
            {
                MessageBox.Show("端口输入错误");
                return;
            }
            //参数初始化
            allenBradleyNet.IpAddress = textBoxIpAddress.Text;
            allenBradleyNet.Port = 44818;
            allenBradleyNet.Slot = slot;
            if (!string.IsNullOrEmpty(textBoxRoute.Text))
            {
                allenBradleyNet.MessageRouter = new MessageRouter(textBoxRoute.Text);
            }
            try
            {
                OperateResult operateResult = allenBradleyNet.ConnectServer();
                if (operateResult.IsSuccess)
                {
                    MessageBox.Show(HslCommunication.StringResources.Language.ConnectedSuccess);
                }
                else
                {
                    MessageBox.Show(HslCommunication.StringResources.Language.ConnectedFailed + operateResult.ToMessageShowString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void BT_Disconnect_Click(object sender, EventArgs e)
        {
            allenBradleyNet.ConnectClose();
        }
    }
}
