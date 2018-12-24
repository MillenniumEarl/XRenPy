using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

		private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			var tabItem = e.Source as TabItem;

			if (tabItem == null)
				return;

			if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
			{
				DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.All);
			}
		}
		
		private void TabItem_Drop(object sender, DragEventArgs e)
		{
			if (e.Source != addTab && e.Source != addTabButton)
			{
				var tabItemTarget = e.Source as TabItem;
				var tabItemSource = e.Data.GetData(typeof(TabItem)) as TabItem;

				if (!tabItemTarget.Equals(tabItemSource))
				{
					var tabControl = tabItemTarget.Parent as TabControl;
					int sourceIndex = tabControl.Items.IndexOf(tabItemSource);
					int targetIndex = tabControl.Items.IndexOf(tabItemTarget);

					tabControl.Items.Remove(tabItemSource);
					tabControl.Items.Insert(targetIndex, tabItemSource);

					tabControl.Items.Remove(tabItemTarget);
					tabControl.Items.Insert(sourceIndex, tabItemTarget);
				}
			}
		}

		private void mainexp_Expanded(object sender, RoutedEventArgs e)
		{
			if (sender == projectExpander)
				optionsExpander.IsExpanded = false;
			else projectExpander.IsExpanded = false;
		}

		private void maingrid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			optionsExpander.IsExpanded = false;
			projectExpander.IsExpanded = false;
		}

		private void media_Expanded(object sender, RoutedEventArgs e)
		{
			media.Visibility = Visibility.Visible;
		}

		private void media_Collapsed(object sender, RoutedEventArgs e)
		{
			if (movieplayer.IsLoaded) movieplayer.Stop();
			imageViewer.Source = null;
			media.Visibility = Visibility.Hidden;
			show = false;
		}
	}
}
