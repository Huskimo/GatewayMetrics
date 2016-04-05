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
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;


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
            public string label { get; set; }
            public string direction { get; set; }
            public string status { get; set; }
            public string emailId { get; set; }
            public string type { get; set; }
            public string id { get; set; }
            public string details { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //initating regex
            Regex rSize = new Regex(@"(?<=Size=\[)[0-9]*(?=\])");
            Regex rAttach = new Regex(@"(?<=Attachments=\[)[a-zA-Z0-9]*(?=\])");
            Regex rDir = new Regex(@"(Inbound|Outbound)");
            Regex rLabel = new Regex(@"(?<=X-\$Switch=\[)[a-zA-Z]*(?=\])");
            Regex rEmailId = new Regex(@"(?<=\[)[a-zA-Z0-9]+(?=\.eml)");
            //Regex rStatus = new Regex(@"Size =\[[0-9]+\]");

            //clears table at beginning to allow for any changes in the file while running (i.e acts as a refresh button)
            dataGridView1.Rows.Clear();

            //get file path from user
            String x = Interaction.InputBox("Please enter the file location of the logs", "File Location", "D:\\Hasaan\\Desktop\\Logs", -1, -1);
           
            //ensure file path exists, if not display error message
            if (!Directory.Exists(x))
            {
                MessageBox.Show("The path '"+x+"' does not exist. Please try again.", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                importButton.Text = "Import Text Files";
            }
            else {

                //file location on my Egress Laptop C:\\Users\\hasaan.ausat\\Desktop\\Logs C:\\Program Files\\Egress\\SDX\\logs
                var fileCount = (from doc in Directory.EnumerateFiles(@x, "*.log", SearchOption.AllDirectories)
                                 select doc).Count();
                //display message if folder is empty or continue loading files in to program
                if (fileCount == 0)
                {
                    MessageBox.Show("There are no log files to import", null, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    importButton.Text = "Import Text Files";
                }
                else {

                    //changes text on button
                    importButton.Text = "Refresh";

                    string[] dirs = Directory.GetFiles(@x);

                    //assigning temp variables for each line
                    String tempDate = "";
                    String tempTime = "";
                    String tempTimeOut = "";
                    String tempSize = "";
                    String tempAttach = "";
                    String tempLabel = "";
                    String tempDir = "";
                    String tempStatus = "";
                    String tempId = "";
                    String tempEmailId = "";
                    String tempType = "";
                    String tempDetails = "";

                    String tempBlockDetails = "";

                    List<String> fullDetails = new List<String>();

                    //List<String> allDetails = new List<String>();

                    List<log> logList = new List<log>();

                    List<log> concatDetails = new List<log>();

                    List<log> logList2 = new List<log>();

                    //For every file do..
                    for (int j = 0; j < fileCount; j++)
                    {
                        //reading in from text file
                        StreamReader file = new StreamReader(dirs[j]);

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
                            log tempLogObject = new log { date = tempDate, time = tempTime, type = tempType, id = tempId, details = tempDetails};
                            logList.Insert(0, tempLogObject);

                            //ensuring only specific ID's are searched for and displayed
                            if (tempId == "1180" /*|| tempId == "1112" || tempId =="1113"*/)
                            {
                                //check if ID is the same as the last
                                if (logList.Count == 1 || logList[0].id == logList[1].id)
                                {
                                    //if so push on the list as objects are part of group
                                    concatDetails.Insert(0, tempLogObject);
                                }
                                else {
                                    //if not new group created
                                    concatDetails.Insert(0, tempLogObject);
                                    //previous group has finished - concats all the details
                                    for (int k = concatDetails.Count; k > 0; k--)
                                    {
                                        tempBlockDetails = tempBlockDetails + concatDetails[k - 1].details;
                                    }
                                    fullDetails.Insert(0, tempBlockDetails);

                                    //apply regex tests and assign output to temp variables
                                  
                                    tempSize = rSize.Match(fullDetails[0]).ToString();
                                    if (rAttach.Match(fullDetails[0]).ToString() == "")
                                    {
                                        tempAttach = "No";
                                    }
                                    else {
                                        tempAttach = "Yes";
                                    }
                                    tempLabel = rLabel.Match(fullDetails[0]).ToString();
                                    tempDir = rDir.Match(fullDetails[0]).ToString();
                                    //tempStatus = rStatus.Match(fullDetails[0]).ToString();

                                    log tempLogObject2 = new log {timeOut = tempTimeOut, size = tempSize, attach = tempAttach, label = tempLabel, direction = tempDir, status = tempStatus };
                                    logList2.Insert(0, tempLogObject2);

                                    tempEmailId = rEmailId.Match(fullDetails[0]).ToString();
                                   
                                    //spliting date and time and performing a subtraction in order to find difference
                                    char splitChar = ':';
                                    char splitChar2 = '-';
                                    String text = concatDetails[0].time;
                                    String text2 = concatDetails[0].date;
                                    String[] times = text.Split(splitChar);
                                    String[] dates = text2.Split(splitChar2);
                                    System.DateTime date1 = new System.DateTime(Convert.ToInt32(dates[0]), Convert.ToInt32(dates[1]), Convert.ToInt32(dates[2]), Convert.ToInt32(times[0]), Convert.ToInt32(times[1]), Convert.ToInt32(times[2]));
                                    System.DateTime time2 = new System.DateTime(Convert.ToInt32(dates[0]), Convert.ToInt32(dates[1]), Convert.ToInt32(dates[2]), 14, 56, 50);
                                    TimeSpan diff = time2.Subtract(date1);
                                    String text3 = diff.ToString();
                                    String[] times2 = text3.Split(splitChar);

                                    //converting difference output into seconds
                                    TimeSpan interval = new TimeSpan(0, Convert.ToInt32(times2[0]), Convert.ToInt32(times2[1]), Convert.ToInt32(times2[2]),0);
                                    String y = interval.TotalSeconds.ToString();

                                    //while (fullDetails.Count > 1) {
                                    //display in the grid
                                    dataGridView1.Rows.Add(concatDetails[0].date, concatDetails[0].time, null, y, logList2[0].size, logList2[0].attach, logList2[0].label, logList2[0].direction, concatDetails[0].type, concatDetails[0].id, fullDetails[0]);
                                    //clear temp variables for next group
                                    //  }
                                    tempBlockDetails = "";
                                    concatDetails.Clear();
                                }
                            }

                            /* for (int l = fullDetails.Count; l > 0; l--)
                             {
                                 //display in the grid
                                 dataGridView1.Rows.Add(concatDetails[l].date, concatDetails[l].time, concatDetails[l].type, concatDetails[l].id, fullDetails[l]);
                                 //clear temp variables for next group
                                 tempBlockDetails = "";
                                 concatDetails.Clear();
                             } */
                        }
                        //close the file after use
                        file.Close();
                    }
                }
            }
        }
    }
}
