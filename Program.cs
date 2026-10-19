using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using CersaiCkycApi.Authentication;
using CersaiCkycApi.Configuration;
using CersaiCkycApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// -------------------- Configuration --------------------
builder.Services.Configure<CersaiOptions>(
    builder.Configuration.GetSection(CersaiOptions.SectionName));
builder.Services.Configure<ApiKeyOptions>(
    builder.Configuration.GetSection(ApiKeyOptions.SectionName));

var cersaiOptions = builder.Configuration
    .GetSection(CersaiOptions.SectionName)
    .Get<CersaiOptions>() ?? new CersaiOptions();

// -------------------- mTLS HttpClient (outbound to CERSAI) --------------------
builder.Services.AddHttpClient("CersaiMtls", client =>
{
    client.BaseAddress = new Uri(cersaiOptions.BaseUrl.TrimEnd('/') + "/");
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
    client.Timeout = TimeSpan.FromSeconds(60);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler
    {
        ClientCertificateOptions = ClientCertificateOption.Manual,
        SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13
    };

    if (!string.IsNullOrWhiteSpace(cersaiOptions.ClientCertificatePath) &&
        File.Exists(cersaiOptions.ClientCertificatePath))
    {
        var cert = string.IsNullOrEmpty(cersaiOptions.ClientCertificatePassword)
            ? X509CertificateLoader.LoadPkcs12FromFile(cersaiOptions.ClientCertificatePath, null)
            : X509CertificateLoader.LoadPkcs12FromFile(
                cersaiOptions.ClientCertificatePath,
                cersaiOptions.ClientCertificatePassword);

        handler.ClientCertificates.Add(cert);
    }
    else
    {
        Console.WriteLine("WARNING: Client certificate not configured. mTLS will fail against real CERSAI endpoints.");
    }

    if (!string.IsNullOrWhiteSpace(cersaiOptions.ServerCaCertificatePath) &&
        File.Exists(cersaiOptions.ServerCaCertificatePath))
    {
        var ca = X509CertificateLoader.LoadCertificateFromFile(cersaiOptions.ServerCaCertificatePath);
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
        {
            if (cert is null) return false;
            using var chain2 = new X509Chain();
            chain2.ChainPolicy.ExtraStore.Add(ca);
            chain2.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
            chain2.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            return chain2.Build(new X509Certificate2(cert));
        };
    }

    return handler;
});

// -------------------- API Key Authentication (inbound) --------------------
builder.Services
    .AddAuthentication(ApiKeyAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
        ApiKeyAuthenticationHandler.SchemeName, _ => { });

builder.Services.AddAuthorization();

// -------------------- Application services --------------------
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IEncryptionService, StubEncryptionService>();
builder.Services.AddScoped<ICkycClient, CkycClient>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// -------------------- Swagger / OpenAPI --------------------
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CERSAI CKYCRR 2.0 Client API",
        Version = "v1",
        Description = """
            ASP.NET Core client/proxy for CERSAI Central KYC Record Registry (CKYCRR 2.0).

            **Inbound security:** Pass a valid API key in the `X-Api-Key` header.
            **Outbound security:** Calls to CERSAI use mTLS (client certificate) + OAuth 2.0 Bearer token.

            **Groups:**
            - **Individual** – natural person KYC
            - **Legal** – entity KYC
            - **Common** – update status & fetch updates
            """
    });

    // API Key security definition for Swagger UI "Authorize" button
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API key authentication. Enter your key in the value field. Example: my-secret-api-key-001",
        Name = "X-Api-Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKey"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });

    c.TagActionsBy(api =>
    {
        if (api.ActionDescriptor.RouteValues.TryGetValue("controller", out var controller))
        {
            return controller switch
            {
                "IndividualCkyc" => new[] { "Individual" },
                "LegalCkyc" => new[] { "Legal" },
                "CommonCkyc" => new[] { "Common" },
                _ => new[] { controller ?? "Other" }
            };
        }
        return new[] { "Other" };
    });

    c.OrderActionsBy(api => api.RelativePath);

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CERSAI CKYCRR 2.0 API v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "CERSAI CKYCRR API";
    c.DefaultModelsExpandDepth(1);
    c.DisplayRequestDuration();
    c.EnableTryItOutByDefault();
    c.EnableDeepLinking();
    c.EnableFilter();
});

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
