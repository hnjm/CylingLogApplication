﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyclingLogApplication
{
    public partial class RideDataEntry : Form
    {
        //MainForm mainForm;
        public RideDataEntry(MainForm mainForm)
        {
            InitializeComponent();

            // Set the Minimum, Maximum, and initial Value.
            numericUpDown1.Value = 0;
            numericUpDown1.Maximum = 200;
            numericUpDown1.Minimum = 0;
            numericUpDown1.DecimalPlaces = 2;
            numericUpDown1.Increment = 0.10M;

            numericUpDown2.Value = 0;
            numericUpDown2.Maximum = 50;
            numericUpDown2.Minimum = 0;
            numericUpDown2.DecimalPlaces = 2;
            numericUpDown2.Increment = 0.01M;

            textBox1.ScrollBars = ScrollBars.Horizontal;

            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            //For 24 H format
            dateTimePicker2.CustomFormat = "HH:mm:ss";
            dateTimePicker2.ShowUpDown = true;
            dateTimePicker2.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            //Update Ride Type CB3:
            //comboBox3.Items.Add("Recovery");
            //comboBox3.Items.Add("Base");
            //comboBox3.Items.Add("Distance");
            //comboBox3.Items.Add("Speed");
            //comboBox3.Items.Add("Race");

            List<string> routeList = mainForm.GetRoutes();
            for (int i = 0; i < routeList.Count; i++)
            {
                cbRouteDataEntry.Items.Add(routeList.ElementAt(i));
            }

            List<string> logYearList = mainForm.GetLogYears();
            for (int i = 0; i < logYearList.Count; i++)
            {
                cbLogYearDataEntry.Items.Add(logYearList.ElementAt(i));
            }
        }
        public void AddLogYearDataEntry(string item)
        {
            cbLogYearDataEntry.Items.Add(item);
        }

        public void RemoveLogYearDataEntry(string item)
        {
            cbLogYearDataEntry.Items.Remove(item);
        }



        public void AddRouteDataEntry(string item)
        {
            cbRouteDataEntry.Items.Add(item);
        }

        public void RemoveRouteDataEntry(string item)
        {
            cbRouteDataEntry.Items.Remove(item);
        }

        public void AddBikeDataEntry(string item)
        {
            cbBikeDataEntry.Items.Add(item);
        }

        public void RemoveBikeDataEntry(string item)
        {
            cbBikeDataEntry.Items.Remove(item);
        }

        //Diable x close option:
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            textBox1.Text = dateTimePicker1.Text;
        }

        private void submitData(object sender, EventArgs e)
        {
            RideInformationAdd(true);
        }

        private void closeRideDataEntry(object sender, EventArgs e)
        {
            //Close();
            //this.Invoke(new MethodInvoker(delegate { this.Close(); }), null);
            DialogResult result = MessageBox.Show("Any unsaved changes will be lost, do you want to continue?", "Exit Data Entry Form", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                //Close();
                this.Invoke(new MethodInvoker(delegate { this.Close(); }), null);
            }

        }


        //private void textBox1_Validating(object sender, CancelEventArgs e)
        //{
        //    TextBox box = sender as TextBox;
        //    string pattern = "\\d{1,2}:\\d{2}\\s*(AM|PM)";
        //    //textBox1.Text = string.Format("{00:00:00}", 55);
        //    if (box != null)
        //    {
        //        if (!Regex.IsMatch(box.Text, pattern, RegexOptions.CultureInvariant))
        //        {
        //            MessageBox.Show("Not a valid time format ('hh:mm AM|PM').");
        //            e.Cancel = true;
        //            box.Select(0, box.Text.Length);
        //        }
        //    }
        //}

        //private void RideDataEntry_FormClosing(object sender, FormClosingEventArgs e)
        //{

        //    DialogResult result = MessageBox.Show("Do you really want to exit?", "Dialog Title", MessageBoxButtons.YesNo);
        //    if (result == DialogResult.Yes)
        //    {
        //        //Close();
        //        //this.Invoke(new MethodInvoker(delegate { this.Close(); }), null);
        //        RideDataEntry.ActiveForm.Close();
        //    }
        //    else
        //    {
        //        e.Cancel = true;
        //    }
        //}



        private void RideInformationAdd(bool ReturnInactive)
        {
            //Make sure certain required fields are filled in:
            if (cbLogYearDataEntry.SelectedIndex < 0)
            {
                MessageBox.Show("A Log year must be selected.");
                return;
            }
            if (cbRouteDataEntry.SelectedIndex < 0)
            {
                MessageBox.Show("A Route must be selected.");
                return;
            }
            if (cbBikeDataEntry.SelectedIndex < 0)
            {
                MessageBox.Show("A Bike must be selected.");
                return;
            }
            if (cbRideTypeDataEntry.SelectedIndex < 0)
            {
                MessageBox.Show("A Ride Type must be selected.");
                return;
            }
            if (cbLocationDataEntry.SelectedIndex < 0)
            {
                MessageBox.Show("A Ride Location must be selected.");
                return;
            }


            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;
            int weekValue = cal.GetWeekOfYear(DateTime.Now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

            List<object> objectValues = new List<object>();
            objectValues.Add(dateTimePicker2.Value);            //Moving Time:
            objectValues.Add(numericUpDown2.Value);             //Ride Distance:
            objectValues.Add(numericUpDown1.Value);             //Average Speed:
            objectValues.Add(cbBikeDataEntry.SelectedItem.ToString());    //Bike:
            objectValues.Add(cbRideTypeDataEntry.SelectedItem.ToString());//Ride Type:
            double windspeed = (double)numericUpDown4.Value;
            objectValues.Add(numericUpDown4.Value);             //Wind:
            double temp = (double)numericUpDown3.Value;
            objectValues.Add(numericUpDown3.Value);             //Temp:
            objectValues.Add(dateTimePicker1.Value);            //Date:
            objectValues.Add(avg_cadence.Text);                 //Average Cadence:
            objectValues.Add(avg_heart_rate.Text);              //Average Heart Rate:
            objectValues.Add(max_heart_rate.Text);              //Max Heart Rate:
            objectValues.Add(calories.Text);                    //Calories:
            objectValues.Add(total_ascent.Text);                //Total Ascent:
            objectValues.Add(total_descent.Text);               //Total Descent:
            objectValues.Add(max_speed.Text);                   //Max Speed:
            objectValues.Add(avg_power.Text);                   //Average Power:
            objectValues.Add(max_power.Text);                   //Max Power:
            objectValues.Add(cbRouteDataEntry.SelectedItem.ToString());   //Route:
            objectValues.Add(textBox1.Text);                    //Comments:

            MainForm mainForm = new MainForm();
            string logYearName = cbLogYearDataEntry.SelectedItem.ToString();
            int logIndex = mainForm.getLogYearIndex(logYearName);
            objectValues.Add(logIndex);                         //LogYear index:
            objectValues.Add(weekValue);                        //Week number:
            objectValues.Add(cbLocationDataEntry.SelectedItem.ToString());//Location:
            double winchill = 0;

            if (windspeed > 3 && temp > 50)
            {
                winchill = 35.74 + (0.6215) * (temp) - (35.75) * (Math.Pow(windspeed, 0.16)) + (0.4275) * (Math.Pow(windspeed, 0.16));
                objectValues.Add(winchill.ToString());          //Winchill:
            }
            else
            {
                objectValues.Add("");                           //Winchill:
            }

            using (var results = ExecuteSimpleQueryConnection("Ride_Information_Add", objectValues))
            {
                string ToReturn = "";

                //if (results.HasRows)
                //    while (results.Read())
                //        ToReturn = results.GetString(results.GetOrdinal("field1"));

                return;
            }
        }

        public SqlDataReader ExecuteSimpleQueryConnection(string ProcedureName, List<object> _Parameters)
        {
            string tmpProcedureName = "EXECUTE " + ProcedureName + " ";
            SqlDataReader ToReturn = null;

            try
            {
                for (int i = 0; i < _Parameters.Count; i++)
                {
                    tmpProcedureName += "@" + i.ToString() + ",";
                }

                tmpProcedureName = tmpProcedureName.TrimEnd(',') + ";";
                DatabaseConnection databaseConnection = new DatabaseConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""\\mac\home\documents\visual studio 2015\Projects\CyclingLogApplication\CyclingLogApplication\CyclingLogDatabase.mdf"";Integrated Security=True");
                ToReturn = databaseConnection.ExecuteQueryConnection(tmpProcedureName, _Parameters);

            } catch (Exception ex)
            {
                Logger.Log("[ERROR] Exception occurred:  " + ex.Message.ToString(), 0, 0);
            }

            return ToReturn;
        }

        private void RideDataEntryLoad(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'cyclingLogDatabaseDataSet.Table_Ride_Information' table. You can move, or remove it, as needed.
            this.table_Ride_InformationTableAdapter.Fill(this.cyclingLogDatabaseDataSet.Table_Ride_Information);
            cbLogYearDataEntry.SelectedIndex = cbRouteDataEntry.FindStringExact("");
        }

        private void RideDataEntryFormClosed(object sender, FormClosedEventArgs e)
        {
            clearDataEntryFields();
        }

        private void ImportData(object sender, EventArgs e)
        {
            string[] headingList = new string[16];
            string[] splitList = new string[16];
            string[] tempSplitList = new string[16];
            string[] summary = new string[16];
            string tempStr = "";

            //TODO: I think only the Summary line is required:

            try
            {
                using (OpenFileDialog openfileDialog = new OpenFileDialog() { Filter = "CSV|*.csv", Multiselect = false })
                {
                    if (openfileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string line;
                        StreamReader file = new StreamReader(openfileDialog.FileName);
                        int rowCount = 0;

                        while ((line = file.ReadLine()) != null)
                        {
                            var tempList = line.Split(',');

                            if (rowCount == 0)
                            {
                                //Line 1 is the headings
                                headingList = line.Split(',');
                                //MessageBox.Show(headingList[0]);
                            }
                            else if (tempList[0].Equals("Summary"))
                            {
                                summary = line.Split(',');
                                //MessageBox.Show(summary[0]);
                            }
                            else if (rowCount == 1)
                            {
                                splitList = line.Split(',');
                                //MessageBox.Show(splitList[9]);
                            }
                            else
                            {
                                // split item and need to add to or avg in with the previous split
                                tempSplitList = line.Split(',');
                                //MessageBox.Show(tempSplitList[0]);
                            }
                            rowCount++;
                        }
                    }
                }
                //0Data items:
                //1Time
                //2Moving Time
                //3Distance
                //4Elevation Gain
                //5Elevation Loss
                //6Avg Speed
                //7Avg Moving Speed//
                //8Max Speed
                //9Avg HR
                //10Max HR
                //11Avg Bike Cadence
                //12Max Bike Cadence
                //13Avg Temperature
                //14Calories

                //Split time and enter hours-min-sec
                string temp = splitList[2];
                string[] temp2 = temp.Split(':');

                //Check size to see if hours is included:
                //hh:mm:ss
                if (temp2.Length == 0)
                {
                    //MessageBox.Show("Count is 0");
                    dateTimePicker2.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                }
                else if (temp2.Length == 1)
                {
                    //MessageBox.Show("Count is 1");
                    dateTimePicker2.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, Int32.Parse(temp2[0]));
                }
                else if (temp2.Length == 2)
                {
                    //MessageBox.Show("Count is 2");
                    dateTimePicker2.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, Int32.Parse(temp2[0]), Int32.Parse(temp2[1]));
                }
                else if (temp2.Length == 3)
                {
                    //MessageBox.Show("Count is 3");
                    dateTimePicker2.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Int32.Parse(temp2[0]), Int32.Parse(temp2[1]), Int32.Parse(temp2[2]));
                }

                numericUpDown2.Value = System.Convert.ToDecimal(splitList[3]);                                              //4Ride Distance:

                //NOTE: Need to check if any of the values have double quotes and is so, also need to include the next index since they were split because of the comma ex("1,200"):
                //==============================================================
                //4Total Ascent:
                int colIndex = 4;
                int headingIndex = 4;
                tempStr = splitList[colIndex];
                //MessageBox.Show("value: " + tempStr[0]);
                if (tempStr[0].Equals('"'))
                {
                    //MessageBox.Show("A double quote was found");
                    tempStr = splitList[colIndex] + splitList[colIndex + 1];
                    colIndex++;
                    total_ascent.Text = tempStr.Replace("\"", "");
                }
                else
                {
                    //MessageBox.Show("No double quote found");
                    total_ascent.Text = splitList[colIndex];
                }
                colIndex++;
                headingIndex++;
                //==============================================================
                //5Total Descent:
                tempStr = splitList[colIndex];
                if (tempStr[0].Equals('"'))
                {
                    tempStr = splitList[colIndex] + splitList[colIndex + 1];
                    colIndex++;
                    total_descent.Text = tempStr.Replace("\"", "");
                }
                else
                {
                    total_descent.Text = splitList[colIndex];
                }
                colIndex++;
                headingIndex++;
                //==============================================================                                                                        
                colIndex++;
                headingIndex++;
                numericUpDown1.Value = System.Convert.ToDecimal(splitList[colIndex]); ;                                     //8Average moving Speed:
                colIndex++;
                headingIndex++;
                max_speed.Text = splitList[colIndex];                                                                       //9Max Speed:
                colIndex++;
                headingIndex++;

                for (int index = colIndex; index < splitList.Length; index++)
                {
                    if (headingList[headingIndex].Equals("Avg HR"))
                    {
                        avg_heart_rate.Text = splitList[index];                                                              //10Average Cadence:
                    }
                    else if (headingList[headingIndex].Equals("Max HR"))
                    {
                        max_heart_rate.Text = splitList[index];                                                               //Max Heart Rate:
                    }
                    else if (headingList[headingIndex].Equals("Avg Bike Cadence"))
                    {
                        avg_cadence.Text = splitList[index];                                                                   //10Average Cadence:
                    }
                    else if (headingList[headingIndex].Equals("Avg Temperature"))
                    {
                        numericUpDown3.Value = System.Convert.ToDecimal(splitList[index]);                                                                           //12Temp:
                    }
                    else if (headingList[headingIndex].Equals("Calories"))
                    {
                        tempStr = splitList[index];
                        if (tempStr[0].Equals('"'))
                        {
                            tempStr = splitList[index] + splitList[index + 1];
                            index++;
                            calories.Text = tempStr.Replace("\"", "");
                        }
                        else
                        {
                            calories.Text = splitList[index];
                        }
                    }
                    headingIndex++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("[ERROR] Exception occurred: " + ex.Message);
            }
        }

        private void clearDataEntryFields()
        {
            //Reset and clear values:
            dateTimePicker2.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);     //Moving Time:
            numericUpDown2.Value = 0;                                                                                   //Ride Distance:
            numericUpDown1.Value = 0;                                                                                   //Average Speed:
            cbBikeDataEntry.SelectedIndex = cbBikeDataEntry.FindStringExact(""); ;                                      //Bike:
            cbRideTypeDataEntry.SelectedIndex = cbRideTypeDataEntry.FindStringExact(""); ;                                                  //Ride Type:
            numericUpDown4.Value = 0;                                                                                   //Wind:
            numericUpDown3.Value = 0;                                                                                   //Temp:
            dateTimePicker1.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0); ;   //Date:
            avg_cadence.Text = "";                                                                                      //Average Cadence:
            avg_heart_rate.Text = "";                                                                                   //Average Heart Rate:
            max_heart_rate.Text = "";                                                                                   //Max Heart Rate:
            calories.Text = "";                                                                                         //Calories:
            total_ascent.Text = "";                                                                                     //Total Ascent:
            total_descent.Text = "";                                                                                    //Total Descent:
            max_speed.Text = "";                                                                                        //Max Speed:
            avg_power.Text = "";                                                                                        //Average Power:
            max_power.Text = "";                                                                                        //Max Power:
            cbRouteDataEntry.SelectedIndex = cbRouteDataEntry.FindStringExact(""); ;                                    //Route:
            textBox1.Text = "";                                                                                         //Comments:
            cbLogYearDataEntry.SelectedIndex = cbLogYearDataEntry.FindStringExact("");                                  //LogYear index:
            cbLocationDataEntry.SelectedIndex = cbLocationDataEntry.FindStringExact("");
        }

        private void clearDataEntryFields_click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Clearing all fields. Do you want to continue?", "Clear Fields", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                clearDataEntryFields();
            }
        }

        private void cbLogYearDataEntry_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm();
            mainForm.setLastLogSelected(cbLogYearDataEntry.SelectedIndex.ToString());
        }

        private void cbBikeDataEntry_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm();
            mainForm.setLastBikeSelected(cbBikeDataEntry.SelectedIndex.ToString());
        }
    }
}