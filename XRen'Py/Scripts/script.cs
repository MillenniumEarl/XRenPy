using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace X_Ren_Py
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private void loadScript()
		{
			//код для загрузки script.rpy
			FileStream fs = new FileStream(projectFolder + "script.rpy", FileMode.Open);
			StreamReader reader = new StreamReader(fs);
			//контент ОЧЕНЬ неоднозначен, потому для его правильного распознания нужен код и в init, и в метках
			string singleLine;
			while (!reader.EndOfStream)
			{
				singleLine = reader.ReadLine().TrimStart(' ');
				for (int compare = 0; compare < comparerScript.Length; compare++)
				{
					if (singleLine.StartsWith(comparerScript[compare]))
						switch (comparerScript[compare])
						{//INIT
							case "define":
								if (singleLine.Contains("Character"))
								{
									XCharacter character = new XCharacter();
									character.loadCharacter(singleLine, characterIcons);
									characterListView.Items.Add(character);
								}
								else if (singleLine.Contains("audio."))
								{
									XAudio audio = new XAudio();
									audio.loadAudio(singleLine, projectFolder + game);
									audioMouseActions(audio);
									musicListView.Items.Add(audio);
								}
								break;
							case "image":
								if (!singleLine.Contains("Movie"))
								{	if (singleLine.StartsWith("image side"))
										characterIcons.Add(new Image() { Source = imageShow(projectFolder + singleLine.Substring(singleLine.IndexOf('"')).Replace("\"", "")) });
									else
									{
										XImage image = new XImage();
										image.loadImage(singleLine, projectFolder + "images\\");
										imageMouseActions(image);
										backImageListView.Items.Add(image);
									}
								}
								else
								{
									XMovie movie = new XMovie();
									movie.loadMovie(singleLine, projectFolder + "images\\");
									movieMouseActions(movie);
									movieListView.Items.Add(movie);
								}
								break;
							case "label":
								{
									ListView newLabel = createLabel(singleLine.Substring(6, singleLine.Length - 7));
									ImageProperties Background = new ImageProperties();
									XFrame frame;
									bool root = true;
									string readingLine = reader.ReadLine().TrimStart(' ');
									while (readingLine != "return")
									{
										frame = createFrame(root);
										List<string> framebody = new List<string> { readingLine };

										while (!Regex.IsMatch(readingLine, @"([\S\s]+)?.*""([\S\s]+)?.*"""))
										{
											readingLine = reader.ReadLine().TrimStart(' ');
											framebody.Add(readingLine);
										}

										foreach (string line in framebody)
											if (!Regex.IsMatch(line, @"([\S\s]+)?.*""([\S\s]+)?.*"""))
											{
												switch (line.Substring(0, line.IndexOf(' ')))
												{
													case "scene":
														{
															string[] all = line.Split(' ');
															frame.BackgroundImage = backImageListView.Items.OfType<XImage>().First(item => item.Alias == all[1]);
															if (all.Length > 2) if (all[2] == "with") frame.AnimationInType = (byte)animationInTypeComboBox.Items.IndexOf(animationInTypeComboBox.Items.OfType<string>().First(item => item == all[3]));
															Background = frame.BackgroundImageProps;
															break;
														}
													case "show":
														{
															string[] all = line.Split(' ');
															XImage image = backImageListView.Items.OfType<XImage>().First(item => item.Alias == all[1]);
															backImageListView.Items.Remove(image);
															imageListView.Items.Add(image);
															ImageProperties props = new ImageProperties() { Frame = frame, Image = image, Displayable = newDisplayable() };

															for (int i = 2; i < all.Length; i++)
															{
																if (all[i] == "with") props.AnimationInType = (byte)animationInTypeComboBox.Items.IndexOf(animationInTypeComboBox.Items.OfType<string>().First(item => item == all[i + 1]));
																else if (all[i] == "at") props.Align = (byte)alignComboBox.Items.IndexOf(alignComboBox.Items.OfType<string>().First(item => item == all[i + 1]));
															}
															ImageInFrameProps.Add(props);
															break;
														}
													case "hide":
														{
															string[] all = line.Split(' ');
															if (backImageListView.Items.OfType<XImage>().Any(item => item.Alias == all[1]))
															{
																frame.BackgroundImage = null;
																if (all.Length > 2) if (all[2] == "with")
																		Background.AnimationOutType = (byte)animationOutTypeComboBox.Items.IndexOf(animationOutTypeComboBox.Items.OfType<string>().First(item => item == all[3]));
															}
															//по нынешней логике, надо найти первый элемент с этой же картинкой, остальные просто игнорируются
															else
																if (all.Length > 2) if (all[2] == "with")
																	ImageInFrameProps.First(item => item.Image.Alias == all[1]).AnimationOutType = (byte)animationOutTypeComboBox.Items.IndexOf(animationOutTypeComboBox.Items.OfType<string>().First(item => item == all[3]));
															break;
														}
													case "stop":
														{
															frame.stopAudio = true;
															break;
														}
													case "play":
														{
															string[] all = line.Split(' ');
															XAudio audio = musicListView.Items.OfType<XAudio>().First(item => item.Alias == all[2]);
															if (all[1] != "music")
															{
																musicListView.Items.Remove(audio);
																if (all[1] == "sound") soundListView.Items.Add(audio);
																else voiceListView.Items.Add(audio);
															}
															AudioProperties props = new AudioProperties() { Frame = frame, Audio = audio };
															for (int i = 2; i < all.Length; i++)
															{
																if (all[i] == "fadein") props.FadeIn = float.Parse(all[i + 1]);
																else if (all[i] == "fadeout") props.FadeOut = float.Parse(all[i + 1]);
																else if (all[i] == "noloop") props.Loop = false;
															}
															AudioInFrameProps.Add(props);
															break;
														}
													default: break;
												}
											}
											else
											{
												if (line.IndexOf('"') == 0) { frame.Character = characterListView.Items[0] as XCharacter; frame.Text = line.Replace("\"", ""); }
												else
												{
													frame.Character = characterListView.Items.OfType<XCharacter>().First(item => item.Alias == line.Substring(0, line.IndexOf(' ')));
													textBox.Text = line.Substring(line.IndexOf(' ')).Replace("\"", " ");
												}
											}
										newLabel.Items.Add(frame);
										root = false;
										readingLine = reader.ReadLine().TrimStart(' ');
									}
								}
								break;
							default: return;
						}
				}

			}
		}

		private void saveScript()
		{
			projectExpander.IsExpanded = false;
			createDirectories();
			FileStream fs = new FileStream(projectFolder + "script.rpy", FileMode.Create);
			StreamWriter writer = new StreamWriter(fs);
			writer.WriteLine(scriptstart + nextLine);
			//init
			writer.WriteLine(backgroundImages);
			foreach (XImage image in backImageListView.Items)
			{
				writer.WriteLine("image " + image.Alias + equalsQuote(image.Header));
				contentCollector(image.Path, projectFolder + imagesFolder + image.Header);
			}

			writer.WriteLine(characterImages);
			foreach (XImage image in imageListView.Items)
			{
				writer.WriteLine("image " + image.Alias + equalsQuote(image.Header));
				contentCollector(image.Path, projectFolder + imagesFolder + image.Header);
			}

			writer.WriteLine(musicAudio);
			foreach (XAudio audio in musicListView.Items)
			{
				writer.WriteLine(define + "audio." + audio.Alias + equalsQuote(musicFolder + audio.Header));
				contentCollector(audio.Path, projectFolder + musicFolder + audio.Header);
			}

			writer.WriteLine(soundsAudio);
			foreach (XAudio audio in soundListView.Items)
			{
				writer.WriteLine(define + "audio." + audio.Alias + equalsQuote(soundsFolder + audio.Header));
				contentCollector(audio.Path, projectFolder + soundsFolder + audio.Header);
			}

			writer.WriteLine(voiceAudio);
			foreach (XAudio audio in voiceListView.Items)
			{
				writer.WriteLine(define + "audio." + audio.Alias + equalsQuote(voicesFolder + audio.Header));
				contentCollector(audio.Path, projectFolder + voicesFolder + audio.Header);
			}

			writer.WriteLine(Movies);
			foreach (XMovie movie in movieListView.Items)
			{
				string mask = "";
				if (movie.MaskPath != null) mask = ", mask" + equalsQuote(moviesFolder + movie.MaskPath);
				writer.WriteLine("image " + movie.Alias + "=Movie(play" + equalsQuote(moviesFolder + movie.Header) + mask + ")");
				contentCollector(movie.Path, projectFolder + moviesFolder + movie.Header);
			}

			writer.WriteLine(Characters);
			for (int i = 3; i < characterListView.Items.Count; i++)
			{
				XCharacter chosenCharacter = characterListView.Items[i] as XCharacter;
				string icon="",color = "", bold = "", italic = "", what_color = "", what_bold = "", what_italic = "";
				if (chosenCharacter.Icon != null) { icon = ", image" + equalsQuote(chosenCharacter.Alias); writer.WriteLine("image side " + chosenCharacter.Alias + equalsQuote(chosenCharacter.Content.ToString())); }
				if (chosenCharacter.NameColor.ToString() != "") color = ", color" + equalsQuote(chosenCharacter.NameColor.ToString().Remove(1, 2));
				if (chosenCharacter.NameIsBold) bold = ", who_bold=True";
				if (chosenCharacter.NameIsItalic) italic = ", who_italic=True";
				if (chosenCharacter.TextColor.ToString() != "") what_color = ", what_color" + equalsQuote(chosenCharacter.TextColor.ToString().Remove(1, 2));
				if (chosenCharacter.TextIsBold) what_bold = ", what_bold=True";
				if (chosenCharacter.TextIsItalic) what_italic = ", what_italic=True";
				writer.WriteLine(define + chosenCharacter.Alias + character + quote(chosenCharacter.Content.ToString()) + icon + color + bold + italic + what_color + what_bold + what_italic + ")");
			}

			//end init
			//labels	

			for (int chosenLabelNumber = 1; chosenLabelNumber < tabControlStruct.Items.Count; chosenLabelNumber++)
			{
				writer.WriteLine(nextLine + label + (tabControlStruct.Items[chosenLabelNumber] as TabItem).Header + ':');
				XFrame previousFrame = new XFrame { };//предыдущий кадр для сравнения. Дабы не обновлять весь контент каждый кадр, как здесь, можно откидывать ненужные обновления после сравнения
				for (int chosenFrameNumber = 0; chosenFrameNumber < ((tabControlStruct.Items[chosenLabelNumber] as TabItem).Content as ListView).Items.Count; chosenFrameNumber++)
				{
					//все что касается конкретного кадра
					XFrame chosenFrame = ((tabControlStruct.Items[chosenLabelNumber] as TabItem).Content as ListView).Items[chosenFrameNumber] as XFrame;
					//background					
					if (chosenFrame.BackgroundImage != null)
					{
						string animationType = "";
						if (chosenFrame.AnimationInType != 0) animationType = " with " + animationInTypeComboBox.Items[chosenFrame.AnimationInType];
						if (previousFrame.BackgroundImage != chosenFrame.BackgroundImage)
							writer.WriteLine(tab + "scene " + chosenFrame.BackgroundImage.Alias + animationType);
					}
					else
					{//когда выбран корневой фрейм, смысла скрывать фон нет - до него фона не было, потому j>0
						string animationType = "";
						if (chosenFrame.AnimationOutType != 0) animationType = " with " + animationOutTypeComboBox.Items[chosenFrame.AnimationOutType];
						if (chosenFrameNumber > 0 && previousFrame.BackgroundImage != null)
							writer.WriteLine(tab + "hide " + previousFrame.BackgroundImage.Alias + animationType);
					}

					//images
					//случай, когда в данном фрейме есть картинки
					if (ImageInFrameProps.Any(img => img.Frame == chosenFrame))
					{//и в предыдущем тоже есть - самый сложный, но самый частый
						if (ImageInFrameProps.Any(img => img.Frame == previousFrame))
						{
							//сравниваем все картинки между собой
							var prevImages = ImageInFrameProps.Where(previmg => previmg.Frame == previousFrame);
							var chosImages = ImageInFrameProps.Where(chosimg => chosimg.Frame == chosenFrame);
							//если находим те, что есть в нынешнем фрейме, но в предыдущем их нет							
							foreach (ImageProperties property in prevImages.Where(previmg => (!chosImages.Any(chosenimg => previmg.Image == chosenimg.Image))))
								exportHideImage(writer, property);
							foreach (ImageProperties property in chosImages.Where(chosenimg => (!prevImages.Any(previmg => chosenimg.Image == previmg.Image))))
								exportShowImage(writer, property);
						}
						else
						{//если в предыдущем нету ничего, а в нынешнем есть
							foreach (ImageProperties property in ImageInFrameProps.Where(chosenimg => chosenimg.Frame == chosenFrame))
							{
								if (!ImageInFrameProps.Any(img => img.Image == property.Image && img.Frame == previousFrame))
									exportShowImage(writer, property);
							}
						}
					}
					else
					{
						//в нынешнем нет, а в предыдущем есть
						if (ImageInFrameProps.Any(img => img.Frame == previousFrame))
						{//hide картинок предыдущего фрейма
							foreach (ImageProperties property in ImageInFrameProps.Where(previmg => previmg.Frame == previousFrame))
							{
								if (!ImageInFrameProps.Any(img => img.Image == property.Image && img.Frame == currentFrame))
									exportHideImage(writer, property);
							}
						}
					}

					//audio
					if (chosenFrame.stopAudio) { writer.WriteLine(tab + "stop audio"); writer.WriteLine(tab + "stop music"); }
					if (AudioInFrameProps.Any(img => img.Frame == chosenFrame))
					{
						string fadein = "";
						string fadeout = "";
						string loop = "";
						foreach (AudioProperties property in AudioInFrameProps.Where(chosenaud => (chosenaud.Frame == chosenFrame)))
						{
							if (property.FadeIn != 0) fadein = " fadein " + property.FadeIn;
							if (property.FadeOut != 0) fadeout = " fadeout " + property.FadeOut;
							if (!property.Loop && property.Audio.Type == "music ") loop = " noloop";
							else if (property.Loop && !(property.Audio.Type == "music ")) loop = " loop";
							writer.WriteLine(tab + "play " + property.Audio.Type + property.Audio.Alias + fadein + fadeout + loop);
						}
					}

					//movie
					if (chosenFrame.Movie != null)
					{
						if (previousFrame.Movie != chosenFrame.Movie)
							writer.WriteLine(tab + "show " + chosenFrame.Movie.Alias);
					}
					else
					{
						if (chosenFrameNumber > 0 && previousFrame.BackgroundImage != null)
							writer.WriteLine(tab + "hide " + previousFrame.Movie.Alias);
					}

					//Character
					string character = "";
					if (chosenFrame.Character != characterListView.Items[0]) character = chosenFrame.Character.Alias + " ";
					writer.WriteLine(tab + character + quote(chosenFrame.Text));

					//menu
					if (chosenFrame.isMenu)
					{
						string optionLabel = "";
						writer.WriteLine(tab + "menu:");
						foreach (XMenuOption option in chosenFrame.MenuOptions)
						{
							writer.WriteLine(tab + tab + quote(option.Choice) + ':');
							if (option.MenuAction.SelectedItem != passAction) optionLabel = " " + (option.ActionLabel.SelectedItem as ComboBoxItem).Content.ToString();
							writer.WriteLine(tab + tab + tab + (option.MenuAction.SelectedItem as ComboBoxItem).Content + optionLabel);
						}
					}

					previousFrame = chosenFrame;
				}
				writer.WriteLine(tab + Return + nextLine);
			}

			writer.Close();


			//BackgroundWorker worker = new BackgroundWorker();
			//worker.DoWork += (o, ea) =>
			//{
			//	foreach (XImage image in backImageListView.Items)
			//	{
			//		writer.WriteLine("image " + image.Content.Substring(0, image.Content.LastIndexOf('.')) + "=\"" + image.Content + "\"");
			//		contentCollector(image.Path, projectFolder + imagesFolder + image.Content);
			//	}

			//	foreach (XImage image in imageListView.Items)
			//	{
			//		writer.WriteLine("image " + image.Content.Substring(0, image.Content.LastIndexOf('.')) + "=\"" + image.Content + "\"");
			//		contentCollector(image.Path, projectFolder + imagesFolder + image.Content);
			//	}
			//	writer.Close();

			//	//use the Dispatcher to delegate the listOfStrings collection back to the UI
			//	//Dispatcher.Invoke((Action)(() => _listBox.ItemsSource = listOfString));
			//};
			//worker.RunWorkerCompleted += (o, ea) =>
			//{
			//	busyIndicator.IsBusy = false;
			//};
			//busyIndicator.IsBusy = true;
			//worker.RunWorkerAsync();	
		}

		private XFrame createFrame(bool root)
		{
			XFrame frame;
			if (!root) { frame = new XFrame() { Content = "Frame " + framecount + " []", ContextMenu = cmFrame, Character = characterListView.Items[0] as XCharacter,AnimationInType=0, AnimationOutType = 0 }; }
			else { frame = new XFrame() { Content = "Root frame " + framecount + " []", ContextMenu = cmRootframe, Character = characterListView.Items[0] as XCharacter, AnimationInType = 0, AnimationOutType = 0 }; }
			frame.Selected += selectFrame_Click;
			//frame.MouseDoubleClick += namechange_DoubleClick;
			framecount++;
			return frame;
		}
		private void preSaveCurrentFrame()
		{
			//перед выбором фрейма нужно сохранить содержимое нынешнего выбранного фрейма
			currentFrame.Content = currentFrame.Content.ToString().Substring(0, currentFrame.Content.ToString().IndexOf('['))+'['+textBox.Text+']';	
		}

		private void selectFrame_Click(object sender, RoutedEventArgs e)
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
				convertFrameMenu.Header = framemenu;
				menuOptionsVisualList.ItemsSource = null;
			}
			else
			{
				menuStack.Visibility = Visibility.Visible;
				convertFrameMenu.Header = menuframe;
				menuOptionsVisualList.ItemsSource = currentFrame.MenuOptions; 				
			}

			if (currentFrame.BackgroundImage != null)
			{
				imageBackground.Source = imageShow(currentFrame.BackgroundImage.Path);
				(backImageListView.Items[backImageListView.Items.IndexOf(currentFrame.BackgroundImage)] as XImage).IsChecked = true;
			}
			else imageBackground.Source = null;

			characterLabel.Content = currentFrame.Character.Content;

			foreach (ImageProperties imageprops in ImageInFrameProps.Where(frame => frame.Frame == currentFrame))
			{ 
				imageprops.Image.IsChecked = true;
				imageprops.Image.Background = currentFrameResourceColor;
			}
			foreach (AudioProperties audprops in AudioInFrameProps.Where(frame => frame.Frame == currentFrame))
			{
				audprops.Audio.IsChecked = true;
				audprops.Audio.Background = currentFrameResourceColor;
			}
			
			if (currentFrame.stopAudio)	stopAudio.IsChecked = true;
			else stopAudio.IsChecked = false;

		addorselect = true;
		}
		private void addNextFrame_Click(object sender, RoutedEventArgs e)
		{
			preSaveCurrentFrame();
			XFrame frame = createFrame(false);
			ListView selectedList = getSelectedList();

			if (sender == addMenu)
			{
				frame.isMenu = true;
				frame.MenuOptions = new ObservableCollection<XMenuOption> { createMenuOption(true) };
			}

			selectedList.Items.Insert(selectedList.Items.IndexOf(selectedList.SelectedItem)+1, frame);
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
			duplicate.BackgroundImage = currentFrame.BackgroundImage;
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
		private void PrevNext_Click(object sender, RoutedEventArgs e)
		{
			preSaveCurrentFrame();
			int index = getSelectedList().Items.IndexOf(getSelectedFrame());
			if (sender == prevFrame && index - 1 >= 0) (getSelectedList().Items[index - 1] as XFrame).IsSelected = true;
			else if (sender == nextFrame && index + 1 < getSelectedList().Items.Count) (getSelectedList().Items[index + 1] as XFrame).IsSelected = true;
		}
		private void textBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			currentFrame.Text = textBox.Text;
		}
		private XFrame getSelectedFrame() { return getSelectedList().SelectedItem as XFrame; }
		private ListView getSelectedList() { return tabControlStruct.SelectedContent as ListView; }

		//LABELS SECTION
		private void addLabel_Click(object sender, RoutedEventArgs e)
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

		private void deleteLabel_Click(object sender, RoutedEventArgs e)
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
	}
}