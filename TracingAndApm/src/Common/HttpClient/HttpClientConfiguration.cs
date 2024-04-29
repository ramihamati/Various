namespace Common;

public class HttpClientConfiguration
{
    /// <summary>
    /// The HttpClient name used to be retrieved from IHttpClientFactory
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 'http' or 'https'
    /// </summary>
    public string? Scheme { get; set; }

    /// <summary>
    /// 'store', 'file'
    /// <para>'store' : looks in the defined <see cref="CertStoreName"/> and <see cref="CertStoreLocation"/></para> 
    /// <para>'default' : uses UseHsts() which laods the default developer certificate if available</para> 
    /// </summary>
    public string? CertSource { get; set; }

    /// <summary>
    /// Store name if <see cref="CertSource"/> is 'store'
    /// </summary>
    public string? CertStoreName { get; set; }

    /// <summary>
    /// Store location if <see cref="CertSource"/> is 'store'.
    /// <para>Can be <see cref="StoreLocation.CurrentUser"/> or <see cref="StoreLocation.LocalMachine"/> as string</para>
    /// </summary>
    public string? CertStoreLocation { get; set; }

    /// <summary>
    /// To allow only valid certificates
    /// <para>Use false for dev.</para>
    /// </summary>
    public bool CertStoreOnlyValidCertificates { get; set; } = true;

    /// <summary>
    /// File path for the pfx certificate if <see cref="CertSource"/> is 'file'
    /// </summary>
    public string? CertFilePath { get; set; }

    /// <summary>
    /// 'environment', 'configuration'
    /// </summary>
    public string? CertPasswordSource { get; set; }

    /// <summary>
    /// Certificate password if <see cref="CertPasswordSource"/> equals 'configuration'
    /// </summary>
    public string? CertPassword { get; set; }

    /// <summary>
    /// Environment key to get the password if <see cref="CertPasswordSource"/> equals 'environment'
    /// </summary>
    public string? CertPasswordEnvKey { get; set; }

    /// <summary>
    /// Search criteria to find the certificate in the store. Enter the host
    /// </summary>
    public string? Thumbprint { get; set; }

    /// <summary>
    /// https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.defaultrequestversion?view=net-6.0
    /// The DefaultRequestVersion property specifies the default HTTP version to use for requests sent using this HttpClient instance when it constructs the HttpRequestMessage to send
    ///  <para>The DefaultRequestVersion property doesn't apply to the SendAsync method. The HttpRequestMessage parameter passed as the argument to the SendAsync method has its own Version property that controls the HTTP version used for the request
    /// <see cref="System.Net.HttpVersion"/></para>
    /// <para>"1.0" "1.1" "2.0" "3.0"</para>
    /// </summary>
    public string? DefaultRequestVersion { get; set; }
}
