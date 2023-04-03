using Modbus.Device;
using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace ScadaFrame
{
    public class ModbusRtu : CommunicateDevice
    {
        public ModbusRtu(int comName, int baudRate, int dataBits, int stopBits, int parity, bool autoConnect, bool reconnect, int pollTime, byte slaveAddress, string connectTag) : base(autoConnect, reconnect, pollTime, connectTag)
        {
            PortName = comName;
            BaudRate = baudRate;
            DataBits = dataBits;
            StopBits = stopBits;
            Parity = parity;
            SlaveAddress = slaveAddress;
        }
        private SerialPort serialPort;
        private ModbusSerialMaster modbusSerialMaster;

        private int _portName;
        public int PortName
        {
            get { return _portName; }
            set { _portName = value; }
        }

        private int _baudRate;
        public int BaudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        }

        private int _dataBits;
        public int DataBits
        {
            get { return _dataBits; }
            set { _dataBits = value; }
        }

        private StopBits _stopBits;
        public int StopBits
        {
            get { return (int)_stopBits; }
            set { _stopBits = (StopBits)value; }
        }

        private Parity _parity;
        public int Parity
        {
            get { return (int)_parity; }
            set { _parity = (Parity)value; }
        }

        private byte _slaveAddress;
        public byte SlaveAddress
        {
            get { return _slaveAddress; }
            set { _slaveAddress = value; }
        }


        private bool _state;
        public override bool State
        {
            get { return serialPort.IsOpen; }
        }

        public bool ReadBool(byte slaveAddress, string address)
        {
            if (Convert.ToUInt16(address) >= 10000)
            {
                bool[] result = modbusSerialMaster.ReadCoils(slaveAddress, Convert.ToUInt16(Convert.ToInt32(address) - 10001), 1);
                return result[0];
            }
            else
            {
                bool[] result = modbusSerialMaster.ReadInputs(slaveAddress, Convert.ToUInt16(Convert.ToInt32(address) - 1), 1);
                return result[0];
            }
        }

        public short ReadInt(byte slaveAddress, string address)
        {
            if (Convert.ToInt32(address) >= 40000)
            {
                ushort[] result = modbusSerialMaster.ReadHoldingRegisters(slaveAddress, Convert.ToUInt16(Convert.ToInt32(address) - 40001), 1);
                return Convert.ToInt16(result[0]);
            }
            else
            {
                ushort[] result = modbusSerialMaster.ReadInputRegisters(slaveAddress, Convert.ToUInt16(Convert.ToInt32(address) - 30001), 1);
                return Convert.ToInt16(result[0]);
            }
        }

        public uint ReadDInt(byte slaveAddress, string address)
        {
            if (Convert.ToInt32(address) >= 40000)
            {
                ushort[] result = modbusSerialMaster.ReadHoldingRegisters(slaveAddress, Convert.ToUInt16(Convert.ToInt32(address) - 40001), 2);
                return Modbus.Utility.ModbusUtility.GetUInt32(result[0], result[1]);
            }
            else
            {
                ushort[] result = modbusSerialMaster.ReadInputRegisters(slaveAddress, Convert.ToUInt16(Convert.ToInt32(address) - 30001), 2);
                return Modbus.Utility.ModbusUtility.GetUInt32(result[0], result[1]);
            }
        }

        public float ReadReal(byte slaveAddress, string address)
        {
            if (Convert.ToInt32(address) >= 40000)
            {
                ushort[] result = modbusSerialMaster.ReadHoldingRegisters(slaveAddress, Convert.ToUInt16(Convert.ToInt32(address) - 40001), 2);
                return Modbus.Utility.ModbusUtility.GetSingle(result[0], result[1]);
            }
            else
            {
                ushort[] result = modbusSerialMaster.ReadInputRegisters(slaveAddress, Convert.ToUInt16(Convert.ToInt32(address) - 30001), 2);
                return Modbus.Utility.ModbusUtility.GetSingle(result[0], result[1]);
            }
        }

        public void WriteBool(byte slaveAddress, string address, bool value)
        {
            modbusSerialMaster.WriteSingleCoil(slaveAddress, Convert.ToUInt16(Convert.ToInt32(address) - 10001), value);
        }

        public void WriteInt(byte slaveAddress, string address, ushort value)
        {
            modbusSerialMaster.WriteSingleRegister(slaveAddress, Convert.ToUInt16(Convert.ToInt32(address) - 40001), value);
        }

        public void WriteDint(byte slaveAddress, string address, int value)
        {
            ushort theValue1 = BitConverter.ToUInt16(BitConverter.GetBytes(value), 0);
            ushort theValue2 = BitConverter.ToUInt16(BitConverter.GetBytes(value), 2);
            modbusSerialMaster.WriteMultipleRegisters(slaveAddress, Convert.ToUInt16(Convert.ToInt32(address) - 40001), new ushort[] { theValue1, theValue2 });
        }

        public void WriteReal(byte slaveAddress, string address, float value)
        {
            ushort theValue1 = BitConverter.ToUInt16(BitConverter.GetBytes(value), 0);
            ushort theValue2 = BitConverter.ToUInt16(BitConverter.GetBytes(value), 2);
            modbusSerialMaster.WriteMultipleRegisters(slaveAddress, Convert.ToUInt16(Convert.ToInt32(address) - 40001), new ushort[] { theValue1, theValue2 });
        }

        public override bool Connect()
        {
            if (serialPort.IsOpen)
            {
                return true;
            }
            else
            {
                try
                {
                    serialPort.Open();
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
            if (!serialPort.IsOpen)
            {
                return true;
            }
            else
            {
                try
                {
                    serialPort.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    base.ShowMessage(ex.Message.ToString());
                    return false;
                }
            }
        }

        public override bool InitPlc()
        {
            serialPort = new SerialPort();
            serialPort.PortName = "COM" + this.PortName.ToString();
            serialPort.BaudRate = this.BaudRate;
            serialPort.DataBits = this.DataBits;
            serialPort.StopBits = this._stopBits;
            serialPort.Parity = this._parity;
            modbusSerialMaster = ModbusSerialMaster.CreateRtu(serialPort);
            serialPort.ReadTimeout = 1000;
            modbusSerialMaster.Transport.Retries = 1;
            if (modbusSerialMaster != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool GetStateAndReconnect()
        {
            try
            {
                modbusSerialMaster.ReadHoldingRegisters(SlaveAddress, Convert.ToUInt16(ConnectTag), (ushort)1);
                return true;
            }
            catch (Exception ex)
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
                                            item.Value.value = ReadBool(SlaveAddress, item.Value.address);
                                            break;
                                        case "INT":
                                            item.Value.value = ReadInt(SlaveAddress, item.Value.address);
                                            break;
                                        case "DINT":
                                            item.Value.value = ReadDInt(SlaveAddress, item.Value.address);
                                            break;
                                        case "REAL":
                                            item.Value.value = ReadReal(SlaveAddress, item.Value.address);
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
