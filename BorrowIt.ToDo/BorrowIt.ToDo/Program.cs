﻿using System;
using System.IO;
using System.Threading;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Exceptions;

namespace BorrowIt.ToDo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var failuresCount = 0;
            while (failuresCount <= 10)
            {
                try
                {
                    var host = Host.CreateDefaultBuilder(args)
                        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                        .ConfigureWebHostDefaults(webHostBuilder => {
                            webHostBuilder
                                .UseContentRoot(Directory.GetCurrentDirectory())
                                .UseIISIntegration()
                                .UseStartup<Startup>();
                        })
                        .Build();
                    host.Run();
                    return;
                }
                catch (Exception e)
                {
                    if (!e.Source.Contains("RabbitMQ"))
                    {
                        throw;
                    }
                    failuresCount++;
                    Thread.Sleep(30000);
                }
            }
        }
    }
}