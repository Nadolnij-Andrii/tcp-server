using System;
using System.Collections.Generic;
using System.Text;



using System.ServiceModel;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServerCe;
using Microsoft.Synchronization.Data.SqlServer;

namespace WebSyncContract
{
    public class SyncProviderConfigurationUtilities
    {
        public static SqlSyncProvider ConfigureSqlSyncProvider(string scopeName)
        {
            SqlSyncProvider provider = new SqlSyncProvider();
           // provider.ApplyingChanges += Provider_ApplyingChanges;
            provider.ScopeName = scopeName;
           // ApplyAction applyAction += EventHandler<ApplyChangeFailedEventArgs>(SampleClientSyncProvider_ApplyChangeFailed);
            //Service should know list of adapters for given scope name.
            //Sample only shows for 'Sales' scope
            switch (scopeName.ToLower())
            {
                case "СardsScope":

                    break;
                default:
                    throw new FaultException<WebSyncFaultException>(new WebSyncFaultException("Invalid SQL Scope name", null));
            }

            return provider;
        }

        private static void Provider_ApplyingChanges(object sender, DbApplyingChangesEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
