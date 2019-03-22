using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Extensions;
using Dapper.Extensions.MySql;
using Microsoft.Extensions.DependencyInjection;

namespace Shop.OrderService
{
    public class SeedData
    {
        private static IServiceProvider Services { get; set; }

        public static void Initialize(IServiceCollection serviceCollection)
        {
            Services = serviceCollection.BuildServiceProvider();
            var dapper = Services.GetRequiredService<IDapper>();
            dapper.ExecuteAsync(
                @"SET FOREIGN_KEY_CHECKS=0;
DROP TABLE IF EXISTS `Order`;CREATE TABLE `Order` (
  `OrderCode` varchar(255) DEFAULT NULL,
  `UserId` varchar(50) DEFAULT NULL,
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
            ").Wait();
        }
    }
}
