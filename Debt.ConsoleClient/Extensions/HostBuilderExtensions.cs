using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Debt.ConsoleClient.Extensions;

internal static class HostBuilderExtensions
{
    public static HostApplicationBuilder ConfigureEmbeddedConfiguration(this HostApplicationBuilder builder)
    {
        builder.Configuration.SetFileProvider(new EmbeddedFileProvider(typeof(HostBuilderExtensions).Assembly));
        var jsonFiles = Directory.EnumerateFiles(
            Resource.ConfigFolderName, Resource.ConfigAllJsonFilesPattern, SearchOption.AllDirectories);
        foreach (var path in jsonFiles)
            builder.Configuration.AddJsonFile(path, optional: true);
        builder.Configuration.AddJsonFile(Resource.AppSettingsJsonFileName, optional: false);
        return builder;
    }
}