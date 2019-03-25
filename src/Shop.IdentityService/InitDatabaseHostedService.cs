using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper.Extensions;
using Microsoft.Extensions.Hosting;

namespace Shop.IdentityService
{
    public class InitDatabaseHostedService : IHostedService
    {
        private IDapper Dapper { get; }

        public InitDatabaseHostedService(IDapper dapper)
        {
            Dapper = dapper;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Dapper.ExecuteAsync(@"SET sql_mode=(SELECT REPLACE(@@sql_mode,'ONLY_FULL_GROUP_BY',''));
CREATE TABLE IF NOT EXISTS `Users1` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `email` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL,
  `nickname` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `email` (`email`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
