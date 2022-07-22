using Memory;
using SimpleCheatTable.Elevate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SimpleCheatTable
{
    public partial class SimpleCheatTable : Form
    {
        string processName = "";
        Mem m = new Mem();

        public SimpleCheatTable()
        {
            InitializeComponent();
            sctFile.InitialDirectory = Directory.GetCurrentDirectory();
            this.dataGridView1.Columns[1].ReadOnly = true;
            this.dataGridView1.Columns[2].ReadOnly = true;
            this.dataGridView1.Columns[3].ReadOnly = true;

            if (!WindowsIdentity.GetCurrent().IsSystem)
            {
                string binaryPath = System.AppDomain.CurrentDomain.BaseDirectory + "SimpleCheatTable.exe";
                string ProcessToSpoof = "winlogon";
                int parentProcessId;
                Process[] explorerproc = Process.GetProcessesByName(ProcessToSpoof);
                parentProcessId = explorerproc[0].Id;
                RunUnderProcess.Start(parentProcessId, binaryPath);
                Thread.Sleep(1000);
                Environment.Exit(0);
            }


            if (sctFile.ShowDialog() == DialogResult.OK)
            {
                string[] linessct = File.ReadAllLines(sctFile.FileName);
                foreach (var item in linessct)
                {
                    if (item.ToLower().StartsWith("processname"))
                    {
                        processName = item.Split('=')[1].Trim();
                        m.OpenProcess(processName.Replace(".exe", ""));
                        continue;
                    }

                    var splitsctline = item.Split('=');

                    dataGridView1.Rows.Add(false, splitsctline[0], splitsctline[1], splitsctline[2], splitsctline[3]);
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                this.dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);

                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                if ((bool)this.dataGridView1.CurrentCell.Value == true)
                {
                    //Cheat Enable
                    m.FreezeValue(row.Cells[2].Value.ToString(), row.Cells[3].Value.ToString(), row.Cells[4].Value.ToString());
                }
                else
                {
                    //Cheat Disable
                    m.UnfreezeValue(row.Cells[2].Value.ToString());
                }
            }
        }

        private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.RowIndex >= 0)
            {
                this.dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                if ((bool)row.Cells[0].Value == true)
                {
                    m.UnfreezeValue(row.Cells[2].Value.ToString());
                    m.FreezeValue(row.Cells[2].Value.ToString(), row.Cells[3].Value.ToString(), row.Cells[4].Value.ToString());
                }
                else
                {
                    //m.WriteMemory(row.Cells[2].Value.ToString(), row.Cells[3].Value.ToString(), row.Cells[4].Value.ToString());
                }
            }
        }
    }
}
