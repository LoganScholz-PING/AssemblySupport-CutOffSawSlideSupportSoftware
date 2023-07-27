using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblySupport_CutOffSawSlideSupportSoftware
{
    public partial class MainForm
    {
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMOVE_LEFT_Click(object sender, EventArgs e)
        {
            outputStringToSerialPort("MV BCK");
        }

        private void btnMOVE_STOP_Click(object sender, EventArgs e)
        {
            outputStringToSerialPort("MV STOP");
        }

        private void btnMOVE_RIGHT_Click(object sender, EventArgs e)
        {
            outputStringToSerialPort("MV FWD");
        }

        private void btnMOVE_SET_R1_Click(object sender, EventArgs e)
        {
            if(double.TryParse(txtR1.Text, out double R1))
            {
                if ((R1 < 20f) || (R1 > 51f))
                {
                    outputToLog($"[ERROR] Invalid R1 value \"{R1}\" (20 <= R1 <= 51)");
                    return;
                }

                outputStringToSerialPort($"R1={txtR1.Text}");
            }
            else
            {
                outputToLog($"[ERROR] Not a valid number for R1 \"{txtR1.Text}\"");
            }
        }

        private void btnEXECUTE_M1_Click(object sender, EventArgs e)
        {
            outputStringToSerialPort("EX M1");
        }

        private void btnFreq_Click(object sender, EventArgs e)
        {
            if(int.TryParse(txtFreq.Text, out int freq))
            {
                if((freq < 0) || (freq > 8000))
                {
                    outputToLog($"[ERROR] Invalid freq value \"{freq}\" (0 <= freq <= 8000)");
                    return;
                }

                outputStringToSerialPort($"FREQ={freq}");
            }
            else
            {
                outputToLog($"[ERROR] Not a valid number for Freq \"{txtFreq.Text}\"");
            }
        }

        private void btnDuty_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtDuty.Text, out double duty))
            {
                if ((duty < 0f) || (duty > 1f))
                {
                    outputToLog($"[ERROR] Invalid Duty value \"{duty}\" (0.0 <= R1 <= 1.0)");
                    return;
                }

                outputStringToSerialPort($"DUTY={txtDuty.Text}");
            }
            else
            {
                outputToLog($"[ERROR] Not a valid number for Duty \"{txtDuty.Text}\"");
            }
        }

        private void btn_CAL_SET_SHORT_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtSHORT_LENGTH.Text, out double length))
            {
                outputStringToSerialPort($"SET_SHORT={length}");
            }
            else
            {
                outputToLog($"[ERROR] Not a valid number for short length \"{txtSHORT_LENGTH.Text}\"");
            }
        }

        private void btn_CAL_SET_LONG_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtLONG_LENGTH.Text, out double length))
            {
                outputStringToSerialPort($"SET_LONG={length}");
            }
            else
            {
                outputToLog($"[ERROR] Not a valid number for long length \"{txtLONG_LENGTH.Text}\"");
            }
        }

        private void btn_CAL_CALC_SHORT_Click(object sender, EventArgs e)
        {
            outputStringToSerialPort("CAL SHORT");
        }

        private void btn_CAL_CALC_LONG_Click(object sender, EventArgs e)
        {
            outputStringToSerialPort("CAL LONG");
        }

        private void btn_CAL_FINISH_Click(object sender, EventArgs e)
        {
            outputStringToSerialPort("CAL FINISH");
        }

        private void btnSEND_SERIAL_CUSTOM_COMMAND_Click(object sender, EventArgs e)
        {
            outputStringToSerialPort(txtSEND_SERIAL_CUSTOM_COMMAND.Text.Trim());
        }


        private void btnSERIAL_DISCONNECT_Click(object sender, EventArgs e)
        {
            serialDisconnect();
            updateSerialConnectionParams();
        }

        private void btnSERIAL_UPDATE_Click(object sender, EventArgs e)
        {
            updateCOMPorts();
        }

        private void btnSERIAL_CONNECT_Click(object sender, EventArgs e)
        {
            serialConnect();
            updateSerialConnectionParams();
        }

        private void stopAllTimersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stopAndDisposeTimers();
        }

        private void startAllTimersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            initTimers();
        }
    }
}
