using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslControls;

namespace ScadaFrame
{
    public class HistoryRecodePoint
    {
        public HistoryRecodePoint(string pointName, bool curveVisible, Color curveColor, Color axisColor, int[] axisVauleZone, bool showAlarmLine, bool showAlarmZone, bool axisVisible,float valueRecode,PointPare pointPare)
        {
            PointName = pointName;
            CurveVisible = curveVisible;
            CurveColor = curveColor;
            AxisColor = axisColor;
            AxisValueZone = axisVauleZone;
            ShowAlarmLine = showAlarmLine;
            ShowAlarmZone = showAlarmZone;
            AxisVisible = axisVisible;
        }

        private string _pointName;
        /// <summary>
        /// 点参数对象
        /// </summary>
        public string PointName
        {
            get { return _pointName; }
            set { _pointName = value; }
        }
        private bool _curveVisible;
        /// <summary>
        /// 曲线是否可见
        /// </summary>
        public bool CurveVisible
        {
            get { return _curveVisible; }
            set { _curveVisible = value; }
        }
        private Color _curveColor;
        /// <summary>
        /// 曲线颜色
        /// </summary>
        public Color CurveColor
        {
            get { return _curveColor; }
            set { _curveColor = value; }
        }
        private Color _axisColor;
        /// <summary>
        /// 数值轴颜色
        /// </summary>
        public Color AxisColor
        {
            get { return _axisColor; }
            set { _axisColor = value; }
        }
        private int[] _axisValueZone;
        /// <summary>
        /// 数值轴范围
        /// </summary>
        public int[] AxisValueZone
        {
            get { return _axisValueZone; }
            set { _axisValueZone = value; }
        }
        private bool _showAlarmLine;
        /// <summary>
        /// 是否显示报警参考线
        /// </summary>
        public bool ShowAlarmLine
        {
            get { return _showAlarmLine; }
            set { _showAlarmLine = value; }
        }
        private bool _showAlarmZone;
        /// <summary>
        /// 是否显示报警区域
        /// </summary>
        public bool ShowAlarmZone
        {
            get { return _showAlarmZone; }
            set { _showAlarmZone = value; }
        }
        private bool _axisVisible;
        /// <summary>
        /// 数值轴是否可见
        /// </summary>
        public bool AxisVisible
        {
            get { return _axisVisible; }
            set { _axisVisible = value; }
        }
        private float[] _valueRecode;
        /// <summary>
        /// 数据组
        /// </summary>
        public float[] ValueRecode
        {
            get { return _valueRecode; }
            set { _valueRecode = value; }
        }
        private PointPare _pointPare;
        /// <summary>
        /// 点的对象
        /// </summary>
        public PointPare PointPare
        {
            get { return _pointPare; }
            set { _pointPare = value; }
        }


        public void addToCurve(HslCurveHistory hslCurveHistory,int curveIndex)
        {
            //hslCurveHistory.ReferenceAxis.
            hslCurveHistory.SetCurve(PointName, curveIndex, ValueRecode, CurveColor, 1f, CurveStyle.Curve, $"{0:F3} {PointName}");
            if (ShowAlarmLine)
            {
                hslCurveHistory.AddLeftAuxiliary(PointPare.hhAlarm, Color.Red, 1f, true);
                hslCurveHistory.AddLeftAuxiliary(PointPare.hhAlarm, Color.Red, 1f, true);
                hslCurveHistory.AddLeftAuxiliary(PointPare.hhAlarm, Color.Red, 1f, true);
                hslCurveHistory.AddLeftAuxiliary(PointPare.hhAlarm, Color.Red, 1f, true);
            }
        }

    }
}
