using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SenecTray
{
    public partial class MainWindow : Form
    {
        private string path;
        private static HttpClient client = new HttpClient();
        private string token;
        private int BatteryState = 0;
        private BackgroundWorker backgroundWorker1 = new BackgroundWorker();  
        //backgroundWorker1 = new System.ComponentModel.BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();

            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SenecTray");
            //userFolderPath = Path.Combine(clientPath, user);

            token = GetToken();

            backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(GetStatus);  
            backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(update);

            backgroundWorker1.RunWorkerAsync();

            timer1.Start();

            //Start();

        }

        private string GetToken()
        {
            var payload = new User
            {
                username = "bontovics.t@gmail.com",
                password = "tyAQgT4UZac"
            };

            // Serialize our concrete class into a JSON String
            var stringPayload = JsonConvert.SerializeObject(payload);

            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            // Do the actual request and await the response
            var httpResponse = client.PostAsync("https://app-gateway-prod.senecops.com/v1/senec/login", httpContent).Result;

            var responseContent = httpResponse.Content.ReadAsStringAsync().Result;
            var json_data = JObject.Parse(responseContent);
            return json_data["token"].ToObject<string>();
        }

        public void GetStatus(object sender, DoWorkEventArgs e)
        {
            //Thread.Sleep(3000);

            string url = "https://app-gateway-prod.senecops.com/v1/senec/anlagen/200810/dashboard";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(token);

            var response = client.SendAsync(request).Result;

            var responseContent = response.Content.ReadAsStringAsync().Result;
            var json_data = JObject.Parse(responseContent);
            string state = json_data["aktuell"]["speicherfuellstand"]["wert"].ToObject<string>();

            BatteryState = int.Parse(state.Split('.').First());

            //result = response.Content.ReadAsStringAsync().Result;
        }

        public void update(object sender, EventArgs e)
        {
            this.label1.Text = BatteryState.ToString();
            notifyIcon1.Text = "Battery: " + BatteryState.ToString() + " %" + "\n" + DateTime.Now;

            if (BatteryState == 100)
            {
                notifyIcon1.Icon = Properties.Resources.full;
                return;
            }

            if (BatteryState > 80)
            {
                notifyIcon1.Icon = Properties.Resources.stage4;
                return;
            }

            if (BatteryState > 60)
            {
                notifyIcon1.Icon = Properties.Resources.stage3;
                return;
            }

            if (BatteryState > 40)
            {
                notifyIcon1.Icon = Properties.Resources.stage2;
                return;
            }

            if (BatteryState > 20)
            {
                notifyIcon1.Icon = Properties.Resources.stage1;
                return;
            }

            if (BatteryState < 20)
            {
                notifyIcon1.Icon = Properties.Resources.empty;
                return;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                backgroundWorker1.RunWorkerAsync();
            }
            catch
            {

            }
            
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {

        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            this.Size = new Size(0, 0);
        }
    }
}