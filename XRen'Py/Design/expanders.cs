using System.Windows;

namespace X_Ren_Py
{
	public partial class MainWindow : Window
	{
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

		private void char_Collapsed(object sender, RoutedEventArgs e)
		{
			if (characterListView.SelectedItem != null)
			{
				saveCharacter();
			}
		}
	}
}
