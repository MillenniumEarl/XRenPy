using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Input;
using AlbumArtExtraction;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace X_Ren_Py
{
    public partial class MainWindow : Window
    {
        private void audioImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog musDialog = new OpenFileDialog() { Filter = audioextensions, Multiselect = true };

            if (musDialog.ShowDialog() == true)
                for (int file = 0; file < musDialog.FileNames.Length; file++)
                {
                    try
					{
						string name = musDialog.SafeFileNames[file];
						string currentPath = musDialog.FileNames[file];

						XAudio newaudio = new XAudio() { Header = name, Path = currentPath};
						audioMouseActions(newaudio);

						if (tabControlResources.SelectedContent == musicListView) { newaudio.Type = "music "; musicListView.Items.Add(newaudio); }
						else if (tabControlResources.SelectedContent == soundListView) { newaudio.Type = "sound "; soundListView.Items.Add(newaudio); }
						else { newaudio.Type = "voice "; voiceListView.Items.Add(newaudio); }
					}
					catch (Exception) { MessageBox.Show("Please choose the audio!", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                }
        }

		private void audioReload_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog musDialog = new OpenFileDialog() { Filter = audioextensions };
			if (musDialog.ShowDialog() == true)
				try
				{
					currentAudio.Header = musDialog.SafeFileName;
					currentAudio.Path = musDialog.FileName;
					coverartShow(musDialog.FileName);
					movieplayer.Source = new Uri(musDialog.FileName, UriKind.Absolute);
					currentAudio.TextColor = Brushes.Black;
					currentAudio.Checkbox.IsEnabled = true;
				}
				catch (Exception)
				{
					MessageBox.Show("Invalid audio", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
		}

		private void coverartShow(string audiofile)
		{
			ID3v2AlbumArtExtractor extractID3 = new ID3v2AlbumArtExtractor();
			ID3v22AlbumArtExtractor extractID3V2 = new ID3v22AlbumArtExtractor();

			BitmapImage bitmapToShow = new BitmapImage();
			bitmapToShow.BeginInit();
			bitmapToShow.CacheOption = BitmapCacheOption.OnLoad;

				if (extractID3V2.CheckType(audiofile) && extractID3V2.Extract(audiofile) != null)
					bitmapToShow.StreamSource = extractID3V2.Extract(audiofile);
				else if (extractID3.CheckType(audiofile) && extractID3.Extract(audiofile) != null)
					bitmapToShow.StreamSource = extractID3.Extract(audiofile);
				else bitmapToShow.UriSource = new Uri("Images/Music.png", UriKind.Relative);

			bitmapToShow.EndInit();
			coverart.Source = bitmapToShow;
		}

		private void audioMouseActions(XAudio newaudio)
		{
			newaudio.ContextMenu = cmAudio;
			newaudio.Selected += content_Selected;
			newaudio.MouseUp += content_Selected;
			newaudio.MouseLeave += audio_MouseLeave;
			newaudio.MouseEnter += audiomovie_Enter;
			newaudio.Checkbox.Checked += audio_Checked;
			newaudio.Checkbox.Unchecked += audio_Unchecked;
		}

		private void audio_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!show)
				media.IsExpanded = false;
			else
			{
				audioPropsPanel.Visibility = Visibility.Collapsed;
				imagePropsPanel.Visibility = Visibility.Collapsed;
				if (currentAudio != null)
				{					
					movieplayer.Source = new Uri(currentAudio.Path, UriKind.Absolute);
					mediaNameLabel.Content = currentAudio.Header;
					coverartShow(currentAudio.Path);
					if (currentAudio.IsChecked == true)
					{
						getAudioProperties(currentFrame, currentAudio);
						audioPropsPanel.Visibility = Visibility.Visible;
					}
					else audioPropsPanel.Visibility = Visibility.Hidden;
				}
			}
		}

		private void audio_Checked(object sender, RoutedEventArgs e)
        {			
			bool isLooped=false;
            currentAudio = ((sender as CheckBox).Parent as StackPanel).Parent as XAudio;
			//аудиослой для таймлайна должен будет быть тут. Если таймлайн вообще будет
			if (currentAudio.Type == "music ") isLooped = true;
				if (addorselect) AudioInFrameProps.Add(new AudioProperties() { Frame = currentFrame, Audio = currentAudio, Loop=isLooped });
            getAudioProperties(currentFrame, currentAudio);
			audioPropsPanel.Visibility = Visibility.Visible;
			show = true; 
		}
        private void audio_Unchecked(object sender, RoutedEventArgs e)
        {
            XAudio selectedAudio = ((sender as CheckBox).Parent as StackPanel).Parent as XAudio;
            string source = new Uri(selectedAudio.Path).ToString();
            if (removeorunselect) AudioInFrameProps.Remove(AudioInFrameProps.Find(i => i.Frame == currentFrame && i.Audio == selectedAudio));
            media.IsExpanded = false;
			show = false;
		}
        
        private void audioDeleteFromList_Click(object sender, RoutedEventArgs e)
        {
            foreach (AudioProperties audio in AudioInFrameProps.Where(audio => audio.Audio == sender)) AudioInFrameProps.Remove(audio);
            resourcesSelectedItem_delete();
        }
		private void stopAudio_Click(object sender, RoutedEventArgs e)
		{
			if ((sender as MenuItem).IsChecked)
				currentFrame.stopAudio = true;
			else currentFrame.stopAudio = false;
		}
		public void getAudioProperties(XFrame frame, XAudio audio)
		{
			AudioProperties property = AudioInFrameProps.Find(prop => prop.Frame == frame && prop.Audio == audio);
			fadeinTextBox.Text = property.FadeIn.ToString();
			fadeoutTextBox.Text = property.FadeOut.ToString();
			//queueCheckBox.IsChecked = property.Queue;
			loopCheckBox.IsChecked = property.Loop;
			imagePropsPanel.Visibility = Visibility.Hidden;
		}
    }
}
