namespace SimpleUptime.MasterApp.Infrastructure
{
    ////var configuration = new ConfigurationBuilder()
    ////    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
    ////    .AddJsonFile("local.appsettings.json", optional: false, reloadOnChange: true)
    ////    .Build();

    ////// settings
    ////services.AddOptions();
    ////services.Configure<DocumentClientSettings>(options =>
    ////{
    ////    var settings = configuration.GetSection("DocumentClientSettings");
    ////    options.ServiceEndpoint = new Uri(settings["ServiceEndpoint"]);
    ////    options.AuthKey = settings["AuthKey"];
    ////});

    ////// serivces
    ////services.AddTransient<IHttpMonitorRepository, HttpMonitorDocumentRepository>();
    ////services.AddSingleton<IDocumentClient>(provider => DocumentClientFactory.CreateDocumentClientAsync(provider.GetService<IOptions<DocumentClientSettings>>().Value).Result);
    ////services.AddTransient<DatabaseConfigurations>(_ => DatabaseConfigurations.Create());
    ////services.AddTransient<SimpleUptimeDbScript>();

    ////services.AddTransient<ICheckHttpMonitorPublisherService, CheckHttpMonitorPublisherService>();
    ////services.AddTransient<ICheckHttpEndpointPublisher, CheckHttpEndpointPublisher>();
    ////services.AddTransient<ITopicClient, TopicClient>(provider => null);// todo
    ////services.AddTransient<JsonSerializer>(); // todo

}
