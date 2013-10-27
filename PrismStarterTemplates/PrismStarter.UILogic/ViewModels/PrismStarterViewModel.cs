using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;

namespace PrismStarter.UILogic.ViewModels
{
    public class PrismStarterViewModel : ViewModel
    {
        private INavigationService _navigationService;

        public PrismStarterViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}
