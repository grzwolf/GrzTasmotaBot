
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.tabPageOverview = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.buttonSettings = new System.Windows.Forms.Button();
            this.groupBoxTasmotaSockets = new System.Windows.Forms.GroupBox();
            this.checkBoxShowInTelegram = new System.Windows.Forms.CheckBox();
            this.buttonClearWattage = new System.Windows.Forms.Button();
            this.checkBoxPower = new System.Windows.Forms.CheckBox();
            this.ChartPower = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.labelSocket = new System.Windows.Forms.Label();
            this.buttonSocketStatus = new System.Windows.Forms.Button();
            this.buttonSocketOff = new System.Windows.Forms.Button();
            this.buttonSocketOn = new System.Windows.Forms.Button();
            this.groupBoxLogger = new System.Windows.Forms.GroupBox();
            this.buttonClearLogger = new System.Windows.Forms.Button();
            this.textBoxLogger = new System.Windows.Forms.TextBox();
            this.groupBoxAllTasmotas = new System.Windows.Forms.GroupBox();
            this.checkBoxAutoSearch = new System.Windows.Forms.CheckBox();
            this.comboBoxTasmotaDevices = new System.Windows.Forms.ComboBox();
            this.buttonSearchTasmotas = new System.Windows.Forms.Button();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBarMain = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabelMain = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControlDevices = new System.Windows.Forms.TabControl();
            this.timerCheckTelegramLiveTick = new System.Windows.Forms.Timer(this.components);
            this.timerFakeProgress = new System.Windows.Forms.Timer(this.components);
            this.timerTelegramRestart = new System.Windows.Forms.Timer(this.components);
            this.timerUpdateHosts = new System.Windows.Forms.Timer(this.components);
            this.timerAppStatus = new System.Windows.Forms.Timer(this.components);
            this.toolTipCommon = new System.Windows.Forms.ToolTip(this.components);
            this.tabPageOverview.SuspendLayout();
            this.tableLayoutPanelMain.SuspendLayout();
            this.groupBoxTasmotaSockets.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChartPower)).BeginInit();
            this.groupBoxLogger.SuspendLayout();
            this.groupBoxAllTasmotas.SuspendLayout();
            this.statusStripMain.SuspendLayout();
            this.tabControlDevices.SuspendLayout();
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
            // groupBoxTasmotaSockets
            // 
            this.tableLayoutPanelMain.SetColumnSpan(this.groupBoxTasmotaSockets, 2);
            this.groupBoxTasmotaSockets.Controls.Add(this.checkBoxShowInTelegram);
            this.groupBoxTasmotaSockets.Controls.Add(this.buttonClearWattage);
            this.groupBoxTasmotaSockets.Controls.Add(this.checkBoxPower);
            this.groupBoxTasmotaSockets.Controls.Add(this.ChartPower);
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
            // checkBoxShowInTelegram
            // 
            this.checkBoxShowInTelegram.AutoSize = true;
            this.checkBoxShowInTelegram.Location = new System.Drawing.Point(192, 64);
            this.checkBoxShowInTelegram.Name = "checkBoxShowInTelegram";
            this.checkBoxShowInTelegram.Size = new System.Drawing.Size(70, 17);
            this.checkBoxShowInTelegram.TabIndex = 12;
            this.checkBoxShowInTelegram.Text = "Telegram";
            this.checkBoxShowInTelegram.UseVisualStyleBackColor = true;
            this.checkBoxShowInTelegram.Click += new System.EventHandler(this.checkBoxShowInTelegram_Click);
            // 
            // buttonClearWattage
            // 
            this.buttonClearWattage.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClearWattage.Location = new System.Drawing.Point(208, 40);
            this.buttonClearWattage.Name = "buttonClearWattage";
            this.buttonClearWattage.Size = new System.Drawing.Size(40, 18);
            this.buttonClearWattage.TabIndex = 11;
            this.buttonClearWattage.Text = "clear";
            this.toolTipCommon.SetToolTip(this.buttonClearWattage, "clear all data > last 20");
            this.buttonClearWattage.UseVisualStyleBackColor = true;
            this.buttonClearWattage.Click += new System.EventHandler(this.ClearPower_Click);
            // 
            // checkBoxPower
            // 
            this.checkBoxPower.AutoSize = true;
            this.checkBoxPower.Location = new System.Drawing.Point(192, 24);
            this.checkBoxPower.Name = "checkBoxPower";
            this.checkBoxPower.Size = new System.Drawing.Size(55, 17);
            this.checkBoxPower.TabIndex = 9;
            this.checkBoxPower.Text = "Graph";
            this.checkBoxPower.UseVisualStyleBackColor = true;
            this.checkBoxPower.Click += new System.EventHandler(this.checkPower);
            // 
            // ChartPower
            // 
            this.ChartPower.BorderlineColor = System.Drawing.Color.DimGray;
            this.ChartPower.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.Name = "ChartArea1";
            this.ChartPower.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.ChartPower.Legends.Add(legend1);
            this.ChartPower.Location = new System.Drawing.Point(280, 8);
            this.ChartPower.Name = "ChartPower";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series1.IsVisibleInLegend = false;
            series1.Legend = "Legend1";
            series1.Name = "W";
            this.ChartPower.Series.Add(series1);
            this.ChartPower.Size = new System.Drawing.Size(420, 80);
            this.ChartPower.TabIndex = 8;
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
            this.buttonSocketOff.Location = new System.Drawing.Point(104, 56);
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
            this.buttonClearLogger.Text = "clear logger";
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
            // groupBoxAllTasmotas
            // 
            this.tableLayoutPanelMain.SetColumnSpan(this.groupBoxAllTasmotas, 2);
            this.groupBoxAllTasmotas.Controls.Add(this.checkBoxAutoSearch);
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
            // checkBoxAutoSearch
            // 
            this.checkBoxAutoSearch.AutoSize = true;
            this.checkBoxAutoSearch.Location = new System.Drawing.Point(627, 93);
            this.checkBoxAutoSearch.Margin = new System.Windows.Forms.Padding(0);
            this.checkBoxAutoSearch.Name = "checkBoxAutoSearch";
            this.checkBoxAutoSearch.Size = new System.Drawing.Size(47, 17);
            this.checkBoxAutoSearch.TabIndex = 8;
            this.checkBoxAutoSearch.Text = "auto";
            this.checkBoxAutoSearch.UseVisualStyleBackColor = true;
            this.checkBoxAutoSearch.Click += new System.EventHandler(this.checkBoxAutoSearch_Click);
            // 
            // comboBoxTasmotaDevices
            // 
            this.comboBoxTasmotaDevices.FormattingEnabled = true;
            this.comboBoxTasmotaDevices.Location = new System.Drawing.Point(3, 16);
            this.comboBoxTasmotaDevices.Name = "comboBoxTasmotaDevices";
            this.comboBoxTasmotaDevices.Size = new System.Drawing.Size(623, 21);
            this.comboBoxTasmotaDevices.TabIndex = 7;
            this.comboBoxTasmotaDevices.SelectedIndexChanged += new System.EventHandler(this.comboBoxTasmotaDevices_SelectedIndexChanged);
            // 
            // buttonSearchTasmotas
            // 
            this.buttonSearchTasmotas.Location = new System.Drawing.Point(626, 15);
            this.buttonSearchTasmotas.Name = "buttonSearchTasmotas";
            this.buttonSearchTasmotas.Size = new System.Drawing.Size(75, 72);
            this.buttonSearchTasmotas.TabIndex = 0;
            this.buttonSearchTasmotas.Text = "Search";
            this.buttonSearchTasmotas.UseVisualStyleBackColor = true;
            this.buttonSearchTasmotas.Click += new System.EventHandler(this.buttonSearchTasmotas_Click);
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
            // tabControlDevices
            // 
            this.tabControlDevices.Controls.Add(this.tabPageOverview);
            this.tabControlDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlDevices.Location = new System.Drawing.Point(0, 0);
            this.tabControlDevices.Name = "tabControlDevices";
            this.tabControlDevices.SelectedIndex = 0;
            this.tabControlDevices.Size = new System.Drawing.Size(724, 514);
            this.tabControlDevices.TabIndex = 1;
            this.tabControlDevices.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tabControlDevices_MouseDown);
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
            // timerTelegramRestart
            // 
            this.timerTelegramRestart.Interval = 300000;
            this.timerTelegramRestart.Tick += new System.EventHandler(this.timerTelegramRestart_Tick);
            // 
            // timerUpdateHosts
            // 
            this.timerUpdateHosts.Interval = 30000;
            this.timerUpdateHosts.Tag = "0";
            this.timerUpdateHosts.Tick += new System.EventHandler(this.timerUpdateHosts_Tick);
            // 
            // timerAppStatus
            // 
            this.timerAppStatus.Enabled = true;
            this.timerAppStatus.Interval = 60000;
            this.timerAppStatus.Tick += new System.EventHandler(this.timerAppStatus_Tick);
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
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.groupBoxTasmotaSockets.ResumeLayout(false);
            this.groupBoxTasmotaSockets.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChartPower)).EndInit();
            this.groupBoxLogger.ResumeLayout(false);
            this.groupBoxLogger.PerformLayout();
            this.groupBoxAllTasmotas.ResumeLayout(false);
            this.groupBoxAllTasmotas.PerformLayout();
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.tabControlDevices.ResumeLayout(false);
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
        private System.Windows.Forms.Timer timerTelegramRestart;
        private System.Windows.Forms.Timer timerUpdateHosts;
        private System.Windows.Forms.CheckBox checkBoxAutoSearch;
        private System.Windows.Forms.Timer timerAppStatus;
        private System.Windows.Forms.DataVisualization.Charting.Chart ChartPower;
        private System.Windows.Forms.CheckBox checkBoxPower;
        private System.Windows.Forms.Button buttonClearWattage;
        private System.Windows.Forms.ToolTip toolTipCommon;
        private System.Windows.Forms.CheckBox checkBoxShowInTelegram;
    }
}

