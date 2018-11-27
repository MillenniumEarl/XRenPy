using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Input;

namespace X_Ren_Py
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private void imageImport_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog imageDialog = new OpenFileDialog() { Filter = imageextensions, Multiselect = true };

			if (imageDialog.ShowDialog() == true)
				for (int file = 0; file < imageDialog.FileNames.Length; file++)
				{
					try
					{
						string name = imageDialog.SafeFileNames[file];
						string currentPath = imageDialog.FileNames[file];

						XImage newimage = new XImage() { Header = name, Path = currentPath};
						imageMouseActions(newimage);

						if (tabControlResources.SelectedContent == backImageListView) { backImageListView.Items.Add(newimage); }
						else { imageListView.Items.Add(newimage); }
					}
					catch (Exception) { MessageBox.Show("Please choose the image!", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
				}
		}

		private void imageMouseActions(XImage newimage)
		{
			newimage.ContextMenu = cmImage;
			newimage.Selected += content_Selected;
			newimage.MouseLeave += image_MouseLeave;
			newimage.MouseEnter += image_Enter;
			newimage.Checkbox.Checked += image_Checked;
			newimage.Checkbox.Unchecked += image_Unchecked;
		}

		private void image_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!show)
				media.IsExpanded = false;
			else
			{
				audioPropsPanel.Visibility = Visibility.Collapsed;
				imagePropsPanel.Visibility = Visibility.Collapsed;
				if (currentImage != null)
				{
					imageViewer.Source = imageShow(currentImage.Path.ToString());
					mediaNameLabel.Content = currentImage.Header;
					if (currentImage.IsChecked == true)
					{
						imagePropsPanel.Visibility = Visibility.Visible;
						if (currentImage.Parent == imageListView)
							showImagePropsCharacter(currentImage);
						else
						{
							alignLabel.Visibility = Visibility.Collapsed;
							alignComboBox.Visibility = Visibility.Collapsed;
						};
					}
				}
			}
		}
	

		private void image_Enter(object sender, RoutedEventArgs e)
        {
			XImage imageUnderCursor = sender as XImage;
            imageViewer.Source = imageShow(imageUnderCursor.Path.ToString());
            
			if (imageUnderCursor.IsChecked == false) imagePropsPanel.Visibility = Visibility.Hidden;
			else
			{
				imagePropsPanel.Visibility = Visibility.Visible;
				if (imageUnderCursor.Parent == imageListView)
				{
					showImagePropsCharacter(imageUnderCursor);
				}
				else
				{
					showImagePropsBackground();
				}
			}
			audiomovie.Visibility = Visibility.Hidden;
            imageViewer.Visibility = Visibility.Visible;
            media.Visibility = Visibility.Visible;
            media.IsExpanded = true;
        }

		private void image_Checked(object sender, RoutedEventArgs e)
		{
			
			currentImage = ((sender as CheckBox).Parent as StackPanel).Parent as XImage;
			
			if (currentImage.Parent == backImageListView)
			{				
				if (lastImageChecked != null && lastImageChecked != currentImage) lastImageChecked.IsChecked = false;
				lastImageChecked = currentImage;

				if (addorselect) currentFrame.BackgroundImage = currentImage;
				imageBackground.Source = imageShow(currentImage.Path.ToString());
				showImagePropsBackground();
			}
			else
			{
				if (addorselect)
				{					
					ImageInFrameProps.Add(new ImageProperties() { Frame = currentFrame, Image = currentImage, Displayable = newDisplayable() });
				}
				Image img = ImageInFrameProps.Find(prop => prop.Frame == currentFrame && prop.Image == currentImage).Displayable;
				img.Source = imageShow(currentImage.Path.ToString());
				imagegrid.Children.Insert(imagegrid.Children.IndexOf(imageBorder), img);
				showImagePropsCharacter(currentImage);
			}
			imagePropsPanel.Visibility = Visibility.Visible;			
			show = true;
			if (addorselect) selectCheckedItem(sender);
		}

		private Image newDisplayable()
		{
			return new Image()
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Bottom,
				Stretch = System.Windows.Media.Stretch.None
			};
		}

		private void image_Unchecked(object sender, RoutedEventArgs e)
        {
			if (addorselect) selectCheckedItem(sender);
            XImage selectedImage = ((sender as CheckBox).Parent as StackPanel).Parent as XImage;
            if (selectedImage.Parent == backImageListView)
            {
                if (removeorunselect) currentFrame.BackgroundImage = null;
                imageBackground.Source = null;
            }
            else
            {
                string source = new Uri(selectedImage.Path).ToString();
				ImageProperties imgtoremove = ImageInFrameProps.Find(i => i.Frame == currentFrame && i.Image == selectedImage);
				if (removeorunselect) ImageInFrameProps.Remove(imgtoremove); 
				imagegrid.Children.Remove(imgtoremove.Displayable);
            }
			imagePropsPanel.Visibility = Visibility.Hidden;
			show = false;
        }
               
        private void imageDeleteFromList_Click(object sender, RoutedEventArgs e)
        {
            foreach (ImageProperties image in ImageInFrameProps.Where(image=>image.Image==sender)) ImageInFrameProps.Remove(image);
            resourcesSelectedItem_delete();
        }
        
        private BitmapImage imageShow(string path)
        {
            BitmapImage bitmapToShow = new BitmapImage();
            bitmapToShow.BeginInit();
            bitmapToShow.UriSource = new Uri(path);
            bitmapToShow.EndInit();
            return bitmapToShow;
        }

		public void getImageProperties(XFrame frame, XImage image)
		{
			ImageProperties property = ImageInFrameProps.Find(prop => prop.Frame == frame && prop.Image == image);
			alignComboBox.SelectedIndex = property.Align;
			animationInTypeComboBox.SelectedIndex = property.AnimationInType;
			animationOutTypeComboBox.SelectedIndex = property.AnimationOutType;
			audioPropsPanel.Visibility = Visibility.Hidden;
		}

		private void showImagePropsCharacter(XImage image)
		{
			getImageProperties(currentFrame, image);
			alignLabel.Visibility = Visibility.Visible;
			alignComboBox.Visibility = Visibility.Visible;
		}

		private void showImagePropsBackground()
		{
			alignLabel.Visibility = Visibility.Collapsed;
			alignComboBox.Visibility = Visibility.Collapsed;
			animationInTypeComboBox.SelectedIndex = currentFrame.AnimationInType;
			animationOutTypeComboBox.SelectedIndex = currentFrame.AnimationOutType;
		}
		private void alignComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
				ImageProperties img = ImageInFrameProps.Find(prop => prop.Frame == currentFrame && prop.Image == currentImage);
				img.Align = (byte)alignComboBox.SelectedIndex;
				switch (alignComboBox.SelectedIndex)
				{
					case 0: img.Displayable.HorizontalAlignment = HorizontalAlignment.Center; break;
					case 1: img.Displayable.HorizontalAlignment = HorizontalAlignment.Left; break;
					case 2: img.Displayable.HorizontalAlignment = HorizontalAlignment.Right; break;
				}
			}
		private void animationTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender == animationInTypeComboBox)
			{
				if (tabControlResources.SelectedContent == backImageListView) { currentFrame.AnimationInType = (byte)animationInTypeComboBox.SelectedIndex; }
				else { ImageInFrameProps.Find(prop => prop.Frame == currentFrame && prop.Image == currentImage).AnimationInType = (byte)animationInTypeComboBox.SelectedIndex; }
			}
			else
			{
				if (tabControlResources.SelectedContent == backImageListView) { currentFrame.AnimationOutType = (byte)animationOutTypeComboBox.SelectedIndex; }
				else { ImageInFrameProps.Find(prop => prop.Frame == currentFrame && prop.Image == currentImage).AnimationOutType = (byte)animationOutTypeComboBox.SelectedIndex; }
			}
		}
	}
}
