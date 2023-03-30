using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScadaFrame
{
    public class AlarmRecode
    {
        public AlarmRecode(string number,string name ,string describe,string startTime,string stopTime,string type,string value,string confirm)
        {
            AlarmNumber = number;
            AlarmName = name;
            AlarmDescribe = describe;
            StartTime = startTime;
            StopTime = stopTime;
            AlarmType = type;
            ActualValue = value;
            AlarmConfirm = confirm;
        }
        private StringBuilder _alarmNumber = new StringBuilder();
        public string AlarmNumber
        {
            get { return _alarmNumber.ToString(); }
            set 
            {
                _alarmNumber.Clear();
                _alarmNumber.Append(value);
            }
        }
        private StringBuilder _alarmName = new StringBuilder();
        public string AlarmName
        {
            get { return _alarmName.ToString(); }
            set 
            {
                _alarmName.Clear();
                _alarmName.Append(value);
            }
        }
        private StringBuilder _alarmDescribe = new StringBuilder();
        public string AlarmDescribe
        {
            get { return _alarmDescribe.ToString(); }
            set 
            {
                _alarmDescribe.Clear();
                _alarmDescribe.Append(value);
            }
        }
        private StringBuilder _startTime = new StringBuilder();
        public string StartTime
        {
            get { return _startTime.ToString(); }
            set
            {
                _startTime.Clear();
                _startTime.Append(value);
            }
        }
        private StringBuilder _stopTime = new StringBuilder();
        public string StopTime
        {
            get { return _stopTime.ToString(); }
            set 
            {
                _stopTime.Clear();
                _stopTime.Append(value);
            }
        }
        private StringBuilder _alarmType = new StringBuilder();
        public string AlarmType
        {
            get { return _alarmType.ToString(); }
            set 
            {
                _alarmType.Clear();
                _alarmType.Append(value);
            }
        }
        private StringBuilder _actualValue = new StringBuilder();
        public string ActualValue
        {
            get { return _actualValue.ToString(); }
            set 
            {
                _actualValue.Clear();
                _actualValue.Append(value);
            }
        }
        private StringBuilder _alarmConfirm = new StringBuilder();
        public string AlarmConfirm
        {
            get { return _alarmConfirm.ToString(); }
            set 
            {
                _alarmConfirm.Clear();
                _alarmConfirm.Append(value);
            }
        }

    }
}
