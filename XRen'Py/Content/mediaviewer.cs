using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace X_Ren_Py
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private void audiomovie_Enter(object sender, RoutedEventArgs e)
		{
			if (File.Exists((sender as XContent).Path))
			{
				(sender as XContent).TextColor = Brushes.Black;
				(sender as XContent).Checkbox.IsEnabled = true;

				if (sender.GetType() == typeof(XAudio))
				{
					XAudio audioUnderCursor = sender as XAudio;
					coverartShow(audioUnderCursor.Path);

					if (audioUnderCursor.IsChecked != true) audioPropsPanel.Visibility = Visibility.Hidden;
					else
					{
						audioPropsPanel.Visibility = Visibility.Visible;
						getAudioProperties(audioUnderCursor);
					}
				}

				movieplayer.Source = new Uri((sender as XContent).Path, UriKind.Absolute);
				mediaNameLabel.Content = (sender as XContent).Header;

				imageViewer.Visibility = Visibility.Hidden;
				audiomovie.Visibility = Visibility.Visible;
				media.Visibility = Visibility.Visible;
				media.IsExpanded = true;
			}
			else { (sender as XContent).TextColor = Brushes.Red;
				(sender as XContent).Checkbox.IsEnabled = false; }
		}
		private void player_MediaOpened(object sender, RoutedEventArgs e)
		{
			if ((sender as MediaElement).NaturalDuration.HasTimeSpan)
			{
				if(sender==movieplayer) currentTimeSliderMedia.Maximum = movieplayer.NaturalDuration.TimeSpan.TotalSeconds;
				else if (sender== music) currentTimeSliderMusic.Maximum = music.NaturalDuration.TimeSpan.TotalSeconds;
				else if (sender==sound) currentTimeSliderSound.Maximum = sound.NaturalDuration.TimeSpan.TotalSeconds;
				else currentTimeSliderVoice.Maximum = voice.NaturalDuration.TimeSpan.TotalSeconds;
			}
		}

		private void mediaCurrentTime_Tick(object sender, EventArgs e)
		{
			if (movieplayer.Source != null)
			{
				currentTimeSliderMedia.Value = movieplayer.Position.TotalSeconds;
				currentTimeMedia.Content = movieplayer.Position.ToString().Substring(3, 5);
			}
			if (music.Source != null)
			{
				currentTimeSliderMusic.Value = music.Position.TotalSeconds;
				currentTimeMusic.Content = music.Position.ToString().Substring(3, 5);
			}
			if (sound.Source != null)
			{
				currentTimeSliderSound.Value = sound.Position.TotalSeconds;
				currentTimeSound.Content = sound.Position.ToString().Substring(3, 5);
			}
			if (music.Source != null)
			{
				currentTimeSliderVoice.Value = voice.Position.TotalSeconds;
				currentTimeVoice.Content = voice.Position.ToString().Substring(3, 5);
			}
		}

		private void mediaCurrentTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			TimeSpan ts = TimeSpan.FromSeconds(e.NewValue);
			if (sender==currentTimeSliderMedia)	movieplayer.Position = ts;
			else if (sender == currentTimeSliderMusic) music.Position = ts;
			else if (sender == currentTimeSliderSound) sound.Position = ts;
			else voice.Position = ts;
		}

		private void play_Click(object sender, RoutedEventArgs e)
		{
			if (sender == playMovie) movieplayer.Play();
			else if (sender == playMusic) music.Play();
			else if (sender == playSound) sound.Play();
			else voice.Play();
		}
		private void pause_Click(object sender, RoutedEventArgs e)
		{
			if (sender == pauseMovie) movieplayer.Pause();
			else if (sender == pauseMusic) music.Pause();
			else if (sender == pauseSound) sound.Pause();
			else voice.Pause();
		}
		private void stop_Click(object sender, RoutedEventArgs e)
		{
			if (sender == stopMovie) movieplayer.Stop();
			else if (sender == stopMusic) music.Stop();
			else if (sender == stopSound) sound.Stop();
			else voice.Stop();
		}

		private void movieVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			movieplayer.Volume = volumeSliderMedia.Value / 100;
		}

		private void media_Unloaded(object sender, RoutedEventArgs e)
		{
			if (movieplayer.Source == null && music.Source == null && sound.Source == null && voice.Source == null) disptimer.Stop();
			if (sender!=movieplayer) ((sender as MediaElement).Parent as StackPanel).Visibility = Visibility.Collapsed;
		}

		private void media_Loaded(object sender, RoutedEventArgs e)
		{
			disptimer.Start();			
		}
	}
}
