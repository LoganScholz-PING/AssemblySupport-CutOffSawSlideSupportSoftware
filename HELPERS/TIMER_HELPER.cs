using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AssemblySupport_CutOffSawSlideSupportSoftware
{
    public partial class MainForm
    {
        private static System.Timers.Timer timerIO;
        private static System.Timers.Timer timerPosition;
        private static System.Timers.Timer timerCheckSerialQueue;

        public void initTimers()
        {
            if(_sp.IsOpen)
            {
                timerIO = new System.Timers.Timer(10000);
                timerIO.Elapsed += timerIO_Tick;
                timerIO.AutoReset = false;
                timerIO.Enabled = true;

                timerPosition = new System.Timers.Timer(500);
                timerPosition.Elapsed += timerPosition_Tick;
                timerPosition.AutoReset = false;
                timerPosition.Enabled = true;

                timerCheckSerialQueue = new System.Timers.Timer(10);
                timerCheckSerialQueue.Elapsed += timerCheckSerialQueue_Tick;
                timerCheckSerialQueue.AutoReset = false;
                timerCheckSerialQueue.Enabled = true;
            }
        }

        string PRI1 = "PR I1";
        string PRI2 = "PR I2";
        string PRI3 = "PR I3";
        string PRI4 = "PR I4";
        string PRR1 = "PR R1";
        string PRSOL = "PR SOL";
        string PRCALVAR = "PR CALVAR";
        public void timerIO_Tick(Object source, ElapsedEventArgs e)
        {
            timerIO.Stop();

            try
            {
                outputStringToSerialPort(PRI1);
                outputStringToSerialPort(PRI2);
                outputStringToSerialPort(PRI3);
                outputStringToSerialPort(PRI4);
                outputStringToSerialPort(PRR1);
                outputStringToSerialPort(PRSOL);
                outputStringToSerialPort(PRCALVAR);
            }
            catch (Exception ex)
            {
                outputToLog($"[ERROR] timerIO: {ex.Message}");
            }
            finally
            {
                timerIO.Start();
            }
        }

        string PR_POS = "PR POS";
        public void timerPosition_Tick(Object source, ElapsedEventArgs e)
        {
            timerPosition.Stop();

            try
            {
                if(_sp.IsOpen)
                {
                    outputStringToSerialPort(PR_POS);
                }
            }
            catch (Exception ex)
            {
                outputToLog($"[ERROR] timerPosition: {ex.Message}");
            }
            finally
            {
                timerPosition.Start();
            }
        }

        string help = "!";
        string I1 = "I1";
        string I2 = "I2";
        string I3 = "I3";
        string I4 = "I4";
        string R1 = "R1";
        string SOL = "SOL";
        string POS = "POS";
        string CALVAR = "CALVAR";
        public void timerCheckSerialQueue_Tick(Object source, ElapsedEventArgs e)
        {
            timerCheckSerialQueue.Stop();

            try
            {
                while(_q.Count > 0)
                {
                    string s = _q.Dequeue();
                    lbINCOMINGSERIAL.Items.Insert(0, s);

                    if(s.Contains(help))
                    {
                        // see serial output
                    }
                    else if (s.Contains(I1))
                    {
                        lblPR_I1.Text = s.Substring(s.LastIndexOf('=') + 1);
                    }
                    else if (s.Contains(I2))
                    {
                        lblPR_I2.Text = s.Substring(s.LastIndexOf('=') + 1);
                    }
                    else if (s.Contains(I3))
                    {
                        lblPR_I3.Text = s.Substring(s.LastIndexOf('=') + 1);
                    }
                    else if (s.Contains(I4))
                    {
                        lblPR_I4.Text = s.Substring(s.LastIndexOf('=') + 1);
                    }
                    else if (s.Contains(R1))
                    {
                        lblPR_R1.Text = s.Substring(s.LastIndexOf('=') + 1);
                    }
                    else if (s.Contains(SOL))
                    {
                        lblPR_SOL.Text = s.Substring(s.LastIndexOf('=') + 1);
                    }
                    else if (s.Contains(POS))
                    {
                        lblPR_POS.Text = s.Substring(s.LastIndexOf('=') + 1);
                    }
                    else if (s.Contains(CALVAR))
                    {
                        string tmp_calvar = s.Substring(s.LastIndexOf('=') + 1);
                        lblCAL_CALVAR.Text = tmp_calvar;

                        string[] calvar_params = tmp_calvar.Split(',');
                        if(calvar_params.Length > 0)
                        {
                            lblCAL_CURRENT_LENGTH.Text = calvar_params[0];
                            lblCAL_LENGTH_ADJ.Text     = calvar_params[1];
                            lblCAL_CURR_OFFSET.Text    = calvar_params[2];
                            lblCAL_CURR_VOLTS.Text     = calvar_params[3];
                            lblCAL_CAL_COEFF_0.Text    = calvar_params[4];
                            lblCAL_CAL_COEFF_1.Text    = calvar_params[5];
                            lblCAL_CAL_COEFF_2.Text    = calvar_params[6];
                            lblCAL_CLUB_COEFF_0.Text   = calvar_params[7];
                            lblCAL_CLUB_COEFF_1.Text   = calvar_params[8];
                            lblCAL_CLUB_COEFF_2.Text   = calvar_params[9];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                outputToLog($"[ERROR] timerCheckSerialQueue: {ex.Message}");
            }
            finally
            {
                timerCheckSerialQueue.Start();
            }
        }

        public void stopAndDisposeTimers()
        {
            if(timerIO != null)
            {
                timerIO.Stop();
                timerIO.Dispose();
            }
            
            if(timerPosition != null)
            {
                timerPosition.Stop();
                timerPosition.Dispose();
            }

            if(timerCheckSerialQueue != null)
            {
                timerCheckSerialQueue.Stop();
                timerCheckSerialQueue.Dispose();
            }
        }
    }
}
