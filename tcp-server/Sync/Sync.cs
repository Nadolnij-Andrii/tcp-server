
using System;
using System.Data.SqlClient;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServer;
using NLog;
using WebSyncContract;
namespace tcp_server
{
    class Sync 
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        string connString;
        public Sync()
        {
            SqlConn conn = new SqlConn();
            connString = conn.connString;
            SqlConnection sqlConnection = new SqlConnection(connString);
            provsisonDB(sqlConnection);
        }
        void provsisonDB(SqlConnection clientConn)
        {
            try
            {
                DbSyncScopeDescription scopeDesc = new DbSyncScopeDescription("CardsScope");

                // get the description of the Products table from SyncDB dtabase
                DbSyncTableDescription tableDesc = SqlSyncDescriptionBuilder.GetDescriptionForTable("cards", clientConn);

                // add the table description to the sync scope definition
                scopeDesc.Tables.Add(tableDesc);

                // create a server scope provisioning object based on the ProductScope
                SqlSyncScopeProvisioning serverProvision = new SqlSyncScopeProvisioning(clientConn, scopeDesc);

                // skipping the creation of table since table already exists on server
                serverProvision.SetCreateTableDefault(DbSyncCreationOption.Skip);

                // start the provisioning process
                serverProvision.Apply();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
            }
        }
        //public void Synchronization(SqlConnection clientConn, SqlConnection serverConn)
        //{
        //    SqlSyncProvider sqlSyncProvider = new SqlSyncProvider("CardsScope",clientConn);
            
        //    SqlSyncProvider sqlSyncProvider1 = new SqlSyncProvider("CardsScope", serverConn);
        //    SyncOrchestrator syncOrchestrator = new SyncOrchestrator();
        //    syncOrchestrator.LocalProvider = sqlSyncProvider;
        //    syncOrchestrator.RemoteProvider = sqlSyncProvider1;
        //    syncOrchestrator.Direction = SyncDirectionOrder.UploadAndDownload;
        //    syncStatistics = syncOrchestrator.Synchronize();
        //    Console.WriteLine("Start Time: " + syncStatistics.SyncStartTime);
        //    Console.WriteLine("Total Changes Uploaded: " + syncStatistics.UploadChangesTotal);
        //    Console.WriteLine("Total Changes Downloaded: " + syncStatistics.DownloadChangesTotal);
        //    Console.WriteLine("Complete Time: " + syncStatistics.SyncEndTime);
        //    Console.WriteLine(String.Empty);
        //}
        public bool Synchronization(string syncString)
        {
            try
            {
                SqlSyncProvider srcProvider =new SqlSyncProvider("CardsScope",new SqlConnection(connString));
                SqlSyncProvider destinationProvider = new SqlSyncProvider("CardsScope", new SqlConnection(connString));

                string hostName = ((SqlConnection)destinationProvider.Connection).DataSource;
                SqlSyncProviderProxy destinationProxy = new SqlSyncProviderProxy(
                    "CardsScope", syncString);

                ////Set memory data cache size property. 0 represents non batched mode
                //srcProvider.MemoryDataCacheSize = this._batchSize;

                ////No need to set memory cache size for Proxy as since the source is enabled for batching, both upload and download will
                ////be batched. 

                ////Set batch spool location. Default value if not set is %Temp% directory.
                //if (!string.IsNullOrEmpty(this.batchSpoolLocation.Text))
                //{
                //    srcProvider.BatchingDirectory = this.batchSpoolLocation.Text;
                //    destinationProxy.BatchingDirectory = this.batchSpoolLocation.Text;
                //}
                //string remoteUri = @"http://localhost:8733/Design_Time_Addresses/RemoteProvider/RemotePeerSyncContract/";
                //string remoteUri = @"http://localhost:8733/Design_Time_Addresses/RemoteProvider/RemotePeerSyncContract/";
                SynchronizationHelper synchronizationHelper = new SynchronizationHelper() ;
                SyncOperationStatistics stats = synchronizationHelper.SynchronizeProviders(srcProvider, destinationProxy);
                Console.WriteLine("Start synchronization session with server: {0}", syncString);
                logger.Info("Start synchronization session with server: {0}", syncString);
                Console.WriteLine("Start Time: " + stats.SyncStartTime);
                logger.Info("Start Time: " + stats.SyncStartTime);
                Console.WriteLine("Total Changes Uploaded: " + stats.UploadChangesTotal);
                logger.Info("Total Changes Uploaded: " + stats.UploadChangesTotal);
                Console.WriteLine("Total Changes Downloaded: " + stats.DownloadChangesTotal);
                logger.Info("Total Changes Downloaded: " + stats.DownloadChangesTotal);
                Console.WriteLine("Complete Time: " + stats.SyncEndTime);
                logger.Info("Complete Time: " + stats.SyncEndTime);
                Console.WriteLine("End synchronization session.");
                logger.Info("End synchronization session.");
                Console.WriteLine(String.Empty);
                return true;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return false;
            }
        }
        //private void Synchronize(int localPeerID, int remotePeerID)
        //{
        //    //try
        //    //{
        //    //    localSyncProvider = SyncProviderHelper.CreateSyncProvider(localPeerID);
        //    //    localSyncProvider.SyncProviderPosition = SyncProviderPosition.Local;


        //    //    remoteSyncProvider = SyncProviderHelper.CreateSyncProvider(remotePeerID);
        //    //    remoteSyncProvider.SyncProviderPosition = SyncProviderPosition.Remote;


        //    //    collaborationSyncAgent = new SyncOrchestrator();
        //    //    collaborationSyncAgent.LocalProvider = localSyncProvider;
        //    //    collaborationSyncAgent.RemoteProvider = remoteSyncProvider;
        //    //    collaborationSyncAgent.Direction = SyncDirectionOrder.UploadAndDownload;
        //    //    syncStatistics = collaborationSyncAgent.Synchronize();

        //    //    Console.WriteLine("Start Time: " + syncStatistics.SyncStartTime);
        //    //    Console.WriteLine("Total Changes Uploaded: " + syncStatistics.UploadChangesTotal);
        //    //    Console.WriteLine("Total Changes Downloaded: " + syncStatistics.DownloadChangesTotal);
        //    //    Console.WriteLine("Complete Time: " + syncStatistics.SyncEndTime);
        //    //    Console.WriteLine(String.Empty);

        //    //}
        //    //catch (DbOutdatedSyncException exc)
        //    //{
        //    //    throw new Exception("Peer Database is outdated.Please run the  CleanData.sql script in SQL or execute Clean Data Command from UI" + exc.ToString());
        //    //}
        //    //catch (Exception exc)
        //    //{
        //    //    Console.WriteLine(exc.ToString());
        //    //}

        //}
    }
}