using System;
using System.Drawing;
using System.IO;                            
using System.Globalization;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace GrzTools {
    // tools static methods collector 
    public static class Tools {

        // remove whitespaces from string: https://stackoverflow.com/questions/6219454/efficient-way-to-remove-all-whitespace-from-string 
        public static string RemoveSpaces(string input) {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        // remove whitespaces from string: https://stackoverflow.com/questions/6219454/efficient-way-to-remove-all-whitespace-from-string 
        public static string RemoveHashes(string input) {
            return new string(input.ToCharArray()
                .Where(c => c != '#')
                .ToArray());
        }

        // basic Tasmota gadget commands assigned to a living device identified by its Telegram friendly name called teleName
        public static List<String> GetBasicSocketCommands(string teleName, string[] cmds) {
            List<String> retList = new List<String>();
            if ( teleName.Length > 0 ) {
                foreach ( var cmd in cmds ) {
                    retList.Add(teleName + cmd);
                }
            }
            return retList;
        }

        // devices commands to be sent with Telegram
        public static string FormatTelegramDecicesCommands(List<String> cmdList, int cmdCount) {
            var retVal = "\n\n";
            var ndx = 0;
            foreach ( var cmd in cmdList ) {
                ndx++;
                retVal += "/" + cmd + (ndx % cmdCount == 0 ? "\n\n" : "\n");
            }
            return retVal;
        }

    }

    // logger class
    public static class Logger {
        // write to log flag
        public static bool WriteToLog { get; set; }
        public static String FullFileNameBase { get; set; }
        // unconditional logging
        public static void logTextLnU(DateTime now, string logtxt) {
            _writeLogOverrule = true;
            logTextLn(now, logtxt);
            _writeLogOverrule = false;
        }
        public static void logTextU(string logtxt) {
            _writeLogOverrule = true;
            logTextToFile(logtxt);
            _writeLogOverrule = false;
        }
        // logging depending on WriteToLog
        public static void logTextLn(DateTime now, string logtxt) {
            logtxt = now.ToString("dd.MM.yyyy HH:mm:ss_fff ", CultureInfo.InvariantCulture) + logtxt;
            logText(logtxt + "\r\n");
        }
        public static void logText(string logtxt) {
            logTextToFile(logtxt);
        }
        // private
        private static bool _writeLogOverrule = false;
        private static bool _busy = false;
        private static void logTextToFile(string logtxt) {
            if ( !WriteToLog && !_writeLogOverrule ) {
                return;
            }
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            while ( _busy ) {
                Application.DoEvents();
                if ( sw.ElapsedMilliseconds > 1000 ) {
                    sw.Stop();
                    return;
                }
            }
            sw.Stop();
            _busy = true;
            try {
                if ( FullFileNameBase == null || FullFileNameBase.Length == 0 ) {
                    FullFileNameBase = Application.ExecutablePath;
                }
                string logFileName = FullFileNameBase + DateTime.Now.ToString("_yyyyMMdd", CultureInfo.InvariantCulture) + ".log";
                System.IO.StreamWriter lsw = System.IO.File.AppendText(logFileName);
                lsw.Write(logtxt);
                lsw.Close();
            } catch {; }
            _busy = false;
        }
    }

    // non blocking & self closing message box
    public class AutoMessageBox {
        AutoMessageBox(string text, string caption, int timeout) {
            Form w = new Form() { Size = new Size(0, 0) };
            TaskEx.Delay(timeout)
                  .ContinueWith((t) => w.Close(), TaskScheduler.FromCurrentSynchronizationContext());
            MessageBox.Show(w, text, caption);
        }
        public static void Show(string text, string caption, int timeout) {
            new AutoMessageBox(text, caption, timeout);
        }
        static class TaskEx {
            public static Task Delay(int dueTimeMs) {
                TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
                CancellationTokenRegistration ctr = new CancellationTokenRegistration();
                System.Threading.Timer timer = new System.Threading.Timer(delegate (object self) {
                    ctr.Dispose();
                    ((System.Threading.Timer)self).Dispose();
                    tcs.TrySetResult(null);
                });
                timer.Change(dueTimeMs, -1);
                return tcs.Task;
            }
        }
    }

}
