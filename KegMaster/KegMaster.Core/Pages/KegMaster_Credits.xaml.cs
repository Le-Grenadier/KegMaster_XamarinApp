using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KegMaster.Core.Pages
{
	public class CreditType
	{
		string name;
		string title;
		public CreditType(string t, string n) { Title = t; Name = n; }
		public string Title { get { return title; } set { title = value; } }
		public string Name { get { return name; } set { name = value; } }
	}

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class KegMaster_Credits : ContentPage
	{
		public ObservableCollection<CreditType> Credits { get; set; }

		public KegMaster_Credits()
		{
			InitializeComponent();

			Credits = new ObservableCollection<CreditType>();
			Credits.Add(new CreditType("Software Engineer", "John Grenard"));
			Credits.Add(new CreditType("Design Engineer",   "John Grenard"));
			Credits.Add(new CreditType("Kind Words and Support", "Rachel Williams"));
		    Credits.Add(new CreditType("Background Photo", "Timothy Dykes on Unsplash"));

			MyListView.ItemsSource = Credits;
		}
	}
}
