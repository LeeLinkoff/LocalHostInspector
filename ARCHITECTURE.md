# LocalHost Inspector Architecture

## Solution Structure

```text
LocalHostInspector.sln

src/
├── LocalHostInspector.Web
│   ├── Program.cs
│   ├── Endpoints/
│   └── wwwroot/
│
├── LocalHostInspector.Core
│   ├── Engine/
│   ├── Interfaces/
│   ├── Models/
│   └── Rules/
│
├── LocalHostInspector.Diagnostics.Windows
│   └── Services/
│
└── tests/
    └── LocalHostInspector.Tests
```

## Project Responsibilities

### LocalHostInspector.Web

Responsible for:

- Application startup
- Dependency injection
- HTTP endpoints
- Static web content
- Hosting

This project intentionally contains no business logic.

---

### LocalHostInspector.Core

Responsible for:

- Diagnosis engine
- Business rules
- Interfaces
- Models
- Domain orchestration

The Core project contains no Windows-specific implementation.

---

### LocalHostInspector.Diagnostics.Windows

Responsible for Windows-specific diagnostics.

Current services include:

- CommandRunner
- WindowsDnsService
- WindowsHostsFileService
- IisWebServerService
- AuthoritativeDnsProviderService

This project implements the interfaces defined by `LocalHostInspector.Core`.

---

### LocalHostInspector.Tests

Contains unit tests for the Core logic and diagnostic services.

## Dependency Diagram

```text
                 LocalHostInspector.Web
                    /               \
                   /                 \
                  ▼                   ▼
      LocalHostInspector.Core <----- LocalHostInspector.Diagnostics.Windows
```

Dependencies:

- Web → Core
- Web → Diagnostics.Windows
- Diagnostics.Windows → Core
- Core → (none)

No circular dependencies exist.

## Request Flow

```text
Browser
   │
   ▼
Program.cs
   │
   ▼
Diagnostics Endpoint
   │
   ▼
DiagnosisEngine
   │
   ├── IDnsService
   ├── IHostsFileService
   └── IWebServerService
            │
            ▼
Windows Service Implementations
            │
            ├── WindowsDnsService
            │      ├── System DNS Lookup
            │      ├── Google Public DNS Lookup
            │      ├── Cloudflare Public DNS Lookup
            │      ├── Authoritative Name Server Lookup
            │      └── AuthoritativeDnsProviderService (RDAP)
            │
            ├── WindowsHostsFileService
            └── IisWebServerService
            │
            ▼
DiagnosticResult
            │
            ▼
Rules
            │
            ▼
Conclusion + Evidence
            │
            ▼
JSON Response
            │
            ▼
app.js
            │
            ▼
Browser UI
```

## Design Principles

- Separation of concerns
- Dependency inversion
- Interface-based design
- Testability
- Extensibility
- Platform isolation


## DNS Diagnostic Pipeline

The Windows DNS service performs multiple independent lookups to determine why a hostname resolves to the local computer.

Current diagnostic sequence:

1. Windows resolver (system DNS)
2. Google Public DNS
3. Cloudflare Public DNS
4. Authoritative name server discovery
5. RDAP lookup of an authoritative name server to identify the authoritative DNS provider
6. Hosts file inspection
7. IIS site binding inspection

The collected results are combined into a `DiagnosticResult`, which is evaluated by the rule engine to produce the final conclusion and supporting evidence.


## Why This Is Not an MVC Application

MVC organizes multi-view user interaction and presentation state, none of which this application has. LocalHost Inspector is a diagnostics engine with a single screen and a single JSON endpoint, so the web project stays a thin HTTP host while the diagnosis engine remains independent of any presentation framework.

## Why the Architecture Is More Structured Than a Typical Utility

The architecture was intentionally designed around the expected evolution of the application rather than the size of the initial implementation.

The first prototype consisted of a single source file. As additional diagnostic capabilities were added, including multiple DNS providers, hosts file inspection, IIS inspection, authoritative DNS analysis, provider identification, and a growing set of diagnosis rules, it became increasingly difficult to maintain a single-file implementation.

Separating the application into Web, Core, and platform-specific diagnostic projects isolates business logic from platform-specific implementations and presentation concerns. This allows new diagnostic providers, operating systems, web servers, and diagnosis rules to be added with minimal impact on existing components while keeping the diagnosis engine focused solely on evaluating diagnostic evidence.

The isolation also pays off directly in testability. `LocalHostInspector.Core` has no dependency on any Windows-specific API (no `System.Diagnostics.Process`, `Microsoft.Win32`, `System.DirectoryServices`, `System.Management`, or `System.ServiceProcess`), so `DiagnosisEngine` and the rule set can be unit tested against fake `IDnsService`, `IHostsFileService`, and `IWebServerService` implementations, with no real DNS lookups, no registry access, and no IIS calls required.

## Future Expansion

Future platform-specific implementations can be added without changing the Core project.

Examples:

```text
LocalHostInspector.Diagnostics.Linux
LocalHostInspector.Diagnostics.MacOS
LocalHostInspector.Diagnostics.Kubernetes
```

Each project would implement the interfaces in `LocalHostInspector.Core`, allowing the diagnosis engine to remain unchanged.
