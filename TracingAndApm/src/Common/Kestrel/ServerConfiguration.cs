using Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace Common;

public static class ServerConfiguration
{
    /// <summary>
    /// Looks in the store for a certificate
    /// </summary>
    public const string CONF_CERT_SOURCE_STORE = "store";
    /// <summary>
    /// Provide a path for the pfx
    /// </summary>
    public const string CONF_CERT_SOURCE_FILE = "file";

    /// <summary>
    /// Simply using UseHsts() which looks for a default developer certificate if any.
    /// </summary>
    public const string CONF_CERT_SOURCE_DEFAULT = "default";

    /// <summary>
    /// Listener option scheme http
    /// </summary>
    public const string CONF_SCHEME_HTTP = "http";

    /// <summary>
    /// Listener option scheme https
    /// </summary>
    public const string CONF_SCHEME_HTTPS = "https";

    /// <summary>
    /// windows environemnt. If Host=localhost it will automatically bind to the default localhost address
    /// </summary>
    public const string CONF_OS_WINDOWS = "windows";

    /// <summary>
    /// linux environment. If Host=localhost it will automatically bind to the default localhost=0.0.0.0 address
    /// </summary>
    public const string CONF_OS_LINUX = "linux";

    /// <summary>
    /// If certificate is from path, take the password from the configuration 
    /// </summary>
    public const string CONF_CERT_PASS_SOURCE_CONFIGURATION = "configuration";

    /// <summary>
    /// If certificate is from path, take the password from the environment. The key is in the configuration file
    /// </summary>
    public const string CONF_CERT_PASS_SOURCE_ENV = "environment";

    /// <summary>
    /// No password
    /// </summary>
    public const string CONF_CERT_PASS_SOURCE_NONE = "none";

    private static ILogger? logger;

    //do not make it an extension method. It's more clear to know where it originated from
    public static void ConfigureEndpoints(
        KestrelServerOptions options,
        string configurationSection = "HttpServer")
    {
        if (logger is not null)
        {
            ILoggerFactory? loggerFactory = options.ApplicationServices.GetService<ILoggerFactory>();

            if (loggerFactory is not null)
            {
                logger = loggerFactory.CreateLogger(typeof(ServerConfiguration));
            }
        }

        IConfiguration configuration = options.ApplicationServices.GetRequiredService<IConfiguration>();
        IWebHostEnvironment environment = options.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

        ServerSettings serverSettings = new();
        configuration.GetSection($"{configurationSection}:Settings").Bind(serverSettings);

        options.Limits.MaxConcurrentConnections = serverSettings.MaxConcurrentConnections;
        options.Limits.MaxConcurrentUpgradedConnections = serverSettings.MaxConcurrentUpgradedConnections;
        options.Limits.MaxRequestBodySize = serverSettings.MaxRequestBodySize;

        options.Limits.MinRequestBodyDataRate =
            new MinDataRate(bytesPerSecond: 100,
                gracePeriod: TimeSpan.FromSeconds(10));
        options.Limits.MinResponseDataRate =
            new MinDataRate(bytesPerSecond: 100,
                gracePeriod: TimeSpan.FromSeconds(10));

        List<EndpointConfiguration> endpoints = new();

        configuration.GetSection($"{configurationSection}:Endpoints").Bind(endpoints);

        foreach (EndpointConfiguration endpoint in endpoints)
        {
            int port = ReadPortFromConfiguration(endpoint);
            IPAddress iPAddress = ReadIPAddressFromConfiguration(endpoint);

            options.Listen(iPAddress, port,
                listenOptions =>
                {
                    if (endpoint.Scheme == CONF_SCHEME_HTTP)
                    {
                        //do nothing
                    }
                    else if (endpoint.Scheme == CONF_SCHEME_HTTPS)
                    {
                        if (endpoint.CertSource == CONF_CERT_SOURCE_FILE)
                        {
                            X509Certificate2 certificate = LoadCertificateFromPath(endpoint);
                            listenOptions.UseHttps(certificate);

                        }
                        else if (endpoint.CertSource == CONF_CERT_SOURCE_STORE)
                        {
                            X509Certificate2 certificate = LoadCertificateFromStore(endpoint, environment);
                            listenOptions.UseHttps(certificate);
                        }
                        else if (endpoint.CertSource == CONF_CERT_SOURCE_DEFAULT)
                        {
                            listenOptions.UseHttps();
                        }
                        else
                        {
                            throw new Exception($"Invalid CertSource. Allowed only \'{CONF_CERT_SOURCE_FILE}\' or \'{CONF_CERT_SOURCE_STORE}\' or \'{CONF_CERT_SOURCE_DEFAULT}\'");
                        }
                    }
                    else
                    {
                        throw new Exception($"Invalid scheme. Scheme can be \'{CONF_SCHEME_HTTP}\' or '\'{CONF_SCHEME_HTTPS}\'");
                    }

                    List<HttpProtocols> protocols = endpoint
                        .ListenProtocols?
                        .Select(protocol =>
                        {
                            if (Enum.TryParse(typeof(HttpProtocols), protocol, out var typedProtocol))
                            {
                                return typedProtocol;
                            }

                            return default;
                        })
                        .Where(protocol => protocol is not null)
                        .OfType<HttpProtocols>()
                        .ToList() ?? new();

                    if (protocols?.Count > 0)
                    {
                        listenOptions.Protocols = protocols.Aggregate((f, s) => f | s);
                    }
                    else
                    {
                        // keep default value
                        //listenOptions.Protocols = HttpProtocols.Http2;
                    }

                    Console.WriteLine($"Listening to {endpoint.Scheme}[{listenOptions.Protocols}]://{iPAddress.ToString()}:{port}");
                });
        }
    }

