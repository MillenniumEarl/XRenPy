using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace X_Ren_Py
{
	public class XCharacter : ListViewItem
	{
        public string Alias { get; set; }
        public void ContentToAlias()
        {
            Alias = Content.ToString().ToLower().Replace(" ", "").Replace("-", "").Replace("\'", "");
            CharView.Content = Content;
        }
        public Color NameColor { get; set; } = Brushes.Transparent.Color;
        public Color TextColor { get; set; } = Brushes.Transparent.Color;
        public bool NameIsBold { get; set; } = false;
        public bool NameIsItalic { get; set; } = false;
        public bool TextIsBold { get; set; } = false;
        public bool TextIsItalic { get; set; } = false;
        public bool IsNvl { get; set; } = false;
        public MainWindow.XImage Icon { get; set; }
        public ComboBoxItem CharView { get; set; } = new ComboBoxItem { };
        public string IconSource { get { return Icon.Path; } }

        public XCharacter()
        {
            CharView.Tag = this;
        }
        
    }    

	public partial class MainWindow : Window
	{
        public XCharacter loadCharacter(string singleLine)
		{
			XCharacter newCharacter = new XCharacter();
			newCharacter.Selected += editableChar_Selected;            
			try
			{
				int firstquote = singleLine.IndexOf('"') + 1;
				newCharacter.Content = singleLine.Substring(firstquote, singleLine.IndexOf('"', firstquote) - firstquote);
				newCharacter.Alias = singleLine.Substring(7, singleLine.IndexOf('=') - 7).TrimEnd(' ');
                string[] all = singleLine.Substring(firstquote).Replace("\"", "").Replace(" ", "").Split(',');
				for (int prop = 1; prop < all.Length; prop++)
				{
					if (all[prop].StartsWith("image"))
					{
						int indexIn = singleLine.LastIndexOf('/') + 1;
						int length = singleLine.LastIndexOf('.') - indexIn;
						newCharacter.Icon = sideListView.Items.OfType<XImage>().First(icon => (icon.Path.Substring(indexIn, length) == all[prop].Substring(all[prop].IndexOf('"')).Replace("\"", "")));
					}
                    else if (all[prop].StartsWith("kind") && all[prop].Contains("nvl")) newCharacter.IsNvl = true;
                    else if (all[prop].StartsWith("color")) newCharacter.NameColor = (Color)ColorConverter.ConvertFromString(all[prop].Substring(6).TrimEnd(')'));
					else if (all[prop].StartsWith("who_bold") && all[prop].Contains("True")) newCharacter.NameIsBold = true;
					else if (all[prop].StartsWith("who_italic") && all[prop].Contains("True")) newCharacter.NameIsItalic = true;
					else if (all[prop].StartsWith("what_color")) newCharacter.TextColor = (Color)ColorConverter.ConvertFromString(all[prop].Substring(11).TrimEnd(')'));
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
                if (line.IndexOf('"') == 0) frame.Character = charNone;
				else {frame.Character = characterList.First(item => item.Alias == line.Substring(0, line.IndexOf(' '))); }

				frame.Text = line.Substring(line.IndexOf('"')).Trim('"');
				frame.Content = "[" + frame.Text + ']';
			}
		}

		private void addCharacter_Click(object sender, RoutedEventArgs e)
        {if (characterName.Text != "")
            {
                try
                {
                    bool exist = false;
					foreach (XCharacter item in characterList)
					{
						if (item.Content.ToString() == characterName.Text)
						{
							if (MessageBox.Show("Already in use! Replace the character?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
							{
								characterSetProperties(item);
								exist = true;
								break;
							}
                        }
                    }
					if (!exist)
					{
						XCharacter character = new XCharacter() { };
						characterSetProperties(character);
						character.Selected += editableChar_Selected;
						characterList.Add(character);
                        characterSelector.Items.Add(character.CharView);
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
                characterSetProperties(edit);
                if ((characterSelector.SelectedItem as ComboBoxItem).Tag == edit) characterLabel.Content = edit.Content;
            }
            else MessageBox.Show("No character is selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void characterSetProperties(XCharacter character)
        {
            character.Content = characterName.Text;
			character.ContentToAlias();            
            if (characterNvl.IsChecked == true) character.IsNvl = true;
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
			if (charText_colorPicker.SelectedColor != Brushes.Transparent.Color) character.Background = new SolidColorBrush((Color)charText_colorPicker.SelectedColor); else character.Background = null;
			if (charName_colorPicker.SelectedColor != Brushes.Transparent.Color) character.Foreground = new SolidColorBrush((Color)charName_colorPicker.SelectedColor); else character.Foreground = Brushes.Black;
        }

        private void editableChar_Selected(object sender, RoutedEventArgs e)
        {
            XCharacter character = sender as XCharacter;
			iconCharacter.Source = null;
			editCharacter.IsEnabled = true;
			characterName.Text = character.Content.ToString();
            charName_colorPicker.SelectedColor = character.NameColor;
            charText_colorPicker.SelectedColor = character.TextColor;
            characterNvl.IsChecked=character.IsNvl;
            characterNameBold.IsChecked = character.NameIsBold;
            characterNameItalic.IsChecked = character.NameIsItalic;
            characterTextBold.IsChecked = character.TextIsBold;
            characterTextItalic.IsChecked = character.TextIsItalic;
			if(character.Icon != null) iconCharacter.Source = imageShow(character.IconSource);
		}

		private void uneditableCharacter_Selected(object sender, RoutedEventArgs e)
		{
			iconCharacter.Source = null;
			editCharacter.IsEnabled = false;
		}

		//private void selectCharacter_Click(object sender, RoutedEventArgs e)//вот тут
		//{
  //          currentCharacter = characterListView.SelectedItem as XCharacter;
  //          currentFrame.Character = currentCharacter;
  //          showCharacter();
		//}

		private void showCharacter()
		{
			characterLabel.Content = currentFrame.Character.Content;
			if (currentFrame.Character.NameColor != Brushes.Transparent.Color) characterLabel.Foreground = new SolidColorBrush(currentFrame.Character.NameColor); else { characterLabel.Foreground = new SolidColorBrush(default_ColorHeaders.SelectedColor.Value); }//дефолтный цвет из GUI
			if (currentFrame.Character.NameIsBold) characterLabel.FontWeight = FontWeights.Bold;
			else characterLabel.FontWeight = FontWeights.Normal;
			if (currentFrame.Character.NameIsItalic) characterLabel.FontStyle = FontStyles.Italic;
			else characterLabel.FontStyle = FontStyles.Normal;
			if (currentFrame.Character.TextColor != Brushes.Transparent.Color) textBox.Foreground = new SolidColorBrush(currentFrame.Character.TextColor); else { textBox.Foreground = new SolidColorBrush(default_ColorText.SelectedColor.Value); }//дефолтный цвет из GUI
			if (currentFrame.Character.TextIsBold) textBox.FontWeight = FontWeights.Bold;
			else textBox.FontWeight = FontWeights.Normal;
			if (currentFrame.Character.TextIsItalic) textBox.FontStyle = FontStyles.Italic;
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
			else {
				if (tabControlResources.SelectedIndex != 6) tabControlResources.SelectedIndex = 6;
				else MessageBox.Show("No side image selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
		}

		private void charactersButton_Click(object sender, RoutedEventArgs e)
		{
			if (charactersButton.Background == Brushes.LightBlue)
			{
				charactersButton.Background = new SolidColorBrush(Color.FromArgb(102, 255, 255, 255));
				CharactersGrid.Visibility = Visibility.Collapsed;				
			}
			else
			{
				charactersButton.Background = Brushes.LightBlue;
				CharactersGrid.Visibility = Visibility.Visible;			
			}
		}

        private void CharacterSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem character = (sender as ComboBox).SelectedItem as ComboBoxItem;
            characterLabel.Content = character.Content.ToString();
            currentFrame.Character = character.Tag as XCharacter;
            currentCharacter = currentFrame.Character;
            showCharacter();
        }
    }
}
