using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScadaFrame
{

    public partial class Form1 : Form
    {
        internal UserInformation loginInformation;
        public Form1(UserInformation userInformation)
        {
            InitializeComponent();
            loginInformation = userInformation;
        }
        #region 配合TabControl进行页面切换

        private void bt_Page_Connection_Click(object sender, EventArgs e)
        {
            uiTabControlMenu.SelectedIndex = 1;//通讯设置
        }

        private void bt_Page_IO_Click(object sender, EventArgs e)
        {
            uiTabControlMenu.SelectedIndex = 2;//IO采集
        }

        private void bt_Page_CurveNow_Click(object sender, EventArgs e)
        {
            uiTabControlMenu.SelectedIndex = 3;//实时曲线
        }

        private void bt_Page_CurveHistroy_Click(object sender, EventArgs e)
        {
            uiTabControlMenu.SelectedIndex = 4;//历史曲线
        }

        private void bt_PageAlarm_Click(object sender, EventArgs e)
        {
            uiTabControlMenu.SelectedIndex = 5;//报警查询
        }

        private void bt_Page_Log_Click(object sender, EventArgs e)
        {
            uiTabControlMenu.SelectedIndex = 6;//日志管理
        }

        private void bt_Page_DB_Click(object sender, EventArgs e)
        {
            uiTabControlMenu.SelectedIndex = 7;//数据库管理(点表)
        }

        private void bt_Page_Permission_Click(object sender, EventArgs e)
        {
            uiTabControlMenu.SelectedIndex = 8;//用户权限管理
        }

        private void bt_Page_Report_Click(object sender, EventArgs e)
        {
            uiTabControlMenu.SelectedIndex = 9;//报表
        }

        private void bt_Page_Flow_Click(object sender, EventArgs e)
        {
            CMS_FlowSelect.Size = new Size(bt_Page_Flow.Width + 20, bt_Page_Flow.Height + 20);
            bt_Page_Flow.ShowContextMenuStrip(CMS_FlowSelect, 7, bt_Page_Flow.Height);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            uiTabControlMenu.SelectedIndex = 10;//控制画面1
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            uiTabControlMenu.SelectedIndex = 11;//控制画面2
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            uiTabControlMenu.SelectedIndex = 12;//控制画面3
        }
        #endregion
        public Sunny.UI.UIPage messageBox = new UIPage();
        public StringBuilder rowsCopy = new StringBuilder();//用于存放复制单元格的值
        string PointConfigPath = "";
        string DeviceConfigPath = "";
        public Control[] deviceConfigContorl;
        public int deviceCount = 0;
        public bool deviceConnectState = false;
        public System.Timers.Timer monitorDeviceTimer;
        public Dictionary<string, CommunicateDevice> deviceDictionary = new Dictionary<string, CommunicateDevice>();
        public Dictionary<string[], List<object>> devicePareDictionary = new Dictionary<string[], List<object>>();
        public Dictionary<string, int> deviceNum = new Dictionary<string, int>();
        public sqlhandle sqlhandle;
        public List<AlarmRecode> alarmRecodes = new List<AlarmRecode>();
        //public SiemensPLC siemensPLC = new SiemensPLC();
        private void Form1_Load(object sender, EventArgs e)
        {
            sqlhandle = new sqlhandle("DESKTOP-46E3VFR", "SCADA", "SA", "1");
            sqlhandle.connection();
            //AlarmServiceStatue = AlarmServiceStart();
            uiTabControlMenu.SelectedIndex = 0;//流程图
            #region 初始化时设置部分控件属性
            //设置按钮颜色
            foreach (Control item in tableLayoutPanel8.Controls)
            {
                ((UISymbolButton)item).FillColor = Color.DarkSlateGray;
                ((UISymbolButton)item).FillColor2 = Color.Silver;
                ((UISymbolButton)item).FillHoverColor = Color.LightSeaGreen;
            }
            foreach (Control item in tableLayoutPanel7.Controls)
            {
                ((UISymbolButton)item).FillColor = Color.DarkSlateGray;
                ((UISymbolButton)item).FillColor2 = Color.Silver;
                ((UISymbolButton)item).FillHoverColor = Color.LightSeaGreen;
            }
            uiComboBox_AlarmType.Items.AddRange(new string[] { "所有报警", "状态量报警", "低报", "低低报", "高报", "高高报" });
            uiComboBox_AlarmType.SelectedIndex = 0;
            DTP_AlarmStartTime.Text = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day-1} 00:00:00";
            DTP_AlarmStopTime.Text = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day} 00:00:00";
            #endregion
            #region 加载预处理
            //动态设置连接单元下拉框选项
            string[] deviceDrive = { "Siemens", "AB", "ModbusRtu", "ModbusTCP", "OPCClient", "OPCServer" };
            ((DataGridViewComboBoxColumn)uiDataGridViewDevice.Columns[1]).Items.AddRange(deviceDrive);
            #endregion
            //siemensPLC.errorMessageEvent += new Action<string>(ShowMessage);
            #region 通讯设备初始化处理
            //加载设备表
            LoadDeviceCSV(Directory.GetCurrentDirectory() + "\\Device.csv");
            //从设备表中读取通讯参数放入Dictionary中
            InitDeviceCSV();
            //开始轮询所有勾选自动连接的设备并按照每5秒一次的扫描间隔来监控所有设备的状态
            PollingDevice(devicePareDictionary);
            //加载点表内通讯设备下拉框
            string[] connectionUnit = new string[uiDataGridViewDevice.Rows.Count - 1];
            for (int i = 0; i < uiDataGridViewDevice.Rows.Count - 1; i++)
            {
                connectionUnit[i] = uiDataGridViewDevice.Rows[i].Cells[0].Value.ToString();
            }
            ((DataGridViewComboBoxColumn)uiDataGridViewDB.Columns[1]).Items.AddRange(connectionUnit);
            #endregion
            #region 点表初始化处理
            //加载点表
            InitPointCSV(Directory.GetCurrentDirectory() + "\\Variable.csv");
            //将点位信息加载入设备对象的点位信息类字典中
            PointCreat();
            //轮询扫描读取的点位
            PointScan();
            #endregion
            #region 报警处理
            AlarmRecode();
            //先执行一次此函数以初始化实时报警表
            ActualAlarmQuery();
            #endregion
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //关闭数据库连接
            sqlhandle.disconnection();
            sqlhandle.Dispose();
        }

        #region 点表内按钮操作
        private void bt_PointConfig_Input_Click(object sender, EventArgs e)
        {
            uiDataGridViewDB.ClearRows();
            try
            {
                #region 读取文件的另一种实现方式
                /*  使用OLEDB接口导入文件*/
                //using (OpenFileDialog openExcel = new OpenFileDialog())
                //{
                //    openExcel.InitialDirectory = Directory.GetCurrentDirectory();//设置初始化路径
                //    openExcel.Filter = "表格|*.xls";//设置格式
                //    string excelPath = "";
                //    if (openExcel.ShowDialog() == DialogResult.OK)
                //    {
                //        excelPath = openExcel.FileName;
                //        string connectString = "provider=microsoft.jet.oledb.4.0;data source=" + excelPath + ";extended properties=excel 8.0";//连接字符串
                //        OleDbConnection excelOleDb = new OleDbConnection(connectString);
                //        string commandString = "select * from [Variable$A:K]";
                //        OleDbCommand oleDbCommand = new OleDbCommand(commandString, excelOleDb);
                //        OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);
                //        DataTable dataTable = new DataTable();
                //        oleDbDataAdapter.Fill(dataTable);
                //        uiDataGridViewDB.DataSource = dataTable;//数据源设置完毕
                //        //设置页面选择控件
                //        uiPaginationDB.TotalCount = uiDataGridViewDB.RowCount;
                //        uiPaginationDB.PageSize = 50;
                //        uiDataGridViewFooterDB.DataGridView = uiDataGridViewDB;
                //        excelOleDb.Dispose();
                //    }
                //}
                #endregion
                //实例化一个选择问价窗口
                OpenFileDialog openCsv = new OpenFileDialog();
                //设置默认路径为当前路径
                openCsv.InitialDirectory = Directory.GetCurrentDirectory();
                //过滤显示文件的类型
                openCsv.Filter = "CSV文件|*.csv";
                if (openCsv.ShowDialog() == DialogResult.OK)
                {
                    //保存当前文件的地址
                    PointConfigPath = openCsv.FileName;
                    //使用GC托管的环境来实现文件流读取CSV
                    using (StreamReader CSVreader = new StreamReader(openCsv.FileName, Encoding.Default))
                    {
                        CSVreader.ReadLine();
                        bool alarmEnable = false;
                        while (true)
                        {
                            string IOstring = CSVreader.ReadLine();//读取CSV文件中的第一行
                            if (IOstring == null)//如果最后一行
                            {
                                break;
                            }
                            string[] cellString = IOstring.Split(new char[] { ',' });//以逗号将数据流分割
                            //数据导入单元格
                            uiDataGridViewDB.AddRow();

                            for (int i = 0; i < 14; i++)
                            {
                                if (i >= 6 && i <= 8)//将字符串转换为bool量需要单独处理
                                {
                                    alarmEnable = bool.Parse(cellString[i].ToString());
                                    uiDataGridViewDB.Rows[uiDataGridViewDB.RowCount - 2].Cells[i].Value = alarmEnable;
                                }
                                else
                                {
                                    uiDataGridViewDB.Rows[uiDataGridViewDB.RowCount - 2].Cells[i].Value = cellString[i].ToString();
                                }
                            }
                        }
                        CSVreader.Close();
                    }
                    openCsv.Dispose();
                }
            }
            catch (Exception ex)
            {
                messageBox.ShowErrorDialog(ex.Message.ToString());
            }
        }

        private void bt_PointConfig_Backup_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog CSVsave = new SaveFileDialog();
                CSVsave.InitialDirectory = Directory.GetCurrentDirectory();
                CSVsave.Filter = "CSV文件|*.csv";
                if (CSVsave.ShowDialog() == DialogResult.OK)
                {
                    SavePointCSV(CSVsave.FileName);
                    messageBox.ShowSuccessDialog("备份成功");
                }
                CSVsave.Dispose();
            }
            catch (Exception ex)
            {
                messageBox.ShowErrorDialog(ex.Message.ToString());
            }
        }
        /// <summary>
        /// 将通过数据流的方式保存点表
        /// </summary>
        /// <param name="path">CSV文件路径</param>
        public void SavePointCSV(string path)
        {
            StreamWriter streamWriter = new StreamWriter(path, false, Encoding.Default);
            StringBuilder WriteLine = new StringBuilder();
            for (int i = 0; i < uiDataGridViewDB.Rows[0].Cells.Count; i++)
            {
                WriteLine.Append($"{uiDataGridViewDB.Columns[i].HeaderText.ToString()},");
            }
            streamWriter.WriteLine(WriteLine.ToString());
            for (int i = 0; i < uiDataGridViewDB.RowCount - 1; i++)
            {
                WriteLine.Clear();//清除上一行的字符串
                for (int j = 0; j < uiDataGridViewDB.Rows[i].Cells.Count; j++)
                {
                    //将combobox的值转换为string需要单独处理
                    if (j == 1 || j == 3)
                    {
                        WriteLine.Append($"{((DataGridViewComboBoxCell)uiDataGridViewDB.Rows[i].Cells[j]).Value.ToString()},");
                    }
                    //将checkBox的值转换为string需要单独处理
                    else if (j >= 6 && j <= 8)
                    {
                        WriteLine.Append($"{uiDataGridViewDB.Rows[i].Cells[j].EditedFormattedValue.ToString()},");
                    }
                    else
                    {
                        WriteLine.Append($"{uiDataGridViewDB.Rows[i].Cells[j].Value.ToString()},");
                    }
                }
                streamWriter.WriteLine(WriteLine.ToString());
            }
            streamWriter.Flush();
            streamWriter.Close();
        }
        public void InitPointCSV(string path)
        {
            using (StreamReader CSVreader = new StreamReader(path, Encoding.Default))
            {
                CSVreader.ReadLine();
                bool alarmEnable = false;
                while (true)
                {
                    string IOstring = CSVreader.ReadLine();//读取CSV文件中的第一行
                    if (IOstring == null)//如果最后一行
                    {
                        break;
                    }
                    string[] cellString = IOstring.Split(new char[] { ',' });//以逗号将数据流分割
                                                                             //数据导入单元格
                    uiDataGridViewDB.AddRow();

                    for (int i = 0; i < 14; i++)
                    {
                        if (i >= 6 && i <= 8)//将字符串转换为bool量需要单独处理
                        {
                            alarmEnable = bool.Parse(cellString[i].ToString());
                            uiDataGridViewDB.Rows[uiDataGridViewDB.RowCount - 2].Cells[i].Value = alarmEnable;
                        }
                        else
                        {
                            uiDataGridViewDB.Rows[uiDataGridViewDB.RowCount - 2].Cells[i].Value = cellString[i].ToString();
                        }
                    }
                }
                CSVreader.Close();
            }
        }
        private void bt_PointConfig_Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (PointConfigPath == "")
                {
                    SavePointCSV(Directory.GetCurrentDirectory() + "\\Variable.csv");
                }
                else
                {
                    SavePointCSV(PointConfigPath);
                }
                messageBox.ShowSuccessDialog("保存成功");
            }
            catch (Exception ex)
            {
                messageBox.ShowErrorDialog(ex.Message.ToString());
            }
        }
        #endregion

        #region 点表右键菜单处理

        private void toolStripMenuItemCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(uiDataGridViewDB.GetClipboardContent());//复制至粘贴板
        }

        private void toolStripMenuItemPaste_Click(object sender, EventArgs e)
        {
            try
            {
                string pasteText = Clipboard.GetText();//获取粘贴板中的字符串
                string[] test = pasteText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);//获得每行的字符串放入数组内
                bool alarmEnable = false;
                int selectIndex = uiDataGridViewDB.SelectedIndex;//注意这里的selectedIndex会随着行数的增加而增加，我们需要保留当前值来使用
                for (int i = 0; i < test.Count(); i++)
                {
                    string[] vals = test[i].Split('\t');
                    uiDataGridViewDB.Rows.Add();
                    for (int j = 0; j < vals.Count(); j++)
                    {
                        if (i >= 6 && i <= 8)//将字符串转换为bool量需要单独处理
                        {
                            alarmEnable = bool.Parse(vals[j].ToString());
                            uiDataGridViewDB.Rows[selectIndex].Cells[j].Value = alarmEnable;//以当前选中行为基础增加
                        }
                        else
                        {
                            uiDataGridViewDB.Rows[selectIndex].Cells[j].Value = vals[j].ToString();
                        }
                    }
                    selectIndex++;
                }


            }
            catch (Exception ex)
            {
                messageBox.ShowErrorDialog(ex.Message.ToString());
            }
        }

        private void toolStripMenuItemDelet_Click(object sender, EventArgs e)
        {
            uiDataGridViewDB.Rows.RemoveAt(uiDataGridViewDB.CurrentRow.Index);
        }

        private void toolStripMenuItemEnableAlarm_Click(object sender, EventArgs e)
        {
            string pointName = uiDataGridViewDB.Rows[uiDataGridViewDB.CurrentRow.Index].Cells[0].Value.ToString();
            foreach (var item in deviceDictionary)
            {
                if (item.Value.PointDictionary.ContainsKey(pointName) && item.Value.PointDictionary[pointName].scan)//当点位在实时点表字典中且点位处于扫描中
                {
                    item.Value.PointDictionary[pointName].alarmEnable = true;//首先将运行中的点位报警功能置为true
                    uiDataGridViewDB.Rows[uiDataGridViewDB.CurrentRow.Index].Cells[7].Value = true;//将外部点表更新
                    SavePointCSV(Directory.GetCurrentDirectory() + "\\Variable.csv");
                }
            }
        }

        #endregion

        #region 异常显示功能(测试)
        /// <summary>
        /// 错误信息弹出，需要添加至委托执行
        /// </summary>
        /// <param name="str">invlke传递的值</param>
        public void ShowMessage(string str)
        {
            messageBox.ShowErrorDialog(str);
        }
        private void bt_PVvalue_Click(object sender, EventArgs e)
        {
            //siemensPLC.ShowMessage();
        }
        #endregion

        #region 设备表内操作按钮
        /* 文件操作区 */
        private void bt_DeviceConfig_Input_Click(object sender, EventArgs e)
        {
            uiDataGridViewDevice.ClearRows();
            try
            {
                //实例化一个选择问价窗口
                OpenFileDialog openCsv = new OpenFileDialog();
                //设置默认路径为当前路径
                openCsv.InitialDirectory = Directory.GetCurrentDirectory();
                //过滤显示文件的类型
                openCsv.Filter = "CSV文件|*.csv";
                if (openCsv.ShowDialog() == DialogResult.OK)
                {
                    //保存当前文件的地址
                    DeviceConfigPath = openCsv.FileName;
                    //使用GC托管的环境来实现文件流读取CSV
                    LoadDeviceCSV(openCsv.FileName);
                    openCsv.Dispose();
                }
            }
            catch (Exception ex)
            {
                messageBox.ShowErrorDialog(ex.Message.ToString());
            }
        }

        public void LoadDeviceCSV(string path)
        {
            //加载设备表
            using (StreamReader CSVreader = new StreamReader(path, Encoding.Default))
            {
                CSVreader.ReadLine();
                bool Tag = false;
                while (true)
                {
                    string IOstring = CSVreader.ReadLine();//读取CSV文件中的第一行
                    if (IOstring == null)//如果最后一行
                    {
                        break;
                    }
                    string[] cellString = IOstring.Split(new char[] { ',' });//以逗号将数据流分割
                                                                             //数据导入单元格
                    uiDataGridViewDevice.AddRow();

                    for (int i = 0; i < 8; i++)
                    {
                        if (i >= 4 && i <= 5)//将字符串转换为bool量需要单独处理
                        {
                            Tag = bool.Parse(cellString[i].ToString());
                            uiDataGridViewDevice.Rows[uiDataGridViewDevice.RowCount - 2].Cells[i].Value = Tag;
                        }
                        else
                        {
                            uiDataGridViewDevice.Rows[uiDataGridViewDevice.RowCount - 2].Cells[i].Value = cellString[i].ToString();
                        }
                    }
                }
                CSVreader.Close();
            }
        }

        public void SaveDeviceCSV(string path)
        {
            StreamWriter streamWriter = new StreamWriter(path, false, Encoding.Default);
            StringBuilder WriteLine = new StringBuilder();
            for (int i = 0; i < uiDataGridViewDevice.Rows[0].Cells.Count; i++)
            {
                WriteLine.Append($"{uiDataGridViewDevice.Columns[i].HeaderText.ToString()},");
            }
            streamWriter.WriteLine(WriteLine.ToString());
            for (int i = 0; i < uiDataGridViewDevice.RowCount - 1; i++)
            {
                WriteLine.Clear();//清除上一行的字符串
                for (int j = 0; j < uiDataGridViewDevice.Rows[i].Cells.Count - 3; j++)
                {
                    //将combobox的值转换为string需要单独处理
                    if (j == 1)
                    {
                        WriteLine.Append($"{((DataGridViewComboBoxCell)uiDataGridViewDevice.Rows[i].Cells[j]).Value.ToString()},");
                    }
                    //将checkBox的值转换为string需要单独处理
                    else if (j >= 4 && j <= 5)
                    {
                        WriteLine.Append($"{uiDataGridViewDevice.Rows[i].Cells[j].EditedFormattedValue.ToString()},");
                    }
                    else
                    {
                        WriteLine.Append($"{uiDataGridViewDevice.Rows[i].Cells[j].Value.ToString()},");
                    }
                }
                streamWriter.WriteLine(WriteLine.ToString());
            }
            streamWriter.Flush();
            streamWriter.Close();
        }

        private void bt_DeviceConfig_Save_Click(object sender, EventArgs e)
        {
            try
            {
                SaveDeviceCSV(DeviceConfigPath);
                messageBox.ShowSuccessDialog("保存成功");
            }
            catch (Exception ex)
            {

                messageBox.ShowErrorDialog(ex.Message.ToString());
            }
        }

        private void bt_DeviceConfig_Backup_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                saveFileDialog.Filter = "CSV文件|*.CSV";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SaveDeviceCSV(saveFileDialog.FileName);
                    messageBox.ShowSuccessDialog("保存成功");
                }
                saveFileDialog.Dispose();
            }
            catch (Exception ex)
            {
                messageBox.ShowErrorDialog(ex.Message.ToString());
            }

        }

        /* 参数编辑区 */
        private void uiDataGridViewDevice_DoubleClick(object sender, EventArgs e)
        {
            if (uiDataGridViewDevice.CurrentRow.Index != uiDataGridViewDevice.RowCount - 1)//如果选择的是不是最后一行就进行处理
            {
                switch (((DataGridViewComboBoxCell)uiDataGridViewDevice.Rows[uiDataGridViewDevice.CurrentRow.Index].Cells[1]).Value.ToString())
                {
                    /*Siemens AB ModbusRtu ModbusTCP OPCClient OPCServer*/
                    case "Siemens":
                        uiTabControl1.SelectedIndex = 0;
                        //将表格内数据填充至编辑区域
                        deviceConfigContorl = new Control[9] { tb_Siemens_Num, cb_Siemens_Drive, tb_Siemens_DeviceType, itb_Siemens_IP, tb_Siemens_Back, tb_Siemens_Slot, ckbx_AutoConnect, cebx_Reconnect, tb_Siemens_PollTime };
                        DevicePareToEditArea(deviceConfigContorl);
                        break;
                    case "AB":
                        uiTabControl1.SelectedIndex = 1;
                        break;
                    case "ModbusRtu":
                        uiTabControl1.SelectedIndex = 2;
                        deviceConfigContorl = new Control[9] { tb_ModRtu_Num, cb_ModRtu_Drive, tb_ModRtu_DeviceType, tb_ModRtu_Pare, tb_ModRtu_Port, tb_ModRtu_Address, ckbx_ModRtu_AutoConnect, cebx_ModRtu_Reconnect, tb_ModRtu_PollTime };
                        DevicePareToEditArea(deviceConfigContorl);
                        break;
                    case "ModbusTCP":
                        uiTabControl1.SelectedIndex = 3;
                        deviceConfigContorl = new Control[9] { tb_ModTcp_Num, cb_ModTcp_Drive, tb_ModTcp_DeviceType, tb_ModTcp_IP, tb_ModTcp_Port, tb_ModTcp_Address, ckbx_ModTcp_AutoConnect, cebx_ModTcp_Reconnect, tb_ModTcp_PollTime };
                        DevicePareToEditArea(deviceConfigContorl);
                        break;
                    case "OPCClient":
                        uiTabControl1.SelectedIndex = 4;
                        break;
                    case "OPCServer":
                        uiTabControl1.SelectedIndex = 5;
                        break;
                    default:
                        break;
                }

            }
        }
        /// <summary>
        /// 将设备表内的数据导入编辑区域
        /// </summary>
        /// <param name="deviceControl">控件数组，按照UI中从下到上的方式添加</param>
        public void DevicePareToEditArea(Control[] deviceControl)
        {
            deviceControl[0].Text = uiDataGridViewDevice.Rows[uiDataGridViewDevice.CurrentRow.Index].Cells[0].Value.ToString();
            deviceControl[1].Text = ((DataGridViewComboBoxCell)uiDataGridViewDevice.Rows[uiDataGridViewDevice.CurrentRow.Index].Cells[1]).Value.ToString();
            deviceControl[2].Text = uiDataGridViewDevice.Rows[uiDataGridViewDevice.CurrentRow.Index].Cells[2].Value.ToString();
            string[] connectPare_Siemens = (uiDataGridViewDevice.Rows[uiDataGridViewDevice.CurrentRow.Index].Cells[3].Value.ToString()).Split('|');
            if (deviceControl[3].Name.Contains("IP"))
            {
                ((UIIPTextBox)deviceControl[3]).Text = connectPare_Siemens[0];
            }
            else
            {
                deviceControl[3].Text = connectPare_Siemens[0];
            }
            deviceControl[4].Text = connectPare_Siemens[1];
            deviceControl[5].Text = connectPare_Siemens[2];
            ((UICheckBox)deviceControl[6]).Checked = bool.Parse(uiDataGridViewDevice.Rows[uiDataGridViewDevice.CurrentRow.Index].Cells[4].EditedFormattedValue.ToString());
            ((UICheckBox)deviceControl[7]).Checked = bool.Parse(uiDataGridViewDevice.Rows[uiDataGridViewDevice.CurrentRow.Index].Cells[5].EditedFormattedValue.ToString());
            deviceControl[8].Text = uiDataGridViewDevice.Rows[uiDataGridViewDevice.CurrentRow.Index].Cells[7].Value.ToString();
        }
        /// <summary>
        /// 将编辑区域的参数写入设备表中
        /// </summary>
        /// <param name="deviceControl">控件数组，按照UI界面总上到下的顺序写入</param>
        /// <param name="rowIndex">由于表格设置了自增尾行，所以如果是新建的话就填入uiDataGridViewDevice.RowCount - 2  如果是修改的话就填入uiDataGridViewDevice.CurrentRow.Index</param>
        public void DevicePareFromEditArea(Control[] deviceControl, int rowIndex)
        {
            uiDataGridViewDevice.Rows[rowIndex].Cells[0].Value = deviceControl[0].Text;
            uiDataGridViewDevice.Rows[rowIndex].Cells[1].Value = deviceControl[1].Text;
            uiDataGridViewDevice.Rows[rowIndex].Cells[2].Value = deviceControl[2].Text;
            StringBuilder connectPare = new StringBuilder();
            connectPare.Clear();
            if (deviceControl[3].Name.Contains("IP"))
            {
                connectPare.Append($"{((UIIPTextBox)deviceControl[3]).Text}|");
            }
            else
            {
                connectPare.Append($"{deviceControl[3].Text}|");
            }
            connectPare.Append($"{deviceControl[4].Text}|");
            connectPare.Append($"{deviceControl[5].Text}");
            uiDataGridViewDevice.Rows[rowIndex].Cells[3].Value = connectPare.ToString();
            uiDataGridViewDevice.Rows[rowIndex].Cells[4].Value = ((UICheckBox)deviceControl[6]).Checked;
            uiDataGridViewDevice.Rows[rowIndex].Cells[5].Value = ((UICheckBox)deviceControl[7]).Checked;
            uiDataGridViewDevice.Rows[rowIndex].Cells[7].Value = deviceControl[8].Text;
        }

        /*西门子通讯参数处理*/
        private void bt_Siemens_Creat_Click(object sender, EventArgs e)
        {
            uiDataGridViewDevice.AddRow();
            deviceConfigContorl = new Control[9] { tb_Siemens_Num, cb_Siemens_Drive, tb_Siemens_DeviceType, itb_Siemens_IP, tb_Siemens_Back, tb_Siemens_Slot, ckbx_AutoConnect, cebx_Reconnect, tb_Siemens_PollTime };
            DevicePareFromEditArea(deviceConfigContorl, uiDataGridViewDevice.RowCount - 2);
        }

        private void bt_Siemens_Modify_Click(object sender, EventArgs e)
        {
            deviceConfigContorl = new Control[9] { tb_Siemens_Num, cb_Siemens_Drive, tb_Siemens_DeviceType, itb_Siemens_IP, tb_Siemens_Back, tb_Siemens_Slot, ckbx_AutoConnect, cebx_Reconnect, tb_Siemens_PollTime };
            DevicePareFromEditArea(deviceConfigContorl, uiDataGridViewDevice.CurrentRow.Index);
        }

        private void bt_Siemens_Delet_Click(object sender, EventArgs e)
        {
            uiDataGridViewDevice.Rows.RemoveAt(uiDataGridViewDevice.CurrentRow.Index);
        }
        /*ModBusRtu通讯参数处理*/
        private void bt_ModRtu_Creat_Click(object sender, EventArgs e)
        {
            uiDataGridViewDevice.AddRow();
            deviceConfigContorl = new Control[9] { tb_ModRtu_Num, cb_ModRtu_Drive, tb_ModRtu_DeviceType, tb_ModRtu_Pare, tb_ModRtu_Port, tb_ModRtu_Address, ckbx_ModRtu_AutoConnect, cebx_ModRtu_Reconnect, tb_ModRtu_PollTime };
            DevicePareFromEditArea(deviceConfigContorl, uiDataGridViewDevice.RowCount - 2);
        }

        private void bt_ModRtu_Modify_Click(object sender, EventArgs e)
        {
            deviceConfigContorl = new Control[9] { tb_ModRtu_Num, cb_ModRtu_Drive, tb_ModRtu_DeviceType, tb_ModRtu_Pare, tb_ModRtu_Port, tb_ModRtu_Address, ckbx_ModRtu_AutoConnect, cebx_ModRtu_Reconnect, tb_ModRtu_PollTime };
            DevicePareFromEditArea(deviceConfigContorl, uiDataGridViewDevice.CurrentRow.Index);
        }

        private void bt_ModRtu_Delet_Click(object sender, EventArgs e)
        {
            uiDataGridViewDevice.Rows.RemoveAt(uiDataGridViewDevice.CurrentRow.Index);
        }

        /*ModBusTcp通讯参数处理*/
        private void bt_ModTcp_Creat_Click(object sender, EventArgs e)
        {
            uiDataGridViewDevice.AddRow();
            deviceConfigContorl = new Control[9] { tb_ModTcp_Num, cb_ModTcp_Drive, tb_ModTcp_DeviceType, tb_ModTcp_IP, tb_ModTcp_Port, tb_ModTcp_Address, ckbx_ModTcp_AutoConnect, cebx_ModTcp_Reconnect, tb_ModTcp_PollTime };
            DevicePareFromEditArea(deviceConfigContorl, uiDataGridViewDevice.RowCount - 2);
        }

        private void bt_ModTcp_Modify_Click(object sender, EventArgs e)
        {
            deviceConfigContorl = new Control[9] { tb_ModTcp_Num, cb_ModTcp_Drive, tb_ModTcp_DeviceType, tb_ModTcp_IP, tb_ModTcp_Port, tb_ModTcp_Address, ckbx_ModTcp_AutoConnect, cebx_ModTcp_Reconnect, tb_ModTcp_PollTime };
            DevicePareFromEditArea(deviceConfigContorl, uiDataGridViewDevice.CurrentRow.Index);
        }

        private void bt_ModTcp_Delet_Click(object sender, EventArgs e)
        {
            uiDataGridViewDevice.Rows.RemoveAt(uiDataGridViewDevice.CurrentRow.Index);
        }

        private void bt_DeviceScan_Start_Click(object sender, EventArgs e)
        {
            deviceDictionary[uiDataGridViewDevice.CurrentRow.Cells[0].Value.ToString()].scanDeviceManualResetEvent.Set();//将对设备进行处理的所有线程激活
        }

        private void bt_DeviceScan_Stop_Click(object sender, EventArgs e)
        {
            deviceDictionary[uiDataGridViewDevice.CurrentRow.Cells[0].Value.ToString()].scanDeviceManualResetEvent.Reset();//将对设备进行处理的所有线程挂起
        }
        #endregion

        #region 设备表内右键菜单处理
        private void MenuItem_Device_Delte_Click(object sender, EventArgs e)
        {
            uiDataGridViewDevice.Rows.Remove(uiDataGridViewDevice.Rows[uiDataGridViewDevice.SelectedIndex]);
        }
        private void MenuItem_Device_Copy_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(uiDataGridViewDevice.GetClipboardContent());//复制至粘贴板
        }
        private void MenuItem_Device_Prset_Click(object sender, EventArgs e)
        {
            try
            {
                string pasteText = Clipboard.GetText();//获取粘贴板中的字符串
                string[] test = pasteText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);//获得每行的字符串放入数组内
                bool alarmEnable = false;
                int selectIndex = uiDataGridViewDevice.SelectedIndex;//注意这里的selectedIndex会随着行数的增加而增加，我们需要保留当前值来使用
                for (int i = 0; i < test.Count(); i++)
                {
                    string[] vals = test[i].Split('\t');
                    uiDataGridViewDevice.Rows.Add();
                    for (int j = 0; j < vals.Count(); j++)
                    {
                        if (i >= 4 && i <= 5)//将字符串转换为bool量需要单独处理
                        {
                            alarmEnable = bool.Parse(vals[j].ToString());
                            uiDataGridViewDevice.Rows[selectIndex].Cells[j].Value = alarmEnable;//以当前选中行为基础增加
                        }
                        else
                        {
                            uiDataGridViewDevice.Rows[selectIndex].Cells[j].Value = vals[j].ToString();
                        }
                    }
                    selectIndex++;
                }
            }
            catch (Exception ex)
            {
                messageBox.ShowErrorDialog(ex.Message.ToString());
            }
        }

        #endregion

        #region 所有设备连接方法、轮询读取方法、报警轮询方法、报警查询方法

        /// <summary>
        /// 读取设备表内参数并填充至Dictionary内
        /// </summary>
        /// <param name="path"></param>
        private void InitDeviceCSV()
        {
            //填充设备字典
            for (int q = 0; q < uiDataGridViewDevice.RowCount - 1; q++)
            {
                List<object> pareList = new List<object>();
                string[] str = (uiDataGridViewDevice.Rows[q].Cells[3].Value.ToString()).Split('|');
                switch (((DataGridViewComboBoxCell)uiDataGridViewDevice.Rows[q].Cells[1]).Value.ToString())
                {
                    case "Siemens"://string ip,string cpuType,short rack,short slot,bool autoConnect,bool reconnect,int pollTime,string connectTag  按照顺序依次添加
                        pareList.Add(str[0]);
                        pareList.Add(uiDataGridViewDevice.Rows[q].Cells[2].Value.ToString());
                        pareList.Add(Convert.ToInt16(str[1]));
                        pareList.Add(Convert.ToInt16(str[2]));
                        pareList.Add(uiDataGridViewDevice.Rows[q].Cells[4].EditedFormattedValue);
                        pareList.Add(uiDataGridViewDevice.Rows[q].Cells[5].EditedFormattedValue);
                        pareList.Add(Convert.ToInt32(uiDataGridViewDevice.Rows[q].Cells[7].Value));
                        pareList.Add(uiDataGridViewDevice.Rows[q].Cells[6].Value.ToString());
                        devicePareDictionary.Add(new string[] { uiDataGridViewDevice.Rows[q].Cells[0].Value.ToString(), uiDataGridViewDevice.Rows[q].Cells[1].Value.ToString() }, pareList);
                        continue;//这里必须使用coninue否则将会直接跳出For循环
                    //case "AB":
                    //    break;
                    case "ModbusRtu"://string portName, int baudRate, int dataBits, int stopBits, int parity, bool autoConnect, bool reconnect, int pollTime,byte slaveAddress,string connectTag
                        pareList.Add(Convert.ToInt32(str[1]));
                        string[] comStr = str[0].Split('-');
                        pareList.Add(Convert.ToInt32(comStr[0]));
                        pareList.Add(Convert.ToInt32(comStr[2]));
                        pareList.Add(Convert.ToInt32(comStr[3]));
                        pareList.Add(Convert.ToInt32(comStr[1]));
                        pareList.Add(uiDataGridViewDevice.Rows[q].Cells[4].EditedFormattedValue);
                        pareList.Add(uiDataGridViewDevice.Rows[q].Cells[5].EditedFormattedValue);
                        pareList.Add(Convert.ToInt32(uiDataGridViewDevice.Rows[q].Cells[7].Value));
                        pareList.Add(Convert.ToByte(str[2]));
                        pareList.Add(uiDataGridViewDevice.Rows[q].Cells[6].Value.ToString());
                        devicePareDictionary.Add(new string[] { uiDataGridViewDevice.Rows[q].Cells[0].Value.ToString(), uiDataGridViewDevice.Rows[q].Cells[1].Value.ToString() }, pareList);
                        continue;
                    case "ModbusTCP"://string ip, int portNum,int stationAddress, bool autoConnect, bool reconnect, int pollTime,string connectTag
                        pareList.Add(str[0]);
                        pareList.Add(Convert.ToInt32(str[1]));
                        pareList.Add(Convert.ToInt32(str[2]));
                        pareList.Add(uiDataGridViewDevice.Rows[q].Cells[4].EditedFormattedValue);
                        pareList.Add(uiDataGridViewDevice.Rows[q].Cells[5].EditedFormattedValue);
                        pareList.Add(Convert.ToInt32(uiDataGridViewDevice.Rows[q].Cells[7].Value));
                        pareList.Add(uiDataGridViewDevice.Rows[q].Cells[6].Value.ToString());
                        devicePareDictionary.Add(new string[] { uiDataGridViewDevice.Rows[q].Cells[0].Value.ToString(), uiDataGridViewDevice.Rows[q].Cells[1].Value.ToString() }, pareList);
                        continue;
                        //case "OPCClient":
                        //    break;
                        //case "OPCServer":
                        //    break;

                }
            }
        }
        /// <summary>
        /// 连接允许连接的设备
        /// </summary>
        /// <param name="device"></param>
        private void PollingDevice(Dictionary<string[], List<object>> device)
        {
            //生成设备
            foreach (var item in device)
            {
                deviceDictionary.Add(item.Key[0], CommunicateDevice.CreatDevices(item.Key[1], item.Value));
            }
            deviceCount = deviceDictionary.Count;
            //连接设备
            foreach (var item in deviceDictionary)
            {
                if (item.Value.AutoConnect && item.Value.InitPlc())
                {
                    Task.Run(new Action(() =>
                    {
                        item.Value.Connect();
                    }));
                }
            }
            //记录设备在设备表中的行号
            for (int i = 0; i < uiDataGridViewDevice.Rows.Count - 1; i++)
            {
                deviceNum.Add(uiDataGridViewDevice.Rows[i].Cells[0].Value.ToString(), i);
            }
            //在多线程内循环执行读取设备状态并重连操作，将结果刷新至UI 
            foreach (var item in deviceDictionary)
            {
                Task.Run(new Action(() =>
                {
                    while (true)
                    {
                        item.Value.scanDeviceManualResetEvent.WaitOne();
                        deviceConnectState = item.Value.GetStateAndReconnect();
                        this.BeginInvoke(new Action(() =>
                        {
                            if (deviceConnectState)
                            {
                                uiDataGridViewDevice.Rows[deviceNum[item.Key]].Cells[8].Value = "连接成功";
                            }
                            else
                            {
                                uiDataGridViewDevice.Rows[deviceNum[item.Key]].Cells[8].Value = "连接失败";
                            }
                        }));
                        Thread.Sleep(5000);
                    }
                }));
            }
        }
        /// <summary>
        ///将点添加至对应设备中
        /// </summary>
        public void PointCreat()
        {
            for (int i = 0; i < uiDataGridViewDB.Rows.Count - 1; i++)
            {
                PointPare pointPare = new PointPare();
                pointPare.address = uiDataGridViewDB.Rows[i].Cells[2].Value.ToString();
                pointPare.communcationDevice = uiDataGridViewDB.Rows[i].Cells[1].Value.ToString();
                pointPare.dataType = ((DataGridViewComboBoxCell)uiDataGridViewDB.Rows[i].Cells[3]).Value.ToString();
                pointPare.deviceName = uiDataGridViewDB.Rows[i].Cells[5].Value.ToString();
                pointPare.scan = (bool)uiDataGridViewDB.Rows[i].Cells[6].EditedFormattedValue;
                pointPare.alarmEnable = (bool)uiDataGridViewDB.Rows[i].Cells[7].EditedFormattedValue;
                pointPare.history = (bool)uiDataGridViewDB.Rows[i].Cells[8].EditedFormattedValue;
                pointPare.hAlarm = Convert.ToSingle(uiDataGridViewDB.Rows[i].Cells[9].Value);
                pointPare.hhAlarm = Convert.ToSingle(uiDataGridViewDB.Rows[i].Cells[10].Value);
                pointPare.lAlarm = Convert.ToSingle(uiDataGridViewDB.Rows[i].Cells[11].Value);
                pointPare.llAlarm = Convert.ToSingle(uiDataGridViewDB.Rows[i].Cells[12].Value);
                pointPare.unit = uiDataGridViewDB.Rows[i].Cells[13].Value.ToString();
                pointPare.describe = uiDataGridViewDB.Rows[i].Cells[4].Value.ToString();
                deviceDictionary[uiDataGridViewDB.Rows[i].Cells[1].Value.ToString()].PointDictionary.Add(uiDataGridViewDB.Rows[i].Cells[0].Value.ToString(), pointPare);
            }
            ////将所有要读取的点位方法注册至委托上
            //foreach (var item in deviceDictionary)
            //{
            //    item.Value.PointReadHandle();
            //}
        }
        /// <summary>
        /// 为每个设备开启一个线程来读取数据
        /// </summary>
        public void PointScan()
        {
            foreach (var item in deviceDictionary)
            {
                item.Value.PointReadHandle();
            }
        }
        /// <summary>
        /// 采集报警参数并写入数据库形成记录,附加UI刷新事件的注册--通过更改数据库为事件来触发UI刷新节省系统资源
        /// </summary>
        public void AlarmRecode()
        {
            foreach (var item in deviceDictionary)
            {
                item.Value.actualAlarmRefresh += new Action(ActualAlarmQuery);
            }
            Task.Run(new Action(() =>
            {
                while (true)
                {
                    foreach (var item in deviceDictionary)
                    {
                        item.Value.AlarmHandle(sqlhandle);
                    }
                    Thread.Sleep(1000);
                }
            }));
        }
        /// <summary>
        /// 查询实时报警记录并写入实时报警参数列表中
        /// </summary>
        public void ActualAlarmQuery()
        {
            string sql = "SELECT * from AlarmRecode   where confirm = '未确认' or stopTime is NULL order by startTime desc";
            int i;
            try
            {
                alarmRecodes.Clear();
                using (IDataReader idr = sqlhandle.read(sql))
                {
                    i = 1;
                    while (idr.Read())
                    {
                        //uiDataGridViewActualAlarm.Rows.Add(i, idr[1].ToString(), idr[2].ToString(), idr[3].ToString(), (idr[4] is null ? "" : idr[4].ToString()), idr[5].ToString(), idr[6].ToString(), idr[8].ToString());
                        alarmRecodes.Add(new AlarmRecode(i.ToString(), idr[1].ToString(), idr[2].ToString(), idr[3].ToString(), (idr[4] is null ? "" : idr[4].ToString()), idr[5].ToString(), idr[6].ToString(), idr[8].ToString()));
                        i++;
                    }
                    AlarmPageChangeHandle(uiPaginationActualAlarm.ActivePage, uiPaginationActualAlarm.PageSize);
                    idr.Close();
                    idr.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }
        /// <summary>
        /// 分页控件页面切换事件处理
        /// </summary>
        /// <param name="index">当前页面位置</param>
        /// <param name="count">设定的页面内显示的记录数</param>
        public void AlarmPageChangeHandle(int index, int count)
        {
            this.Invoke(new Action(() =>
            {
                uiDataGridViewActualAlarm.Rows.Clear();
                for (int i = (index - 1) * count; i < (index - 1) * count + count; i++)
                {
                    if (i >= alarmRecodes.Count) continue;
                    uiDataGridViewActualAlarm.Rows.Add(i.ToString(), alarmRecodes[i].AlarmName, alarmRecodes[i].AlarmDescribe, alarmRecodes[i].StartTime, alarmRecodes[i].StopTime, alarmRecodes[i].AlarmType, alarmRecodes[i].ActualValue, alarmRecodes[i].AlarmConfirm);//首先填充表格
                    try
                    {
                        if (uiDataGridViewActualAlarm.Rows[i-(index-1)*count].Cells[7].Value.ToString() == "未确认" || uiDataGridViewActualAlarm.Rows[i - (index - 1) * count].Cells[4].Value.ToString() == "")//根据不同的条件改变报警信息字体的颜色
                        {
                            switch (uiDataGridViewActualAlarm.Rows[i - (index - 1) * count].Cells[5].Value.ToString())
                            {
                                case "状态量报警":
                                    uiDataGridViewActualAlarm.Rows[i - (index - 1) * count].DefaultCellStyle.ForeColor = Color.Red;
                                    uiDataGridViewActualAlarm.Rows[i - (index - 1) * count].DefaultCellStyle.SelectionForeColor = Color.Red;
                                    break;
                                case "低低报":
                                    uiDataGridViewActualAlarm.Rows[i - (index - 1) * count].DefaultCellStyle.ForeColor = Color.Red;
                                    uiDataGridViewActualAlarm.Rows[i - (index - 1) * count].DefaultCellStyle.SelectionForeColor = Color.Red;
                                    break;
                                case "低报":
                                    uiDataGridViewActualAlarm.Rows[i - (index - 1) * count].DefaultCellStyle.ForeColor = Color.Goldenrod;
                                    uiDataGridViewActualAlarm.Rows[i - (index - 1) * count].DefaultCellStyle.SelectionForeColor = Color.Goldenrod;
                                    break;
                                case "高报":
                                    uiDataGridViewActualAlarm.Rows[i - (index - 1) * count].DefaultCellStyle.ForeColor = Color.Goldenrod;
                                    uiDataGridViewActualAlarm.Rows[i - (index - 1) * count].DefaultCellStyle.SelectionForeColor = Color.Goldenrod;
                                    break;
                                case "高高报":
                                    uiDataGridViewActualAlarm.Rows[i - (index - 1) * count].DefaultCellStyle.ForeColor = Color.Red;
                                    uiDataGridViewActualAlarm.Rows[i - (index - 1) * count].DefaultCellStyle.SelectionForeColor = Color.Red;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            uiDataGridViewActualAlarm.Rows[i - (index - 1) * count].DefaultCellStyle.ForeColor = Color.Black;
                            uiDataGridViewActualAlarm.Rows[i - (index - 1) * count].DefaultCellStyle.SelectionForeColor = Color.Black;
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    
                }
            }));
        }
        /// <summary>
        /// 页面切换事件处理器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pagingSource"></param>
        /// <param name="pageIndex"></param>
        /// <param name="count"></param>
        private void uiPaginationActualAlarm_PageChanged(object sender, object pagingSource, int pageIndex, int count)
        {
            ActualAlarmQuery();
        }
        #endregion

        #region 实时报警表内右键菜单处理
        private void ToolStripMenuItemAlarmDisable_Click(object sender, EventArgs e)
        {
            string pointName = uiDataGridViewActualAlarm.Rows[uiDataGridViewActualAlarm.CurrentRow.Index].Cells[1].Value.ToString();
            //首先将表格内的启用报警置为FALSE
            for (int i = 0; i < uiDataGridViewDB.Rows.Count - 1; i++)//后续都是用LINQ来进行操作
            {
                if (uiDataGridViewDB.Rows[i].Cells[0].Value.ToString() == pointName)
                {
                    uiDataGridViewDB.Rows[i].Cells[7].Value = false;
                }
            }
            //改变表格之后保存点表
            SavePointCSV(Directory.GetCurrentDirectory() + "\\Variable.csv");
            //将运行中的目标设备中的点报警检测功能停用，在后续的循环中将不在检测此点位是否报警
            foreach (var itemDevice in deviceDictionary)//后续都是用LINQ来进行操作
            {
                foreach (var itemPoint in itemDevice.Value.PointDictionary)
                {
                    if (itemPoint.Key == pointName)
                    {
                        itemPoint.Value.alarmEnable = false;
                        return;
                    }
                }
            }
        }
        private void ToolStripMenuItemAlarmConfirm_Click(object sender, EventArgs e)
        {
            //uiDataGridViewDB.Rows.RemoveAt(uiDataGridViewDB.CurrentRow.Index);
            string sql = $"update AlarmRecode set confirm = '已确认' where pointName = '{uiDataGridViewActualAlarm.Rows[uiDataGridViewActualAlarm.CurrentRow.Index].Cells[1].Value.ToString()}'";
            sqlhandle.excute(sql);
            ActualAlarmQuery();

        }
        private void ToolStripMenuItemAlarmConfirmAll_Click(object sender, EventArgs e)
        {
            StringBuilder sqlbuilder = new StringBuilder();
            for (int i = 0; i < uiDataGridViewActualAlarm.Rows.Count; i++)
            {
                sqlbuilder.Append($"update AlarmRecode set confirm = '已确认' where pointName = '{uiDataGridViewActualAlarm.Rows[i].Cells[1].Value.ToString()}';");
            }
            sqlhandle.excute(sqlbuilder.ToString());
            ActualAlarmQuery();
        }

        #endregion

        #region 历史报警记录处理
        private void bt_AlarmRangeQuery_Click(object sender, EventArgs e)
        {
            int index = 1;
            uiDataGridViewAlarmHistory.Rows.Clear();
            string startTime = DTP_AlarmStartTime.Text;
            string stopTime = DTP_AlarmStopTime.Text;
            string alarmType;
            if (uiComboBox_AlarmType.Text != "所有报警")
            {
                alarmType = $" and alarmType = '{uiComboBox_AlarmType.Text}'";
            }
            else
            {
                alarmType = "";
            }
            string sql = $"SELECT * from AlarmRecode WHERE startTime BETWEEN '{startTime}' and '{stopTime}' {alarmType}";
            IDataReader dataReader = sqlhandle.read(sql);
            while (dataReader.Read())
            {
                uiDataGridViewAlarmHistory.Rows.Add(index.ToString(), dataReader[1].ToString(), dataReader[2].ToString(), dataReader[3].ToString(), dataReader[4].ToString(), dataReader[5].ToString(), dataReader[6].ToString(), dataReader[7].ToString());
                index++;
            }
            dataReader.Close();
            dataReader.Dispose();
        }

        private void bt_AlarmTodayQuery_Click(object sender, EventArgs e)
        {
            int index = 1;
            uiDataGridViewAlarmHistory.Rows.Clear();
            string sql = "";
            if (uiComboBox_AlarmType.Text == "所有报警")
            {
                sql = $"SELECT  * FROM AlarmRecode where CONVERT(varchar,startTime,120) LIKE '{DateTime.Now.Year}-{DateTime.Now.Month.ToString().PadLeft(2, '0')}-{DateTime.Now.Day.ToString().PadLeft(2, '0')}%' and LEN(stopTime)>0 and confirm = '已确认'";
            }
            else
            {
                sql = $"SELECT  * FROM AlarmRecode where CONVERT(varchar,startTime,120) LIKE '{DateTime.Now.Year}-{DateTime.Now.Month.ToString().PadLeft(2, '0')}-{DateTime.Now.Day.ToString().PadLeft(2, '0')}%' and LEN(stopTime)>0 and confirm = '已确认' and alarmType = '{uiComboBox_AlarmType.Text}'";
            }
            IDataReader dataReader = sqlhandle.read(sql);
            while (dataReader.Read())
            {
                uiDataGridViewAlarmHistory.Rows.Add(index.ToString(), dataReader[1].ToString(), dataReader[2].ToString(), dataReader[3].ToString(), dataReader[4].ToString(), dataReader[5].ToString(), dataReader[6].ToString(), dataReader[7].ToString());
                index++;
            }
            dataReader.Close();
            dataReader.Dispose();
        }

        private void bt_AlarmExpert_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            StringBuilder builderWriteLine = new StringBuilder();
            saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            saveFileDialog.Filter = "CSV文件|*.csv";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamWriter stream = new StreamWriter(saveFileDialog.FileName, false, Encoding.Default);//写入表头
                for (int i = 0; i < uiDataGridViewAlarmHistory.Rows[0].Cells.Count; i++)
                {
                    builderWriteLine.Append($"{uiDataGridViewAlarmHistory.Columns[i].HeaderText.ToString()},");
                }
                stream.WriteLine(builderWriteLine.ToString());
                for (int i = 0; i < uiDataGridViewAlarmHistory.Rows.Count-1; i++)//写入内容
                {
                    builderWriteLine.Clear();
                    for (int j = 0; j < uiDataGridViewAlarmHistory.Rows[i].Cells.Count; j++)
                    {
                        builderWriteLine.Append($"{uiDataGridViewAlarmHistory.Rows[i].Cells[j].Value.ToString()},");
                    }
                    stream.WriteLine(builderWriteLine.ToString());
                }
                stream.Flush();
                stream.Close();
                messageBox.ShowSuccessDialog("报警记录导出成功");
            }
        }
        #endregion
    }
}
