using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication;
using HslCommunication.Profinet.AllenBradley;

namespace ScadaFrame
{
    class ABPLC : CommunicateDevice
    {
        public ABPLC(string ip, byte cpuSlot , string route,bool autoConnect, bool reconnect, int pollTime, string connectTag) : base(autoConnect, reconnect, pollTime, connectTag)
        {
            this.Ip = ip;
            this.CpuSlot = cpuSlot;
            this.Route = route;
        }

        private string _ip;
        public string Ip
        {
            get { return _ip; }
            set { _ip = value; }
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
                    return false;
                }
            }
            else if(!connectState.IsSuccess)
            {
                try
                {
                    connectState = abPLC.ConnectServer();
                    return true;
                }
                catch (Exception ex)
                {
                    base.ShowMessage(ex.Message.ToString());
                    return false;
                }
            }
            return true;
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
