using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace pingList
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        object obj;
        private void btnExec_Click(object sender, EventArgs e)
        {
            StringBuilder msg = new StringBuilder();
            StringBuilder ping = new StringBuilder();
            StringBuilder ipsOks = new StringBuilder();
            StringBuilder ipsNOks = new StringBuilder();
            StringBuilder ipsOffs = new StringBuilder();
            DateTime dtInicio = DateTime.Now;
            int countOff = 0;
            int countON = 0;
            int countCut = 0;
            int countTT = 0;
            if (obj == null)
            {
                obj = new object();
                btnExec.ResetText();
                btnExec.Text = "Executando...";
                btnExec.Refresh();
            }
            try
            {
                lock (obj)
                {
                    dtInicio = DateTime.Now;
                    int res = lbIps.Lines.Length;
                    foreach (var item in lbIps.Lines)
                    {

                        btnExec.Enabled = false;
                        lbIps.Enabled = false;
                        txtNmReport.Enabled = false;
                        lbProgress.ResetText();
                        lbProgress.Text = "Executando ip " + item + "...";
                        lbProgress.Refresh();
                        lblRes.ResetText();
                        lblRes.Text = "Restam " + (--res) + " IPs...";
                        lblRes.Refresh();
                        ping.AppendLine($"Inicio {DateTime.Now} - {lbProgress.Text}");
                        ProcessStartInfo ProcessInfo = new ProcessStartInfo("cmd.exe", @" /c ping " + item);
                        ProcessInfo.UseShellExecute = false;
                        ProcessInfo.RedirectStandardOutput = true;
                        ProcessInfo.RedirectStandardError = true;
                        ProcessInfo.CreateNoWindow = true;

                        Process p1 = Process.Start(ProcessInfo);

                        if (p1.StartInfo.RedirectStandardOutput)
                        {
                            string msgPing = p1.StandardOutput.ReadToEnd();

                            if (msgPing.Contains("bytes") && msgPing.Contains("TTL"))
                            {
                                var splt = msgPing.Split('=').Count(s => s.Contains("bytes"));

                                if (splt >= 4)
                                {
                                    ipsOks.AppendLine(item);
                                }
                                if (splt < 4 && splt >= 1)
                                {
                                    countCut++;
                                    ipsNOks.AppendLine(item);
                                }
                                countON++;
                            }
                            else
                            {
                                countOff++;
                                ipsOffs.AppendLine(item);
                            }


                            ping.AppendLine(msgPing);
                        }
                        countTT++;
                        if (p1.StartInfo.RedirectStandardError)
                        {
                            StreamReader myStreamReader = p1.StandardError;
                            string strMg = myStreamReader.ReadLine();
                            if (!string.IsNullOrWhiteSpace(strMg))
                                MessageBox.Show("RedirectStandardError" + strMg, "Saida");
                        }
                        p1.WaitForExit();
                        p1.Close();
                    }
                    msg.AppendLine("**********************************************************************************");
                    msg.AppendLine("***data ini:" + dtInicio.ToString()+"***\r\n");
                    msg.AppendLine("***data fim:" + DateTime.Now.ToString()+"*** \r\n") ;
                    msg.AppendLine("***QUANTIDADE DE IPS OFF=" + countOff);
                    msg.AppendLine("***QUANTIDADE DE IPS ON=" + countON);
                    msg.AppendLine("***QUANTIDADE DE IPS ESTAVEIS=" + (countON - countCut));
                    msg.AppendLine("***QUANTIDADE DE IPS INSTAVEIS=" + countCut);
                    msg.AppendLine("***QUANTIDADE TOTAL DE IPS=" + countTT);
                    msg.AppendLine("**********************************************************************************");
                    msg.AppendLine("***IPs OK***\r\n"+ipsOks.ToString());
                    msg.AppendLine("***IPs instáveis***\r\n"+ipsNOks.ToString());
                    msg.AppendLine("***IPs Off***\r\n"+ipsOffs.ToString());
                    msg.AppendLine("**********************************************************************************");
                    msg.AppendLine("***Relatorio de comando PING***\r\n" + ping.ToString());
                    msg.AppendLine("**********************************************************************************");
                    if (!string.IsNullOrWhiteSpace(txtNmReport.Text))
                    {
                        File.WriteAllText(txtNmReport.Text + ".txt", msg.ToString());
                        MessageBox.Show($" arquivo {txtNmReport.Text}.txt gravado com sucesso!!");
                    }
                    else
                    {
                        File.WriteAllText("saidaÌPs.txt", msg.ToString());
                        MessageBox.Show(" arquivo saidaÌPs.txt gravado com sucesso!!");
                    }
                }
                lbProgress.Text = "                          ...";
                btnExec.Enabled = true;
                lbIps.Enabled = true;
                txtNmReport.Enabled = true;
                btnExec.Text = "Executar";
                lblRes.Text = "...";            }
            catch (Exception ex)
            {
                msg.AppendLine("*****\\" + ex.StackTrace.ToString());
                MessageBox.Show(msg.ToString(), "Erro");
            }

        }

        private void lbIps_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
