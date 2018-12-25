using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace X_Ren_Py
{
    public partial class MainWindow : Window
	{
		public XCharacter loadCharacter(string singleLine)
		{
			XCharacter newCharacter = new XCharacter();
			try
			{
				int firstquote = singleLine.IndexOf('"') + 1;
				newCharacter.Content = singleLine.Substring(firstquote, singleLine.IndexOf('"', firstquote) - firstquote);
				newCharacter.Alias = singleLine.Substring(7, singleLine.IndexOf('=') - 7).TrimEnd(' ');
				string[] all = singleLine.Substring(firstquote, singleLine.LastIndexOf('"') - firstquote).Replace("\"", "").Replace(" ", "").Split(',');
				for (int prop = 0; prop < all.Length; prop++)
				{
					if (all[prop].StartsWith("image"))
					{
						int indexIn = singleLine.LastIndexOf('/') + 1;
						int length = singleLine.LastIndexOf('.') - indexIn;
						newCharacter.Icon = sideListView.Items.OfType<XImage>().First(icon => (icon.Path.Substring(indexIn, length) == all[prop].Substring(all[prop].IndexOf('"')).Replace("\"", "")));
					}
					else if (all[prop].StartsWith("color")) newCharacter.NameColor = (Color)ColorConverter.ConvertFromString(all[prop].Substring(6));
					else if (all[prop].StartsWith("who_bold") && all[prop].Contains("True")) newCharacter.NameIsBold = true;
					else if (all[prop].StartsWith("who_italic") && all[prop].Contains("True")) newCharacter.NameIsItalic = true;
					else if (all[prop].StartsWith("what_color")) newCharacter.TextColor = (Color)ColorConverter.ConvertFromString(all[prop].Substring(11));
					else if (all[prop].StartsWith("what_bold") && all[prop].Contains("True")) newCharacter.TextIsBold = true;
					else if (all[prop].StartsWith("what_italic") && all[prop].Contains("True")) newCharacter.TextIsItalic = true;
				}
			}
			catch (Exception) { MessageBox.Show("Error: Character loading", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
			return newCharacter;
		}

		private void loadText(XFrame frame, string line)
		{
			if (line != "")
			{
				if (line.IndexOf('"') == 0) frame.Character = characterListView.Items[0] as XCharacter;
				else frame.Character = characterListView.Items.OfType<XCharacter>().First(item => item.Alias == line.Substring(0, line.IndexOf(' ')));

				frame.Text = line.Substring(line.IndexOf('"')).Trim('"');
			}
		}

		private void addCharacter_Click(object sender, RoutedEventArgs e)
        {if (characterName.Text != "")
            {
                try
                {
                    bool exist = false;
					foreach (XCharacter item in characterListView.Items)
					{
						if (item.Content.ToString() == characterName.Text)
						{
							if (MessageBox.Show("Already in use! Replace the character?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
							{
								characterProperties(item);
								exist = true;
								break;
							}
                        }
                    }
					if (!exist)
					{
						XCharacter character = new XCharacter() { };
						characterProperties(character);
						character.Selected += editableChar_Selected;
						characterListView.Items.Add(character);
					}
                }
                catch (Exception) { MessageBox.Show("Some fields are empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
            else MessageBox.Show("Character name is empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        }

        private void editCharacter_Click(object sender, RoutedEventArgs e)
        {
            if (characterListView.SelectedItem != null)
            {
                XCharacter edit = characterListView.SelectedItem as XCharacter;
                characterProperties(edit);
            }
            else MessageBox.Show("No character is selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void characterProperties(XCharacter character)
        {
            character.Content = characterName.Text;
			character.ContentToAlias();
			try	{ character.NameColor = (Color)charName_colorPicker.SelectedColor; } catch (Exception) { };
			try { character.TextColor = (Color)charText_colorPicker.SelectedColor; } catch (Exception) { };
            if (characterNameBold.IsChecked == true) character.NameIsBold = true;
            else character.NameIsBold = false;
            if (characterNameItalic.IsChecked == true) character.NameIsItalic = true;
            else character.NameIsItalic = false;
            if (characterTextBold.IsChecked == true) character.TextIsBold = true;
            else character.TextIsBold = false;
            if (characterTextItalic.IsChecked == true) character.TextIsItalic = true;
            else character.TextIsItalic = false;
			if (iconCharacter.Source!=null) character.Icon = sideListView.Items.OfType<XImage>().First(sideimage => (new Uri(sideimage.Path).ToString() == iconCharacter.Source.ToString()));
			if (charText_colorPicker.SelectedColor != Color.FromArgb(0, 255, 255, 255)) character.Background = new SolidColorBrush((Color)charText_colorPicker.SelectedColor); else character.Background = null;
			if (charName_colorPicker.SelectedColor != Color.FromArgb(0, 255, 255, 255)) character.Foreground = new SolidColorBrush((Color)charName_colorPicker.SelectedColor); else character.Foreground = Brushes.Black;
		}

        private void editableChar_Selected(object sender, RoutedEventArgs e)
        {
			iconCharacter.Source = null;
			editCharacter.IsEnabled = true;
			characterName.Text = (sender as XCharacter).Content.ToString();
            charName_colorPicker.SelectedColor = (sender as XCharacter).NameColor;
            charText_colorPicker.SelectedColor = (sender as XCharacter).TextColor;
            characterNameBold.IsChecked = (sender as XCharacter).NameIsBold;
            characterNameItalic.IsChecked = (sender as XCharacter).NameIsItalic;
            characterTextBold.IsChecked = (sender as XCharacter).TextIsBold;
            characterTextItalic.IsChecked = (sender as XCharacter).TextIsItalic;
			if((sender as XCharacter).Icon != null) iconCharacter.Source = imageShow((sender as XCharacter).IconSource);
		}

		private void uneditableCharacter_Selected(object sender, RoutedEventArgs e)
		{
			iconCharacter.Source = null;
			editCharacter.IsEnabled = false;
		}

		private void saveCharacter()
        {
            currentFrame.Character = characterListView.SelectedItem as XCharacter;			
            characterLabel.Content = currentFrame.Character.Content;
			if (currentFrame.Character.NameColor != Color.FromArgb(0, 255, 255, 255)) characterLabel.Foreground = new SolidColorBrush(currentFrame.Character.NameColor); else { characterLabel.Foreground = new SolidColorBrush(default_ColorHeaders.SelectedColor.Value); }//дефолтный цвет из GUI
            if (characterNameBold.IsChecked == true) characterLabel.FontWeight = FontWeights.Bold;
            else characterLabel.FontWeight = FontWeights.Normal;
            if (characterNameItalic.IsChecked == true) characterLabel.FontStyle = FontStyles.Italic;
            else characterLabel.FontStyle = FontStyles.Normal;
			if (currentFrame.Character.TextColor != Color.FromArgb(0, 255, 255, 255)) textBox.Foreground = new SolidColorBrush(currentFrame.Character.TextColor); else { textBox.Foreground = new SolidColorBrush(default_ColorText.SelectedColor.Value); }//дефолтный цвет из GUI
			if (characterTextBold.IsChecked == true) textBox.FontWeight = FontWeights.Bold;
            else textBox.FontWeight = FontWeights.Normal;
            if (characterTextItalic.IsChecked == true) textBox.FontStyle = FontStyles.Italic;
            else textBox.FontStyle = FontStyles.Normal;
        }

		private void characterSideImageConnect_Click(object sender, RoutedEventArgs e)
		{
			if (sideListView.SelectedItem != null)
			{
				XImage image = sideListView.SelectedItem as XImage;
				(characterListView.SelectedItem as XCharacter).Icon = image;
				iconCharacter.Source = imageShow(image.Path);
			}
			else MessageBox.Show("No side image selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void charactersButton_Click(object sender, RoutedEventArgs e)
		{
			if (charactersButton.Background == Brushes.LightBlue)
			{
				charactersButton.Background = new SolidColorBrush(Color.FromArgb(102, 255, 255, 255));
				CharactersGrid.Visibility = Visibility.Collapsed;
				saveCharacter();
			}
			else
			{
				charactersButton.Background = Brushes.LightBlue;
				CharactersGrid.Visibility = Visibility.Visible;
				characterListView.SelectedItem = currentFrame.Character;
			}
		}
	}
}
