using PrismTable.UILogic.Events;
using PrismTable.UILogic.Models;
using PrismTable.UILogic.Repositories;
using PrismTable.UILogic.Services;
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

namespace PrismTable.UILogic.ViewModels
{
    public class EntityDetailPageViewModel : ViewModel
    {
        private IEntityRepository _entityRepository;
        private readonly INavigationService _navService;
        private readonly IEventAggregator _eventAggregator;
        private Entity _entity;
        private bool _loadingData;
        private int _numRowsUpdated;
        private CrudResult _crudResult;
        private string _errorMessage;
        private string _errorMessageTitle;

        public DelegateCommand GoBackCommand { get; private set; }
        public DelegateCommand NewEntityCommand { get; private set; }
        public DelegateCommand UpdateEntityCommand { get; private set; }
        public DelegateCommand DeleteEntityCommand { get; private set; }
        public Action<object> TextBoxLostFocusAction { get; private set; }

        public EntityDetailPageViewModel(IEntityRepository entityRepository, INavigationService navService, IEventAggregator eventAggregator) {
            _entityRepository = entityRepository;
            _navService = navService;
            _eventAggregator = eventAggregator;
            GoBackCommand = new DelegateCommand(
                () => _navService.GoBack(),
                () => _navService.CanGoBack());
            NewEntityCommand = new DelegateCommand(OnNewEntity, CanNewEntity);
            UpdateEntityCommand = new DelegateCommand(OnUpdateEntity, CanUpdateEntity);
            DeleteEntityCommand = new DelegateCommand(OnDeleteEntity, CanDeleteEntity);
            TextBoxLostFocusAction = OnTextBoxLostFocusAction;
        }

        [RestorableState]
        public Entity SelectedEntity {
            get {
                return _entity;
            }
            private set {
                SetProperty(ref _entity, value);
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

            // Note: Each time app selects from main page (EntityListPage) detail page (EntityDetailPage) is recreated.
            // Meaning that constructor is run and SelectedEntity is null.
            // If SuspendAndTerminate (e.g. debug mode) SelectedEntity is saved to SessionState (because of [RestorableState] attribute).
            // Therefore, if SelectedEntity has been saved, use it instead of doing GetEntityAsync.
            if (SelectedEntity == null) {
                string errorMessage = string.Empty;
                int entityId = (int)navigationParameter;

                if (entityId == 0) {
                    SelectedEntity = new Entity();
                    SelectedEntity.ValidateProperties();
                    RunAllCanExecute();
                }
                else {
                    try {
                        LoadingData = true;
                        CrudResult = await _entityRepository.GetEntityAsync(entityId);
                        SelectedEntity = JsonConvert.DeserializeObject<List<Entity>>(CrudResult.Content.ToString()).FirstOrDefault<Entity>();
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

        // Enable New button when there is no Selected Entity
        private bool CanNewEntity() {
            return true;
        }

        // Disable Update button when there are Errors
        private bool CanUpdateEntity() {
			if (SelectedEntity != null) {
	            return SelectedEntity.Errors.Errors.Count == 0;
			}
			else {
				return false;
			}
        }

        // Enable Delete button when there is a Selected Entity
        private bool CanDeleteEntity() {
			if (SelectedEntity != null) {
	            return SelectedEntity.Id != 0;
			}
			else {
				return false;
			}            
        }

        // When New button is pressed
        private void OnNewEntity() {
            SelectedEntity = new Entity();
            SelectedEntity.ValidateProperties();
            RunAllCanExecute();
        }

        // When Update button is pressed
        private async void OnUpdateEntity() {
            string errorMessage = string.Empty;
            bool isCreating = false;

            SelectedEntity.ValidateProperties();
            var updateErrors = SelectedEntity.GetAllErrors().Values.SelectMany(pc => pc).ToList();

            if (updateErrors.Count == 0) {
                try {
                    LoadingData = true;
                    if (SelectedEntity.Id == 0) {
                        isCreating = true;
                        CrudResult = await _entityRepository.CreateEntityAsync(SelectedEntity);
                        SelectedEntity = JsonConvert.DeserializeObject<List<Entity>>(CrudResult.Content.ToString()).FirstOrDefault<Entity>();
                    }
                    else {
                        CrudResult = await _entityRepository.UpdateEntityAsync(SelectedEntity);
                    }
                }
                catch (ModelValidationException mvex) {
                    // there were server-side validation errors
                    DisplayEntityErrorMessages(mvex.ValidationResult);
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
        private async void OnDeleteEntity() {
            var messageDialog = new MessageDialog("Delete this Entity?", "Delete confirmation");
            messageDialog.Commands.Add(new UICommand("Cancel", (command) =>
            {
            }));

            messageDialog.Commands.Add(new UICommand("Delete", async (command) =>
            {
                try {
                    LoadingData = true;
                    CrudResult = await _entityRepository.DeleteEntityAsync(SelectedEntity.Id);
                    _eventAggregator.GetEvent<EntityDeletedEvent>().Publish(SelectedEntity);
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
        private void DisplayEntityErrorMessages(ModelValidationResult modelValidationResult) {
            var entityUpdateErrors = new Dictionary<string, ReadOnlyCollection<string>>();

            // Property keys format: address.{Propertyname}
            foreach (var propkey in modelValidationResult.ModelState.Keys) {
                string propertyName = propkey.Substring(propkey.IndexOf('.') + 1); // strip off order. prefix

                // 'modelValidationResults.ModelState[propkey]' is the collection of string error messages
                // for the property. Later on in UILayer, FirstErrorConverter will display the one of the collection.
                // 'propertyName' will only occur once for each property in the Model so a new ReadOnlyCollection
                // can be created on each pass of the foreach loop.
                entityUpdateErrors.Add(propertyName, new ReadOnlyCollection<string>(modelValidationResult.ModelState[propkey]));
            }

            if (entityUpdateErrors.Count > 0) {
                SelectedEntity.Errors.SetAllErrors(entityUpdateErrors);
            }
        }

        // When a TextBox loses focus, RunAllCanExecute().
        private void OnTextBoxLostFocusAction(object parameter) {
            RunAllCanExecute();
        }

        // Run 'CanExecute' for all buttons
        private void RunAllCanExecute() {
            NewEntityCommand.RaiseCanExecuteChanged();
            UpdateEntityCommand.RaiseCanExecuteChanged();
            DeleteEntityCommand.RaiseCanExecuteChanged();           
        }
    }
}
