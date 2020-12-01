using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
using System.Data.SqlClient;

namespace tcp_server
{
    /// <summary>
    /// Contains Static Method to Create Sync Adapter
    /// </summary>
    public class SyncProviderHelper
    {

        public static DbSyncProvider CreateSyncProvider(int peerID)
        {
            DbSyncProvider syncProvider = new DbSyncProvider();
            syncProvider.ScopeName = "CardsScope";
            syncProvider.Connection = new SqlConnection(DBHelper.GetPeerConncetionStringByPeerID(peerID));

            syncProvider.SyncAdapters.Add(SyncAdapterHelper.CreateSyncAdpters());

            syncProvider.SelectNewTimestampCommand = SyncProviderCommandHelper.GetSelectNewTimestampCommand();

            syncProvider.SelectScopeInfoCommand = SyncProviderCommandHelper.GetSelectScopeInfoCommand();
            syncProvider.UpdateScopeInfoCommand = SyncProviderCommandHelper.GetUpdateScopeInfoCommand();

            return syncProvider;
        }
    }
}
