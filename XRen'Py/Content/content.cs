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
            private Label Label = new Label() { Margin = new Thickness(1, 0, 1, 0), Padding = new Thickness(1, 0, 1, 0) };
            public string Alias { get; set; }
            public string Path { get; set; }
            public bool? IsChecked { get { return Checkbox.IsChecked; } set { Checkbox.IsChecked = value; } }
            public CheckBox Checkbox { get; set; } = new CheckBox() { Margin = new Thickness(0), Padding = new Thickness(0), IsThreeState = true };
            public string Header { get { return Label.Content.ToString(); } set { Label.Content = value; Alias = value.ToLower().Substring(0, value.LastIndexOf('.')).Replace(" ","").Replace("-", ""); } }
		    public Brush TextColor { get { return Label.Foreground; } set { Label.Foreground = value; } }
		
		public XContent()
		{			
			StackPanel stack = new StackPanel { Orientation = Orientation.Horizontal};
			stack.Children.Add(Checkbox);
			stack.Children.Add(Label);
			Content = stack;
			Checkbox.Tag = this;    
        }     
    }
		public class ContentProperties
		{
            public XFrame Frame { get; set; }
            public XFrame StopFrame { get; set; }
            public List<XFrame> StopFrames { get; set; }
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
