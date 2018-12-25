using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media;
using System;

namespace X_Ren_Py
{	
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
