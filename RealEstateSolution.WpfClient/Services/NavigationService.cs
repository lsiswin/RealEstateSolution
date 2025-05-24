using Prism.Regions;
using System;

namespace RealEstateSolution.WpfClient.Services
{
    public class NavigationService
    {
        private static IRegionManager? _regionManager;

        public  void Initialize(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public static void NavigateTo(string viewName, object? parameters = null)
        {
            if (_regionManager == null)
            {
                throw new InvalidOperationException("导航服务未初始化");
            }

            _regionManager.RequestNavigate("MainRegion", viewName, new NavigationParameters
            {
                { "params", parameters }
            });
        }

        public static void GoBack()
        {
            if (_regionManager == null)
            {
                throw new InvalidOperationException("导航服务未初始化");
            }

            var journal = _regionManager.Regions["MainRegion"].NavigationService.Journal;
            if (journal.CanGoBack)
            {
                journal.GoBack();
            }
        }
    }
} 