using LocalHostInspector.Core.Engine;
using LocalHostInspector.Core.Interfaces;
using LocalHostInspector.Core.Rules;
using LocalHostInspector.Diagnostics.Windows.Services;
using LocalHostInspector.Web.Endpoints;


var builder = WebApplication.CreateBuilder(args);


// --------------------------------------------------------------------
// SINGLETON -- AddSingleton<T>()

// Purpose:
//   One instance created, shared across the entire app lifetime.
//   No repeated construction cost. Shared state where needed.

// Best for:
//   Stateless rule classes (like HostsFileRule)
//   Caches, config wrappers, HTTP clients, background services

// THE RULE:
//   Never inject a scoped (per-request) service into a singleton.
//   The scoped service gets trapped and reused across requests = state corruption.
//   ASP.NET Core throws at startup if it catches it.

// Safe injection targets inside a singleton:
//   Other singletons -- OK
//   No dependencies at all (pure stateless) -- OK
//   Scoped or Transient services -- VIOLATION
// --------------------------------------------------------------------


// Services
builder.Services.AddSingleton<ICommandRunner, CommandRunner>();
builder.Services.AddSingleton<IDnsService, WindowsDnsService>();
builder.Services.AddSingleton<IHostsFileService, WindowsHostsFileService>();
builder.Services.AddSingleton<IWebServerService, IisWebServerService>();
builder.Services.AddSingleton<IAuthoritativeDnsProviderService, AuthoritativeDnsProviderService>();

// Rules
// -----------------------------------------------------------------------------
// Diagnosis Rules
//
// Rules are evaluated in the order they are registered.
//
// The first rule whose Evaluate() method returns true determines the diagnostic
// conclusion presented to the user. Once a match is found, no additional rules
// are evaluated.
//
// Rules should be registered from MOST specific to LEAST specific.
//
// IMPORTANT:
// FallbackRule MUST always be registered last because it always returns true.
// If it is registered earlier, it will prevent all subsequent rules from
// executing.
//
// As new diagnostic capabilities are added (Split DNS, Reverse Proxy,
// Kubernetes, Docker, Apache, Nginx, etc.), register those rules BEFORE
// FallbackRule.
// -----------------------------------------------------------------------------
builder.Services.AddSingleton<IDiagnosisRule, WindowsLoopbackRule>();
builder.Services.AddSingleton<IDiagnosisRule, HostsFileRule>();
builder.Services.AddSingleton<IDiagnosisRule, WebServerRule>();
builder.Services.AddSingleton<IDiagnosisRule, PublicDnsLoopbackRule>();
builder.Services.AddSingleton<IDiagnosisRule, PublicDnsRule>();
builder.Services.AddSingleton<IDiagnosisRule, FallbackRule>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Engine
builder.Services.AddSingleton<DiagnosisEngine>();
var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseSwagger();
app.UseSwaggerUI();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapDiagnosticsEndpoints();

app.Run();