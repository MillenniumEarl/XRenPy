using System;
using System.Windows;
using System.IO;
using Ookii.Dialogs.Wpf;
using System.Windows.Controls;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
		}

		private void clearAll()
		{
			framecount = 0;
			projectExpander.IsExpanded = false;
			for(int i=2; i<imagegrid.Children.IndexOf(imageBorder);i++) imagegrid.Children.RemoveAt(i);
			for (int i = 1; i <=tabControlStruct.Items.Count; i++) tabControlStruct.Items.RemoveAt(i);
			foreach (TabItem tab in tabControlResources.Items) (tab.Content as ListView).Items.Clear();
			ImageInFrameProps.Clear(); AudioInFrameProps.Clear();
		}
		public string equalsQuote(string content) { return "=\"" + content + "\""; }
		public string quote(string content) { return "\"" + content + "\""; }
		private void LoadProject_Click(object sender, RoutedEventArgs e)
		{
			VistaFolderBrowserDialog selectFolder = new VistaFolderBrowserDialog();

			if (selectFolder.ShowDialog() == true)
				try
				{
					if (File.Exists(selectFolder.SelectedPath.ToString() + script)||(!File.Exists(selectFolder.SelectedPath.ToString() + options)))
					{						
						projectFolder = selectFolder.SelectedPath.ToString() + game;
						loadProject();
					}

					else MessageBox.Show("Not a project folder!", "Incorrect folder", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				catch (Exception) { MessageBox.Show("Please choose the folder!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);	}			
		}

		private void SaveProject_Click(object sender, RoutedEventArgs e)
		{
			saveProject();			
		}
		private void SaveProjectAS_Click(object sender, RoutedEventArgs e)
		{
			VistaFolderBrowserDialog selectFolder = new VistaFolderBrowserDialog();

			if (selectFolder.ShowDialog() == true)
				
				{ string tempFolder = projectFolder;
				projectFolder = selectFolder.SelectedPath.ToString()+game;
					if (!(File.Exists(selectFolder.SelectedPath.ToString() + script) || File.Exists(selectFolder.SelectedPath.ToString() + options)))
					{							
						saveProject();
					}
					else
					{
						MessageBoxResult result = MessageBox.Show("Existing project folder! Replace?", "Incorrect folder", MessageBoxButton.YesNoCancel, MessageBoxImage.Error);
						if (result == MessageBoxResult.Yes)
						{
						try { Directory.Delete(projectFolder, true); saveProject();}
						catch (Exception) {
							MessageBox.Show("Impossible to delete the folder!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
							projectFolder = tempFolder;
								}							
						}
					}
				}
		}

		private void loadProject()
		{
			clearAll();
			projectExpander.IsExpanded = false;
			//код для загрузки проекта
			FileStream fs = new FileStream(projectFolder+ "script.rpy", FileMode.Open);
			StreamReader reader = new StreamReader(fs);

			//контент ОЧЕНЬ неоднозначен, потому для его правильного распознания нужен код и в init, и в метках
			//с очень большой вероятностью контент будет использоваться в конкретных местах либо не более одного раза за игру, либо 

			string singleLine;
			while (!reader.EndOfStream)
			{
				singleLine = reader.ReadLine().TrimStart(' ');
				for (int compare = 0; compare < comparer.Length; compare++)
				{
					if (singleLine.StartsWith(comparer[compare]))
						switch (comparer[compare])
						{//INIT
							case "define":
								if (singleLine.Contains("Character"))
								{
									XCharacter character = new XCharacter();
									character.loadCharacter(singleLine);
									characterListView.Items.Add(character);
								}
								else if (singleLine.Contains("audio."))
								{
									XAudio audio = new XAudio();
									audio.loadAudio(singleLine, projectFolder+game);
									audioMouseActions(audio);
									musicListView.Items.Add(audio);
								}
								break;
							case "image":
								if (!singleLine.Contains("Movie"))
								{
									XImage image = new XImage();
									image.loadImage(singleLine, projectFolder+"images\\");
									imageMouseActions(image);
									backImageListView.Items.Add(image);
								}
								else
								{
									XMovie movie = new XMovie();
									movie.loadMovie(singleLine, projectFolder+"images\\");
									movieMouseActions(movie);
									movieListView.Items.Add(movie);
								}
								break;
							case "label":
								{
									ListView newLabel = createLabel(singleLine.Substring(6, singleLine.Length - 7));
									ImageProperties Background= new ImageProperties();
									XFrame frame;
									bool root = true;
									string readingLine = reader.ReadLine().TrimStart(' ');									
									while (readingLine != "return")
									{
										frame = createFrame(root);
										List<string> framebody = new List<string> {readingLine};

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
															frame.BackgroundImage = backImageListView.Items.OfType<XImage>().Where(item => item.Alias == all[1]).Single();
															if (all.Length > 2) if (all[2] == "with") frame.AnimationInType = (byte)animationInTypeComboBox.Items.IndexOf(animationInTypeComboBox.Items.OfType<string>().Where(item => item == all[3]).Single());
															Background = frame.BackgroundImageProps;
															break;
														}
													case "hide":
														{//only background currently
															string[] all = line.Split(' ');
															if (backImageListView.Items.OfType<XImage>().Any(item => item.Alias == all[1]))
															{
																frame.BackgroundImage = null;
																if (all.Length > 2) if (all[2] == "with")
																		Background.AnimationOutType = (byte)animationOutTypeComboBox.Items.IndexOf(animationOutTypeComboBox.Items.OfType<string>().Where(item => item == all[3]).Single());
															}
															//по нынешней логике, надо найти первый элемент с этой же картинкой, остальные просто игнорируются
															else
																if (all.Length > 2) if (all[2] == "with")
																	ImageInFrameProps.Where(item => item.Image.Alias == all[1]).Single().AnimationOutType= (byte)animationOutTypeComboBox.Items.IndexOf(animationOutTypeComboBox.Items.OfType<string>().Where(item => item == all[3]).Single());
															break;
														}
													case "show":
														{
															string[] all = line.Split(' ');
															XImage image = backImageListView.Items.OfType<XImage>().Where(item => item.Alias == all[1]).Single();
															backImageListView.Items.Remove(image);
															imageListView.Items.Add(image);
															ImageProperties props = new ImageProperties() { Frame = frame, Image = image };

															for (int i = 2; i < all.Length; i++)
															{
																if (all[i] == "with") props.AnimationInType = (byte)animationInTypeComboBox.Items.IndexOf(animationInTypeComboBox.Items.OfType<string>().Where(item => item == all[i + 1]).Single());
																else if (all[i] == "at") props.Align = (byte)alignComboBox.Items.IndexOf(alignComboBox.Items.OfType<string>().Where(item => item == all[i + 1]).Single());
															}
															ImageInFrameProps.Add(props);
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
															XAudio audio = musicListView.Items.OfType<XAudio>().Where(item => item.Alias == all[2]).Single();
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
													frame.Character = characterListView.Items.OfType<XCharacter>().Where(item => item.Alias == line.Substring(0, line.IndexOf(' '))).Single();
													textBox.Text = line.Substring(line.IndexOf(' ')).Replace("\"", " ");
												}
											}
										newLabel.Items.Add(frame);
										//previousSettings = frame;
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


		private void saveProject()
		{
			projectExpander.IsExpanded = false;
			createDirectories();
			FileStream fs = new FileStream(projectFolder+game+"script.rpy", FileMode.Create);
			StreamWriter writer = new StreamWriter(fs);
			//код для сохранения проекта
			writer.WriteLine(scriptstart + nextLine);
			//options.rpy
			//init
			writer.WriteLine(backgroundImages);
			foreach (XImage image in backImageListView.Items)
			{
				writer.WriteLine("image " + image.Alias + equalsQuote(image.Content));
				contentCollector(image.Path, projectFolder + imagesFolder + image.Content);
			}

			writer.WriteLine(characterImages);
			foreach (XImage image in imageListView.Items)
			{
				writer.WriteLine("image " + image.Alias + equalsQuote(image.Content));
				contentCollector(image.Path, projectFolder + imagesFolder + image.Content);
			}

			writer.WriteLine(musicAudio);
			foreach (XAudio audio in musicListView.Items)
			{
				writer.WriteLine(define+"audio." + audio.Alias + equalsQuote(musicFolder + audio.Content));
				contentCollector(audio.Path, projectFolder + musicFolder + audio.Content);
			}

			writer.WriteLine(soundsAudio);
			foreach (XAudio audio in soundListView.Items)
			{
				writer.WriteLine(define + "audio." + audio.Alias + equalsQuote(soundsFolder + audio.Content));
				contentCollector(audio.Path, projectFolder + soundsFolder + audio.Content);
			}

			writer.WriteLine(voiceAudio);
			foreach (XAudio audio in voiceListView.Items)
			{
				writer.WriteLine(define + "audio." + audio.Alias + equalsQuote(voicesFolder +audio.Content));
				contentCollector(audio.Path, projectFolder + voicesFolder + audio.Content);
			}

			writer.WriteLine(Movies);
			foreach (XMovie movie in movieListView.Items)
			{
				string mask = "";
				if (movie.MaskPath != null) mask = ", mask"+equalsQuote(moviesFolder +movie.MaskPath);
				writer.WriteLine("image " + movie.Alias + "=Movie(play"+equalsQuote(moviesFolder + movie.Content) + mask+")");
				contentCollector(movie.Path, projectFolder + moviesFolder + movie.Content);
			}

			writer.WriteLine(Characters);
			for (int i = 3; i < characterListView.Items.Count; i++)
			{
				XCharacter chosenCharacter = characterListView.Items[i] as XCharacter;
				string color = "", bold = "", italic = "", what_color = "", what_bold = "", what_italic = "";
				if (chosenCharacter.NameColor.ToString() != "") color = ", color" + equalsQuote(chosenCharacter.NameColor.ToString().Remove(1,2));
				if (chosenCharacter.NameIsBold) bold = ", who_bold=True";
				if (chosenCharacter.NameIsItalic) italic = ", who_italic=True";
				if (chosenCharacter.TextColor.ToString() != "") what_color = ", what_color" + equalsQuote(chosenCharacter.TextColor.ToString().Remove(1, 2));
				if (chosenCharacter.TextIsBold) what_bold = ", what_bold=True";
				if (chosenCharacter.TextIsItalic) what_italic = ", what_italic=True";
				writer.WriteLine(define + chosenCharacter.Alias + character + quote(chosenCharacter.Content.ToString()) + color + bold + italic + what_color + what_bold + what_italic + ")");
			}

			//end init
			//labels	

			for (int chosenLabelNumber = 1; chosenLabelNumber < tabControlStruct.Items.Count; chosenLabelNumber++)
			{
				writer.WriteLine(nextLine+label+(tabControlStruct.Items[chosenLabelNumber] as TabItem).Header +':');
				XFrame previousFrame = new XFrame { };//предыдущий кадр для сравнения. Дабы не обновлять весь контент каждый кадр, как здесь, можно откидывать ненужные обновления после сравнения
				for (int chosenFrameNumber = 0; chosenFrameNumber < ((tabControlStruct.Items[chosenLabelNumber] as TabItem).Content as ListView).Items.Count; chosenFrameNumber++)
				{
					//все что касается конкретного кадра
					XFrame chosenFrame = ((tabControlStruct.Items[chosenLabelNumber] as TabItem).Content as ListView).Items[chosenFrameNumber] as XFrame;
					//background					
					if (chosenFrame.BackgroundImage != null)
					{
						string animationType = "";
						if (chosenFrame.AnimationInType!=0) animationType = " with " + animationInTypeComboBox.Items[chosenFrame.AnimationInType];
						if (previousFrame.BackgroundImage != chosenFrame.BackgroundImage)
							writer.WriteLine(tab + "scene " + chosenFrame.BackgroundImage.Alias + animationType);
					}
					else
					{//когда выбран корневой фрейм, смысла скрывать фон нет - до него фона не было, потому j>0
						string animationType = "";
						if (chosenFrame.AnimationOutType != 0) animationType = " with " + animationOutTypeComboBox.Items[chosenFrame.AnimationOutType];
						if (chosenFrameNumber > 0 && previousFrame.BackgroundImage!=null)
							writer.WriteLine(tab + "hide " + previousFrame.BackgroundImage.Alias+ animationType);
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
						string fadein="";
						string fadeout="";
						string loop="";
						foreach (AudioProperties property in AudioInFrameProps.Where(chosenaud => (chosenaud.Frame == chosenFrame)))
						{	if (property.FadeIn != 0) fadein = " fadein " + property.FadeIn;
							if (property.FadeOut != 0) fadeout = " fadeout " + property.FadeOut;
							if (!property.Loop && property.Audio.Type == "music ") loop = " noloop";
							else if (property.Loop && !(property.Audio.Type == "music ")) loop = " loop";
							writer.WriteLine(tab + "play " + property.Audio.Type + property.Audio.Alias+fadein+fadeout+loop);
						}
					}

					//movie

					//Character
					string character = "";
					if (chosenFrame.Character != characterListView.Items[0]) character = chosenFrame.Character.Alias+" ";
					writer.WriteLine(tab + character + quote(chosenFrame.Text));
					
					//menu
					if(chosenFrame.isMenu)
					{
						string optionLabel = "";
						writer.WriteLine(tab + "menu:");
						foreach (XMenuOption option in chosenFrame.MenuOptions)
						{	
							writer.WriteLine(tab + tab + quote(option.Choice)+':');
							if (option.MenuAction.SelectedItem != passAction) optionLabel = " "+(option.ActionLabel.SelectedItem as ComboBoxItem).Content.ToString();
							writer.WriteLine(tab + tab + tab + (option.MenuAction.SelectedItem as ComboBoxItem).Content+optionLabel);
						}
					}

					previousFrame = chosenFrame;
				}
				writer.WriteLine(tab + Return+nextLine);
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
	}
}

