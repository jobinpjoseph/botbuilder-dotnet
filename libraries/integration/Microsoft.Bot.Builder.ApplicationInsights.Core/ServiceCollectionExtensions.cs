﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Builder.ApplicationInsights;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Bot.Builder.ApplicationInsights.Core
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds and configures services for Application Insights to the <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> which specifies the contract for a collection of service descriptors.</param>
        /// <param name="botConfiguration">Bot configuration that contains the Application Insights configuration information.</param>
        /// <param name="telemetryClient">(Optional) The Application Insights client used for logging.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddBotApplicationInsights(this IServiceCollection services, BotConfiguration botConfiguration, ApplicationInsightsServiceOptions opts = null)
        {
            if (botConfiguration == null)
            {
                throw new ArgumentNullException(nameof(botConfiguration));
            }

            // Validate the bot file is correct.
            var appInsightsConfig = botConfiguration.Services.FirstOrDefault(s => s.Type == "appInsights");
            if (appInsightsConfig == null)
            {
                throw new InvalidOperationException("The .bot file is missing the Application Insights (appinsights) service.");
            }

            // Enables Bot Telemetry to save user/session id's as the bot user id and session
            services.AddMemoryCache();
            services.TryAddTransient<TelemetrySaveBodyASPMiddleware>();
            services.TryAddSingleton<ITelemetryInitializer>(new OperationCorrelationTelemetryInitializer());
            services.TryAddSingleton<ITelemetryInitializer, TelemetryBotIdInitializer>();
            services.TryAddSingleton<IBotTelemetryClient, BotTelemetryClient>();
            return services;
        }
    }
}
