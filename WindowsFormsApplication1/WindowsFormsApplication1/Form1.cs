using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using System.Net.Mail;
using System.Net;

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
            public string emailId { get; set; }
            public string emailId2 { get; set; }
            public string type { get; set; }
            public string id { get; set; }
            public string id2 { get; set; }
            public string details { get; set; }
        }

        public DataTable GetDataTable()
        {
            //initating regex
            Regex rSize = new Regex(@"(?<=Size=\[)[0-9]*(?=\])");
            Regex rAttach = new Regex(@"(?<=Attachments=\[)[a-zA-Z0-9]*(?=\])");
            Regex rDir = new Regex(@"(Inbound|Outbound)");
            Regex rLabel = new Regex(@"(?<=X-\$Switch=\[)[a-zA-Z]*(?=\])");
            Regex rEmailId = new Regex(@"(?<=\[)[a-zA-Z0-9]+(?=\.eml)");

            //create new data table
            DataTable dtMetrics = new DataTable();

            //clears table at beginning to allow for any changes in the file while running (i.e acts as a refresh button)
            dtMetrics.Rows.Clear();

            //create columns in data table
            dtMetrics.Columns.Add("Date");
            dtMetrics.Columns.Add("Time In");
            dtMetrics.Columns.Add("Time Out");
            dtMetrics.Columns.Add("Time Taken (s)");
            dtMetrics.Columns.Add("Email Size (bytes)");
            dtMetrics.Columns.Add("Has Attachments");
            dtMetrics.Columns.Add("Security Label ID");
            dtMetrics.Columns.Add("Direction");
            dtMetrics.Columns.Add("ID");

            //get file path from user
            String x = Interaction.InputBox("Please enter the file location of the logs", "File Location", "D:\\Hasaan\\Desktop\\Logs", -1, -1);

            //ensure file path exists, if not display error message
            if (!Directory.Exists(x))
            {
                MessageBox.Show("The path '" + x + "' does not exist. Please try again.", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                importButton.Text = "Import Log Files";
                exportCSVButton.Enabled = false;
                filter.Enabled = false;
                comboBox1.Enabled = false;
            }
            else {

                //file location on my Egress Laptop C:\\Users\\hasaan.ausat\\Desktop\\Logs on Gateway Machine C:\\Program Files\\Egress\\SDX\\logs
                var fileCount = (from doc in Directory.EnumerateFiles(@x, "*.log", SearchOption.AllDirectories)
                                 select doc).Count();
                //display message if folder is empty or continue loading files in to program
                if (fileCount == 0)
                {
                    MessageBox.Show("There are no log files to import", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    importButton.Text = "Import Log Files";
                    exportCSVButton.Enabled = false;
                    filter.Enabled = false;
                    comboBox1.Enabled = false;
                }
                else {

                    //changes text on button
                    importButton.Text = "Refresh";

                    exportCSVButton.Enabled = true;
                    filter.Enabled = true;
                    comboBox1.Enabled = true;

                    string[] dirs = Directory.GetFiles(@x);

                    //assigning temp variables for each line
                    String tempDate = "";
                    String tempTime = "";
                    String tempTimeOut = "";
                    String tempSize = "";
                    String tempAttach = "";
                    String tempLabel = "";
                    String tempDir = "";
                    String tempId = "";
                    String tempEmailId = "";
                    String tempEmailId2 = "";
                    String tempType = "";
                    String tempDetails = "";

                    String tempBlockDetails = "";

                    List<String> fullDetails = new List<String>();

                    List<log> logList = new List<log>();

                    List<log> concatDetails = new List<log>();

                    List<log> logList2 = new List<log>();

                    char splitChar = ':';
                    char splitChar2 = '-';
                    String text = "";
                    String text2 = "";
                    String text3 = "14:00:00";
                    String[] times = new String[3];
                    String[] dates = new String[3];
                    String[] timeouts = new String[3];
                    DateTime time1 = new DateTime();
                    DateTime time2 = new DateTime();
                    TimeSpan diff = TimeSpan.Zero;
                    String[] times2 = new String[10];
                    String secs = "";

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
                            if (tempId == "1180")
                            {

                            }
                            tempEmailId = rEmailId.Match(tempDetails).ToString();

                            //creating log object and pushing it onto the logList
                            log tempLogObject = new log { date = tempDate, time = tempTime, type = tempType, id = tempId, details = tempDetails, emailId = tempEmailId };
                            logList.Insert(0, tempLogObject);

                            //ensuring only specific ID's are searched for and displayed
                            if (tempId == "1180")
                            {
                                //check if ID is the same as the last
                                if (logList.Count == 1 || logList[0].id == logList[1].id)
                                {
                                    //if so push on the list as objects are part of group
                                    concatDetails.Insert(0, tempLogObject);
                                }
                                else {
                                    //previous group has finished - concats all the details
                                    concatDetails.Insert(0, tempLogObject);
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
                                    tempDir = rDir.Match(fullDetails[0]).ToString();
                                    if (rLabel.Match(fullDetails[0]).ToString() == "" && tempDir != "Inbound")
                                    {
                                        tempLabel = "public";
                                    }
                                    else
                                    {
                                        tempLabel = rLabel.Match(fullDetails[0]).ToString();
                                    }
                                    tempEmailId2 = rEmailId.Match(fullDetails[0]).ToString();
                                    //if (concatDetails[0].id == "1180" && concatDetails[0].id == "1112")
                                    //{
                                    //    tempTimeOut = concatDetails[0].time;
                                    //} else
                                    //{
                                    //    tempTimeOut = "00:00:00";
                                    //}
                                    //MessageBox.Show(tempTimeOut);
                                    log tempLogObject2 = new log { id2 = tempId, emailId2 = tempEmailId2, timeOut = tempTimeOut, size = tempSize, attach = tempAttach, label = tempLabel, direction = tempDir };
                                    logList2.Insert(0, tempLogObject2);

                                    //if (concatDetails[0].id == "1180" && logList2[0].id2 == "1112" && concatDetails[0].emailId == logList2[0].emailId2)
                                    //{
                                    //    if (logList[0].id == "1112" || logList2[0].id2 == "1112")
                                    //    {
                                    //        logList2[0].timeOut = concatDetails[0].time;
                                    //    }
                                    //}
                                    //select time from loglist where ((loglist.id = 1180 and loglist2.id == 1112) and  (loglist.emailtempid = loglist2.emailtempid2)) or ((loglist.id == 1180 and loglist2.id == 1113) and  ((loglist.emailtempid = loglist2.emailtempid2))

                                    //spliting date and time and performing a subtraction in order to find difference
                                    text = concatDetails[0].time;
                                    text2 = concatDetails[0].date;
                                    times = text.Split(splitChar);
                                    dates = text2.Split(splitChar2);
                                    timeouts = text3.Split(splitChar);
                                    time1 = new DateTime(Convert.ToInt32(dates[0]), Convert.ToInt32(dates[1]), Convert.ToInt32(dates[2]), Convert.ToInt32(times[0]), Convert.ToInt32(times[1]), Convert.ToInt32(times[2]));
                                    time2 = new DateTime(Convert.ToInt32(dates[0]), Convert.ToInt32(dates[1]), Convert.ToInt32(dates[2]), Convert.ToInt32(timeouts[0]), Convert.ToInt32(timeouts[1]), Convert.ToInt32(timeouts[2]));
                                    diff = time2.Subtract(time1);
                                    secs = diff.TotalSeconds.ToString();

                                    //display in the grid     
                                    dtMetrics.Rows.Add(concatDetails[0].date, concatDetails[0].time, text3, secs, logList2[0].size, logList2[0].attach, logList2[0].label, logList2[0].direction, concatDetails[0].id);
                                    
                                    //clear temp variables for next group
                                    tempBlockDetails = "";
                                    concatDetails.Clear();

                                }
                            }
                            
                        }
                        //close the file after use
                        file.Close();
                    }
                }
            }
            dataGridView1.DataSource = dtMetrics;
            return dtMetrics;
        }

        public void importButton_Click(object sender, EventArgs e)
        {
            GetDataTable();
        }

        //export to csv function - accessed on 6/4/16
        //http://stackoverflow.com/questions/9943787/exporting-datagridview-to-csv-file
        public void SaveToCSV(DataGridView dGV)
        {
            string filename = "";
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV (*.csv)|*.csv";
            sfd.FileName = "Output.csv";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Data will be exported and you will be notified when it is ready.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (File.Exists(filename))
                {
                    try
                    {
                        File.Delete(filename);
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                int columnCount = dGV.ColumnCount;
                string columnNames = "";
                string[] output = new string[dGV.RowCount + 1];
                for (int i = 0; i < columnCount; i++)
                {
                    columnNames += dGV.Columns[i].HeaderText.ToString() + ",";
                }
                output[0] += columnNames;
                for (int i = 1; (i - 1) < dGV.RowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        output[i] += dGV.Rows[i - 1].Cells[j].Value.ToString() + ",";
                    }
                }
                File.WriteAllLines(sfd.FileName, output, Encoding.UTF8);
                MessageBox.Show("Your file was generated and its ready for use.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                sendEmail.Enabled = true;
            }
        }

        private void exportCSVButton_Click(object sender, EventArgs e)
        {
            SaveToCSV(dataGridView1);
        }

        //sending an email
        private void sendEmail_Click(object sender, EventArgs e)
        {
            String password = "";
            String emailAddFrom = "";
            String emailAddTo = "";
            Match match = Regex.Match(textBox1.Text, "[a-zA-Z]+@hotmail.com");
            if (!match.Success)
            {
                MessageBox.Show("Please enter a hotmail email address", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else
            {
                emailAddFrom = textBox1.Text;
                if (textBox2.Text == "")
                {
                    MessageBox.Show("Please enter your password!", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    password = textBox2.Text;
                    emailAddTo = Interaction.InputBox("Please enter the email address(es) that you are sending to (comma-separated)", "Email Address", "a@egress.com, b@egress.com", -1, -1);
                    match = Regex.Match(emailAddTo, "[a-zA-Z]+@egress.com");
                    if (!match.Success)
                    {
                        MessageBox.Show("Please enter valid Egress email address(es) you would like to send to", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        
                    } else
                    {
                        MailMessage mail = new MailMessage(emailAddFrom, emailAddTo + "," + emailAddFrom);
                        SmtpClient client = new SmtpClient();
                        client.Port = 587;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.UseDefaultCredentials = false;
                        client.Host = "smtp-mail.outlook.com";
                        client.EnableSsl = true;
                        client.Credentials = new NetworkCredential(emailAddFrom, password);
                        mail.Subject = "Gateway Metrics";
                        mail.Body = textBox4.Text;
                        OpenFileDialog ofd = new OpenFileDialog();
                        ofd.ShowDialog();
                        mail.Attachments.Add(new Attachment(ofd.FileName));
                        client.SendMailAsync(mail);
                        textBox1.Text = "";
                        textBox2.Text = "";
                        textBox4.Text = "";
                        password = "";
                    }
                }  
            }
        }

        //filtering the table based on column seleced and value entered
        public void button1_Click_3(object sender, EventArgs e)
        {
            String filterBy = "";
            String filterValue = "";
            if (comboBox1.Text == "")
            {
                MessageBox.Show("Please select the column you would like to filter the table by", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                filterBy = comboBox1.Text;
                if (textBox3.Text == "")
                {
                    MessageBox.Show("Please enter the value you would like to filter by", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    filterValue = textBox3.Text;
                    using (DataTable dtMetrics = GetDataTable())
                    {
                        DataView dv;
                        dv = new DataView(dtMetrics, filterBy + " = '" + filterValue + "'", null, DataViewRowState.CurrentRows);
                        dataGridView1.DataSource = dv;
                        importButton.Text = "Unfilter";
                        textBox3.Text = "";
                    }
                }
            }
        }

        private void deleteFiles_Click(object sender, EventArgs e)
        {
            String x = Interaction.InputBox("Please enter the file location of the logs", "File Location", "D:\\Hasaan\\Desktop\\Logs", -1, -1);
            if (!Directory.Exists(x))
            {
                MessageBox.Show("The path '" + x + "' does not exist. Please try again.", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else
            {
                if (Directory.GetFiles(@x, "*.log").Count() == 0)
                {
                    MessageBox.Show("There are no log files in the given directory.", null, MessageBoxButtons.OK, MessageBoxIcon.Information);
                } else
                {
                   string yesNo = MessageBox.Show("Are you sure you want to delete the log files?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation).ToString();
                    if (yesNo == "Yes")
                    {
                        Array.ForEach(Directory.GetFiles(@x, "*.log"), File.Delete);
                        MessageBox.Show("The log files in "+ x +" have now been deleted.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
    }
}
