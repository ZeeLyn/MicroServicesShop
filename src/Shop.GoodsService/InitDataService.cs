using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper.Extensions;
using Microsoft.Extensions.Hosting;

namespace Shop.GoodsService
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
                @"SET FOREIGN_KEY_CHECKS=0;DROP TABLE IF EXISTS `Category`;CREATE TABLE `Category` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) DEFAULT NULL,
  PRIMARY KEY(`Id`)
) ENGINE = InnoDB AUTO_INCREMENT = 4 DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci;
            INSERT INTO `Category` VALUES('1', 'Book');
            INSERT INTO `Category` VALUES('2', 'Clothes');
            INSERT INTO `Category` VALUES('3', 'Home Appliances');
            DROP TABLE IF EXISTS `Goods`;
            CREATE TABLE `Goods` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varchar(255) DEFAULT NULL,
  `Pic` varchar(255) DEFAULT NULL,
  `Price` decimal(10, 2) DEFAULT NULL,
   `CategoryId` int(11) DEFAULT NULL,
   `Description` text,
  `CreatedTime` datetime DEFAULT NULL,
  `Stock` int(11) DEFAULT '0',
  PRIMARY KEY(`Id`)
) ENGINE = InnoDB AUTO_INCREMENT = 8 DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci;
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

            INSERT INTO `Goods` VALUES('1', 'Three Days to See', '/image/1854764794af65d7.jpg', '17.80', '1', 'All of us have read thrilling stories in which the hero had only a limited and specified time to live. Sometimes it was as long as a year; sometimes as short as twenty-four hours. But always we were interested in discovering just how the doomed man chose to spend his last days or his last hours. I speak, of course, of free men who have a choice, not condemned criminals whose sphere of activities is strictly delimited.', '2019-03-06 16:53:27', '20');
            INSERT INTO `Goods` VALUES('2', 'Pride And Prejudice', '/image/85a0d3fce746433d.jpg', '34.80', '1', 'IT is a truth universally acknowledged, that a single man in possession of a good fortune must be in want of a wife. \r\nHowever little known the feelings or views of such a man may be on his first entering a neighbourhood, this truth is so well fixed in the minds of the surrounding families, that he is considered as the rightful property of some one or other of their daughters.', '2019-03-06 16:55:17', '30');
            INSERT INTO `Goods` VALUES('3', 'T shirt', '/image/5ac8ab16Nbdd8334e.jpg', '56.00', '2', 'T-shirt, also known as T-shirt, originated in the United States; because of the variety of styles and applies to men, women and children, it is quickly becoming popular all over the world.', '2019-03-06 16:56:58', '50');
            INSERT INTO `Goods` VALUES('4', 'Shirt', '/image/e38f791e23424ff7.jpg', '100.00', '2', 'A shirt is a top that can be worn between the inner and outer tops and can also be worn separately. China’s Zhou Dynasty has a shirt, called the middle, and later called the middle. In the Han Dynasty, the close-fitting shirt was called the toilet. The name of the shirt has been used in the Song Dynasty. Now known as Chinese shirts. The 18th century BC, the 18th dynasty of ancient Egypt has a shirt, a collarless, sleeveless corset', '2019-03-06 16:58:11', '40');
            INSERT INTO `Goods` VALUES('5', 'Washing Machine', '/image/d5f66a3171d06a91.jpg', '500.00', '3', 'The washing machine is a cleaning appliance that uses mechanical energy to generate mechanical action to wash clothes, and is classified into household and collective types according to its rated washing capacity.', '2019-03-06 16:59:09', '100');
            INSERT INTO `Goods` VALUES('6', 'Hisense H55E3A', '/image/15c1cb95a141ad81.jpg', '400.00', '3', 'Short for color TVs Instantly transmit moving visual images using electricity. Similar to the movie, the television uses the visual residual effect of the human eye to visualize a still image of a frame of gradation, forming a visual moving image.', '2019-03-06 17:01:53', '120');
            ");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
