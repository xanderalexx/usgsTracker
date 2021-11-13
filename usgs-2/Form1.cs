using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace usgs_2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            timer1.Interval = 5000;
            timer1.Start();
        }
        NotifyIcon notifyIcon = new NotifyIcon();
        string lastsaved = null;
        dynamic currenttop = null;
        int tickcount = 0;
        public static string GetResponseText(string address)
        {
            var request = (HttpWebRequest)WebRequest.Create(address);
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var encoding = Encoding.GetEncoding(response.CharacterSet);

                    using (var responseStream = response.GetResponseStream())
                    using (var reader = new StreamReader(responseStream, encoding))
                        return reader.ReadToEnd();
                }
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        return text;
                    }
                }
            }
        }
        private void showBalloon(string title, string body)
        {
            notifyIcon.Visible = true;
            notifyIcon.Icon = SystemIcons.Application;

            if (title != null)
            {
                notifyIcon.BalloonTipTitle = title;
            }

            if (body != null)
            {
                notifyIcon.BalloonTipText = body;
            }

            notifyIcon.ShowBalloonTip(30000);
        }
        public dynamic checkusgs(string orderby, string limit)
        {
            string latitude = "35.040554";
            string longitude = "-117.138306";
            string maxradius = "251.3414";
            string responsetext = GetResponseText("https://earthquake.usgs.gov/fdsnws/event/1/query?format=geojson&latitude=" + latitude + "&longitude=" + longitude + "&maxradiuskm=" + maxradius + "&orderby=" + orderby + "&limit=" + limit);
            dynamic usgsdata = JsonConvert.DeserializeObject<dynamic>(responsetext);
            return usgsdata;
        }

        public dynamic checkusgstop()
        {
            string latitude = "35.040554";
            string longitude = "-117.138306";
            string maxradius = "251.3414";
            string date = DateTime.Today.ToString("s");
            string responsetext = GetResponseText("https://earthquake.usgs.gov/fdsnws/event/1/query?format=geojson&latitude=" + latitude + "&longitude=" + longitude + "&maxradiuskm=" + maxradius + "&orderby=magnitude&limit=1" + "&starttime=" + date);
            dynamic usgsdata = JsonConvert.DeserializeObject<dynamic>(responsetext);
            return usgsdata;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            blankalllabels();

            tickcount += 1;
            label105.Text = "checking...";

            dynamic usgsdata = checkusgs("time", "10");
            dynamic usgstop = checkusgstop();

            setalllabels(usgsdata);
            setalllabelstop(usgstop);

            label105.Text = "(tickcount = " + tickcount.ToString() + ")";
            /*if(currenttop == null)
            {
                currenttop = usgstop;
                setalllabelstop(currenttop);
                showBalloon("New top Earthquake!", currenttop.features[0].properties.title.ToString());
            }*/

            /*if(currenttop != null)
            {
                if (usgsdata.features[0].properties.mag > currenttop.features[0].properties.mag)
                {
                    currenttop = usgstop;
                    setalllabelstop(currenttop);
                    showBalloon("New top Earthquake!", currenttop.features[0].properties.title.ToString());
                }
            }*/

            if (lastsaved == null)
            {
                lastsaved = usgsdata.features[0].properties.time.ToString();
                showBalloon("New Earthquake!", usgsdata.features[0].properties.title.ToString());
            }
            else
            {
                try
                {
                    if (lastsaved == usgsdata.features[0].properties.time.ToString())
                    {

                    }
                    else
                    {
                        showBalloon("New Earthquake!", usgsdata.features[0].properties.title.ToString());
                        lastsaved = usgsdata.features[0].properties.time.ToString();
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, usgsdata.features[0].properties.time.ToString());
                }
            }
        }

        public void setalllabelstop(dynamic usgsdata)
        {
            label54.Text = usgsdata.features[0].properties.mag.ToString();
            label55.Text = usgsdata.features[0].properties.place;
            label59.Text = usgsdata.features[0].properties.updated.ToString();
            label61.Text = usgsdata.features[0].properties.tz.ToString();
            label63.Text = usgsdata.features[0].properties.url;
            label65.Text = usgsdata.features[0].properties.detail;
            label67.Text = usgsdata.features[0].properties.felt.ToString();
            label69.Text = usgsdata.features[0].properties.cdi.ToString();
            label71.Text = usgsdata.features[0].properties.mmi.ToString();
            label73.Text = usgsdata.features[0].properties.alert.ToString();
            label75.Text = usgsdata.features[0].properties.status;
            label77.Text = usgsdata.features[0].properties.tsunami.ToString();
            label79.Text = usgsdata.features[0].properties.sig.ToString();
            label81.Text = usgsdata.features[0].properties.net;
            label83.Text = usgsdata.features[0].properties.code;
            label85.Text = usgsdata.features[0].properties.ids.ToString();
            label87.Text = usgsdata.features[0].properties.sources.ToString();
            label89.Text = usgsdata.features[0].properties.types.ToString();
            label91.Text = usgsdata.features[0].properties.nst.ToString();
            label93.Text = usgsdata.features[0].properties.dmin.ToString();
            label95.Text = usgsdata.features[0].properties.rms.ToString();
            label97.Text = usgsdata.features[0].properties.gap.ToString();
            label99.Text = usgsdata.features[0].properties.magType;
            label101.Text = usgsdata.features[0].properties.type;
            label103.Text = usgsdata.features[0].properties.title;
        }

        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static DateTime FromUnixTime(long unixTime)
        {
            return epoch.AddSeconds(unixTime);
        }

        public void setalllabels(dynamic usgsdata)
        {
            label1.Text = usgsdata.features[0].properties.mag.ToString();
            label2.Text = usgsdata.features[0].properties.place;
            label3.Text = usgsdata.features[0].properties.time.ToString();
            label4.Text = usgsdata.features[0].properties.updated.ToString();
            label5.Text = usgsdata.features[0].properties.tz.ToString();
            label6.Text = usgsdata.features[0].properties.url;
            label7.Text = usgsdata.features[0].properties.detail;
            label8.Text = usgsdata.features[0].properties.felt.ToString();
            label9.Text = usgsdata.features[0].properties.cdi.ToString();
            label10.Text = usgsdata.features[0].properties.mmi.ToString();
            label11.Text = usgsdata.features[0].properties.alert.ToString();
            label12.Text = usgsdata.features[0].properties.status;
            label13.Text = usgsdata.features[0].properties.tsunami.ToString();
            label14.Text = usgsdata.features[0].properties.sig.ToString();
            label15.Text = usgsdata.features[0].properties.net;
            label16.Text = usgsdata.features[0].properties.code;
            label17.Text = usgsdata.features[0].properties.ids.ToString();
            label18.Text = usgsdata.features[0].properties.sources.ToString();
            label19.Text = usgsdata.features[0].properties.types.ToString();
            label20.Text = usgsdata.features[0].properties.nst.ToString();
            label21.Text = usgsdata.features[0].properties.dmin.ToString();
            label22.Text = usgsdata.features[0].properties.rms.ToString();
            label23.Text = usgsdata.features[0].properties.gap.ToString();
            label24.Text = usgsdata.features[0].properties.magType;
            label25.Text = usgsdata.features[0].properties.type;
            label26.Text = usgsdata.features[0].properties.title;
        }

        public void blankalllabels()
        {
            label1.Text = "???";
            label2.Text = "???";
            label3.Text = "???";
            label4.Text = "???";
            label5.Text = "???";
            label6.Text = "???";
            label7.Text = "???";
            label8.Text = "???";
            label9.Text = "???";
            label10.Text = "???";
            label11.Text = "???";
            label12.Text = "???";
            label13.Text = "???";
            label14.Text = "???";
            label15.Text = "???";
            label16.Text = "???";
            label17.Text = "???";
            label18.Text = "???";
            label19.Text = "???";
            label20.Text = "???";
            label21.Text = "???";
            label22.Text = "???";
            label23.Text = "???";
            label24.Text = "???";
            label25.Text = "???";
            label26.Text = "???";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Interval = Convert.ToInt32(textBox1.Text);
        }

        private void label48_Click(object sender, EventArgs e)
        {

        }
    }
}

