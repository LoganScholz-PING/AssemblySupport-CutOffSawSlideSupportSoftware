namespace AssemblySupport_CutOffSawSlideSupportSoftware
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            initSerialComm();
            initTimers();
            updateSerialConnectionParams();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _monitor_running = false;
            stopAndDisposeTimers();
        }

        public void outputToLog(string msg)
        {
            string tmp = $"[{DateTime.Now.ToString("MM-dd-yyyy(HH:mm:ss)")}]: {msg}\r\n";
            txtLOGGING.AppendText(tmp);
        }


    }
}