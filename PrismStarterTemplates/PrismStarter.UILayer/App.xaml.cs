using PrismStarter.UILogic.Models;
using PrismStarter.UILogic.Services;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Globalization;
using Windows.ApplicationModel.Activation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace PrismStarter.UILayer
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : MvvmAppBase
    {
        private IUnityContainer _container = new UnityContainer();
        private IEventAggregator _eventAggregator;

        public App() {
            this.InitializeComponent();
        }

        protected override void OnLaunchApplication(LaunchActivatedEventArgs args) {
            NavigationService.Navigate("PrismStarter", null);
        }

        protected override void OnInitialize(IActivatedEventArgs args) {
            base.OnInitialize(args);

            _eventAggregator = new EventAggregator();

            // UnityContainer->RegisterInstance
            // Default is singleton with lifetime of container scope

            // UnityContainer->Register Type
            // Default is to create a new instance when resolved or injected
            // Using Lifetime Managers - http://msdn.microsoft.com/en-us/library/ff647854.aspx
            // Lifetime managers - classes that control how and when instances are created by the Unity container.
            // A LifetimeManager that holds onto the instance given to it. When the ContainerControlledLifetimeManager is disposed, the instance is disposed with it.

            _container.RegisterInstance<IEventAggregator>(_eventAggregator);

            // Register repositories


            // Register web service proxies


            // Register NavigationService 
            // Option 1: factory method used when ViewModel is created.
            // Now whenever the ViewModel locator View-to-ViewModel type mapping happens, 
            // it will then look to see if there is a registered factory method for the ViewModel type, 
            // and if so will use it to construct the view model instead of using the default logic that just constructs the ViewModel with a default constructor. 
            // You can see in the snippet above that this allows me to pass in the NavigationService from the MvvmAppBase class as the implementation of INavigationService 
            // that the view model is depending on.
            //ViewModelLocator.Register(typeof(EntityListPage).ToString(),
            //    () => new EntityListPageViewModel(NavigationService));

            // Register NavigationService 
            // Option 2: Dependency Injection Container
            _container.RegisterInstance<INavigationService>(NavigationService);

            _container.RegisterInstance<ISessionStateService>(SessionStateService);

            ViewModelLocator.SetDefaultViewModelFactory(
                (viewModelType) => _container.Resolve(viewModelType));

            ViewModelLocator.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewModelTypeName = string.Format(CultureInfo.InvariantCulture, "PrismStarter.UILogic.ViewModels.{0}ViewModel, PrismStarter.UILogic, Version=1.0.0.0, Culture=neutral", viewType.Name);
                var viewModelType = Type.GetType(viewModelTypeName);
                return viewModelType;
            });
        }

        protected override void OnRegisterKnownTypesForSerialization() {
            base.OnRegisterKnownTypesForSerialization();
        }
    }
}
