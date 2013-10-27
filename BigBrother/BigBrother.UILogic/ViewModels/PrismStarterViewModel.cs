using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;

namespace BigBrother.UILogic.ViewModels
{
    public class BigBrotherViewModel : ViewModel
    {
        private INavigationService _navigationService;

        public BigBrotherViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}
