using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;                  
using System.Net.Http;                                
using CefSharp.WinForms;                 // Chromium based webbrowser via NuGet
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using TeleSharp.Entities;                // Telegram
using TeleSharp.Entities.SendEntities;   // Telegram
using GrzTools;                          // tools
using System.Reflection;
using System.Drawing.Imaging;
using System.IO;

namespace GrzTasmotaBot {
    public partial class MainForm : Form {

        // app settings
        public static AppSettings Settings = new AppSettings();

        // Tasmota vars
        List<host> TasmotaHostsList = new List<host>();               // list of hosts in the local subnet
        List<string> TasmotaSocketsList = new List<string>();         // list of device names, which identify Tasmota socket devices  
        string TasmotaDeviceFilter = TasmotaDeviceType.UNKNOWN;

        // data 
        List<string> gadgetDataList = new List<string>();

        // Telegram bot
        TeleSharp.TeleSharp _Bot = null;                              // the bot
        static String TELEGRAM_STANDARD_COMMANDS = "Valid commands, pick one:\n\n/hello  /help  /time  /location";
        String TelegramFinalCommands = TELEGRAM_STANDARD_COMMANDS;    // common string to send after receiving /help
        String TelegramDevicesCommands = "";                          // device specific string to send after receiving /help
        List<String> TelegramDeviceCommandList = new List<string>();  // list of device specific commands to receive, used in RX message parser
        DateTime _connectionLiveTick = DateTime.Now;                  // error handling
        int _telegramOnErrorCount = 0;                                // error handling
        int _telegramLiveTickErrorCount = 0;                          // error handling 
        bool _runPing = false;                                        // error handling
        int _telegramRestartCounter = 0;                              // error handling

        public MainForm() {

            InitializeComponent();

            // add "about entry" etc. to app's system menu
            SetupSystemMenu();

            // get settings from INI
            AppSettings.IniFile ini = new AppSettings.IniFile(System.Windows.Forms.Application.ExecutablePath + ".ini");

            // fill settings property grid
            Settings.fillPropertyGridFromIni();

            //
            // !! TBD currently only sockets TBD !!
            //
            // load supported Tasmota device type name list
            string socketTypesFile = System.Windows.Forms.Application.StartupPath + "\\TasmotaSockets.txt";
            if ( System.IO.File.Exists(socketTypesFile) ) {
                TasmotaSocketsList = System.IO.File.ReadAllText(socketTypesFile)
                    .Split(new[] { "\r\n" }, StringSplitOptions.None)
                    .ToList();
                TasmotaDeviceFilter = TasmotaDeviceType.SOCKET;
                this.Text += " - supported device types = " + TasmotaDeviceType.SOCKET;
            } else {
                MessageBox.Show("File with device definitions is missing: " + socketTypesFile, "Error");
            }

            // have TasmotaSocket methods in an array, used to pick a matching method from a received Telegram command
            List<MethodInfo> methods = new List<MethodInfo>();
            MethodInfo[] allMethods = typeof(TasmotaSocket).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            for ( int i = allMethods.Length - 1; i >= 0; i-- ) {
                string methodName = allMethods[i].Name;
                // discard non TASMOTA_SOCKET_COMMANDS like ToString etc
                foreach ( var cmd in TasmotaSocket.TASMOTA_SOCKET_COMMANDS ) {
                    string cmdMod = cmd.Substring(1);
                    if ( methodName.EndsWith(cmdMod) ) {
                        methods.Add(allMethods[i]);
                        break;
                    }
                }
            }
            TasmotaSocket.methods = methods.ToArray();

            // get previous session host list from INI
            TasmotaHostsList = Settings.getHostsListFromPropertyGrid();
        }

        // app 1st time shown
        async private void MainForm_Shown(object sender, EventArgs e) {
            // form size, location, etc
            updateAppPropertiesFromSettings();
            // show Tasmota hosts
            await UpdateHostsOnUI();
            // comboBoxTasmotaDevices_SelectedIndexChanged(..) takes care about, whether the device belongs to the supported TasmotaSocketsList
            if ( TasmotaHostsList.Count > 0 ) {
                this.comboBoxTasmotaDevices.SelectedIndex = 0;
            }
            // first app status
            this.timerAppStatus_Tick(null, null);
        }

        // auto search Tasmota devices from UI
        private void checkBoxAutoSearch_Click(object sender, EventArgs e) {
            // manual click overrides Settings.HostsUpdateInterval just temporarily 
            if ( this.checkBoxAutoSearch.Checked ) {
                if ( Settings.HostsUpdateInterval > 0 ) {
                    this.checkBoxAutoSearch.Text = "auto " + Settings.HostsUpdateInterval.ToString() + "s";
                    this.timerUpdateHosts.Interval = Settings.HostsUpdateInterval * 1000;
                    this.timerUpdateHosts.Start();
                }
            } else {
                this.timerUpdateHosts.Stop();
            }
        }

        // obvious
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            // no matter what, even if it was not started
            this.timerUpdateHosts.Stop();
            // current app props
            updateSettingsFromAppProperties();
            // shall INI get updated with the current session's TasmotaHostsList                
            bool updateHostsList = IsUpdateSettingsHostsListDue();
            // update property grid with 
            if ( updateHostsList ) {
                Settings.setPropertyGridToHostsList(TasmotaHostsList);
            }
            // write settings to INI
            Settings.writePropertyGridToIni(updateHostsList);
            // if app live cycle comes here, there was no app crash, write such info to INI for next startup log
            AppSettings.IniFile ini = new AppSettings.IniFile(System.Windows.Forms.Application.ExecutablePath + ".ini");
            ini.IniWriteValue("GrzTasmotaBot", "AppCrash", "False");
            Logger.logTextLnU(DateTime.Now, "app closed by user");
        }

        // show "about" in system menu
        const int WM_SYSCOMMAND = 0x112;
        [DllImport("user32.dll")]
        private static extern int GetSystemMenu(int hwnd, int bRevert);
        [DllImport("user32.dll")]
        private static extern int AppendMenu(int hMenu, int Flagsw, int IDNewItem, string lpNewItem);
        private void SetupSystemMenu() {
            // get handle to app system menu
            int menu = GetSystemMenu(this.Handle.ToInt32(), 0);
            // add a separator
            AppendMenu(menu, 0xA00, 0, null);
            // add items with unique message ID
            AppendMenu(menu, 0, 1235, "Send 'test' to 1st whitelist entry");
            AppendMenu(menu, 0, 1234, "About GrzTasmotaBot");
        }
        protected override void WndProc(ref System.Windows.Forms.Message m) {
            // WM_SYSCOMMAND is 0x112
            if ( m.Msg == WM_SYSCOMMAND ) {
                // Telegram test message
                if ( m.WParam.ToInt32() == 1235 ) {
                    // send a test message
                    if ( Settings.UseTelegramBot && _Bot != null && Settings.TelegramWhitelist.Count > 0 ) {
                        string chatid = Settings.TelegramWhitelist[0].Split(',')[1];
                        _Bot.SendMessage(new SendMessageParams {
                            ChatId = chatid,
                            Text = "test"
                        });
                        this.Invoke(new Action(() => {
                            this.textBoxLogger.AppendText(DateTime.Now.ToString() + " TX ' test ' to: " + chatid + "\r\n");
                        }));
                    } else {
                        MessageBox.Show("Telegram bot or whitelist.", "Missing");
                    }
                }
                // show About box: check for added menu item's message ID
                if ( m.WParam.ToInt32() == 1234 ) {
                    // show About box here...
                    AboutBox dlg = new AboutBox();
                    dlg.ShowDialog();
                    dlg.Dispose();
                }
            }

            // it is essential to call the base behavior
            base.WndProc(ref m);
        }

        // clear Telegram messages logger
        private void buttonClearLogger_Click(object sender, EventArgs e) {
            this.textBoxLogger.Text = "";
        }

