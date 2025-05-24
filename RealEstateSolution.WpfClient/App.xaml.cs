using System.Windows;
using DryIoc;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Regions;
using RealEstateSolution.WpfClient.Services;
using RealEstateSolution.WpfClient.ViewModels;
using RealEstateSolution.WpfClient.Views;

namespace RealEstateSolution.WpfClient;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : PrismApplication
{
    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }
    protected override void OnInitialized()
    {
        base.OnInitialized();
        var regionManager = Container.Resolve<IRegionManager>();
        regionManager.RegisterViewWithRegion("MainRegion", typeof(LoginView));
    }
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // 注册视图和视图模型
        containerRegistry.RegisterForNavigation<LoginView, LoginViewModel>();
        containerRegistry.RegisterForNavigation<MainContent, MainContentViewModel>();
        containerRegistry.RegisterDialog<RegisterDialog, RegisterDialogViewModel>();

        // 注册服务
        containerRegistry.RegisterSingleton<AccoutService>();
        containerRegistry.RegisterSingleton<NavigationService>();
    }
}