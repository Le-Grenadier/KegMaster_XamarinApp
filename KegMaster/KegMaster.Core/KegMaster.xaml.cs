using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using KegMaster.Core.Database;
using System.Net.Http;
using System.Net;
using System.IO;

namespace KegMaster.Core
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class KegMaster : TabbedPage
    {
        private HttpClient _client;
        private const string FunctionURL = Constants.FunctionURL;

        ConnectionManager manager;
        ObservableCollection<KegItem> kegs;
        KegItem updatedKeg;
        string rowItem; /* Format = """ReqId:"", TapNo:{n}, qrySelect:{json_key}'""" */
        int tabIdx;
        int kegUpdateDelay, itemUpdateDelay;

        public KegMaster()
        {
            InitializeComponent();

            manager = ConnectionManager.DefaultManager;
            /* Meter rate of Db access */
            MessagingCenter.Subscribe<ContentPage, KegItem>(this, "KegItem_KegUpdate", async (sender, arg) => { updatedKeg = arg; kegUpdateDelay = 10; });
            MessagingCenter.Subscribe<ContentPage, string>(this, "KegItem_PushEmbeddedUpdate", async (sender, arg) => { rowItem = arg; itemUpdateDelay = 10; });
            Device.StartTimer(TimeSpan.FromSeconds(1), () => { DataSyncBackground(); return true; });// return true to repeat counting, false to stop timer

        }

        /*
         * Init Keg data
         */
        protected override async void OnAppearing()
        {
            await DataFetch(syncKegs: true);
            base.OnAppearing();
        }


        /*
         * Send data to tab when opened
         */
        protected override async void OnCurrentPageChanged()//(TabbedPage tab)
        {
            tabIdx = this.CurrentPage.TabIndex;
            await DataNotifyTab(tabIdx);
        }

        /*
         * Pass database data between tabs
         */
        protected override void OnTabIndexPropertyChanged(int oldValue, int newValue)
        {
            base.OnTabIndexPropertyChanged(oldValue, newValue);
        }

        /*
         * Send data to server and initiate refresh
         */
        private async Task DataSyncBackground()
        {
            Exception error = null;

            itemUpdateDelay--;
            if (0 == itemUpdateDelay && rowItem != null)
            {
                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(FunctionURL);
                    req.Method = "POST";
                    req.ContentType = "application/json";
                    Stream stream = req.GetRequestStream();
                    string json = string.Format("{{ReqId:\"\", {0}}}", rowItem);
                    byte[] buffer = Encoding.UTF8.GetBytes(json);
                    stream.Write(buffer, 0, buffer.Length);
                    HttpWebResponse result = (HttpWebResponse)req.GetResponse();
                }
                catch(Exception ex)
                {
                    error = ex;
                }
                if (error != null)
                {
                    await DisplayAlert("There was an error", error.Message, "OK");
                }
            }

            kegUpdateDelay--;
            if (0 == kegUpdateDelay && updatedKeg != null)
            {
                var item = kegs.FirstOrDefault(i => i.TapNo == this.tabIdx);
                if (item != null)
                {
                    item = updatedKeg;
                }
                await DataNotifyTab(this.tabIdx);

                await this.DataFetch(syncKegs: true);
            }

        }

        /*
         * Fetch Data from Server
         */
        async Task DataFetch(bool syncKegs)
        {
            this.kegs = await manager.GetActiveKegsAsync(syncKegs);
            await DataNotifyTab(this.tabIdx);
        }

        /*
         * Push Data to Server
         */
        async Task DataPush(KegItem item)
        {
            await manager.SaveKegAsync(item);
            
        }

        /*
         * Send Data to acrive Tab
         */
        async Task DataNotifyTab(int idx)
        {
            IEnumerable<KegItem> items;

            /* Assumes most recent entry processed first */
            items = null == kegs  ? null : kegs.Where(kegItem => kegItem.TapNo == idx);
            if (null == items)
            {
                var e = new KegItem();
                e.TapNo = this.tabIdx;
                items = new[] { e };
            }
            MessagingCenter.Send<KegMaster, KegItem>(this, "KegItem_TabContext", items.ElementAtOrDefault(0));

        }

        private HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient();
                }

                return _client;
            }
        }
    }
}
