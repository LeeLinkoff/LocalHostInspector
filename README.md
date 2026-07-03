# LocalHost Inspector

## Why I Created This Project

I frequently encountered situations where a hostname unexpectedly
resolved to my local machine. Determining why this happened often
required manually checking multiple sources, including:

-   The Windows hosts file
-   Public DNS servers
-   IIS bindings
-   Local web server configuration

Although each individual step is straightforward, performing them
repeatedly is time-consuming and error-prone. The goal of LocalHost
Inspector is to automate that investigation and provide a clear
explanation of why a request resolved to the local system.

Rather than simply displaying raw diagnostic data, the application
attempts to identify the most likely cause and present both a conclusion
and the evidence supporting that conclusion.

## Why the Architecture Appears "Over-Engineered"

The initial prototype was implemented as a single file and was intended
to be a quick utility.

As the project evolved, it became clear that additional diagnostic
capabilities would continue to be added over time. Instead of allowing
the application to grow into one large source file, I chose to refactor
it into separate layers with clearly defined responsibilities.

This approach intentionally favors maintainability and extensibility
over minimizing the number of source files.

The project is separated into:

-   **Web** -- User interface, HTTP endpoints, dependency injection, and
    application hosting.
-   **Core** -- Business logic, diagnostic models, rules, interfaces,
    and orchestration.
-   **Diagnostics.Windows** -- Windows-specific implementations
    responsible for collecting system information.
-   **Tests** -- Unit tests validating the application's behavior.

Each component has a single responsibility, making the code easier to
understand, test, and maintain.



## Current Diagnostic Capabilities

LocalHost Inspector performs multiple independent diagnostics to determine why a hostname resolves to the local computer.

Current diagnostics include:

- Windows hosts file inspection
- Windows DNS resolver lookup
- Google Public DNS lookup
- Cloudflare Public DNS lookup
- Authoritative name server discovery
- RDAP lookup to identify the authoritative DNS provider and available public contact information
- IIS web site binding inspection

The application correlates these independent sources and applies diagnostic rules to determine the most likely cause of the resolution, presenting both a conclusion and the supporting evidence.

## Typical Diagnostic Flow

For each request the application performs the following steps:

1. Parse the requested URL and hostname.
2. Inspect the Windows hosts file.
3. Query the Windows DNS resolver.
4. Query Google Public DNS.
5. Query Cloudflare Public DNS.
6. Discover the domain's authoritative name servers.
7. Perform an RDAP lookup against an authoritative name server to identify the responsible DNS provider.
8. Inspect IIS bindings.
9. Evaluate the collected evidence using the rule engine.
10. Return a structured JSON response consumed by the browser UI.

## Extensibility

Although the current implementation targets Windows, the architecture
was intentionally designed to support future expansion.

Examples include:

-   Additional web servers (Apache, Nginx, Kestrel, reverse proxies,
    etc.)
-   Additional diagnostic rules
-   Additional DNS providers
-   Additional operating systems
-   New diagnostic modules

Most future enhancements can be implemented by adding new services or
rules without modifying the application's core orchestration logic.

## Portability

While the C# implementation is not portable to another language, the
architecture is.

Because the project separates orchestration, business rules,
platform-specific diagnostics, and presentation, those responsibilities
can be reimplemented in another language while preserving the overall
design.

A future implementation written in Go, Rust, Java, Python, or C++ could
follow the same architecture with minimal design changes.

## Design Philosophy

This project follows several software engineering principles:

-   Separation of concerns
-   Dependency inversion
-   Single responsibility
-   Interface-based design
-   Testability
-   Extensibility

Although these principles introduce additional structure compared to a
simple utility application, they provide a solid foundation for
long-term maintenance and future enhancements.
