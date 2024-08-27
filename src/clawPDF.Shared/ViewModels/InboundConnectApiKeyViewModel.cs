namespace clawSoft.clawPDF.Shared.ViewModels
{
    public class InboundConnectApiKeyViewModel : ViewModelBase
    {
        private string _inboundConnectApiKey;

        public InboundConnectApiKeyResponse Response;

        public InboundConnectApiKeyViewModel()
        {
            Response = InboundConnectApiKeyResponse.Cancel;

            OkCommand = new DelegateCommand(ExecuteOk, CanExecuteOk);
            RemoveCommand = new DelegateCommand(ExecuteRemove);
            SkipCommand = new DelegateCommand(ExecuteSkip);
        }

        public DelegateCommand OkCommand { get; protected set; }
        public DelegateCommand RemoveCommand { get; protected set; }
        public DelegateCommand SkipCommand { get; protected set; }

        public string InboundConnectApiKey
        {
            get => _inboundConnectApiKey;
            set
            {
                _inboundConnectApiKey = value;
                OkCommand.RaiseCanExecuteChanged();
            }
        }

        private void ExecuteSkip(object obj)
        {
            _inboundConnectApiKey = "";
            Response = InboundConnectApiKeyResponse.Skip;
            RaiseCloseView(true);
        }

        private void ExecuteRemove(object obj)
        {
            _inboundConnectApiKey = "";
            Response = InboundConnectApiKeyResponse.Remove;
            RaiseCloseView(true);
        }

        private bool CanExecuteOk(object obj)
        {
            return !string.IsNullOrEmpty(_inboundConnectApiKey);
        }

        private void ExecuteOk(object obj)
        {
            Response = InboundConnectApiKeyResponse.OK;
            RaiseCloseView(true);
        }
    }

    public enum InboundConnectApiKeyResponse
    {
        OK,
        Remove,
        Skip,
        Cancel
    }
}