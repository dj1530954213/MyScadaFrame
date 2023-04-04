using HslCommunication;
using HslCommunication.Profinet.AllenBradley;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ScadaFrame
{
    class ABPLC : CommunicateDevice
    {
        public ABPLC(string ip, byte cpuSlot, string route, bool autoConnect, bool reconnect, int pollTime, string connectTag) : base(autoConnect, reconnect, pollTime, connectTag)
        {
            this.Ip = ip;
            this.CpuSlot = cpuSlot;
            this.Route = route;
        }

        private string _ip;
        public string Ip
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

        private byte _cpuSlot;
        public byte CpuSlot
        {
            get { return _cpuSlot; }
            set { _cpuSlot = value; }
        }

        private string _route;
        public string Route
        {
            get { return _route; }
            set { _route = value; }
        }

        public override bool State
        {
            get
            {
                return false;
            }
        }

        public AllenBradleyNet abPLC = new AllenBradleyNet();
        public OperateResult connectState;

        public override bool Connect()
        {
            if (connectState is null)
            {
                try
                {
                    connectState = abPLC.ConnectServer();
                    return true;
                }
                catch (Exception ex)
                {
                    base.ShowMessage(ex.Message.ToString());
                    connectState.IsSuccess = false;
                    return false;
                }
            }
            else if (!connectState.IsSuccess)
            {
                try
                {
                    connectState = abPLC.ConnectServer();
                    return true;
                }
                catch (Exception ex)
                {
                    base.ShowMessage(ex.Message.ToString());
                    connectState.IsSuccess = false;
                    return false;
                }
            }
            return true;
        }
        public bool ReadBool(string address)
        {
            return abPLC.ReadBool(address).Content;
        }
        public byte ReadByte(string address)
        {
            return abPLC.ReadByte(address).Content;
        }
        public short ReadShort(string address)
        {
            return abPLC.ReadInt16(address).Content;
        }
        public int ReadDInt(string address)
        {
            return abPLC.ReadInt32(address).Content;
        }
        public float ReadFloat(string address)
        {
            return abPLC.ReadFloat(address).Content;
        }
        public void WriteBoole(string address, bool value)
        {
            abPLC.Write(address, value);
        }
        public void WriteInt(byte slaveAddress, string address, ushort value)
        {
            abPLC.Write(address, value);
        }
        public void WriteDint(byte slaveAddress, string address, int value)
        {
            abPLC.Write(address,value);
        }
        public void WriteReal(byte slaveAddress, string address, float value)
        {
            abPLC.Write(address, value);
        }
        public override bool DisConnect()
        {
            if (!(connectState is null) && connectState.IsSuccess)
            {
                try
                {
                    connectState = abPLC.ConnectClose();
                    return true;
                }
                catch (Exception ex)
                {
                    base.ShowMessage(ex.Message.ToString());
                }
            }
            return false;

        }
        public override bool GetStateAndReconnect()
        {
            try
            {
                if (connectState.IsSuccess)
                {
                    connectState = abPLC.ReadBool(ConnectTag);
                    return connectState.IsSuccess;
                }
                else
                {
                    connectState = abPLC.ConnectServer();
                    return connectState.IsSuccess;
                }
            }
            catch (Exception ex)
            {
                connectState.IsSuccess = false;
                base.ShowMessage(ex.Message.ToString());
                return false;
            }
        }
        public override bool InitPlc()
        {
            abPLC.IpAddress = this.Ip;
            abPLC.Port = 44818;
            abPLC.Slot = this.CpuSlot;
            abPLC.MessageRouter = new MessageRouter(this.Route);
            if (abPLC != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
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
                                    //case "BOOL":
                                    //    item.Value.value = ReadBool(item.Value.address);
                                    //    break;
                                    //case "INT":
                                    //    item.Value.value = ReadShort(item.Value.address);
                                    //    break;
                                    //case "DINT":
                                    //    item.Value.value = ReadDInt(item.Value.address);
                                    //    break;
                                    //case "REAL":
                                    //    item.Value.value = ReadFloat(item.Value.address);
                                    //    break;
                                    //case "BYTE":
                                    //    item.Value.value = ReadByte(item.Value.address);
                                    //    break;
                                    case "BOOL":
                                        item.Value.value = ReadBool(item.Value.address);
                                        break;
                                    case "INT":
                                        item.Value.value = base.deadZoneCheck<short>(ref item.Value.valueRecode, ReadShort(item.Value.address), item.Value.deadZone, ref item.Value.valueChanged);
                                        //item.Value.value = ReadShort(item.Value.address);
                                        break;
                                    case "DINT":
                                        item.Value.value = base.deadZoneCheck<int>(ref item.Value.valueRecode, ReadDInt(item.Value.address), item.Value.deadZone, ref item.Value.valueChanged);
                                        //item.Value.value = ReadDInt(item.Value.address);
                                        break;
                                    case "REAL":
                                        item.Value.value = base.deadZoneCheck<float>(ref item.Value.valueRecode, ReadFloat(item.Value.address), item.Value.deadZone, ref item.Value.valueChanged);
                                        //item.Value.value = ReadFloat(item.Value.address);
                                        break;
                                    case "BYTE":
                                        item.Value.value = base.deadZoneCheck<float>(ref item.Value.valueRecode, ReadFloat(item.Value.address), item.Value.deadZone, ref item.Value.valueChanged);
                                        //item.Value.value = ReadByte(item.Value.address);
                                        break;
                                }
                            }
                        }
                    }
                    Thread.Sleep(this.PollTime);
                }
            });
        }
    }
}
