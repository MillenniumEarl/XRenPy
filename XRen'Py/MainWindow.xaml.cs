using System.Windows;

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
			createDirectories();
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

	}
}
