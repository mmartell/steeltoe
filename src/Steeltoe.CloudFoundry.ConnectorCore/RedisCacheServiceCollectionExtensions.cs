﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Steeltoe.CloudFoundry.Connector.Services;
using System;
using System.IO;
using System.Reflection;

namespace Steeltoe.CloudFoundry.Connector.Redis
{
    public static class RedisCacheServiceCollectionExtensions
    {
        #region Microsoft.Extensions.Caching.Redis

        /// <summary>
        /// Add IDistributedCache to ServiceCollection
        /// </summary>
        /// <param name="services">Service collection to add to</param>
        /// <param name="config">App configuration</param>
        /// <param name="registerInterface">Optionally disable registering the interface type with DI</param>
        /// <param name="logFactory">logger factory</param>
        /// <returns>IServiceCollection for chaining</returns>
        public static IServiceCollection AddDistributedRedisCache(this IServiceCollection services, IConfiguration config, bool registerInterface = true, ILoggerFactory logFactory = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            return services.AddDistributedRedisCache(config, config, null);
        }

        /// <summary>
        /// Add IDistributedCache to ServiceCollection
        /// </summary>
        /// <param name="services">Service collection to add to</param>
        /// <param name="config">App configuration</param>
        /// <param name="serviceName">Name of service to add</param>
        /// <param name="registerInterface">Optionally disable registering the interface type with DI</param>
        /// <param name="logFactory">logger factory</param>
        /// <returns>IServiceCollection for chaining</returns>
        public static IServiceCollection AddDistributedRedisCache(this IServiceCollection services, IConfiguration config, string serviceName, bool registerInterface = true, ILoggerFactory logFactory = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            return services.AddDistributedRedisCache(config, config, serviceName);
        }

        /// <summary>
        /// Add IDistributedCache to ServiceCollection
        /// </summary>
        /// <param name="services">Service collection to add to</param>
        /// <param name="applicationConfiguration">App configuration</param>
        /// <param name="connectorConfiguration">Connector configuration</param>
        /// <param name="serviceName">Name of service to add</param>
        /// <param name="registerInterface">Optionally disable registering the interface type with DI</param>
        /// <param name="contextLifetime">Lifetime of the service to inject</param>
        /// <returns>IServiceCollection for chaining</returns>
        public static IServiceCollection AddDistributedRedisCache(this IServiceCollection services, IConfiguration applicationConfiguration, IConfiguration connectorConfiguration, string serviceName, bool registerInterface = true, ServiceLifetime contextLifetime = ServiceLifetime.Singleton)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (applicationConfiguration == null)
            {
                throw new ArgumentNullException(nameof(applicationConfiguration));
            }

            var configToConfigure = connectorConfiguration ?? applicationConfiguration;
            RedisServiceInfo info;
            if (serviceName != null)
            {
                info = configToConfigure.GetRequiredServiceInfo<RedisServiceInfo>(serviceName);
            }
            else
            {
                info = configToConfigure.GetSingletonServiceInfo<RedisServiceInfo>();
            }

            DoAddIDistributedCache(services, info, configToConfigure, registerInterface, contextLifetime);
            return services;
        }

        #endregion

        #region StackExchange.Redis

