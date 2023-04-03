
namespace ABCommunicationDemo
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.BT_Connect = new System.Windows.Forms.Button();
            this.textBoxIpAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPortName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxSlotNum = new System.Windows.Forms.TextBox();
            this.BT_Disconnect = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxRoute = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // BT_Connect
            // 
            this.BT_Connect.Location = new System.Drawing.Point(1083, 53);
            this.BT_Connect.Name = "BT_Connect";
            this.BT_Connect.Size = new System.Drawing.Size(127, 38);
            this.BT_Connect.TabIndex = 0;
            this.BT_Connect.Text = "连接";
            this.BT_Connect.UseVisualStyleBackColor = true;
            this.BT_Connect.Click += new System.EventHandler(this.BT_Connect_Click);
            // 
            // textBoxIpAddress
            // 
            this.textBoxIpAddress.Location = new System.Drawing.Point(111, 67);
            this.textBoxIpAddress.Multiline = true;
            this.textBoxIpAddress.Name = "textBoxIpAddress";
            this.textBoxIpAddress.Size = new System.Drawing.Size(149, 25);
            this.textBoxIpAddress.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "IP地址";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(284, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "端口号";
            // 
            // textBoxPortName
            // 
            this.textBoxPortName.Location = new System.Drawing.Point(342, 66);
            this.textBoxPortName.Name = "textBoxPortName";
            this.textBoxPortName.Size = new System.Drawing.Size(166, 25);
            this.textBoxPortName.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(540, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "槽号";
            // 
            // textBoxSlotNum
            // 
            this.textBoxSlotNum.Location = new System.Drawing.Point(596, 66);
            this.textBoxSlotNum.Name = "textBoxSlotNum";
            this.textBoxSlotNum.Size = new System.Drawing.Size(166, 25);
            this.textBoxSlotNum.TabIndex = 5;
            // 
            // BT_Disconnect
            // 
            this.BT_Disconnect.Location = new System.Drawing.Point(1216, 53);
            this.BT_Disconnect.Name = "BT_Disconnect";
            this.BT_Disconnect.Size = new System.Drawing.Size(127, 38);
            this.BT_Disconnect.TabIndex = 7;
            this.BT_Disconnect.Text = "断开连接";
            this.BT_Disconnect.UseVisualStyleBackColor = true;
            this.BT_Disconnect.Click += new System.EventHandler(this.BT_Disconnect_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(784, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "CIP路径";
            // 
            // textBoxRoute
            // 
            this.textBoxRoute.Location = new System.Drawing.Point(849, 66);
            this.textBoxRoute.Name = "textBoxRoute";
            this.textBoxRoute.Size = new System.Drawing.Size(166, 25);
            this.textBoxRoute.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1395, 736);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxRoute);
            this.Controls.Add(this.BT_Disconnect);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxSlotNum);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxPortName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxIpAddress);
            this.Controls.Add(this.BT_Connect);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BT_Connect;
        private System.Windows.Forms.TextBox textBoxIpAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxPortName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSlotNum;
        private System.Windows.Forms.Button BT_Disconnect;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxRoute;
    }
}

