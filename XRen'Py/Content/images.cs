using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;

namespace X_Ren_Py
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public class XImage : XContent
		{
			public XImage()	{ContextMenu = cmImage;}
			public void loadImage(string singleLine, string folder)
			{
				try
				{
					int firstquote = singleLine.IndexOf('"') + 1;
					Path = folder + singleLine.Substring(firstquote, singleLine.LastIndexOf('"') - firstquote);
					Header = Path.Replace(folder, "").Substring(singleLine.LastIndexOf('/') + 1);
					Alias = singleLine.Substring(6, singleLine.IndexOf('=') - 6).TrimEnd(' ');
				}
				catch (Exception) { MessageBox.Show("Error: Image loading", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
			}
		}
		public class ImageBackProperties : ContentProperties
		{
            public XImage Image { get; set; }
            public byte AnimationInType { get; set; }
            public byte AnimationOutType { get; set; }
        }
		
		public class ImageCharProperties : ImageBackProperties
		{
            public Image Displayable { get; set; }
            public byte Align { get; set; }
        }

		private void imageImport_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog imageDialog = new OpenFileDialog() { Filter = imageextensions, Multiselect = true };

			if (imageDialog.ShowDialog() == true)
				for (int file = 0; file < imageDialog.FileNames.Length; file++)
				{
					try
					{
						string name = imageDialog.SafeFileNames[file];
						if (!(tabControlResources.SelectedContent as ListView).Items.OfType<XImage>().Any(item => item.Header == name))
						{
							XImage image = new XImage() { Header = name, Path = projectFolder + imagesFolder + name };							
							imageMouseActions(image);

							if (tabControlResources.SelectedContent != sideListView) (tabControlResources.SelectedContent as ListView).Items.Add(image);
							else { image.Checkbox.Visibility = Visibility.Hidden; sideListView.Items.Add(image); }

							contentCollector(imageDialog.FileNames[file], image.Path);
						}		
						else MessageBox.Show("Image is already in use!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
					catch (Exception) { MessageBox.Show("Please choose the image!", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
				}
		}

		private void imageReload_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog imageDialog = new OpenFileDialog() { Filter = imageextensions };
			if (imageDialog.ShowDialog() == true)
				try
				{
					currentImage.Header = imageDialog.SafeFileName;
					currentImage.Path = imageDialog.FileName;
					imageViewer.Source = imageShow(currentImage.Path);
					currentImage.TextColor = Brushes.Black;
					currentImage.Checkbox.IsEnabled = true;
				}
				catch (Exception)
				{
					MessageBox.Show("Invalid image", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
		}

		private void imageMouseActions(XImage newimage)
		{
			newimage.Selected += content_Selected;
			newimage.MouseUp += content_Selected;
			newimage.MouseLeave += image_MouseLeave;
			newimage.MouseEnter += image_Enter;
			newimage.Checkbox.Checked += image_Checked;
			newimage.Checkbox.Unchecked += image_Unchecked;
			newimage.Checkbox.Indeterminate += image_Indeterminate;
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
					imageViewer.Source = imageShow(currentImage.Path);
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
			if (File.Exists(imageUnderCursor.Path))
			{
				imageUnderCursor.TextColor = Brushes.Black;
				imageUnderCursor.Checkbox.IsEnabled = true;
				imageViewer.Source = imageShow(imageUnderCursor.Path);

				if (imageUnderCursor.IsChecked != true) imagePropsPanel.Visibility = Visibility.Hidden;
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
			else { imageUnderCursor.TextColor = Brushes.Red; imageUnderCursor.Checkbox.IsEnabled = false; }
		}

		private void image_Checked(object sender, RoutedEventArgs e)
		{
			currentImage = (sender as CheckBox).Tag as XImage;

			if (currentImage.Parent == backImageListView)
			{
				//случай, когда эта же картинка имет этот же фрейм как стоп-маркер
				if (BackInFrameProps.Any(prop => (prop.StopFrame == currentFrame || (prop.StopFrames!=null&& prop.StopFrames.Intersect(previousFrames)==currentFrame)) && prop.Image == currentImage))
				{
					ImageBackProperties back = BackInFrameProps.First(prop => (prop.StopFrame == currentFrame || (prop.StopFrames != null && prop.StopFrames.Intersect(previousFrames) == currentFrame)) && prop.Image == currentImage);
					if (back.StopFrame == currentFrame) back.StopFrame = null; else back.StopFrames.Remove(currentFrame);
					(sender as CheckBox).IsChecked = null;
				}
				else
				{
					if (lastBackChecked != null && lastBackChecked != currentImage)
					{
						if(addorselect) BackInFrameProps.Last(prop => prop.Image == lastBackChecked && previousFrames.Contains(prop.Frame)).StopFrame = currentFrame;
						lastBackChecked.IsChecked = false;						
					}
					lastBackChecked = currentImage;

					if (addorselect) BackInFrameProps.Add(new ImageBackProperties() { Frame = currentFrame, Image = currentImage });
					imageBackground.Source = imageShow(currentImage.Path);
					showImagePropsBackground();
				}
			}
			else
			{
				if (ImageInFrameProps.Any(prop => (prop.StopFrame == currentFrame || (prop.StopFrames != null && prop.StopFrames.Intersect(previousFrames) == currentFrame)) && prop.Image == currentImage))
				{
					ImageCharProperties img= ImageInFrameProps.First(prop => (prop.StopFrame == currentFrame || (prop.StopFrames != null && prop.StopFrames.Intersect(previousFrames) == currentFrame)) && prop.Image == currentImage);
					if (img.StopFrame == currentFrame) img.StopFrame = null; else img.StopFrames.Remove(currentFrame);
					(sender as CheckBox).IsChecked = null;
				}
				else
				{
					if (addorselect) ImageInFrameProps.Add(new ImageCharProperties() { Frame = currentFrame, Image = currentImage, Displayable = newDisplayable() });
					Image img = ImageInFrameProps.Find(prop => prop.Frame == currentFrame && prop.Image == currentImage).Displayable;
					img.Source = imageShow(currentImage.Path);
					imagegrid.Children.Insert(imagegrid.Children.IndexOf(imageBorder), img);
					showImagePropsCharacter(currentImage);
				}
			}
			if (addorselect) imagePropsPanel.Visibility = Visibility.Visible;
			show = true;
			waschecked = true;
		}
		
		private void image_Indeterminate(object sender, RoutedEventArgs e)
		{
			if (waschecked&&addorselect) (sender as CheckBox).IsChecked = false;
			else {
				XImage selectedImage = (sender as CheckBox).Tag as XImage;
				if (selectedImage.Parent == backImageListView) imageBackground.Source = imageShow(selectedImage.Path);
				else imagegrid.Children.Insert(imagegrid.Children.IndexOf(imageBorder), ImageInFrameProps.Last(prop => previousFrames.Contains(prop.Frame) && prop.Image == selectedImage).Displayable);
			};
		}

		private void image_Unchecked(object sender, RoutedEventArgs e)
		{
			XImage selectedImage = (sender as CheckBox).Tag as XImage;
			//Теперь INDETERMINATE определяет удаление пропа. UNCHECKED определяет присутствие стоп-маркера.
			if (waschecked)
			{
				if (selectedImage.Parent == backImageListView)
				{
					if (removeorunselect) BackInFrameProps.Remove(BackInFrameProps.First(i => i.Frame == currentFrame && i.Image == selectedImage));
					imageBackground.Source = null;
				}
				else
				{
					ImageCharProperties imgtoremove = ImageInFrameProps.First(i => i.Frame == currentFrame && i.Image == selectedImage);
					imagegrid.Children.Remove(imgtoremove.Displayable);
						if (removeorunselect) ImageInFrameProps.Remove(imgtoremove);
				}
				waschecked = false;
			}
			else
			{
				if (selectedImage.Parent == backImageListView)
				{	if (BackInFrameProps.Any(i => previousFrames.Contains(i.Frame) && i.Image == selectedImage))
					{
						ImageBackProperties imgtostop = BackInFrameProps.Last(i => previousFrames.Contains(i.Frame) && i.Image == selectedImage);
						if (removeorunselect) if (imgtostop.Frame.MenuOptions==null) imgtostop.StopFrame = currentFrame; else imgtostop.StopFrames.Add(currentFrame);
					}
					imageBackground.Source = null;										
				}
				else
				{if (ImageInFrameProps.Any(i => previousFrames.Contains(i.Frame) && i.Image == selectedImage))
					{
						ImageCharProperties imgtostop = ImageInFrameProps.Last(i => previousFrames.Contains(i.Frame) && i.Image == selectedImage);
						if (removeorunselect) if (imgtostop.Frame.MenuOptions == null) imgtostop.StopFrame = currentFrame; else imgtostop.StopFrames.Add(currentFrame);
						imagegrid.Children.Remove(imgtostop.Displayable);
					}
				}
			}
			imagePropsPanel.Visibility = Visibility.Hidden;
			show = false;
		}
	
		private Image newDisplayable()
		{
			return new Image()
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Bottom,
				Stretch = Stretch.None,
				ClipToBounds = true
			};
		}

		private void imageDeleteFromList_Click(object sender, RoutedEventArgs e)
		{	
			foreach (ImageCharProperties image in ImageInFrameProps.Where(image => image.Image == sender)) ImageInFrameProps.Remove(image);
			resourcesSelectedItem_delete();
		}

		private BitmapImage imageShow(string path)
		{
			System.Drawing.Bitmap DPI = new System.Drawing.Bitmap(path);
			int pxwdth = Convert.ToInt32(DPI.Width * (DPI.HorizontalResolution / 96));
			int pxhght = Convert.ToInt32(DPI.Height * (DPI.VerticalResolution / 96));
			DPI.Dispose();

			BitmapImage bitmapToShow = new BitmapImage();
			bitmapToShow.BeginInit();
			bitmapToShow.UriSource = new Uri(path);
			bitmapToShow.CacheOption = BitmapCacheOption.OnLoad;
			bitmapToShow.DecodePixelWidth = pxwdth;
			bitmapToShow.DecodePixelHeight = pxhght;
			bitmapToShow.EndInit();

			return bitmapToShow;
		}

		private void showImagePropsCharacter(XImage image)
		{
			ImageCharProperties property = ImageInFrameProps.Find(prop => prop.Frame == currentFrame && prop.Image == image);
			alignComboBox.SelectedIndex = property.Align;
			animationInTypeComboBox.SelectedIndex = property.AnimationInType;
			animationOutTypeComboBox.SelectedIndex = property.AnimationOutType;
			audioPropsPanel.Visibility = Visibility.Hidden;
			alignLabel.Visibility = Visibility.Visible;
			alignComboBox.Visibility = Visibility.Visible;
		}

		private void showImagePropsBackground()
		{
			ImageBackProperties property = BackInFrameProps.Find(prop => prop.Frame == currentFrame);
			animationInTypeComboBox.SelectedIndex = property.AnimationInType;
			animationOutTypeComboBox.SelectedIndex = property.AnimationOutType;
			audioPropsPanel.Visibility = Visibility.Hidden;
			alignLabel.Visibility = Visibility.Collapsed;
			alignComboBox.Visibility = Visibility.Collapsed;
		}

		private void alignComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ImageCharProperties img = ImageInFrameProps.Find(prop => prop.Frame == currentFrame && prop.Image == currentImage);
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
				if (tabControlResources.SelectedContent == backImageListView)
				 BackInFrameProps.Find(prop => prop.Frame == currentFrame && prop.Image == currentImage).AnimationInType = (byte)animationInTypeComboBox.SelectedIndex; 
				else ImageInFrameProps.Find(prop => prop.Frame == currentFrame && prop.Image == currentImage).AnimationInType = (byte)animationInTypeComboBox.SelectedIndex;
			}
			else
			{
				if (tabControlResources.SelectedContent == backImageListView)
				 BackInFrameProps.Find(prop => prop.Frame == currentFrame && prop.Image == currentImage).AnimationOutType = (byte)animationOutTypeComboBox.SelectedIndex; 
				else ImageInFrameProps.Find(prop => prop.Frame == currentFrame && prop.Image == currentImage).AnimationOutType = (byte)animationOutTypeComboBox.SelectedIndex; 
			}
		}
	}
}
