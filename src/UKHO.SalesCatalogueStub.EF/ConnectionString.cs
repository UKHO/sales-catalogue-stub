﻿using System.Data.SqlClient;

namespace UKHO.SalesCatalogueStub.Api.EF
{
    /// <summary>
    /// 
    /// </summary>
    public static class ConnectionString
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="initialCatalog"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string
            Build(string dataSource, string initialCatalog = "", string userId = "", string password = "") =>
            new SqlConnectionStringBuilder
            {
                DataSource = dataSource,
                InitialCatalog = initialCatalog,
                IntegratedSecurity = false,
                MultipleActiveResultSets = true,
                Encrypt = true,
                ConnectTimeout = 20,
                UserID = userId,
                Password = password
            }.ToString();

    }
}
