using System.Windows;
using PerMonitorDPI;

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
			new PerMonitorDpiBehavior(this);
		}
	}
}
