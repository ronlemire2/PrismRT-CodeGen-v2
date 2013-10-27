using BigBrother.UILogic.Models;
using BigBrother.UILogic.Repositories;
using BigBrother.UILogic.Services;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Windows.UI.Popups;
using Newtonsoft.Json;

namespace BigBrother.UILogic.ViewModels
{
    public class PhoneCallListPageViewModel : ViewModel
    {
        private IPhoneCallRepository _phoneCallRepository;
        private readonly INavigationService _navService;
        private readonly IEventAggregator _eventAggregator; 
        private IReadOnlyCollection<PhoneCall> _phoneCallList;
        private bool _loadingData;
        private string _errorMessage;
        private string _errorMessageTitle;
 
        public PhoneCallListPageViewModel(IPhoneCallRepository phoneCallRepository, INavigationService navService, IEventAggregator eventAggregator) {
            _phoneCallRepository = phoneCallRepository;
            _navService = navService;
            _eventAggregator = eventAggregator;
            NavCommand = new DelegateCommand<PhoneCall>(OnNavCommand);
            PhoneCallDetailNavCommand = new DelegateCommand(() => _navService.Navigate("PhoneCallDetail", 0));
       }

        public DelegateCommand<PhoneCall> NavCommand { get; set; }
        public DelegateCommand PhoneCallDetailNavCommand { get; set; }

        public IReadOnlyCollection<PhoneCall> PhoneCallList { 
            get {
                return _phoneCallList;
            }
            private set {
                SetProperty(ref _phoneCallList, value);
            }
        }

        public bool LoadingData {
            get { return _loadingData; }
            private set { SetProperty(ref _loadingData, value); }
        }

        public string ErrorMessage {
            get { return _errorMessage; }
            private set { SetProperty(ref _errorMessage, value); }
        }

        public string ErrorMessageTitle {
            get { return _errorMessageTitle; }
            private set { SetProperty(ref _errorMessageTitle, value); }
        }

        public async override void OnNavigatedTo(object navigationParameter, Windows.UI.Xaml.Navigation.NavigationMode navigationMode, Dictionary<string, object> viewModelState) {
            base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);

            ErrorMessageTitle = string.Empty;
            ErrorMessage = string.Empty;

            try {
                LoadingData = true;
                CrudResult crudResult = await _phoneCallRepository.GetPhoneCallsAsync();
                PhoneCallList = JsonConvert.DeserializeObject<List<PhoneCall>>(crudResult.Content.ToString());
            }
            catch (HttpRequestException ex) {
                ErrorMessageTitle = ErrorMessagesHelper.GetAllAsyncFailedError;
                ErrorMessage = string.Format("{0}{1}", Environment.NewLine, ex.Message);
            }
            catch (Exception ex) {
                ErrorMessageTitle = ErrorMessagesHelper.ExceptionError;
                ErrorMessage = string.Format("{0}{1}", Environment.NewLine, ex.Message);
            }
            finally {
                LoadingData = false;
            }
            if (ErrorMessage != null && ErrorMessage != string.Empty) {
                MessageDialog messageDialog = new MessageDialog(ErrorMessage, ErrorMessageTitle);
                await messageDialog.ShowAsync();
            }
        }

        private void OnNavCommand(PhoneCall phoneCall) {
            _navService.Navigate("PhoneCallDetail", phoneCall.Id);
        }


    }
}