/*

/*
 * using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace usgs
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 5000;
        }

        public static string GetResponseText(string address)
        {
            var request = (HttpWebRequest)WebRequest.Create(address);
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var encoding = Encoding.GetEncoding(response.CharacterSet);

                    using (var responseStream = response.GetResponseStream())
                    using (var reader = new StreamReader(responseStream, encoding))
                        return reader.ReadToEnd();
                }
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        return text;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dynamic usgsdata = checkusgs("time", "10");

            foreach (dynamic features in usgsdata.features)
            {
                double time = Convert.ToDouble(features.properties.time) / 1000;
                label5.Text = features.properties.place;
                label6.Text = features.properties.mag.ToString();
                label11.Text = UnixTimeStampToDateTime(time).ToString();
            }

            showBalloon("test", "test");

            usgsdata = checkusgs("magnitude", "999");
        }

        private void showBalloon(string title, string body)
        {
            NotifyIcon notifyIcon = new NotifyIcon();
            notifyIcon.Visible = true;
            notifyIcon.Icon = SystemIcons.Application;

            if (title != null)
            {
                notifyIcon.BalloonTipTitle = title;
            }

            if (body != null)
            {
                notifyIcon.BalloonTipText = body;
            }

            notifyIcon.ShowBalloonTip(30000);
        }

        public dynamic checkusgs(string orderby, string limit)
        {
            string latitude = "35.040554";
            string longitude = "-117.138306";
            string maxradius = "251.3414";

            string responsetext = GetResponseText("https://earthquake.usgs.gov/fdsnws/event/1/query?format=geojson&latitude=" + latitude + "&longitude=" + longitude + "&maxradiuskm=" + maxradius + "&orderby=" + orderby + "&limit=" + limit);

            dynamic usgsdata = JsonConvert.DeserializeObject<dynamic>(responsetext);

            return usgsdata;

            foreach (object in usgsdata)
            {
                if(type.ToString() == "Feature")
                {
                    string magn = "hmm";
                    magn = usgsdata.properties["mag"].ToString();
                }
            }

            //Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(responsetext);
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
{
    System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
    dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
    return dtDateTime;
}

private void timer1_Tick(object sender, EventArgs e)
{
    checkusgs("time", "10");
}

private void checkBox1_CheckedChanged(object sender, EventArgs e)
{
    if (checkBox1.Checked == true)
    {
        checkusgs("time", "10");
        timer1.Start();
    }
    else
    {
        timer1.Stop();
    }
}
    }
}

    */