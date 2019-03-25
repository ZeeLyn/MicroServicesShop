using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper.Extensions;
using Microsoft.Extensions.Hosting;

namespace Shop.OrderService
{
    public class InitDataService : IHostedService
    {
        private IDapper Dapper { get; }

        public InitDataService(IDapper dapper)
        {
            Dapper = dapper;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Dapper.ExecuteAsync(
                @"SET FOREIGN_KEY_CHECKS=0;
DROP TABLE IF EXISTS `Order`;CREATE TABLE `Order` (
  `OrderCode` varchar(255) DEFAULT NULL,
  `UserId` int(11) DEFAULT NULL,
  `PayCode` varchar(50) DEFAULT NULL,
  `Amount` decimal(10,0) DEFAULT NULL,
  `PayStatus` tinyint(4) DEFAULT NULL,
  `OrderStatus` tinyint(4) DEFAULT NULL,
  `CreatedOn` datetime DEFAULT NULL,
  `CompletedTime` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
DROP TABLE IF EXISTS `OrderDetail`;
CREATE TABLE `OrderDetail` (
  `OrderCode` varchar(255) DEFAULT NULL,
  `GoodsId` int(11) DEFAULT NULL,
  `Count` int(11) DEFAULT NULL,
  `Price` decimal(10,2) DEFAULT NULL,
  `Amount` decimal(10,2) DEFAULT NULL,
  `CreatedOn` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
            ");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
        
    }
}
