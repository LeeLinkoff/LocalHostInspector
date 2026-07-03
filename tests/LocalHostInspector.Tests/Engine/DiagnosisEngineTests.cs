using LocalHostInspector.Core.Engine;
using LocalHostInspector.Core.Interfaces;
using LocalHostInspector.Core.Models;
using LocalHostInspector.Core.Rules;

using Moq;



namespace LocalHostInspector.Tests.Engine;


public class DiagnosisEngineTests
{
    [Fact]
    public async Task InspectAsync_ReturnsCollectedResults()
    {
        // Arrange
        var dns = new Mock<IDnsService>();
        var hosts = new Mock<IHostsFileService>();
        var web = new Mock<IWebServerService>();

        dns.Setup(x => x.InspectAsync("localhost"))
            .ReturnsAsync(new DnsResult());

        hosts.Setup(x => x.InspectAsync("localhost"))
            .ReturnsAsync(new HostsFileResult());

        web.Setup(x => x.InspectAsync("localhost"))
            .ReturnsAsync(new WebServerResult());

        var rules = new List<IDiagnosisRule>
        {
            new HostsFileRule(),
            new WebServerRule(),
            new PublicDnsRule(),
            new FallbackRule()
        };

        var engine = new DiagnosisEngine(
            dns.Object,
            hosts.Object,
            web.Object,
            rules);

        // Act
        var result = await engine.InspectAsync("localhost");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("localhost", result.Host);
        Assert.NotNull(result.Dns);
        Assert.NotNull(result.HostsFile);
        Assert.NotNull(result.WebServer);
    }

    [Fact]
    public async Task InspectAsync_CallsAllServices()
    {
        var dns = new Mock<IDnsService>();
        var hosts = new Mock<IHostsFileService>();
        var web = new Mock<IWebServerService>();

        dns.Setup(x => x.InspectAsync(It.IsAny<string>()))
            .ReturnsAsync(new DnsResult());

        hosts.Setup(x => x.InspectAsync(It.IsAny<string>()))
            .ReturnsAsync(new HostsFileResult());

        web.Setup(x => x.InspectAsync(It.IsAny<string>()))
            .ReturnsAsync(new WebServerResult());

        var rules = new List<IDiagnosisRule>
        {
            new HostsFileRule(),
            new WebServerRule(),
            new PublicDnsRule(),
            new FallbackRule()
        };

        var engine = new DiagnosisEngine(dns.Object, hosts.Object, web.Object, rules);
        await engine.InspectAsync("localhost");

        dns.Verify(x => x.InspectAsync("localhost"), Times.Once);
        hosts.Verify(x => x.InspectAsync("localhost"), Times.Once);
        web.Verify(x => x.InspectAsync("localhost"), Times.Once);
    }
}