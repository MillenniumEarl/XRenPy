using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnidecodeSharpFork;

namespace X_Ren_Py
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		private void loadOptions()
		{
			FileStream fs = new FileStream(projectFolder + "options.rpy", FileMode.Open);
			StreamReader reader = new StreamReader(fs);

			string singleLine;
			while (!reader.EndOfStream)
			{
				singleLine = reader.ReadLine().TrimStart(' ');
				for (int compare = 0; compare < comparerOptions.Length; compare++)
				{
					if (singleLine.StartsWith(comparerOptions[compare]))
						switch (comparerOptions[compare])
						{
							case "define config.name": title.Text = singleLine.Substring(singleLine.IndexOf('"') + 1, singleLine.LastIndexOf('"') - (singleLine.IndexOf('"') + 1)); break;
							case "define gui.show_name": if (singleLine.Substring(singleLine.Length - 5).TrimStart(' ') == "True") titleVisible.IsChecked = true; else titleVisible.IsChecked = false; break;
							case "define config.version": version.Text = singleLine.Substring(singleLine.IndexOf('"') + 1, singleLine.LastIndexOf('"') - (singleLine.IndexOf('"') + 1)); break;
							case "define gui.about": about.Text = singleLine.Substring(singleLine.IndexOf("\"\"\"") + 3, singleLine.LastIndexOf("\"\"\"") - (singleLine.IndexOf("\"\"\"") + 3)); break;
							case "define build.name": buildName.Content = singleLine.Substring(singleLine.IndexOf('"') + 1, singleLine.LastIndexOf('"') - (singleLine.IndexOf('"') + 1)); break;
							case "define config.has_sound": if (singleLine.Substring(singleLine.Length - 5).TrimStart(' ') == "True") containsSound.IsChecked = true; else containsSound.IsChecked = false; break;
							case "define config.has_music": if (singleLine.Substring(singleLine.Length - 5).TrimStart(' ') == "True") containsMusic.IsChecked = true; else containsMusic.IsChecked = false; break;
							case "define config.has_voice": if (singleLine.Substring(singleLine.Length - 5).TrimStart(' ') == "True") containsVoice.IsChecked = true; else containsVoice.IsChecked = false; break;
							case "default preferences.text_cps": textShowSpeed.Text = singleLine.Substring(singleLine.IndexOf('=') + 1).TrimStart(' '); break;
							case "default preferences.afm_time": autoReaderLatency.Text = singleLine.Substring(singleLine.IndexOf('=') + 1).TrimStart(' '); break;
							default: break;
						}
				}
			}
		}

		private void saveOptions()
		{
			FileStream fs = new FileStream(projectFolder + "options.rpy", FileMode.OpenOrCreate);
			StreamReader reader = new StreamReader(fs);
			List<string> builder= new List<string> { };

			if (reader.ReadToEnd() != "")
			{
				string singleLine;
				
				while (!reader.EndOfStream)
				{
					singleLine = reader.ReadLine().TrimStart(' ');
					for (int compare = 0; compare < comparerOptions.Length; compare++)
					{
						if (singleLine.StartsWith(comparerOptions[compare]))
							switch (comparerOptions[compare])
							{
								case "define config.name": builder.Add("define config.name = _(" + quote(title.Text) + ')'); break;
								case "define gui.show_name": string show = "True"; if (titleVisible.IsChecked == false) show = "False"; builder.Add("define gui.show_name = " + show); break;
								case "define config.version": builder.Add("define config.version = " + quote(version.Text)); break;
								case "define gui.about": builder.Add("define gui.about = _p(\"\"\"" + about.Text); break;
								case "define build.name": builder.Add("define build.name = " + buildName.Content); break;
								case "define config.has_sound": string sound = "True"; if (containsSound.IsChecked == false) sound = "False"; builder.Add("define config.has_sound = " + sound); break;
								case "define config.has_music": string music = "True"; if (containsMusic.IsChecked == false) music = "False"; builder.Add("define config.has_music = " + music); break;
								case "define config.has_voice": string voice = "True"; if (containsVoice.IsChecked == false) voice = "False"; builder.Add("define config.has_voice = " + voice); break;
								case "default preferences.text_cps": builder.Add("default preferences.text_cps = " + textShowSpeed.Text); break;
								case "default preferences.afm_time": builder.Add("default preferences.afm_time" + autoReaderLatency.Text); break;
								default: builder.Add(singleLine); break;
							}
					}
				}				
			}
			else
			{
				builder.Add("define config.name = _(" + quote(title.Text) + ')');
				string show = "True"; if (titleVisible.IsChecked == false) show = "False"; builder.Add("define gui.show_name = " + show);
				builder.Add("define config.version = " + quote(version.Text));
				builder.Add("define gui.about = _p(\"\"\"" + about.Text+"\"\"\")");
				builder.Add("define build.name = " + buildName.Content);
				string sound = "True"; if (containsSound.IsChecked == false) sound = "False"; builder.Add("define config.has_sound = " + sound);
				string music = "True"; if (containsMusic.IsChecked == false) music = "False"; builder.Add("define config.has_music = " + music);
				string voice = "True"; if (containsVoice.IsChecked == false) voice = "False"; builder.Add("define config.has_voice = " + voice);
				builder.Add("default preferences.text_cps = " + textShowSpeed.Text);
				builder.Add("default preferences.afm_time" + autoReaderLatency.Text);
			}
			reader.Close();
			File.WriteAllLines(projectFolder + "options.rpy", builder);
		}
	
		private void digitTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)//перевірка на ввід цифр
		{
			if (!char.IsDigit(e.Text, e.Text.Length - 1))
				e.Handled = true;
		}
		private void floatTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			char input = e.Text.ToCharArray()[0];
			if (!char.IsDigit(input))
			{
				if (!((sender as TextBox).Text.Length == 1 && input == '.'))
					e.Handled = true;
			}
		}
		
		private void projectWidth_TextChanged(object sender, TextChangedEventArgs e)
		{
			int width;
			if (projectWidth.Text == "") width = 0;
			else
			{
				width = Convert.ToInt32(projectWidth.Text);
				movieBackground.Width = width;
				imageBackground.Width = width;
				imagegrid.Width = width;
			}
		}

		private void projectHeight_TextChanged(object sender, TextChangedEventArgs e)
		{
			int height;
			if (projectHeight.Text == "") height = 0;
			else
			{
				height = Convert.ToInt32(projectHeight.Text);
				movieBackground.Height = height;
				imageBackground.Height = height;
				imagegrid.Height = height;
			}
		}

		private void title_TextChanged(object sender, TextChangedEventArgs e)
		{
			buildName.Content = Unidecoder.Unidecode(title.Text.ToLower().Replace(' ', '-').Replace(':', '-').Replace(';', '-'));			
		}

	}
}
