using System;
using System.Windows;
using System.Windows.Media;

namespace X_Ren_Py
{
    public partial class MainWindow : Window
    {
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
						character.Selected += selectionChar_Click;
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
			character.Background = new SolidColorBrush((Color)charText_colorPicker.SelectedColor);
            character.Foreground = new SolidColorBrush((Color)charName_colorPicker.SelectedColor);
        }

        private void selectionChar_Click(object sender, RoutedEventArgs e)
        {
            characterName.Text = (sender as XCharacter).Content.ToString();
            charName_colorPicker.SelectedColor = (sender as XCharacter).NameColor;
            charText_colorPicker.SelectedColor = (sender as XCharacter).TextColor;
            characterNameBold.IsChecked = (sender as XCharacter).NameIsBold;
            characterNameItalic.IsChecked = (sender as XCharacter).NameIsItalic;
            characterTextBold.IsChecked = (sender as XCharacter).TextIsBold;
            characterTextItalic.IsChecked = (sender as XCharacter).TextIsItalic;
        }
        
        private void saveCharacter()
        {
            currentFrame.Character = characterListView.SelectedItem as XCharacter;
            characterLabel.Content = currentFrame.Character.Content;
            characterLabel.Foreground = new SolidColorBrush(currentFrame.Character.NameColor);
            if (characterNameBold.IsChecked == true) characterLabel.FontWeight = FontWeights.Bold;
            else characterLabel.FontWeight = FontWeights.Normal;
            if (characterNameItalic.IsChecked == true) characterLabel.FontStyle = FontStyles.Italic;
            else characterLabel.FontStyle = FontStyles.Normal;
            textBox.Foreground = new SolidColorBrush(currentFrame.Character.TextColor);
            if (characterTextBold.IsChecked == true) textBox.FontWeight = FontWeights.Bold;
            else textBox.FontWeight = FontWeights.Normal;
            if (characterTextItalic.IsChecked == true) textBox.FontStyle = FontStyles.Italic;
            else textBox.FontStyle = FontStyles.Normal;
        }
    }
}
