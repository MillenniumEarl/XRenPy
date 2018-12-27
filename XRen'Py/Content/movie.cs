using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;

namespace X_Ren_Py
{
    public partial class MainWindow : Window
    {
        private void movieImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog vidDialog = new OpenFileDialog() { Filter = vidextensions, Multiselect = true };

			if (vidDialog.ShowDialog() == true)
				for (int file = 0; file < vidDialog.FileNames.Length; file++)
				{
					try
					{
						string name = vidDialog.SafeFileNames[file];
						if (!(tabControlResources.SelectedContent as ListView).Items.OfType<XMovie>().Any(item => item.Header == name))
						{
							string currentPath = vidDialog.FileNames[file];

							XMovie newmovie = new XMovie() { Header = name, Path = currentPath };
							movieMouseActions(newmovie);
							movieListView.Items.Add(newmovie);
						}
						else MessageBox.Show("Movie is already in use!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
					catch (Exception) { MessageBox.Show("Please choose the movie!", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
				}
        }

		private void movieReload_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog vidDialog = new OpenFileDialog() { Filter = vidextensions };

			if (vidDialog.ShowDialog() == true)
				try
				{
					currentMovie.Header = vidDialog.SafeFileName;
					currentMovie.Path = vidDialog.FileName;
					movieplayer.Source = new Uri(vidDialog.FileName, UriKind.Absolute);
					currentMovie.TextColor = Brushes.Black;
					currentMovie.Checkbox.IsEnabled = true;
				}
				catch (Exception)
				{
					MessageBox.Show("Invalid movie", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
		}

		private void movieMouseActions(XMovie newmovie)
		{
			newmovie.ContextMenu = cmMovie;
			newmovie.Selected += content_Selected;
			newmovie.MouseUp += content_Selected;
			newmovie.MouseLeave += movie_MouseLeave;
			newmovie.MouseEnter += audiomovie_Enter;
			newmovie.Checkbox.Checked += movie_Checked;
			newmovie.Checkbox.Unchecked += movie_Unchecked;
		}

		private void movie_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!show)
				media.IsExpanded = false;
			else
			{
				audioPropsPanel.Visibility = Visibility.Collapsed;
				imagePropsPanel.Visibility = Visibility.Collapsed;
				if (currentMovie != null)
				{	
					mediaNameLabel.Content = currentMovie.Header;
					movieplayer.Source = new Uri(currentMovie.Path, UriKind.Absolute);
				}
			}
		}

		private void movie_Checked(object sender, RoutedEventArgs e)
        {			            
			currentMovie= ((sender as CheckBox).Parent as StackPanel).Parent as XMovie;

			if (lastMovieChecked != null && lastMovieChecked != currentMovie) lastMovieChecked.IsChecked = false;
			lastMovieChecked = currentMovie;

			if (addorselect) currentFrame.Movie = currentMovie;
            movieBackground.Source = new Uri(currentMovie.Path);
			show = true;
        }
        private void movie_Unchecked(object sender, RoutedEventArgs e)
        {
            XMovie selectedMovie = (sender as CheckBox).Parent as XMovie;
            if (removeorunselect) currentFrame.Movie = null;
            movieBackground.Source = null;
            imageBackground.Source = null;
            show = false;
        }
       
        private void deleteVideo_Click(object sender, RoutedEventArgs e)        { resourcesSelectedItem_delete(); }
        
    }
}