    private static IPAddress ReadIPAddressFromConfiguration(EndpointConfiguration endpoint)
    {
        if (endpoint.Host == "localhost")
        {
            if (endpoint.OS == CONF_OS_LINUX)
            {
                return IPAddress.Parse("0.0.0.0");
            }
            else
            {
                //ipAddresses.Add(IPAddress.IPv6Loopback);
                //ipAddresses.Add(IPAddress.Loopback);
                var host = Dns.GetHostEntry(Dns.GetHostName());

                string localhostSecondaryPattern = string.IsNullOrWhiteSpace(endpoint.WinOSLocalHostFamilyRange)
                            ? "192"
                            : endpoint.WinOSLocalHostFamilyRange;

                IPAddress? localIp = Array.Find(host.AddressList,
                    x => x.AddressFamily == AddressFamily.InterNetwork
                        && x.ToString().StartsWith(localhostSecondaryPattern));

                if (localIp is null)
                {
                    //need 192 (local address) and not 127. 192 is the ip allocated in the network and visible also if we
                    //have systems in docker in linux calling systems in windows
                    throw new Exception($"Could not find a local ip address on windows with IPv4 and starting with {localhostSecondaryPattern}");
                }

                return localIp;
            }
        }
        else if (IPAddress.TryParse(endpoint.Host, out var address))
        {
            return address;
        }
        else
        {
            return IPAddress.IPv6Any;
        }
    }

    private static int ReadPortFromConfiguration(EndpointConfiguration endpoint)
    {
        int port = -1;

        if (endpoint.Port != null)
        {
            port = endpoint.Port.Value;
        }
        else if (endpoint.Scheme == CONF_SCHEME_HTTP)
        {
            port = 80; //default
        }
        else if (endpoint.Scheme == CONF_SCHEME_HTTPS)
        {
            port = 443; //default
        }
        else
        {
            throw new Exception($"Could not determine port. Either provide a port number of a valid scheme: \'{CONF_SCHEME_HTTP}\' " +
                $"or \'{CONF_SCHEME_HTTPS}\'");
        }

        return port;
    }

