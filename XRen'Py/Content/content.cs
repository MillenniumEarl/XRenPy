using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace X_Ren_Py
{

	public class XFrame : ListViewItem
	{
		//string _Content;
		private string _Text = "";
		private bool _isMenu = false;
		private bool _stopAudio = false;
		public ObservableCollection<XMenuOption> _MenuOptions;
		private XCharacter _Character;
		private ImageProperties _BackgroundImageProps=new ImageProperties();
		private XMovie _Movie;

		public string Text { get { return _Text; } set { _Text = value; } }
		public bool isMenu { get { return _isMenu; } set { _isMenu = value; } }
		public bool stopAudio { get { return _stopAudio; } set { _stopAudio = value; } }
		public ObservableCollection<XMenuOption> MenuOptions { get { return _MenuOptions; } set { _MenuOptions = value; } }
		public XCharacter Character { get { return _Character; } set { _Character = value; } }

		public ImageProperties BackgroundImageProps { get { return _BackgroundImageProps; } set { _BackgroundImageProps = value; } }
		public XImage BackgroundImage { get { return _BackgroundImageProps.Image; } set { _BackgroundImageProps.Image = value; } }
		public byte AnimationInType { get { return _BackgroundImageProps.AnimationInType; } set { _BackgroundImageProps.AnimationInType = value; } }
		public byte AnimationOutType { get { return _BackgroundImageProps.AnimationOutType; } set { _BackgroundImageProps.AnimationOutType = value; } }
		public XMovie Movie { get { return _Movie; } set { _Movie = value; } }
	}
	public class XMenuOption : ListViewItem
	{
		private Label _Href = new Label() { Width = 540, FontSize = 22, Padding = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Center, HorizontalContentAlignment = HorizontalAlignment.Center, Visibility = Visibility.Collapsed };
		private TextBox _Choice = new TextBox() { Width = 300, FontSize = 22, Text = "Menu option", Padding = new Thickness(5) };
		private ComboBox _Action = new ComboBox() { Width = 80, FontSize = 22, Padding = new Thickness(5) };
		private ComboBox _Label = new ComboBox() { Width = 160, FontSize = 22, Padding = new Thickness(5) };
		private Button Edit = new Button() { Background = Brushes.LightBlue, FontSize = 22, Padding = new Thickness(5), Content = "✏" };
		public Button Delete = new Button() { FontSize = 22, Padding = new Thickness(5), Content = "🗑" };
		public string Href { get { return _Href.Content.ToString(); } }
		public string Choice { get { return _Choice.Text; } set { _Choice.Text = value; _Href.Content = value; } }
		public ComboBox MenuAction { get { return _Action; } set { _Action = value; } }
		public ComboBox ActionLabel { get { return _Label; } set { _Label = value; } }

		public XMenuOption()
		{
			StackPanel stack = new StackPanel();
			stack.Margin = new Thickness(5);
			stack.HorizontalAlignment = HorizontalAlignment.Stretch;
			stack.Orientation = Orientation.Horizontal;
			MenuAction.SelectionChanged += MenuAction_SelectionChanged;
			Edit.Click += Edit_Click;
			stack.Children.Add(Edit);
			stack.Children.Add(_Href);
			stack.Children.Add(_Choice);
			stack.Children.Add(_Action);
			stack.Children.Add(ActionLabel);
			stack.Children.Add(Delete);
			Content = stack;
		}

		private void Edit_Click(object sender, RoutedEventArgs e)
		{			
			if (Edit.Background == Brushes.LightBlue)
			{
				Edit.Background = Brushes.WhiteSmoke;
				_Href.Visibility = Visibility.Visible;
				_Choice.Visibility = Visibility.Collapsed;
				MenuAction.Visibility = Visibility.Collapsed;
				ActionLabel.Visibility = Visibility.Collapsed;
				Choice = Choice;
			}
			else
			{
				Edit.Background = Brushes.LightBlue;
				_Href.Visibility = Visibility.Collapsed;
				_Choice.Visibility = Visibility.Visible;
				MenuAction.Visibility = Visibility.Visible;
				ActionLabel.Visibility = Visibility.Visible;
			}
		}

		private void MenuAction_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (MenuAction.SelectedIndex == 2) { ActionLabel.IsEnabled = false; }
			else ActionLabel.IsEnabled = true;
		}
	}

	public class XCharacter : ListViewItem
	{
		//private string _Content;
		private string _Alias;
		private bool _NameIsBold;
		private bool _NameIsItalic;
		private bool _TextIsBold;
		private bool _TextIsItalic;
		private Color _NameColor;
		private Color _TextColor;
		private Image _Icon;
		public string Alias { get { return _Alias; } set { _Alias = value; } }

		public void ContentToAlias() { _Alias = Content.ToString().ToLower().Replace(" ", "").Replace("-", "").Replace("\'", ""); }
		public Color NameColor { get { return _NameColor; } set { _NameColor = value; } }
		public Color TextColor { get { return _TextColor; } set { _TextColor = value; } }
		public bool NameIsBold { get { return _NameIsBold; } set { _NameIsBold = value; } }
		public bool NameIsItalic { get { return _NameIsItalic; } set { _NameIsItalic = value; } }
		public bool TextIsBold { get { return _TextIsBold; } set { _TextIsBold = value; } }
		public bool TextIsItalic { get { return _TextIsItalic; } set { _TextIsItalic = value; } }
		public Image Icon { get { return _Icon; } set { _Icon = value; } }

}

    public class XContent:ListViewItem
    {
		protected string _Alias;
		protected string _Path;

        protected CheckBox _CheckBox = new CheckBox() { Margin = new Thickness(0), Padding = new Thickness(0) };
        protected Label _Label = new Label() { Margin = new Thickness(1, 0, 1, 0), Padding = new Thickness(1, 0, 1, 0) };

		public string Alias { get { return _Alias; } set { _Alias = value; } }
		public string Path { get { return _Path; } set { _Path = value; } }

		public bool? IsChecked { get { return _CheckBox.IsChecked; } set { _CheckBox.IsChecked = value; } }
        public CheckBox Checkbox { get { return _CheckBox; } set { _CheckBox = value; } }
        public string Header { get { return _Label.Content.ToString(); } set { _Label.Content = value; _Alias = value.ToLower().Substring(0, value.LastIndexOf('.')).Replace(" ","").Replace("-", ""); } }
		public Brush TextColor { get { return _Label.Foreground; } set { _Label.Foreground = value; } }
		
		protected void createContent()
		{			
			StackPanel stack = new StackPanel { Orientation = Orientation.Horizontal};
			stack.Children.Add(_CheckBox);
			stack.Children.Add(_Label);
			Content = stack;       
        }     
    }

    public class XImage : XContent
    {
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

		public XImage()
        {
            createContent();
        }
    }
    public class XAudio : XContent
	{
		private string _Type= "music ";
		public string Type { get { return _Type; } set { _Type = value; } }

		public void loadAudio(string singleLine, string folder)
		{
			try
			{
				int firstquote = singleLine.IndexOf('"') + 1;
				Path = folder + singleLine.Substring(firstquote, singleLine.LastIndexOf('"') - firstquote);
				Header = Path.Substring(singleLine.LastIndexOf('/') + 1);
				Alias = singleLine.Substring(13, singleLine.IndexOf('=') - 13).TrimEnd(' ');
			}
			catch (Exception) { MessageBox.Show("Error: Audio loading", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
		}
		public XAudio()
        {
            createContent();
        }
    }
    public class XMovie : XContent
    {
        //string _Content;
        string _MaskPath; //путь к маске видео. если она есть
        public string MaskPath { get { return _MaskPath; } set { _MaskPath = value; } }
		public void loadMovie(string singleLine, string folder)
		{
			try
			{
				int firstquote = singleLine.IndexOf('"') + 1;

				string[] all = singleLine.Substring(firstquote, singleLine.LastIndexOf('"') - firstquote).Replace("\"", "").Replace(" ", "").Split(',');
				for (int prop = 0; prop < all.Length; prop++)
				{
					if (all[prop].StartsWith("play")) Path = all[prop].Substring(5);
					else if (all[prop].StartsWith("mask")) MaskPath = all[prop].Substring(5);
				}
				Header = Path.Substring(singleLine.LastIndexOf('/') + 1);
				Alias = singleLine.Substring(6, singleLine.IndexOf('=') - 6).TrimEnd(' ');
			}
			catch (Exception) { MessageBox.Show("Error: Movie loading", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
		}
        public XMovie()
        {
            createContent();
        }
    }

    public class ImageProperties
    {
        private XFrame _Frame;
        private XImage _Image;
		private Image _Displayable;
		private byte _Align;//0 - по умолчанию
		private byte _AnimationInType;//0-нет анимации
		private byte _AnimationOutType;//0-нет анимации

		public XFrame Frame { get { return _Frame; } set { _Frame = value; } }
        public XImage Image { get { return _Image; } set { _Image = value; } }
		public Image Displayable { get { return _Displayable; } set { _Displayable = value; } }
		public byte Align { get { return _Align; } set { _Align = value; } }
        public byte AnimationInType { get { return _AnimationInType; } set { _AnimationInType = value; } }
		public byte AnimationOutType { get { return _AnimationOutType; } set { _AnimationOutType = value; } }
	}
    public class AudioProperties
    {
        private XFrame _Frame;
        private XAudio _Audio;
        private float _FadeIn;
        private float _FadeOut;
        //private bool _Queue = false;//0-не в очереди на проигрывание
        private bool _Loop = false;
        public XFrame Frame { get { return _Frame; } set { _Frame = value; } }
        public XAudio Audio { get { return _Audio; } set { _Audio = value; } }
        public float FadeIn { get { return _FadeIn; } set { _FadeIn = value; } }
        public float FadeOut { get { return _FadeOut; } set { _FadeOut = value; } }
        //public bool Queue { get { return _Queue; } set { _Queue = value; } }
        public bool Loop { get { return _Loop; } set { _Loop = value; } }
    }
    
    public partial class MainWindow : Window
    {

		protected void content_Selected(object sender, RoutedEventArgs e)  
        {
            show = true;
			if (File.Exists((sender as XContent).Path))
			{
				switch (sender.GetType().ToString())
				{
					case "X_Ren_Py.XImage":
						{
							currentImage = sender as XImage;
							if ((sender as XImage).IsChecked == true && ((sender as XImage).Parent as ListView) == imageListView) if (!addorselect) getImageProperties(currentFrame, sender as XImage);
						};
						break;
					case "X_Ren_Py.XAudio":
						{
							currentAudio = sender as XAudio;
							if ((sender as XAudio).IsChecked == true) if (!addorselect) getAudioProperties(currentFrame, sender as XAudio);
						};
						break;
					case "X_Ren_Py.XMovie":
						currentMovie = sender as XMovie;
						break;
					default: return;
				}
			}
        }
        
		private void namechange_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            HeaderChange inputDialog = new HeaderChange();
            if (inputDialog.ShowDialog() == true && inputDialog.Answer != "")
            {
				menuLabelList.First(item => item.Content == (sender as TabItem).Header).Content = inputDialog.Answer;
							(sender as TabItem).Header = inputDialog.Answer;
            }
            else MessageBox.Show("Empty header!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void contentCollector(string currentPath, string projectPath)
        {
			if (File.Exists(projectPath))
			{
				if (projectPath != currentPath)
				{
					if (!Equals(File.ReadAllBytes(projectPath), File.ReadAllBytes(currentPath)))
					{
						File.Delete(projectPath);
						File.Copy(currentPath, projectPath);
					}
				}
			}
			else File.Copy(currentPath, projectPath);
		}

        private void resourcesSelectedItem_delete()
        {
            ListView selectedList = tabControlResources.SelectedContent as ListView;
            selectedList.Items.Remove(selectedList.SelectedItem);
        }
		private void uncheckAll()
		{
			removeorunselect = false;
			foreach (TabItem tab in tabControlResources.Items)
			{
				foreach (XContent resource in (tab.Content as ListView).Items)
				{
					resource.IsChecked = false;
					resource.Background = unusedResourceColor;
				}
			}
			removeorunselect = true;
		}
	}
}
