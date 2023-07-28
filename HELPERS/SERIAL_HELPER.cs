using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO.Ports;
using System.Management;

namespace AssemblySupport_CutOffSawSlideSupportSoftware
{
    public partial class MainForm
    {
        public SerialPort _sp = new SerialPort();

        Queue<string> _q = new Queue<string>();

        public void initSerialComm()
        {
            _sp.PortName = "COM10";
            _sp.BaudRate = 115200;
            _sp.DataBits = 8;
            _sp.Parity = Parity.None;
            _sp.Encoding = System.Text.Encoding.GetEncoding(28591);
            _sp.ReadTimeout = 50;
            _sp.StopBits = StopBits.One;
            _sp.Handshake = Handshake.None;

            updateCOMPorts();
            attemptToAutoConnectoToESP32();
        }

        public void updateCOMPorts()
        {
            try
            {
                cbSERIAL_PORTS.Items.Clear();

                using(var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
                {
                    var portNames = SerialPort.GetPortNames();
                    var ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());

                    var portList = portNames.Select(n => ports.FirstOrDefault(s => s.Contains(n))).ToList();

                    foreach(string s in portList)
                    {
                        cbSERIAL_PORTS.Items.Add(s);
                    }
                }
            }
            catch (Exception ex)
            {
                outputToLog($"[ERROR] Could not update COM list: {ex.Message}");
            }
        }

        public void attemptToAutoConnectoToESP32()
        {
            if(!(cbSERIAL_PORTS.Items.Count > 0))
            {
                outputToLog("[ERROR-attemptToAutoConnectToESP32] - Serial port combo box is empty! ManagementObjectSearcher failed?");
                return;
            }

            foreach(string s in cbSERIAL_PORTS.Items)
            {
                if(s.Contains("CP210x"))
                {
                    try
                    {
                        cbSERIAL_PORTS.SelectedItem = s;
                        if(_sp.IsOpen) _sp.Close();

                        int start_idx = s.IndexOf("(");
                        int end_idx = s.IndexOf(")");

                        if(start_idx == -1 || end_idx == -1)
                        {
                            outputToLog($"[ISSUE] NO SUFFICIENT SERIAL DEVICE FOUND. MANUALLY SELECT COM PORT");
                            return;
                        }

                        _sp.PortName = s.Substring(start_idx + 1, (end_idx - 1) - start_idx);

                        Thread proc = new Thread(MonitorSerialPort);

                        proc.Start();

                        outputToLog($"[INFO] Auto-Connected to {_sp.PortName}");
                        //updateSerialConnectionParams();
                    }
                    catch (Exception ex)
                    {
                        outputToLog($"[ERROR] Could not auto connect to ESP32: {ex.Message}");
                    }
                }
            }
        }

        public void serialDisconnect()
        {
            try
            {
                if(_sp.IsOpen)
                {
                    _sp.Close();
                    _monitor_running = false;

                    stopAndDisposeTimers();
                }
            }
            catch (Exception ex)
            {
                outputToLog($"[ERROR] Problem disconnecting from serial port: {ex.Message}");
            }
        }

        public void serialConnect()
        {
            try
            {
                if (cbSERIAL_PORTS.SelectedIndex != -1)
                {
                    foreach (string s in cbSERIAL_PORTS.Items)
                    {
                        int start_idx = s.IndexOf("(");
                        int end_idx = s.IndexOf(")");
                        _sp.PortName = s.Substring(start_idx + 1, (end_idx - 1) - start_idx);
                    }

                    Thread proc = new Thread(MonitorSerialPort);
                    proc.Start();
                }
                else
                {
                    outputToLog("[WARNING] Please select a COM port from the drop-down list!");
                }
            }
            catch (Exception ex)
            {
                outputToLog($"[ERROR] Cannot connect to serial port: {ex.Message}");
            }
        }

        Boolean _monitor_running = false;
        void MonitorSerialPort()
        {
            string s;

            try
            {
                _sp.ReadTimeout = 100;
                _sp.Open();
                initTimers();
                _monitor_running = true;
                updateSerialConnectionParams();
            }
            catch (Exception)
            {
                _monitor_running = false;
            }

            while(_monitor_running)
            {
                try
                {
                    s = _sp.ReadLine();
                    _q.Enqueue(s);
                }
                catch (Exception) { }
            }

            if(_sp.IsOpen) _sp.Close();
        }

        public void outputStringToSerialPort(string msg)
        {
            if(_sp.IsOpen)
            {
                _sp.Write(msg + "\r");
                lbOUTGOINGSERIAL.Items.Insert(0, msg);
            }
            else
            {
                outputToLog($"[ERROR] Could not send data [{msg}] - serial port not open.");
            }
        }

        public void updateSerialConnectionParams()
        {
            lblSERIAL_COMPORT.Text = _sp.PortName.ToString();
            lblSERIAL_CONNECTED.Text = _monitor_running ? "CONNECTED" : "DISCONNECTED";
            lblSERIAL_SPEED.Text = _sp.BaudRate.ToString();
        }
    }
}
