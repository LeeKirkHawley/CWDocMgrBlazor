using System.Net.Http.Headers;
using System.Text;

namespace CWDocMgrBlazor.Client
{
    public class HttpLoggingHandler : DelegatingHandler
    {
        private readonly ILogger<HttpLoggingHandler> _logger;

        public HttpLoggingHandler(ILogger<HttpLoggingHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Request: {Method} {Uri}", request.Method, request.RequestUri);

            var response = await base.SendAsync(request, cancellationToken);

            _logger.LogInformation("Response Status: {StatusCode}", response.StatusCode);

            if (response.Content is not null)
            {
                var ct = response.Content.Headers.ContentType;
                var mediaType = ct?.MediaType?.ToLowerInvariant() ?? string.Empty;

                // Log only text-like content
                bool isText =
                    mediaType.StartsWith("text/") ||
                    mediaType == "application/json" ||
                    mediaType == "application/xml" ||
                    mediaType == "application/xhtml+xml";

                if (isText)
                {
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogInformation("Incoming Content: {Preview}...", content.Length > 500 ? content[..500] : content);
                    // Do NOT re-wrap content; leave it as-is to avoid breaking downstream readers
                }
                else
                {
                    _logger.LogInformation("Skipping body logging for media type {MediaType}", mediaType);
                }
            }

            return response;
        }
    }
}