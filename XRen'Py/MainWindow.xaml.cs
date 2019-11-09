using System;
using System.Windows;
using System.Windows.Controls;
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

        private void Tutorial_Click(object sender, RoutedEventArgs e)
        {
            projectExpander.IsExpanded = false;
            
        }

        private void ComboBox_FontText_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            textBox.FontFamily = (comboBox_FontText.SelectedItem as ComboBoxItem).FontFamily;
            textBox.FontStretch = (comboBox_FontText.SelectedItem as ComboBoxItem).FontStretch;
            textBox.FontStyle = (comboBox_FontText.SelectedItem as ComboBoxItem).FontStyle;
        }

        private void ComboBox_FontChar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            characterLabel.FontFamily = (comboBox_FontChar.SelectedItem as ComboBoxItem).FontFamily;
            characterLabel.FontStretch = (comboBox_FontChar.SelectedItem as ComboBoxItem).FontStretch;
            characterLabel.FontStyle = (comboBox_FontChar.SelectedItem as ComboBoxItem).FontStyle;
        }

        private void FontCharacterNameSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            characterSelector.FontSize = Convert.ToDouble(fontCharacterNameSize.Text);
        }

        private void FontTextSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox.FontSize = Convert.ToDouble(fontTextSize.Text);
        }
    }
}
