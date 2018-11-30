using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace X_Ren_Py
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		private void loadGUI()
		{
			FileStream fs = new FileStream(projectFolder + "gui.rpy", FileMode.Open);
			StreamReader reader = new StreamReader(fs);

			string singleLine;
			while (!reader.EndOfStream)
			{
				singleLine = reader.ReadLine().TrimStart(' ');
				for (int compare = 0; compare < comparerGui.Length; compare++)
				{
					if (singleLine.StartsWith(comparerGui[compare]))
					{
						switch (comparerGui[compare])
						{
							case "gui.init":
								projectWidth.Text = singleLine.Substring(singleLine.IndexOf('(') + 1, singleLine.IndexOf(',') - (singleLine.IndexOf('(') + 1));
								projectHeight.Text = singleLine.Substring(singleLine.IndexOf(',') + 1, singleLine.IndexOf(')') - (singleLine.IndexOf(',') + 1));
								break;
							case "define gui.accent_color": default_ColorHeaders.SelectedColor= (Color)ColorConverter.ConvertFromString(singleLine.Substring(24)); break;
							case "define gui.idle_color": default_ColorIdle.SelectedColor = (Color)ColorConverter.ConvertFromString(singleLine.Substring(22)); break;
							case "define gui.idle_small_color": default_ColorSmallIdle.SelectedColor = (Color)ColorConverter.ConvertFromString(singleLine.Substring(28)); break;
							case "define gui.hover_color": default_ColorHover.SelectedColor = (Color)ColorConverter.ConvertFromString(singleLine.Substring(23)); break;
							case "define gui.selected_color": default_ColorSelected.SelectedColor = (Color)ColorConverter.ConvertFromString(singleLine.Substring(26)); break;
							case "define gui.insensitive_color": default_ColorInsensitive.SelectedColor = (Color)ColorConverter.ConvertFromString(singleLine.Substring(29)); break;
							case "define gui.muted_color": default_ColorMuted.SelectedColor = (Color)ColorConverter.ConvertFromString(singleLine.Substring(23)); break;
							case "define gui.hover_muted_color": default_ColorMuted.SelectedColor = (Color)ColorConverter.ConvertFromString(singleLine.Substring(29)); break;
							case "define gui.text_color": default_ColorText.SelectedColor = (Color)ColorConverter.ConvertFromString(singleLine.Substring(25)); break;
							//case "something font": comboBox_FontText.SelectedItem = foreach (string fontFile in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Fonts))) if( Path.GetFileName(fontFile); break;
							case "define gui.text_size": fontTextSize.Text = singleLine.Substring(singleLine.IndexOf('=') + 1).TrimStart(' '); break;
							case "define gui.name_text_size": fontCharacterNameSize.Text = singleLine.Substring(singleLine.IndexOf('=') + 1).TrimStart(' '); break;
							case "define gui.interface_text_size": fontInterfaceSize.Text = singleLine.Substring(singleLine.IndexOf('=') + 1).TrimStart(' '); break;
							case "define gui.label_text_size": fontLabelSize.Text = singleLine.Substring(singleLine.IndexOf('=') + 1).TrimStart(' '); break;
							case "define gui.notify_text_size": fontNotifySize.Text = singleLine.Substring(singleLine.IndexOf('=') + 1).TrimStart(' '); break;
							case "define gui.title_text_size": fontTitleNameSize.Text = singleLine.Substring(singleLine.IndexOf('=') + 1).TrimStart(' '); break;
							default: break;
						}
						break;
					}
				}
			}
		}
	
		private void saveGUI()
		{
			List<string> builder = new List<string> { };

			if (File.Exists(projectFolder + "gui.rpy"))
			{
				FileStream fs = new FileStream(projectFolder + "gui.rpy", FileMode.Open);
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
								case "gui.init": builder.Add(tab+ "gui.init(" + projectWidth.Text + ','+ projectHeight.Text+')'); break;
								case "define gui.accent_color":	builder.Add("define gui.accent_color = " + quote(default_ColorHeaders.SelectedColor.ToString())); break;
								case "define gui.idle_color": builder.Add("define gui.idle_color = " + quote(default_ColorIdle.SelectedColor.ToString())); break;
								case "define gui.idle_small_color": builder.Add("define gui.idle_small_color = " + quote(default_ColorSmallIdle.SelectedColor.ToString())); break;
								case "define gui.hover_color": builder.Add("define gui.hover_color = " + quote(default_ColorHover.SelectedColor.ToString())); break;
								case "define gui.selected_color": builder.Add("define gui.selected_color = " + quote(default_ColorHover.SelectedColor.ToString())); break;								
								case "define gui.insensitive_color": builder.Add("define gui.insensitive_color = " + quote(default_ColorHover.SelectedColor.ToString())); break;
								case "define gui.muted_color": builder.Add("define gui.muted_color = " + quote(default_ColorHover.SelectedColor.ToString())); break;
								case "define gui.hover_muted_color": builder.Add("define gui.hover_muted_color = " + quote(default_ColorHover.SelectedColor.ToString())); break;
								case "define gui.text_color": builder.Add("define gui.text_color = " + quote(default_ColorHover.SelectedColor.ToString())); break;
								//case "something font": comboBox_Fontransition": builder.Add("define config.exit_transition = " + gameExitTransition.SelectedItem); break;
								case "define gui.text_size": builder.Add("define gui.text_size = " + fontTextSize.Text); break;
								case "define gui.name_text_size": builder.Add("define gui.name_text_size = " + fontCharacterNameSize.Text); break;
								case "define gui.interface_text_size": builder.Add("define gui.interface_text_size = " + fontInterfaceSize.Text); break;
								case "define gui.label_text_size": builder.Add("define gui.label_text_size = " + fontLabelSize.Text); break;
								case "define gui.notify_text_size": builder.Add("define gui.notify_text_size = " + fontNotifySize.Text); break;
								case "define gui.title_text_size": builder.Add("define gui.title_text_size = " + fontTitleNameSize.Text); break;
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
				File.Create(projectFolder + "gui.rpy");
				builder.Add(tab + "gui.init(" + projectWidth.Text + ',' + projectHeight.Text + ')');
				builder.Add("define gui.accent_color = " + quote(default_ColorHeaders.SelectedColor.ToString()));
				builder.Add("define gui.idle_color = " + quote(default_ColorIdle.SelectedColor.ToString()));
				builder.Add("define gui.idle_small_color = " + quote(default_ColorSmallIdle.SelectedColor.ToString()));
				builder.Add("define gui.hover_color = " + quote(default_ColorHover.SelectedColor.ToString()));
				builder.Add("define gui.selected_color = " + quote(default_ColorHover.SelectedColor.ToString()));	
				builder.Add("define gui.insensitive_color = " + quote(default_ColorHover.SelectedColor.ToString()));
				builder.Add("define gui.muted_color = " + quote(default_ColorHover.SelectedColor.ToString()));
				builder.Add("define gui.hover_muted_color = " + quote(default_ColorHover.SelectedColor.ToString())); 
				builder.Add("define gui.text_color = " + quote(default_ColorHover.SelectedColor.ToString()));
				builder.Add("define gui.text_size = " + fontTextSize.Text);
				builder.Add("define gui.name_text_size = " + fontCharacterNameSize.Text);
				builder.Add("define gui.interface_text_size = " + fontInterfaceSize.Text);
				builder.Add("define gui.label_text_size = " + fontLabelSize.Text);
				builder.Add("define gui.notify_text_size = " + fontNotifySize.Text);
				builder.Add("define gui.title_text_size = " + fontTitleNameSize.Text);
			}
			File.WriteAllLines(projectFolder + "gui.rpy", builder);
		}

		private void GUIButton_Click(object sender, RoutedEventArgs e)
		{
			if (GUIButton.Background == Brushes.LightBlue)
			{
				GUIButton.Background = new SolidColorBrush(Color.FromArgb(102,255,255,255));
				GUIGrid.Visibility = Visibility.Collapsed;
			}
			else
			{
				GUIButton.Background = Brushes.LightBlue;
				GUIGrid.Visibility = Visibility.Visible;
			}
		}
		private void comboBox_Initialized(object sender, EventArgs e)
		{
			foreach (FontFamily font in Fonts.SystemFontFamilies) (sender as ComboBox).Items.Add(new ComboBoxItem() { Content = font.Source, FontFamily = font });
			(sender as ComboBox).SelectedIndex = 0;
		}

	}
}