        // Tasmota sockets UI controls
        private void buttonSocketOn_Click(object sender, EventArgs e) {
            TasmotaSocket.SetOn(TasmotaHostsList[this.comboBoxTasmotaDevices.SelectedIndex].hostip);
            this.labelSocket.Text = TasmotaSocket.GetStatus(TasmotaHostsList[this.comboBoxTasmotaDevices.SelectedIndex].hostip);
        }
        private void buttonSocketOff_Click(object sender, EventArgs e) {
            TasmotaSocket.SetOff(TasmotaHostsList[this.comboBoxTasmotaDevices.SelectedIndex].hostip);
            this.labelSocket.Text = TasmotaSocket.GetStatus(TasmotaHostsList[this.comboBoxTasmotaDevices.SelectedIndex].hostip);
        }
        private void buttonSocketStatus_Click(object sender, EventArgs e) {
            this.labelSocket.Text = TasmotaSocket.GetStatus(TasmotaHostsList[this.comboBoxTasmotaDevices.SelectedIndex].hostip);
        }
        private void updatePower() {
            // get current power value
            string wattageStr = TasmotaSocket.GetPower(TasmotaHostsList[this.comboBoxTasmotaDevices.SelectedIndex].hostip);
            this.labelPower.Text = wattageStr + " Watt";
            if ( this.checkBoxPower.Checked ) {
                // update chart
                this.ChartPower.Series["W"].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(this.ChartPower.Series["W"].Points.Count, Int32.Parse(wattageStr)));
                // update data file
                string gadgetDataFileName = TasmotaHostsList[this.comboBoxTasmotaDevices.SelectedIndex].name + ".data";
                System.IO.File.AppendAllText(gadgetDataFileName, wattageStr + ",");
            }
        }
        private void checkPower(object sender, EventArgs e) {
            if ( this.checkBoxPower.Checked ) {
                // chart starts clean  
                this.ChartPower.Series["W"].Points.Clear();
                this.ChartPower.ChartAreas[0].AxisX.Minimum = 0;
                this.ChartPower.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
                this.ChartPower.ChartAreas[0].AxisX.MajorTickMark.Enabled = false;
                this.ChartPower.ChartAreas[0].AxisX.MinorTickMark.Enabled = false;
                this.ChartPower.ChartAreas[0].AxisX.Interval = -1;
                this.ChartPower.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 6.0F, System.Drawing.FontStyle.Regular);
                // use historical wattage data if existing
                gadgetDataList.Clear();
                string gadgetDataFileName = TasmotaHostsList[this.comboBoxTasmotaDevices.SelectedIndex].name + ".data";
                if ( System.IO.File.Exists(gadgetDataFileName) ) {
                    gadgetDataList = System.IO.File.ReadAllText(gadgetDataFileName)
                        .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }
                // insert historical data into chart
                foreach ( string data in gadgetDataList ) {
                    this.ChartPower.Series["W"].Points.Add(
                        new System.Windows.Forms.DataVisualization.Charting.DataPoint(
                            this.ChartPower.Series["W"].Points.Count, Int32.Parse(data)
                        )
                    );
                }
                // show most current power value
                updatePower();
            }
        }
        // from UI delete historical data, but last 20
        private void ClearPower_Click(object sender, EventArgs e) {
            if ( MessageBox.Show("You sure?", "Delete historical data", MessageBoxButtons.YesNo) == DialogResult.Yes ) {
                ClearPowerHistory();
            }
        }
        void ClearPowerHistory() {
            // clear data list 
            gadgetDataList.Clear();
            // fill data list from data file
            string gadgetDataFileName = TasmotaHostsList[this.comboBoxTasmotaDevices.SelectedIndex].name + ".data";
            if ( System.IO.File.Exists(gadgetDataFileName) ) {
                gadgetDataList = System.IO.File.ReadAllText(gadgetDataFileName)
                    .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            }
            // leave last 20 data in list untouched
            gadgetDataList.RemoveRange(0, Math.Min(Math.Max(0, gadgetDataList.Count - 20), gadgetDataList.Count));
            // delete data file
            System.IO.File.Delete(gadgetDataFileName);
            // chart start clean
            this.ChartPower.Series[0].Points.Clear();
            this.ChartPower.ChartAreas[0].AxisX.Minimum = 0;
            // insert historical data into chart and refill data file
            foreach ( string data in gadgetDataList ) {
                this.ChartPower.Series["W"].Points.Add(
                    new System.Windows.Forms.DataVisualization.Charting.DataPoint(
                        this.ChartPower.Series["W"].Points.Count, Int32.Parse(data)
                    )
                );
                System.IO.File.AppendAllText(gadgetDataFileName, data + ",");
            }
            // first update
            updatePower();
        }

        // Tasmota device was selected
        async private void comboBoxTasmotaDevices_SelectedIndexChanged(object sender, EventArgs e) {
            // disable device specific controls
            this.groupBoxTasmotaSockets.Enabled = false;
            // reset Tasmota device status
            this.labelSocket.Text = "unknown";
            // device complies to device filter setting
            if ( TasmotaHostsList[this.comboBoxTasmotaDevices.SelectedIndex].type == TasmotaDeviceFilter ) {
                // device is supposed to be pingable
                if ( TasmotaHostsList[this.comboBoxTasmotaDevices.SelectedIndex].pingable ) {
                    this.toolStripStatusLabelMain.Text = "";
                    // make pingable device really sure, it might got disconnected after last refresh
                    List<host> list = new List<host> { new host(TasmotaHostsList[this.comboBoxTasmotaDevices.SelectedIndex].hostip, "") };
                    list = await RefreshIpAddressesStatus(list);
                    // now it should be clear, if device is pingable
                    if ( list[0].pingable ) {
                        // enable device specific controls
                        this.groupBoxTasmotaSockets.Enabled = true;
                        // get device status
                        this.labelSocket.Text = TasmotaSocket.GetStatus(TasmotaHostsList[this.comboBoxTasmotaDevices.SelectedIndex].hostip);
                    }
                } else {
                    this.toolStripStatusLabelMain.Text = "Tasmota device '" + TasmotaHostsList[this.comboBoxTasmotaDevices.SelectedIndex].name + "' not available.";
                }
            }
            // ping check
            if ( TasmotaHostsList[this.comboBoxTasmotaDevices.SelectedIndex].pingable ) {
                // only update power graph, if gadget is online - otherwise UI hangs
                if ( this.checkBoxPower.Checked ) {
                    // simulate click on power checkbox
                    checkPower(null, null);
                } else {
                    // show current power value
                    updatePower();
                }
            } else {
                // clear not pingable gadget chart 
                this.ChartPower.Series[0].Points.Clear();
                this.ChartPower.ChartAreas[0].AxisX.Minimum = 0;
            }
        }

        // periodically check Tasmota hosts
        private async void timerUpdateHosts_Tick(object sender, EventArgs e) {
            // timer Tag carries the number of refreshs
            this.timerUpdateHosts.Tag = Convert.ToInt32(Convert.ToInt32(timerUpdateHosts.Tag) + 1);
            if ( Convert.ToInt32(this.timerUpdateHosts.Tag.ToString()) >= 10 ) {
                // execute full search every 10th refresh
                this.buttonSearchTasmotas.PerformClick();
                this.timerUpdateHosts.Tag = Convert.ToInt32(0);
            } else {
                // just pingable status refresh
                this.toolStripStatusLabelMain.Text = "refreshing";
                // refreshes combobox with Tasmota devices
                await UpdateHostsOnUI();
                // fallback check, just in case the index is not valid: should not happen at all
                if ( this.comboBoxTasmotaDevices.SelectedIndex == -1 ) {
                    this.comboBoxTasmotaDevices.SelectedIndex = 0;
                }
            }
        }

        // button search for Tasmota devices
        async void buttonSearchTasmotas_Click(object sender, EventArgs e) {
            // get ready
            if ( this.buttonSearchTasmotas.Text == "- wait -" ) {
                return;
            }
            this.toolStripProgressBarMain.Value = 0;
            this.toolStripProgressBarMain.Maximum = 100;
            this.timerFakeProgress.Start();
            this.toolStripStatusLabelMain.Text = "Status: scanning local network IP addresses";
            String buttonText = this.buttonSearchTasmotas.Text;
            this.buttonSearchTasmotas.Text = "- wait -";
            this.buttonSearchTasmotas.Enabled = false;
            this.labelSocket.Text = "unknown";
            this.timerUpdateHosts.Stop();

            // obtain PC's own IP as a base for the search in the local network
            string ipThis = this.IpThisPC();
            if ( ipThis.Length < 7 ) {
                this.buttonSearchTasmotas.Text = buttonText;
                if ( Settings.HostsUpdateInterval > 0 && this.checkBoxAutoSearch.Checked ) {
                    this.timerUpdateHosts.Interval = Settings.HostsUpdateInterval * 1000;
                    this.timerUpdateHosts.Start();
                }
                MessageBox.Show("IP address " + ipThis + " is wrong, cannot continue.", "Error");
                this.labelSocket.Text = "Error: IP address " + ipThis + " is wrong, cannot continue.";
                return;
            }

            // builds a hostList of all pingable IP Addresses in the searched network
            List<host> hostList = await this.SearchActiveIpAddresses(ipThis);

            // get http GET responses from pingable IP addresses 
            this.toolStripStatusLabelMain.Text = "Status: " + hostList.Count.ToString() + " hosts found, now scanning for Tasmota devices";
            hostList = await GetHttpGETResponses(hostList);

            // check http GET responses for Tasmota content and only keep Tasmota gadgets
            hostList = GetTasmotaContentHosts(hostList);

            // update status bar info
            this.toolStripStatusLabelMain.Text = "Status: found " + hostList.Count.ToString() + " Tasmota devices";

            // add tabs according to found Tasmota devices
            List<host> hostListMod = UpdateTasmotaDeviceListAndBrowsertabs(hostList);

            // merge hostListClean into TasmotaHostsList ?
            if ( AreHostListsDifferent(TasmotaHostsList, hostListMod) ) {
                foreach (var host in hostListMod ) {
                    bool found = false;
                    for ( int i = 0; i < TasmotaHostsList.Count; i++ ) {
                        if ( host.name == TasmotaHostsList[i].name && host.hostip == TasmotaHostsList[i].hostip ) {
                            found = true;
                            // a found item is definitely pingable
                            TasmotaHostsList[i].pingable = true;
                            break;
                        }
                    }
                    // means, hostListMod has more items then TasmotaHostsList: add it + set pingable
                    if ( !found ) {
                        TasmotaHostsList.Add(host);
                        TasmotaHostsList[TasmotaHostsList.Count - 1].pingable = true;
                    }
                }
            } else {
                // if both list do not deviate from each other, then all hosts are pingable
                for ( int i = 0; i < TasmotaHostsList.Count; i++ ) {
                    TasmotaHostsList[i].pingable = true;
                }
            }

            // clear Telegram device specific commands 
            TelegramDeviceCommandList.Clear();

            // clear combo
            int lastNdx = this.comboBoxTasmotaDevices.SelectedIndex;
            this.comboBoxTasmotaDevices.Items.Clear();
            // update combobox with Tasmota devices
            foreach ( var host in TasmotaHostsList ) {
                // update combobox
                this.comboBoxTasmotaDevices.Items.Add(host.name + " - " + host.hostip + " - " + host.type + " - " + (host.pingable ? "online" : "<offline>"));
                // build a Tasmota device specific commands list for each host to be later used in Telgram's receiver parser
                if ( host.type == TasmotaDeviceFilter ) {
                    TelegramDeviceCommandList.AddRange(Tools.GetBasicSocketCommands(host.teleName, TasmotaSocket.TASMOTA_SOCKET_COMMANDS));
                }
            }
            // comboBoxTasmotaDevices_SelectedIndexChanged(..) takes care about, whether the device belongs to the supported TasmotaSocketsList
            this.comboBoxTasmotaDevices.SelectedIndex = lastNdx;

            // build the final "/help command" response string, showing Telegram commands to the remote user
            TelegramFinalCommands = TELEGRAM_STANDARD_COMMANDS;
            if ( TasmotaHostsList.Count > 0 ) {
                TelegramDevicesCommands = Tools.FormatTelegramDecicesCommands(TelegramDeviceCommandList, TasmotaSocket.TASMOTA_SOCKET_COMMANDS.Length);
                TelegramFinalCommands = TELEGRAM_STANDARD_COMMANDS + TelegramDevicesCommands;
            }

            // done
            this.buttonSearchTasmotas.Text = buttonText;
            this.buttonSearchTasmotas.Enabled = true;
            this.timerFakeProgress.Stop();
            this.toolStripProgressBarMain.Value = 100;
            if ( Settings.HostsUpdateInterval > 0 && this.checkBoxAutoSearch.Checked ) {
                this.timerUpdateHosts.Interval = Settings.HostsUpdateInterval * 1000;
                this.timerUpdateHosts.Start();
            }
        }

        // app status timer
        private void timerAppStatus_Tick(object sender, EventArgs e) {
            // once per hour or at app start
            if ( sender == null || DateTime.Now.Minute % 60 == 0 && DateTime.Now.Second < 31 ) {
                var pingableCount = TasmotaHostsList.Where(item => item != null && item.pingable).Count();
                Logger.logTextLnU(DateTime.Now, String.Format("App status: Tasmota hosts count={0}\tpingable={1}\tbot alive={2}", TasmotaHostsList.Count, pingableCount, (_Bot != null)));
            }
        }

        // fake progress while scanning network
        private void timerFakeProgress_Tick(object sender, EventArgs e) {
            this.Invoke(new Action(() => {
                if ( this.toolStripProgressBarMain.Value < this.toolStripProgressBarMain.Maximum ) {
                    this.toolStripProgressBarMain.Value++;
                } else {
                    this.toolStripProgressBarMain.Value = 0;
                }
            }));
        }

        // ---------------------------------------------------------------------------------------------------------------------------------------------

        // a host in a network
        public class host {
            public string hostip;   // host IP address
            public string GETstr;   // host's http GET response string
            public string name;     // Tasmota name as to be found in http GET string between: ... <title>whatever_name - Main Menu</title> ...
            public string type;     // Tasmota device type name --> TasmotaDeviceType
            public string teleName; // Telegram Tasmota name deviates from device Tasmota name, not allowed: a)  " "  b) "#" 
            public bool   pingable; // a device might temprarily not pingable
            public host(string ip, string GETstr) {
                this.hostip = ip;
                this.GETstr = GETstr;
                this.pingable = false;
            }
        }

        // Tasmota device type name
        public static class TasmotaDeviceType {
            public const string
                UNKNOWN = "unknown",
                SOCKET  = "socket";
        }

        // Tasmota TasmotaDeviceType.SOCKET methods to be used in UI
        class TasmotaSocket {
            // literal socket type commands
            public static string[] TASMOTA_SOCKET_COMMANDS = { "_SetOn", "_SetOff", "_GetStatus", "_GetPower", "_GetPowerHistory", "_ClearPowerHistory", "_TogglePowerMonitor" };
            // class TasmotaSocket methods array
            public static MethodInfo[] methods = { null };
            // implemented methods to control a socket device
            public static string SetOn(String ipString) {
                return GetHttpCommandResponse("http://" + ipString + "/cm?cmnd=Power on");
            }
            public static string SetOff(String ipString) {
                return GetHttpCommandResponse("http://" + ipString + "/cm?cmnd=Power off");
            }
            public static string GetStatus(String ipString, bool withWattage = false) {
                var retVal = "";
                string response = GetHttpCommandResponse("http://" + ipString + "/cm?cmnd=status");
                if ( response.IndexOf("\"Power\":1") != -1 ) {
                    string wattage = GetPower(ipString);
                    retVal = "Power = ON";
                    if ( withWattage ) {
                        retVal += ", consumption = " + wattage + "W";
                    }
                } else {
                    if ( response.IndexOf("\"Power\":0") != -1 ) {
                        retVal = "Power = off";
                    } else {
                        retVal = "Power undefined";
                    }
                }
                return retVal;
            }
            public static string GetPower(String ipString) {
                String wattageStr = "-1";
                // get current power value
                try {
                    String[] respArr = GetHttpCommandResponse("http://" + ipString + "/cm?cmnd=Status+8").Split(',');
                    foreach ( String item in respArr ) {
                        if ( item.StartsWith("\"Power\":") ) {
                            wattageStr = item.Substring(8);
                            break;
                        }
                    }
                } catch ( ArgumentException ) {
                    wattageStr = "-1";
                }
                return wattageStr;
            }
            // get image graph 
            public static byte[] GetPowerHistory(String ipString, MainForm main) {
                MemoryStream stream = new MemoryStream();
                main.ChartPower.SaveImage(stream, ImageFormat.Png);
                return stream.ToArray();
            }
            // toggle power monitoring
            public static string TogglePowerMonitor(String ipString, MainForm main) {
                main.checkBoxPower.Checked = !main.checkBoxPower.Checked;
                main.checkPower(null, null);
                return ": power monitoring = " + main.checkBoxPower.Checked.ToString();
            }
            // clear power history accoding to ipString
            public static void ClearPowerHistory(String ipString, MainForm main) {
                // activate requested Tasmota device
                GetPower(ipString);
                // clear its historical data
                main.ClearPowerHistory();
            }
        }

        // update the visibility of Tasmota hosts in UI
        async Task UpdateHostsOnUI() {
            // refresh status of pingable hosts
            TasmotaHostsList = await RefreshIpAddressesStatus(TasmotaHostsList);
            
            this.Invoke(new Action(() => {
                // reset search progress
                this.toolStripProgressBarMain.Value = 0;
                // add webbrowser tabs showing pingable Tasmota gadgets regardless what device type
                for ( int i = 0; i < TasmotaHostsList.Count; i++ ) {
                    if ( TasmotaHostsList[i].pingable ) {
                        // update TasmotaHostsList regarding host.teleName
                        TasmotaHostsList[i].teleName = Tools.RemoveSpaces(TasmotaHostsList[i].name);
                        TasmotaHostsList[i].teleName = Tools.RemoveHashes(TasmotaHostsList[i].teleName);
                        // add browser tab if not existing
                        String tasmotaName = TasmotaHostsList[i].name.Trim();
                        int ndx = TabExistsNdx(tasmotaName);
                        if ( ndx == -1 ) {
                            this.tabControlDevices.TabPages.Add(tasmotaName);
                            int tabNo = this.tabControlDevices.TabPages.Count - 1;
                            ChromiumWebBrowser cwb = new ChromiumWebBrowser("http://" + TasmotaHostsList[i].hostip + "/");
                            cwb.Dock = DockStyle.Fill;
                            this.tabControlDevices.TabPages[tabNo].Controls.Add(cwb);
                        }
                    } else {
                        TasmotaHostsList[i].pingable = false;
                        this.toolStripStatusLabelMain.Text = "Tasmota device '" + TasmotaHostsList[i].name + "' not available.";
                    }
                }

                // clear Telegram device specific commands 
                TelegramDeviceCommandList.Clear();

                // update combobox with Tasmota devices
                int lastNdx = this.comboBoxTasmotaDevices.SelectedIndex;
                this.comboBoxTasmotaDevices.Items.Clear();
                foreach ( var host in TasmotaHostsList ) {
                    // update combobox
                    this.comboBoxTasmotaDevices.Items.Add(host.name + " - " + host.hostip + " - " + host.type + " - " + (host.pingable ? "online" : "<offline>"));
                    // build a Tasmota device specific commands list for each host to be later used in Telgram's receiver parser
                    if ( host.type == TasmotaDeviceFilter ) {
                        // only pingable devices
                        if ( host.pingable ) {
                            TelegramDeviceCommandList.AddRange(Tools.GetBasicSocketCommands(host.teleName, TasmotaSocket.TASMOTA_SOCKET_COMMANDS));
                        }
                    }
                }
                this.comboBoxTasmotaDevices.SelectedIndex = lastNdx;
            }));

            // build the final "/help command" response string, showing Telegram commands to the remote user
            TelegramFinalCommands = TELEGRAM_STANDARD_COMMANDS;
            if ( TasmotaHostsList.Count > 0 ) {
                TelegramDevicesCommands = Tools.FormatTelegramDecicesCommands(TelegramDeviceCommandList, TasmotaSocket.TASMOTA_SOCKET_COMMANDS.Length);
                TelegramFinalCommands = TELEGRAM_STANDARD_COMMANDS + TelegramDevicesCommands;
            }
        }

        // compare 2x List<host> return if different
        bool AreHostListsDifferent(List<host>listOne, List<host> listTwo) {
            bool different = false;
            if ( listOne.Count != listTwo.Count ) {
                different = true;
            } else {
                // find listOne matches in listTwo
                bool[] matchesOne = new bool[listOne.Count];
                for ( int i = 0; i < listOne.Count; i++ ) {
                    foreach ( var hostTwo in listTwo ) {
                        if ( hostTwo.name == listOne[i].name && hostTwo.hostip == listOne[i].hostip ) {
                            matchesOne[i] = true;
                            break;
                        }
                    }
                }
                // find listTwo matches in listOne
                bool[] matchesTwo = new bool[listTwo.Count];
                for ( int i = 0; i < listTwo.Count; i++ ) {
                    foreach ( var hostOne in listOne ) {
                        if ( hostOne.name == listTwo[i].name && hostOne.hostip == listTwo[i].hostip ) {
                            matchesTwo[i] = true;
                            break;
                        }
                    }
                }
                // inspect both matches arrays
                bool resOne = true;
                foreach ( var match in matchesOne ) {
                    if ( !match ) {
                        resOne = false;
                    }
                }
                bool resTwo = true;
                foreach ( var match in matchesTwo ) {
                    if ( !match ) {
                        resTwo = false;
                    }
                }
                if ( !resOne || !resTwo ) {
                    different = true;
                }
            }
            // return
            return different;
        }

        // compare TasmotaHostsList (app session) with Setting.ListHosts (INI) and return a decision
        bool IsUpdateSettingsHostsListDue() {
            bool updateHostsList = false;
            if ( TasmotaHostsList.Count != Settings.ListHosts.Count ) {
                updateHostsList = true;
            } else {
                // find app matches in INI
                bool[] matches1 = new bool[TasmotaHostsList.Count];
                for ( int i = 0; i < TasmotaHostsList.Count; i++ ) {
                    string hostStr = TasmotaHostsList[i].hostip + "," + TasmotaHostsList[i].name;
                    foreach ( var hostIni in Settings.ListHosts ) {
                        if ( hostIni.Contains(hostStr) ) {
                            matches1[i] = true;
                            break;
                        }
                    }
                }
                // find INI matches in app
                bool[] matches2 = new bool[Settings.ListHosts.Count];
                for ( int i = 0; i < Settings.ListHosts.Count; i++ ) {
                    foreach ( var host in TasmotaHostsList ) {
                        string appStr = host.hostip + "," + host.name;
                        if ( Settings.ListHosts[i].Contains(appStr) ) {
                            matches2[i] = true;
                            break;
                        }
                    }
                }
                // inspect both matches' arrays
                bool res1 = true;
                foreach ( var match in matches1 ) {
                    if ( !match ) {
                        res1 = false;
                    }
                }
                bool res2 = true;
                foreach ( var match in matches2 ) {
                    if ( !match ) {
                        res2 = false;
                    }
                }
                if ( !res1 || !res2 ) {
                    updateHostsList = true;
                }
            }
            // make sure
            if ( updateHostsList ) {
                var result = MessageBox.Show("Do you want to save the current session's Tasmota devices list?", "Note", MessageBoxButtons.YesNo);
                if ( result != DialogResult.Yes ) {
                    updateHostsList = false;
                }
            }
            // return
            return updateHostsList;
        }

        // obtain PC's IP address running GrzTasmotaBot
        string IpThisPC() {
            string output = "";
            NetworkInterface[] ni = NetworkInterface.GetAllNetworkInterfaces();
            foreach ( NetworkInterface item in ni ) {
                if ( (item.OperationalStatus == OperationalStatus.Up) && ((item.NetworkInterfaceType == NetworkInterfaceType.Ethernet) || (item.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)) ) {
                    IPInterfaceProperties adapterProperties = item.GetIPProperties();
                    // take the ip only if it has a gateway
                    if ( adapterProperties.GatewayAddresses.FirstOrDefault() != null ) {
                        foreach ( UnicastIPAddressInformation ip in adapterProperties.UnicastAddresses ) {
                            if ( ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ) {
                                output = ip.Address.ToString();
                                break;
                            }
                        }
                    }
                }
                if ( output != "" ) {
                    break;
                }
            }
            return output;
        }

        // get pingable status of a given IP adress
        async Task<bool> PingIpAddress(string ipAddress) {
            bool pingable = false;
            // ping task
            var pingTask = Task.Run(async () => {
                using ( Ping ping = new Ping() ) {
                    return await ping.SendPingAsync(ipAddress, 2000);
                }
            });
            // wait for completion of ping task
            var result = await pingTask;
            // filter result
            if ( result.Status == System.Net.NetworkInformation.IPStatus.Success ) {
                pingable = true;
            }
            // return a list of pingable hosts
            return pingable;
        }

        // get all pingable IP adresses in the network, based on the IP from the PC running this app
        async Task<List<host>> SearchActiveIpAddresses(string ipThis) {
            List<host> hostList = new List<host>();
            // network IP pattern
            string ipBase = ipThis.Substring(0, ipThis.LastIndexOf(".") + 1);  // sample: "10.0.1."
            // list of IPs in the local network
            List<String> ipList = new List<String>();
            for ( int i = 1; i < 255; i++ ) {
                string ip = ipBase + i.ToString();
                ipList.Add(ip);
            }
            // list of ping tasks consumes list of IPs
            var pingTasks = ipList.Select(async ip => {
                using ( Ping ping = new Ping() ) {
                    return await ping.SendPingAsync(ip, 2000);
                }
            });
            // wait for completion of ping tasks, possible performance issue with single task: https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1842
            var results = await Task.WhenAll(pingTasks);
            // filter results
            foreach (var pr in results) {
                if ( pr.Status == System.Net.NetworkInformation.IPStatus.Success ) {
                    hostList.Add(new host(pr.Address.ToString(), ""));
                }
            }
            // return a list of pingable hosts
            return hostList;
        }

        // refresh pingable status of list of IP adresses
        async Task<List<host>> RefreshIpAddressesStatus(List<host> list) {
            // reset pingable status
            for ( int i = 0; i < list.Count; i++ ) {
                list[i].pingable = false;
            }
            // list of ping tasks consumes a list of IPs directly derived from argument list
            var pingTasks = list.Select(async item => {
                // one task per ip address
                using ( Ping ping = new Ping() ) {
                    return await ping.SendPingAsync(item.hostip, 2000);
                }
            });
            // wait for completion of ping tasks
            var results = await Task.WhenAll(pingTasks);
            // loop ping results to find ip address matches with argument list
            foreach( var r in results ) {
                // loop argument list
                for ( int i = 0; i < list.Count; i++ ) {
                    // find match, note: non pingable IPs provide r.Address being 0.0.0.0
                    if ( r.Address.ToString() == list[i].hostip ) {
                        list[i].pingable = (r.Status == System.Net.NetworkInformation.IPStatus.Success);
                        break;
                    }
                }
            }
            // return host list with pingable status update
            return list;
        }

        // add tabs with browser according to found Tasmota devices
        List<host> UpdateTasmotaDeviceListAndBrowsertabs(List<host> hostList) {
            for ( int i = 0; i < hostList.Count; i++ ) {
                // get Tasmota gadget name
                String tasmotaName = "Tasmota device";
                int ndxStart = hostList[i].GETstr.IndexOf("<title>") + 7;
                int ndxStop = hostList[i].GETstr.IndexOf("- Main Menu</title>");
                if ( ndxStart != -1 && ndxStop != -1 && ndxStop - ndxStart >= 0 ) {
                    tasmotaName = hostList[i].GETstr.Substring(ndxStart, ndxStop - ndxStart).Trim();
                    hostList[i].name = tasmotaName;
                }
                // create Tasmota device type name
                foreach (var dev in TasmotaSocketsList) {
                    if ( hostList[i].GETstr.IndexOf(dev) != -1 ) {
                        hostList[i].type = TasmotaDeviceType.SOCKET;
                        break;
                    }
                }
                // update regarding host.teleName
                hostList[i].teleName = Tools.RemoveSpaces(hostList[i].name);
                hostList[i].teleName = Tools.RemoveHashes(hostList[i].teleName);
                // add a webbrowser tab showing the Tasmota gadget, if it is not already existing
                int ndx = TabExistsNdx(tasmotaName);
                if ( ndx == -1 ) {
                    this.tabControlDevices.TabPages.Add(tasmotaName);
                    int tabNo = this.tabControlDevices.TabPages.Count - 1;
                    ChromiumWebBrowser cwb = new ChromiumWebBrowser("http://" + hostList[i].hostip + "/");
                    cwb.Dock = DockStyle.Fill;
                    this.tabControlDevices.TabPages[tabNo].Controls.Add(cwb);
                }
            }
            return hostList;
        }
        // check if tab contained in this.tabControlDevices.TabPages already exists and return its index 
        int TabExistsNdx(String text) {
            int retVal = -1;
            var foundTabs = this.tabControlDevices.TabPages.OfType<TabPage>().Where(tab => tab.Text.Equals(text)).ToList();
            if ( foundTabs.Count > 0 ) {
                var index = this.tabControlDevices.TabPages.IndexOf(foundTabs[0]);
                retVal = index;
            }
            return retVal;
        }

        // filter hostList of all pingable hosts to only let pass those with tasmota content
        List<host> GetTasmotaContentHosts(List<host> hostList) {
            for ( int i = hostList.Count - 1; i >= 0; i-- ) {
                if ( hostList[i].GETstr.IndexOf("tasmota", 0, StringComparison.OrdinalIgnoreCase) == -1 ) {
                    hostList.RemoveAt(i);
                }
            }
            return hostList;
        }

        // get http GET responses from a list of hosts
        async Task<List<host>> GetHttpGETResponses(List<host> hostList) {
            HttpClient client = new HttpClient();
            List<Task<String>> taskList = new List<Task<String>>();
            for ( int i = 0; i < hostList.Count; i++ ) {
                try {
                    Task<String> t = client.GetStringAsync("http://" + hostList[i].hostip);
                    taskList.Add(t);
                } catch ( ArgumentNullException ane ) {
                    ;
                } catch ( HttpRequestException hre ) {
                    ;
                } catch ( Exception exc ) {
                    ;
                }
            };
            try {
                await Task.WhenAll(taskList.ToArray());
            } catch ( ArgumentNullException ane ) {
                ;
            } catch ( HttpRequestException hre ) {
                ;
            } catch ( Exception exc ) {
                ;
            }
            for ( int i = 0; i < taskList.Count; i++ ) {
                try {
                    var responseString = await taskList[i];
                    hostList[i].GETstr = responseString;
                } catch ( ArgumentNullException ane ) {
                    ;
                } catch ( HttpRequestException hre ) {
                    ;
                } catch ( Exception exc ) {
                    ;
                }
            }
            return hostList;
        }

        // send http command to a host and get its http response
        static string GetHttpCommandResponse(string command) {
            var responseString = "";
            HttpClient client = new HttpClient();
            try {
                // send and wait for completion
                responseString = Task.Run(() => client.GetStringAsync(command)).Result;
            } catch ( ArgumentNullException ane ) {
                ;
            } catch ( HttpRequestException hre ) {
                ;
            } catch ( Exception exc ) {
                ;
            }
            // return response
            return responseString;
        }

        // ------------------------------------------- Telegram section ---------------------------------------------------------------------------

        // try to restart Telegram, if it should run but it doesn't due to an internal fail
        private void timerTelegramRestart_Tick(object sender, EventArgs e) {
            // stop timerTelegramRestart
            this.timerTelegramRestart.Stop();
            Logger.logTextLn(DateTime.Now, "timerTelegramRestart_Tick: timer stopped");
            // limit app restarts
            if ( Settings.UseTelegramBot && _Bot == null && Settings.TelegramRestartAppCount < 5 ) {
                // restart app after too many failing Telegram restarts in the current app session
                if ( _telegramRestartCounter > 5 ) {
                    // set flag, that this is not an app crash
                    AppSettings.IniFile ini = new AppSettings.IniFile(System.Windows.Forms.Application.ExecutablePath + ".ini");
                    ini.IniWriteValue("GrzTasmotaBot", "AppCrash", "False");
                    // memorize count of Telegram malfunctions forcing an app restart: needed to avoid restart loops
                    Settings.TelegramRestartAppCount++;
                    ini.IniWriteValue("GrzTasmotaBot", "TelegramRestartAppCount", Settings.TelegramRestartAppCount.ToString());
                    Logger.logTextLnU(DateTime.Now, String.Format("timerTelegramRestart_Tick: Telegram restart count > 5, now restarting GrzTasmotaBot"));
                    // restart GrzTasmotaBot: if Telegram restart count in session > 5, then restart app (usual max. 2 over months)
                    string exeName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                    ProcessStartInfo startInfo = new ProcessStartInfo(exeName);
                    try {
                        System.Diagnostics.Process.Start(startInfo);
                        this.Close();
                    } catch ( Exception ) {; }
                } else {
                    // restart Telegram
                    _telegramRestartCounter++;
                    Logger.logTextLnU(DateTime.Now, String.Format("timerTelegramRestart_Tick: Telegram restart #{0} of 5", _telegramRestartCounter));
                    _telegramOnErrorCount = 0;
                    _Bot = new TeleSharp.TeleSharp(Settings.BotAuthenticationToken);
                    _Bot.OnMessage += OnMessage;
                    _Bot.OnError += OnError;
                    _Bot.OnLiveTick += OnLiveTick;
                }
            } else {
                Logger.logTextLnU(DateTime.Now, String.Format("timerTelegramRestart_Tick: missed restart use={0} bot={1} count={2}", Settings.UseTelegramBot, _Bot.ToString(), Settings.TelegramRestartAppCount));
            }
        }
        // Telegram connector provides a live tick info, this timer tick shall act, if Telegram live tick info fails multiple times
        private void timerCheckTelegramLiveTick_Tick(object sender, EventArgs e) {
            if ( _Bot != null ) {
                TimeSpan span = DateTime.Now - _connectionLiveTick;
                if ( span.TotalSeconds > 120 ) {
                    if ( _telegramLiveTickErrorCount > 10 ) {
                        // set flag, that this is not an app crash
                        AppSettings.IniFile ini = new AppSettings.IniFile(System.Windows.Forms.Application.ExecutablePath + ".ini");
                        ini.IniWriteValue("GrzTasmotaBot", "AppCrash", "False");
                        // Telegram malfunction forces an app restart
                        Settings.TelegramRestartAppCount++;
                        ini.IniWriteValue("GrzTasmotaBot", "TelegramRestartAppCount", Settings.TelegramRestartAppCount.ToString());
                        // give up after more than 10 live tick errors and log app restart
                        Logger.logTextLnU(DateTime.Now, String.Format("timerCheckTelegramLiveTick_Tick: Telegram not active for #{0} cycles, now restarting GrzTasmotaBot", _telegramLiveTickErrorCount));
                        // restart GrzTasmotaBot
                        string exeName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                        ProcessStartInfo startInfo = new ProcessStartInfo(exeName);
                        try {
                            System.Diagnostics.Process.Start(startInfo);
                            this.Close();
                        } catch ( Exception ) {; }
                    } else {
                        // try to restart Telegram
                        _telegramLiveTickErrorCount++;
                        Logger.logTextLnU(DateTime.Now, String.Format("timerCheckTelegramLiveTick_Tick: Telegram not active detected, now shut it down #{0}", _telegramLiveTickErrorCount));
                        try {
                            if ( _Bot != null ) {
                                _Bot.OnMessage -= OnMessage;
                                _Bot.OnError -= OnError;
                                _Bot.OnLiveTick -= OnLiveTick;
                                _Bot.Stop();
                                _Bot = null;
                            }
                            Logger.logTextLnU(DateTime.Now, "timerCheckTelegramLiveTick_Tick: trying to restart in 5 min");
                            this.timerTelegramRestart.Start();
                        } catch ( Exception ex ) {
                            Logger.logTextLnU(DateTime.Now, String.Format("timerCheckTelegramLiveTick_Tick ex: {0}", ex.Message));
                        }
                    }
                }
            }
        }
        // Telegram provides a live tick info
        private void OnLiveTick(DateTime now) {
            _connectionLiveTick = now;
            if ( _telegramLiveTickErrorCount > 0 ) {
                // telegram restart after a live tick fail was successful
                Logger.logTextLnU(DateTime.Now, String.Format("OnLiveTick: Telegram now active after previous fail #{0}", _telegramLiveTickErrorCount));
                _telegramLiveTickErrorCount = 0;
            }
        }
        // Telegram connector detected a connection issue
        private void OnError(bool connectionError) {
            _telegramOnErrorCount++;
            Logger.logTextLnU(DateTime.Now, String.Format("OnError: Telegram connect error {0} {1}", _telegramOnErrorCount, connectionError));
            if ( _Bot != null ) {
                _Bot.OnMessage -= OnMessage;
                _Bot.OnError -= OnError;
                _Bot.OnLiveTick -= OnLiveTick;
                _Bot.Stop();
                _Bot = null;
                Logger.logTextLnU(DateTime.Now, "OnError: Telegram connect error, now shut down - trying to restart in 5 min");
                this.timerTelegramRestart.Start();
            } else {
                Logger.logTextLnU(DateTime.Now, "OnError: _Bot == null, but OnError still active");
            }
        }
        // read recently received Telegram messages to the local bot
        private void OnMessage(TeleSharp.Entities.Message message) {
            // get message sender information
            MessageSender sender = (MessageSender)message.Chat ?? message.From;
            Logger.logTextLnU(DateTime.Now, "'" + message.Text + "' from: " + sender.Id.ToString());
            this.Invoke(new Action(() => {
                this.textBoxLogger.AppendText(DateTime.Now.ToString() + " RX '" + message.Text + "' from: " + sender.Id.ToString() + "\r\n");
            }));
            // allow whitelist entries only
            if ( Settings.UseTelegramWhitelist ) {
                bool senderIsWhitelisted = false;
                foreach ( var wlm in Settings.TelegramWhitelist ) {
                    string[] arr = wlm.Split(',');
                    if ( arr.Length == 2 ) {
                        if ( sender.Id.ToString() == arr[1] ) {
                            senderIsWhitelisted = true;
                            break;
                        }
                    }
                }
                if ( !senderIsWhitelisted ) {
                    Logger.logTextLnU(DateTime.Now, "Sender is not whitelisted: " + sender.Id.ToString());
                    return;
                }
            } else {
                AutoMessageBox.Show("Consider using Telegram whitelist features.", "Security Warning", 5000);
            }

            try {
                // received message check
                if ( string.IsNullOrEmpty(message.Text) ) {
                    return;
                }
                // received message parser: at first, handle standard messages 
                switch ( message.Text.ToLower() ) {
                    // help: sends all vailable commands to caller
                    case "/help": {
                            _Bot.SendMessage(new SendMessageParams {
                                ChatId = sender.Id.ToString(),
                                Text = TelegramFinalCommands
                            });
                            this.Invoke(new Action(() => {
                                this.textBoxLogger.AppendText(DateTime.Now.ToString() + " TX '" + TelegramFinalCommands + "' to: " + sender.Id.ToString() + "\r\n");
                            }));
                            break;
                        }
                    // welcome msg
                    case "/hello": {
                            string welcomeMessage = $"Welcome {message.From.Username} !{Environment.NewLine}My name is GrzTasmotaBot{Environment.NewLine}";
                            _Bot.SendMessage(new SendMessageParams {
                                ChatId = sender.Id.ToString(),
                                Text = welcomeMessage
                            });
                            this.Invoke(new Action(() => {
                                this.textBoxLogger.AppendText(DateTime.Now.ToString() + " TX '" + welcomeMessage + "' to: " + sender.Id.ToString() + "\r\n");
                            }));
                            break;
                        }
                    // time
                    case "/time": {
                            _Bot.SendMessage(new SendMessageParams {
                                ChatId = sender.Id.ToString(),
                                Text = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()
                            });
                            this.Invoke(new Action(() => {
                                this.textBoxLogger.AppendText(DateTime.Now.ToString() + " TX ' timestamp ' to: " + sender.Id.ToString() + "\r\n");
                            }));
                            break;
                        }
                    // location = fake
                    case "/location": {
                            _Bot.SendLocation(sender, "50.69421", "3.17456");
                            this.Invoke(new Action(() => {
                                this.textBoxLogger.AppendText(DateTime.Now.ToString() + " TX ' location ' to: " + sender.Id.ToString() + "\r\n");
                            }));
                            break;
                        }
                    // handle device specific messages: get the device specific Telegram command and execute it
                    default: {
                            string teleCmdMatch = TelegramDeviceCommandList.FirstOrDefault(cmd => "/" + cmd == message.Text);
                            // check for match
                            if ( teleCmdMatch == null ) {
                                // we have a 'no Telegram command' error ==> get out and return
                                _Bot.SendMessage(new SendMessageParams {
                                    ChatId = sender.Id.ToString(),
                                    Text = "command  " + message.Text + " not valid"
                                });
                                this.Invoke(new Action(() => {
                                    this.textBoxLogger.AppendText(DateTime.Now.ToString() + " TX ' " + "command  " + message.Text + "  not valid" + " ' to: " + sender.Id.ToString() + "\r\n");
                                }));
                                Logger.logTextLnU(DateTime.Now, String.Format("unknown command '{0}' from {1}", message.Text, sender.Id.ToString()));
                                return;
                            }
                            // simply reply cmd to sender
                            _Bot.SendMessage(new SendMessageParams {
                                ChatId = sender.Id.ToString(),
                                Text = "roger " + teleCmdMatch
                            });
                            this.Invoke(new Action(() => {
                                this.textBoxLogger.AppendText(DateTime.Now.ToString() + " TX ' roger " + teleCmdMatch + "' to: " + sender.Id.ToString() + "\r\n");
                            }));
                            // parse the received cmd: get device, set it in UI, get command, execute command 
                            string[] cmdSplit = teleCmdMatch.Split('_');
                            if ( cmdSplit.Length > 0 ) {

                                // so far we only know the Telegram friendly name
                                string teleName = cmdSplit[0];

                                // find index of 'real Tasmota host name' in TasmotaHostsList with teleName as input
                                int index = TasmotaHostsList.FindIndex(item => item.teleName == teleName);
                                if ( index == -1 ) {
                                    this.Invoke(new Action(() => {
                                        this.textBoxLogger.AppendText(DateTime.Now.ToString() + " host not found " + sender.Id.ToString() + "\r\n");
                                    }));
                                    return;
                                }
                                // select Tasmota host in comboBoxTasmotaDevices --> changes UI's selected Tasmota device
                                this.Invoke(new Action(() => {
                                    this.comboBoxTasmotaDevices.SelectedIndex = index;
                                }));

                                // for the sake of mind, get host IP from UI thread via combobox
                                string hostIp = "";
                                this.Invoke(new Action(() => {
                                    hostIp = TasmotaHostsList[this.comboBoxTasmotaDevices.SelectedIndex].hostip;
                                }));

                                // obtain device specific command name
                                MethodInfo commandMatch = TasmotaSocket.methods.FirstOrDefault(item => item.Name.EndsWith(cmdSplit[1]));
                                if ( commandMatch == null ) {
                                    this.Invoke(new Action(() => {
                                        this.textBoxLogger.AppendText(DateTime.Now.ToString() + "obtain real command name EXCEPTION" + sender.Id.ToString() + "\r\n");
                                    }));
                                    return;
                                }

                                // execute device specific command
                                string response = "";
                                switch ( commandMatch.Name ) {
                                    // get Tasmota current power consumption
                                    case "GetPower":
                                        response = TasmotaSocket.GetPower(hostIp);
                                        break;
                                    // toggle power monitoring
                                    case "TogglePowerMonitor":
                                        this.Invoke(new Action(() => {
                                            response = TasmotaSocket.TogglePowerMonitor(hostIp, this);
                                        }));
                                        break;
                                    // clear power history data
                                    case "ClearPowerHistory":
                                        this.Invoke(new Action(() => {
                                            TasmotaSocket.ClearPowerHistory(hostIp, this);
                                        }));
                                        break;
                                    // send power graph image
                                    case "GetPowerHistory":
                                        // current power monitoring state
                                        bool powerChartWasActive = this.checkBoxPower.Checked;
                                        // outdated data note 
                                        if ( !powerChartWasActive ) {
                                            // turn power monitoring on
                                            this.Invoke(new Action(() => {
                                                this.checkBoxPower.Checked = true;
                                                checkPower(null, null);
                                            }));
                                            _Bot.SendMessage(new SendMessageParams {
                                                ChatId = sender.Id.ToString(),
                                                Text = commandMatch.Name + ": power monitoring is turned off"
                                            });
                                        }
                                        // grab image from power chart and send it 
                                        try {
                                            this.Invoke(new Action(() => {
                                                // force UI refresh
                                                timerUpdateHosts_Tick(null, null);
                                                // get current power value
                                                response = TasmotaSocket.GetPower(hostIp);
                                                // local log
                                                this.textBoxLogger.AppendText(DateTime.Now.ToString() + " TX ' " + teleCmdMatch + " PowerGraph to: " + sender.Id.ToString() + "\r\n");
                                                // get image graph as byte array
                                                byte[] buffer = TasmotaSocket.GetPowerHistory(hostIp, this);
                                                // send image graph as picture
                                                _Bot.SendPhoto(sender, buffer, "Power Graph", response + " W");
                                            }));
                                        } catch ( Exception ex ) {
                                            this.Invoke(new Action(() => {
                                                this.textBoxLogger.AppendText(DateTime.Now.ToString() + " TX ' " + teleCmdMatch + " <exception> to: " + sender.Id.ToString() + "\r\n");
                                            }));
                                        }
                                        // set previous power monitoring state
                                        if ( !powerChartWasActive ) {
                                            // turn power monitoring ff
                                            this.checkBoxPower.Checked = false;
                                            // force UI refresh
                                            this.Invoke(new Action(() => {
                                                timerUpdateHosts_Tick(null, null);
                                            }));
                                        }
                                        // need to return from here
                                        return;
                                    // get Tasmota status: covers GetStatus, SetOn, SetOff
                                    default:
                                        response = TasmotaSocket.GetStatus(hostIp, true);
                                        break;
                                }

                                // send response to Telegram sender
                                _Bot.SendMessage(new SendMessageParams {
                                    ChatId = sender.Id.ToString(),
                                    Text = commandMatch.Name + " " + response
                                });

                                // local logger
                                this.Invoke(new Action(() => {
                                    this.textBoxLogger.AppendText(DateTime.Now.ToString() + " TX ' " + teleCmdMatch + " " + response + " ' to: " + sender.Id.ToString() + "\r\n");
                                }));

                                // update UI label
                                this.Invoke(new Action(() => {
                                    this.labelSocket.Text = response;
                                }));
                            }
                            break;
                        }
                }
            } catch ( Exception ex ) {
                Logger.logTextLnU(DateTime.Now, "EXCEPTION OnMessage: " + ex.Message);
            }
        }

        // periodically ping internet: no need to do it async -> even blocking is ok, because doPingLooper runs in its own from UI separated thread
        public void doPingLooper(ref bool runPing, ref string strTestIP) {
            int pingFailCounter = 0;
            int stopLogCounter = 0;
            do {
                // execute ping: async were a nice to have, but arguments with ref are not allowed
                PingReply reply = null;
                using ( Ping ping = new Ping() ) {
                    reply = ping.Send(strTestIP, 1000);
                }
                // two possibilities
                if ( reply != null && reply.Status == System.Net.NetworkInformation.IPStatus.Success ) {
                    // ping ok
                    Settings.PingOk = true;
                    // notify about previous fails
                    if ( pingFailCounter > 10 ) {
                        Logger.logTextLnU(DateTime.Now, String.Format("doPingLooper: ping is ok - after {0} fails", pingFailCounter));
                    }
                    pingFailCounter = 0;
                    if ( stopLogCounter > 0 ) {
                        Logger.logTextLnU(DateTime.Now, "doPingLooper: ping is ok - after a long time failing");
                    }
                    stopLogCounter = 0;
                } else {
                    // ping fail
                    Settings.PingOk = false;
                    pingFailCounter++;
                }
                // 10x subsequent ping fails in 100s 
                if ( (pingFailCounter > 0) && (pingFailCounter % 10 == 0) ) {
                    Logger.logTextLn(DateTime.Now, "doPingLooper: network reset after 10x ping fail");
                    bool networkUp = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
                    if ( networkUp ) {
                        Logger.logTextLn(DateTime.Now, "doPingLooper: network is up, but 10x ping failed");
                        if ( stopLogCounter < 5 ) {
                            Logger.logTextLn(DateTime.Now, "doPingLooper: local network is up");
                            stopLogCounter++;
                        }
                    } else {
                        if ( stopLogCounter < 5 ) {
                            Logger.logTextLn(DateTime.Now, "doPingLooper: network is down");
                            stopLogCounter++;
                        }
                    }
                }
                //
                System.Threading.Thread.Sleep(10000);
            } while ( runPing );
        }

        // --------------------------------------- Settings helpers ------------------------------------------------------------

        // update settings from app
        void updateSettingsFromAppProperties() {
            Settings.FormSize = this.Size;
            Settings.FormLocation = this.Location;
        }
        // update app from settings
        async void updateAppPropertiesFromSettings() {
            // UI app layout
            this.Size = Settings.FormSize;
            // get all display ranges (multiple monitors) and check, if desired location fits in
            Rectangle dispRange = new Rectangle(0, 0, 0, int.MaxValue);
            foreach ( Screen sc in Screen.AllScreens ) {
                dispRange.X = Math.Min(sc.Bounds.X, dispRange.X);
                dispRange.Width += sc.Bounds.Width;
                dispRange.Height = Math.Min(sc.Bounds.Height, dispRange.Height);
            }
            dispRange.X -= Settings.FormSize.Width / 2;
            dispRange.Height -= Settings.FormSize.Height / 2;
            if ( !dispRange.Contains(Settings.FormLocation) ) {
                Settings.FormLocation = new Point(100, 100);
            }
            this.Location = Settings.FormLocation;
            // log start
            Logger.WriteToLog = Settings.WriteLogfile;
            Logger.logTextU("\r\n---------------------------------------------------------------------------------------------------------------------------\r\n");
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            Logger.logTextLnU(DateTime.Now, String.Format("{0} {1}", assembly.FullName, fvi.FileVersion));
            // distinguish between regular app start and a restart after app crash
            AppSettings.IniFile ini = new AppSettings.IniFile(System.Windows.Forms.Application.ExecutablePath + ".ini");
            if ( bool.Parse(ini.IniReadValue("GrzTasmotaBot", "AppCrash", "False")) ) {
                Logger.logTextLnU(DateTime.Now, "App was restarted after crash");
            } else {
                Logger.logTextLn(DateTime.Now, "App start regular");
            }
            // assume an app crash as default behavior: this flag is reset to False, if app closes the normal way
            ini.IniWriteValue("GrzTasmotaBot", "AppCrash", "True");
            // handle Telegram bot usage
            Settings.PingOk = await PingIpAddress(Settings.PingTestAddress);
            if ( Settings.PingOk ) {
                // could be, that Telegram was recently enabled in Settings, but don't activate it, if restart count is already too large
                if ( Settings.UseTelegramBot && Settings.TelegramRestartAppCount < 5 ) {
                    if ( _Bot == null ) {
                        _Bot = new TeleSharp.TeleSharp(Settings.BotAuthenticationToken);
                        _Bot.OnMessage += OnMessage;
                        _Bot.OnError += OnError;
                        _Bot.OnLiveTick += OnLiveTick;
                        this.timerCheckTelegramLiveTick.Start();
                        Logger.logTextLn(DateTime.Now, "updateAppPropertiesFromSettings: Telegram bot activated");
                    } else {
                        Logger.logTextLn(DateTime.Now, "updateAppPropertiesFromSettings: Telegram is already active");
                    }
                } else {
                    if ( Settings.TelegramRestartAppCount >= 5 ) {
                        Logger.logTextLn(DateTime.Now, "updateAppPropertiesFromSettings: Telegram not activated due to app restart limit");
                    } else {
                        Logger.logTextLn(DateTime.Now, "updateAppPropertiesFromSettings: Telegram not activated");
                    }
                }
            }
            // could be, that Telegram was recently disabled in Settings
            if ( !Settings.UseTelegramBot ) {
                if ( _Bot != null ) {
                    _Bot.OnMessage -= OnMessage;
                    _Bot.OnError -= OnError;
                    _Bot.OnLiveTick -= OnLiveTick;
                    _Bot.Stop();
                    this.timerCheckTelegramLiveTick.Stop();
                    this.timerTelegramRestart.Stop();
                    _Bot = null;
                    Logger.logTextLn(DateTime.Now, "updateAppPropertiesFromSettings: Telegram bot deactivated");
                }
            }
            // ping monitoring in a UI-thread separated task, which is a loop !! overrides Settings.PingOk !!
            Settings.PingTestAddressRef = Settings.PingTestAddress;
            if ( !_runPing ) {
                _runPing = true;
                 #pragma warning disable 4014
                Task.Run(() => { doPingLooper(ref _runPing, ref Settings.PingTestAddressRef); });
                #pragma warning restore 4014
            }
            // search Tasmota hosts refresh timer
            this.checkBoxAutoSearch.Text = "auto";
            this.checkBoxAutoSearch.Checked = false;
            if ( Settings.HostsUpdateInterval > 0 ) {
                this.checkBoxAutoSearch.Checked = true;
                this.checkBoxAutoSearch.Text = "auto " + Settings.HostsUpdateInterval.ToString() + "s";
                this.timerUpdateHosts.Interval = Settings.HostsUpdateInterval * 1000;
                this.timerUpdateHosts.Start();
            }
        }
        // call app settings --> PropertyGrid dialog
        private void buttonSettings_Click(object sender, EventArgs e) {
            // transfer current app settings to Settings class
            updateSettingsFromAppProperties();
            // start settings dialog
            Settings dlg = new Settings(Settings);
            // memorize settings
            AppSettings oldSettings;
            Settings.CopyAllTo(Settings, out oldSettings);
            if ( dlg.ShowDialog() == DialogResult.OK ) {
                // get changed values back from PropertyGrid settings dlg
                Settings = dlg.Setting;
                // update app settings
                updateAppPropertiesFromSettings();
                // backup ini
                string src = System.Windows.Forms.Application.ExecutablePath + ".ini";
                string dst = System.Windows.Forms.Application.ExecutablePath + ".ini_bak";
                // make a backup history
                if ( System.IO.File.Exists(dst) ) {
                    int counter = 0;
                    do {
                        dst = System.Windows.Forms.Application.ExecutablePath + ".ini_bak" + counter.ToString();
                        counter++;
                    } while ( System.IO.File.Exists(dst) );
                    // option to delete oldest backups
                    if ( counter > 10 ) {
                        var retDelete = MessageBox.Show("The most recent two Settings backups will be kept.\n\nContinue?", "Delete oldest Settings backups?", MessageBoxButtons.YesNo);
                        if ( retDelete == DialogResult.Yes ) {
                            // save most recent .ini_bakX to .ini_bak
                            int mostRecentIndex = counter - 2;
                            string srcMostRecent = System.Windows.Forms.Application.ExecutablePath + ".ini_bak" + mostRecentIndex.ToString();
                            string dstOldest = System.Windows.Forms.Application.ExecutablePath + ".ini_bak";
                            System.IO.File.Copy(srcMostRecent, dstOldest, true);
                            // now delete all .ini_bakX
                            for ( int i = mostRecentIndex; i >= 0; i-- ) {
                                string dstDelete = System.Windows.Forms.Application.ExecutablePath + ".ini_bak" + i.ToString();
                                System.IO.File.Delete(dstDelete);
                            }
                            // beside of .ini_bak, ".ini_bak0" will become the most recent bak
                            dst = System.Windows.Forms.Application.ExecutablePath + ".ini_bak0";
                        }
                    }
                }
                try {
                    System.IO.File.Copy(src, dst, false);
                } catch ( Exception ex ) {
                    var ret = MessageBox.Show(ex.Message + "\n\nContinue without Settings backup?.\n\nChanges are directly written to .ini.", "Settings backup failed", MessageBoxButtons.YesNo);
                    if ( ret != DialogResult.Yes ) {
                        // changes to Settings are not saved to ini
                        return;
                    }
                }
                // shall INI get updated with the current session's TasmotaHostsList                
                bool updateHostsList = IsUpdateSettingsHostsListDue();
                // update property grid with 
                if ( updateHostsList ) {
                    Settings.setPropertyGridToHostsList(TasmotaHostsList);
                }
                // write settings to INI
                Settings.writePropertyGridToIni(updateHostsList);
                // hosts refresh timer
                if ( Settings.HostsUpdateInterval > 0 ) {
                    this.timerUpdateHosts.Interval = Settings.HostsUpdateInterval * 1000;
                    this.timerUpdateHosts.Start();
                }
            } else {
                Settings.CopyAllTo(oldSettings, out Settings);
            }
        }

    }

    // --------------------------------- app settings class --------------------------------------------------------------
    public class AppSettings {

        // the literal name of the ini section
        private string iniSection = "GrzTasmotaBot";

        // custom form to show text inside a property grid
        [Editor(typeof(FooEditor), typeof(System.Drawing.Design.UITypeEditor))]
        class FooEditor : System.Drawing.Design.UITypeEditor {
            public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
                return System.Drawing.Design.UITypeEditorEditStyle.Modal;
            }
            public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value) {
                System.Windows.Forms.Design.IWindowsFormsEditorService svc = provider.GetService(typeof(System.Windows.Forms.Design.IWindowsFormsEditorService)) as System.Windows.Forms.Design.IWindowsFormsEditorService;
                String foo = value as String;
                if ( svc != null && foo != null ) {
                    using ( FooForm form = new FooForm() ) {
                        form.Value = foo;
                        svc.ShowDialog(form);
                    }
                }
                return value;
            }
        }
        class FooForm : Form {
            private TextBox textbox;
            private Button okButton;
            public FooForm() {
                textbox = new TextBox();
                textbox.Multiline = true;
                textbox.Dock = DockStyle.Fill;
                textbox.WordWrap = false;
                textbox.Font = new Font(FontFamily.GenericMonospace, textbox.Font.Size);
                textbox.ScrollBars = ScrollBars.Both;
                Controls.Add(textbox);
                okButton = new Button();
                okButton.Text = "OK";
                okButton.Dock = DockStyle.Bottom;
                okButton.DialogResult = DialogResult.OK;
                Controls.Add(okButton);
            }
            public string Value {
                get { return textbox.Text; }
                set { textbox.Text = value; }
            }
        }

        // make a copy of all class properties
        public void CopyAllTo(AppSettings source, out AppSettings target) {
            target = new AppSettings();
            var type = typeof(AppSettings);
            foreach ( var sourceProperty in type.GetProperties() ) {
                var targetProperty = type.GetProperty(sourceProperty.Name);
                targetProperty.SetValue(target, sourceProperty.GetValue(source, null), null);
            }
            foreach ( var sourceField in type.GetFields() ) {
                var targetField = type.GetField(sourceField.Name);
                targetField.SetValue(target, sourceField.GetValue(source));
            }
        }

        // define app properties
        [CategoryAttribute("User Interface")]
        [ReadOnly(true)]
        public Size FormSize { get; set; }
        [ReadOnly(true)]
        [CategoryAttribute("User Interface")]
        public Point FormLocation { get; set; }
        [CategoryAttribute("User Interface")]
        [Description("App writes to logfile")]
        [ReadOnly(false)]
        public Boolean WriteLogfile { get; set; }

        [CategoryAttribute("Internet")]
        [ReadOnly(true)]
        [Description("Current internet status via ping")]
        public Boolean PingOk { get; set; }
        [CategoryAttribute("Internet")]
        [ReadOnly(false)]
        [Description("Internet test IP address for ping")]
        public string PingTestAddress { get; set; }
        public string PingTestAddressRef;

        [CategoryAttribute("Telegram")]
        [Description("Use Telegram bot")]
        [ReadOnly(false)]
        public Boolean UseTelegramBot { get; set; }
        [CategoryAttribute("Telegram")]
        [Description("Number of app restarts due to Telegram errors")]
        [ReadOnly(true)]
        public int TelegramRestartAppCount { get; set; }
        [CategoryAttribute("Telegram")]
        [Description("Telegram bot authentication token")]
        [ReadOnly(false)]
        public string BotAuthenticationToken { get; set; }
        [CategoryAttribute("Telegram")]
        [Description("Use Telegram whitelist prevents unauthorized remote access. Do NOT enable it w/o whitelist members.")]
        [ReadOnly(false)]
        public Boolean UseTelegramWhitelist { get; set; }
        [CategoryAttribute("Telegram")]
        [DisplayName("Telegram whitelist")]
        [Description("List of clients allowed to communicate with the bot.\nUse this format (w/o quotes): 'a readable name,Telegram-ID'\nTelegram-ID could be obtained from UI Messages Logger window.")]
        [ReadOnly(false)]
        public BindingList<string> TelegramWhitelist { get; set; }
        [CategoryAttribute("Telegram")]
        [Description("Open link in browser to learn, how to use a Telegram Bot")]
        [Editor(typeof(FooEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public String HowToUseTelegram { get; set; }

        [CategoryAttribute("Tasmota hosts")]
        [DisplayName("ListHosts")]
        [Description("List all Tasmota hosts (not editable)")]
        public BindingList<string> ListHosts { get; set; }
        [CategoryAttribute("Tasmota hosts")]
        [DisplayName("Auto hosts update interval")]
        [Description("Value in seconds, 0 disables auto update")]
        public int HostsUpdateInterval { get; set; }

        // INI: read PropertyGrid from ini
        public void fillPropertyGridFromIni() {
            IniFile ini = new IniFile(System.Windows.Forms.Application.ExecutablePath + ".ini");
            int tmpInt;
            bool tmpBool;

            // form width
            if ( int.TryParse(ini.IniReadValue(iniSection, "FormWidth", "657"), out tmpInt) ) {
                FormSize = new Size(tmpInt, 0);
            }
            // form height
            if ( int.TryParse(ini.IniReadValue(iniSection, "FormHeight", "588"), out tmpInt) ) {
                FormSize = new Size(FormSize.Width, tmpInt);
            }
            // form x
            if ( int.TryParse(ini.IniReadValue(iniSection, "FormX", "10"), out tmpInt) ) {
                FormLocation = new Point(tmpInt, 0);
            }
            // form y
            if ( int.TryParse(ini.IniReadValue(iniSection, "FormY", "10"), out tmpInt) ) {
                FormLocation = new Point(FormLocation.X, tmpInt);
            }
            // app writes logfile
            if ( bool.TryParse(ini.IniReadValue(iniSection, "WriteLogfile", "False"), out tmpBool) ) {
                WriteLogfile = tmpBool;
            }
            // ping test address + a ref var with the same purpose (get/set cannot be a ref var)
            PingTestAddress = ini.IniReadValue(iniSection, "PingTestAddress", "8.8.8.8");
            PingTestAddressRef = PingTestAddress;
            // use Telegram bot
            if ( bool.TryParse(ini.IniReadValue(iniSection, "UseTelegramBot", "False"), out tmpBool) ) {
                UseTelegramBot = tmpBool;
            }
            // app restart count due to a Telegram malfunction
            if ( int.TryParse(ini.IniReadValue(iniSection, "TelegramRestartAppCount", "0"), out tmpInt) ) {
                TelegramRestartAppCount = tmpInt;
            }
            // get Telegram whitelist from INI
            TelegramWhitelist = new BindingList<string>();
            var ndx = 0;
            while ( true ) {
                string strFull = ini.IniReadValue("TelegramWhitelist", "client" + ndx++.ToString(), ",");
                if ( strFull != "," ) {
                    TelegramWhitelist.Add(strFull);
                } else {
                    break;
                }
            }
            // use Telegram whitelist
            if ( bool.TryParse(ini.IniReadValue(iniSection, "UseTelegramWhitelist", "False"), out tmpBool) ) {
                UseTelegramWhitelist = tmpBool;
                if ( TelegramWhitelist.Count == 0 ) {
                    UseTelegramWhitelist = false;
                }
            }
            // Telegram bot authentication token
            BotAuthenticationToken = ini.IniReadValue(iniSection, "BotAuthenticationToken", "");
            // how to use a Telegram bot
            HowToUseTelegram = "https://core.telegram.org/bots#creating-a-new-bot\\";
            // set all Tasmota hosts in PropertyGrid array
            ListHosts = new BindingList<string>();
            var i = 0;
            while ( true ) {
                // normal read from ini
                string strHostFull = ini.IniReadValue("Tasmota section", "host" + i++.ToString(), ",,");
                if ( strHostFull != ",," ) {
                    string[] arr = strHostFull.Split(',');
                    String strHostSplit = String.Format("{0},{1},{2}", arr[0], arr[1], arr[2]);
                    // build list of Hosts
                    ListHosts.Add(strHostSplit);
                } else {
                    break;
                }
            }
            // hosts update interval
            if ( int.TryParse(ini.IniReadValue("Tasmota section", "HostsUpdateInterval", "0"), out tmpInt) ) {
                HostsUpdateInterval = Math.Abs(tmpInt);
            }
        }

        // INI: write to ini
        public void writePropertyGridToIni(bool updateHosts) {
            // only wipe existing INI, if hosts are updated anyways - otherwise there won't be any hosts
            if ( updateHosts ) {
                System.IO.File.Delete(System.Windows.Forms.Application.ExecutablePath + ".ini");
            }
            // ini from scratch
            IniFile ini = new IniFile(System.Windows.Forms.Application.ExecutablePath + ".ini");
            // form width
            ini.IniWriteValue(iniSection, "FormWidth", FormSize.Width.ToString());
            // form height
            ini.IniWriteValue(iniSection, "FormHeight", FormSize.Height.ToString());
            // form width
            ini.IniWriteValue(iniSection, "FormX", FormLocation.X.ToString());
            // form height
            ini.IniWriteValue(iniSection, "FormY", FormLocation.Y.ToString());
            // app writes logfile
            ini.IniWriteValue(iniSection, "WriteLogfile", WriteLogfile.ToString());
            // ping test address
            ini.IniWriteValue(iniSection, "PingTestAddress", PingTestAddress);
            // use Telegram bot
            ini.IniWriteValue(iniSection, "UseTelegramBot", UseTelegramBot.ToString());
            // write Telegram whitelist to INI
            for ( int i = 0; i < TelegramWhitelist.Count; i++ ) {
                ini.IniWriteValue("TelegramWhitelist", "client" + i.ToString(), TelegramWhitelist[i]);
            }
            // use Telegram whitelist
            if ( TelegramWhitelist.Count == 0 ) {
                UseTelegramWhitelist = false;
            }
            ini.IniWriteValue(iniSection, "UseTelegramWhitelist", UseTelegramWhitelist.ToString());
            // app restart count due to Telegram malfunction
            ini.IniWriteValue(iniSection, "TelegramRestartAppCount", TelegramRestartAppCount.ToString());
            // Telegram bot authentication token
            ini.IniWriteValue(iniSection, "BotAuthenticationToken", BotAuthenticationToken);
            // write Tasmota hosts from PropertyGrid array to INI
            if ( updateHosts ) {
                for ( int i = 0; i < ListHosts.Count; i++ ) {
                    ini.IniWriteValue("Tasmota section", "host" + i.ToString(), ListHosts[i]);
                }
            }
            // hosts update interval 
            ini.IniWriteValue("Tasmota section", "HostsUpdateInterval", HostsUpdateInterval.ToString());
        }

        // obtain a string list of hosts from the settings PropertyGrid 
        public List<string> getHostsStringListFromPropertyGrid() {
            List<string> list = new List<string>();
            for ( int i = 0; i < ListHosts.Count; i++ ) {
                list.Add(ListHosts[i]);
            }
            return list;
        }
        // obtain the list of hosts from the settings PropertyGrid
        public List<MainForm.host> getHostsListFromPropertyGrid() {
            List<MainForm.host> list = new List<MainForm.host>();
            for ( int i = 0; i < ListHosts.Count; i++ ) {
                string[] arr = ListHosts[i].Split(',');
                list.Add(new MainForm.host("", ""));
                list[i].hostip = arr[0]; 
                list[i].GETstr = "";
                list[i].name = arr[1];
                list[i].type = arr[2];
            }
            return list;
        }
        // set the settings PropertyGrid to the list of hosts provided by the ROIs edit dialog
        public void setPropertyGridToHostsList(List<MainForm.host> list) {
            ListHosts = new BindingList<string>();
            for ( int i = 0; i < list.Count; i++ ) {
                ListHosts.Add(list[i].hostip + "," + list[i].name + "," + list[i].type);
            }
        }

        // INI-Files CLass : easiest (though outdated) way to administer app specific setup data
        public class IniFile {
            private string path;
            [DllImport("kernel32")]
            private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
            public IniFile(string path) {
                this.path = path;
            }
            public void IniWriteValue(string Section, string Key, string Value) {
                try {
                    WritePrivateProfileString(Section, Key, Value, this.path);
                } catch ( Exception ex ) {
                    MessageBox.Show("INI-File could not be saved. Please select another 'home folder' in the Main Window.", "Error");
                }
            }
            public string IniReadValue(string Section, string Key, string DefaultValue) {
                StringBuilder retVal = new StringBuilder(255);
                int i = GetPrivateProfileString(Section, Key, DefaultValue, retVal, 255, this.path);
                return retVal.ToString();
            }
        }
    }

}
