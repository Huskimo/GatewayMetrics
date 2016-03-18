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
//using System.Collections.Generic;

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

            var fileCount = (from doc in Directory.EnumerateFiles(@"C:\\Users\\hasaan.ausat\\Desktop\\Logs", "*.log", SearchOption.AllDirectories)
                             select doc).Count();

            string[] dirs = Directory.GetFiles(@"C:\\Users\\hasaan.ausat\\Desktop\\Logs");

            //Saving the details' from each block of 1180 
            //List<string> tempBlockDetails = new List<string>();
            String tempBlockDetails = "";
            String tempDate = "";
            String tempTime = "";
            String tempId = "";
            String tempType = "";
            String tempDetails = "";

            for (int j = 0; j < fileCount; j++)
            {
                //reading in from text file
                System.IO.StreamReader file = new System.IO.StreamReader(dirs[j]);

                //getting line count of the text file
                int logLineCount = File.ReadLines(dirs[j]).Count();


                for (int i = 0; i < logLineCount; i++)
                {
                    //saving text from file to variable
                    string tempLogs = file.ReadLine();

                    //getting length of each line
                    int logLength = tempLogs.Length;
                    int lastChar = logLength - 63;

                    //assigning substrings to temp variables
                    tempDate = tempLogs.Substring(0, 10);
                    tempTime = tempLogs.Substring(11, 8);
                    tempType = tempLogs.Substring(34, 11);
                    tempId = tempLogs.Substring(54, 4);
                    tempDetails = tempLogs.Substring(63, lastChar);

                    if (tempId == "1180")
                    {

                        tempBlockDetails = (tempBlockDetails + tempDetails);
                        
                        // MessageBox.Show(tempBlockDetails);
                        //tempBlockDetails.Add(tempDetails);
                        //MessageBox.Show(tempDetails);
                    } else {
                        break;
                    }
                   
                }
                dataGridView1.Rows.Add(tempDate, tempTime, tempType, tempId, tempBlockDetails);
            }
        }
    }
}