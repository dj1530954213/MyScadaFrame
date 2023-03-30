
namespace ScadaFrame
{
    partial class FrmLogin1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)//如果把该窗体导入别的项目中此处报错由于是局部类所以需要将前端代码的命名空间修改为当前项目的命名空间还需要修改后端代码的命名空间
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.skinPanel1 = new CCWin.SkinControl.SkinPanel();
            this.skinLine1 = new CCWin.SkinControl.SkinLine();
            this.skinPanel4 = new CCWin.SkinControl.SkinPanel();
            this.uiTextBoxPassWord = new Sunny.UI.UITextBox();
            this.skinPanel3 = new CCWin.SkinControl.SkinPanel();
            this.uiTextBoxUserName = new Sunny.UI.UITextBox();
            this.btn_Login = new CCWin.SkinControl.SkinButton();
            this.btn_Close = new CCWin.SkinControl.SkinButton();
            this.skinLabel2 = new CCWin.SkinControl.SkinLabel();
            this.skinPanel5 = new CCWin.SkinControl.SkinPanel();
            this.skinLabel3 = new CCWin.SkinControl.SkinLabel();
            this.uiStyleManager1 = new Sunny.UI.UIStyleManager(this.components);
            this.skinPanel1.SuspendLayout();
            this.skinPanel4.SuspendLayout();
            this.skinPanel3.SuspendLayout();
            this.skinPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // skinPanel1
            // 
            this.skinPanel1.BackColor = System.Drawing.Color.Gray;
            this.skinPanel1.Controls.Add(this.skinLine1);
            this.skinPanel1.Controls.Add(this.skinPanel4);
            this.skinPanel1.Controls.Add(this.skinPanel3);
            this.skinPanel1.Controls.Add(this.btn_Login);
            this.skinPanel1.Controls.Add(this.btn_Close);
            this.skinPanel1.Controls.Add(this.skinLabel2);
            this.skinPanel1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skinPanel1.DownBack = null;
            this.skinPanel1.Location = new System.Drawing.Point(0, 0);
            this.skinPanel1.MouseBack = null;
            this.skinPanel1.Name = "skinPanel1";
            this.skinPanel1.NormlBack = null;
            this.skinPanel1.Radius = 10;
            this.skinPanel1.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.skinPanel1.Size = new System.Drawing.Size(346, 267);
            this.skinPanel1.TabIndex = 13;
            // 
            // skinLine1
            // 
            this.skinLine1.BackColor = System.Drawing.Color.Transparent;
            this.skinLine1.Dock = System.Windows.Forms.DockStyle.Top;
            this.skinLine1.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(57)))), ((int)(((byte)(166)))));
            this.skinLine1.LineHeight = 1;
            this.skinLine1.Location = new System.Drawing.Point(0, 43);
            this.skinLine1.Name = "skinLine1";
            this.skinLine1.Size = new System.Drawing.Size(346, 10);
            this.skinLine1.TabIndex = 19;
            this.skinLine1.Text = "skinLine1";
            // 
            // skinPanel4
            // 
            this.skinPanel4.BackColor = System.Drawing.Color.Transparent;
            this.skinPanel4.Controls.Add(this.uiTextBoxPassWord);
            this.skinPanel4.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinPanel4.DownBack = null;
            this.skinPanel4.Location = new System.Drawing.Point(48, 122);
            this.skinPanel4.MouseBack = null;
            this.skinPanel4.Name = "skinPanel4";
            this.skinPanel4.NormlBack = null;
            this.skinPanel4.Size = new System.Drawing.Size(249, 33);
            this.skinPanel4.TabIndex = 103;
            // 
            // uiTextBoxPassWord
            // 
            this.uiTextBoxPassWord.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBoxPassWord.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTextBoxPassWord.Icon = global::ScadaFrame.Properties.Resources.key;
            this.uiTextBoxPassWord.Location = new System.Drawing.Point(0, 0);
            this.uiTextBoxPassWord.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBoxPassWord.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBoxPassWord.Name = "uiTextBoxPassWord";
            this.uiTextBoxPassWord.PasswordChar = '*';
            this.uiTextBoxPassWord.Radius = 20;
            this.uiTextBoxPassWord.ShowText = false;
            this.uiTextBoxPassWord.Size = new System.Drawing.Size(249, 33);
            this.uiTextBoxPassWord.TabIndex = 1;
            this.uiTextBoxPassWord.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBoxPassWord.Watermark = "密码";
            this.uiTextBoxPassWord.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // skinPanel3
            // 
            this.skinPanel3.BackColor = System.Drawing.Color.Transparent;
            this.skinPanel3.Controls.Add(this.uiTextBoxUserName);
            this.skinPanel3.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinPanel3.DownBack = null;
            this.skinPanel3.Location = new System.Drawing.Point(48, 74);
            this.skinPanel3.MouseBack = null;
            this.skinPanel3.Name = "skinPanel3";
            this.skinPanel3.NormlBack = null;
            this.skinPanel3.Radius = 10;
            this.skinPanel3.Size = new System.Drawing.Size(249, 33);
            this.skinPanel3.TabIndex = 102;
            // 
            // uiTextBoxUserName
            // 
            this.uiTextBoxUserName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBoxUserName.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTextBoxUserName.Icon = global::ScadaFrame.Properties.Resources.name;
            this.uiTextBoxUserName.Location = new System.Drawing.Point(0, 0);
            this.uiTextBoxUserName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBoxUserName.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBoxUserName.Name = "uiTextBoxUserName";
            this.uiTextBoxUserName.Radius = 20;
            this.uiTextBoxUserName.ShowText = false;
            this.uiTextBoxUserName.Size = new System.Drawing.Size(249, 33);
            this.uiTextBoxUserName.TabIndex = 0;
            this.uiTextBoxUserName.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBoxUserName.Watermark = "用户名";
            this.uiTextBoxUserName.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // btn_Login
            // 
            this.btn_Login.BackColor = System.Drawing.Color.Transparent;
            this.btn_Login.BaseColor = System.Drawing.Color.Teal;
            this.btn_Login.BorderColor = System.Drawing.Color.Teal;
            this.btn_Login.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btn_Login.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Login.DownBack = null;
            this.btn_Login.DownBaseColor = System.Drawing.Color.LightSeaGreen;
            this.btn_Login.ForeColor = System.Drawing.Color.White;
            this.btn_Login.IsDrawBorder = false;
            this.btn_Login.IsDrawGlass = false;
            this.btn_Login.Location = new System.Drawing.Point(48, 198);
            this.btn_Login.MouseBack = null;
            this.btn_Login.MouseBaseColor = System.Drawing.Color.LightSeaGreen;
            this.btn_Login.Name = "btn_Login";
            this.btn_Login.NormlBack = null;
            this.btn_Login.Palace = true;
            this.btn_Login.Radius = 10;
            this.btn_Login.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btn_Login.Size = new System.Drawing.Size(90, 32);
            this.btn_Login.TabIndex = 10;
            this.btn_Login.TabStop = false;
            this.btn_Login.Text = "登录";
            this.btn_Login.UseVisualStyleBackColor = false;
            this.btn_Login.Click += new System.EventHandler(this.btn_Login_Click);
            // 
            // btn_Close
            // 
            this.btn_Close.BackColor = System.Drawing.Color.Transparent;
            this.btn_Close.BaseColor = System.Drawing.Color.Firebrick;
            this.btn_Close.BorderColor = System.Drawing.Color.Teal;
            this.btn_Close.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btn_Close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Close.DownBack = null;
            this.btn_Close.DownBaseColor = System.Drawing.Color.IndianRed;
            this.btn_Close.ForeColor = System.Drawing.Color.White;
            this.btn_Close.IsDrawBorder = false;
            this.btn_Close.IsDrawGlass = false;
            this.btn_Close.Location = new System.Drawing.Point(211, 198);
            this.btn_Close.MouseBack = null;
            this.btn_Close.MouseBaseColor = System.Drawing.Color.IndianRed;
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.NormlBack = null;
            this.btn_Close.Palace = true;
            this.btn_Close.Radius = 10;
            this.btn_Close.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btn_Close.Size = new System.Drawing.Size(86, 32);
            this.btn_Close.TabIndex = 10;
            this.btn_Close.TabStop = false;
            this.btn_Close.Text = "退出";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // skinLabel2
            // 
            this.skinLabel2.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel2.BorderColor = System.Drawing.Color.White;
            this.skinLabel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.skinLabel2.Font = new System.Drawing.Font("黑体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel2.Location = new System.Drawing.Point(0, 0);
            this.skinLabel2.Name = "skinLabel2";
            this.skinLabel2.Size = new System.Drawing.Size(346, 43);
            this.skinLabel2.TabIndex = 50;
            this.skinLabel2.Text = "油气回收系统";
            this.skinLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // skinPanel5
            // 
            this.skinPanel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(166)))));
            this.skinPanel5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.skinPanel5.Controls.Add(this.skinPanel1);
            this.skinPanel5.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinPanel5.DownBack = null;
            this.skinPanel5.Location = new System.Drawing.Point(568, 162);
            this.skinPanel5.MouseBack = null;
            this.skinPanel5.Name = "skinPanel5";
            this.skinPanel5.NormlBack = null;
            this.skinPanel5.Radius = 10;
            this.skinPanel5.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.skinPanel5.Size = new System.Drawing.Size(346, 267);
            this.skinPanel5.TabIndex = 18;
            // 
            // skinLabel3
            // 
            this.skinLabel3.ArtTextStyle = CCWin.SkinControl.ArtTextStyle.None;
            this.skinLabel3.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel3.BorderColor = System.Drawing.Color.White;
            this.skinLabel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.skinLabel3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel3.ForeColor = System.Drawing.Color.White;
            this.skinLabel3.Location = new System.Drawing.Point(4, 546);
            this.skinLabel3.Name = "skinLabel3";
            this.skinLabel3.Size = new System.Drawing.Size(953, 27);
            this.skinLabel3.TabIndex = 101;
            this.skinLabel3.Text = "邮箱：1530954213@QQ.COM";
            this.skinLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FrmLogin1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(57)))), ((int)(((byte)(166)))));
            this.BackgroundImage = global::ScadaFrame.Properties.Resources.pic_5tu_big_2019010111726369377;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelButton = this.btn_Close;
            this.CanResize = false;
            this.CaptionFont = new System.Drawing.Font("Arial", 12F);
            this.CaptionHeight = 32;
            this.ClientSize = new System.Drawing.Size(961, 577);
            this.ControlBox = false;
            this.Controls.Add(this.skinLabel3);
            this.Controls.Add(this.skinPanel5);
            this.EffectBack = System.Drawing.Color.Transparent;
            this.EffectCaption = CCWin.TitleType.Title;
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmLogin1";
            this.Radius = 10;
            this.ShadowWidth = 10;
            this.ShowBorder = false;
            this.ShowDrawIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.TitleCenter = true;
            this.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(179)))), ((int)(((byte)(255)))));
            this.TitleOffset = new System.Drawing.Point(20, 0);
            this.Load += new System.EventHandler(this.FrmLogin1_Load);
            this.Shown += new System.EventHandler(this.FrmLogin1_Shown);
            this.skinPanel1.ResumeLayout(false);
            this.skinPanel4.ResumeLayout(false);
            this.skinPanel3.ResumeLayout(false);
            this.skinPanel5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private CCWin.SkinControl.SkinPanel skinPanel1;
        private CCWin.SkinControl.SkinButton btn_Login;
        private CCWin.SkinControl.SkinButton btn_Close;
        private CCWin.SkinControl.SkinPanel skinPanel4;
        private CCWin.SkinControl.SkinPanel skinPanel3;
        private CCWin.SkinControl.SkinPanel skinPanel5;
        private CCWin.SkinControl.SkinLine skinLine1;
        private CCWin.SkinControl.SkinLabel skinLabel2;
        private CCWin.SkinControl.SkinLabel skinLabel3;
        private Sunny.UI.UITextBox uiTextBoxPassWord;
        private Sunny.UI.UITextBox uiTextBoxUserName;
        private Sunny.UI.UIStyleManager uiStyleManager1;
    }
}