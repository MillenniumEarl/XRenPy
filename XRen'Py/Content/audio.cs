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
						if (!(tabControlResources.SelectedContent as ListView).Items.OfType<XAudio>().Any(item => item.Header == name))
						{
							XAudio audio = new XAudio() { Header = name };
							audioMouseActions(audio);

							if (tabControlResources.SelectedContent == musicListView)
							{
								audio.Type = "music "; musicListView.Items.Add(audio);
								audio.Path = projectFolder + musicFolder + name;
							}
							else if (tabControlResources.SelectedContent == soundListView)
							{
								audio.Type = "sound "; soundListView.Items.Add(audio);
								audio.Path = projectFolder + soundsFolder  + name;
							}
							else
							{
								audio.Type = "voice "; voiceListView.Items.Add(audio);
								audio.Path = projectFolder + voicesFolder + name;
							}

							contentCollector(musDialog.FileNames[file], audio.Path);
						}
						else MessageBox.Show("Audio is already in use!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
			newaudio.Checkbox.Indeterminate += audio_Indeterminate;
		}

		private void audio_Indeterminate(object sender, RoutedEventArgs e)
		{
			if (waschecked && addorselect) (sender as CheckBox).IsChecked = false;

			//оставим это на таймлайн
			//else { imageBackground.Source = imageShow(((sender as CheckBox).Tag as XImage).Path); };
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
            currentAudio = (sender as CheckBox).Tag as XAudio;
			//аудиослой для таймлайна должен будет быть тут. Если таймлайн вообще будет
			if (currentAudio.Type == "music ") isLooped = true;
				if (addorselect) AudioInFrameProps.Add(new AudioProperties() { Frame = currentFrame, Audio = currentAudio, Loop=isLooped });
            getAudioProperties(currentFrame, currentAudio);
			audioPropsPanel.Visibility = Visibility.Visible;
			show = true;
			waschecked = true;
		}
        private void audio_Unchecked(object sender, RoutedEventArgs e)
		{
			if (waschecked)
			{
				XAudio selectedAudio = (sender as CheckBox).Tag as XAudio;

				//string source = new Uri(selectedAudio.Path).ToString();
				if (removeorunselect) AudioInFrameProps.Remove(AudioInFrameProps.Find(i => i.Frame == currentFrame && i.Audio == selectedAudio));
				media.IsExpanded = false;
				show = false;
				waschecked = false;
			}
			else
			{
				XAudio selectedAudio = (sender as CheckBox).Tag as XAudio;
				//string source = new Uri(selectedAudio.Path).ToString();
				if (removeorunselect) AudioInFrameProps.Find(i => i.Frame == currentFrame && i.Audio == selectedAudio).StopFrame = currentFrame;
				media.IsExpanded = false;
				show = false;
				waschecked = false;
			}
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

		private void audioMixer_Click(object sender, RoutedEventArgs e)
		{
			if (mixerButton.Background == Brushes.LightBlue)
			{
				mixerButton.Background = new SolidColorBrush(Color.FromArgb(102, 255, 255, 255));
				mixerGrid.Visibility = Visibility.Collapsed;
			}
			else
			{
				mixerButton.Background = Brushes.LightBlue;
				mixerGrid.Visibility = Visibility.Visible;
			}
			//XFrame duplicate = createFrame();
			//duplicate.Text = currentFrame.Text;
			//duplicate.isMenu = currentFrame.isMenu;
			//duplicate.MenuOptions = currentFrame.MenuOptions;
			//duplicate.Character = currentFrame.Character;
			////duplicate.BackgroundImage = currentFrame.BackgroundImage;
			//duplicate.Movie = currentFrame.Movie;

			//List<ImageCharProperties> newimageprops = new List<ImageCharProperties>();
			////существующую коллекцию нельзя менять во время прохождения по ней, а объединить коллекции проще пока не удалось. Потому придется использовать два перечислителя
			//foreach (ImageCharProperties i in ImageInFrameProps)
			//{
			//	if (i.Frame == currentFrame)
			//	{
			//		ImageCharProperties newprop = new ImageCharProperties() { Frame = duplicate, Image = i.Image };
			//		newimageprops.Add(newprop);
			//	}
			//}
			//foreach (ImageCharProperties i in newimageprops) { ImageInFrameProps.Add(i); }

			//List<AudioProperties> newaudioprops = new List<AudioProperties>();
			////существующую коллекцию нельзя менять во время прохождения по ней, а объединить коллекции проще пока не удалось. Потому придется использовать два перечислителя
			//foreach (AudioProperties i in AudioInFrameProps)
			//{
			//	if (i.Frame == currentFrame)
			//	{
			//		AudioProperties newprop = new AudioProperties() { Frame = duplicate, Audio = i.Audio };
			//		newaudioprops.Add(newprop);
			//	}
			//}
			//foreach (AudioProperties i in newaudioprops) { AudioInFrameProps.Add(i); }

			//getSelectedList().Items.Insert(getSelectedList().Items.IndexOf(currentFrame) + 1, duplicate);
			//duplicate.IsSelected = true;
		}
	}
}
