using BigBrother.UILogic.Events;
using BigBrother.UILogic.Models;
using BigBrother.UILogic.Repositories;
using BigBrother.UILogic.Services;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace BigBrother.UILogic.ViewModels
{
    public class PhoneCallDetailPageViewModel : ViewModel
    {
        private IPhoneCallRepository _phoneCallRepository;
        private readonly INavigationService _navService;
        private readonly IEventAggregator _eventAggregator;
        private PhoneCall _phoneCall;
        private bool _loadingData;
        private int _numRowsUpdated;
        private CrudResult _crudResult;
        private string _errorMessage;
        private string _errorMessageTitle;

        public DelegateCommand GoBackCommand { get; private set; }
        public DelegateCommand NewPhoneCallCommand { get; private set; }
        public DelegateCommand UpdatePhoneCallCommand { get; private set; }
        public DelegateCommand DeletePhoneCallCommand { get; private set; }
        public Action<object> TextBoxLostFocusAction { get; private set; }

        public PhoneCallDetailPageViewModel(IPhoneCallRepository phoneCallRepository, INavigationService navService, IEventAggregator eventAggregator) {
            _phoneCallRepository = phoneCallRepository;
            _navService = navService;
            _eventAggregator = eventAggregator;
            GoBackCommand = new DelegateCommand(
                () => _navService.GoBack(),
                () => _navService.CanGoBack());
            NewPhoneCallCommand = new DelegateCommand(OnNewPhoneCall, CanNewPhoneCall);
            UpdatePhoneCallCommand = new DelegateCommand(OnUpdatePhoneCall, CanUpdatePhoneCall);
            DeletePhoneCallCommand = new DelegateCommand(OnDeletePhoneCall, CanDeletePhoneCall);
            TextBoxLostFocusAction = OnTextBoxLostFocusAction;
        }

        [RestorableState]
        public PhoneCall SelectedPhoneCall {
            get {
                return _phoneCall;
            }
            private set {
                SetProperty(ref _phoneCall, value);
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

        public int NumRowsUpdated {
            get { return _numRowsUpdated; }
            private set { SetProperty(ref _numRowsUpdated, value); }
        }

        public CrudResult CrudResult {
            get { return _crudResult; }
            private set { SetProperty(ref _crudResult, value); }
        }

        public async override void OnNavigatedTo(object navigationParameter, Windows.UI.Xaml.Navigation.NavigationMode navigationMode, Dictionary<string, object> viewModelState) {
            base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);

            // Note: Each time app selects from main page (PhoneCallListPage) detail page (PhoneCallDetailPage) is recreated.
            // Meaning that constructor is run and SelectedPhoneCall is null.
            // If SuspendAndTerminate (e.g. debug mode) SelectedPhoneCall is saved to SessionState (because of [RestorableState] attribute).
            // Therefore, if SelectedPhoneCall has been saved, use it instead of doing GetPhoneCallAsync.
            if (SelectedPhoneCall == null) {
                string errorMessage = string.Empty;
                int phoneCallId = (int)navigationParameter;

                if (phoneCallId == 0) {
                    SelectedPhoneCall = new PhoneCall();
                    SelectedPhoneCall.ValidateProperties();
                    RunAllCanExecute();
                }
                else {
                    try {
                        LoadingData = true;
                        CrudResult = await _phoneCallRepository.GetPhoneCallAsync(phoneCallId);
                        SelectedPhoneCall = JsonConvert.DeserializeObject<List<PhoneCall>>(CrudResult.Content.ToString()).FirstOrDefault<PhoneCall>();
                    }
                    catch (HttpRequestException ex) {
                        ErrorMessageTitle = ErrorMessagesHelper.HttpRequestExceptionError;
                        //TODO: Log stack trace to database here
                        ErrorMessage = string.Format("{0}", ex.Message);
                    }
                    finally {
                        LoadingData = false;
                    }
                    if (ErrorMessage != null && ErrorMessage != string.Empty) {
                        MessageDialog messageDialog = new MessageDialog(ErrorMessage, ErrorMessageTitle);
                        await messageDialog.ShowAsync();
                        _navService.GoBack();
                    }
                }
            }

            RunAllCanExecute();
        }

        // Enable New button when there is no Selected PhoneCall
        private bool CanNewPhoneCall() {
            return true;
        }

        // Disable Update button when there are Errors
        private bool CanUpdatePhoneCall() {
			if (SelectedPhoneCall != null) {
	            return SelectedPhoneCall.Errors.Errors.Count == 0;
			}
			else {
				return false;
			}
        }

        // Enable Delete button when there is a Selected PhoneCall
        private bool CanDeletePhoneCall() {
			if (SelectedPhoneCall != null) {
	            return SelectedPhoneCall.Id != 0;
			}
			else {
				return false;
			}            
        }

        // When New button is pressed
        private void OnNewPhoneCall() {
            SelectedPhoneCall = new PhoneCall();
            SelectedPhoneCall.ValidateProperties();
            RunAllCanExecute();
        }

        // When Update button is pressed
        private async void OnUpdatePhoneCall() {
            string errorMessage = string.Empty;
            bool isCreating = false;

            SelectedPhoneCall.ValidateProperties();
            var updateErrors = SelectedPhoneCall.GetAllErrors().Values.SelectMany(pc => pc).ToList();

            if (updateErrors.Count == 0) {
                try {
                    LoadingData = true;
                    if (SelectedPhoneCall.Id == 0) {
                        isCreating = true;
                        CrudResult = await _phoneCallRepository.CreatePhoneCallAsync(SelectedPhoneCall);
                        SelectedPhoneCall = JsonConvert.DeserializeObject<List<PhoneCall>>(CrudResult.Content.ToString()).FirstOrDefault<PhoneCall>();
                    }
                    else {
                        CrudResult = await _phoneCallRepository.UpdatePhoneCallAsync(SelectedPhoneCall);
                    }
                }
                catch (ModelValidationException mvex) {
                    // there were server-side validation errors
                    DisplayPhoneCallErrorMessages(mvex.ValidationResult);
                }
                catch (HttpRequestException ex) {
                    ErrorMessageTitle = isCreating ? ErrorMessagesHelper.CreateAsyncFailedError : ErrorMessagesHelper.UpdateAsyncFailedError;
                    ErrorMessage = ex.Message;
                }
                finally {
                    LoadingData = false;
                    RunAllCanExecute();
                }

                if (ErrorMessage != null && ErrorMessage != string.Empty) {
                    MessageDialog messageDialog = new MessageDialog(ErrorMessage, ErrorMessageTitle);
                    await messageDialog.ShowAsync();
                    _navService.GoBack();
                }
            }
            else {
                RunAllCanExecute();
            }
        }

        // When Delete button is pressed
        private async void OnDeletePhoneCall() {
            var messageDialog = new MessageDialog("Delete this PhoneCall?", "Delete confirmation");
            messageDialog.Commands.Add(new UICommand("Cancel", (command) =>
            {
            }));

            messageDialog.Commands.Add(new UICommand("Delete", async (command) =>
            {
                try {
                    LoadingData = true;
                    CrudResult = await _phoneCallRepository.DeletePhoneCallAsync(SelectedPhoneCall.Id);
                    _eventAggregator.GetEvent<PhoneCallDeletedEvent>().Publish(SelectedPhoneCall);
                }
                catch (HttpRequestException ex) {
                    ErrorMessageTitle = ErrorMessagesHelper.DeleteAsyncFailedError;
                    ErrorMessage = ex.Message;
                }
                finally {
                    LoadingData = false;
                    RunAllCanExecute();
                    _navService.GoBack();
                }
            }));

            messageDialog.DefaultCommandIndex = 0;
            await messageDialog.ShowAsync();

            if (ErrorMessage != null && ErrorMessage != string.Empty) {
                messageDialog = new MessageDialog(ErrorMessage, ErrorMessageTitle);
                await messageDialog.ShowAsync();
                _navService.GoBack();
            }
        }

        // If any Model rules broken, set SelectedCommonDataType Errors collection 
        // which are data bound to UI error textblocks.
        private void DisplayPhoneCallErrorMessages(ModelValidationResult modelValidationResult) {
            var phoneCallUpdateErrors = new Dictionary<string, ReadOnlyCollection<string>>();

            // Property keys format: address.{Propertyname}
            foreach (var propkey in modelValidationResult.ModelState.Keys) {
                string propertyName = propkey.Substring(propkey.IndexOf('.') + 1); // strip off order. prefix

                // 'modelValidationResults.ModelState[propkey]' is the collection of string error messages
                // for the property. Later on in UILayer, FirstErrorConverter will display the one of the collection.
                // 'propertyName' will only occur once for each property in the Model so a new ReadOnlyCollection
                // can be created on each pass of the foreach loop.
                phoneCallUpdateErrors.Add(propertyName, new ReadOnlyCollection<string>(modelValidationResult.ModelState[propkey]));
            }

            if (phoneCallUpdateErrors.Count > 0) {
                SelectedPhoneCall.Errors.SetAllErrors(phoneCallUpdateErrors);
            }
        }

        // When a TextBox loses focus, RunAllCanExecute().
        private void OnTextBoxLostFocusAction(object parameter) {
            RunAllCanExecute();
        }

        // Run 'CanExecute' for all buttons
        private void RunAllCanExecute() {
            NewPhoneCallCommand.RaiseCanExecuteChanged();
            UpdatePhoneCallCommand.RaiseCanExecuteChanged();
            DeletePhoneCallCommand.RaiseCanExecuteChanged();           
        }
    }
}
