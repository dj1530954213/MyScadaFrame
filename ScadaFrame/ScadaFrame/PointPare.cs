using System;

namespace ScadaFrame
{

    public enum AlarmActualState
    {
        //对于此枚举进行如下定义   stateChange  stateNochange说明是由报警和报警恢复的改变    alarmTypeChange   alarmTypeNoChange说明是在处于报警的基础上报警类型之间的改变
        stateChange,
        stateNochange,
        alarmTypeChange,
        alarmTypeNoChange
    }
    public class PointPare
    {
        public string communcationDevice;
        public string address;
        public string dataType;
        public object value;
        public string deviceName;
        public bool scan;
        public bool alarmEnable;
        public bool history;
        public float hAlarm;
        public float lAlarm;
        public float hhAlarm;
        public float llAlarm;
        public string unit;
        public string describe;
        public float deadZone;
        public bool valueChanged = false;
        public object valueRecode;
        /// <summary>
        /// 按照值本身的类型来获取它的值
        /// </summary>
        /// <typeparam name="T">填入typeof(PointDictionary["点名"].value)</typeparam>
        /// <returns></returns>
        public T GetValue<T>()
        {
            return (T)value;
        }
        public bool alarmMark = false;
        public string alarmType = "";
        public AlarmActualState alarmActualState;
        /// <summary>
        /// 检查是否报警
        /// </summary>
        /// <returns>报警等级是否改变</returns>
        public AlarmActualState CheckAlarm()
        {
            if (dataType == "BOOL")//首先判断数据类型
            {
                if (Convert.ToBoolean(value))
                {
                    return AlarmingStateArbiter(ref alarmMark, ref alarmType,"状态量报警");
                }
                else
                {
                    alarmMark = false;
                    alarmActualState = (alarmType == "" ? AlarmActualState.stateNochange : AlarmActualState.stateChange);
                    alarmType = "";
                    return alarmActualState;
                }
            }
            else
            {
                float i = Convert.ToSingle(value);//将除BOOL类型外的所有数据都转换为float类型进行比较
                if (i <= llAlarm)
                {
                    return AlarmingStateArbiter(ref alarmMark, ref alarmType, "低低报");

                }
                else if (i <= lAlarm)
                {
                    return AlarmingStateArbiter(ref alarmMark, ref alarmType, "低报");
                }
                else if (i >= hAlarm)
                {
                    return AlarmingStateArbiter(ref alarmMark, ref alarmType, "高报");
                }
                else if (i >= hhAlarm)
                {
                    return AlarmingStateArbiter(ref alarmMark, ref alarmType, "高高报");
                }
                else
                {
                    alarmMark = false;
                    alarmActualState = (alarmType == "" ? AlarmActualState.stateNochange : AlarmActualState.stateChange);
                    alarmType = "";
                    return alarmActualState;
                }
            }

        }
        /// <summary>
        /// 处理正在报警的报警类型判断
        /// </summary>
        /// <param name="isAlarm">传入报警标志位</param>
        /// <param name="valueType">传入报警类型</param>
        /// <param name="comparison">传入需要比较的报警类型字符串</param>
        /// <returns></returns>
        private AlarmActualState AlarmingStateArbiter(ref bool isAlarm,ref string valueType,string comparison)
        {
            isAlarm = true;//由于此处为报警之后的报警状态判断，所以需要先将报警状态设置为正在报警
            if (valueType == comparison)//如果本次扫描与上次扫描报警类型一致说则返回   正在报警，类型未变
            {
                alarmActualState = AlarmActualState.alarmTypeNoChange;
            }
            else if (valueType == "")//如果上次扫描结果为空说明由未报警转换为报警状态，   返回开始报警
            {
                alarmActualState = AlarmActualState.stateChange;
            }
            else//剩下的扫描状态都是返回    正在报警，类型发生改变
            {
                alarmActualState = AlarmActualState.alarmTypeChange;
            }
            valueType = comparison;//更新此次的报警类型
            return alarmActualState;//返回报警类型的结果
        }
    }
}
