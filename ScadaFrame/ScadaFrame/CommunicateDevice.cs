using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace ScadaFrame
{
    public abstract class CommunicateDevice
    {
        public CommunicateDevice(bool autoConnect, bool reconnect, int pollTime, string connectTag)
        {
            this.AutoConnect = autoConnect;
            this.Reconnect = reconnect;
            this.PollTime = pollTime;
            this.ConnectTag = connectTag;
        }
        public abstract bool State { get; }
        private bool _autoConnect;
        public bool AutoConnect
        {
            get { return _autoConnect; }
            set { _autoConnect = value; }
        }

        private bool _reconnect;
        public bool Reconnect
        {
            get { return _reconnect; }
            set { _reconnect = value; }
        }

        private int _pollTime;
        public int PollTime
        {
            get { return _pollTime; }
            set { _pollTime = value; }
        }

        private string _connectTag;
        public string ConnectTag
        {
            get { return _connectTag; }
            set { _connectTag = value; }
        }

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
        /// <summary>
        /// 点位历史记录处理
        /// </summary>
        /// <param name="sqlhandle"></param>
        public void HistoryRecordHandle(sqlhandle sqlhandle)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Clear();
            stringBuilder.Append("INSERT into historyRecode VALUES ");
            foreach (var item in PointDictionary)
            {
                if (item.Value.history && item.Value.valueChanged)//启用历史数据采集功能且历史值状态改变
                {
                    stringBuilder.Append($"('{item.Key}',{item.Value.value},'{DateTime.Now.ToString()}'),");//进行sql语句拼接
                    item.Value.valueChanged = false;//首先将标志位复位
                }
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            if (stringBuilder.ToString().Length <= "INSERT into historyRecode VALUES ".Length) return;
            IDataReader dataReader = sqlhandle.read(stringBuilder.ToString());//点位查询完成之后
            dataReader.Close();
            dataReader.Dispose();
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
        private void AlarmDBInsert(sqlhandle sqlhandler, PointPare pointPare, string pointName)
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
        private void AlarmDBUpdate(sqlhandle sqlhandle, PointPare pointPare, string pointName)
        {
            string sql = $"update AlarmRecode set stopTime = '{DateTime.Now.ToString()}' where pointName = '{pointName}'";
            sqlhandle.excute(sql);
            actualAlarmRefresh.Invoke();
        }
        /// <summary>
        /// 对实际值与记录值进行死区校验，并返回此次实际值
        /// </summary>
        /// <typeparam name="T">读取点位的数据类型</typeparam>
        /// <param name="valueRecode">pointPare中的数值记录</param>
        /// <param name="realValue">实时读取的值</param>
        /// <param name="deadZone">死区范围</param>
        /// <param name="valueChange">值是否改变的指示</param>
        /// <returns></returns>
        public object deadZoneCheck<T>(ref object valueRecode, T realValue, float deadZone, ref bool valueChange,bool enableHistoryRecode)
        {
            if (!enableHistoryRecode)
            {
                return realValue;
            }
            if (Math.Abs(Convert.ToSingle(valueRecode) - Convert.ToSingle(realValue)) > deadZone)//当前值与记录值差的绝对值大于死区设定值就将数据改变标志位置为1
            {
                valueChange = true;//标志位置为1
                valueRecode = realValue;//并进行记录值更新
                return realValue;
            }
            return realValue;//若绝对值小于死区设定值则不进行记录值得更新
        }
        public Dictionary<string, PointPare> PointDictionary = new Dictionary<string, PointPare>();

    }
}
