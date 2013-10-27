using PrismTable.UILogic.Models;
using PrismTable.UILogic.Repositories;
using PrismTable.UILogic.Services;
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

namespace PrismTable.UILogic.ViewModels
{
    public class EntityListPageViewModel : ViewModel
    {
        private IEntityRepository _entityRepository;
        private readonly INavigationService _navService;
        private readonly IEventAggregator _eventAggregator; 
        private IReadOnlyCollection<Entity> _entityList;
        private bool _loadingData;
        private string _errorMessage;
        private string _errorMessageTitle;
 
        public EntityListPageViewModel(IEntityRepository entityRepository, INavigationService navService, IEventAggregator eventAggregator) {
            _entityRepository = entityRepository;
            _navService = navService;
            _eventAggregator = eventAggregator;
            NavCommand = new DelegateCommand<Entity>(OnNavCommand);
            EntityDetailNavCommand = new DelegateCommand(() => _navService.Navigate("EntityDetail", 0));
       }

        public DelegateCommand<Entity> NavCommand { get; set; }
        public DelegateCommand EntityDetailNavCommand { get; set; }

        public IReadOnlyCollection<Entity> EntityList { 
            get {
                return _entityList;
            }
            private set {
                SetProperty(ref _entityList, value);
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
                CrudResult crudResult = await _entityRepository.GetEntitiesAsync();
                EntityList = JsonConvert.DeserializeObject<List<Entity>>(crudResult.Content.ToString());
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

        private void OnNavCommand(Entity entity) {
            _navService.Navigate("EntityDetail", entity.Id);
        }


    }
}
