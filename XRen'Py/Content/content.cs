using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media;
using System.Collections;
using System.Collections.Generic;

namespace X_Ren_Py
{	

	
    public partial class MainWindow : Window
    {
		public class XContent:ListViewItem
    {
		protected string _Alias;
		protected string _Path;

        protected CheckBox _CheckBox = new CheckBox() { Margin = new Thickness(0), Padding = new Thickness(0), IsThreeState=true };
        protected Label _Label = new Label() { Margin = new Thickness(1, 0, 1, 0), Padding = new Thickness(1, 0, 1, 0) };

		public string Alias { get { return _Alias; } set { _Alias = value; } }
		public string Path { get { return _Path; } set { _Path = value; } }

		public bool? IsChecked { get { return _CheckBox.IsChecked; } set { _CheckBox.IsChecked = value; } }
        public CheckBox Checkbox { get { return _CheckBox; } set { _CheckBox = value; } }
        public string Header { get { return _Label.Content.ToString(); } set { _Label.Content = value; _Alias = value.ToLower().Substring(0, value.LastIndexOf('.')).Replace(" ","").Replace("-", ""); } }
		public Brush TextColor { get { return _Label.Foreground; } set { _Label.Foreground = value; } }
		
		public XContent()
		{			
			StackPanel stack = new StackPanel { Orientation = Orientation.Horizontal};
			stack.Children.Add(_CheckBox);
			stack.Children.Add(_Label);
			Content = stack;
			Checkbox.Tag = this;    
        }     
    }
		public class ContentProperties
		{
			private XFrame _Frame;
			private XFrame _StopFrame;
			private List<XFrame> _StopFrames;
			public XFrame Frame { get { return _Frame; } set { _Frame = value; } }
			public XFrame StopFrame { get { return _StopFrame; } set { _StopFrame = value; } }
			public List<XFrame> StopFrames { get { return _StopFrames; } set { _StopFrames = value; } }
		}

		protected void content_Selected(object sender, RoutedEventArgs e)
		{
			show = true;
			if (File.Exists((sender as XContent).Path))
			{
				if (sender.GetType() == typeof(XImage))
				{
					currentImage = sender as XImage;
					if ((sender as XImage).IsChecked != false)
					{
						if (((sender as XImage).Parent as ListView) == backImageListView) { if (!addorselect) showImagePropsBackground(); }
						else if (((sender as XImage).Parent as ListView) == imageListView) { if (!addorselect) showImagePropsCharacter(sender as XImage); }
					}
				}
				else if (sender.GetType() == typeof(XAudio))
				{
					currentAudio = sender as XAudio;
					if ((sender as XAudio).IsChecked == true) if (!addorselect) getAudioProperties(sender as XAudio);
				}
				else if (sender.GetType() == typeof(XMovie)) currentMovie = sender as XMovie;
			}
		}
        

        private void contentCollector(string fromFile, string toFile)
        {
			if (File.Exists(toFile))
			{
				if (toFile != fromFile)
				{
					if (!Equals(File.ReadAllBytes(toFile), File.ReadAllBytes(fromFile)))
					{
						File.Delete(toFile);
						File.Copy(fromFile, toFile);						
					}
				}
			}
			else File.Copy(fromFile, toFile);
		}

		private void resourcesSelectedItem_delete()
		{
			media.IsExpanded=false;
			ListView selectedList = tabControlResources.SelectedContent as ListView;

			ArrayList elems = new ArrayList();
			foreach (XContent item in selectedList.SelectedItems)	 elems.Add(item);
			foreach (XContent item in elems)
			{
				File.Delete(item.Path);
				selectedList.Items.Remove(item);
			}				
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
