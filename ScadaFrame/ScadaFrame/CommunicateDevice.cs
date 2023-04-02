using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using S7.Net;

namespace ScadaFrame
{
    public abstract class CommunicateDevice
    {
        public CommunicateDevice(bool autoConnect, bool reconnect, int pollTime,string connectTag)
        {
            this.AutoConnect = autoConnect;
            this.Reconnect = reconnect;
            this.PollTime = pollTime;
            this.ConnectTag = connectTag;
        }
        public abstract bool State { get; }
        public abstract string ConnectTag { get; set; }
        public abstract bool AutoConnect { get; set; }
        public abstract bool Reconnect { get; set; }
        public abstract int PollTime { get; set; }
        public bool isScan = true;
        public ManualResetEvent scanDeviceManualResetEvent = new ManualResetEvent(true);
        public event Action<string> errorMessageEvent;//只需要在外部订阅此事件就可以进行异常消息通知
        //public event Action<PointPare> alarmLevel;
        public event Action actualAlarmRefresh;//数据库改变事件
        /// <summary>
        /// 初始化PLC连接对象
        /// </summary>
        /// <returns></returns>
        public abstract bool InitPlc();
        public abstract bool Connect();
        public abstract bool DisConnect();
        /// <summary>
        /// 错误信息显示，并调用委托
        /// </summary>
        /// <param name="message"></param>
        public void ShowMessage(string message)
        {
            if (errorMessageEvent != null)
            {
                errorMessageEvent.Invoke(message);
            }
        }
        /// <summary>
        /// 生成新的通讯设备
        /// </summary>
        /// <param name="driveType">设备类型</param>
        /// <param name="communicatePare">通讯参数</param>
        /// <returns></returns>
        public static CommunicateDevice CreatDevices(string driveType, List<object> communicatePare)
        {
            switch (driveType)
            {
                /*Siemens AB ModbusRtu ModbusTCP OPCClient OPCServer*/
                case "Siemens":
                    return new SiemensPLC((string)communicatePare[0], (string)communicatePare[1], (short)communicatePare[2], (short)communicatePare[3], (bool)communicatePare[4], (bool)communicatePare[5], (int)communicatePare[6], (string)communicatePare[7]);
                //case "AB":
                //    break;
                case "ModbusRtu":
                    return new ModbusRtu((int)communicatePare[0], (int)communicatePare[1], (int)communicatePare[2], (int)communicatePare[3], (int)communicatePare[4], (bool)communicatePare[5], (bool)communicatePare[6], (int)communicatePare[7], (byte)communicatePare[8], (string)communicatePare[9]);
                case "ModbusTCP":
                    return new ModbusTCP((string)communicatePare[0], (int)communicatePare[1], (int)communicatePare[2], (bool)communicatePare[3], (bool)communicatePare[4], (int)communicatePare[5], (string)communicatePare[6]);
                //case "OPCClient":
                //    break;
                //case "OPCServer":
                //    break;
                default:
                    return null;
            }
        }
        /// <summary>
        /// 获取连接状态并进行重连处理
        /// </summary>
        /// <returns></returns>
        public abstract bool GetStateAndReconnect();
        /// <summary>
        /// 点位轮询读取处理
        /// </summary>
        public abstract void PointReadHandle();
        public void HistoryRecord()
        {
            
        }
        /// <summary>
        /// alarm处理函数供form内循环调用
        /// </summary>
        /// <param name="sqlhandle"></param>
        public void AlarmHandle(sqlhandle sqlhandle)
        {
            scanDeviceManualResetEvent.WaitOne();
            foreach (var item in PointDictionary)
            {
                if (item.Value.alarmEnable && !(item.Value.value is null))//如果报警启用  值存在
                {
                    if (item.Value.alarmMark)//处于报警状态中
                    {
                        switch (item.Value.CheckAlarm())
                        {
                            case AlarmActualState.stateChange:
                                //写入报警恢复时间
                                AlarmDBUpdate(sqlhandle, item.Value, item.Key);
                                break;
                            case AlarmActualState.stateNochange:
                                //不进行任何处理
                                break;
                            case AlarmActualState.alarmTypeChange:
                                //添加新的报警记录
                                AlarmDBInsert(sqlhandle, item.Value, item.Key);
                                break;
                            case AlarmActualState.alarmTypeNoChange:
                                //不进行任何处理
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (item.Value.CheckAlarm())
                        {
                            case AlarmActualState.stateChange:
                                //添加新的报警记录
                                AlarmDBInsert(sqlhandle, item.Value, item.Key);
                                break;
                            case AlarmActualState.stateNochange:
                                //不做任何处理
                                break;
                            default:
                                break;
                        }
                    }
                }
                
            }
        }
        private void AlarmDBInsert(sqlhandle sqlhandler, PointPare pointPare,string pointName)
        {
            string sql = "";
            if (pointPare.dataType == "BOOL")
            {
                float value = Convert.ToSingle(pointPare.value);
                //sql = $"insert into AlarmRecode values('{pointName}','{pointPare.describe}','{DateTime.Now.ToString()}','','{pointPare.alarmType}',{value},'dj')";
                sql = $"insert into AlarmRecode (pointName,describe,startTime,alarmType,actualValue,operation,confirm) values('{pointName}','{pointPare.describe}','{DateTime.Now.ToString()}','{pointPare.alarmType}',{value},'dj','未确认')";
                sqlhandler.excute(sql);
                actualAlarmRefresh.Invoke();
                return;
            }
            sql = $"insert into AlarmRecode values('{pointName}','{pointPare.describe}','{DateTime.Now.ToString()}',NULL,'{pointPare.alarmType}',{pointPare.value},'dj','未确认')";
            sqlhandler.excute(sql);
            actualAlarmRefresh.Invoke();
        }
        private void AlarmDBUpdate(sqlhandle sqlhandle, PointPare pointPare,string pointName)
        {
            string sql = $"update AlarmRecode set stopTime = '{DateTime.Now.ToString()}' where pointName = '{pointName}'";
            sqlhandle.excute(sql);
            actualAlarmRefresh.Invoke();
        }
        public  Dictionary<string, PointPare> PointDictionary = new Dictionary<string, PointPare>();
        
    }
}
