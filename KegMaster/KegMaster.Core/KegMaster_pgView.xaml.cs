using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using KegMaster.Core.Database;
using System.Collections.Generic;
using KegMaster.Core.Pages.ManageKegs_Views;

namespace KegMaster.Core
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class KegMaster_pgView : ContentPage
    {
		ObservableCollection<KegItem> kegs = new ObservableCollection<KegItem>();
		ConnectionManager manager;
		bool isBusy;

		/* Constructor assumes kegs exist for each index up to n */
		public KegMaster_pgView()
        {

			InitializeComponent();

			MessagingCenter.Subscribe<TapEdit, KegItem>(this, "KegItem_Updated", (sender, arg) => {
				if (arg.TapNo > kegs.Count) {
					kegs.Add(arg);
				} else {
					kegs.RemoveAt(arg.TapNo);
					kegs.Insert(arg.TapNo, arg);
				}
			});

			DisplayLoading.IsVisible = true;

			MyListView.ItemsSource = kegs;
			MyListView.IsVisible = false;
			manager = ConnectionManager.DefaultManager;
			Task.Run(async() => await loadKegsBackground());
        }

		async Task loadKegsBackground()
		{
			int idx = 0;
			var keg = await getKeg(idx);
			while (keg != null) {
				kegs.Add(keg);
				OnPropertyChanged();

				idx++;
				keg = await getKeg(idx);
			}

			DisplayLoading.IsVisible = false;
			MyListView.IsVisible = true;

		}

		/*----------------------------------------------------------------------
         Get all kegs that have been created, no defined limit currently exists
        ----------------------------------------------------------------------*/
		async Task<KegItem> getKeg(int idx)
        {
			var keg = await manager.GetActiveKegAsync(idx);
            return (keg);
        }

        /*----------------------------------------------------------------------
        Set callback for when on-screen item has been selected
        ----------------------------------------------------------------------*/
        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await Navigation.PushAsync(new Pages.ManageKegs_Views.TapEdit());
            MessagingCenter.Send<KegMaster_pgView, KegItem>(this, "KegItem_EditContext", (Database.KegItem)e.Item);

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
