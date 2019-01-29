using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Collections.Generic;

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
			newimage.ContextMenu = cmImage;
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
			getPreviousFrames();

			if (currentImage.Parent == backImageListView)
			{
				//случай, когда эта же картинка имет этот же фрейм как стоп-маркер
				if (BackInFrameProps.Any(prop => prop.StopFrame == currentFrame && prop.Image == currentImage))
				{
					BackInFrameProps.First(prop => prop.StopFrame == currentFrame && prop.Image == currentImage).StopFrame = null;
					(sender as CheckBox).IsChecked = null;
				}
				else
				{
					if (lastImageChecked != null && lastImageChecked != currentImage)
					{
						lastImageChecked.IsChecked = false;
						if (addorselect) BackInFrameProps.Last(prop => prop.Image == lastImageChecked && previousFrames.Contains(prop.Frame)).StopFrame = currentFrame;
					}
					lastImageChecked = currentImage;

					if (addorselect) BackInFrameProps.Add(new ImageBackProperties() { Frame = currentFrame, Image = currentImage });
					imageBackground.Source = imageShow(currentImage.Path);
					showImagePropsBackground();
				}
			}
			else
			{
				if (addorselect)
					ImageInFrameProps.Add(new ImageCharProperties() { Frame = currentFrame, Image = currentImage, Displayable = newDisplayable() });
				Image img = ImageInFrameProps.Find(prop => prop.Frame == currentFrame && prop.Image == currentImage).Displayable;
				img.Source = imageShow(currentImage.Path);
				imagegrid.Children.Insert(imagegrid.Children.IndexOf(imageBorder), img);
				showImagePropsCharacter(currentImage);
			}
			imagePropsPanel.Visibility = Visibility.Visible;
			show = true;
			waschecked = true;
		}
		
		private void image_Indeterminate(object sender, RoutedEventArgs e)
		{
			//неопределенная метка ресурса обозначает ресурс, который был выбран в одном из предыдущих фреймов
			//если картинка отмечена сейчас, то во всех последующих фреймах она должна быть неопределенной до тех пор, пока какой-либо другой фрейм не присвоит другой фон
			//здесь выхода два - присвоить статус неопределенности "на ходу" основываясь на предыдущих фреймах либо сразу же присваивать неопределенность при выборе данного ресурса
			//проблема первого подхода в том, чтоб рекурсивно дойти до какого-либо предыдущего фрейма, в котором есть первый подходящий связанный ресурс - сложность возникает также в меню, из-за чего трудно пойти "назад"
			//преимущество - отсутствие горы дополнительных действий при загрузке/сохранении проекта и при работе в нем
			//работа программы основана на потоке "вперед", но это шаг, который аннулирует логику работы движка, который проявляет и скрывает соответствующие ресурсы лишь в точках перед определенными фреймами
			//остальные фреймы просто сменяют друг друга, а это означает, что для нормальной работы в программе и адекватной ее разработки необходимо "оборачиваться назад" каждый фрейм
			//однако сам по себе неопределенный ресурс ни на что не влияет, но и должен учитывать те ресурсы, что были скрыты до него, дабы не показывать несколько раз одно и то же
			//из этого исходит следующая логика работы:
			//если в предыдущем кадре нет фона - его нет и в нынешнем
			//если в предыдущем кадре фон пуст, но определен ранее - дойти до кадра, который однозначно определяет фон, и это и будет нынешним фоном
			//однако как определить, нужно ли идти назад по фреймам...

			//что должен делать любой ресурс при неопределенном состоянии - тут
			//а что он должен делать, когда его выбирают как неопределенный? Что вообще ресурсы должны делать при своей неопределенности и как отличить ее от определенных состояний?
			//допустим, когда ЛЮБОЙ ресурс помечается как неопределенный, об этом должна быть соответствующая метка в кадре. А лучше в пропах.
			//самым очевидным вариантом для всего и вся является использование стоп-маркера. 
			//То есть когда ресурс неопределен, но стоп-маркера нет, он неопределен. А когда ресурс и неопределен, и стоп-маркер единичка, он убирается с экрана.
			//а вот как раз стоп-маркер имеет смысл, так как тогда необходимость ставить везде кнопки остановки отпадает... и регулировка происходит по каждому ресурсу отдельно
			//идеально. Что для этого надо? Собрать все ресурсы, что используются во всех предыдущих фреймах, сразу же откинув те, для которых действует стоп-маркер. Бинго.
			//для этого в каждом контенте есть стоп-фрейм, который останавливакет поток определенного ресурса. При соответствии ему ресурс немедленно убирается.
			//а вот несоответствие приводит в неоднозначное положение... поскольку придется рассматривать по данному типу ресурсов все предыдущие кадры, и как только по одному из них найдется данный тип, пумф! - он должен появиться на экране. 
			//жертва во имя удобства пользования программой? Как бы не так... в разы уменьшается количество пропов, однако теперь по ним приходится делать неплохой поиск. Каждый раз, когда выбирается кадр где-то между стартом и стопом.
			//другой тип логики - сохранить все как было, но при выборе ресурса для кадра сразу же проверять, нет ли пропа с тем же ресурсом где-то раньше. 
			//это весьма сбалансированный вариант, однако в таком случае работа стоп-маркера будет отведена отдельно. 
			//отдельным особняком стоит замена стоп-маркера! И та и другая логика говорит, что заменить его можно, найдя соответствующий начальному кадру проп - это и так и так повлияет на все фреймы после. Бинго х2.

			//здесь же должен быть код следующего содержания.
			//ресурс НЕ ДОЛЖЕН быть отмечен как неопределенный вручную. Это нарушает логику, которая должна автоматически выбирать неопределенные ресурсы на основе предыдущих кадров. Из этого имеем:
			//ресурс при переходе из состояния CHECKED в состояние INDETERMINATE обязан удалить проп, который был создан при его первоначальном выборе, и перейти сразу в состояние UNCHECKED.
			//почему так - ресурс был вызван в этом кадре, потому когда мы меняем его состояние сами, у нас не может быть этого же ресурса ранее.
			//ресурс при переходе из INDETERMINATE в состояние UNCHECKED, не будучи изначально CHECKED, обязан становиться стоп-маркером в пропе, который имеет тот же ресурс и один из предыдущих фреймов.
			//почему так - если ресурс имеет стартовый маркер в предыдущих фреймах, он отмечен там и проп удалять не нужно, однако мы должны остановить показ этого ресурса в данном кадре и потому даем стоп-маркер.
			//ресурс при переходе из UNCHECKED в CHECKED создает соответствующий проп с выбранным кадром в качестве стартового.		
			if (waschecked&&addorselect) (sender as CheckBox).IsChecked = false;
			else { imageBackground.Source = imageShow(((sender as CheckBox).Tag as XImage).Path); };
		}

		private void image_Unchecked(object sender, RoutedEventArgs e)
		{
			XImage selectedImage = (sender as CheckBox).Tag as XImage;
			//Теперь INDETERMINATE определяет удаление пропа. UNCHECKED определяет присутствие стоп-маркера.
			if (waschecked)
			{
				if (selectedImage.Parent == backImageListView)
				{
					ImageBackProperties imgtoremove = BackInFrameProps.Find(i => i.Frame == currentFrame && i.Image == selectedImage);
					if (removeorunselect) BackInFrameProps.Remove(imgtoremove);
					imageBackground.Source = null;
				}
				else
				{
					ImageCharProperties imgtoremove = ImageInFrameProps.Find(i => i.Frame == currentFrame && i.Image == selectedImage);
					if (removeorunselect) ImageInFrameProps.Remove(imgtoremove);
					imagegrid.Children.Remove(imgtoremove.Displayable);
				}
				waschecked = false;
			}
			else
			{
				getPreviousFrames();
				if (selectedImage.Parent == backImageListView)
				{
					ImageBackProperties imgtostop = BackInFrameProps.Last(i => previousFrames.Contains(i.Frame)&& i.Image == selectedImage);
					if (removeorunselect) imgtostop.StopFrame = currentFrame;
					imageBackground.Source = null;
				}
				else
				{
					ImageCharProperties imgtostop = ImageInFrameProps.Last(i => previousFrames.Contains(i.Frame) && i.Image == selectedImage);
					if (removeorunselect) imgtostop.StopFrame = currentFrame;
					imagegrid.Children.Remove(imgtostop.Displayable);
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
			BitmapImage bitmapToShow = new BitmapImage();
			bitmapToShow.BeginInit();
			bitmapToShow.UriSource = new Uri(path);
			bitmapToShow.CacheOption = BitmapCacheOption.OnLoad;
			bitmapToShow.EndInit();
			bitmapToShow.DecodePixelWidth = Convert.ToInt32(bitmapToShow.Width * (bitmapToShow.DpiX / 96) * (bitmapToShow.DpiX / 96));
			bitmapToShow.DecodePixelHeight = Convert.ToInt32(bitmapToShow.Height * (bitmapToShow.DpiY / 96) * (bitmapToShow.DpiY / 96));

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
