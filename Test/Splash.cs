using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    public partial class Splash : Form
    {
        int count = 1;
        public Splash()
        {
            InitializeComponent();
            
        }

        private Thread workerThread = null;

        private void Splash_Load(object sender, EventArgs e)
        {
            workerThread = new Thread(LoadList);
            workerThread.Start();
            timer1.Interval = 100;
            timer1.Start();
        }
        
        public void LoadList()
        {
            string myServer = Environment.MachineName;



            DataTable servers = SqlDataSourceEnumerator.Instance.GetDataSources();
            for (int i = 0; i < servers.Rows.Count; i++)
            {
                if ((servers.Rows[i]["InstanceName"] as string) != null)
                {
                    GlobalData.serverList.Add(servers.Rows[i]["ServerName"] + "\\" + servers.Rows[i]["InstanceName"]);
                    
                }
                else
                {
                    GlobalData.serverList.Add(servers.Rows[i]["ServerName"].ToString());
                    
                }
            }
            Thread.Sleep(5000);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (workerThread == null)
            {

                timer1.Stop();
                
                return;
            }

            if (workerThread.IsAlive)
            {
                
                bunifuProgressBar1.Value = count;

                count++;

                return;
            }
                
            timer1.Stop();

            this.Hide();
            
            Form1 form1 = new Form1();
            form1.ShowDialog();
            
            workerThread = null;
            
        }
        
    }
}
