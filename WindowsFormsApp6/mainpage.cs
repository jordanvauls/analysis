using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace WindowsFormsApp6
{
    public partial class mainpage : MetroFramework.Forms.MetroForm
    {
        int EuroUsType;
        string version = "";
        string heartrate = "";
        string airpressure = "";
        string speed = "";
        string starttime = "";
        string length = "";
        string weight = "";
        string pathglob;
        string USEURO = "";
        string pindex = "";
        string cadence = "";
        string altitude = "";
        string power = "";
        double minutes;
        double maxhr;
        double minhr;
        double avgpower;
        string[] splitter;
        double maxpower;
        double HRaverage;
        double maxheartratepercentage;
        double speedavg;

        TimeSpan lengthinhours;
        int count = 0;
        int interval = 0;
        PointPairList list = new PointPairList();
        PointPairList list2 = new PointPairList();
        PointPairList list3 = new PointPairList();
        PointPairList list4 = new PointPairList();
        PointPairList list5 = new PointPairList();
        decimal totalDistance;
        List<int> HeartRateList = new List<int>();
        List<int> SpeedList = new List<int>();
        List<int> CadenceList = new List<int>();
        List<int> altitudeList = new List<int>();
        List<int> Powerlist = new List<int>();
        int versionval;
        string monitortype;
        decimal maxspeed;
        double altmax;
        bool SpeedOn;
        bool CadenceOn;
        bool AltitudeOn;
        bool PowerOn;
        bool Power2On;
        bool power3on;
        bool ccdata;
        bool USEUROon;
        bool AIRPOn;
        bool timepresent = false;
        bool heartratepresent = false;
        bool powerpresent = false;
        bool speedpresent = false;
        bool cadencepresent = false;
        bool altitpresent = false;
        double CadenceAverage;
        double AltitAVG;
        DataTable dt = new DataTable();
        public mainpage()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// this is where the graph is loaded and the series are plotting.
        /// </summary>
        private void LoadGraph()
        {
            dataGridView1.DataSource = dt;


            //booleans for checking smode. turning datagrids off and on
            if (AltitudeOn == false)
            {
                dataGridView1.Columns[4].Visible = false;
                checkBox3.Checked = false;
            }else
            {
                checkBox3.Checked = true;

            }

            if (CadenceOn == false)
            {
                dataGridView1.Columns[3].Visible = false;
                checkBox5.Checked = false;
            }else
            {
                checkBox5.Checked = true;
            }
            if (PowerOn == false)
            {
                dataGridView1.Columns[5].Visible = false;
                checkBox2.Checked = false;
            }
            else
            {
                checkBox2.Checked = true;
            }

            if (SpeedOn == false)
            {
                checkBox4.Checked = false;
                dataGridView1.Columns[2].Visible = false;
            }
            else
            {
                checkBox4.Checked = true;
            }


            //graph pane initalise
            GraphPane myPane = zedGraphControl1.GraphPane;


            //clearing the controls
            zedGraphControl1.GraphPane.CurveList.Clear();
            zedGraphControl1.GraphPane.GraphObjList.Clear();



            //assigning the titles
            myPane.Title.Text = "Cycle Data Graph";
            myPane.XAxis.Title.Text = "Time In Seconds";
            myPane.YAxis.Title.Text = "Vaule";


            // checking if the check boxes are checked...
            if (checkBox1.Checked == true)
            {

                LineItem teamACurve = myPane.AddCurve("HeartRate",
                      list, Color.Red, SymbolType.None);
                teamACurve.Line.Width = 2.0F;
                teamACurve.Line.IsSmooth = true;
                teamACurve.Line.SmoothTension = 1F;
            }

            if (checkBox4.Checked == true)
            {
                LineItem teamBCurve = myPane.AddCurve("Speed",
                      list2, Color.Blue, SymbolType.None);
                teamBCurve.Line.Width = 2.0F;
                teamBCurve.Line.IsSmooth = true;
                teamBCurve.Line.SmoothTension = 1F;
            }

            if (checkBox3.Checked == true)
            {
                LineItem teamcCurve = myPane.AddCurve("Altitude",
            list3, Color.Black, SymbolType.None);
                teamcCurve.Line.Width = 2.0F;
                teamcCurve.Line.IsSmooth = true;
                teamcCurve.Line.SmoothTension = 1F;
            }

            if (checkBox5.Checked == true)
            {
                LineItem teamdCurve = myPane.AddCurve("Cadence",
            list4, Color.Green, SymbolType.None);
                teamdCurve.Line.Width = 2.0F;
                teamdCurve.Line.IsSmooth = true;
                teamdCurve.Line.SmoothTension = 1F;
            }
            if (checkBox2.Checked == true)
            {
                LineItem teamECurve = myPane.AddCurve("Power",
             list5, Color.Orange, SymbolType.None);
                teamECurve.Line.Width = 4.0F;
                teamECurve.Line.IsSmooth = true;
                teamECurve.Line.SmoothTension = 1F;
            }

            //taking the x axis and putting the last value as the max.
            myPane.XAxis.Scale.Max = list.LastOrDefault().X;

            //drawing the graph.
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            zedGraphControl1.Refresh();
        }

        /// <summary>
        /// this is where the analysiso f the data takes place.
        /// </summary>
        private void Analyse()
        {
      

            //nuke everything...
            list.Clear();
            list2.Clear();
            list3.Clear();
            list4.Clear();
            list5.Clear();
            SpeedList.Clear();
            HeartRateList.Clear();
            altitudeList.Clear();
            Powerlist.Clear();
            listBox3.Items.Clear();
            dataGridView1.DataSource = null;
            dt.Columns.Clear();
            dt.Clear();
            dt.Rows.Clear();

            //data table columns
            dt.Columns.Add("Time", typeof(TimeSpan));
            dt.Columns.Add("HeartRate", typeof(int));
            dt.Columns.Add("Speed", typeof(int));
            dt.Columns.Add("Cadence", typeof(int));
            dt.Columns.Add("Altitude", typeof(int));
            dt.Columns.Add("Power", typeof(int));


            //string and read all the lines
            string[] lines = System.IO.File.ReadAllLines(pathglob);
            try
            {
                // put the hr data into a list.
                List<String> HRDATA = File.ReadLines(pathglob)
                .SkipWhile(line => line != "[HRData]")
                .Skip(1)
                .TakeWhile(line => line != "")
                .ToList();
                listBox1.DataSource = HRDATA;

                //put param data into a list.
                List<String> ParamData = File.ReadLines(pathglob)
              .SkipWhile(line => line != "[Params]")
              .Skip(1)
              .TakeWhile(line => line != "")
              .ToList();
                listBox2.DataSource = ParamData;

                // adding to list box
                listBox3.Items.Add("SMODE VALUES:");
                listBox3.Items.Add("---------------------");
                string[] Splitlength;
                count = 0;
             
                #region parseparams

                //parsing through param data.
                foreach (var Param in ParamData)
                {



                    //splitters.
                    splitter = Param.Split('=');
                    Splitlength = Param.Split(':');

                    //splitting by each section.
                    if (splitter[0] == "Monitor")
                    {
                        monitortype = splitter[1].ToString();
                    }

                    if (splitter[0] == "Version")
                    {
                        label3.Text = splitter[1].ToString();
                        versionval = Convert.ToInt32(label3.Text);
                    }
                    if (splitter[0] == "SMode")
                    {

                        //working out the smode.
                        label32.Text = splitter[1].ToString();
                        var collection = splitter[1].ToString().Select(c => Int32.Parse(c.ToString()));
                        for (int i = 0; i < collection.Count(); i++)
                        {
                            if (i == 0)
                            {
                                int speed3 = Convert.ToInt32(collection.ToList()[i]);
                                if (speed3 == 1)
                                {
                                  
                                 
                                    SpeedOn = true;
                                    listBox3.Items.Add("Speed : 1");
                                }
                                else
                                {
                                    SpeedOn = false;
                                    listBox3.Items.Add("Speed : 0");
                                }
                            }
                            if (i == 1)
                            {
                                int cadence = Convert.ToInt32(collection.ToList()[i]);
                                if (cadence == 1)
                                {


                                    listBox3.Items.Add("Cadence : 1");
                                    CadenceOn = true;
                                }
                                else
                                {
                                    listBox3.Items.Add("Cadence : 0");
                                    CadenceOn = false;
                                }
                            }
                            if (i == 2)
                            {
                                int altitude = Convert.ToInt32(collection.ToList()[i]);
                                if (altitude == 1)
                                {
                                          
                                
                                    listBox3.Items.Add("Altitude : 1");
                                    AltitudeOn = true;
                                }
                                else
                                {
                                    listBox3.Items.Add("Altitude : 0");
                                    AltitudeOn = false;
                                }
                            }
                            if (i == 3)
                            {
                                int poweronint = Convert.ToInt32(collection.ToList()[i]);
                                if (poweronint == 1)
                                {
                                    
                                    listBox3.Items.Add("Power : 1");
                                    PowerOn = true;
                                }
                                else
                                {
                                    listBox3.Items.Add("Power : 0");
                                    PowerOn = false;
                                }
                            }
                            if (i == 4)
                            {
                                int power2 = Convert.ToInt32(collection.ToList()[i]);
                                if (power2 == 1)
                                {
                                    //dataGridView1.Columns.Add("Column", "Power Balance & Pedalling Index");
                                    listBox3.Items.Add("Power Left/Right Balance : 1");
                                    Power2On = true;
                                }
                                else
                                {
                                    listBox3.Items.Add("Power Left/Right Balance : 0");
                                    Power2On = false;
                                }
                            }
                            if (i == 5)
                            {
                                int power3 = Convert.ToInt32(collection.ToList()[i]);
                                if (power3 == 1)
                                {


                                    //dataGridView1.Columns.Add("Column", "Power Pedalling Index");
                                    listBox3.Items.Add("Power Pedalling Index : 1");
                                    power3on = true;
                                }
                                else
                                {
                                    listBox3.Items.Add("Power Pedalling Index : 0");
                                    power3on = false;
                                }
                            }
                            if (i == 6)
                            {
                                int HRCCDATA = Convert.ToInt32(collection.ToList()[i]);
                                if (HRCCDATA == 1)
                                {
                                    //dataGridView1.Columns.Add("Column", "HR/CC DATA");
                                    listBox3.Items.Add("HR/CC Data : 1");
                                    ccdata = true;
                                }
                                else
                                {
                                    listBox3.Items.Add("HR/CC Data : 0");
                                    ccdata = false;
                                }
                            }
                            if (i == 7)
                            {
                                int USEURO = Convert.ToInt32(collection.ToList()[i]);
                                if (USEURO == 1)
                                {
                                    //dataGridView1.Columns.Add("Column", "Speed (MPH");
                                    listBox3.Items.Add("US/EURO Unit : 1");
                                    USEUROon = true;
                                }
                                else
                                {
                                    //dataGridView1.Columns.Add("Column", "Speed (KMPH)");
                                    listBox3.Items.Add("US/EURO Unit : 0");
                                    USEUROon = false;
                                }
                            }
                            if (versionval == 107)
                            {
                                if (i == 8)
                                {
                                    int airp = Convert.ToInt32(collection.ToList()[i]);
                                    if (airp == 1)
                                    {
                                        // dataGridView1.Columns.Add("Column", "Air Pressure");
                                        listBox3.Items.Add("Air Pressure : 1");
                                        AIRPOn = true;
                                    }
                                    else
                                    {
                                        listBox3.Items.Add("Air Pressure : 0");
                                        AIRPOn = false;
                                    }
                                }
                            }
                        }
                    }

                    if (splitter[0] == "MaxHR")
                    {
                        HeartRateData.Text = splitter[1].ToString();
                    }

                    if (splitter[0] == "StartTime")
                    {
                        label5.Text = splitter[1].ToString();
                    }

                    if (splitter[0] == "Length")
                    {
                        label7.Text = splitter[1].ToString();
                        lengthinhours = TimeSpan.Parse(label7.Text);



                    }

                    if (splitter[0] == "Interval")
                    {
                        label11.Text = splitter[1].ToString();
                        interval = Convert.ToInt32(label11.Text);
                    }
                    if (splitter[0] == "Date")
                    {
                        starttime = splitter[1].ToString();

                        DateTime theTime = DateTime.ParseExact(starttime,
                                      "yyyyMMdd",
                                      CultureInfo.InvariantCulture,
                                      DateTimeStyles.None);

                        label9.Text = theTime.ToString();
                    }

                }
                #endregion
                //parsing the hrdata.
                for (int i = 0; i < HRDATA.Count; i++)
                {
                    count++;
                    string[] splitter = HRDATA[i].Split('\t');
                    heartrate = splitter[0].ToString();
                    HeartRateList.Add(Convert.ToInt32(heartrate));
                    list.Add(Convert.ToDouble(count), Convert.ToDouble(heartrate));
                    int columnindex = 1;

                    //creating the timespan.
                    TimeSpan time = TimeSpan.FromSeconds(count * interval);

                    DataRow dr = dt.NewRow();
                    dr[0] = time;
                    dr[1] = heartrate;
                    if (SpeedOn == true)
                    {
                        speed = splitter[columnindex].ToString();


                        double SpeedKPH = Convert.ToInt32(speed) / 10;
                        double SpeedMPH = SpeedKPH / 1.609;

                        if (kmph.Checked == true)
                        {
                      
                            SpeedList.Add(Convert.ToInt32(SpeedKPH));
                            list2.Add(Convert.ToDouble(count), Convert.ToInt32(SpeedKPH));
                            label34.Text = "KM/H";
                            label35.Text = "Kilometers";
                            dr[2] = SpeedKPH;
                        }
                        else if (mph.Checked == true)
                        {
                  
                            SpeedList.Add(Convert.ToInt32(SpeedMPH));
                            list2.Add(Convert.ToDouble(count), SpeedMPH);
                            label34.Text = "MP/H";
                            label35.Text = "Miles";
                            dr[2] = SpeedMPH;
                        }
                        maxspeed = (SpeedList.Max());
                        speedavg = (SpeedList.Average());
                        columnindex++;
                    }
                    if (CadenceOn == true)
                    {

                        cadence = splitter[columnindex].ToString();
                        CadenceList.Add(Convert.ToInt32(splitter[columnindex]));
                        list4.Add(Convert.ToDouble(count), Convert.ToDouble(cadence));
                        CadenceAverage = CadenceList.Average();
                        columnindex++;
                        dr[3] = cadence;

                    }
                    if (AltitudeOn == true)
                    {
                        altitude = splitter[columnindex].ToString();
                        altitudeList.Add(Convert.ToInt32(splitter[columnindex]));
                        list3.Add(Convert.ToDouble(count), Convert.ToDouble(altitude));
                        altmax = altitudeList.Max();
                        AltitAVG = altitudeList.Average();
                        columnindex++;
                        dr[4] = altitude;

                    }
                    if (PowerOn == true)
                    {
                        power = splitter[columnindex].ToString();
                        Powerlist.Add(Convert.ToInt32(splitter[columnindex]));
                        list5.Add(Convert.ToDouble(count), Convert.ToDouble(power));
                        avgpower = Powerlist.Average();
                        maxpower = Powerlist.Max();
                        columnindex++;
                        dr[5] = power;
                    }
                    if (Power2On == true)
                    {

                        columnindex++;
                    }
                    if (power3on == true)
                    {
                        columnindex++;
                    }
                    if (ccdata == true)
                    {
                        columnindex++;
                    }
                    if (USEUROon == true)
                    {
                        mph.Checked = true;
                        columnindex++;
                    }

                    if (versionval == 107)
                    {
                        if (AIRPOn == true)
                        {

                        }
                    }
                    //if (Power2On == true)
                    //{
                    //    power = splitter[columnindex].ToString();
                    //    Powerlist.Add(Convert.ToInt32(splitter[columnindex]));
                    //    list5.Add(Convert.ToDouble(count), Convert.ToDouble(power));
                    //    columnindex++;

                    //}
                    //if (power3on == true)
                    //{
                    //    power = splitter[columnindex].ToString();
                    //    Powerlist.Add(Convert.ToInt32(splitter[columnindex]));
                    //    list5.Add(Convert.ToDouble(count), Convert.ToDouble(power));
                    //    columnindex++;

                    //}
                    dt.Rows.Add(dr);
                }


           
                //getting max heart rate.
                maxhr = HeartRateList.Max();
                minhr = HeartRateList.Min();









                //doing the calculations.
                Decimal HRaverage2 = Math.Round(Convert.ToDecimal(HRaverage));
                Decimal speedavg2 = Math.Round(Convert.ToDecimal(speedavg));
                Decimal AltitAVG2 = Math.Round(Convert.ToDecimal(AltitAVG));
                double hours = lengthinhours.TotalHours;
                minutes = lengthinhours.TotalMinutes;
                totalDistance = Convert.ToDecimal(speedavg) * Convert.ToDecimal(hours);
                label31.Text = Convert.ToString(Math.Round(totalDistance));
                label18.Text = Convert.ToString(HRaverage2);
                label19.Text = Convert.ToString(AltitAVG2);
                label20.Text = Convert.ToString(speedavg2);
                label21.Text = Convert.ToString(Math.Round(maxhr));
                label22.Text = Convert.ToString(Math.Round(minhr));
                label23.Text = Convert.ToString(Math.Round(maxspeed));
                label25.Text = Convert.ToString(Math.Round(avgpower));
                label26.Text = Convert.ToString(Math.Round(maxpower));
                label28.Text = Convert.ToString(Math.Round(altmax));





                //monitors values to get the model.
                int Monitorval = Convert.ToInt32(monitortype);

                if (Monitorval == 1) { label37.Text = "Model: " + "Polar Sport Tester / Vantage XL"; }
                if (Monitorval == 2) { label37.Text = "Model: " + "Polar Vantage NV (VNV)"; }
                if (Monitorval == 3) { label37.Text = "Model: " + "Polar Accurex Plus"; }
                if (Monitorval == 4) { label37.Text = "Model: " + "Polar XTrainer Plus"; }
                if (Monitorval == 6) { label37.Text = "Model: " + "Polar S520"; }
                if (Monitorval == 7) { label37.Text = "Model: " + "Polar Coach"; }
                if (Monitorval == 8) { label37.Text = "Model: " + "Polar S210"; }
                if (Monitorval == 9) { label37.Text = "Model: " + "Polar S410"; }
                if (Monitorval == 10) { label37.Text = "Model: " + "Polar S610 / S610i"; }
                if (Monitorval == 12) { label37.Text = "Model: " + "Polar S710 / S710i / S720i"; }
                if (Monitorval == 13) { label37.Text = "Model: " + "Polar S810 / S810i"; }
                if (Monitorval == 15) { label37.Text = "Model: " + "Polar E600"; }
                if (Monitorval == 20) { label37.Text = "Model: " + "Polar AXN500"; }
                if (Monitorval == 21) { label37.Text = "Model: " + "Polar AXN700"; }
                if (Monitorval == 22) { label37.Text = "Model: " + "Polar S625X / S725X"; }
                if (Monitorval == 23) { label37.Text = "Model: " + "Polar S725"; }
                if (Monitorval == 33) { label37.Text = "Model: " + "Polar CS400"; }
                if (Monitorval == 34) { label37.Text = "Model: " + "Polar CS600X"; }
                if (Monitorval == 35) { label37.Text = "Model: " + "Polar CS600"; }
                if (Monitorval == 36) { label37.Text = "Model: " + "Polar RS400"; }
                if (Monitorval == 37) { label37.Text = "Model: " + "Polar RS800"; }
                if (Monitorval == 38) { label37.Text = "Model: " + "Polar RS800X"; }
              
                LoadGraph();


             
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void zedGraphControl1_Load(object sender, EventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            //opening the file.
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Text File";
            theDialog.Filter = "HRM files (*.HRM)|*.HRM|txt files (*.txt)|*.txt|All files (*.*)|*.*";
            theDialog.InitialDirectory = @"C:\";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                string path = theDialog.FileName;
                pathglob = path;
                Analyse();

            }

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            Analyse();
        }
        /// <summary>
        /// changing the metrics to KMP/h
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void kmph_CheckedChanged(object sender, EventArgs e)
        {
            dt.Clear();
            Analyse();
        }
        /// <summary>
        /// changing the metrics to mph
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mph_CheckedChanged(object sender, EventArgs e)
        {
            dt.Clear();
            Analyse();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void zedGraphControl1_Load_1(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            LoadGraph();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            LoadGraph();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            LoadGraph();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            LoadGraph();
        }
        /// <summary>
        /// numeric up down used for ftp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

            //calculations for the hr avg
            double HRavgPERCENT = Math.Round(HRaverage / (Convert.ToDouble(numericUpDown1.Value)) * 100, 2);
            double HRMINPERCENT = Math.Round(minhr / (Convert.ToDouble(numericUpDown1.Value)) * 100, 2);
            double HRmaxPERCENT = Math.Round(maxhr / (Convert.ToDouble(numericUpDown1.Value)) * 100, 2);
            label41.Text = HRavgPERCENT.ToString() + "%";
            label42.Text = HRmaxPERCENT.ToString() + "%";
            label43.Text = HRMINPERCENT.ToString() + "%";
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            //calculations for the ftp
            double FTPAVGPERCENT = Math.Round(avgpower / (Convert.ToDouble(numericUpDown2.Value)) * 100, 2);
            double FTPMAXPERCENT = Math.Round(maxpower / (Convert.ToDouble(numericUpDown2.Value)) * 100, 2);

            label45.Text = FTPAVGPERCENT.ToString() + "%";
            label47.Text = FTPMAXPERCENT.ToString() + "%";

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked == true)
            {
                dataGridView1.Visible = true;
            }
            else
            {
                dataGridView1.Visible = false;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}


