using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

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

			StreamReader readerlabels = new StreamReader(fs);

			while (!readerlabels.EndOfStream)
			{
				string label = readerlabels.ReadLine().TrimStart(' ');
				if (label.StartsWith("label"))
				{ ListView newLabel = createLabel(label.Substring(6, label.Length - 7)); }
			}
			readerlabels.Dispose(); fs.Dispose();

			fs = new FileStream(projectFolder + "script.rpy", FileMode.Open);
			StreamReader reader = new StreamReader(fs);
			//контент ОЧЕНЬ неоднозначен, потому для его правильного распознания нужен код и в init, и в метках
			string singleLine;
			while (!reader.EndOfStream)
			{
				singleLine = reader.ReadLine().TrimStart(' ');

				if (singleLine.StartsWith("define"))
				{
					if (singleLine.Contains("Character"))
					{
						XCharacter character = loadCharacter(singleLine);
						characterList.Add(character);
					}
					else if (singleLine.Contains("audio."))
					{
						XAudio audio = new XAudio();
						audio.loadAudio(singleLine, projectFolder + game);
						audioMouseActions(audio);
						musicListView.Items.Add(audio);
					}
				}
				else if (singleLine.StartsWith("image"))
				{
					if (!singleLine.Contains("Movie("))
					{
						XImage image = new XImage();
						image.loadImage(singleLine, projectFolder + "images\\");
						imageMouseActions(image);
						if (!singleLine.StartsWith("image side")) backImageListView.Items.Add(image);
						else
						{
							image.Checkbox.Visibility = Visibility.Hidden;
							sideListView.Items.Add(image);
						}
					}
					else
					{
						XMovie movie = new XMovie();
						movie.loadMovie(singleLine, projectFolder + "movies\\");
						movieMouseActions(movie);
						movieListView.Items.Add(movie);
					}
				}
				else if (singleLine.StartsWith("label"))
				{
					XLabel selectedLabel = tabControlStruct.Items.OfType<XLabel>().First(label => label.Text == singleLine.Substring(6, singleLine.Length - 7));
					selectedLabel.IsSelected = true;
					XFrame frame;
					bool buildmenu = false;
					bool firstframe = true;

					singleLine = reader.ReadLine().Trim(' ');

					while (singleLine != "return" && !reader.EndOfStream)
					{
                        if (singleLine!="")
                        {
                            frame = createFrame();
                            currentFrame = frame;
                            (selectedLabel.Content as ListView).Items.Add(frame);
                            if (firstframe) { setPreviousFrames(); firstframe = false; }
                            else previousFrames.Add(frame);

                            List<string> framebody = new List<string> { };

                            while (singleLine != "return" && !reader.EndOfStream)
                            {
                                if (singleLine.StartsWith("menu"))
                                {
                                    buildmenu = true;
                                    frame.MenuOptions = new ObservableCollection<XMenuOption> { };
                                }
                                else if (Regex.IsMatch(singleLine, @"[\S\s]*""[\S\s]*"":$"))
                                {
                                    XMenuOption newmenuoption = createMenuOption(frame.MenuOptions.Count == 0);
                                    newmenuoption.Choice = value(singleLine);

                                    singleLine = reader.ReadLine().Trim(' ');
                                    if (singleLine.StartsWith("jump"))
                                    {
                                        newmenuoption.MenuAction.SelectedItem = jumpAction;
                                        newmenuoption.ActionLabel.SelectedIndex = menuLabelList.IndexOf(menuLabelList.First(label => label.Content.ToString() == singleLine.Substring(singleLine.IndexOf(' ') + 1)));
                                        (tabControlStruct.Items[newmenuoption.ActionLabel.SelectedIndex] as XLabel).MenuChoice = frame;
                                    }
                                    else if (singleLine.StartsWith("call"))
                                    {
                                        newmenuoption.MenuAction.SelectedItem = callAction;
                                        newmenuoption.ActionLabel.SelectedIndex = menuLabelList.IndexOf(menuLabelList.First(label => label.Content.ToString() == singleLine.Substring(singleLine.IndexOf(' ') + 1)));
                                        (tabControlStruct.Items[newmenuoption.ActionLabel.SelectedIndex] as XLabel).MenuChoice = frame;
                                    }
                                    else newmenuoption.MenuAction.SelectedItem = passAction;
                                    frame.MenuOptions.Add(newmenuoption);
                                }
                                else if (Regex.IsMatch(singleLine, @"[\S\s]*""[\S\s]*""$"))
                                {
                                    if (buildmenu && frame.MenuOptions.Count != 0) break;
                                    loadText(frame, singleLine);
                                    if (!buildmenu) break;
                                }
                                else
                                {
                                    if (!buildmenu) framebody.Add(singleLine);
                                    else break;
                                }
                                singleLine = reader.ReadLine().Trim(' ');
                            }

                            for (int line = 0; line < framebody.Count; line++)
                            {
                                if (framebody[line].StartsWith("pause"))
                                    frame.PauseFrame = true;
                                else if (framebody[line].StartsWith("scene"))
                                {
                                    string[] all = framebody[line].Split(' ');
                                    XImage selectedImage = backImageListView.Items.OfType<XImage>().First(item => item.Alias == all[1]);
                                    ImageBackProperties BackProp = new ImageBackProperties() { Frame = frame, Image = selectedImage };
                                    if (all.Length > 2) if (all[2] == "with") BackProp.AnimationInType = (byte)animationInTypeComboBox.Items.IndexOf(animationInTypeComboBox.Items.OfType<string>().First(item => item == all[3]));
                                    //секция поиска
                                    if (BackInFrameProps.Any(prop => previousFrames.Contains(prop.Frame) && !previousFrames.Contains(prop.StopFrame)))
                                    {
                                        ImageBackProperties previous = BackInFrameProps.Last(prop => previousFrames.Contains(prop.Frame) && !previousFrames.Contains(prop.StopFrame));
                                        //а вдруг меню, а мы неподготовлены? надо расставить все нужные метки
                                        if (previous.Frame.MenuOptions == null)
                                            previous.StopFrame = frame;
                                        else
                                        {
                                            if (previous.StopFrames == null)
                                                previous.StopFrames = new List<XFrame> { };
                                            previous.StopFrames.Add(frame);
                                        };
                                    }//усьо
                                    BackInFrameProps.Add(BackProp);
                                }
                                else if (framebody[line].StartsWith("show"))
                                {
                                    string[] all = framebody[line].Split(' ');
                                    XImage selectedImage;
                                    if (backImageListView.Items.OfType<XImage>().Any(item => item.Alias == all[1]))
                                    {
                                        selectedImage = backImageListView.Items.OfType<XImage>().First(item => item.Alias == all[1]);
                                        backImageListView.Items.Remove(selectedImage);
                                        imageListView.Items.Add(selectedImage);
                                    }
                                    else selectedImage = imageListView.Items.OfType<XImage>().First(item => item.Alias == all[1]);

                                    ImageCharProperties props = new ImageCharProperties() { Frame = frame, Image = selectedImage, Displayable = newDisplayable() };
                                    props.Displayable.Source = imageShow(selectedImage.Path);

                                    for (int i = 2; i < all.Length; i++)
                                    {
                                        if (all[i] == "with") props.AnimationInType = (byte)animationInTypeComboBox.Items.IndexOf(animationInTypeComboBox.Items.OfType<string>().First(item => item == all[i + 1]));
                                        else if (all[i] == "at") props.Align = (byte)alignComboBox.Items.IndexOf(alignComboBox.Items.OfType<string>().First(item => item == all[i + 1]));
                                    }
                                    ImageInFrameProps.Add(props);
                                }
                                else if (framebody[line].StartsWith("hide"))
                                {
                                    string[] all = framebody[line].Split(' ');
                                    if (backImageListView.Items.OfType<XImage>().Any(prop => prop.Alias == all[1]))
                                    {
                                        ImageBackProperties previous = BackInFrameProps.Last(prop => previousFrames.Contains(prop.Frame) && prop.Image.Alias == all[1]);
                                        if (all.Length > 2) if (all[2] == "with")
                                                previous.AnimationOutType = (byte)animationOutTypeComboBox.Items.IndexOf(animationOutTypeComboBox.Items.OfType<string>().First(item => item == all[3]));
                                        if (previous.Frame.MenuOptions == null)
                                            previous.StopFrame = frame;
                                        else
                                        {
                                            if (previous.StopFrames == null)
                                                previous.StopFrames = new List<XFrame> { };
                                            previous.StopFrames.Add(frame);
                                        };
                                    }
                                    else
                                    {
                                        ImageCharProperties previous = ImageInFrameProps.Last(prop => previousFrames.Contains(prop.Frame) && prop.Image.Alias == all[1]);
                                        if (all.Length > 2) if (all[2] == "with")
                                                previous.AnimationOutType = (byte)animationOutTypeComboBox.Items.IndexOf(animationOutTypeComboBox.Items.OfType<string>().First(item => item == all[3]));
                                        if (previous.Frame.MenuOptions == null)
                                            previous.StopFrame = frame;
                                        else
                                        {
                                            if (previous.StopFrames == null)
                                                previous.StopFrames = new List<XFrame> { };
                                            previous.StopFrames.Add(frame);
                                        };
                                    }
                                }
                                else if (framebody[line].StartsWith("stop"))
                                {
                                    string type = framebody[line].Substring(5) + " ";
                                    AudioProperties previous = AudioInFrameProps.Last(prop => previousFrames.Contains(prop.Frame) && prop.Audio.Type == type);
                                    if (previous.Frame.MenuOptions == null)
                                        previous.StopFrame = frame;
                                    else
                                    {
                                        if (previous.StopFrames == null)
                                            previous.StopFrames = new List<XFrame> { };
                                        previous.StopFrames.Add(frame);
                                    };
                                }
                                else if (framebody[line].StartsWith("play"))
                                {
                                    string[] all = framebody[line].Split(' ');
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
                                }
                            }

                            if (singleLine != "return")
                            {
                                if (buildmenu) buildmenu = false;
                                else { frame.IsSelected = true; singleLine = reader.ReadLine().TrimStart(' '); }
                            } 
                        }
					}
				}
			}
			previousFrames.Clear();
			fs.Close();
		}

		/// <summary>
		/// 
		/// </summary>
		private void saveScript()
		{
			projectExpander.IsExpanded = false;
			createDirectories();
			FileStream fs = new FileStream(projectFolder + "script.rpy", FileMode.Create);
			StreamWriter writer = new StreamWriter(fs);
			writer.WriteLine(scriptstart + nextLine);
			//init
			writer.WriteLine(backgroundImages);
			foreach (XImage image in backImageListView.Items) writer.WriteLine("image " + image.Alias + eQuote(image.Header));

			writer.WriteLine(characterImages);
			foreach (XImage image in imageListView.Items) writer.WriteLine("image " + image.Alias + eQuote(image.Header));

			writer.WriteLine(musicAudio);
			foreach (XAudio audio in musicListView.Items) writer.WriteLine(define + "audio." + audio.Alias + eQuote(musicFolder + audio.Header));

			writer.WriteLine(soundsAudio);
			foreach (XAudio audio in soundListView.Items) writer.WriteLine(define + "audio." + audio.Alias + eQuote(soundsFolder + audio.Header));

			writer.WriteLine(voiceAudio);
			foreach (XAudio audio in voiceListView.Items) writer.WriteLine(define + "audio." + audio.Alias + eQuote(voicesFolder + audio.Header));

			writer.WriteLine(Movies);
			foreach (XMovie movie in movieListView.Items)
			{
				string mask = "";
				if (movie.MaskPath != null) mask = ", mask" + eQuote(moviesFolder + movie.MaskPath);
				writer.WriteLine("image " + movie.Alias + "=Movie(play" + eQuote(moviesFolder + movie.Header) + mask + ")");
			}

			writer.WriteLine(Characters);
			for (int i = 4; i < characterList.Count; i++)
			{
				XCharacter chosenCharacter = characterList[i] as XCharacter;
				string icon = "", color = "", nvl="", bold = "", italic = "", what_color = "", what_bold = "", what_italic = "";
				if (chosenCharacter.IsNvl) nvl = ", kind=nvl";
                if (chosenCharacter.Icon != null) { icon = ", image" + eQuote(chosenCharacter.Alias); writer.WriteLine("image side " + chosenCharacter.Alias + eQuote(chosenCharacter.Content.ToString())); }
				if (chosenCharacter.NameColor.ToString() != "") color = ", color" + eQuote(chosenCharacter.NameColor.ToString().Remove(1, 2));                
                if (chosenCharacter.NameIsBold) bold = ", who_bold=True";
				if (chosenCharacter.NameIsItalic) italic = ", who_italic=True";
				if (chosenCharacter.TextColor.ToString() != "") what_color = ", what_color" + eQuote(chosenCharacter.TextColor.ToString().Remove(1, 2));
				if (chosenCharacter.TextIsBold) what_bold = ", what_bold=True";
				if (chosenCharacter.TextIsItalic) what_italic = ", what_italic=True";
				writer.WriteLine(define + chosenCharacter.Alias + character + quote(chosenCharacter.Content.ToString()) + nvl + icon + color + bold + italic + what_color + what_bold + what_italic + ")");
			}

			//end init
			//labels	

			for (int chosenLabelNumber = 0; chosenLabelNumber < tabControlStruct.Items.Count-1; chosenLabelNumber++)
			{
				writer.WriteLine(nextLine + label + (tabControlStruct.Items[chosenLabelNumber] as XLabel).Text + ':');

				for (int chosenFrameNumber = 0; chosenFrameNumber < ((tabControlStruct.Items[chosenLabelNumber] as XLabel).Content as ListView).Items.Count; chosenFrameNumber++)
				{
					//все что касается конкретного кадра
					XFrame chosenFrame = ((tabControlStruct.Items[chosenLabelNumber] as XLabel).Content as ListView).Items[chosenFrameNumber] as XFrame;

                    if (chosenFrame.PauseFrame) writer.WriteLine(tab + "pause");
                    else
                    {
                        //background	
                        ImageBackProperties BackProp;

                        if (BackInFrameProps.Any(prop => prop.StopFrame == chosenFrame))
                        {
                            BackProp = BackInFrameProps.First(prop => prop.StopFrame == chosenFrame);
                            string animationType = "";
                            if (BackProp.AnimationOutType != 0) animationType = " with " + animationOutTypeComboBox.Items[BackProp.AnimationOutType];
                            writer.WriteLine(tab + "hide " + BackProp.Image.Alias + animationType);
                        }
                        if (BackInFrameProps.Any(prop => prop.Frame == chosenFrame))
                        {
                            BackProp = BackInFrameProps.First(prop => prop.Frame == chosenFrame);
                            string animationType = "";
                            if (BackProp.AnimationInType != 0) animationType = " with " + animationInTypeComboBox.Items[BackProp.AnimationInType];
                            writer.WriteLine(tab + "scene " + BackProp.Image.Alias + animationType);
                        }

                        //images
                        if (ImageInFrameProps.Any(prop => prop.StopFrame == chosenFrame))
                        {
                            foreach (ImageCharProperties property in ImageInFrameProps.Where(prop => prop.StopFrame == chosenFrame))
                            {
                                string animationType = "";
                                if (property.AnimationOutType != 0) animationType = " with " + animationOutTypeComboBox.Items[property.AnimationOutType];
                                writer.WriteLine(tab + "hide " + property.Image.Alias + animationType);
                            }
                        }
                        if (ImageInFrameProps.Any(prop => prop.Frame == chosenFrame))
                        {
                            foreach (ImageCharProperties property in ImageInFrameProps.Where(prop => prop.Frame == chosenFrame))
                            {
                                string align = "";
                                string animationType = "";
                                if (property.Align != 0) align = " at " + alignComboBox.Items[property.Align];
                                if (property.AnimationInType != 0) animationType = " with " + animationInTypeComboBox.Items[property.AnimationInType];
                                writer.WriteLine(tab + "show " + property.Image.Alias + align + animationType);
                            }
                        }

                        //audio
                        if (AudioInFrameProps.Any(mus => mus.StopFrame == chosenFrame))
                        {
                            foreach (AudioProperties property in AudioInFrameProps.Where(prop => (prop.StopFrame == chosenFrame)))
                            {
                                if (property.Audio.Type == "music ") writer.WriteLine(tab + "stop music");
                                else if (property.Audio.Type == "sound ") writer.WriteLine(tab + "stop sound");
                                else writer.WriteLine(tab + "stop voice");
                            }
                        }
                        if (AudioInFrameProps.Any(mus => mus.Frame == chosenFrame))
                        {
                            string fadein = "";
                            string fadeout = "";
                            string loop = "";
                            foreach (AudioProperties property in AudioInFrameProps.Where(prop => (prop.Frame == chosenFrame)))
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
                            writer.WriteLine(tab + "$ renpy.movie_cutscene(" + quote(chosenFrame.Movie.Content.ToString()) + ")");
                        }

                        //menu
                        if (chosenFrame.MenuOptions != null)
                        {
                            writer.WriteLine(tab + "menu:");
                            if (chosenFrame.Text != "") writer.WriteLine(tab + tab + quote(chosenFrame.Text));
                            foreach (XMenuOption option in chosenFrame.MenuOptions)
                            {
                                string optionLabel = "";
                                writer.WriteLine(tab + tab + quote(option.Choice) + ':');
                                if (option.MenuAction.SelectedItem != passAction) optionLabel = " " + (option.ActionLabel.SelectedItem as ComboBoxItem).Content.ToString();
                                writer.WriteLine(tab + tab + tab + (option.MenuAction.SelectedItem as ComboBoxItem).Content + optionLabel);
                            }
                        }
                        else
                        {
                            //Character and text
                            string character = "";
                            if (chosenFrame.Character != charNone) character = chosenFrame.Character.Alias + " ";
                            writer.WriteLine(tab + character + quote(chosenFrame.Text));
                        }
                    }
				}
				writer.WriteLine(tab + Return);
			}
			writer.Close();
			fs.Close();
		}
	}
}