namespace Common;

public static class HttpClientConfigurationOptions
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
    /// Listener option scheme http
    /// </summary>
    public const string CONF_SCHEME_HTTP = "http";

    /// <summary>
    /// Listener option scheme https
    /// </summary>
    public const string CONF_SCHEME_HTTPS = "https";

    /// <summary>
    /// If certificate is from path, take the password from the configuration 
    /// </summary>
    public const string CONF_CERT_PASS_SOURCE_CONFIGURATION = "configuration";

    /// <summary>
    /// If certificate is from path, take the password from the environment. The key is in the configuration file
    /// </summary>
    public const string CONF_CERT_PASS_SOURCE_ENV = "environment";

    public const string CONF_CERT_PASS_SOURCE_NONE = "none";
}
