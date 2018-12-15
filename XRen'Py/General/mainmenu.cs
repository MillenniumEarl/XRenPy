using System;
using System.Windows;
using System.IO;
using Ookii.Dialogs.Wpf;
using System.Windows.Controls;

namespace X_Ren_Py
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private void clearAll()
		{
			framecount = 0;
			projectExpander.IsExpanded = false;
			for(int i=2; i<imagegrid.Children.IndexOf(imageBorder);i++) imagegrid.Children.RemoveAt(i);
			for (int i = 0; i <tabControlStruct.Items.Count; i++) tabControlStruct.Items.RemoveAt(i);
			foreach (TabItem tab in tabControlResources.Items) (tab.Content as ListView).Items.Clear();
			ImageInFrameProps.Clear(); AudioInFrameProps.Clear();
		}

		private void NewProject_Click(object sender, RoutedEventArgs e)
		{
			clearAll();
			//код загрузки нового проекта
		}

		private void LoadProject_Click(object sender, RoutedEventArgs e)
		{
			VistaFolderBrowserDialog selectFolder = new VistaFolderBrowserDialog();

			if (selectFolder.ShowDialog() == true)

				if (File.Exists(selectFolder.SelectedPath.ToString() + script) &&
				File.Exists(selectFolder.SelectedPath.ToString() + options) &&
				File.Exists(selectFolder.SelectedPath.ToString() + gui))
				{
					projectExpander.IsExpanded = false;
					projectFolder = selectFolder.SelectedPath.ToString() + game;
					clearAll();					
					loadScript();
					loadOptions();
					loadGUI();
				}
				else MessageBox.Show("Not a project folder or some files are missing!", "Incorrect folder", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		
		private void SaveProject_Click(object sender, RoutedEventArgs e)
		{
			saveScript();
			saveOptions();
			saveGUI();		
		}

		private void SaveProjectAS_Click(object sender, RoutedEventArgs e)
		{
			VistaFolderBrowserDialog selectFolder = new VistaFolderBrowserDialog();

			if (selectFolder.ShowDialog() == true)
				
				{ string tempFolder = projectFolder;
				projectFolder = selectFolder.SelectedPath.ToString()+game;
					if (!(File.Exists(selectFolder.SelectedPath.ToString() + script) && 
					File.Exists(selectFolder.SelectedPath.ToString() + options) &&
					File.Exists(selectFolder.SelectedPath.ToString() + gui)))
					{							
						saveScript();
						saveOptions();
						saveGUI();
				}
					else
					{
						MessageBoxResult result = MessageBox.Show("Existing project folder! Replace?", "Incorrect folder", MessageBoxButton.YesNoCancel, MessageBoxImage.Error);
						if (result == MessageBoxResult.Yes)
						{
						try {
							Directory.Delete(projectFolder, true);
							saveScript();
							saveOptions();
							saveGUI();
						}
						catch (Exception)
						{
							MessageBox.Show("Impossible to delete the folder!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
							projectFolder = tempFolder;
						}						
						}
					}
				}
		}

		private void exportHideImage(StreamWriter writer, ImageProperties property)
		{
			string animationType="";
			if (property.AnimationOutType != 0) animationType = " with " + animationOutTypeComboBox.Items[property.AnimationOutType];
			writer.WriteLine(tab + "hide " + property.Image.Alias+animationType);
		}

		private void exportShowImage(StreamWriter writer, ImageProperties property)
		{
			string align = "";
			string animationType = "";			
			if (property.Align != 0) align = " at " + alignComboBox.Items[property.Align];
			if (property.AnimationInType != 0) animationType = " with " + animationInTypeComboBox.Items[property.AnimationInType];
			writer.WriteLine(tab + "show " + property.Image.Alias + align + animationType);
		}

		private void Exit_Click(object sender, RoutedEventArgs e){Close();}
		public string equalsQuote(string content) { return "=\"" + content + "\""; }
		public string quote(string content) { return "\"" + content + "\""; }
	}
}

