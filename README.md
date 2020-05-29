[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=gordon_matt%40live%2ecom&lc=AU&currency_code=AUD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted)

![Peachpie Responsive File Manager](https://github.com/gordon-matt/peachpie-responsive-file-manager/raw/master/Misc/logo.png)

# peachpie-responsive-file-manager
Responsive File Manager running on .NET Core with Peachpie

## Getting Started

1. Get the ResponsiveFileManager NuGet package from: https://www.nuget.org/packages/ResponsiveFileManager/

2. Create the following class:

```csharp
public class ResponsiveFileManagerOptions
{
    /// <summary>
    /// Path from base_url to base of upload folder. Use start and final /
    /// </summary>
    public string UploadDirectory { get; set; }

    /// <summary>
    /// Relative path from filemanager folder to upload folder. Use final /
    /// </summary>
    public string CurrentPath { get; set; }

    /// <summary>
    /// Relative path from filemanager folder to thumbs folder. Use final / and DO NOT put inside upload folder.
    /// </summary>
    public string ThumbsBasePath { get; set; }
}
```

3. Add the following to your **appsettings.json**:

```json
"ResponsiveFileManagerOptions": {
    // Path from base_url to base of upload folder. Use start and final /
    "UploadDirectory": "/Media/Uploads/",

    // Relative path from filemanager folder to upload folder. Use final /
    "CurrentPath": "../Media/Uploads/",

    // Relative path from filemanager folder to thumbs folder. Use final / and DO NOT put inside upload folder.
    "ThumbsBasePath": "../Media/Thumbs/"
}
```

4. Open your `Startup.cs` and ensure it looks something like this:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // etc
	
    // Adds a default in-memory implementation of IDistributedCache.
    services.AddDistributedMemoryCache();

    services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
    });
	
    // etc
}

public void Configure(IApplicationBuilder app)
{
    // etc

    app.UseSession();

    var rfmOptions = new ResponsiveFileManagerOptions();
    Configuration.GetSection("ResponsiveFileManagerOptions").Bind(rfmOptions);
	
    string filemanager = Path.GetFullPath(Path.Combine(Assembly.GetEntryAssembly().Location, "../filemanager"));

    app.UseDefaultFiles();
    app.UseStaticFiles(); // For default wwwroot location
    app.UseStaticFiles(new StaticFileOptions()
    {
        FileProvider = new PhysicalFileProvider(filemanager)
    });

    app.UsePhp(new PhpRequestOptions(scriptAssemblyName: "ResponsiveFileManager")
    {
        BeforeRequest = (Context ctx) =>
        {
            ctx.Globals["appsettings"] = new PhpArray()
            {
                { "upload_dir", rfmOptions.UploadDirectory },
                { "current_path", rfmOptions.CurrentPath },
                { "thumbs_base_path", rfmOptions.ThumbsBasePath }
            };
        }
    });

    // etc
}
```

You can use the source code in this repo, as follows:

1. Open the solution in Visual Studio 2017 or newer.
2. Set the **WebApplication** project as the default, if it 
isn't already.
3. restore libs in WebApplication folder:
  - `dotnet tool install -g Microsoft.Web.LibraryManager.Cli`
  - `libman restore`
4. Run and test one of the 3 demo pages
5. Look at the `Startup.cs` file for configuration to copy 
to your own project to use with the NuGet package.

## Donate
If you find this project helpful, consider buying me a cup of coffee.  :-)

#### PayPal:

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=gordon_matt%40live%2ecom&lc=AU&currency_code=AUD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted)

#### Crypto:
- **Bitcoin**: 1EeDfbcqoEaz6bbcWsymwPbYv4uyEaZ3Lp
- **Ethereum**: 0x277552efd6ea9ca9052a249e781abf1719ea9414
- **Litecoin**: LRUP8hukWGXRrcPK6Tm7iUp9vPvnNNt3uz
