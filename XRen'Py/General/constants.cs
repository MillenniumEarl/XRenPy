using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Threading;

namespace X_Ren_Py
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		string projectFolder = Environment.CurrentDirectory + @"/temp" + game;
		static string game = @"/game/";
		string script = game + @"script.rpy";
		string options = game + @"options.rpy";
		string screens = game + @"screens.rpy";
		string gui = game + @"gui.rpy";
		
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
		MenuItem addFrame, addRootFrame, convertToFrame, convertToMenu, convertToPause, deleteFrame, addMenu, addPause, addImage, deleteImage, reloadImage, addAudio, deleteAudio, reloadAudio, addMovie, deleteMovie, reloadMovie, addIcon, reloadIcon, deleteIcon;
		static ContextMenu cmFrame, cmLabel, cmImage, cmAudio, cmMovie, cmIcon;

		//общие для всех элементы комбобоксов
		ComboBoxItem jumpAction, callAction, passAction;

		XFrame currentFrame;
		XImage currentImage;
		XAudio currentAudio;
		XMovie currentMovie;
        XCharacter currentCharacter;
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
        
        XCharacter charNone = new XCharacter { Background=new SolidColorBrush(Color.FromArgb(255,240,240,240)), Content = "none", Alias = "none"};
        XCharacter charNvl = new XCharacter { Background = new SolidColorBrush(Color.FromArgb(255, 230, 230, 230)), Content = "nvl", Alias = "nvl" };
        XCharacter charCentered = new XCharacter { Background = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220)), Content = "centered", Alias = "centered" };
        XCharacter charExtend = new XCharacter { Background = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220)), Content = "extend", Alias = "extend" };
        ObservableCollection<XCharacter> characterList = new ObservableCollection<XCharacter> { };

        List<ImageBackProperties> BackInFrameProps = new List<ImageBackProperties> { };
		List<ImageCharProperties> ImageInFrameProps = new List<ImageCharProperties> { };
		List<AudioProperties> AudioInFrameProps = new List<AudioProperties> { };
		bool removeorunselect = true;//переключатель удаления взаимосвязей выделенных ресурсов. При выборе фрейма их не надо удалять, потому 0, при снятии галочки вручную - 1
		bool addorselect = true;//то же самое на случай добавления или показа ресурсов
		bool waschecked = false;//если галочка стояла, 1. если нет, 0
		bool show = false;
        bool containsMusic = false;
        bool containsSound = false;
        bool containsVoice = false;

        private XImage lastBackChecked;//для содержания последнего выбранного элемента из списка фоновых картинок
		private XAudio lastMusicChecked;//без возможности поставить звук в очередь нужно использовать это
		private XAudio lastSoundChecked;
		private XAudio lastVoiceChecked;
		private XMovie lastMovieChecked;//для содержания последнего выбранного элемента из списка фоновых видео
		
		DispatcherTimer disptimer = new DispatcherTimer();

		public void initializeAll()
		{
			//contextMenus
			addFrame = new MenuItem() { Header = "Add empty frame" }; addFrame.Click += addNextFrame_Click;
			addRootFrame = new MenuItem() { Header = "Add empty frame" }; addRootFrame.Click += addNextFrame_Click;
			convertToFrame = new MenuItem() { Header = "➤Frame" }; convertToFrame.Click += convertToFrame_Click;
            convertToMenu = new MenuItem() { Header = "➤Menu" }; convertToMenu.Click += convertToMenu_Click;
            convertToPause = new MenuItem() { Header = "➤Pause" }; convertToPause.Click += convertToPause_Click;
            deleteFrame = new MenuItem() { Header = "Delete frame/menu" }; deleteFrame.Click += deleteFrame_Click;
			addMenu = new MenuItem() { Header = "Add menu" }; addMenu.Click += addNextFrame_Click;
            addPause = new MenuItem() { Header = "Add pause" }; addPause.Click += addNextFrame_Click;
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

			cmFrame = new ContextMenu { ItemsSource = new MenuItem[] {addRootFrame, convertToFrame, convertToMenu, convertToPause, deleteFrame} };
			cmLabel = new ContextMenu { ItemsSource = new MenuItem[] { addFrame, addMenu, addPause} };
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
            
            charNone.Selected += uneditableCharacter_Selected;
            charNvl.Selected += uneditableCharacter_Selected;
            charCentered.Selected += uneditableCharacter_Selected;
            charExtend.Selected += uneditableCharacter_Selected;
            charNone.CharView.Content = "none";
            charNvl.CharView.Content = "nvl";
            charCentered.CharView.Content = "centered";
            charExtend.CharView.Content = "extend";
            characterSelector.Items.Add(charNone.CharView);
            characterSelector.Items.Add(charNvl.CharView);
            characterSelector.Items.Add(charCentered.CharView);
            characterSelector.Items.Add(charExtend.CharView);

            characterListView.ItemsSource = characterList;
            //characterSelector.ItemsSource = characterListView.Items;

            //project folder, start and characters
            emptyProject();

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

			//media:DispatcherTimer
			disptimer.Tick += new EventHandler(mediaCurrentTime_Tick);
			disptimer.Interval = new TimeSpan(0, 0, 1);
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
