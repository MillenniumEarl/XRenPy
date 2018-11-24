using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace X_Ren_Py
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            initializeAll();
        }

        //string correctChecker(string text)
        //{   //перенос строки
        //    if (text.Contains("\n")) { }
        //    //двойные кавычки
        //    if (text.Contains("\""))
        //        text = text.Replace("\"", "\\\"");
        //    //кириллица
        //    Regex reg = new Regex("[а-яёА-ЯЁ]+");
        //    if (reg.IsMatch(text))
        //        text = "u" + text;
        //    return text;
        //}

		private void textBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			currentFrame.Text = textBox.Text;
		}

		private void floatTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			char input = e.Text.ToCharArray()[0];
			if (!char.IsDigit(input))
				{
					if (!((sender as TextBox).Text.Length == 1 && (input == '.' || input == ',')))
						e.Handled = true;
				}	
		}

	}
}
