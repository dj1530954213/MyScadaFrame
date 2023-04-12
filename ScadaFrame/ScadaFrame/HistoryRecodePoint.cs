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
        public HistoryRecodePoint(string pointName, bool curveVisible, Color curveColor,bool showAlarmLine, float[] valueRecode,PointPare pointPare)
        {
            PointName = pointName;
            CurveVisible = curveVisible;
            CurveColor = curveColor;
            ShowAlarmLine = showAlarmLine;
            ValueRecode = valueRecode;
            PointPare = pointPare;
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
        private bool _showAlarmLine;
        /// <summary>
        /// 是否显示报警参考线
        /// </summary>
        public bool ShowAlarmLine
        {
            get { return _showAlarmLine; }
            set { _showAlarmLine = value; }
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
        /// 点位对象
        /// </summary>
        public PointPare PointPare
        {
            get { return _pointPare; }
            set { _pointPare = value; }
        }
        public void addToCurve(ref HslCurveHistory hslCurveHistory)
        {
            //hslCurveHistory.ReferenceAxis.
            hslCurveHistory.SetLeftCurve(PointName, ValueRecode, CurveColor, CurveStyle.Curve, $"{0:F3} {PointPare.unit}");//添加或更改曲线
            if (ShowAlarmLine)//增加报警辨识辅助线
            {
                hslCurveHistory.AddLeftAuxiliary(PointPare.hhAlarm, Color.Red, 1f, true);
                hslCurveHistory.AddLeftAuxiliary(PointPare.hAlarm, Color.YellowGreen, 1f, true);
                hslCurveHistory.AddLeftAuxiliary(PointPare.lAlarm, Color.YellowGreen, 1f, true);
                hslCurveHistory.AddLeftAuxiliary(PointPare.llAlarm, Color.Red, 1f, true);
            }
            else//删除报警辨识辅助线
            {
                hslCurveHistory.RemoveAuxiliary(PointPare.hhAlarm);
                hslCurveHistory.RemoveAuxiliary(PointPare.hAlarm);
                hslCurveHistory.RemoveAuxiliary(PointPare.lAlarm);
                hslCurveHistory.RemoveAuxiliary(PointPare.llAlarm);
            }
            hslCurveHistory.SetCurveVisible(PointName,CurveVisible);//设置曲线是否可见
        }
    }
}
