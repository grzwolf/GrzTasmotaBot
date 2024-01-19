
namespace GrzTasmotaBot {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if ( disposing && (components != null) ) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.tabPageOverview = new System.Windows.Forms.TabPage();
            this.groupBoxLogger = new System.Windows.Forms.GroupBox();
            this.buttonClearLogger = new System.Windows.Forms.Button();
            this.textBoxLogger = new System.Windows.Forms.TextBox();
            this.comboBoxTasmotaDevices = new System.Windows.Forms.ComboBox();
            this.groupBoxTasmotaSockets = new System.Windows.Forms.GroupBox();
            this.labelSocket = new System.Windows.Forms.Label();
            this.buttonSocketStatus = new System.Windows.Forms.Button();
            this.buttonSocketOff = new System.Windows.Forms.Button();
            this.buttonSocketOn = new System.Windows.Forms.Button();
            this.buttonSettings = new System.Windows.Forms.Button();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBarMain = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabelMain = new System.Windows.Forms.ToolStripStatusLabel();
            this.buttonSearchTasmotas = new System.Windows.Forms.Button();
            this.tabControlDevices = new System.Windows.Forms.TabControl();
            this.timerCheckTelegramLiveTick = new System.Windows.Forms.Timer(this.components);
            this.timerFakeProgress = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxAllTasmotas = new System.Windows.Forms.GroupBox();
            this.tabPageOverview.SuspendLayout();
            this.groupBoxLogger.SuspendLayout();
            this.groupBoxTasmotaSockets.SuspendLayout();
            this.statusStripMain.SuspendLayout();
            this.tabControlDevices.SuspendLayout();
            this.tableLayoutPanelMain.SuspendLayout();
            this.groupBoxAllTasmotas.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPageOverview
            // 
            this.tabPageOverview.Controls.Add(this.tableLayoutPanelMain);
            this.tabPageOverview.Controls.Add(this.statusStripMain);
            this.tabPageOverview.Location = new System.Drawing.Point(4, 22);
            this.tabPageOverview.Name = "tabPageOverview";
            this.tabPageOverview.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageOverview.Size = new System.Drawing.Size(716, 488);
            this.tabPageOverview.TabIndex = 0;
            this.tabPageOverview.Text = "Overview";
            this.tabPageOverview.UseVisualStyleBackColor = true;
            // 
            // groupBoxLogger
            // 
            this.tableLayoutPanelMain.SetColumnSpan(this.groupBoxLogger, 2);
            this.groupBoxLogger.Controls.Add(this.buttonClearLogger);
            this.groupBoxLogger.Controls.Add(this.textBoxLogger);
            this.groupBoxLogger.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxLogger.Location = new System.Drawing.Point(3, 263);
            this.groupBoxLogger.Name = "groupBoxLogger";
            this.groupBoxLogger.Size = new System.Drawing.Size(704, 194);
            this.groupBoxLogger.TabIndex = 10;
            this.groupBoxLogger.TabStop = false;
            this.groupBoxLogger.Text = "Messages Logger";
            // 
            // buttonClearLogger
            // 
            this.buttonClearLogger.Location = new System.Drawing.Point(8, 160);
            this.buttonClearLogger.Name = "buttonClearLogger";
            this.buttonClearLogger.Size = new System.Drawing.Size(75, 23);
            this.buttonClearLogger.TabIndex = 10;
            this.buttonClearLogger.Text = "clear";
            this.buttonClearLogger.UseVisualStyleBackColor = true;
            this.buttonClearLogger.Click += new System.EventHandler(this.buttonClearLogger_Click);
            // 
            // textBoxLogger
            // 
            this.textBoxLogger.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxLogger.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxLogger.Location = new System.Drawing.Point(3, 16);
            this.textBoxLogger.Multiline = true;
            this.textBoxLogger.Name = "textBoxLogger";
            this.textBoxLogger.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxLogger.Size = new System.Drawing.Size(698, 136);
            this.textBoxLogger.TabIndex = 9;
            this.textBoxLogger.WordWrap = false;
            // 
            // comboBoxTasmotaDevices
            // 
            this.comboBoxTasmotaDevices.Dock = System.Windows.Forms.DockStyle.Top;
            this.comboBoxTasmotaDevices.FormattingEnabled = true;
            this.comboBoxTasmotaDevices.Location = new System.Drawing.Point(3, 16);
            this.comboBoxTasmotaDevices.Name = "comboBoxTasmotaDevices";
            this.comboBoxTasmotaDevices.Size = new System.Drawing.Size(623, 21);
            this.comboBoxTasmotaDevices.TabIndex = 7;
            this.comboBoxTasmotaDevices.SelectedIndexChanged += new System.EventHandler(this.comboBoxTasmotaDevices_SelectedIndexChanged);
            // 
            // groupBoxTasmotaSockets
            // 
            this.tableLayoutPanelMain.SetColumnSpan(this.groupBoxTasmotaSockets, 2);
            this.groupBoxTasmotaSockets.Controls.Add(this.labelSocket);
            this.groupBoxTasmotaSockets.Controls.Add(this.buttonSocketStatus);
            this.groupBoxTasmotaSockets.Controls.Add(this.buttonSocketOff);
            this.groupBoxTasmotaSockets.Controls.Add(this.buttonSocketOn);
            this.groupBoxTasmotaSockets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxTasmotaSockets.Location = new System.Drawing.Point(3, 168);
            this.groupBoxTasmotaSockets.Name = "groupBoxTasmotaSockets";
            this.groupBoxTasmotaSockets.Size = new System.Drawing.Size(704, 89);
            this.groupBoxTasmotaSockets.TabIndex = 8;
            this.groupBoxTasmotaSockets.TabStop = false;
            this.groupBoxTasmotaSockets.Text = "Tasmota socket type devices";
            // 
            // labelSocket
            // 
            this.labelSocket.AutoSize = true;
            this.labelSocket.Location = new System.Drawing.Point(8, 64);
            this.labelSocket.Name = "labelSocket";
            this.labelSocket.Size = new System.Drawing.Size(51, 13);
            this.labelSocket.TabIndex = 5;
            this.labelSocket.Text = "unknown";
            // 
            // buttonSocketStatus
            // 
            this.buttonSocketStatus.Location = new System.Drawing.Point(8, 24);
            this.buttonSocketStatus.Name = "buttonSocketStatus";
            this.buttonSocketStatus.Size = new System.Drawing.Size(75, 23);
            this.buttonSocketStatus.TabIndex = 6;
            this.buttonSocketStatus.Text = "status";
            this.buttonSocketStatus.UseVisualStyleBackColor = true;
            this.buttonSocketStatus.Click += new System.EventHandler(this.buttonSocketStatus_Click);
            // 
            // buttonSocketOff
            // 
            this.buttonSocketOff.Location = new System.Drawing.Point(192, 24);
            this.buttonSocketOff.Name = "buttonSocketOff";
            this.buttonSocketOff.Size = new System.Drawing.Size(75, 23);
            this.buttonSocketOff.TabIndex = 4;
            this.buttonSocketOff.Text = "Off";
            this.buttonSocketOff.UseVisualStyleBackColor = true;
            this.buttonSocketOff.Click += new System.EventHandler(this.buttonSocketOff_Click);
            // 
            // buttonSocketOn
            // 
            this.buttonSocketOn.Location = new System.Drawing.Point(104, 24);
            this.buttonSocketOn.Name = "buttonSocketOn";
            this.buttonSocketOn.Size = new System.Drawing.Size(75, 23);
            this.buttonSocketOn.TabIndex = 3;
            this.buttonSocketOn.Text = "On";
            this.buttonSocketOn.UseVisualStyleBackColor = true;
            this.buttonSocketOn.Click += new System.EventHandler(this.buttonSocketOn_Click);
            // 
            // buttonSettings
            // 
            this.buttonSettings.Location = new System.Drawing.Point(3, 3);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Size = new System.Drawing.Size(75, 33);
            this.buttonSettings.TabIndex = 2;
            this.buttonSettings.Text = "Settings";
            this.buttonSettings.UseVisualStyleBackColor = true;
            this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
            // 
            // statusStripMain
            // 
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBarMain,
            this.toolStripStatusLabelMain});
            this.statusStripMain.Location = new System.Drawing.Point(3, 463);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Size = new System.Drawing.Size(710, 22);
            this.statusStripMain.TabIndex = 1;
            this.statusStripMain.Text = "statusStrip1";
            // 
            // toolStripProgressBarMain
            // 
            this.toolStripProgressBarMain.Name = "toolStripProgressBarMain";
            this.toolStripProgressBarMain.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStripStatusLabelMain
            // 
            this.toolStripStatusLabelMain.Name = "toolStripStatusLabelMain";
            this.toolStripStatusLabelMain.Size = new System.Drawing.Size(42, 17);
            this.toolStripStatusLabelMain.Text = "Status:";
            // 
            // buttonSearchTasmotas
            // 
            this.buttonSearchTasmotas.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonSearchTasmotas.Location = new System.Drawing.Point(626, 16);
            this.buttonSearchTasmotas.Name = "buttonSearchTasmotas";
            this.buttonSearchTasmotas.Size = new System.Drawing.Size(75, 95);
            this.buttonSearchTasmotas.TabIndex = 0;
            this.buttonSearchTasmotas.Text = "Search";
            this.buttonSearchTasmotas.UseVisualStyleBackColor = true;
            this.buttonSearchTasmotas.Click += new System.EventHandler(this.buttonSearchTasmotas_Click);
            // 
            // tabControlDevices
            // 
            this.tabControlDevices.Controls.Add(this.tabPageOverview);
            this.tabControlDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlDevices.Location = new System.Drawing.Point(0, 0);
            this.tabControlDevices.Name = "tabControlDevices";
            this.tabControlDevices.SelectedIndex = 0;
            this.tabControlDevices.Size = new System.Drawing.Size(724, 514);
            this.tabControlDevices.TabIndex = 1;
            // 
            // timerCheckTelegramLiveTick
            // 
            this.timerCheckTelegramLiveTick.Interval = 30000;
            this.timerCheckTelegramLiveTick.Tick += new System.EventHandler(this.timerCheckTelegramLiveTick_Tick);
            // 
            // timerFakeProgress
            // 
            this.timerFakeProgress.Interval = 250;
            this.timerFakeProgress.Tick += new System.EventHandler(this.timerFakeProgress_Tick);
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 2;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelMain.Controls.Add(this.buttonSettings, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxTasmotaSockets, 0, 2);
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxLogger, 0, 3);
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxAllTasmotas, 0, 1);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 4;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(710, 460);
            this.tableLayoutPanelMain.TabIndex = 11;
            // 
            // groupBoxAllTasmotas
            // 
            this.tableLayoutPanelMain.SetColumnSpan(this.groupBoxAllTasmotas, 2);
            this.groupBoxAllTasmotas.Controls.Add(this.comboBoxTasmotaDevices);
            this.groupBoxAllTasmotas.Controls.Add(this.buttonSearchTasmotas);
            this.groupBoxAllTasmotas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxAllTasmotas.Location = new System.Drawing.Point(3, 48);
            this.groupBoxAllTasmotas.Name = "groupBoxAllTasmotas";
            this.groupBoxAllTasmotas.Size = new System.Drawing.Size(704, 114);
            this.groupBoxAllTasmotas.TabIndex = 3;
            this.groupBoxAllTasmotas.TabStop = false;
            this.groupBoxAllTasmotas.Text = "All Tasmota Devices";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 514);
            this.Controls.Add(this.tabControlDevices);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(740, 553);
            this.MinimumSize = new System.Drawing.Size(740, 553);
            this.Name = "MainForm";
            this.Text = "GrzTasmotaBot - control Tasmota gadgets via Telegram bot";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.tabPageOverview.ResumeLayout(false);
            this.tabPageOverview.PerformLayout();
            this.groupBoxLogger.ResumeLayout(false);
            this.groupBoxLogger.PerformLayout();
            this.groupBoxTasmotaSockets.ResumeLayout(false);
            this.groupBoxTasmotaSockets.PerformLayout();
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.tabControlDevices.ResumeLayout(false);
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.groupBoxAllTasmotas.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPageOverview;
        private System.Windows.Forms.Button buttonSearchTasmotas;
        private System.Windows.Forms.TabControl tabControlDevices;
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelMain;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBarMain;
        private System.Windows.Forms.Button buttonSettings;
        private System.Windows.Forms.Timer timerCheckTelegramLiveTick;
        private System.Windows.Forms.Timer timerFakeProgress;
        private System.Windows.Forms.Button buttonSocketStatus;
        private System.Windows.Forms.Label labelSocket;
        private System.Windows.Forms.Button buttonSocketOff;
        private System.Windows.Forms.Button buttonSocketOn;
        private System.Windows.Forms.ComboBox comboBoxTasmotaDevices;
        private System.Windows.Forms.GroupBox groupBoxTasmotaSockets;
        private System.Windows.Forms.GroupBox groupBoxLogger;
        private System.Windows.Forms.Button buttonClearLogger;
        private System.Windows.Forms.TextBox textBoxLogger;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.GroupBox groupBoxAllTasmotas;
    }
}

