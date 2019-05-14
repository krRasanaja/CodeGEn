using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Test.Models;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #region getInstanceInSQLServer

        private void Form1_Load(object sender, EventArgs e)
        {
            string myServer = Environment.MachineName;

            List<string> serverList = new List<string>();

            DataTable servers = SqlDataSourceEnumerator.Instance.GetDataSources();
            for (int i = 0; i < servers.Rows.Count; i++)
            {
                //{
                //{
                if ((servers.Rows[i]["InstanceName"] as string) != null)
                {
                    serverList.Add(servers.Rows[i]["ServerName"] + "\\" + servers.Rows[i]["InstanceName"]);
                    cmbServers.Items.Add(servers.Rows[i]["ServerName"] + "\\" + servers.Rows[i]["InstanceName"]);
                }
                else
                {
                    serverList.Add(servers.Rows[i]["ServerName"].ToString());
                    cmbServers.Items.Add(servers.Rows[i]["ServerName"]);
                }

                //}
            }

            cbDataBases.Text = "--Select--";
            cbDataBases.Enabled = false;
            cbTables.Enabled = false;
        }

        #endregion getInstanceInSQLServer

        #region dataBaseList

        public List<string> GetDatabaseList()
        {
            List<string> list = new List<string>();

            string server = cmbServers.Text;
            string userName = txtUserName.Text.Trim();
            string password = txtPassword.Text;

            // Open connection to the database
            string conString = "server=" + server + ";uid=" + userName + ";pwd=" + password + ";";

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                // Set up a command with the given query and associate
                // this with the current connection.
                using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(dr[0].ToString());
                        }
                    }
                }
            }

            return list;
        }

        #endregion dataBaseList

        #region TableBaseList

        public List<string> GetTableList()
        {
            List<string> tables = new List<string>();

            string server = cmbServers.Text;
            string userName = txtUserName.Text.Trim();
            string password = txtPassword.Text;
            string databse = cbDataBases.Text;

            // Open connection to the database
            string conString = "server=" + server + ";uid=" + userName + ";pwd=" + password + ";Database=" + databse;

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                DataTable t = con.GetSchema("Tables");
                //List<DataRow> list = t.AsEnumerable().ToList();

                //foreach (DataRow obj in list)
                //{
                //    tables.Add(list[2].ToString());
                //    cbTables.Items.Add(list[2].ToString());
                //}
                cbTables.Items.Clear();

                foreach (DataRow obj in t.Rows)
                {
                    cbTables.Items.Add(obj["TABLE_NAME"].ToString());
                }
            }

            cbTables.Enabled = true;

            return tables;
        }

        #endregion TableBaseList

        #region getColumnDetails

        public List<TabelsDetails> getColumnDetails()
        {
            string server = cmbServers.Text;
            string userName = txtUserName.Text.Trim();
            string password = txtPassword.Text;
            string databse = cbDataBases.Text;
            string table = cbTables.Text;

            List<TabelsDetails> coloumnsDetails = new List<TabelsDetails>();

            // Open connection to the database
            string conString = "server=" + server + ";uid=" + userName + ";pwd=" + password + ";Database=" + databse;

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + table + "'", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            TabelsDetails OtabelsDetails = new TabelsDetails();
                            OtabelsDetails.COLUMN_NAME = dr[3].ToString();
                            OtabelsDetails.DATA_TYPE = dr[7].ToString();
                            OtabelsDetails.IS_NULLABLE = dr[6].ToString();

                            coloumnsDetails.Add(OtabelsDetails);
                        }
                    }
                }
            }
            dataGridView1.DataSource = coloumnsDetails;
            return coloumnsDetails;
        }

        #endregion getColumnDetails

        #region GenarateTextFile

        public void genarateTextFiles()
        {
            List<TabelsDetails> tableData = getColumnDetails();
            string location = textBox1.Text;
            string Name = cbTables.Text + "DTO";
            string fileName = location + "\\" + Name + ".cs";
            try
            {
                // Check if file already exists. If yes, delete it.
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Create a new file
                using (FileStream fs = File.Create(fileName))
                {
                    // Add some text to file
                    Byte[] title = new UTF8Encoding(true).GetBytes(Name);
                    fs.Write(title, 0, title.Length);
                    byte[] author = new UTF8Encoding(true).GetBytes("Kalindu Rasanjana");
                    fs.Write(author, 0, author.Length);
                }

                using (StreamWriter sw = File.CreateText(fileName))
                {
                    sw.WriteLine("// Code Genarated With CodeGen  " + Name + " created: {0}", DateTime.Now.ToString());
                    sw.WriteLine("using System;");
                    sw.WriteLine("");
                    sw.WriteLine("namespace " + textBox2.Text);
                    sw.WriteLine("{");
                    sw.WriteLine("\tpublic class " + cbTables.Text + "DTO");
                    sw.WriteLine("\t{");

                    foreach (TabelsDetails obj in tableData)
                    {
                        switch (obj.DATA_TYPE.ToLower())
                        {
                            case "varchar":
                                sw.WriteLine("\t\t public string " + obj.COLUMN_NAME + " { get; set;}");
                                sw.WriteLine("");
                                break;

                            case "nvarchar":
                                sw.WriteLine("\t\t public string " + obj.COLUMN_NAME + " { get; set;}");
                                sw.WriteLine("");
                                break;

                            case "datetime":
                                sw.WriteLine("\t\t public DateTime  " + obj.COLUMN_NAME + " { get; set;}");
                                sw.WriteLine("");
                                break;

                            case "decimal":
                                sw.WriteLine("\t\t public decimal  " + obj.COLUMN_NAME + " { get; set;}");
                                sw.WriteLine("");
                                break;

                            case "int":
                                sw.WriteLine("\t\t public int  " + obj.COLUMN_NAME + " { get; set;}");
                                sw.WriteLine("");
                                break;
                            default:
                                sw.WriteLine("\t\t public enterCorrectDataType  " + obj.COLUMN_NAME + " { get; set;}");
                                sw.WriteLine("");
                                break;
                        }
                    }
                    sw.WriteLine("\t} ");
                    sw.WriteLine("} ");
                }

                MessageBox.Show("DTO File Created");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void genarateTextFilesBL()
        {
            List<TabelsDetails> tableData = getColumnDetails();
            string location = textBox1.Text;
            string Name = cbTables.Text + "BL";
            string fileName = location + "\\" + Name + ".cs";
            try
            {
                // Check if file already exists. If yes, delete it.
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Create a new file
                using (FileStream fs = File.Create(fileName))
                {
                    // Add some text to file
                    Byte[] title = new UTF8Encoding(true).GetBytes(Name);
                    fs.Write(title, 0, title.Length);
                    byte[] author = new UTF8Encoding(true).GetBytes("Kalindu Rasanjana");
                    fs.Write(author, 0, author.Length);
                }
                
                using (StreamWriter sw = File.CreateText(fileName))
                {
                    sw.WriteLine("// Code Genarated With CodeGen  " + Name + " created: {0}", DateTime.Now.ToString());
                    sw.WriteLine("");
                    sw.WriteLine("using System;");
                    sw.WriteLine("using System.Collections.Generic;");
                    sw.WriteLine("using System.Data.SqlClient;");
                    sw.WriteLine("");
                    sw.WriteLine("namespace " + textBox2.Text);
                    sw.WriteLine("{");
                    sw.WriteLine("\tpublic class " + cbTables.Text+"BL");
                    sw.WriteLine("\t{");
                    sw.WriteLine("");
                    sw.WriteLine("\t\tprivate readonly string conString;");
                    sw.WriteLine("");

                    #region Insert

                    sw.WriteLine("");
                    sw.WriteLine("\t\t#region Insert");
                    sw.WriteLine("");
                    sw.WriteLine("\t\tpublic void insert" + cbTables.Text + "(" + cbTables.Text + "DTO  o" + cbTables.Text + "DTO)");
                    sw.WriteLine("\t\t{");
                    sw.WriteLine("\t\t\ttry");
                    sw.WriteLine("\t\t\t{");

                    
                    sw.WriteLine("\t\t\t\tusing (SqlConnection con = new SqlConnection(conString))");
                    sw.WriteLine("\t\t\t\t{");
                    sw.WriteLine("\t\t\t\t\tStringBuilder sb = new StringBuilder();");
                    sw.WriteLine("");
                    sw.WriteLine("\t\t\t\t\tsb.AppendLine = (\"Insert Into " + cbTables.Text + " Values( \");");
                    for (int i = 0; i < tableData.Count; i++)
                    {
                        if(i != tableData.Count-1)
                        {
                            sw.WriteLine("\t\t\t\t\tsb.AppendLine = (\" @" + tableData[i].COLUMN_NAME + ", \");");
                        }
                        else
                        {
                            sw.WriteLine("\t\t\t\t\tsb.AppendLine = (\" @" + tableData[i].COLUMN_NAME + " )\");");

                        }
                    }
                    sw.WriteLine("");
                    sw.WriteLine("\t\t\t\t\tSqlCommand cmd = new SqlCommand(sb.ToString(), con);");
                    foreach (TabelsDetails obj in tableData)
                    {
                        sw.WriteLine("\t\t\t\t\tif (" + cbTables.Text+"DTO."+obj.COLUMN_NAME+".Equals(null))");
                        sw.WriteLine("\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + obj.COLUMN_NAME+"\", DBNull.Value);");

                        sw.WriteLine("\t\t\t\t\telse");
                        sw.WriteLine("\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + obj.COLUMN_NAME + "\", " + cbTables.Text + "DTO." + obj.COLUMN_NAME + ");");
                        sw.WriteLine("");
                    }
                    sw.WriteLine("");
                    sw.WriteLine("\t\t\t\t\tcon.open();");
                    sw.WriteLine("\t\t\t\t\tcmd.ExecuteNonQuery();");
                    sw.WriteLine("\t\t\t\t}");
                    sw.WriteLine("\t\t\t}");
                    sw.WriteLine("\t\t\tcatch (Exception ex)");
                    sw.WriteLine("\t\t\t{");
                    sw.WriteLine("\t\t\t\tthrow ex;");
                    sw.WriteLine("\t\t\t}");

                    sw.WriteLine("\t\t}");
                    sw.WriteLine("");
                    sw.WriteLine("\t\t#endregion Insert");
                    sw.WriteLine("");

                    #endregion Insert

                    #region Update

                    sw.WriteLine("");
                    sw.WriteLine("\t\t#region Update");
                    sw.WriteLine("");
                    sw.WriteLine("\t\tpublic void Update" + cbTables.Text + "(" + cbTables.Text + "DTO  o" + cbTables.Text + "DTO)");
                    sw.WriteLine("\t\t{");
                    sw.WriteLine("\t\t\ttry");
                    sw.WriteLine("\t\t\t{");


                    sw.WriteLine("\t\t\t\tusing (SqlConnection con = new SqlConnection(conString))");
                    sw.WriteLine("\t\t\t\t{");
                    sw.WriteLine("\t\t\t\t\tStringBuilder sb = new StringBuilder();");
                    sw.WriteLine("");
                    sw.WriteLine("\t\t\t\t\tsb.AppendLine = (\"UPDATE "+cbTables.Text+" set \");");
                    for (int i = 0; i < tableData.Count; i++)
                    {
                        if (i != tableData.Count - 1)
                        {
                            sw.WriteLine("\t\t\t\t\tsb.AppendLine = (\" " + tableData[i].COLUMN_NAME + " = @"+ tableData[i].COLUMN_NAME + " , \");");
                        }
                        else
                        {
                            sw.WriteLine("\t\t\t\t\tsb.AppendLine = (\" " + tableData[i].COLUMN_NAME + " = @" + tableData[i].COLUMN_NAME + "  \");");

                        }
                    }
                    sw.WriteLine("");
                    sw.WriteLine("\t\t\t\t\tSqlCommand cmd = new SqlCommand(sb.ToString(), con);");
                    foreach (TabelsDetails obj in tableData)
                    {
                        sw.WriteLine("\t\t\t\t\tif (" + cbTables.Text + "DTO." + obj.COLUMN_NAME + ".Equals(null))");
                        sw.WriteLine("\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + obj.COLUMN_NAME + "\", DBNull.Value);");

                        sw.WriteLine("\t\t\t\t\telse");
                        sw.WriteLine("\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + obj.COLUMN_NAME + "\", " + cbTables.Text + "DTO." + obj.COLUMN_NAME + ");");
                        sw.WriteLine("");
                    }
                    sw.WriteLine("");
                    sw.WriteLine("\t\t\t\t\tcon.open();");
                    sw.WriteLine("\t\t\t\t\tcmd.ExecuteNonQuery();");
                    sw.WriteLine("\t\t\t\t}");
                    sw.WriteLine("\t\t\t}");
                    sw.WriteLine("\t\t\tcatch (Exception ex)");
                    sw.WriteLine("\t\t\t{");
                    sw.WriteLine("\t\t\t\tthrow ex;");
                    sw.WriteLine("\t\t\t}");

                    sw.WriteLine("\t\t}");
                    sw.WriteLine("");
                    sw.WriteLine("\t\t#endregion Update");
                    sw.WriteLine("");

                    #endregion Update

                    sw.WriteLine("\t} ");
                    sw.WriteLine("} ");
                }

                MessageBox.Show("BL File Created");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion GenarateTextFile

        #region Events

        private void btnGetDataBases_Click(object sender, EventArgs e)
        {
            if (txtPassword.Equals(string.Empty) || txtUserName.Equals(string.Empty))
            {
                MessageBox.Show("Enter UserName And Password");
            }
            else
            {
                List<string> databases = GetDatabaseList();
                if (databases.Count > 0)
                {
                    cbDataBases.DataSource = GetDatabaseList();
                    cbDataBases.Enabled = true;
                }
            }
        }

        private void cbDataBases_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetTableList();
        }

        private void btnBrows_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            // Show the FolderBrowserDialog.
            DialogResult result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = folderDlg.SelectedPath;
                Environment.SpecialFolder root = folderDlg.RootFolder;
            }
        }

        private void cbTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            getColumnDetails();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (!textBox1.Text.Equals(string.Empty) || textBox2.Text.Equals(string.Empty))
            {
                genarateTextFiles();
            }
            else
            {
                MessageBox.Show("Enter All Entries");
            }
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (!textBox1.Text.Equals(string.Empty) || textBox2.Text.Equals(string.Empty))
            {
                genarateTextFilesBL();

            }
            else
            {
                MessageBox.Show("Enter All Entries");
            }

        }

        #endregion Events


    }
}