using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSES_UMU4_EXPORT_DATA
{
    public partial class Main : Form
    {
        ConnectDB ConDB = new ConnectDB();
        string Server, DBNane, User, PW;
        string TargetPaht, DestPath, FCT2;
        public Main()
        {
            InitializeComponent();
            TextBox.CheckForIllegalCrossThreadCalls = false;
            AppSetting();
        }
        private void Main_Load(object sender, EventArgs e)
        {
            Int32 TimeInt = 1000;
            timerSQL.Interval = TimeInt;
            timerSQL.Tick += new EventHandler(timerSQL_Tick);

            timerSQL.Enabled = true;
            lbResult.Text = "Application Start...";
            lbResult.BackColor = Color.Red;
        }
        public void PrintResult(string RText)
        {
            string dateLog = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                rTBResult.AppendText(dateLog + "----> " + RText);
                //rTBResult.Text += RText;
                rTBResult.AppendText(Environment.NewLine);
            }
            catch
            {
                rTBResult.Text = "-";
            }
        }
        public void logError(string Texts)
        {
            string pathApp = Application.StartupPath;
            string Date = DateTime.Now.ToString("yyyyMMdd");
            string timeStemp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //Clipboard.SetDataObject(Date +"---->"+ Texts);

            string subdir = pathApp + "\\Log_Error\\" + Date;

            if (!Directory.Exists(subdir))
            {
                Directory.CreateDirectory(subdir);
            }
            try
            {
                File.AppendAllText(subdir + "\\" + "Error_" + Date + ".txt", timeStemp + "--->" + Texts + Environment.NewLine);
            }
            catch
            {
                PrintResult("Can't export file.");
            }
        }
        public void ReadFile()
        {
            string dir = TargetPaht;
            try
            {
                rTBResult.Clear();
                if (Directory.Exists(dir))
                {
                    string[] fileEntries = Directory.GetFiles(dir, "*.txt");
                    int vTotalFile = fileEntries.Length;

                    if (vTotalFile > 0)
                    {
                       // foreach (string files in fileEntries)
                        //{
                        string vFName = fileEntries[0].Substring(dir.Length);
                        string[] values = vFName.Split('_');
                        string vModel = values[0];
                        string vSerial = values[1];

                        string vResult = ExpoerSerialFile(vModel, vSerial);

                        PrintResult(vSerial + " Result:" + vResult);

                        if (vResult == "1")
                        {
                            try
                            {
                                File.Delete(Path.Combine(dir, vFName));
                                PrintResult("export: Success ");
                            }
                            catch (Exception e)
                            {
                                logError(e.ToString());
                            }
                        }
                        //}
                    }
                    else
                    {
                        lbResult.Text = "Waiting File...";
                        lbResult.BackColor = Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                logError(ex.ToString());
            }
        }
        public string ExpoerSerialFile(string Model, string Serial)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string PathKENSA = appSettings.Get("DestPath");

            try
            {
                string Date = DateTime.Now.ToString("yyyyMMddHHmmss");
                string vSerial = Serial;
                DataTable vSerialData = ConDB.STBL_GET_ALL_DATA_EXPORT(vSerial);
                string vFileName = PathKENSA + Model + "_" + vSerial + "_" + Date + ".csv";

                WriteToCsvFile(vSerialData, vFileName);
                return "1";
            }
            catch (Exception e)
            {
                string msg = e.Message;
                logError(Serial + "-->" +msg.ToString());
                return "0";
            }
        }
        public void WriteToCsvFile(DataTable dataTable, string filePath)
        {
            StringBuilder fileContent = new StringBuilder();

            //foreach (var col in dataTable.Columns)
            //{
            //    fileContent.Append(col.ToString() + ",");
            //}

            //fileContent.Replace(",", Environment.NewLine, fileContent.Length - 1, 1);

            foreach (DataRow dr in dataTable.Rows)
            {
                foreach (var column in dr.ItemArray)
                {
                    fileContent.Append(column.ToString() + ",");
                }

                fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);
            }

            File.WriteAllText(filePath, fileContent.ToString());
        }
        private void AppSetting()
        {
            var appSettings = ConfigurationManager.AppSettings;
            Server = appSettings.Get("Server");
            DBNane = appSettings.Get("DBNane");
            User = appSettings.Get("User");
            PW = appSettings.Get("PW");
            TargetPaht = appSettings.Get("TargetPaht");
            DestPath = appSettings.Get("DestPath");

            tbServer.Text = Server;
            tbDBName.Text = DBNane;
            tbUser.Text = User;
            tbPassword.Text = PW;

            tbTargetPath.Text = TargetPaht;
            tbDestPath.Text = DestPath;
        }
        private void timerSQL_Tick(object sender, EventArgs e)
        {
            string oDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            lbLastDate.Text = oDate;

            if (bgWReadFile.IsBusy != true)
            {
                bgWReadFile.RunWorkerAsync();
            }
        }
        private void bgWReadFile_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            PrintResult("Finish..");
        }
        private void bgWReadFile_DoWork(object sender, DoWorkEventArgs e)
        {
            ReadFile();
        }
        private void saveConfig()
        {
            string Server = tbServer.Text;
            string DBName = tbDBName.Text;
            string User = tbUser.Text;
            string Password = tbPassword.Text;
            string TargetPath = tbTargetPath.Text;
            string DestPath = tbDestPath.Text;

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            config.AppSettings.Settings.Remove("Server");
            config.AppSettings.Settings.Add("Server", Server);
            config.AppSettings.Settings.Remove("DBNane");
            config.AppSettings.Settings.Add("DBNane", DBName);
            config.AppSettings.Settings.Remove("User");
            config.AppSettings.Settings.Add("User", User);
            config.AppSettings.Settings.Remove("PW");
            config.AppSettings.Settings.Add("PW", Password);
            config.AppSettings.Settings.Remove("TargetPath");
            config.AppSettings.Settings.Add("TargetPath", TargetPath);
            config.AppSettings.Settings.Remove("DestPath");
            config.AppSettings.Settings.Add("DestPath", DestPath);

            config.Save(ConfigurationSaveMode.Minimal);
            AppSetting();
            Application.Restart();
            Environment.Exit(0);
        }
        private void btSettingSave_Click(object sender, EventArgs e)
        {
            saveConfig();
        }
        private void btStart_Click(object sender, EventArgs e)
        {
            this.timerSQL.Enabled = true;
        }
        private void btStop_Click(object sender, EventArgs e)
        {
            this.timerSQL.Enabled = false;
        }
        private void btExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void btSettingSet_Click(object sender, EventArgs e)
        {
            tbServer.Enabled = true;
            tbDBName.Enabled = true;
            tbUser.Enabled = true;
            tbPassword.Enabled = true;
            tbDestPath.Enabled = true;
            tbTargetPath.Enabled = true;
        }
    }
}
