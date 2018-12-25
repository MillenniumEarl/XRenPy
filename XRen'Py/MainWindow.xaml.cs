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
			var label = e.Source as XLabel;

			if (label == null)
				return;

			if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
			{
				DragDrop.DoDragDrop(label, label, DragDropEffects.All);
			}
		}
		
		private void TabItem_Drop(object sender, DragEventArgs e)
		{
			if (e.Source != addTab && e.Source != addTabButton)
			{
				XLabel labelTarget;
				labelTarget = ((e.Source as Label).Parent as StackPanel).Parent as XLabel;

				XLabel labelSource = e.Data.GetData(typeof(XLabel)) as XLabel;

				if (!labelTarget.Equals(labelSource))
				{
					var tabControl = labelTarget.Parent as TabControl;
					int sourceIndex = tabControl.Items.IndexOf(labelSource);
					int targetIndex = tabControl.Items.IndexOf(labelTarget);

					tabControl.Items.Remove(labelSource);
					tabControl.Items.Insert(targetIndex, labelSource);

					tabControl.Items.Remove(labelTarget);
					tabControl.Items.Insert(sourceIndex, labelTarget);
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

		public string eQuote(string content) { return "=\"" + content + "\""; }
		public string esQuote(string content) { return " = \"" + content + "\""; }
		public string quote(string content) { return "\"" + content + "\""; }
		private string simplifyTransition(string transition) { if (transition.IndexOf('(') > 0) return transition.Substring(0, transition.IndexOf('(')).ToLower(); else return transition; }
		public static string value(string info)
		{
			info = info.Substring(info.IndexOf('=') + 1).TrimStart(' ').TrimEnd(':');

			if (info.StartsWith("\"")) info = info.Trim('"');
			else
			if (info.StartsWith("'")) info = info.Trim('\'');

			return info;
		}
	}
}
