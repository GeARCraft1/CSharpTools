using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Utils
{
    /// <summary>
    /// A MessageBox that can read Text and Password
    /// Please use ShowDialog();
    /// </summary>
    public partial class MessageBoxTextInput : Window
    {
        private bool internalClosing = false;
        public string ResponseText
        {
            get
            {
                if (isPassword)
                {
                    return passInput.Password;
                }
                else
                {
                    return textInput.Text;
                }
            }
            set {
                if (isPassword)
                {
                    passInput.Password = value;
                }
                else
                {
                    textInput.Text = value;
                }
            }
        }
        public bool isPassword = false;
        public MessageBoxTextInput(string question, string OKText = "OK", string title = "", bool isPassword = false)
        {
            InitializeComponent();
            Box.Title = title;
            questionLabel.Content = question;
            submit.Content = OKText;
            if (isPassword)
            {
                this.isPassword = true;
                passInput.Visibility = Visibility.Visible;
                textInput.Visibility = Visibility.Hidden;
            }

        }
        
        private void submit_Click(object sender, RoutedEventArgs e)
        {
            internalClosing = true;
            DialogResult = true;
        }

        private void Box_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!internalClosing)
            {
                DialogResult = false;
            }
        }
    }
}
