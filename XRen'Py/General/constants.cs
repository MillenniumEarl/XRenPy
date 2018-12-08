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

		//строки для gui.rpy
		//define gui.text_font = "DejaVuSans.ttf"
		//define gui.name_text_font = "DejaVuSans.ttf"
		//define gui.interface_text_font = "DejaVuSans.ttf"

		//строка-компаратор
		string[] comparerScript = { "define", "image", "label" };
		string[] comparerOptions = {"define config.name","define gui.show_name","define config.version","define gui.about",
			"define build.name","define config.has_sound","define config.has_music","define config.has_voice", "default preferences.text_cps", "default preferences.afm_time",
"define config.enter_transition","define config.exit_transition","define config.after_load_transition","define config.end_game_transition","define config.window_icon"};
		string[] comparerGui = { "gui.init", "define gui.accent_color", "define gui.idle_color", "define gui.idle_small_color", "define gui.hover_color", "define gui.selected_color",
			"define gui.insensitive_color", "define gui.muted_color", "define gui.hover_muted_color", "define gui.text_color", "define gui.text_size","define gui.name_text_size","define gui.interface_text_size",
			"define gui.label_text_size", "define gui.notify_text_size", "define gui.title_text_size" };
		//ресурсы
		string imageextensions = "Image files (*.bmp, *.jpg, *.png, *.webp)|*.bmp;*.jpg;*.png;*.webp";
		string audioextensions = "Audio files (*.wav, *.ogg, *.mp3, *.opus)|*.wav;*.ogg;*.mp3;*.opus";
		string vidextensions = "Video files (*.wmv,*.webm, *.mkv, *.avi, *.ogv)|*.wmv;*.webm;*.mkv;*.avi;*.ogv";
		SolidColorBrush currentFrameResourceColor = new SolidColorBrush(Color.FromArgb(127, 0, 127, 0));
		SolidColorBrush unusedResourceColor = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

		//контекстные меню
		MenuItem addLabel, deleteLabel, addFrame, addRootFrame, duplicateFrame, duplicateRootframe, convertFrameMenu, deleteFrame, addMenu, addImage, deleteImage, reloadImage, addAudio, deleteAudio, stopAudio, reloadAudio, addMovie, deleteMovie, reloadMovie;
		ContextMenu cmFrame, cmRootframe, cmLabel, cmImage, cmAudio, cmMovie;

		//текст для динамических кнопок
		string framemenu = "Frame➤Menu";
		string menuframe = "Menu➤Frame";

		//общие для всех элементы комбобоксов
		ComboBoxItem jumpAction, callAction, passAction, emptyLabel;

		int framecount = 0;
		XFrame currentFrame;
		XImage currentImage;
		XAudio currentAudio;
		XMovie currentMovie;
		List<ComboBoxItem> menuActions = new List<ComboBoxItem> { };
		List<string> animationIn = new List<string>
		{ "None","dissolve","fade","pixellate","move","moveinright","moveinleft","moveintop","moveinbottom","easeinright","easeinleft","easeintop","easeinbottom","zoomin","zoominout",
			"vpunch","hpunch","blinds","squares","wipeleft","wiperight","wipeup","wipedown","slideleft","slideright","slideup","slidedown","pushright","pushleft","pushtop","pushbottom","irisin"};
		List<string> animationOut = new List<string>
		{"None","dissolve","fade","pixellate","move","moveoutright","moveoutleft","moveouttop","moveoutbottom","easeoutright","easeoutleft","easeouttop","easeoutbottom","zoomout","zoominout",
			"vpunch","hpunch","blinds","squares","wipeleft","wiperight","wipeup","wipedown","slideawayleft","slideawayright","slideawayup","slideawaydown","pushright","pushleft","pushtop","pushbottom","irisout" };
		ObservableCollection<ComboBoxItem> menuLabelList = new ObservableCollection<ComboBoxItem> { };
		List<Image> characterIcons = new List<Image> { };
		List<ImageProperties> ImageInFrameProps = new List<ImageProperties> { };
		List<AudioProperties> AudioInFrameProps = new List<AudioProperties> { };
		bool removeorunselect = true;//переключатель удаления взаимосвязей выделенных ресурсов. При выборе фрейма их не надо удалять, потому 0, при снятии галочки вручную - 1
		bool addorselect = true;//то же самое на случай добавления или показа ресурсов
		bool show = false;
		private XImage lastImageChecked;//для содержания последнего выбранного элемента из списка фоновых картинок
		private XMovie lastMovieChecked;//для содержания последнего выбранного элемента из списка фоновых видео

		public void initializeAll()
		{
			//contextMenus
			addLabel = new MenuItem() { Header = "Add label" }; addLabel.Click += addLabel_Click;
			deleteLabel = new MenuItem() { Header = "Delete label" }; deleteLabel.Click += deleteLabel_Click;
			addFrame = new MenuItem() { Header = "Add empty frame" }; addFrame.Click += addNextFrame_Click;
			addRootFrame = new MenuItem() { Header = "Add empty frame" }; addRootFrame.Click += addNextFrame_Click;
			convertFrameMenu = new MenuItem() { }; convertFrameMenu.Click += convertFrameMenu_Click;
			duplicateFrame = new MenuItem() { Header = "Duplicate frame" }; duplicateFrame.Click += duplicateFrame_Click;
			duplicateRootframe = new MenuItem() { Header = "Duplicate frame" }; duplicateRootframe.Click += duplicateFrame_Click;
			deleteFrame = new MenuItem() { Header = "Delete frame/menu" }; deleteFrame.Click += deleteFrame_Click;
			addMenu = new MenuItem() { Header = "Add menu" }; addMenu.Click += addNextFrame_Click;
			addImage = new MenuItem() { Header = "Add image" }; addImage.Click += imageImport_Click;
			reloadImage = new MenuItem() { Header = "Reload image" }; reloadImage.Click += imageReload_Click;
			deleteImage = new MenuItem() { Header = "Delete image" }; deleteImage.Click += imageDeleteFromList_Click;
			addAudio = new MenuItem() { Header = "Add audio" }; addAudio.Click += audioImport_Click;
			reloadAudio = new MenuItem() { Header = "Reload audio" }; reloadAudio.Click += audioReload_Click;
			deleteAudio = new MenuItem() { Header = "Delete audio" }; deleteAudio.Click += audioDeleteFromList_Click;
			stopAudio = new MenuItem() { Header = "Stop audio", IsCheckable = true }; stopAudio.Checked += stopAudio_Click;
			addMovie = new MenuItem() { Header = "Add movie" }; addMovie.Click += movieImport_Click;
			reloadMovie = new MenuItem() { Header = "Reload audio" }; reloadMovie.Click += movieReload_Click;
			deleteMovie = new MenuItem() { Header = "Delete movie" }; deleteMovie.Click += deleteVideo_Click;

			cmFrame = new ContextMenu { ItemsSource = new MenuItem[] { addFrame, duplicateFrame, convertFrameMenu, deleteFrame, stopAudio } };
			cmRootframe = new ContextMenu { ItemsSource = new MenuItem[] { addRootFrame, duplicateRootframe } };
			cmLabel = new ContextMenu { ItemsSource = new MenuItem[] { addLabel, addFrame, addMenu, deleteLabel } };
			cmImage = new ContextMenu { ItemsSource = new MenuItem[] { addImage, reloadImage, deleteImage } };
			cmAudio = new ContextMenu { ItemsSource = new MenuItem[] { addAudio, reloadAudio, deleteAudio } };
			cmMovie = new ContextMenu { ItemsSource = new MenuItem[] { addMovie, reloadMovie, deleteMovie } };

			//menuOptions
			jumpAction = new ComboBoxItem() { Content = "jump" };
			callAction = new ComboBoxItem() { Content = "call" };
			passAction = new ComboBoxItem() { Content = "pass" };
			emptyLabel = new ComboBoxItem() { Visibility = Visibility.Collapsed };
			menuActions.Add(jumpAction);
			menuActions.Add(callAction);
			menuActions.Add(passAction);
			menuLabelList.Add(emptyLabel);

			//image animations
			animationInTypeComboBox.ItemsSource = animationIn;
			animationOutTypeComboBox.ItemsSource = animationOut;

			//start
			XFrame firstFrame = createFrame(true);
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
