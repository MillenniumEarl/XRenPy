using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace X_Ren_Py
{
	public class XLabel : TabItem
	{
		private TextBox _Editable = new TextBox() { MaxLength = 10, Margin = new Thickness(0), Padding = new Thickness(0), Visibility = Visibility.Collapsed };
		public Label _Visible = new Label() { Padding = new Thickness(3, 1, 3, 1), AllowDrop = true };
		private ListView _Content = new ListView() { Background = null, Margin = new Thickness(0), Padding = new Thickness(0), SelectionMode = SelectionMode.Single};
		private Button _Apply = new Button() { Padding = new Thickness(0), Width = 14, FontWeight = FontWeights.Bold, Content = "✓", Visibility = Visibility.Collapsed };
		public Button _Delete = new Button() { Padding = new Thickness(0), Width = 14, FontWeight = FontWeights.Bold, Content = "✗", };
		public ComboBoxItem comboBox = new ComboBoxItem();
		public MainWindow.XFrame MenuChoice;
		public string Text { get { return _Visible.Content.ToString(); } set { _Visible.Content = value; _Editable.Text = value; comboBox.Content = value; } }

		public XLabel()
		{
			Padding = new Thickness(2, 0, 2, 0);
			StackPanel stack = new StackPanel() { Height = 20, Margin = new Thickness(0), Orientation = Orientation.Horizontal };
			stack.Children.Add(_Delete);
			stack.Children.Add(_Apply);
			stack.Children.Add(_Visible);
			stack.Children.Add(_Editable);
			_Visible.MouseDoubleClick += label_DoubleClick;
			_Apply.Click += apply_Click;
			Header = stack;
			Content = _Content;			
			_Delete.Tag = this;
			_Visible.Tag = this;
		}

		private void apply_Click(object sender, RoutedEventArgs e)
		{
			if (_Editable.Text != "")
			{
				if (_Editable.Text != "start")
				{
					Text = _Editable.Text;
					_Visible.Visibility = Visibility.Visible;
					_Delete.Visibility = Visibility.Visible;
					_Editable.Visibility = Visibility.Collapsed;
					_Apply.Visibility = Visibility.Collapsed;
				}
				else MessageBox.Show("Error: Label can't be renamed to \"start\"", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else MessageBox.Show("Error: Label name can't be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void label_DoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (_Visible.Content.ToString() != "start")
			{
				_Editable.Visibility = Visibility.Visible;
				_Apply.Visibility = Visibility.Visible;
				_Visible.Visibility = Visibility.Collapsed;
				_Delete.Visibility = Visibility.Collapsed;
			}
			else MessageBox.Show("Error: Label \"start\" can't be renamed", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{		
		private void addLabel_Click(object sender, RoutedEventArgs e)
		{
				ListView labelbody = createLabel("newlabel");
				XFrame frame = createFrame();
				labelbody.Items.Add(frame);
				frame.IsSelected = true;
		}
		private void selectLabel_Click(object sender, RoutedEventArgs e)
		{
			if (currentFrame.Tag != tabControlStruct.SelectedItem)
			if ((tabControlStruct.SelectedContent as ListView).SelectedItem != null)
			{XFrame frame=(tabControlStruct.SelectedContent as ListView).SelectedItem as XFrame;
				frame.IsSelected = false;
				frame.IsSelected = true;
			}
			else ((tabControlStruct.SelectedContent as ListView).Items[0] as XFrame).IsSelected = true;
		}

		private void deleteLabel_Click(object sender, RoutedEventArgs e)
		{
				tabControlStruct.SelectedItem = tabControlStruct.Items[tabControlStruct.Items.IndexOf((sender as Button).Tag as XLabel) - 1];
				tabControlStruct.Items.Remove((sender as Button).Tag as XLabel);
		}

		private ListView createLabel(string name)
		{
			XLabel label = new XLabel() { Text = name};
			if (name == "start")
			{
				label._Delete.Visibility = Visibility.Collapsed;				
			}
			else label._Delete.Click += deleteLabel_Click;
			label._Visible.MouseUp += selectLabel_Click;
			tabControlStruct.Items.Insert(tabControlStruct.Items.IndexOf(addTab), label);
			(label.Content as ListView).ContextMenu = cmLabel;
			label.IsSelected = true; 
			menuLabelList.Add(label.comboBox);			
			return label.Content as ListView;
		}

		private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			var label = e.Source as XLabel;

			if (label == null)
				return;

			if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
			{
				DragDrop.DoDragDrop(label, label, DragDropEffects.All);
			}
		}

		private void TabItem_Drop(object sender, DragEventArgs e)
		{
			if (e.Source != addTab && e.Source != addTabButton)
			{
				XLabel labelTarget = (e.Source as Label).Tag as XLabel;
				XLabel labelSource = e.Data.GetData(typeof(XLabel)) as XLabel;

				if (!labelTarget.Equals(labelSource))
				{
					var tabControl = labelTarget.Parent as TabControl;
					int sourceIndex = tabControl.Items.IndexOf(labelSource);
					int targetIndex = tabControl.Items.IndexOf(labelTarget);

					tabControl.Items.Remove(labelSource);
					tabControl.Items.Insert(targetIndex, labelSource);

					tabControl.Items.Remove(labelTarget);
					tabControl.Items.Insert(sourceIndex, labelTarget);
				}
			}
		}
	}
}