using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Seljmov.Blazor.Identity.Shared;
using Shared;
using ConfigurationOptions = Shared.ConfigurationOptions;

namespace Client.Auth;

/// <summary>
/// Методы расширения для построителя хоста WebAssembly.
/// </summary>
public static class WebAssemblyHostBuilderExtensions
{
    /// <summary>
    /// Построить хост с авторизацией.
    /// </summary>
    /// <param name="builder">Построитель хоста WebAssembly.</param>
    /// <returns>Хост WebAssembly.</returns>
    public static async Task<WebAssemblyHost> BuildWithAuthorizationAsync(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddScoped<TokenRepository>();
        builder.Services.AddSingleton<AuthStateProvider>();
        builder.Services.AddSingleton<AuthenticationStateProvider>(provider => provider.GetRequiredService<AuthStateProvider>());
        
        var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
        builder.Services.AddScoped(_ => httpClient);

        var configurationOptions = await httpClient.GetFromJsonAsync<ConfigurationOptions>(RouteConstants.ConfigurationData.GetOptionsUrl());
        builder.RegisterServerClients(configurationOptions);
        
        // Регистрация сервиса аутентификации
        builder.Services.AddScoped<AuthService>(sp =>
            new AuthService(
                httpClient: sp.GetRequiredService<AuthHttpClientFactory>().CreateAuthenticatedClient(),
                authStateProvider: sp.GetRequiredService<AuthStateProvider>(),
                authFlow: sp.GetRequiredService<AuthFlow>()
            ));
        
        builder.Services.AddScoped<AuthFlow>(sp =>
            new AuthFlow(
                httpClient: httpClient,
                authStateProvider: sp.GetRequiredService<AuthStateProvider>(),
                navigation: sp.GetRequiredService<NavigationManager>()
            ));

        builder.Services.AddAuthorizationCore(options =>
        {
            foreach (var policy in AuthPolicies.AllPolicies.Concat(["123"]))
            {
                options.AddPolicy(policy, policyBuilder =>
                {
                    policyBuilder.RequireAssertion(context =>
                    {
                        return context.User.Claims.Any(claim => claim.Type == ClaimTypes.Role && claim.Value.Contains(policy, StringComparison.InvariantCultureIgnoreCase));
                    });
                });
            }
        });
        
        var host = builder.Build();
        await host.Services.GetRequiredService<AuthStateProvider>().InitializeAuthenticationStateAsync();
        
        return host;
    }

    private static void RegisterServerClients(this WebAssemblyHostBuilder builder, ConfigurationOptions? options)
    {
        builder.Services.AddScoped<AuthHttpClientHandler>();
        
        builder.Services.AddHttpClient(AuthHttpClientFactory.AuthHttpClientName, client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
        if (options is null)
        {
            foreach (var clientName in AuthHttpClientFactory.HttpClientNames.Except([AuthHttpClientFactory.AuthHttpClientName]))
            {
                builder.Services
                    .AddHttpClient(clientName, client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                    .AddHttpMessageHandler<AuthHttpClientHandler>();
            }
        }
        else
        {
            builder.Services
                .AddHttpClient(AuthHttpClientFactory.UsersHttpClientName, client => client.BaseAddress = new Uri(options.UsersServerUrl))
                .AddHttpMessageHandler<AuthHttpClientHandler>();
        }
        
        builder.Services.AddScoped<AuthHttpClientFactory>();
    }
}
