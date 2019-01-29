using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace X_Ren_Py
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		static string game = @"/game/";
		string script = game + @"script.rpy";
		string options = game + @"options.rpy";
		string screens = game + @"screens.rpy";
		string gui = game + @"gui.rpy";

		string projectFolder = Environment.CurrentDirectory + @"/temp" + game;
		string imagesFolder = @"images/";
		string musicFolder = @"music/";
		string soundsFolder = @"sounds/";
		string voicesFolder = @"voices/";
		string moviesFolder = @"movies/";
		string guiFolder = @"gui/";

		static string tab = "    ";
		static string nextLine = "\r\n";

		//строки для script.rpy         
		string nextBlock = ":" + nextLine + tab;
		string nextOption = nextLine + tab + "\"";

		string define = "define ";
		string character = "= Character(u";
		string label = "label ";
		string scriptstart = "#Zero Chaotic XRen'Py";
		string backgroundImages = "#Background images";
		string characterImages = "#Character and other images";
		string musicAudio = "#Music";
		string soundsAudio = "#Sounds";
		string voiceAudio = "#Voice";
		string Movies = "#Movies";
		string Characters = "#Characters";
		string Return = "return";

		//строки для options.rpy
		//"define config.window";
		//"define config.save_directory";

		//строка-компаратор
		string[] comparerScript = { "define", "image", "label" };

		//ресурсы
		string imageextensions = "Image files (*.bmp, *.jpg, *.png, *.webp)|*.bmp;*.jpg;*.png;*.webp";
		string audioextensions = "Audio files (*.wav, *.ogg, *.mp3, *.opus)|*.wav;*.ogg;*.mp3;*.opus";
		string vidextensions = "Video files (*.wmv,*.webm, *.mkv, *.avi, *.ogv)|*.wmv;*.webm;*.mkv;*.avi;*.ogv";
		SolidColorBrush currentFrameResourceColor = new SolidColorBrush(Color.FromArgb(127, 0, 127, 0));
		SolidColorBrush unusedResourceColor = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

		//контекстные меню
		MenuItem addFrame, addRootFrame, convertFrameMenu, deleteFrame, addMenu, 
			addImage, deleteImage, reloadImage, addAudio, deleteAudio, reloadAudio, addMovie, deleteMovie, reloadMovie, addIcon, reloadIcon, deleteIcon;
		ContextMenu cmFrame, cmLabel, cmImage, cmAudio, cmMovie, cmIcon;

		//текст для динамических кнопок
		string framemenu = "Frame➤Menu";
		string menuframe = "Menu➤Frame";

		//общие для всех элементы комбобоксов
		ComboBoxItem jumpAction, callAction, passAction;

		XFrame currentFrame;
		XImage currentImage;
		XAudio currentAudio;
		XMovie currentMovie;
		List<XFrame> previousFrames = new List<XFrame> { };
		List<ComboBoxItem> fonts;
		List<ComboBoxItem> menuActions = new List<ComboBoxItem> { };
		List<string> animationIn = new List<string>
		{ "None","dissolve","fade","pixellate","move","moveinright","moveinleft","moveintop","moveinbottom","easeinright","easeinleft","easeintop","easeinbottom","zoomin","zoominout",
			"vpunch","hpunch","blinds","squares","wipeleft","wiperight","wipeup","wipedown","slideleft","slideright","slideup","slidedown","pushright","pushleft","pushtop","pushbottom","irisin"};
		List<string> animationOut = new List<string>
		{"None","dissolve","fade","pixellate","move","moveoutright","moveoutleft","moveouttop","moveoutbottom","easeoutright","easeoutleft","easeouttop","easeoutbottom","zoomout","zoominout",
			"vpunch","hpunch","blinds","squares","wipeleft","wiperight","wipeup","wipedown","slideawayleft","slideawayright","slideawayup","slideawaydown","pushright","pushleft","pushtop","pushbottom","irisout" };
		ObservableCollection<ComboBoxItem> menuLabelList = new ObservableCollection<ComboBoxItem> { };

		List<ImageBackProperties> BackInFrameProps = new List<ImageBackProperties> { };
		List<ImageCharProperties> ImageInFrameProps = new List<ImageCharProperties> { };
		List<AudioProperties> AudioInFrameProps = new List<AudioProperties> { };
		bool removeorunselect = true;//переключатель удаления взаимосвязей выделенных ресурсов. При выборе фрейма их не надо удалять, потому 0, при снятии галочки вручную - 1
		bool addorselect = true;//то же самое на случай добавления или показа ресурсов
		bool waschecked = false;//если галочка стояла, 1. если нет, 0
		bool show = false;
		private XImage lastImageChecked;//для содержания последнего выбранного элемента из списка фоновых картинок
		private XMovie lastMovieChecked;//для содержания последнего выбранного элемента из списка фоновых видео

		public void initializeAll()
		{
			//contextMenus
			addFrame = new MenuItem() { Header = "Add empty frame" }; addFrame.Click += addNextFrame_Click;
			addRootFrame = new MenuItem() { Header = "Add empty frame" }; addRootFrame.Click += addNextFrame_Click;
			convertFrameMenu = new MenuItem() { }; convertFrameMenu.Click += convertFrameMenu_Click;
			deleteFrame = new MenuItem() { Header = "Delete frame/menu" }; deleteFrame.Click += deleteFrame_Click;
			addMenu = new MenuItem() { Header = "Add menu" }; addMenu.Click += addNextFrame_Click;
			addImage = new MenuItem() { Header = "Add image" }; addImage.Click += imageImport_Click;
			reloadImage = new MenuItem() { Header = "Reload image" }; reloadImage.Click += imageReload_Click;
			deleteImage = new MenuItem() { Header = "Delete image" }; deleteImage.Click += imageDeleteFromList_Click;
			addAudio = new MenuItem() { Header = "Add audio" }; addAudio.Click += audioImport_Click;
			reloadAudio = new MenuItem() { Header = "Reload audio" }; reloadAudio.Click += audioReload_Click;
			deleteAudio = new MenuItem() { Header = "Delete audio" }; deleteAudio.Click += audioDeleteFromList_Click;
			addMovie = new MenuItem() { Header = "Add movie" }; addMovie.Click += movieImport_Click;
			reloadMovie = new MenuItem() { Header = "Reload movie" }; reloadMovie.Click += movieReload_Click;
			deleteMovie = new MenuItem() { Header = "Delete movie" }; deleteMovie.Click += deleteVideo_Click;
			addIcon = new MenuItem() { Header = "Add icon" }; addIcon.Click += imageImport_Click;
			reloadIcon = new MenuItem() { Header = "Reload icon" }; reloadIcon.Click += imageReload_Click;
			deleteIcon = new MenuItem() { Header = "Delete icon" }; deleteIcon.Click += imageDeleteFromList_Click;


			cmFrame = new ContextMenu { ItemsSource = new MenuItem[] {addRootFrame, convertFrameMenu, deleteFrame} };
			cmLabel = new ContextMenu { ItemsSource = new MenuItem[] { addFrame, addMenu} };
			cmImage = new ContextMenu { ItemsSource = new MenuItem[] { addImage, reloadImage, deleteImage } };
			cmAudio = new ContextMenu { ItemsSource = new MenuItem[] { addAudio, reloadAudio, deleteAudio } };
			cmMovie = new ContextMenu { ItemsSource = new MenuItem[] { addMovie, reloadMovie, deleteMovie } };
			cmIcon = new ContextMenu { ItemsSource = new MenuItem[] { addIcon, reloadIcon, deleteIcon } };

			//menuOptions
			jumpAction = new ComboBoxItem() { Content = "jump" };
			callAction = new ComboBoxItem() { Content = "call" };
			passAction = new ComboBoxItem() { Content = "pass" };			
			menuActions.Add(jumpAction);
			menuActions.Add(callAction);
			menuActions.Add(passAction);

			//image animations
			animationInTypeComboBox.ItemsSource = animationIn;
			animationOutTypeComboBox.ItemsSource = animationOut;

			//start
			XFrame firstFrame = createFrame();
			ListView startListView = createLabel("start");
			startListView.Items.Add(firstFrame);
			currentFrame = firstFrame;
			firstFrame.IsSelected = true;
			//characters
			characterListView.SelectedIndex = 0;

			//options
			title.Text = "default";
			gameOpenTransition.ItemsSource = animationIn; gameOpenTransition.SelectedIndex = 1;
			gameExitTransition.ItemsSource = animationOut; gameExitTransition.SelectedIndex = 1;
			gameStartTransition.ItemsSource = animationIn; gameStartTransition.SelectedIndex = 0;
			gameIntraTransition.ItemsSource = animationIn.Intersect(animationOut); gameIntraTransition.SelectedIndex = 1;
			gameEndTransition.ItemsSource = animationOut; gameEndTransition.SelectedIndex = 0;
			dialogShowTransition.ItemsSource = animationIn; dialogShowTransition.SelectedIndex = 1;
			dialogHideTransition.ItemsSource = animationOut; dialogHideTransition.SelectedIndex = 1;

			//gui: fonts
			fonts = new List<ComboBoxItem> { };
			FileInfo[] fontFiles = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Fonts)).GetFiles();
			foreach (FileInfo font in fontFiles)
			{
				if (Fonts.GetFontFamilies(font.FullName).Count != 0)
				{
					Typeface type = Fonts.GetTypefaces(font.FullName).First();
					string style = ""; string weight = ""; string stretch = "";
					if (type.Style != FontStyles.Normal) style = " " + type.Style.ToString();
					if (type.Weight != FontWeights.Normal) weight = " " + type.Weight.ToString();
					if (type.Stretch != FontStretches.Normal) stretch = " " + type.Stretch.ToString();
					ComboBoxItem newFont = new ComboBoxItem()
					{
						Content = type.FontFamily.Source.Substring(type.FontFamily.Source.IndexOf('#') + 1) + style + weight + stretch,
						FontFamily = type.FontFamily,
						FontStyle = type.Style,
						FontWeight = type.Weight,
						FontStretch = type.Stretch,
						Tag = font.Name
					};
					fonts.Add(newFont);
				}
			}
			comboBox_FontText.ItemsSource = fonts;
			comboBox_FontChar.ItemsSource = fonts;
			comboBox_FontInterface.ItemsSource = fonts;
			ComboBoxItem selectedfont=fonts.FirstOrDefault(font => font.Content.ToString().Equals("DejaVu Sans"));
			comboBox_FontText.SelectedItem = selectedfont;
			comboBox_FontChar.SelectedItem = selectedfont;
			comboBox_FontInterface.SelectedItem = selectedfont;
		}

		private void createDirectories()
		{
			Directory.CreateDirectory(projectFolder + imagesFolder);
			Directory.CreateDirectory(projectFolder + musicFolder);
			Directory.CreateDirectory(projectFolder + soundsFolder);
			Directory.CreateDirectory(projectFolder + voicesFolder);
			Directory.CreateDirectory(projectFolder + moviesFolder);
			Directory.CreateDirectory(projectFolder + guiFolder);
		}
	}
}
