﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
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
					{
						switch (comparerOptions[compare])
						{
							case "define config.name": title.Text = singleLine.Substring(singleLine.IndexOf('"') + 1, singleLine.LastIndexOf('"') - (singleLine.IndexOf('"') + 1)); break;
							case "define gui.show_name": if (singleLine.Substring(singleLine.Length - 5).TrimStart(' ') == "True") titleVisible.IsChecked = true; else titleVisible.IsChecked = false; break;
							case "define config.version": version.Text = singleLine.Substring(singleLine.IndexOf('"') + 1, singleLine.LastIndexOf('"') - (singleLine.IndexOf('"') + 1)); break;
							case "define gui.about": about.Text = singleLine.Substring(singleLine.IndexOf("\"\"\"") + 3); break;
							case "define build.name": buildName.Content = singleLine.Substring(singleLine.IndexOf('"')).Replace("\"",""); break;
							case "define config.has_sound": if (singleLine.Substring(singleLine.Length - 5).TrimStart(' ') == "True") containsSound.IsChecked = true; else containsSound.IsChecked = false; break;
							case "define config.has_music": if (singleLine.Substring(singleLine.Length - 5).TrimStart(' ') == "True") containsMusic.IsChecked = true; else containsMusic.IsChecked = false; break;
							case "define config.has_voice": if (singleLine.Substring(singleLine.Length - 5).TrimStart(' ') == "True") containsVoice.IsChecked = true; else containsVoice.IsChecked = false; break;
							case "default preferences.text_cps": textShowSpeed.Text = singleLine.Substring(singleLine.IndexOf('=') + 1).TrimStart(' '); break;
							case "default preferences.afm_time": autoReaderLatency.Text = singleLine.Substring(singleLine.IndexOf('=') + 1).TrimStart(' '); break;
							case "define config.enter_transition": gameOpenTransition.SelectedItem = singleLine.Substring(singleLine.IndexOf('=') + 1).TrimStart(' '); break;
							case "define config.intra_transition": gameIntraTransition.SelectedItem = singleLine.Substring(singleLine.IndexOf('=') + 1).TrimStart(' '); break;
							case "define config.exit_transition": gameExitTransition.SelectedItem = singleLine.Substring(singleLine.IndexOf('=') + 1).TrimStart(' '); break;
							case "define config.after_load_transition": gameStartTransition.SelectedItem = singleLine.Substring(singleLine.IndexOf('=') + 1).TrimStart(' '); break;
							case "define config.end_game_transition": gameEndTransition.SelectedItem = singleLine.Substring(singleLine.IndexOf('=') + 1).TrimStart(' '); break;
							case "define config.window_show_transition": dialogShowTransition.SelectedItem = singleLine.Substring(singleLine.IndexOf('=') + 1).TrimStart(' '); break;
							case "define config.window_hide_transition": dialogHideTransition.SelectedItem = singleLine.Substring(singleLine.IndexOf('=') + 1).TrimStart(' '); break;
							case "define config.window_icon": icon.Icon = new Image { Source = imageShow(projectFolder + singleLine.Substring(singleLine.IndexOf('"')).Replace("\"", "")) }; break;
							default: break;
						}
						break;
					}
				}
			}
		}

		private void saveOptions()
		{			
			List<string> builder= new List<string> { };

			if (File.Exists(projectFolder + "options.rpy"))
			{
				FileStream fs = new FileStream(projectFolder + "options.rpy", FileMode.Open);
				StreamReader reader = new StreamReader(fs);
				string singleLine;

				while (!reader.EndOfStream)
				{
					singleLine = reader.ReadLine();
					bool found = false;
					for (int compare = 0; compare < comparerOptions.Length; compare++)
					{
						if (singleLine.StartsWith(comparerOptions[compare]))
						{
							found = true;
							switch (comparerOptions[compare])
							{
								case "define config.name":builder.Add("define config.name = _(" + quote(title.Text) + ')');	break;
								case "define gui.show_name":
										string show = "True";
										if (titleVisible.IsChecked == false) show = "False";
										builder.Add("define gui.show_name = " + show);	break;									
								case "define config.version":builder.Add("define config.version = " + quote(version.Text));break;									
								case "define gui.about":builder.Add("define gui.about = _p(\"\"\"" + about.Text);break;							
								case "define build.name":builder.Add("define build.name = " + quote(buildName.Content.ToString()));break;
								case "define config.has_sound":
										string sound = "True";
										if (containsSound.IsChecked == false) sound = "False";
										builder.Add("define config.has_sound = " + sound); break;
								case "define config.has_music":
										string music = "True";
										if (containsMusic.IsChecked == false) music = "False";
										builder.Add("define config.has_music = " + music); break;
								case "define config.has_voice":
										string voice = "True";
										if (containsVoice.IsChecked == false) voice = "False";
										builder.Add("define config.has_voice = " + voice); break;
								case "default preferences.text_cps": builder.Add("default preferences.text_cps = " + textShowSpeed.Text); break;
								case "default preferences.afm_time":builder.Add("default preferences.afm_time = " + autoReaderLatency.Text);break;
								case "define config.enter_transition": builder.Add("define config.enter_transition = " + gameOpenTransition.SelectedItem); break;
								case "define config.intra_transition": builder.Add("define config.intra_transition = "+gameIntraTransition.SelectedItem); break;
								case "define config.exit_transition": builder.Add("define config.exit_transition = " + gameExitTransition.SelectedItem); break;
								case "define config.after_load_transition": builder.Add("define config.after_load_transition = " + gameStartTransition.SelectedItem); break;
								case "define config.end_game_transition": builder.Add("define config.end_game_transition = " + gameEndTransition.SelectedItem); break;
								case "define config.window_show_transition": builder.Add("define config.window_show_transition = "+dialogShowTransition.SelectedItem); break;
								case "define config.window_hide_transition": builder.Add("define config.window_hide_transition = "+dialogHideTransition.SelectedItem); break;
								case "define config.window_icon": try{string iconPath = icon.Icon.Source.ToString();builder.Add("define config.window_icon = " + quote(iconPath.Substring(iconPath.IndexOf(game) + 6)));}
										catch (Exception) { builder.Add(singleLine); };	break;
								default: break;
							}
						}

					}
					if (!found) builder.Add(singleLine);
				}
				reader.Close();
			}
			else
			{
				File.Create(projectFolder + "options.rpy");
				builder.Add("define config.name = _(" + quote(title.Text) + ')');
				string show = "True"; if (titleVisible.IsChecked == false) show = "False"; builder.Add("define gui.show_name = " + show);
				builder.Add("define config.version = " + quote(version.Text));
				builder.Add("define gui.about = _p(\"\"\"" + about.Text);
				builder.Add("\"\"\")");
				builder.Add("define build.name = " + quote(buildName.Content.ToString()));
				string sound = "True"; if (containsSound.IsChecked == false) sound = "False"; builder.Add("define config.has_sound = " + sound);
				string music = "True"; if (containsMusic.IsChecked == false) music = "False"; builder.Add("define config.has_music = " + music);
				string voice = "True"; if (containsVoice.IsChecked == false) voice = "False"; builder.Add("define config.has_voice = " + voice);
				builder.Add("default preferences.text_cps = " + textShowSpeed.Text);
				builder.Add("default preferences.afm_time = " + autoReaderLatency.Text);
				builder.Add("define config.enter_transition = " + gameOpenTransition.SelectedItem);
				builder.Add("define config.intra_transition = " + gameIntraTransition.SelectedItem);
				builder.Add("define config.exit_transition = " + gameExitTransition.SelectedItem);
				builder.Add("define config.after_load_transition = " + gameStartTransition.SelectedItem);
				builder.Add("define config.end_game_transition = " + gameEndTransition.SelectedItem);
				builder.Add("define config.window_show_transition = " + dialogShowTransition.SelectedItem);
				builder.Add("define config.window_hide_transition = " + dialogHideTransition.SelectedItem);
				try
				{ string iconPath = icon.Icon.Source.ToString();
				builder.Add("define config.window_icon = " + quote(iconPath.Substring(iconPath.IndexOf(game) + 6)));
				}
				catch (Exception) { };
			}			
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

		private void icon_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog imageDialog = new OpenFileDialog() { Filter = "Icon files (*.ico,*.png)|*.ico;*.png" };

			if (imageDialog.ShowDialog() == true)
			{
				try
				{
					contentCollector(imageDialog.FileName, projectFolder+guiFolder + imageDialog.SafeFileName);
					icon.Icon = new Image { Source = imageShow(projectFolder + guiFolder + imageDialog.SafeFileName) };
				}
				catch (Exception) { MessageBox.Show("Please choose the image!", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
			}
		}
	}
}
