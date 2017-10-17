using System;
using System.Collections.Generic;
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
        Task<List<RemoteInstitution>> GetCachedInstitutionsAsync();
    }

    public class AzureDataService : IRemoteDataService
    {
        private const string DbPath = "syncstore.db";
        private const string AzurePath = "http://dona-uy.azurewebsites.net";

        private static IMobileServiceSyncTable<RemoteInstitution> _institutionsRemoteTable;
        private static MobileServiceClient MobileService { get; set; }
        private static ISettings AppSettings => CrossSettings.Current;

        public DateTime DateOfLastTimeInstitutionsUpdate
        {
            get => AppSettings.GetValueOrDefault("LastTimeInstitutionsUpdate", DateTime.MinValue);
            set => AppSettings.AddOrUpdateValue("LastTimeInstitutionsUpdate", value);
        }

        // Singleton implementation
        public static AzureDataService Instance = new AzureDataService();
        private AzureDataService() { }

        public async Task Initialize()
        {
            if (!MobileService?.SyncContext?.IsInitialized ?? true)
                MobileService = new MobileServiceClient(AzurePath);

            if (_institutionsRemoteTable == null)
            {
                // setup the local sqlite store and intialize the institutions table
                var store = new MobileServiceSQLiteStore(DbPath);
                store.DefineTable<RemoteInstitution>();
                await MobileService.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

                // get the sync table that will call out to azure
                _institutionsRemoteTable = MobileService.GetSyncTable<RemoteInstitution>();
            }
        }

        public async Task SyncInstitutions()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                if (_institutionsRemoteTable == null)
                    await Initialize();

                // pull down all latest changes
                await _institutionsRemoteTable.PullAsync("allInstitutions", _institutionsRemoteTable.IncludeTotalCount());

                // save in the cross settings the last time of institutions were updated
                DateOfLastTimeInstitutionsUpdate = DateTime.UtcNow;
            }
        }

        public async Task<List<RemoteInstitution>> GetCachedInstitutionsAsync()
        {
            if (_institutionsRemoteTable == null)
                await Initialize();

            return await _institutionsRemoteTable.ToListAsync();
        }
    }
}
