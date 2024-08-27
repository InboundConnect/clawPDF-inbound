using System.Text;
using pdfforge.DataStorage;

// Custom Code starts here
// START_CUSTOM_SECTION:INCLUDES
// END_CUSTOM_SECTION:INCLUDES
// Custom Code ends here. Do not edit below

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace clawSoft.clawPDF.Core.Settings
{
    /// <summary>
    ///     Upload the converted documents with FTP
    /// </summary>
    public class InboundConnect
    {
        /// <summary>
        ///     Password that is used to authenticate at the server
        /// </summary>
        private string _apiKey;

        public InboundConnect()
        {
            Init();
        }

        /// <summary>
        ///     Target directory on the server
        /// </summary>
        public string Api { get; set; }

        /// <summary>
        ///     If true, this action will be executed
        /// </summary>
        public bool Enabled { get; set; }

        public string ApiKey
        {
            get
            {
                try
                {
                    return Data.Decrypt(_apiKey);
                }
                catch
                {
                    return "";
                }
            }
            set => _apiKey = Data.Encrypt(value);
        }

        /// <summary>
        ///     User name that is used to authenticate at the server
        /// </summary>
        public string UserName { get; set; }

        private void Init()
        {
            Api = "";
            Enabled = false;
            ApiKey = "";
            UserName = "";
        }

        public void ReadValues(Data data, string path)
        {
            try
            {
                Enabled = bool.Parse(data.GetValue(@"" + path + @"Enabled"));
            }
            catch
            {
                Enabled = false;
            }

            _apiKey = data.GetValue(@"" + path + @"ApiKey");

            try
            {
                Api = Data.UnescapeString(data.GetValue(@"" + path + @"Api"));
            }
            catch
            {
                Api = "";
            }

            try
            {
                UserName = Data.UnescapeString(data.GetValue(@"" + path + @"UserName"));
            }
            catch
            {
                UserName = "";
            }
        }

        public void StoreValues(Data data, string path)
        {
            data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
            data.SetValue(@"" + path + @"ApiKey", _apiKey);
            data.SetValue(@"" + path + @"Api", Data.EscapeString(Api));
            data.SetValue(@"" + path + @"UserName", Data.EscapeString(UserName));
        }

        public InboundConnect Copy()
        {
            var copy = new InboundConnect();

            copy.Enabled = Enabled;
            copy.ApiKey = ApiKey;
            copy.Api = Api;
            copy.UserName = UserName;

            return copy;
        }

        public override bool Equals(object o)
        {
            if (!(o is InboundConnect)) return false;
            var v = o as InboundConnect;

            if (!Enabled.Equals(v.Enabled)) return false;
            if (!ApiKey.Equals(v.ApiKey)) return false;
            if (!Api.Equals(v.Api)) return false;
            if (!UserName.Equals(v.UserName)) return false;

            return true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Enabled=" + Enabled);
            sb.AppendLine("ApiKey=" + ApiKey);
            sb.AppendLine("Api=" + Api);
            sb.AppendLine("UserName=" + UserName);

            return sb.ToString();
        }

        public override int GetHashCode()
        {
            // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }

        // Custom Code starts here
        // START_CUSTOM_SECTION:GENERAL
        // END_CUSTOM_SECTION:GENERAL
        // Custom Code ends here. Do not edit below
    }
}