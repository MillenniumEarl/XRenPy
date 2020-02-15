using System;
using System.Windows;
using System.IO;
using Ookii.Dialogs.Wpf;
using System.Windows.Controls;
using System.Windows.Media;

namespace X_Ren_Py
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private void NewProject_Click(object sender, RoutedEventArgs e)
		{
			clearAll();
			//код загрузки нового проекта
			emptyProject();
		}


		private void LoadProject_Click(object sender, RoutedEventArgs e)
		{
			VistaFolderBrowserDialog selectFolder = new VistaFolderBrowserDialog();

			if (selectFolder.ShowDialog() == true)

				if (File.Exists(selectFolder.SelectedPath.ToString() + script) &&
				File.Exists(selectFolder.SelectedPath.ToString() + options) &&
				File.Exists(selectFolder.SelectedPath.ToString() + gui))
                {
                    MainWindow mainwindow = this;
                    MainWindow newwindow = new MainWindow();
                    try
                    {
                        newwindow.projectExpander.IsExpanded = false;
                        newwindow.projectFolder = selectFolder.SelectedPath.ToString() + game;
                        newwindow.clearAll();
                        newwindow.loadScript();
                        newwindow.loadOptions();
                        newwindow.loadGUI();
                        Close();
                        newwindow.Activate();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Can't load the project!", "Incompatible files", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
				}
				else { MessageBox.Show("Not a project folder or some files are missing!", "Incorrect folder", MessageBoxButton.OK, MessageBoxImage.Error); }
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

		private void Exit_Click(object sender, RoutedEventArgs e){Close();}

		private void clearAll()
		{
			projectExpander.IsExpanded = false;
			uncheckAll();
			for (int i = 2; i < imagegrid.Children.IndexOf(imageBorder); ) imagegrid.Children.RemoveAt(i);
			foreach (TabItem tab in tabControlResources.Items) (tab.Content as ListView).Items.Clear();
			tabControlStruct.Items.Clear();
			tabControlStruct.Items.Add(addTab);
			BackInFrameProps.Clear();
			ImageInFrameProps.Clear();
			AudioInFrameProps.Clear();
			menuLabelList.Clear();
			characterList.Clear();
			disptimer.Stop();
			characterLabel.Content = "none";
			textBox.Text = "";
			characterLabel.Foreground= new SolidColorBrush(default_ColorHeaders.SelectedColor.Value);
			textBox.Foreground = new SolidColorBrush(default_ColorText.SelectedColor.Value);
		}

		private void emptyProject()
		{
			projectFolder = Environment.CurrentDirectory + @"/temp" + game;
            //characters
            characterList.Add(charNone);
            characterList.Add(charNvl);
            characterList.Add(charCentered);
            characterList.Add(charExtend);
            characterListView.SelectedItem=charNone;
            characterSelector.SelectedItem = charNone;
            currentCharacter = charNone;
            //start			
            ListView startListView = createLabel("start");
			XFrame firstFrame = createFrame();
			startListView.Items.Add(firstFrame);
			currentFrame = firstFrame;
			firstFrame.IsSelected = true;
        }
	}
}