        /// <summary>
        /// Add Redis Connection Multiplexer to ServiceCollection
        /// </summary>
        /// <param name="services">Service collection to add to</param>
        /// <param name="config">App configuration</param>
        /// <param name="registerInterface">Optionally disable registering the interface type with DI</param>
        /// <returns>IServiceCollection for chaining</returns>
        public static IServiceCollection AddRedisConnectionMultiplexer(this IServiceCollection services, IConfiguration config, bool registerInterface = true)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            return services.AddRedisConnectionMultiplexer(config, config, null);
        }

        /// <summary>
        /// Add Redis Connection Multiplexer to ServiceCollection
        /// </summary>
        /// <param name="services">Service collection to add to</param>
        /// <param name="config">App configuration</param>
        /// <param name="serviceName">Name of service to add</param>
        /// <param name="registerInterface">Optionally disable registering the interface type with DI</param>
        /// <returns>IServiceCollection for chaining</returns>
        public static IServiceCollection AddRedisConnectionMultiplexer(this IServiceCollection services, IConfiguration config, string serviceName, bool registerInterface = true)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            return services.AddRedisConnectionMultiplexer(config, config, serviceName);
        }

        /// <summary>
        /// Add Redis Connection Multiplexer to ServiceCollection
        /// </summary>
        /// <param name="services">Service collection to add to</param>
        /// <param name="applicationConfiguration">App configuration</param>
        /// <param name="connectorConfiguration">Connector configuration</param>
        /// <param name="serviceName">Name of service to add</param>
        /// <param name="registerInterface">Optionally disable registering the interface type with DI</param>
        /// <param name="contextLifetime">Lifetime of the service to inject</param>
        /// <returns>IServiceCollection for chaining</returns>
        public static IServiceCollection AddRedisConnectionMultiplexer(this IServiceCollection services, IConfiguration applicationConfiguration, IConfiguration connectorConfiguration, string serviceName, bool registerInterface = true, ServiceLifetime contextLifetime = ServiceLifetime.Singleton)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (applicationConfiguration == null)
            {
                throw new ArgumentNullException(nameof(applicationConfiguration));
            }

            var configToConfigure = connectorConfiguration ?? applicationConfiguration;
            RedisServiceInfo info = serviceName == null ? configToConfigure.GetSingletonServiceInfo<RedisServiceInfo>() : configToConfigure.GetRequiredServiceInfo<RedisServiceInfo>(serviceName);
            DoAddConnectionMultiplexer(services, info, configToConfigure, registerInterface, contextLifetime);
            return services;
        }

        #endregion

        private static void DoAddIDistributedCache(IServiceCollection services, RedisServiceInfo info, IConfiguration config, bool registerInterface, ServiceLifetime contextLifetime)
        {
            string[] redisAssemblies = new string[] { "Microsoft.Extensions.Caching.Abstractions", "Microsoft.Extensions.Caching.Redis" };
            string[] redisInterfaceTypeNames = new string[] { "Microsoft.Extensions.Caching.Distributed.IDistributedCache" };
            string[] redisImplementationTypeNames = new string[] { "Microsoft.Extensions.Caching.Redis.RedisCache" };
            string[] redisOptionNames = new string[] { "Microsoft.Extensions.Caching.Redis.RedisCacheOptions" };

            Type redisInterface = ConnectorHelpers.FindType(redisAssemblies, redisInterfaceTypeNames);
            Type redisConnection = ConnectorHelpers.FindType(redisAssemblies, redisImplementationTypeNames);
            Type redisOptions = ConnectorHelpers.FindType(redisAssemblies, redisOptionNames);

            if (redisInterface == null || redisConnection == null || redisOptions == null)
            {
                throw new ConnectorException("Unable to find required Redis types, are you missing the Microsoft.Extensions.Caching.Redis Nuget package?");
            }

            RedisCacheConnectorOptions redisConfig = new RedisCacheConnectorOptions(config);
            RedisServiceConnectorFactory factory = new RedisServiceConnectorFactory(info, redisConfig, redisConnection, redisOptions, null);
            if (registerInterface)
            {
                services.Add(new ServiceDescriptor(redisInterface, factory.Create, contextLifetime));
            }

            services.Add(new ServiceDescriptor(redisConnection, factory.Create, contextLifetime));
        }

        private static void DoAddConnectionMultiplexer(IServiceCollection services, RedisServiceInfo info, IConfiguration config, bool registerInterface, ServiceLifetime contextLifetime)
        {
            string[] redisAssemblies = new string[] { "StackExchange.Redis", "StackExchange.Redis.StrongName" };
            string[] redisInterfaceTypeNames = new string[] { "StackExchange.Redis.IConnectionMultiplexer" };
            string[] redisImplementationTypeNames = new string[] { "StackExchange.Redis.ConnectionMultiplexer" };
            string[] redisOptionNames = new string[] { "StackExchange.Redis.ConfigurationOptions" };

            Type redisInterface = ConnectorHelpers.FindType(redisAssemblies, redisInterfaceTypeNames);
            Type redisImplementation = ConnectorHelpers.FindType(redisAssemblies, redisImplementationTypeNames);
            Type redisOptions = ConnectorHelpers.FindType(redisAssemblies, redisOptionNames);
            MethodInfo initializer = ConnectorHelpers.FindMethod(redisImplementation, "Connect", new Type[] { redisOptions, typeof(TextWriter) });

            if (redisInterface == null || redisImplementation == null || redisOptions == null || initializer == null)
            {
                throw new ConnectorException("Unable to find required Redis types, are you missing a StackExchange.Redis Nuget Package?");
            }

            RedisCacheConnectorOptions redisConfig = new RedisCacheConnectorOptions(config);
            RedisServiceConnectorFactory factory = new RedisServiceConnectorFactory(info, redisConfig, redisImplementation, redisOptions, initializer ?? null);
            if (registerInterface)
            {
                services.Add(new ServiceDescriptor(redisInterface, factory.Create, contextLifetime));
            }

            services.Add(new ServiceDescriptor(redisImplementation, factory.Create, contextLifetime));
        }
    }
}