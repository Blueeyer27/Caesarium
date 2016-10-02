using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Caesarium.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private RelayCommand<object> _loginCommand;
        public RelayCommand<object> LoginCommand 
        { 
            get { return _loginCommand ?? (_loginCommand = new RelayCommand<object>(param => Login(param))); }
            set { _loginCommand = value; } 
        }

        public LoginViewModel()
        {
            //LoginCommand = new RelayCommand<object>(param => Login(param));
        }

        private bool isAuthenticated;

        private string _loginError;
        public string LoginError
        {
            get { return _loginError; }
            set
            {
                _loginError = value;
                RaisePropertyChanged("LoginError");
            }
        }

        private string _userLogin;
        public string UserLogin
        {
            get { return _userLogin; }
            set
            {
                _userLogin = value;
                RaisePropertyChanged("UserLogin");
            }
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            LoginError = "Incorrect login or password! \n Please try again...";
        }

        public void Login(object param)
        {
            var passwordBox = param as PasswordBox;
            String password = passwordBox.Password;

            try
            {
                isAuthenticated = (!String.IsNullOrWhiteSpace(UserLogin) && !String.IsNullOrWhiteSpace(password));
            }
            catch (ArgumentException)
            {
                isAuthenticated = false;
            }

            if (!isAuthenticated) LoginError = "Incorrect login or password! \n Please try again...";
            else
            {
                LoginError = "";
                var mainWindow = new MainWindow();
                mainWindow.Show();
                Application.Current.MainWindow.Close(); //Closing current window
            }
        }
    }
}
