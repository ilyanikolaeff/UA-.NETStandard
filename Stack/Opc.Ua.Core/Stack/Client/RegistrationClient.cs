/* Copyright (c) 1996-2016, OPC Foundation. All rights reserved.
   The source code in this file is covered under a dual-license scenario:
     - RCL: for OPC Foundation members in good-standing
     - GPL V2: everybody else
   RCL license terms accompanied with this source code. See http://opcfoundation.org/License/RCL/1.00/
   GNU General Public License as published by the Free Software Foundation;
   version 2 of the License are accompanied with this source code. See http://opcfoundation.org/License/GPLv2
   This source code is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
*/

using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;

namespace Opc.Ua
{
    /// <summary>
    /// An object used by clients to access a UA discovery service.
    /// </summary>
    public partial class RegistrationClient
    {
        #region Constructors

        /// <summary>
        /// Creates a binding for to use for discovering servers.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="description">The description.</param>
        /// <param name="endpointConfiguration">The endpoint configuration.</param>
        /// <param name="instanceCertificate">The instance certificate.</param>
        /// <returns></returns>
        public static RegistrationClient Create( 
            ApplicationConfiguration configuration,
            EndpointDescription      description,
            EndpointConfiguration    endpointConfiguration,
            X509Certificate2         instanceCertificate)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (description == null) throw new ArgumentNullException("description");

            ITransportChannel channel = RegistrationChannel.Create(
                configuration, 
                description, 
                endpointConfiguration, 
                instanceCertificate,
                new ServiceMessageContext());

            return new RegistrationClient(channel);
        }

        #endregion
    }
    
    /// <summary>
    /// A channel object used by clients to access a UA discovery service.
    /// </summary>
    public partial class RegistrationChannel
    {
        #region Constructors
        /// <summary>
        /// Creates a new transport channel that supports the IRegistrationChannel service contract.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="description">The description for the endpoint.</param>
        /// <param name="endpointConfiguration">The configuration to use with the endpoint.</param>
        /// <param name="clientCertificate">The client certificate.</param>
        /// <param name="messageContext">The message context to use when serializing the messages.</param>
        /// <returns></returns>
        public static ITransportChannel Create(
            ApplicationConfiguration configuration,
            EndpointDescription description,
            EndpointConfiguration endpointConfiguration,
            X509Certificate2 clientCertificate,
            ServiceMessageContext messageContext)
        {
            // create a UA binary channel.
            ITransportChannel channel = CreateUaBinaryChannel(
                configuration,
                description,
                endpointConfiguration,
                clientCertificate,
                messageContext);

            // create a WCF XML channel.
            if (channel == null)
            {
                Uri endpointUrl = new Uri(description.EndpointUrl);
                RegistrationChannel wcfXmlChannel = new RegistrationChannel();

                TransportChannelSettings settings = new TransportChannelSettings();
                settings.Configuration = endpointConfiguration;
                settings.Description = description;
                settings.ClientCertificate = clientCertificate;
                wcfXmlChannel.Initialize(endpointUrl, settings);
                
                channel = wcfXmlChannel;
            }

            return channel;
        }

        #endregion
    } 
}
