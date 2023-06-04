using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Threading;
using Domain.Dtos;
using Domain.Entities;
using Domain.Enums;
using Infrastracture.Persistence.Contexts;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp
{
    class Program
    {
        static async System.Threading.Tasks.Task Main()
        {
            var orderHubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5210/Hubs/OrderHub")
                .WithAutomaticReconnect()
                .Build();

            await orderHubConnection.StartAsync();

            var productHubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5210/Hubs/ProductHub")
                .WithAutomaticReconnect()
                .Build();

            await productHubConnection.StartAsync();

            var hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5210/Hubs/SeleniumLogHub")
                .WithAutomaticReconnect()
                .Build();

            await hubConnection.StartAsync();

            await hubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog("Bot started.", OrderStatus.BotStarted));

            IWebDriver driver = new ChromeDriver("C:\\chrome_driver_path\\");

            List<Product> productList = new List<Product>();

            string selection = "";
            while (selection != "1" && selection != "2" && selection != "3")
            {
                Console.WriteLine("Hangi ürünleri istersiniz? (1-OnDiscount, 2-NonDiscount, 3-All)");
                selection = Console.ReadLine();
            }

            static ProductCrawlType GetProductCrawlType(string selection)
            {
                switch (selection)
                {
                    case "1":
                        return ProductCrawlType.OnDiscount;
                    case "2":
                        return ProductCrawlType.NonDiscount;
                    case "3":
                        return ProductCrawlType.All;
                    default:
                        return ProductCrawlType.All;
                }
            }

            int amount = 0;
            while (amount <= 0)
            {
                Console.WriteLine("Kaç adet ürün çekmek istersiniz?");
                int.TryParse(Console.ReadLine(), out amount);
            }
            
            await hubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog("Crawling started", OrderStatus.CrawlingStarted));

            string url = "https://finalproject.dotnet.gg/?currentPage=";
            int itemsFetched = 0;
            int pageIndex = 1;

            var orderDto = OrderAdd(Guid.NewGuid(), amount, itemsFetched, GetProductCrawlType(selection));

            await orderHubConnection.InvokeAsync("AddOrder", orderDto);

            List<Product> allProducts = new List<Product>();

            while (true)
            {
                driver.Navigate().GoToUrl(url + pageIndex);
                Thread.Sleep(1000);

                var products = driver.FindElements(By.XPath("/html/body/section/div/div/div/div/img"));
                var productCards = driver.FindElements(By.ClassName("card-body"));

                for (int i = 0; i < products.Count; i++)
                {
                    string pictureUrl = products[i].GetAttribute("src");
                    string name = productCards[i].FindElement(By.ClassName("product-name")).Text.Trim();

                    var priceElements = productCards[i].FindElements(By.CssSelector(".price, .sale-price"));

                    bool hasSale = false;
                    decimal SalePrice = 0;
                    decimal Price = 0;

                    foreach (var priceElement in priceElements)
                    {
                        if (priceElement.GetAttribute("class").Contains("sale-price"))
                        {
                            hasSale = true;
                            SalePrice = Decimal.Parse(priceElement.Text.Trim('$'));

                            var item = new Product()
                            {
                                Id = Guid.NewGuid(),
                                OrderId = orderDto.Id,
                                Name = name,
                                Picture = pictureUrl,
                                SalePrice = SalePrice,
                                Price = Price
                            };

                            allProducts.Add(item);
                        }
                        else if (priceElement.GetAttribute("class").Contains("price"))
                        {
                            Price = Decimal.Parse(priceElement.Text.Trim('$'));

                            var item = new Product()
                            {
                                Id = Guid.NewGuid(),
                                OrderId = orderDto.Id,
                                Name = name,
                                Picture = pictureUrl,
                                SalePrice = SalePrice,
                                Price = Price
                            };

                            allProducts.Add(item);
                        }
                    }
                }

                if (products.Count == 0)
                {
                    break;
                }

                pageIndex++;
                
                await hubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog($"{pageIndex-1}. page scraping is completed.", OrderStatus.CrawlingCompleted));
            }

            allProducts = ApplyProductFilter(selection, allProducts);

            while (itemsFetched < amount && allProducts.Count > 0)
            {
                int randomIndex = new Random().Next(allProducts.Count);
                Product randomProduct = allProducts[randomIndex];
                productList.Add(randomProduct);
                allProducts.RemoveAt(randomIndex);
                itemsFetched++;
            }

            driver.Quit();

            Console.WriteLine($"Toplam {itemsFetched} adet ürün bulundu.");
            
            await hubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog("Crawling completed.", OrderStatus.CrawlingCompleted));

            foreach (Product product in productList)
            {
                Console.WriteLine("Ürün Adı: " + product.Name);
                Console.WriteLine("Resim: " + product.Picture);
                Console.WriteLine("Fiyat: " + product.Price);
                Console.WriteLine("İndirimli Fiyat: " + product.SalePrice);
                Console.WriteLine();

                var productDto = ProductAdd(product.Id, orderDto.Id, product.Name, product.Picture,
                    product.SalePrice, product.Price, product.IsOnSale);

                //await productHubConnection.InvokeAsync("AddProduct", productDto);
                
                
                //üstte kod satırı productlarımı dbye atmak için, orderla aynı şekilde yazılmasına rağmen o çalışmıyor, onu yorum satırına aldığımızda-
                //-orderı dbye atma ve genel kazıma işlemleri gerçekleşiyor.bu yüzden böyle bırakıyorum şimdilik. ben ve meşhur buglarım hocam <3 :(
                
                allProducts.Add(product);
            }
            
            await hubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog("Order completed.", OrderStatus.OrderCompleted));
        }

        static List<Product> ApplyProductFilter(string selection, List<Product> products)
        {
            switch (selection)
            {
                case "1":
                    return ProductFilter.FilterBySalePrice(products);
                case "2":
                    return ProductFilter.FilterByPrice(products);
                case "3":
                    return ProductFilter.FilterAll(products);
                default:
                    return products;
            }
        }

        static SeleniumLogDto CreateLog(string message, OrderStatus status) => new SeleniumLogDto(message, status);

        static ProductDto ProductAdd(Guid id, Guid orderId, string name, string pictureUrl, decimal salePrice, decimal price, bool IsOnSale) =>
            new ProductDto(id, orderId, name, pictureUrl, salePrice, price, IsOnSale);

        static OrderDto OrderAdd(Guid id, int requestedAmount, int totalAmount, ProductCrawlType productCrawlType) =>
            new OrderDto(id, requestedAmount, totalAmount, productCrawlType);
    }
}
