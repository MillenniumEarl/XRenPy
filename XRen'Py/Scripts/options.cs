using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace X_Ren_Py
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)//перевірка на ввід цифр
		{
			if (!char.IsDigit(e.Text, e.Text.Length - 1))
				e.Handled = true;
		}

		private void projectWidth_TextChanged(object sender, TextChangedEventArgs e)
		{
			int width;
			if (projectWidth.Text == "") width = 0;
			width = Convert.ToInt32(projectWidth.Text);
			movieBackground.Width = width;
			imageBackground.Width = width;
			imagegrid.Width = width;
		}

		private void projectHeight_TextChanged(object sender, TextChangedEventArgs e)
		{
			int height;
			if (projectHeight.Text == "") height = 0;
			height = Convert.ToInt32(projectHeight.Text);
			movieBackground.Height = height;
			imageBackground.Height = height;
			imagegrid.Height = height;
		}


	}
}
