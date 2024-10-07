namespace clawSoft.clawPDF.Shared.ViewModels
{
    public class InboundConnectBookingNumberViewModel : ViewModelBase
    {
        private string _inboundConnectBookingNumber;

        public InboundConnectBookingNumberResponse Response;

        public InboundConnectBookingNumberViewModel()
        {
            Response = InboundConnectBookingNumberResponse.Cancel;

            OkCommand = new DelegateCommand(ExecuteOk, CanExecuteOk);
        }

        public DelegateCommand OkCommand { get; protected set; }
        public DelegateCommand RemoveCommand { get; protected set; }
        public DelegateCommand SkipCommand { get; protected set; }

        public string InboundConnectBookingNumber
        {
            get => _inboundConnectBookingNumber;
            set
            {
                _inboundConnectBookingNumber = value;
                OkCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanExecuteOk(object obj)
        {
            return !string.IsNullOrEmpty(_inboundConnectBookingNumber);
        }

        private void ExecuteOk(object obj)
        {
            Response = InboundConnectBookingNumberResponse.OK;
            RaiseCloseView(true);
        }
    }

    public enum InboundConnectBookingNumberResponse
    {
        OK,
        Cancel
    }
}