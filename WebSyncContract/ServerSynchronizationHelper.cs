using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

using System.IO;

using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServerCe;
using Microsoft.Synchronization.Data.SqlServer;
using System.Configuration;

namespace WebSyncContract
{
    public class ServerSynchronizationHelper
    {

        /// <summary>
        /// Configure the SqlSyncprovider.  Note that this method assumes you have a direct conection
        /// to the server as this is more of a design time use case vs. runtime use case.  We think
        /// of provisioning the server as something that occurs before an application is deployed whereas
        /// provisioning the client is somethng that happens during runtime (on intitial sync) after the 
        /// application is deployed.
        ///  
        /// </summary>
        /// <param name="hostName"></param>
        /// <returns></returns>
        public SqlSyncProvider ConfigureSqlSyncProvider(string scopeName, string hostName)
        {
            SqlSyncProvider provider = new SqlSyncProvider();
            provider.ApplyChangeFailed += new EventHandler<DbApplyChangeFailedEventArgs>(Provider_ApplyingChanges);
            
            provider.ScopeName = scopeName;
            SqlConn conn = new SqlConn();
            provider.Connection = new SqlConnection(conn.connString);
            MakeBackUp();
            //create anew scope description and add the appropriate tables to this scope
            DbSyncScopeDescription scopeDesc = new DbSyncScopeDescription("CardsScope");

            //class to be used to provision the scope defined above
            SqlSyncScopeProvisioning serverConfig = new SqlSyncScopeProvisioning((SqlConnection)provider.Connection);

            //determine if this scope already exists on the server and if not go ahead and provision
            if (!serverConfig.ScopeExists("CardsScope"))
            {
                //add the approrpiate tables to this scope
                scopeDesc.Tables.Add(SqlSyncDescriptionBuilder.GetDescriptionForTable("[" + conn.schema + "].[cards]", (System.Data.SqlClient.SqlConnection)provider.Connection));
                //note that it is important to call this after the tables have been added to the scope
                serverConfig.PopulateFromScopeDescription(scopeDesc);
                //indicate that the base table already exists and does not need to be created
                serverConfig.SetCreateTableDefault(DbSyncCreationOption.Skip);
                //provision the server
                serverConfig.Apply();
            }
             conn.close();
            return provider;
        }



        private void Provider_ApplyingChanges(object sender, DbApplyChangeFailedEventArgs e)
        {
            if (e.Conflict.Type == DbConflictType.LocalUpdateRemoteUpdate)
            {
                int i = e.Conflict.LocalChange.Rows.Count ;
            }
            else if (e.Conflict.Type == DbConflictType.LocalCleanedupDeleteRemoteUpdate)
            {
                e.Action = ApplyAction.Continue;
            }
            else if (e.Conflict.Type == DbConflictType.LocalDeleteRemoteUpdate)
            {
                e.Action = ApplyAction.RetryWithForceWrite;
            }
            else if (e.Conflict.Type == DbConflictType.LocalInsertRemoteInsert)
            {
                e.Action = ApplyAction.Continue;
            }
            else if (e.Conflict.Type == DbConflictType.LocalUpdateRemoteDelete)
            {
                e.Action = ApplyAction.Continue;
            }
            else if (e.Conflict.Type == DbConflictType.ErrorsOccurred)
            {
                Console.WriteLine(e.Error.Message);
            }
            else
            {
                Console.WriteLine(e.Error.Message);
            }
        }

        private void MakeBackUp()
        {
            string backUpPath = ConfigurationManager.AppSettings.Get("backUpPath");
            if (backUpPath == null || backUpPath == "")
            {
                backUpPath = Directory.GetCurrentDirectory() + "\\BackUp\\";
            }
            SqlConn conn = new SqlConn();
            Directory.CreateDirectory(backUpPath);
            string[] files = Directory.GetFiles(@backUpPath, "*.Bak");
            conn.makeBackUp(backUpPath);
            conn.close();
        }
    }
}
