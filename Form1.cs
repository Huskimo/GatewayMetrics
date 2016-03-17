using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        class log
        {
            public string date { get; set; }
            public string time { get; set; }
            public string type { get; set; }
            public string id { get; set; }
            public string details { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //reading in from text file
            System.IO.StreamReader file = new System.IO.StreamReader("C:\\Users\\hasaan.ausat\\Desktop\\Gateway-20151226.txt");

            //getting line count of the text file
            int logLineCount = File.ReadLines(@"C:\\Users\\hasaan.ausat\\Desktop\\Gateway-20151226.txt").Count();

            for (int i = 0; i < logLineCount; i++)
            {

                //saving text from file to variable
                string tempLogs = file.ReadLine();

                //getting length of each line
                int logLength = tempLogs.Length;
                int lastChar = logLength - 63;

                //assigning substrings to temp variables
                string tempDate = tempLogs.Substring(0, 10);
                string tempTime = tempLogs.Substring(11, 8);
                string tempType = tempLogs.Substring(34, 11);
                string tempId = tempLogs.Substring(54, 4);
                string tempDetails = tempLogs.Substring(63, lastChar);

                dataGridView1.Rows.Add(tempDate, tempTime, tempType, tempId, tempDetails);
            }
        }
    }
}
