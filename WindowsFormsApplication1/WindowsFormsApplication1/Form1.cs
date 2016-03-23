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
            public string timeOut { get; set; }
            public string size { get; set; }
            public string attach { get; set; }
            public string direction { get; set; }
            public string status { get; set; }
            public string type { get; set; }
            public string id { get; set; }
            public string details { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //file location on my Egress Laptop C:\\Users\\hasaan.ausat\\Desktop\\Logs
            var fileCount = (from doc in Directory.EnumerateFiles(@"D:\\Hasaan\\Documents\\GitHub\\GatewayMetrics\\Logs", "*.log", SearchOption.AllDirectories)
                             select doc).Count();

            string[] dirs = Directory.GetFiles(@"D:\\Hasaan\\Documents\\GitHub\\GatewayMetrics\\Logs");

       	    //assigning temp variables for each line
            String tempDate = "";
            String tempTime = "";
            String tempId = "";
            String tempType = "";
            String tempDetails = "";
            
            String tempBlockDetails = "";

            List<log> logList = new List<log>();
            
            List<log> concatDetails = new List<log>();
             

            //For every file do..
            for (int j = 0; j < fileCount; j++)
            {
                //reading in from text file
                System.IO.StreamReader file = new System.IO.StreamReader(dirs[j]);

                //getting line count of the text file
                int logLineCount = File.ReadLines(dirs[j]).Count();

                //On every line
                for (int i = 0; i < logLineCount; i++)
                {
                    //getting line from file to variable
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

                    //creating log object and pushing it onto the logList
                    log tempLogObject = new log { date = tempDate, time = tempTime, type = tempType, id = tempId, details = tempDetails };
                    logList.Insert(0, tempLogObject);
                    
                    //ensuring only specific ID's are searched for and displayed
                    if (tempId == "1180" || tempId == "1181" || tempId == "1112")
                    {
                        //check if ID is the same as the last
                        if (logList.Count == 1 || logList[0].id == logList[1].id)
                        {
                            //if so push on the list as objects are part of group
                            //concatDetails.Insert(0, tempLogObject);
                         
                        } else {
                            //if not new group created
                            concatDetails.Insert(0, tempLogObject);
                            //previous group has finished - concats all the details
                            for (int k = concatDetails.Count; k > 0; k--) {
                                tempBlockDetails = tempBlockDetails + concatDetails[k-1].details;                                
                            }
                            //display in the grid
                            dataGridView1.Rows.Add(concatDetails[0].date, concatDetails[0].time, concatDetails[0].type, concatDetails[0].id, tempBlockDetails);
                            //clear temp variables for next group
                            tempBlockDetails = "";
                            concatDetails.Clear();
                        }
                    }
                }
            }
        }
    }
}
