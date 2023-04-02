using S7.Net;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ScadaFrame
{
    public class SiemensPLC : CommunicateDevice
    {
        public SiemensPLC(string ip, string cpuType, short rack, short slot, bool autoConnect, bool reconnect, int pollTime, string connectTag) : base(autoConnect, reconnect, pollTime, connectTag)
        {
            this.IP = ip;
            this.Cpu = cpuType;
            this.Rack = rack;
            this.Slot = slot;
        }
        //public override event Action<string> errorMessageEvent;
        Plc plc;
        private string _ip;
        public string IP
        {
            get { return _ip; }
            set
            {
                if (Regex.IsMatch(value, @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$"))
                {
                    _ip = value;
                }
            }
        }

        private CpuType _cpu;
        public string Cpu
        {
            get
            {
                return _cpu.ToString();
            }
            set
            {
                switch (value)
                {
                    case "S7200":
                        _cpu = CpuType.S7200;
                        break;
                    case "Logo0BA8":
                        _cpu = CpuType.Logo0BA8;
                        break;
                    case "S7200Smart":
                        _cpu = CpuType.S7200Smart;
                        break;
                    case "S7300":
                        _cpu = CpuType.S7300;
                        break;
                    case "S7400":
                        _cpu = CpuType.S7400;
                        break;
                    case "S71200":
                        _cpu = CpuType.S71200;
                        break;
                    case "S71500":
                        _cpu = CpuType.S71500;
                        break;
                    default:
                        _cpu = CpuType.S71500;
                        break;
                }
            }
        }

        private short _rack;
        public short Rack
        {
            get { return _rack; }
            set { _rack = value; }
        }

        private short _slot;
        public short Slot
        {
            get { return _slot; }
            set { _slot = value; }
        }

        public override bool State
        {
            get
            {
                if (plc != null && plc.IsConnected)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool _autoConnect;
        public override bool AutoConnect
        {
            get { return _autoConnect; }
            set { _autoConnect = value; }
        }

        private bool _reconnect;
        public override bool Reconnect
        {
            get { return _reconnect; }
            set { _reconnect = value; }
        }

        private int _pollTime;
        public override int PollTime
        {
            get { return _pollTime; }
            set { _pollTime = value; }
        }

        private string _connectTag;
        public override string ConnectTag
        {
            get { return _connectTag; }
            set { _connectTag = value; }
        }

        public override bool InitPlc()
        {
            plc = new Plc(_cpu, _ip, _rack, _slot);
            if (plc != null)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 连接PLC
        /// </summary>
        /// <returns></returns>
        public override bool Connect()
        {
            if (plc.IsConnected == false)
            {
                try
                {
                    plc.Open();
                    if (plc.IsConnected == true)
                        return true;
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    base.ShowMessage(ex.Message.ToString());
                    return false;
                }
            }
            else
                return true;
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public override bool DisConnect()
        {
            if (plc.IsConnected == true)
            {
                try
                {
                    plc.Close();
                    if (plc.IsConnected == false)
                        return true;
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    base.ShowMessage(ex.Message.ToString());
                    return false;
                }
            }
            else
                return true;
        }
        /// <summary>
        /// 读取BOOL类型
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public bool ReadBool(string address)
        {
            bool value = (bool)plc.Read(address);
            return value;
        }
        /// <summary>
        /// 读取字节
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public byte ReadByte(string address)
        {
            byte value = (byte)plc.Read(address);
            return value;
        }
        /// <summary>
        /// 读取int类型
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public short ReadShort(string address)
        {
            short value = ((ushort)plc.Read(address)).ConvertToShort();
            return value;
        }
        /// <summary>
        /// 读取DINT类型
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public int ReadDInt(string address)
        {
            int value = ((uint)plc.Read(address)).ConvertToInt();
            return value;
        }
        /// <summary>
        /// 读取浮点型
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public float ReadFloat(string address)
        {
            float value = ((uint)plc.Read(address)).ConvertToFloat();
            return value;
        }
        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="dataType">写入的数据类型 一般为DataType.DataBlock</param>
        /// <param name="dbAddress">DB块的地址</param>
        /// <param name="offSet">偏移量</param>
        /// <param name="value">待写入的字符串</param>
        public void WriteString(DataType dataType, int dbAddress, int offSet, string value)
        {
            byte[] byt = Encoding.ASCII.GetBytes(value);
            byte[] s7byt = S7.Net.Types.S7String.ToByteArray(value, byt.Length);
            plc.WriteBytes(dataType, dbAddress, offSet, byt);
        }
        /// <summary>
        /// 写入bool量
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="value"></param>
        public void WriteBoole(string address, bool value)
        {
            plc.Write(address, value);
        }
        /// <summary>
        /// 写入16位int整数
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="value"></param>
        public void WriteInt(string address, int value)
        {
            plc.Write(address, Convert.ToInt16(value));
        }
        /// <summary>
        /// 写入浮点型
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="value"></param>
        public void WriteFloat(string address, float value)
        {
            plc.Write(address, Convert.ToSingle(value));
        }
        /// <summary>
        /// 获取连接状态并重连
        /// </summary>
        /// <returns></returns>
        public override bool GetStateAndReconnect()
        {
            try
            {
                if (plc.IsConnected)
                {
                    ReadBool(this.ConnectTag);//需要读取一次才能更新连接状态
                    return plc.IsConnected;
                }
                else
                {
                    Connect();
                    return plc.IsConnected;
                }
            }
            catch (Exception)
            {
                return plc.IsConnected;
            }
        }
        /// <summary>
        /// 点位读取
        /// </summary>
        public override void PointReadHandle()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (State)//首先判断设备是否连接，如果未连接就不执行，节约系统资源
                    {
                        scanDeviceManualResetEvent.WaitOne();//用于外部挂起线程
                        foreach (var item in base.PointDictionary)
                        {
                            if (item.Value.scan)//如果变量未开启扫描则不执行，节约资源
                            {
                                switch (item.Value.dataType)
                                {
                                    //BOOL INT DINT REAL BYTE
                                    case "BOOL":
                                        item.Value.value = ReadBool(item.Value.address);
                                        break;
                                    case "INT":
                                        item.Value.value = ReadShort(item.Value.address);
                                        break;
                                    case "DINT":
                                        item.Value.value = ReadDInt(item.Value.address);
                                        break;
                                    case "REAL":
                                        item.Value.value = ReadFloat(item.Value.address);
                                        break;
                                    case "BYTE":
                                        item.Value.value = ReadByte(item.Value.address);
                                        break;
                                }
                            }
                        }
                    }
                    Thread.Sleep(this.PollTime);
                }
            });
        }
        //public void ShowMessage()
        //{
        //    errorMessageEvent.Invoke("test");
        //}

    }
}
