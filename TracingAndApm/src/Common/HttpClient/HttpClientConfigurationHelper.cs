using Common;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Extensions.DependencyInjection;

internal static class HttpClientConfigurationHelper
{
    private static X509Certificate2 LoadCertificateFromPath(HttpClientConfiguration config)
    {
        if (string.IsNullOrEmpty(config.CertFilePath))
        {
            throw new Exception($"You selected {nameof(HttpClientConfiguration.CertSource)}={HttpClientConfigurationOptions.CONF_CERT_SOURCE_FILE}. " +
                $"You have to define \'{nameof(HttpClientConfiguration.CertFilePath)}\'");
        }

        if (string.IsNullOrEmpty(config.CertPasswordSource))
        {
            throw new Exception($"You selected {nameof(HttpClientConfiguration.CertSource)}={HttpClientConfigurationOptions.CONF_CERT_SOURCE_FILE}. " +
                $"You have to defined \'{nameof(HttpClientConfiguration.CertPasswordSource)}\'");
        }

        string? password;

        if (config.CertPasswordSource == HttpClientConfigurationOptions.CONF_CERT_PASS_SOURCE_CONFIGURATION)
        {
            if (string.IsNullOrEmpty(config.CertPassword))
            {
                throw new Exception($"You selected {nameof(HttpClientConfiguration.CertPasswordSource)}=\'{HttpClientConfigurationOptions.CONF_CERT_PASS_SOURCE_CONFIGURATION}\'. " +
                    $"The {nameof(HttpClientConfiguration.CertPassword)} is not defined");
            }

            password = config.CertPassword;
        }
        else if (config.CertPasswordSource == HttpClientConfigurationOptions.CONF_CERT_PASS_SOURCE_ENV)
        {
            if (string.IsNullOrEmpty(config.CertPasswordEnvKey))
            {
                throw new Exception($"You selected {nameof(HttpClientConfiguration.CertPasswordSource)}=\'{HttpClientConfigurationOptions.CONF_CERT_PASS_SOURCE_ENV}\'." +
                    $"The {nameof(HttpClientConfiguration.CertPasswordEnvKey)} is not defined");
            }
            password = Environment.GetEnvironmentVariable(config.CertPasswordEnvKey) ?? string.Empty;

            if (string.IsNullOrEmpty(password))
            {
                throw new Exception($"Could not find the password in the environment variable \'{config.CertPasswordEnvKey}\'");
            }
        }
        else if (config.CertPasswordSource == HttpClientConfigurationOptions.CONF_CERT_PASS_SOURCE_NONE)
        {
            password = null;
        }
        else
        {
            throw new Exception($"{nameof(HttpClientConfiguration.CertPasswordSource)} can be only \'configuration\' or 'environment'");
        }

        // fixing will remove starting / and it's bad in linux

        //determine the absolue path of the certificate
        //string[] splitPath = config.CertFilePath.Split('/', '\\').Where(x => !(x.ContainsOnly('/') || x.ContainsOnly('\\'))).ToArray();

        //string fixedPath = string.Join(Path.DirectorySeparatorChar, splitPath);

        //if (File.Exists(fixedPath))
        //{
        //    fixedPath = Path.Combine(AppContext.BaseDirectory, fixedPath);

        //    if (!File.Exists(fixedPath))
        //    {
        //        throw new Exception("Could not find the certificate. Please provide a correct absolute or relative path." +
        //            "\n Also make sure that if the certificate is a solution folder,that is marked with Copy As Newer");
        //    }
        //}

        return new X509Certificate2(config.CertFilePath, password);
    }

    private static X509Certificate2 LoadCertificateFromStore(HttpClientConfiguration config)
    {
        if (string.IsNullOrEmpty(config.CertStoreName)
            || string.IsNullOrEmpty(config.CertStoreLocation))
        {
            throw new Exception($"You selected {nameof(HttpClientConfiguration.CertSource)} to be equal to \'{HttpClientConfigurationOptions.CONF_CERT_SOURCE_STORE}\' " +
                $"but the {nameof(HttpClientConfiguration.CertStoreLocation)} and {nameof(HttpClientConfiguration.CertStoreName)} are not " +
                $"properly defined.");
        }

        if (string.IsNullOrWhiteSpace(config.Thumbprint))
        {
            throw new Exception($"You selected {nameof(HttpClientConfiguration.CertSource)} to be equal to \'{HttpClientConfigurationOptions.CONF_CERT_SOURCE_STORE}\' " +
                  $"but the {nameof(HttpClientConfiguration.Thumbprint)} is not defined.");
        }
        //store not used. I kept it as an example

        using var store = new X509Store(config.CertStoreName, Enum.Parse<StoreLocation>(config.CertStoreLocation));

        store.Open(OpenFlags.ReadOnly);
        var certificate = store.Certificates.Find(
            X509FindType.FindByThumbprint,
            config.Thumbprint,
            validOnly: config.CertStoreOnlyValidCertificates);

        if (certificate.Count == 0)
        {
            throw new InvalidOperationException($"Certificate not found for {config.Thumbprint}.");
        }

        return certificate[0];

        throw new InvalidOperationException("No valid certificate configuration found for the current endpoint.");
    }

    internal static HttpMessageHandler GetHttpMessageHandler(HttpClientConfiguration conf)
    {
        if (string.Equals(conf.CertSource, HttpClientConfigurationOptions.CONF_CERT_SOURCE_STORE, StringComparison.OrdinalIgnoreCase))
        {
            X509Certificate2 certificate = LoadCertificateFromStore(conf);
            return GetHttpMessageHandler(certificate);
        }
        else if (string.Equals(conf.CertSource, HttpClientConfigurationOptions.CONF_CERT_SOURCE_FILE, StringComparison.OrdinalIgnoreCase))
        {
            X509Certificate2 certificate = LoadCertificateFromPath(conf);
            return GetHttpMessageHandler(certificate);
        }
        else
        {
            throw new Exception($"Invalid certificate store. Allowed only \'{HttpClientConfigurationOptions.CONF_CERT_SOURCE_FILE}\' " +
                $"or \'{HttpClientConfigurationOptions.CONF_CERT_SOURCE_STORE}\'");
        }
    }

    private static HttpClientHandler GetHttpMessageHandler(X509Certificate2 certificate)
    {
#if DEBUG
        var httpClientHandler = new HttpClientHandler
        {
            //TODO: change this to be only in debug mode
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
#else
        var httpClientHandler = new HttpClientHandler();
#endif

        httpClientHandler.ClientCertificates.Add(certificate);
        return httpClientHandler;
    }
}
