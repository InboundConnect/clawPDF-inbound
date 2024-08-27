using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using clawSoft.clawPDF.Shared.Helper;
using clawSoft.clawPDF.Shared.ViewModels;

namespace clawSoft.clawPDF.Shared.Views
{
    public partial class InboundConnectApiKeyWindow : Window
    {
        public InboundConnectApiKeyWindow(InboundConnectApiKeyMiddleButton middleButton)
        {
            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            InitializeComponent();

            switch (middleButton)
            {
                case InboundConnectApiKeyMiddleButton.Skip:
                    RemoveButton.Visibility = Visibility.Collapsed;
                    PasswordHintText.Visibility = Visibility.Collapsed;
                    break;

                default:
                    SkipButton.Visibility = Visibility.Collapsed;
                    break;
            }

            InboundConnectApiKeyViewModel.CloseViewAction = delegate (bool? result) { DialogResult = result; };
        }

        public InboundConnectApiKeyViewModel InboundConnectApiKeyViewModel => (InboundConnectApiKeyViewModel)DataContext;

        public string InboundConnectApiKey
        {
            get => InboundConnectApiKeyViewModel.InboundConnectApiKey;
            set
            {
                InboundConnectApiKeyViewModel.InboundConnectApiKey = value;
                InboundConnectApiKeyBox.Password = value;
            }
        }

        public InboundConnectApiKeyResponse Response => InboundConnectApiKeyViewModel.Response;

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            InboundConnectApiKeyViewModel.InboundConnectApiKey = InboundConnectApiKeyBox.Password;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            IntPtr hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
            ThemeHelper.ChangeTitleBar(hWnd);

            TranslationHelper.Instance.TranslatorInstance.Translate(this);
        }

        public InboundConnectApiKeyResponse ShowDialogTopMost()
        {
            TopMostHelper.ShowDialogTopMost(this, false);
            return Response;
        }
    }

    public enum InboundConnectApiKeyMiddleButton
    {
        Remove,
        Skip
    }
}