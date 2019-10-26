using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;


namespace KegMaster.Core.Database
{
    public class ConnectionManager
    {
        static ConnectionManager defaultInstance = new ConnectionManager();
        MobileServiceClient client;

        IMobileServiceTable<KegItem> kegTable;
        /* TODO
         * IMobileServiceTable<UserItem> userTable;
         * IMobileServiceTable<PourItem> PourTable;
         * IMobileServiceTable<ReviewItem> reviewTable;
         */

        /*
         * Constructor
         */
        public ConnectionManager()
        {
            client = new MobileServiceClient(Constants.ApplicationURL);
            //kegTable. = client;

            this.kegTable = client.GetTable<KegItem>();

            this.client = new MobileServiceClient(Constants.ApplicationURL);            var path = Path.Combine(MobileServiceClient.DefaultDatabasePath, "KegItem.dbo");            var store = new MobileServiceSQLiteStore(path);

            //store.DefineTable

            this.client.SyncContext.InitializeAsync(store);            //kegTable = this.client.GetSyncTable<KegItem>();
        }

        /*
         * Get/Set connection manager instance
         */
        public static ConnectionManager DefaultManager
        {
            get
            {
                return defaultInstance;
            }
            private set
            {
                defaultInstance = value;
            }
        }

        /*
         * Get Client instance
         */
        public MobileServiceClient CurrentClient
        {
            get { return client; }
        }

        /*
         * Async Query for Active KegItems
         */
        public async Task<KegItem> GetActiveKegAsync(int tapNo = 0)
        {
            try
            {
				//TODO: Find more elegant way of doing this
				IEnumerable<KegItem> item = await kegTable.Take(1).Where(tap => tap.TapNo == tapNo).ToListAsync();
                return new ObservableCollection<KegItem>(item)[0];
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine("Invalid sync operation: {0}", new[] { msioe.Message });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Sync error: {0}", new[] { e.Message });
            }
            return null;
        }

        /*
         * Async - Query for Active KegItems
         */
        public async Task<ObservableCollection<KegItem>> GetHistoricKegsAsync(bool syncItems = false)
        {
            try
            {
                IEnumerable<KegItem> items = await kegTable
                    // TODO: Filter active kegs
                    //.Where(kegItem => !kegItem.Done)
                    .ToEnumerableAsync();

                return new ObservableCollection<KegItem>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine("Invalid sync operation: {0}", new[] { msioe.Message });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Sync error: {0}", new[] { e.Message });
            }
            return null;
        }

        /*
         * Async - Save KegItem. Create a new entry if needed.
         */
        public async Task SaveKegAsync(KegItem item)
        {
            try
            {
                if (item.Id == null)
                {
                    await this.kegTable.InsertAsync(item);
                }
                else
                {
                    await this.kegTable.UpdateAsync(item);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Save error: {0}", new[] { e.Message });
            }
        }

    }


}
