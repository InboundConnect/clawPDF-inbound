using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using clawSoft.clawPDF.Core.Actions;
using clawSoft.clawPDF.Shared.Helper;
using clawSoft.clawPDF.Shared.ViewModels;
using clawSoft.clawPDF.Utilities.Tokens;

namespace clawSoft.clawPDF.Shared.Views.ActionControls
{
    public partial class InboundConnectActionControl : ActionControl
    {
        public InboundConnectActionControl()
        {
            InitializeComponent();

            DisplayName =
                TranslationHelper.Instance.TranslatorInstance.GetTranslation("InboundConnectActionSettings", "DisplayName",
                    "Send to Inbound Connect");
            Description = TranslationHelper.Instance.TranslatorInstance.GetTranslation("InboundConnectActionSettings",
                "Description",
                "The Inbound Connect action allows you to send the output PDF direct to a booking within Inbound Connect.");

            TranslationHelper.Instance.TranslatorInstance.Translate(this);
        }

        public List<string> Tokens { get; private set; }

        public override bool IsActionEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return CurrentProfile.Scripting.Enabled;
            }
            set => CurrentProfile.InboundConnect.Enabled = value;
        }

        private string ApiKey
        {
            get
            {
                if (CurrentProfile == null)
                    return null;
                return CurrentProfile.InboundConnect.ApiKey;
            }
            set => CurrentProfile.InboundConnect.ApiKey = value;
        }

        private void SetPasswordButton_OnClick(object sender, RoutedEventArgs e)
        {
            var pwWindow = new InboundConnectApiKeyWindow(InboundConnectApiKeyMiddleButton.Remove);
            pwWindow.InboundConnectApiKey = ApiKey;

            pwWindow.ShowDialogTopMost();
            if (pwWindow.Response == InboundConnectApiKeyResponse.OK)
                ApiKey = pwWindow.InboundConnectApiKey;
            else if (pwWindow.Response == InboundConnectApiKeyResponse.Remove) ApiKey = "";
        }
    }
}