using Modbus.Device;
using System;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ScadaFrame
{
    class ModbusTCP : CommunicateDevice
    {
        public ModbusTCP(string ip, int portNum, int stationAddress, bool autoConnect, bool reconnect, int pollTime, string connectTag) : base(autoConnect, reconnect, pollTime, connectTag)
        {
            IP = ip;
            PortNum = portNum;
            StationAddress = stationAddress;
        }
        TcpClient tcpClient;
        ModbusIpMaster master;
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

        private int _portNum;
        public int PortNum
        {
            get { return _portNum; }
            set { _portNum = value; }
        }

        private int _stationAddress;
        public int StationAddress
        {
            get { return _stationAddress; }
            set { _stationAddress = value; }
        }

        public override bool State
        {
            get
            {
                if (tcpClient != null)
                {
                    return tcpClient.Connected;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool ReadBool(string address)
        {
            if (Convert.ToUInt16(address) >= 10000)
            {
                bool[] result = master.ReadInputs(Convert.ToUInt16(Convert.ToInt32(address) - 10001), 1);
                return result[0];
            }
            else
            {
                bool[] result = master.ReadCoils(Convert.ToUInt16(Convert.ToInt32(address) - 1), 1);
                return result[0];
            }
        }

        public short ReadInt(string address)
        {
            if (Convert.ToInt32(address) >= 40000)
            {
                ushort[] result = master.ReadHoldingRegisters(Convert.ToUInt16(Convert.ToInt32(address) - 40001), 1);
                return Convert.ToInt16(result[0]);
            }
            else
            {
                ushort[] result = master.ReadInputRegisters(Convert.ToUInt16(Convert.ToInt32(address) - 30001), 1);
                return Convert.ToInt16(result[0]);
            }
        }

        public uint ReadDInt(string address)
        {
            if (Convert.ToInt32(address) >= 40000)
            {
                ushort[] result = master.ReadHoldingRegisters(Convert.ToUInt16(Convert.ToInt32(address) - 40001), 2);
                return Modbus.Utility.ModbusUtility.GetUInt32(result[0], result[1]);
            }
            else
            {
                ushort[] result = master.ReadInputRegisters(Convert.ToUInt16(Convert.ToInt32(address) - 30001), 2);
                return Modbus.Utility.ModbusUtility.GetUInt32(result[0], result[1]);
            }
        }

        public float ReadReal(string address)
        {
            if (Convert.ToInt32(address) >= 40000)
            {
                ushort[] result = master.ReadHoldingRegisters(Convert.ToUInt16(Convert.ToInt32(address) - 40001), 2);
                return Modbus.Utility.ModbusUtility.GetSingle(result[0], result[1]);
            }
            else
            {
                ushort[] result = master.ReadInputRegisters(Convert.ToUInt16(Convert.ToInt32(address) - 30001), 2);
                return Modbus.Utility.ModbusUtility.GetSingle(result[0], result[1]);
            }
        }

        public void WriteBool(string address, bool value)
        {
            master.WriteSingleCoil(Convert.ToUInt16(Convert.ToInt32(address) - 10001), value);
        }

        public void WriteInt(string address, ushort value)
        {
            master.WriteSingleRegister(Convert.ToUInt16(Convert.ToInt32(address) - 40001), value);
        }

        public void WriteDint(string address, int value)
        {
            ushort theValue1 = BitConverter.ToUInt16(BitConverter.GetBytes(value), 0);
            ushort theValue2 = BitConverter.ToUInt16(BitConverter.GetBytes(value), 2);
            master.WriteMultipleRegisters(Convert.ToUInt16(Convert.ToInt32(address) - 40001), new ushort[] { theValue1, theValue2 });
        }

        public void WriteReal(string address, float value)
        {
            ushort theValue1 = BitConverter.ToUInt16(BitConverter.GetBytes(value), 0);
            ushort theValue2 = BitConverter.ToUInt16(BitConverter.GetBytes(value), 2);
            master.WriteMultipleRegisters(Convert.ToUInt16(Convert.ToInt32(address) - 40001), new ushort[] { theValue1, theValue2 });
        }

        public override bool Connect()
        {
            if (tcpClient.Connected)
            {
                return true;
            }
            else
            {
                try
                {
                    tcpClient.Connect(IP, PortNum);
                    return true;
                }
                catch (Exception ex)
                {
                    base.ShowMessage(ex.Message.ToString());
                    return false;
                }
            }
        }

        public override bool DisConnect()
        {
            if (tcpClient.Connected)
            {
                try
                {
                    tcpClient.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    base.ShowMessage(ex.Message.ToString());
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public override bool InitPlc()
        {
            tcpClient = new TcpClient();
            if (master == null)
            {
                try
                {
                    master = ModbusIpMaster.CreateIp(tcpClient);
                    return true;
                }
                catch (Exception ex)
                {
                    base.ShowMessage(ex.Message.ToString());
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public override bool GetStateAndReconnect()
        {
            try
            {
                if (tcpClient.Connected)
                {
                    master.ReadHoldingRegisters(Convert.ToUInt16(this.ConnectTag), 1);
                    return tcpClient.Connected;
                }
                else
                {
                    master.Dispose();
                    tcpClient = new TcpClient();
                    tcpClient.Connect(IP, PortNum);
                    master = ModbusIpMaster.CreateIp(tcpClient);
                    return tcpClient.Connected;
                }
            }
            catch (Exception)
            {
                return tcpClient.Connected;
            }
        }

        public override void PointReadHandle()
        {

            Task.Run(() =>
            {
                while (true)
                {
                    if (State)
                    {
                        scanDeviceManualResetEvent.WaitOne();
                        foreach (var item in base.PointDictionary)
                        {
                            if (item.Value.scan)
                            {
                                try
                                {
                                    switch (item.Value.dataType)
                                    {
                                        //BOOL INT DINT REAL BYTE
                                        case "BOOL"://通过委托的方法来实现对每一个变量读取函数的回调
                                            item.Value.value = ReadBool(item.Value.address);
                                            Console.WriteLine($"{item.Key}:{item.Value.value}");
                                            break;
                                        case "INT":
                                            item.Value.value = ReadInt(item.Value.address);
                                            Console.WriteLine($"{item.Key}:{item.Value.value}");
                                            break;
                                        case "DINT":
                                            item.Value.value = ReadDInt(item.Value.address);
                                            Console.WriteLine($"{item.Key}:{item.Value.value}");
                                            break;
                                        case "REAL":
                                            item.Value.value = ReadReal(item.Value.address);
                                            Console.WriteLine($"{item.Key}:{item.Value.value}");
                                            break;
                                        //case "BYTE":
                                        //    base.readDelegate += (() =>
                                        //    {
                                        //        item.Value.value = ReadByte(item.Value.address);
                                        //    });
                                        default:
                                            break;
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
                    Thread.Sleep(2000);
                }
            });
        }

    }
}
