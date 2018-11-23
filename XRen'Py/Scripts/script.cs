using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace X_Ren_Py
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private XFrame createFrame(bool root)
		{
			XFrame frame;
			if (!root) { frame = new XFrame() { Content = "Frame " + framecount, ContextMenu = cmFrame, Character = characterListView.Items[0] as XCharacter,AnimationInType=0, AnimationOutType = 0 }; }
			else { frame = new XFrame() { Content = "Root frame " + framecount, ContextMenu = cmRootframe, Character = characterListView.Items[0] as XCharacter, AnimationInType = 0, AnimationOutType = 0 }; }
			frame.Selected += selection_Click;
			frame.MouseDoubleClick += namechange_DoubleClick;
			framecount++;
			return frame;
		}
		private void preSaveCurrentFrame()
		{
			//перед выбором фрейма нужно сохранить содержимое нынешнего выбранного фрейма
			//может случиться, что мы начинаем выбирать другой фрейм без закрытия экспандеров с опциями и персонажем
			characterExpander.IsExpanded = false;
		}

		private void selection_Click(object sender, RoutedEventArgs e)
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
				convertButton.Content = framemenu;
				menuOptionsVisualList.ItemsSource = null;
			}
			else
			{
				menuStack.Visibility = Visibility.Visible;
				convertButton.Content = menuframe;
				menuOptionsVisualList.ItemsSource = currentFrame.MenuOptions; 				
			}

			if (currentFrame.BackgroundImg != null)
			{
				imageBackground.Source = imageShow(currentFrame.BackgroundImg.Path);
				(backImageListView.Items[backImageListView.Items.IndexOf(currentFrame.BackgroundImg)] as XImage).IsChecked = true;
			}
			else imageBackground.Source = null;

			characterLabel.Content = currentFrame.Character.Content;

			foreach (ImageProperties imageprops in ImageInFrameProps.Where(frame => frame.Frame == currentFrame))
			{
					(imageListView.Items[imageListView.Items.IndexOf(imageprops.Image)] as XImage).IsChecked = true;
			}

			foreach (AudioProperties audprops in AudioInFrameProps.Where(frame => frame.Frame == currentFrame))
			{
					if (musicListView.Items.Contains(audprops.Audio))
						(musicListView.Items[musicListView.Items.IndexOf(audprops.Audio)] as XAudio).IsChecked = true;
					else if (soundListView.Items.Contains(audprops.Audio))
						(soundListView.Items[soundListView.Items.IndexOf(audprops.Audio)] as XAudio).IsChecked = true;
					else (voiceListView.Items[voiceListView.Items.IndexOf(audprops.Audio)] as XAudio).IsChecked = true;
			}
			
			if (currentFrame.stopAudio)	stopAudio.IsChecked = true;
			else stopAudio.IsChecked = false;

		addorselect = true;
		}
		private void addInsertFrame_Click(object sender, RoutedEventArgs e)
		{
			preSaveCurrentFrame();
			XFrame frame = createFrame(false);
			ListView selectedList = getSelectedList();
			if (sender == addFrame || sender == ANFbttn) { selectedList.Items.Add(frame); }
			else if (sender == insertFrame || sender == IPFbttn) { selectedList.Items.Insert(selectedList.Items.IndexOf(selectedList.SelectedItem), frame); }
			else if (sender == addMenu) { frame.isMenu = true; frame.MenuOptions = new ObservableCollection<XMenuOption> { createMenuOption(true) }; selectedList.Items.Add(frame); }
			frame.IsSelected = true;
		}
		private void duplicateFrame_Click(object sender, RoutedEventArgs e)
		{
			preSaveCurrentFrame();

			XFrame duplicate = createFrame(false);
			duplicate.Text = currentFrame.Text;
			duplicate.isMenu = currentFrame.isMenu;
			duplicate.MenuOptions = currentFrame.MenuOptions;
			duplicate.Character = currentFrame.Character;
			duplicate.BackgroundImg = currentFrame.BackgroundImg;
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
		private void deleteFrame_Click(object sender, EventArgs e) { getSelectedList().Items.Remove(getSelectedFrame()); }
			
		private void addTab_Click(object sender, RoutedEventArgs e)
		{
			HeaderChange inputDialog = new HeaderChange();
			inputDialog.Title = "Adding tab";
			if (inputDialog.ShowDialog() == true && inputDialog.Answer != "")
			{
				string name = inputDialog.Answer;
				ListView labelbody = createLabel(name);
				labelbody.Items.Add(createFrame(true));				
			}
			else MessageBox.Show("Empty header!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void deleteTab_Click(object sender, RoutedEventArgs e)
		{
			if ((getSelectedList().Parent as TabItem).Header.ToString() != "start") tabControlStruct.Items.Remove(sender);
			else MessageBox.Show("Label START cannot be deleted!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private ListView createLabel(string name)
		{
			TabItem label = new TabItem() { Header = name };
			label.MouseDoubleClick += namechange_DoubleClick;
			tabControlStruct.Items.Add(label);
			label.IsSelected = true;
			menuLabelList.Add(new ComboBoxItem { Content = label.Header });

			ListView labelbody = new ListView() { Background = null, Margin = new Thickness(0), Padding = new Thickness(0), ContextMenu = cmLabel, SelectionMode = SelectionMode.Single };
			label.Content = labelbody;
			return labelbody;
		}

		private void PrevNext_Click(object sender, RoutedEventArgs e)
		{
			preSaveCurrentFrame();
			int index = getSelectedList().Items.IndexOf(getSelectedFrame());
			if (sender == prevFrame && index - 1 >= 0) (getSelectedList().Items[index - 1] as XFrame).IsSelected = true;
			else if (sender == nextFrame && index + 1 < getSelectedList().Items.Count) (getSelectedList().Items[index + 1] as XFrame).IsSelected = true;
		}
		private XFrame getSelectedFrame() { return getSelectedList().SelectedItem as XFrame; }
		private ListView getSelectedList() { return tabControlStruct.SelectedContent as ListView; }

		private void uncheckAll()
		{
			removeorunselect = false;
			foreach (TabItem tab in tabControlResources.Items)
			{
				foreach (XContent checkbox in (tab.Content as ListView).Items)
				{
					checkbox.IsChecked = false;
				}
			}
			removeorunselect = true;
		}
	}
}