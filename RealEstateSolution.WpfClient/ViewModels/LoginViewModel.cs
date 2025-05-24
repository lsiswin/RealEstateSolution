using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RealEstateSolution.AuthService.Models;
using RealEstateSolution.WpfClient.Services;
using RealEstateSolution.WpfClient.Utils;
using RealEstateSolution.WpfClient.Views;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RealEstateSolution.WpfClient.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private readonly AccoutService _accoutService;
        private readonly IRegionManager _regionManager;
        private string _currentCaptchaCode;

        private string _phone = string.Empty;
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _verificationCode = string.Empty;
        public string VerificationCode
        {
            get => _verificationCode;
            set => SetProperty(ref _verificationCode, value);
        }

        private bool _isAgent = true;
        public bool IsAgent
        {
            get => _isAgent;
            set
            {
                SetProperty(ref _isAgent, value);
                RaisePropertyChanged(nameof(CanRegister));
            }
        }

        private BitmapSource _captchaImage;
        public BitmapSource CaptchaImage
        {
            get => _captchaImage;
            set => SetProperty(ref _captchaImage, value);
        }

        public bool CanRegister => !IsAgent;

        private bool _isLoginMode = true;
        public bool IsLoginMode
        {
            get => _isLoginMode;
            set => SetProperty(ref _isLoginMode, value);
        }

        public DelegateCommand LoginCommand { get; }
        public DelegateCommand SwitchModeCommand { get; }
        public DelegateCommand RefreshCaptchaCommand { get; }
        public DelegateCommand ShowRegisterDialogCommand { get; }

        public LoginViewModel(AccoutService accoutService, IRegionManager regionManager)
        {
            _accoutService = accoutService;
            _regionManager = regionManager;

            LoginCommand = new DelegateCommand(ExecuteLogin);
            SwitchModeCommand = new DelegateCommand(ExecuteSwitchMode);
            RefreshCaptchaCommand = new DelegateCommand(ExecuteRefreshCaptcha);
            ShowRegisterDialogCommand = new DelegateCommand(ExecuteShowRegisterDialog);

            // 初始化验证码
            ExecuteRefreshCaptcha();
        }

        private void ExecuteShowRegisterDialog()
        {
            throw new NotImplementedException();
        }

        private async void ExecuteLogin()
        {
            if (string.IsNullOrWhiteSpace(VerificationCode) || 
                !VerificationCode.Equals(_currentCaptchaCode, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("验证码错误，请重新输入。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                ExecuteRefreshCaptcha();
                return;
            }

            try
            {
                var request = new LoginRequest
                {
                    UserName = Phone,
                    Password = Password,
                    RememberMe = true
                };

                var response = await _accoutService.LoginAsync(request);

                if (response?.Success == true)
                {
                    _regionManager.RequestNavigate("MainRegion", "MainContent");
                }
                else
                {
                    MessageBox.Show(response?.Message ?? "登录失败，请检查输入信息。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    ExecuteRefreshCaptcha();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                ExecuteRefreshCaptcha();
            }
        }

        private void ExecuteSwitchMode()
        {
            if (!CanRegister) return;
            IsLoginMode = !IsLoginMode;
            ExecuteRefreshCaptcha();
        }

        private void ExecuteRefreshCaptcha()
        {
            var (image, code) = CaptchaGenerator.Generate();
            CaptchaImage = image;
            _currentCaptchaCode = code;
            VerificationCode = string.Empty;
        }
    }
} 