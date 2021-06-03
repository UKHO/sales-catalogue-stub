using Microsoft.Extensions.Configuration;

namespace PostManTests
{

    public class ConfigBuilder
    {
        public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("local.settings.json", optional: true)
                .Build();
        }

        public static PostManTestsConfig GetApplicationConfiguration(string outputPath)
        {
            var configuration = new PostManTestsConfig();

            var iConfig = GetIConfigurationRoot(outputPath);

            iConfig
                .GetSection("PostManTestsConfig")
                .Bind(configuration);

            return configuration;
        }
    }
}