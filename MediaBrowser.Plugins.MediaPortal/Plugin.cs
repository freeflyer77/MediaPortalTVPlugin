﻿using System;
using System.Collections.Generic;

using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Common.Plugins;

using MediaBrowser.Controller;

using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Plugins.MediaPortal.Configuration;
using MediaBrowser.Plugins.MediaPortal.Helpers;
using MediaBrowser.Plugins.MediaPortal.Interfaces;
using MediaBrowser.Plugins.MediaPortal.Services.Proxies;
using System.IO;
using MediaBrowser.Model.Drawing;

namespace MediaBrowser.Plugins.MediaPortal
{
    /// <summary>
    /// Class Plugin
    /// </summary>
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages, IHasThumbImage
    {
        public static TvServiceProxy TvProxy { get; private set; }
        public static StreamingServiceProxy StreamingProxy { get; private set; }

        public static IPluginLogger Logger { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin" /> class.
        /// </summary>
        /// <param name="applicationPaths">The application paths.</param>
        /// <param name="xmlSerializer">The XML serializer.</param>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="jsonSerializer">The json serializer.</param>
        /// <param name="networkManager">The network manager.</param>
        /// <param name="logger">The logger.</param>
        public Plugin(
            IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, IHttpClient httpClient,
            IJsonSerializer jsonSerializer, INetworkManager networkManager, ILogger logger, TmdbLookup tmdbLookup)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;

            Logger = new PluginLogger(logger);

            // Create our shared service proxies
            StreamingProxy = new StreamingServiceProxy(httpClient, jsonSerializer, networkManager);
            TvProxy = new TvServiceProxy(httpClient, jsonSerializer, StreamingProxy, tmdbLookup);
        }

        /// <summary>
        /// Gets the name of the plugin
        /// </summary>
        /// <value>The name.</value>
        public override string Name
        {
            get { return "MediaPortal TV Plugin"; }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get
            {
                return "MediaPortal TV Plugin to enable Live TV streaming and scheduling.";
            }
        }

        public Stream GetThumbImage()
        {
            var type = GetType();
            return type.Assembly.GetManifestResourceStream(type.Namespace + ".thumb.png");
        }

        public ImageFormat ThumbImageFormat
        {
            get
            {
                return ImageFormat.Png;
            }
        }

        private Guid _id = new Guid("2c6a0219-7621-4b06-8a64-da3f7038b649");
        public override Guid Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static Plugin Instance { get; private set; }

        /// <summary>
        /// Holds our registration information
        /// </summary>
        public MBRegistrationRecord Registration { get; set; }

        /// <summary>
        /// Updates the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public override void UpdateConfiguration(BasePluginConfiguration configuration)
        {
            var oldConfig = Configuration;

            base.UpdateConfiguration(configuration);

            ServerEntryPoint.Instance.OnConfigurationUpdated(oldConfig, (PluginConfiguration)configuration);
        }

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = "MediaPortal",
                    EmbeddedResourcePath = "MediaBrowser.Plugins.MediaPortal.Configuration.configPage.html"
                }
            };
        }
    }
}