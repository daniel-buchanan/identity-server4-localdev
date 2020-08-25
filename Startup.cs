// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.IO;
using IdentityServer4;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace identity_server4_localdev
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var rootPath = Environment.ContentRootPath;
            if(!rootPath.EndsWith(Path.DirectorySeparatorChar))
                rootPath = rootPath + Path.DirectorySeparatorChar;

            var configPath = $"config{Path.DirectorySeparatorChar}config.json";

            var configurationPath = Path.Combine(rootPath, configPath);
            if(!File.Exists(configurationPath))
                throw new FileNotFoundException($"The Configuration file 'config.json' was not found at the path: {configurationPath}");

            var configContents = File.ReadAllText(configurationPath);
            var config = JsonConvert.DeserializeObject<Config>(configContents, new JsonClaimConverter());

            services.AddControllersWithViews();

            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;

                    // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                    options.EmitStaticAudienceClaim = true;
                    options.MutualTls.Enabled = false;
                })
                .AddTestUsers(config.Users);

            // in-memory, code config
            builder.AddInMemoryIdentityResources(config.IdentityResources);
            builder.AddInMemoryApiScopes(config.Scopes);
            builder.AddInMemoryClients(config.Clients);

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            services.AddAuthentication();
                // .AddGoogle(options =>
                // {
                //     options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                //     // register your IdentityServer with Google at https://console.developers.google.com
                //     // enable the Google+ API
                //     // set the redirect URI to https://localhost:5001/signin-google
                //     options.ClientId = "copy client ID from Google here";
                //     options.ClientSecret = "copy client secret from Google here";
                // });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}