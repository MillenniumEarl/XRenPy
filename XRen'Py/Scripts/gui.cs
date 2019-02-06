using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		private void loadGUI()
		{
			FileStream fs = new FileStream(projectFolder + "gui.rpy", FileMode.Open);
			StreamReader reader = new StreamReader(fs);

			string singleLine;
			while (!reader.EndOfStream)
			{
				singleLine = reader.ReadLine().TrimStart(' ');
				string linevalue = value(singleLine);
				if (singleLine.StartsWith("gui.init"))
				{
					projectWidth.Text = singleLine.Substring(singleLine.IndexOf('(') + 1, singleLine.IndexOf(',') - (singleLine.IndexOf('(') + 1));
					projectHeight.Text = singleLine.Substring(singleLine.IndexOf(',') + 1, singleLine.IndexOf(')') - (singleLine.IndexOf(',') + 1));
				}
				else if (singleLine.StartsWith("define gui.accent_color")) default_ColorHeaders.SelectedColor = (Color)ColorConverter.ConvertFromString(linevalue);
				else if (singleLine.StartsWith("define gui.idle_color")) default_ColorIdle.SelectedColor = (Color)ColorConverter.ConvertFromString(linevalue);
				else if (singleLine.StartsWith("define gui.idle_small_color")) default_ColorSmallIdle.SelectedColor = (Color)ColorConverter.ConvertFromString(linevalue);
				else if (singleLine.StartsWith("define gui.hover_color")) default_ColorHover.SelectedColor = (Color)ColorConverter.ConvertFromString(linevalue);
				else if (singleLine.StartsWith("define gui.selected_color")) default_ColorSelected.SelectedColor = (Color)ColorConverter.ConvertFromString(linevalue);
				else if (singleLine.StartsWith("define gui.insensitive_color")) default_ColorInsensitive.SelectedColor = (Color)ColorConverter.ConvertFromString(linevalue);
				else if (singleLine.StartsWith("define gui.muted_color")) default_ColorMuted.SelectedColor = (Color)ColorConverter.ConvertFromString(linevalue);
				else if (singleLine.StartsWith("define gui.hover_muted_color")) default_ColorMuted.SelectedColor = (Color)ColorConverter.ConvertFromString(linevalue);
				else if (singleLine.StartsWith("define gui.text_color")) default_ColorText.SelectedColor = (Color)ColorConverter.ConvertFromString(linevalue);
				else if (singleLine.StartsWith("define gui.text_font")) comboBox_FontText.SelectedItem = fonts.FirstOrDefault(font => font.Tag.ToString().Equals(linevalue));
				else if (singleLine.StartsWith("define gui.name_text_font")) comboBox_FontChar.SelectedItem = fonts.FirstOrDefault(font => font.Tag.ToString().Equals(linevalue));
				else if (singleLine.StartsWith("define gui.interface_text_font")) comboBox_FontInterface.SelectedItem = fonts.FirstOrDefault(font => font.Tag.ToString().Equals(linevalue));
				else if (singleLine.StartsWith("define gui.text_size")) fontTextSize.Text = linevalue;
				else if (singleLine.StartsWith("define gui.name_text_size")) fontCharacterNameSize.Text = linevalue;
				else if (singleLine.StartsWith("define gui.interface_text_size")) fontInterfaceSize.Text = linevalue;
				else if (singleLine.StartsWith("define gui.label_text_size")) fontLabelSize.Text = linevalue;
				else if (singleLine.StartsWith("define gui.notify_text_size")) fontNotifySize.Text = linevalue;
				else if (singleLine.StartsWith("define gui.title_text_size")) fontTitleNameSize.Text = linevalue;
			}
			fs.Close();
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
						if (singleLine.StartsWith("gui.init")) builder.Add(tab + "gui.init(" + projectWidth.Text + ',' + projectHeight.Text + ')');
						else if (singleLine.StartsWith("define gui.accent_color")) builder.Add("define gui.accent_color" + esQuote(default_ColorHeaders.SelectedColor.ToString()));
						else if (singleLine.StartsWith("define gui.idle_color")) builder.Add("define gui.idle_color" + esQuote(default_ColorIdle.SelectedColor.ToString()));
						else if (singleLine.StartsWith("define gui.idle_small_color")) builder.Add("define gui.idle_small_color" + esQuote(default_ColorSmallIdle.SelectedColor.ToString()));
						else if (singleLine.StartsWith("define gui.hover_color")) builder.Add("define gui.hover_color" + esQuote(default_ColorHover.SelectedColor.ToString()));
						else if (singleLine.StartsWith("define gui.selected_color")) builder.Add("define gui.selected_color" + esQuote(default_ColorSelected.SelectedColor.ToString()));
						else if (singleLine.StartsWith("define gui.insensitive_color")) builder.Add("define gui.insensitive_color" + esQuote(default_ColorInsensitive.SelectedColor.ToString()));
						else if (singleLine.StartsWith("define gui.muted_color")) builder.Add("define gui.muted_color" + esQuote(default_ColorMuted.SelectedColor.ToString()));
						else if (singleLine.StartsWith("define gui.hover_muted_color")) builder.Add("define gui.hover_muted_color" + esQuote(default_ColorHoverMuted.SelectedColor.ToString()));
						else if (singleLine.StartsWith("define gui.text_color")) builder.Add("define gui.text_color" + esQuote(default_ColorText.SelectedColor.ToString()));
						else if (singleLine.StartsWith("define gui.text_font")) builder.Add("define gui.text_font" + esQuote((comboBox_FontText.SelectedItem as ComboBoxItem).Tag.ToString()));
						else if (singleLine.StartsWith("define gui.name_text_font")) builder.Add("define gui.name_text_font" + esQuote((comboBox_FontChar.SelectedItem as ComboBoxItem).Tag.ToString()));
						else if (singleLine.StartsWith("define gui.interface_text_font")) builder.Add("define gui.interface_text_font" + esQuote((comboBox_FontInterface.SelectedItem as ComboBoxItem).Tag.ToString()));
						else if (singleLine.StartsWith("define gui.text_size")) builder.Add("define gui.text_size = " + fontTextSize.Text);
						else if (singleLine.StartsWith("define gui.name_text_size")) builder.Add("define gui.name_text_size = " + fontCharacterNameSize.Text);
						else if (singleLine.StartsWith("define gui.interface_text_size")) builder.Add("define gui.interface_text_size = " + fontInterfaceSize.Text);
						else if (singleLine.StartsWith("define gui.label_text_size")) builder.Add("define gui.label_text_size = " + fontLabelSize.Text);
						else if (singleLine.StartsWith("define gui.notify_text_size")) builder.Add("define gui.notify_text_size = " + fontNotifySize.Text);
						else if (singleLine.StartsWith("define gui.title_text_size")) builder.Add("define gui.title_text_size = " + fontTitleNameSize.Text);
						else builder.Add(singleLine);				
				}
				reader.Close();
				fs.Close();
			}
			else
			{
				File.Create(projectFolder + "gui.rpy").Close();
				builder.Add("init python:");
				builder.Add(tab + "gui.init(" + projectWidth.Text + ',' + projectHeight.Text + ')');
				builder.Add("define gui.accent_color" + esQuote(default_ColorHeaders.SelectedColor.ToString()));
				builder.Add("define gui.idle_color" + esQuote(default_ColorIdle.SelectedColor.ToString()));
				builder.Add("define gui.idle_small_color" + esQuote(default_ColorSmallIdle.SelectedColor.ToString()));
				builder.Add("define gui.hover_color" + esQuote(default_ColorHover.SelectedColor.ToString()));
				builder.Add("define gui.selected_color" + esQuote(default_ColorSelected.SelectedColor.ToString()));
				builder.Add("define gui.insensitive_color" + esQuote(default_ColorInsensitive.SelectedColor.ToString()));
				builder.Add("define gui.muted_color" + esQuote(default_ColorMuted.SelectedColor.ToString()));
				builder.Add("define gui.hover_muted_color" + esQuote(default_ColorHoverMuted.SelectedColor.ToString()));
				builder.Add("define gui.text_color" + esQuote(default_ColorText.SelectedColor.ToString()));
				builder.Add("define gui.text_font" + esQuote((comboBox_FontText.SelectedItem as ComboBoxItem).Tag.ToString()));
				builder.Add("define gui.name_text_font" + esQuote((comboBox_FontChar.SelectedItem as ComboBoxItem).Tag.ToString()));
				builder.Add("define gui.interface_text_font" + esQuote((comboBox_FontInterface.SelectedItem as ComboBoxItem).Tag.ToString()));
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
				GUIButton.Background = new SolidColorBrush(Color.FromArgb(102, 255, 255, 255));
				GUIGrid.Visibility = Visibility.Collapsed;
			}
			else
			{
				GUIButton.Background = Brushes.LightBlue;
				GUIGrid.Visibility = Visibility.Visible;
			}
		}
	}
}
