using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace X_Ren_Py
{
	public class XFrame : ListViewItem
	{
		//string _Content;
		private string _Text = "";
		private bool _isMenu = false;
		private bool _stopAudio = false;
		private ObservableCollection<XMenuOption> _MenuOptions;
		private XCharacter _Character;
		private ImageProperties _BackgroundImageProps = new ImageProperties();
		private XMovie _Movie;

		public string Text { get { return _Text; } set { _Text = value; } }
		public bool isMenu { get { return _isMenu; } set { _isMenu = value; } }
		public bool stopAudio { get { return _stopAudio; } set { _stopAudio = value; } }
		public ObservableCollection<XMenuOption> MenuOptions { get { return _MenuOptions; } set { _MenuOptions = value; } }
		public XCharacter Character { get { return _Character; } set { _Character = value; } }

		public ImageProperties BackgroundImageProps { get { return _BackgroundImageProps; } set { _BackgroundImageProps = value; } }
		public XImage BackgroundImage { get { return _BackgroundImageProps.Image; } set { _BackgroundImageProps.Image = value; } }
		public byte AnimationInType { get { return _BackgroundImageProps.AnimationInType; } set { _BackgroundImageProps.AnimationInType = value; } }
		public byte AnimationOutType { get { return _BackgroundImageProps.AnimationOutType; } set { _BackgroundImageProps.AnimationOutType = value; } }
		public XMovie Movie { get { return _Movie; } set { _Movie = value; } }

		public XFrame()
		{
			Content = "Frame []";
			AnimationInType = 0;
			AnimationOutType = 0;
		}
	}

	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private XFrame createFrame()
		{
			XFrame frame = new XFrame() { Character = characterListView.Items[0] as XCharacter, ContextMenu = cmFrame};
			frame.Selected += selectFrame_Click;
			frame.MouseUp += selectFrame_Click;
			return frame;
		}
		private void preSaveCurrentFrame()
		{
			//перед выбором фрейма нужно сохранить содержимое нынешнего выбранного фрейма
			currentFrame.Content = currentFrame.Content.ToString().Substring(0, currentFrame.Content.ToString().IndexOf('[')) + '[' + textBox.Text + ']';
		}

		private void selectFrame_Click(object sender, RoutedEventArgs e)
		{
			preSaveCurrentFrame();
			uncheckAll();
			addorselect = false;
			currentFrame = sender as XFrame;
			//после выбора фрейма нужно показать его содержимое. оно должно храниться в объекте этого фрейма с привязкой ко всем остальным объектам
			textBox.Text = currentFrame.Text;

			if (!currentFrame.isMenu)
			{
				menuStack.Visibility = Visibility.Hidden;
				convertFrameMenu.Header = framemenu;
				menuOptionsVisualList.ItemsSource = null;
			}
			else
			{
				menuStack.Visibility = Visibility.Visible;
				convertFrameMenu.Header = menuframe;
				menuOptionsVisualList.ItemsSource = currentFrame.MenuOptions;
			}

			if (currentFrame.BackgroundImage != null)
			{
				imageBackground.Source = imageShow(currentFrame.BackgroundImage.Path);
				(backImageListView.Items[backImageListView.Items.IndexOf(currentFrame.BackgroundImage)] as XImage).IsChecked = true;
			}
			else imageBackground.Source = null;

			characterLabel.Content = currentFrame.Character.Content;

			foreach (ImageProperties imageprops in ImageInFrameProps.Where(frame => frame.Frame == currentFrame))
			{
				imageprops.Image.IsChecked = true;
				imageprops.Image.Background = currentFrameResourceColor;
			}
			foreach (AudioProperties audprops in AudioInFrameProps.Where(frame => frame.Frame == currentFrame))
			{
				audprops.Audio.IsChecked = true;
				audprops.Audio.Background = currentFrameResourceColor;
			}

			if (currentFrame.stopAudio) stopAudio.IsChecked = true;
			else stopAudio.IsChecked = false;

			addorselect = true;
		}
		private void addNextFrame_Click(object sender, RoutedEventArgs e)
		{
			preSaveCurrentFrame();
			XFrame frame = createFrame();
			ListView selectedList = getSelectedList();

			if (sender == addMenu)
			{
				frame.isMenu = true;
				frame.MenuOptions = new ObservableCollection<XMenuOption> { createMenuOption(true) };
			}

			selectedList.Items.Insert(selectedList.Items.IndexOf(selectedList.SelectedItem) + 1, frame);
			frame.IsSelected = true;
		}
		private void duplicateFrame_Click(object sender, RoutedEventArgs e)
		{
			preSaveCurrentFrame();

			XFrame duplicate = createFrame();
			duplicate.Text = currentFrame.Text;
			duplicate.isMenu = currentFrame.isMenu;
			duplicate.MenuOptions = currentFrame.MenuOptions;
			duplicate.Character = currentFrame.Character;
			duplicate.BackgroundImage = currentFrame.BackgroundImage;
			duplicate.Movie = currentFrame.Movie;

			List<ImageProperties> newimageprops = new List<ImageProperties>();
			//существующую коллекцию нельзя менять во время прохождения по ней, а объединить коллекции проще пока не удалось. Потому придется использовать два перечислителя
			foreach (ImageProperties i in ImageInFrameProps)
			{
				if (i.Frame == currentFrame)
				{
					ImageProperties newprop = new ImageProperties() { Frame = duplicate, Image = i.Image };
					newimageprops.Add(newprop);
				}
			}
			foreach (ImageProperties i in newimageprops) { ImageInFrameProps.Add(i); }

			List<AudioProperties> newaudioprops = new List<AudioProperties>();
			//существующую коллекцию нельзя менять во время прохождения по ней, а объединить коллекции проще пока не удалось. Потому придется использовать два перечислителя
			foreach (AudioProperties i in AudioInFrameProps)
			{
				if (i.Frame == currentFrame)
				{
					AudioProperties newprop = new AudioProperties() { Frame = duplicate, Audio = i.Audio };
					newaudioprops.Add(newprop);
				}
			}
			foreach (AudioProperties i in newaudioprops) { AudioInFrameProps.Add(i); }

			getSelectedList().Items.Insert(getSelectedList().Items.IndexOf(currentFrame) + 1, duplicate);
			duplicate.IsSelected = true;
		}
		private void deleteFrame_Click(object sender, EventArgs e) { if(getSelectedList().Items.Count>1) getSelectedList().Items.Remove(getSelectedFrame()); else MessageBox.Show("Error: Label must contain at least one frame", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
		private void PrevNext_Click(object sender, RoutedEventArgs e)
		{
			preSaveCurrentFrame();
			int index = getSelectedList().Items.IndexOf(getSelectedFrame());
			if (sender == prevFrame && index - 1 >= 0) (getSelectedList().Items[index - 1] as XFrame).IsSelected = true;
			else if (sender == nextFrame && index + 1 < getSelectedList().Items.Count) (getSelectedList().Items[index + 1] as XFrame).IsSelected = true;
		}
		private void textBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			currentFrame.Text = textBox.Text;
		}
		private XFrame getSelectedFrame() { return getSelectedList().SelectedItem as XFrame; }
		private ListView getSelectedList() { return tabControlStruct.SelectedContent as ListView; }

	}
}