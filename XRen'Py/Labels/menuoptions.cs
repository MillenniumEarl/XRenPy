using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace X_Ren_Py
{
	public class XMenuOption : ListViewItem
	{
		private Label _Href = new Label() { Foreground=Brushes.White, Width = 540, FontSize = 22, Padding = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Center, HorizontalContentAlignment = HorizontalAlignment.Center, Visibility = Visibility.Collapsed };
		private TextBox _Choice = new TextBox() { Width = 300, FontSize = 22, Text = "Menu option", Padding = new Thickness(5) };
        private Button Edit = new Button() { Background = Brushes.LightBlue, FontSize = 22, Padding = new Thickness(5), Content = "✏" };
		public Button Delete = new Button() { FontSize = 22, Padding = new Thickness(5), Content = "🗑" };
		public string Href { get { return _Href.Content.ToString(); } }
		public string Choice { get { return _Choice.Text; } set { _Choice.Text = value; _Href.Content = value; } }
        public ComboBox MenuAction { get; set; } = new ComboBox() { Width = 80, FontSize = 22, Padding = new Thickness(5) };
        public ComboBox ActionLabel { get; set; } = new ComboBox() { Width = 160, FontSize = 22, Padding = new Thickness(5) };        

        public XMenuOption()
		{
            StackPanel stack = new StackPanel();
			stack.Margin = new Thickness(5);
			stack.HorizontalAlignment = HorizontalAlignment.Stretch;
			stack.Orientation = Orientation.Horizontal;
			MenuAction.SelectionChanged += MenuAction_SelectionChanged;
			Edit.Click += Edit_Click;
			stack.Children.Add(Edit);
			stack.Children.Add(_Href);
			stack.Children.Add(_Choice);
			stack.Children.Add(MenuAction);
			stack.Children.Add(ActionLabel);
			stack.Children.Add(Delete);
			Content = stack;
			Delete.Tag = this;
            Background = new LinearGradientBrush
            {
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Color.FromArgb(0,0,0,0), 0),
                    new GradientStop(Brushes.Black.Color, 0.2),
                    new GradientStop(Brushes.Black.Color, 0.8),
                    new GradientStop(Color.FromArgb(0,0,0,0), 1)
                }
            };
            MouseEnter += XMenuOption_MouseEnter;
            MouseLeave += XMenuOption_MouseLeave;
            Margin = new Thickness(5);
        }

        private void XMenuOption_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Edit.Background != Brushes.LightBlue)
            {
                Edit.Visibility = Visibility.Hidden;
                Delete.Visibility = Visibility.Hidden;
            }
        }

        private void XMenuOption_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Edit.Background != Brushes.LightBlue)
            {
                Edit.Visibility = Visibility.Visible;
                Delete.Visibility = Visibility.Visible;
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
		{
			if (Edit.Background == Brushes.LightBlue)
			{
				Edit.Background = Brushes.WhiteSmoke;
				_Href.Visibility = Visibility.Visible;
				_Choice.Visibility = Visibility.Collapsed;
				MenuAction.Visibility = Visibility.Collapsed;
				ActionLabel.Visibility = Visibility.Collapsed;
				Choice = Choice;
			}
			else
			{
				Edit.Background = Brushes.LightBlue;
				_Href.Visibility = Visibility.Collapsed;
				_Choice.Visibility = Visibility.Visible;
				MenuAction.Visibility = Visibility.Visible;
				ActionLabel.Visibility = Visibility.Visible;
			}
		}

		private void MenuAction_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (MenuAction.SelectedIndex == 2) { ActionLabel.IsEnabled = false; }
			else ActionLabel.IsEnabled = true;
		}
	}

	public partial class MainWindow : Window
    {   
        private XMenuOption createMenuOption(bool root)
        { XMenuOption newmenuoption = new XMenuOption();
			newmenuoption.Delete.IsEnabled = !root; 			
			newmenuoption.MenuAction.ItemsSource = menuActions;
			newmenuoption.MenuAction.SelectedItem = passAction;
			newmenuoption.ActionLabel.ItemsSource = menuLabelList;
			newmenuoption.ActionLabel.SelectedIndex = 0;
			newmenuoption.Delete.Click += deleteOption_Click;
			newmenuoption.MouseUp += Link_Click;
			newmenuoption.Tag = currentFrame;
			return newmenuoption;
        }
		
		private void convertFrameMenu_Click(object sender, RoutedEventArgs e)
		{
			if (convertFrameMenu.Header.ToString() == framemenu)
			{
				currentFrame.MenuOptions = new ObservableCollection<XMenuOption> { createMenuOption(true) };
				menuOptionsVisualList.ItemsSource = currentFrame.MenuOptions;
				convertFrameMenu.Header = menuframe;
				menuStack.Visibility = Visibility.Visible;
			}
			else
			{
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
			currentFrame.MenuOptions.Remove((sender as Button).Tag as XMenuOption);
		}

		private void Link_Click(object sender, MouseButtonEventArgs e)
		{
			if (!((sender as XMenuOption).MenuAction.SelectedItem == passAction))
			{
				tabControlStruct.SelectedIndex = (sender as XMenuOption).ActionLabel.SelectedIndex;
				(tabControlStruct.SelectedItem as XLabel).MenuChoice = (sender as XMenuOption).Tag as XFrame;
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
