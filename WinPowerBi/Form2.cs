using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System;

using System.Collections.Generic;

using System.Linq;

using System.Windows.Forms;

using Microsoft.Rest;
using System.Globalization;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Security;
using Microsoft.PowerBI.Api.V2;

using System.IO;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;


namespace WinPowerBi
{
    public partial class Form2 : Form
    {
        //TOBE: Change the TEST_ ids with the correct ids to correct to powerbi
        private string UserId = "TEST_USER_ID";
        private string Password = "TEST_PASSWORD";
        private string ClientId = "TEST_CLIENTID";
        private string aadInstance = String.Format(CultureInfo.InvariantCulture, "https://login.microsoftonline.com/{0}", "TEST_APP_ID");
        private string GroupId = "TEST_GROUP_ID";
        private string ReportId = "TEST_REPORT_ID";

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Window_Loaded();
        }

        private string token;
        private async void Window_Loaded()
        {
            string authority = aadInstance;

            TokenCache TC = new TokenCache();
            
            var authContext = new AuthenticationContext(authority, TC);
            var c = new UserPasswordCredential(UserId, Password);

            var result = await authContext.AcquireTokenAsync("https://analysis.windows.net/powerbi/api", ClientId, c);
            
            token = result.AccessToken;

            var tokenCredentials = new TokenCredentials (token, "Bearer");
           

            using (var client = new PowerBIClient(new Uri("https://api.powerbi.com"), tokenCredentials))
            {

                var b = client.Reports.GetReportInGroup(GroupId, ReportId);

                var embedUrl = b.EmbedUrl;
                webBrowser1.Navigate(embedUrl);
              
                webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (token != null)
            {
                var message = "{\"action\":\"loadReport\",\"accessToken\":\"" + token + "\"}";
                webBrowser1.Document.InvokeScript("postMessage", new object[] { message, "*" });
            }
            else
            {
                MessageBox.Show("AccessToken is not defined.");
            }
            webBrowser1.DocumentCompleted -= webBrowser1_DocumentCompleted;
        }

       
    }


    


}
