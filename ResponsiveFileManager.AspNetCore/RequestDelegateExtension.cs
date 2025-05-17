using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Pchp.Core;
using ResponsiveFileManager;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// <see cref="IApplicationBuilder"/> extension for enabling Responsive File Manager.
/// </summary>
public static class RequestDelegateExtension
{
    const string filemanagerPath = "filemanager";

    static string FileManagerPath()
    {
        var defaultPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), filemanagerPath);

        foreach (var path in new string[] {
            defaultPath,
            Path.Combine(Directory.GetCurrentDirectory(), filemanagerPath),
        })
        {
            if (Directory.Exists(path))
            {
                return Path.GetFullPath(path);
            }
        }

        // default
        return defaultPath;
    }

    /// <summary>
    /// Installs ResponsiveFileManager middleware.
    /// </summary>
    /// <param name="app">The application builder.</param>
    public static IApplicationBuilder UseResponsiveFileManager(this IApplicationBuilder app)
    {
        var root = Path.GetDirectoryName(FileManagerPath()); // parent of 'filemanager' is root of the app
        var requestPath = new PathString($"/{filemanagerPath}");

        app.UseStaticFiles(new StaticFileOptions
        {
            RequestPath = requestPath,
            FileProvider = new PhysicalFileProvider(Path.Combine(root, filemanagerPath)),
        });

        app.UsePhp(
            requestPath, // filter by prefix
            ctx =>
            {
                // construct the options
                var options = new ResponsiveFileManagerOptions();
                ctx.GetService<IConfiguration>().GetSection(nameof(ResponsiveFileManagerOptions)).Bind(options);
                ctx.GetService<IConfigureOptions<ResponsiveFileManagerOptions>>()?.Configure(options);

                // pass the options object to PHP globals
                ctx.Globals["rfm_options"] = PhpValue.FromClass(options);
            },
            root // root of the app
        );

        return app;
    }
}