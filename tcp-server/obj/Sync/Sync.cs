
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServer;

namespace tcp_server
{
    class Sync
    {
        DbSyncProvider localSyncProvider;
        DbSyncProvider remoteSyncProvider;
        SyncOrchestrator collaborationSyncAgent;
        SyncOperationStatistics syncStatistics;
        public void sync(object obj)
        {
            SqlConnection conn0 = new SqlConnection("Data Source=DESKTOP-JS52HPM\\SQLEXPRESS;Initial Catalog=dbo;User ID=sa;Password=123456;");
            SqlConnection conn1 = new SqlConnection("Data Source=DESKTOP-JS52HPM\\SQLEXPRESS;Initial Catalog=dbo1;User ID=sa;Password=123456;");
            //provsisonDB(conn0);
            //provsisonDB(conn1);
            Synchronization(1,2);
        }
        void provsisonDB(SqlConnection serverConn)
        {
            DbSyncScopeDescription scopeDesc = new DbSyncScopeDescription("CardsScope");

            // get the description of the Products table from SyncDB dtabase
            DbSyncTableDescription tableDesc = SqlSyncDescriptionBuilder.GetDescriptionForTable("cards", serverConn);

            // add the table description to the sync scope definition
            scopeDesc.Tables.Add(tableDesc);

            // create a server scope provisioning object based on the ProductScope
            SqlSyncScopeProvisioning serverProvision = new SqlSyncScopeProvisioning(serverConn, scopeDesc);

            // skipping the creation of table since table already exists on server
            serverProvision.SetCreateTableDefault(DbSyncCreationOption.Skip);

            // start the provisioning process
            serverProvision.Apply();
        }

        public void Synchronization(int localPeerID, int remotePeerID)
        {
            try
            {
                localSyncProvider = SyncProviderHelper.CreateSyncProvider(localPeerID);
                localSyncProvider.SyncProviderPosition = SyncProviderPosition.Local;

                remoteSyncProvider = SyncProviderHelper.CreateSyncProvider(remotePeerID);
                remoteSyncProvider.SyncProviderPosition = SyncProviderPosition.Remote;
                collaborationSyncAgent = new SyncOrchestrator();
                collaborationSyncAgent.LocalProvider = localSyncProvider;
                collaborationSyncAgent.RemoteProvider = remoteSyncProvider;
                collaborationSyncAgent.Direction = SyncDirectionOrder.UploadAndDownload;
                syncStatistics = collaborationSyncAgent.Synchronize();
            }
            catch(Exception exc)
            {
                Console.WriteLine(exc.ToString());
            }
        }
    }
}