    private static X509Certificate2 LoadCertificateFromPath(EndpointConfiguration config)
    {
        logger?.LogDebug("Loading certificate from a file path");
        logger?.LogInformation($"Certificate Path: {config.CertFilePath}");

        if (string.IsNullOrEmpty(config.CertFilePath))
        {
            throw new Exception($"You selected {nameof(EndpointConfiguration.CertSource)}={CONF_CERT_SOURCE_FILE}. " +
                $"You have to define \'{nameof(EndpointConfiguration.CertFilePath)}\'");
        }

        logger?.LogInformation($"Certificate Password Source: {config.CertPasswordSource}");
        if (string.IsNullOrEmpty(config.CertPasswordSource))
        {
            throw new Exception($"You selected {nameof(EndpointConfiguration.CertSource)}={CONF_CERT_SOURCE_FILE}. " +
                $"You have to defined \'{nameof(EndpointConfiguration.CertPasswordSource)}\'");
        }

        string? password = "";

        if (config.CertPasswordSource == CONF_CERT_PASS_SOURCE_CONFIGURATION)
        {
            if (string.IsNullOrEmpty(config.CertPassword))
            {
                throw new Exception($"You selected {nameof(EndpointConfiguration.CertPasswordSource)}=\'{CONF_CERT_PASS_SOURCE_CONFIGURATION}\'. " +
                    $"The {nameof(EndpointConfiguration.CertPassword)} is not defined");
            }

            password = config.CertPassword;

        }
        else if (config.CertPasswordSource == CONF_CERT_PASS_SOURCE_ENV)
        {
            if (string.IsNullOrEmpty(config.CertPasswordEnvKey))
            {
                throw new Exception($"You selected {nameof(EndpointConfiguration.CertPasswordSource)}=\'{CONF_CERT_PASS_SOURCE_ENV}\'." +
                    $"The {nameof(EndpointConfiguration.CertPasswordEnvKey)} is not defined");
            }
            password = Environment.GetEnvironmentVariable(config.CertPasswordEnvKey) ?? "";
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception($"Could not find the password in the environment variable \'{config.CertPasswordEnvKey}\'");
            }
        }
        else if (config.CertPasswordSource == CONF_CERT_PASS_SOURCE_NONE)
        {
            password = null;
        }
        else
        {
            throw new Exception($"{nameof(EndpointConfiguration.CertPasswordSource)} can be only \'configuration\' or 'environment'");
        }

        //determine the absolue path of the certificate
        string fixedPath = config.CertFilePath;

        if (config.OS == CONF_OS_LINUX)
        {
            if (File.Exists(Path.Combine(AppContext.BaseDirectory, fixedPath)))
            {
                fixedPath = Path.Combine(AppContext.BaseDirectory, fixedPath);
            }
            if (File.Exists(Path.Combine(AppContext.BaseDirectory, "/" + fixedPath)))
            {
                fixedPath = Path.Combine(AppContext.BaseDirectory, "/" + fixedPath);
            }
            if (File.Exists("/" + fixedPath))
            {
                fixedPath = "/" + fixedPath;
            }
        }
        else
        {
            if (File.Exists(Path.Combine(AppContext.BaseDirectory, fixedPath)))
            {
                fixedPath = Path.Combine(AppContext.BaseDirectory, fixedPath);
            }
        }

        if (!File.Exists(fixedPath))
        {
            throw new Exception("Could not find the certificate. Please provide a correct absolute or relative path." +
                "\n Also make sure that if the certificate is a solution folder,that is marked with Copy As Newer");
        }

        return new X509Certificate2(fixedPath, password);
    }

    private static X509Certificate2 LoadCertificateFromStore(EndpointConfiguration config, IWebHostEnvironment environment)
    {
        logger?.LogDebug("Loading certificate from store");

        if (string.IsNullOrEmpty(config.CertStoreName) || string.IsNullOrEmpty(config.CertStoreLocation))
        {
            throw new Exception($"You selected {nameof(EndpointConfiguration.CertSource)} to be equal to \'{CONF_CERT_SOURCE_STORE}\' " +
                $"but the {nameof(EndpointConfiguration.CertStoreLocation)} and {nameof(EndpointConfiguration.CertStoreName)} are not " +
                $"properly defined.");
        }

        logger?.LogInformation($"Cettificate Store Name : {config.CertStoreName}");
        logger?.LogInformation($"Cettificate Store Location : {config.CertStoreLocation}");

        //store not used. I kept it as an example
        if (config.CertStoreName == null
            || config.CertStoreLocation == null)
        {
            throw new InvalidOperationException("No valid certificate configuration found for the current endpoint.");
        }

        if (config.Thumbprint is null)
        {
            throw new InvalidOperationException("The configuration does not provide a thumbprint value to search for.");
        }

        using var store = new X509Store(config.CertStoreName, Enum.Parse<StoreLocation>(config.CertStoreLocation));
        store.Open(OpenFlags.ReadOnly);
        var certificate = store.Certificates.Find(
            X509FindType.FindByThumbprint,
            config.Thumbprint,
            validOnly: !environment.IsDevelopment());

        if (certificate.Count == 0)
        {
            throw new InvalidOperationException($"Certificate not found for {config.Host}.");
        }

        return certificate[0];
    }
}
