using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using clawSoft.clawPDF.Shared.Helper;
using clawSoft.clawPDF.Shared.ViewModels;

namespace clawSoft.clawPDF.Shared.Views
{
    public partial class InboundConnectBookingNumberWindow : Window
    {
        public InboundConnectBookingNumberWindow()
        {
            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            InitializeComponent();

            InboundConnectBookingNumberViewModel.CloseViewAction = delegate (bool? result) { DialogResult = result; };
        }

        public InboundConnectBookingNumberViewModel InboundConnectBookingNumberViewModel => (InboundConnectBookingNumberViewModel)DataContext;

        public string InboundConnectBookingNumber
        {
            get => InboundConnectBookingNumberViewModel.InboundConnectBookingNumber;
            set
            {
                InboundConnectBookingNumberViewModel.InboundConnectBookingNumber = value;
            }
        }

        public InboundConnectBookingNumberResponse Response => InboundConnectBookingNumberViewModel.Response;

        private void OnBookingNumberChanged(object sender, RoutedEventArgs e)
        {
            InboundConnectBookingNumberViewModel.InboundConnectBookingNumber = InboundConnectBookingNumberInput.Text;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            IntPtr hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
            ThemeHelper.ChangeTitleBar(hWnd);

            TranslationHelper.Instance.TranslatorInstance.Translate(this);
        }

        public InboundConnectBookingNumberResponse ShowDialogTopMost()
        {
            TopMostHelper.ShowDialogTopMost(this, false);
            return Response;
        }
    }
}