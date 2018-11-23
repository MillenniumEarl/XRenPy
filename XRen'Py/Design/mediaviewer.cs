using System;
using System.Windows;
using System.Windows.Threading;
using AlbumArtExtraction;
using System.Windows.Media.Imaging;

namespace X_Ren_Py
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void audiomovie_Enter(object sender, RoutedEventArgs e)
        {
            if (sender.GetType() == typeof(XAudio))
            {
				XAudio audioUnderCursor = sender as XAudio;
				coverartShow(audioUnderCursor.Path.ToString());

				if (audioUnderCursor.IsChecked == false) audioPropsPanel.Visibility = Visibility.Hidden;
				else
				{
					audioPropsPanel.Visibility = Visibility.Visible;
					getAudioProperties(currentFrame, audioUnderCursor);
				}
            }
				movieplayer.Source = new Uri((sender as XContent).Path.ToString(), UriKind.Absolute);
                mediaNameLabel.Content = (sender as XContent).Content;

            DispatcherTimer disptimer = new DispatcherTimer();
            disptimer.Tick += new EventHandler(mediaCurrentTime_Tick);
            disptimer.Interval = new TimeSpan(0, 0, 1);
            disptimer.Start();

            imageViewer.Visibility = Visibility.Hidden;
            audiomovie.Visibility = Visibility.Visible;
            media.Visibility = Visibility.Visible;
            media.IsExpanded = true;
        }
        private void movieplayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (movieplayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan ts = TimeSpan.FromMilliseconds(movieplayer.NaturalDuration.TimeSpan.TotalMilliseconds);
                currentTimeSliderMedia.Maximum = ts.TotalSeconds;
            }
        }
        private void coverartShow(string audiofile)
        {
            ID3v2AlbumArtExtractor extractID3 = new ID3v2AlbumArtExtractor();
            ID3v22AlbumArtExtractor extractID3V2 = new ID3v22AlbumArtExtractor();

            BitmapImage bitmapToShow = new BitmapImage();
            bitmapToShow.BeginInit();
            if (extractID3V2.CheckType(audiofile) && extractID3V2.Extract(audiofile) != null)
                bitmapToShow.StreamSource = extractID3V2.Extract(audiofile);
            else if (extractID3.CheckType(audiofile) && extractID3.Extract(audiofile) != null)
                bitmapToShow.StreamSource = extractID3.Extract(audiofile);
            else bitmapToShow.UriSource = new Uri("Images/Music.png", UriKind.Relative);
            bitmapToShow.EndInit();
            coverart.Source = bitmapToShow;
        }

        private void mediaCurrentTime_Tick(object sender, EventArgs e)
        {
            currentTimeSliderMedia.Value = movieplayer.Position.TotalSeconds;
            if (movieplayer.Position.Seconds < 10)
                currentTimeMedia.Content = movieplayer.Position.Minutes + ":0" + movieplayer.Position.Seconds;
            else currentTimeMedia.Content = movieplayer.Position.Minutes + ":" + movieplayer.Position.Seconds;
        }
        private void mediaCurrentTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimeSpan ts = TimeSpan.FromSeconds(e.NewValue);
            movieplayer.Position = ts;
        }

        private void playMovie_Click(object sender, RoutedEventArgs e) { movieplayer.Play(); }
        private void pauseMovie_Click(object sender, RoutedEventArgs e) { movieplayer.Pause(); }
        private void stopMovie_Click(object sender, RoutedEventArgs e) { movieplayer.Stop(); }
        private void movieVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            movieplayer.Volume = volumeSliderMedia.Value / 100;
        }
		}
}
