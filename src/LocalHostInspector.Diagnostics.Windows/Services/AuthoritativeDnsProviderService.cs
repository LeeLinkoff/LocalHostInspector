using LocalHostInspector.Core.Interfaces;
using LocalHostInspector.Core.Models;
using System.Net;
using System.Text.Json;


namespace LocalHostInspector.Diagnostics.Windows.Services;


public class AuthoritativeDnsProviderService : IAuthoritativeDnsProviderService
{
    private readonly HttpClient _httpClient = new();

    public async Task<AuthoritativeDnsProvider?> GetProviderAsync(IEnumerable<string> nameServers)
    {
        var firstServer = nameServers.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(firstServer))
            return null;

        IPAddress[] addresses;

        try
        {
            addresses = await Dns.GetHostAddressesAsync(firstServer);
        }
        catch
        {
            return null;
        }

        var address = addresses.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

        if (address == null)
            return null;

        string[] rdapServers =
        {
            "https://rdap.arin.net/registry/ip/",
            "https://rdap.db.ripe.net/ip/",
            "https://rdap.apnic.net/ip/",
            "https://rdap.lacnic.net/rdap/ip/",
            "https://rdap.afrinic.net/rdap/ip/"
        };

        foreach (var server in rdapServers)
        {
            try
            {
                var response = await _httpClient.GetAsync(server + address);

                if (!response.IsSuccessStatusCode)
                    continue;

                using var stream = await response.Content.ReadAsStreamAsync();

                using var document = await JsonDocument.ParseAsync(stream);

                if (document.RootElement.TryGetProperty("name", out var name))
                {
                    var provider = new AuthoritativeDnsProvider
                    {
                        Organization = name.GetString(),
                        Address = address.ToString()
                    };

                    if (document.RootElement.TryGetProperty("handle", out var network))
                    {
                        provider.Network = network.GetString();
                    }

                    if (document.RootElement.TryGetProperty("entities", out var entities))
                    {
                        foreach (var entity in entities.EnumerateArray())
                        {
                            if (!entity.TryGetProperty("vcardArray", out var vcard))
                                continue;

                            foreach (var item in vcard[1].EnumerateArray())
                            {
                                if (item.GetArrayLength() < 4)
                                    continue;

                                var property = item[0].GetString();

                                switch (property)
                                {
                                    case "adr":
                                        if (item[3].ValueKind == JsonValueKind.Array)
                                        {
                                            provider.Address = string.Join(", ",
                                                item[3]
                                                    .EnumerateArray()
                                                    .Where(x => x.ValueKind == JsonValueKind.String)
                                                    .Select(x => x.GetString())
                                                    .Where(x => !string.IsNullOrWhiteSpace(x)));
                                        }
                                        break;

                                    case "email":
                                        {
                                            if (item[3].ValueKind != JsonValueKind.String)
                                                break;

                                            var value = item[3].GetString();

                                            if (provider.Email == null)
                                                provider.Email = value;
                                            else
                                                provider.AbuseEmail ??= value;

                                            break;
                                        }

                                    case "tel":
                                        {
                                            if (item[3].ValueKind != JsonValueKind.String)
                                                break;

                                            var value = item[3].GetString();

                                            if (provider.Phone == null)
                                                provider.Phone = value;
                                            else
                                                provider.AbusePhone ??= value;

                                            break;
                                        }
                                }
                            }
                        }
                    }

                    return provider;
                }

                if (document.RootElement.TryGetProperty("handle", out var handle))
                {
                    return new AuthoritativeDnsProvider
                    {
                        Network = handle.GetString(),
                        Address = address.ToString()
                    };
                }
            }
            catch(Exception ex)
            {
                throw new Exception($"RDAP lookup failed for {server}{address}", ex);
            }
        }

        return null;
    }
}