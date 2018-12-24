using System;
using System.Windows;
using System.Windows.Input;

namespace X_Ren_Py
{
    /// <summary>
    /// Логика взаимодействия для HeaderChange.xaml
    /// </summary>
    public partial class HeaderChange : Window
    {
        public HeaderChange()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            returnHeader.SelectAll();
            returnHeader.Focus();
        }

        public string Answer
        {
            get { return returnHeader.Text; }
        }

        private void returnHeader_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) DialogResult = true;
        }
    }
}
