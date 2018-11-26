using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Collections.ObjectModel;

namespace X_Ren_Py
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		static string game = @"\game\";
        string script = game+@"script.rpy";
        string options = game + @"options.rpy";
        string screens = game + @"screens.rpy";
		string gui = game + @"gui.rpy";

		string projectFolder = Environment.CurrentDirectory+@"/temp"+game;
		string imagesFolder = @"images/";
		string musicFolder = @"music/";
		string soundsFolder = @"sounds/";
		string voicesFolder = @"voices/";
		string moviesFolder = @"movies/";

		static string tab = "    ";
		static string nextLine = "\r\n";

        //строки для script.rpy         
		string nextBlock = ":" + nextLine+ tab;
        string nextOption = nextLine + tab+"\"";
        
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
		string Return="return";

		//строки для options.rpy

		//string gameEnterTransition = "define config.enter_transition";
		//string gameExitTransition = "define config.exit_transition";
		//string gameIntraTransition = "define config.intra_transition";
		//string gameLoadTransition = "define config.after_load_transition";
		//string gameEndTransition = "define config.end_game_transition";
		//string gameWindowShow= "define config.window";
		//string gameWindowShowTransition = "define config.window_show_transition";
		//string gameWindowHideTransition = "define config.window_hide_transition";
		//string gameSaveDir = "define config.save_directory";
		//string gameIcon = "define config.window_icon";

		//строки для gui.rpy


		//строка-компаратор
		string[] comparerScript = {"define", "image", "label"};
		string[] comparerOptions = {"define config.name", "define gui.show_name", "define config.version", "define gui.about", "define build.name", "define config.has_sound", "define config.has_music", "default preferences.text_cps", "default preferences.afm_time" };
		//ресурсы
		string imageextensions = "Файлы рисунков (*.bmp, *.jpg, *.png, *.webp)|*.bmp;*.jpg;*.png;*.webp";
        string audioextensions = "Файлы аудио (*.wav, *.ogg, *.mp3, *.opus)|*.wav;*.ogg;*.mp3;*.opus";
        string vidextensions = "Файлы видео (*.wmv,*.webm, *.mkv, *.avi, *.ogv)|*.wmv;*.webm;*.mkv;*.avi;*.ogv";

        //контекстные меню
        MenuItem addTab, deleteTab, addFrame, insertFrame, duplicateFrame, duplicateRootframe, deleteFrame, addMenu, addImage, deleteImage, addAudio, deleteAudio, stopAudio, addMovie, deleteMovie;
        ContextMenu cmFrame, cmRootframe, cmLabel, cmImage, cmAudio, cmMovie;

        //текст для динамических кнопок
        string framemenu = "➤Menu";
        string menuframe = "➤Frame";

		//общие для всех элементы комбобоксов
		ComboBoxItem jumpAction, callAction, passAction, emptyLabel;

        int framecount = 0;
        XFrame currentFrame;
        XImage currentImage;
        XAudio currentAudio;
        XMovie currentMovie;
		List<ComboBoxItem> menuActions= new List<ComboBoxItem> { };
		ObservableCollection<ComboBoxItem> menuLabelList = new ObservableCollection<ComboBoxItem> { };
        List<ImageProperties> ImageInFrameProps = new List<ImageProperties> { };
        List<AudioProperties> AudioInFrameProps = new List<AudioProperties> { };
        bool removeorunselect=true;//переключатель удаления взаимосвязей выделенных ресурсов. При выборе фрейма их не надо удалять, потому 0, при снятии галочки вручную - 1
        bool addorselect=true;//то же самое на случай добавления или показа ресурсов
        bool show = false;
        private XImage lastImageChecked;//для содержания последнего выбранного элемента из списка фоновых картинок
        private XMovie lastMovieChecked;//для содержания последнего выбранного элемента из списка фоновых видео

        public void initializeAll()
		{
			createDirectories();
			//contextMenus
			addTab= new MenuItem() { Header = "Add label" }; addTab.Click += addLabel_Click;
			deleteTab= new MenuItem() { Header = "Delete label" }; deleteTab.Click += deleteLabel_Click;
			addFrame = new MenuItem() { Header = "Add frame" }; addFrame.Click += addInsertFrame_Click;			
			insertFrame = new MenuItem() { Header = "Insert frame" }; insertFrame.Click += addInsertFrame_Click;
			duplicateFrame = new MenuItem() { Header = "Duplicate frame" }; duplicateFrame.Click += duplicateFrame_Click;
			duplicateRootframe = new MenuItem() { Header = "Duplicate frame" }; duplicateRootframe.Click += duplicateFrame_Click;
			deleteFrame = new MenuItem() { Header = "Delete frame/menu" }; deleteFrame.Click += deleteFrame_Click;
			addMenu = new MenuItem() { Header = "Add menu" }; addMenu.Click += addInsertFrame_Click;
			addImage = new MenuItem() { Header = "Add image file" }; addImage.Click += imageImport_Click;
			deleteImage = new MenuItem() { Header = "Delete image file" }; deleteImage.Click += imageDeleteFromList_Click;
			addAudio = new MenuItem() { Header = "Add audio file" }; addAudio.Click += audioImport_Click;
			deleteAudio = new MenuItem() { Header = "Delete audio file" }; deleteAudio.Click += audioDeleteFromList_Click;
			stopAudio = new MenuItem() { Header = "Stop audio", IsCheckable = true }; stopAudio.Checked += stopAudio_Click;
			addMovie = new MenuItem() { Header = "Add movie file" }; addMovie.Click += movieImport_Click;
			deleteMovie = new MenuItem() { Header = "Delete movie file" }; deleteMovie.Click += deleteVideo_Click;

			cmFrame = new ContextMenu { ItemsSource = new MenuItem[] { insertFrame, duplicateFrame, deleteFrame, stopAudio } };
			cmRootframe = new ContextMenu { ItemsSource = new MenuItem[] { duplicateRootframe } };
			cmLabel = new ContextMenu { ItemsSource = new MenuItem[] { addTab, addFrame, addMenu, deleteTab } };
			cmImage = new ContextMenu { ItemsSource = new MenuItem[] { addImage, deleteImage } };
			cmAudio = new ContextMenu { ItemsSource = new MenuItem[] { addAudio, deleteAudio } };
			cmMovie = new ContextMenu { ItemsSource = new MenuItem[] { addMovie, deleteMovie } };

			//menuOptions
			jumpAction = new ComboBoxItem() { Content = "jump"};
			callAction = new ComboBoxItem() { Content = "call" };
			passAction = new ComboBoxItem() { Content = "pass" };
			emptyLabel = new ComboBoxItem() { Visibility=Visibility.Collapsed };
			menuActions.Add(jumpAction);
			menuActions.Add(callAction);
			menuActions.Add(passAction);
			menuLabelList.Add(emptyLabel);

			//start
			ListView startListView=createLabel("start");
			startListView.Items.Add(createFrame(true));
			currentFrame = startListView.Items[0] as XFrame;
			(startListView.Items[0] as XFrame).IsSelected = true;			

			//options
			projectWidth.Text = "1280";
			projectHeight.Text = "720";
		}
		
		private void createDirectories()
		{
			Directory.CreateDirectory(projectFolder + imagesFolder);
			Directory.CreateDirectory(projectFolder + musicFolder);
			Directory.CreateDirectory(projectFolder + soundsFolder);
			Directory.CreateDirectory(projectFolder + voicesFolder);
			Directory.CreateDirectory(projectFolder + moviesFolder);
		}
	}
}                
                               