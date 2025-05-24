using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RealEstateSolution.AuthService.Models;
using RealEstateSolution.WpfClient.Services;
using System;
using System.Windows;

namespace RealEstateSolution.WpfClient.ViewModels
{
    public class RegisterDialogViewModel : BindableBase, IDialogAware
    {
        private readonly AccoutService _accoutService;
        private readonly Action _closeDialog;

        private string _userName = string.Empty;
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _phoneNumber = string.Empty;

        public event Action<IDialogResult> RequestClose;

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        public DelegateCommand RegisterCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public string Title { get; set; } = "注册";

        public RegisterDialogViewModel(AccoutService accoutService, Action closeDialog)
        {
            _accoutService = accoutService;
            _closeDialog = closeDialog;
            RegisterCommand = new DelegateCommand(ExecuteRegister);
            CancelCommand = new DelegateCommand(ExecuteCancel);
        }

        private async void ExecuteRegister()
        {
            try
            {
                var request = new RegisterRequest
                {
                    UserName = UserName,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    Password = "获取PasswordBox的密码" // 需要从View中获取
                };

                var response = await _accoutService.RegisterAsync(request);

                if (response?.Success == true)
                {
                    MessageBox.Show("注册成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    _closeDialog();
                }
                else
                {
                    MessageBox.Show(response?.Message ?? "注册失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteCancel()
        {
            _closeDialog();
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
} 