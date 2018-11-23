using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Input;

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

						XAudio newaudio = new XAudio() { Content = name, Path = currentPath};
						audioMouseActions(newaudio);

						if (tabControlResources.SelectedContent == musicListView) { newaudio.Type = "music "; musicListView.Items.Add(newaudio); }
						else if (tabControlResources.SelectedContent == soundListView) { newaudio.Type = "sound "; soundListView.Items.Add(newaudio); }
						else { newaudio.Type = "voice "; voiceListView.Items.Add(newaudio); }
					}
					catch (Exception) { MessageBox.Show("Please choose the audio!", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                }
        }

		private void audioMouseActions(XAudio newaudio)
		{
			newaudio.ContextMenu = cmAudio;
			newaudio.MouseUp += content_MouseUp;
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
					movieplayer.Source = new Uri(currentAudio.Path.ToString(), UriKind.Absolute);
					mediaNameLabel.Content = currentAudio.Content;
					coverartShow(currentAudio.Path.ToString());
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
            selectCheckedItem(sender);
			bool isLooped=false;
            currentAudio = (sender as CheckBox).Parent as XAudio;
			//аудиослой для таймлайна должен будет быть тут. Если таймлайн вообще будет
			if (currentAudio.Type == "music ") isLooped = true;
				if (addorselect) AudioInFrameProps.Add(new AudioProperties() { Frame = currentFrame, Audio = currentAudio, Loop=isLooped });
            getAudioProperties(currentFrame, currentAudio);
			audioPropsPanel.Visibility = Visibility.Visible;
			show = true;         
		}
        private void audio_Unchecked(object sender, RoutedEventArgs e)
        {
            selectCheckedItem(sender);

            XAudio selectedAudio = (sender as CheckBox).Parent as XAudio;
            string source = new Uri(selectedAudio.Path).ToString();
            if (removeorunselect) AudioInFrameProps.Remove(AudioInFrameProps.Where(i => i.Frame == currentFrame && i.Audio == selectedAudio).Single());
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
			AudioProperties property = AudioInFrameProps.Where(prop => prop.Frame == frame && prop.Audio == audio).Single();
			fadeinTextBox.Text = property.FadeIn.ToString();
			fadeoutTextBox.Text = property.FadeOut.ToString();
			//queueCheckBox.IsChecked = property.Queue;
			loopCheckBox.IsChecked = property.Loop;
			imagePropsPanel.Visibility = Visibility.Hidden;
		}
    }
}
