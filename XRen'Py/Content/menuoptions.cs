using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace X_Ren_Py
{
    public partial class MainWindow : Window
    {   
        private XMenuOption createMenuOption(bool root)
        { XMenuOption newmenuoption = new XMenuOption() { Choice= "Menu option"};
			newmenuoption.Delete.IsEnabled = !root; 			
			newmenuoption.MenuAction.ItemsSource = menuActions;
			newmenuoption.MenuAction.SelectedIndex = 2;
			newmenuoption.ActionLabel.ItemsSource = menuLabelList;
			newmenuoption.ActionLabel.SelectedIndex = 0;
			newmenuoption.Delete.Click += deleteOption_Click;
			newmenuoption.MouseUp += Link_Click;
			return newmenuoption;
        }
		
		private void convertFrameMenu_Click(object sender, RoutedEventArgs e)
		{
			if (convertFrameMenu.Header.ToString() == framemenu)
			{
				currentFrame.isMenu = true;
				currentFrame.MenuOptions = new ObservableCollection<XMenuOption> { createMenuOption(true) };
				menuOptionsVisualList.ItemsSource = currentFrame.MenuOptions;
				convertFrameMenu.Header = menuframe;
				menuStack.Visibility = Visibility.Visible;
			}
			else
			{
				currentFrame.isMenu = false;
				currentFrame.MenuOptions = null;
				menuOptionsVisualList.ItemsSource = null;
				convertFrameMenu.Header = framemenu;
				menuStack.Visibility = Visibility.Collapsed;
			}
		}

		private void addOption_Click(object sender, RoutedEventArgs e)
		{					
			currentFrame.MenuOptions.Add(createMenuOption(false));
		}

		private void deleteOption_Click(object sender, RoutedEventArgs e)
        {
			currentFrame.MenuOptions.Remove(((sender as Button).Parent as StackPanel).Parent as XMenuOption);
		}

		private void Link_Click(object sender, MouseButtonEventArgs e)
		{
			if (!((sender as XMenuOption).MenuAction.SelectedItem == passAction))
			{
				tabControlStruct.SelectedIndex = (sender as XMenuOption).ActionLabel.SelectedIndex;
				(getSelectedList().Items[0] as XFrame).IsSelected = true;
			}
			else try
				{
					getSelectedList().SelectedIndex = getSelectedList().SelectedIndex + 1;
				}
				catch (Exception)
				{
					MessageBox.Show("No next frames!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
		}
		
	}
}
