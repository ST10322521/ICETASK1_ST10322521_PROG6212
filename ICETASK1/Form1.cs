using System.Diagnostics;
using System.Reflection;

namespace ICETASK1
{
    public partial class Form1 : Form
    {
        int processId;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var runningProcs = from proc in Process.GetProcesses(".")
                               orderby proc.ProcessName
                               select proc;

            foreach (var p in runningProcs)
            {
                listBox1.Items.Add(string.Format("-> PID: {0} \t Name: {1} ", p.Id, p.ProcessName));
            }
        }

        //Broader exception handling implemented so program does not crash when chrome isnt found
        private void btnStartChrome_Click(object sender, EventArgs e)
        {
            Process chromeProc;
            try
            {
                ProcessStartInfo startInfor = new ProcessStartInfo("chrome.exe", "www.varsitycollege.co.za");
                startInfor.WindowStyle = ProcessWindowStyle.Maximized;
                chromeProc = Process.Start(startInfor);
                processId = chromeProc.Id;
                //Corrected typo in MessageBox: porcessId -> processId
                MessageBox.Show(processId.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnKillChrome_Click(object sender, EventArgs e)
        {
            Process[] chromeProc = Process.GetProcessesByName("chrome");
            try
            {
                foreach (Process p in chromeProc)
                {
                    p.Kill();
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEndTaskMng_Click(object sender, EventArgs e)
        {
            Process p = Process.GetCurrentProcess();
            try
            {
                p.Kill();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Dynamic PID extraction from selected item in listbox implemented, added error handling for no access to process
        private void btnThreads_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a process first.");
                return;
            }

            if (!tryFindPID(out int i))
            {
                MessageBox.Show("Invalid PID.");
                return;
            }

            Process theProc = null;
            try
            {
                theProc = Process.GetProcessById(i);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            if (theProc != null)
            {
                string info = "";
                try
                {
                    
                    ProcessThreadCollection theThreads = theProc.Threads;
                    foreach (ProcessThread pt in theThreads)
                    {
                        info += string.Format("-> Thread ID: {0}\tStart Time: {1}\tPriority: {2} \n", pt.Id, pt.StartTime.ToShortTimeString(), pt.PriorityLevel);
                    }
                }
                catch (Exception ex)
                {
                    info += "Could not access thread information: " + ex.Message;
                }
                MessageBox.Show(info);
            }
        }

        // Error handling for access denied implemented, dynamic PID extraction
        private void btnLoadedModules_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a process first.");
                return;
            }

            if (!tryFindPID(out int i))
            {
                MessageBox.Show("Invalid PID.");
                return;
            }

            // Typo corrected: MessageBow -> MessageBox, changed newId to i to allow for new method added
            MessageBox.Show("'" + i + "'");

            Process theProc = null;
            try
            {
                theProc = Process.GetProcessById(i);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }

            string info = "";

            try
            {
                ProcessModuleCollection theMods = theProc.Modules;
                foreach (ProcessModule pm in theMods)
                {
                    info += string.Format("-> Module Name: {0} \n", pm.ModuleName);
                }
                MessageBox.Show(info);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                MessageBox.Show("Access denied to some modules: " + ex.Message);
            }
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            AppDomain defaultAD = AppDomain.CurrentDomain;

            string output = "";
            output += "Name of this domain: " + defaultAD.FriendlyName + "\n";
            output += "ID of domain in this process: " + defaultAD.Id + "\n";
            output += "Is this the default domain? " + defaultAD.IsDefaultAppDomain() + "\n";
            output += "Base directory of this domain: " + defaultAD.BaseDirectory;

            MessageBox.Show(output);
        }

        private void btnAssemblies_Click(object sender, EventArgs e)
        {
            AppDomain defaultAD = AppDomain.CurrentDomain;

            Assembly[] loadedAssemblies = defaultAD.GetAssemblies();
            string output = "Assemblies loaded in " + defaultAD.FriendlyName;

            foreach (Assembly a in loadedAssemblies)
            {
                output += "-> Name: " + a.GetName().Name + "\n";
                output += "-> Version: " + a.GetName().Version + "\n";
            }
            MessageBox.Show(output);

        }

        // Method to try extract PID from selected listbox item, avoid the assumption that PID is are 5 characters and shorter than int32 max value
        private bool tryFindPID(out int pid)
        {
            pid = 0;
            string id = listBox1.SelectedItem.ToString();
            string newId = id.Split('\t')[0].Replace("-> PID: ", "").Trim();
            return int.TryParse(newId, out pid);
        }
    }
}
