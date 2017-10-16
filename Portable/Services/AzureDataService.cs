using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dona.Forms.Model;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Plugin.Connectivity;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace dona.Forms.Services
{
    public interface IRemoteDataService
    {
        DateTime DateOfLastTimeInstitutionsUpdate { get; }

        Task Initialize();
        Task SyncInstitutions();
        IList<RemoteInstitution> GetInstitutions();
    }

    public class AzureDataService : IRemoteDataService
    {
        private const string DbPath = "syncstore.db";
        private IMobileServiceSyncTable<RemoteInstitution> _institutionsRemoteTable;
        private MobileServiceClient MobileService { get; set; }

        public static AzureDataService Instance = new AzureDataService();
        private static IList<RemoteInstitution> _remoteInstitutionsCache;

        private static ISettings AppSettings => CrossSettings.Current;

        public DateTime DateOfLastTimeInstitutionsUpdate
        {
            get => AppSettings.GetValueOrDefault("LastTimeInstitutionsUpdate", DateTime.MinValue);
            set => AppSettings.AddOrUpdateValue("LastTimeInstitutionsUpdate", value);
        }

        // Singleton
        private AzureDataService() { }

        public async Task Initialize()
        {
            if (MobileService?.SyncContext?.IsInitialized ?? false) return;

            // create the client
            MobileService = new MobileServiceClient("http://dona-uy.azurewebsites.net");

            // setup the local sqlite store and intialize the institutions table
            var store = new MobileServiceSQLiteStore(DbPath);
            store.DefineTable<RemoteInstitution>();
            await MobileService.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            // get the sync table that will call out to azure
            _institutionsRemoteTable = MobileService.GetSyncTable<RemoteInstitution>();
            _remoteInstitutionsCache = await _institutionsRemoteTable.ToListAsync();
        }

        public async Task SyncInstitutions()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                // pull down all latest changes
                await _institutionsRemoteTable.PullAsync("allInstitutions", _institutionsRemoteTable.IncludeTotalCount());
                _remoteInstitutionsCache = await _institutionsRemoteTable.ToListAsync();

                // save in the cross settings the last time of institutions were updated
                DateOfLastTimeInstitutionsUpdate = DateTime.UtcNow;
            }
        }

        public IList<RemoteInstitution> GetInstitutions()
        {
            return _remoteInstitutionsCache;
        }
    }
}
