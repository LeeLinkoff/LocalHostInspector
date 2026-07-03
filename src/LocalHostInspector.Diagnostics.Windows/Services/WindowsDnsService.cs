using LocalHostInspector.Core.Interfaces;
using LocalHostInspector.Core.Models;


namespace LocalHostInspector.Diagnostics.Windows.Services;


public class WindowsDnsService : IDnsService
{
    private readonly ICommandRunner _commandRunner;
    private readonly IAuthoritativeDnsProviderService _providerService;

    public WindowsDnsService(ICommandRunner commandRunner, IAuthoritativeDnsProviderService providerService)
    {
        _commandRunner = commandRunner;
        _providerService = providerService;
    }

    public async Task<DnsResult> InspectAsync(string host)
    {
        var systemLookup = await _commandRunner.RunAsync("nslookup", host);

        var googleLookup = await _commandRunner.RunAsync("nslookup", $"{host} 8.8.8.8");

        var cloudflareLookup = await _commandRunner.RunAsync("nslookup", $"{host} 1.1.1.1");

        var authoritativeLookup = await _commandRunner.RunAsync("nslookup", $"-type=ns {host}");

        var authoritativeServers = ParseAuthoritativeNameServers(authoritativeLookup);

        var authoritativeProvider = await _providerService.GetProviderAsync(authoritativeServers);

        return new DnsResult
        {
            SystemLookup = systemLookup,
            SystemAddress = ParseReturnedAddress(systemLookup),

            GoogleLookup = googleLookup,
            GoogleAddress = ParseReturnedAddress(googleLookup),

            CloudflareLookup = cloudflareLookup,
            CloudflareAddress = ParseReturnedAddress(cloudflareLookup),

            AuthoritativeLookup = authoritativeLookup,
            AuthoritativeAddress = ParseReturnedAddress(authoritativeLookup),

            AuthoritativeNameServers = authoritativeServers,
            AuthoritativeProvider = authoritativeProvider
        };
    }

    private static string? ParseReturnedAddress(string lookup)
    {
        var lines = lookup.Split(
            Environment.NewLine,
            StringSplitOptions.RemoveEmptyEntries);

        bool sawName = false;

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            if (trimmed.StartsWith("Name:", StringComparison.OrdinalIgnoreCase))
            {
                sawName = true;
                continue;
            }

            if (sawName &&
                trimmed.StartsWith("Address:", StringComparison.OrdinalIgnoreCase))
            {
                return trimmed["Address:".Length..].Trim();
            }
        }

        return null;
    }

    private static List<string> ParseAuthoritativeNameServers(string lookup)
    {
        var servers = new List<string>();

        var lines = lookup.Split(
            Environment.NewLine,
            StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var index = line.IndexOf("nameserver =", StringComparison.OrdinalIgnoreCase);

            if (index >= 0)
            {
                servers.Add(line[(index + "nameserver =".Length)..].Trim());
            }
        }

        return servers;
    }

    private static string? DetermineDnsProvider(IEnumerable<string> nameServers)
    {
        foreach (var server in nameServers)
        {
            var value = server.ToLowerInvariant();

            if (value.Contains("domaincontrol.com"))
                return "GoDaddy";

            if (value.Contains("cloudflare"))
                return "Cloudflare";

            if (value.Contains("awsdns"))
                return "Amazon Route 53";

            if (value.Contains("azure-dns"))
                return "Microsoft Azure DNS";

            if (value.Contains("googledomains"))
                return "Google Cloud DNS";

            if (value.Contains("akam.net"))
                return "Akamai";
        }

        return null;
    }
}