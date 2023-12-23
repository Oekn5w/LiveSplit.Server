using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public partial class Settings : UserControl
    {
        public ushort Port { get; set; }

        public string LocalIP { get; set; }
        public bool AutoStart { get; set; }

        public bool EnableVisServer { get; set; }

        public ushort ConnectionsExpected { get; set; }
        public Color ColNegative { get; set; }
        public Color ColPositive { get; set; }
        public Color ColPartial { get; set; }

        public string GetIP()
        {
            IPAddress[] ipv4Addresses = Array.FindAll(
                Dns.GetHostEntry(string.Empty).AddressList,
                a => a.AddressFamily == AddressFamily.InterNetwork);

            return String.Join(",", Array.ConvertAll(ipv4Addresses, x => x.ToString()));
        }

        public string PortString
        {
            get { return Port.ToString(); }
            set { Port = ushort.Parse(value); }
        }

        public Settings()
        {
            InitializeComponent();

            Port = 16834;
            LocalIP = GetIP();
            label3.Text = LocalIP;
            AutoStart = true;

            txtPort.DataBindings.Add("Text", this, "PortString", false, DataSourceUpdateMode.OnPropertyChanged);
            cbAutoStart.DataBindings.Add("Checked", this, "AutoStart", false, DataSourceUpdateMode.OnPropertyChanged);

            ConnectionsExpected = 1;
            numberOfConnections.DataBindings.Add("Value", this, "ConnectionsExpected", false, DataSourceUpdateMode.OnPropertyChanged);

            ColNegative = Color.FromArgb(204, 55, 41);
            ColPositive = Color.FromArgb(41, 204, 84);
            ColPartial = Color.FromArgb(225, 225, 0);

            btnColor1.DataBindings.Add("BackColor", this, "ColNegative", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor2.DataBindings.Add("BackColor", this, "ColPositive", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor3.DataBindings.Add("BackColor", this, "ColPartial", false, DataSourceUpdateMode.OnPropertyChanged);

            EnableVisServer = true;
            cbIndicatorServer.DataBindings.Add("Checked", this, "EnableVisServer", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            return SettingsHelper.CreateSetting(document, parent, "Port", PortString)
               ^ SettingsHelper.CreateSetting(document, parent, "AutoStart", AutoStart)
               ^ SettingsHelper.CreateSetting(document, parent, "EnableVisServer", EnableVisServer)
               ^ SettingsHelper.CreateSetting(document, parent, "ConnectionsExpected", ConnectionsExpected)
               ^ SettingsHelper.CreateSetting(document, parent, "ColNegative", ColNegative)
               ^ SettingsHelper.CreateSetting(document, parent, "ColPositive", ColPositive)
               ^ SettingsHelper.CreateSetting(document, parent, "ColPartial", ColPartial)
               ;
        }

        public void SetSettings(XmlNode settings)
        {
            var element = (XmlElement)settings;
            PortString = SettingsHelper.ParseString(element["Port"]);
            AutoStart = SettingsHelper.ParseBool(element["AutoStart"]);
            EnableVisServer = SettingsHelper.ParseBool(element["EnableVisServer"]);
            ConnectionsExpected = (ushort)SettingsHelper.ParseInt(element["ConnectionsExpected"]);
            ColNegative = SettingsHelper.ParseColor(element["ColNegative"]);
            ColPositive = SettingsHelper.ParseColor(element["ColPositive"]);
            ColPartial = SettingsHelper.ParseColor(element["ColPartial"]);
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            SettingsHelper.ColorButtonClick((Button)sender, this);
        }
    }
}
