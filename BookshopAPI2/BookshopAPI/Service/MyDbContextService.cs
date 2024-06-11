using BookshopAPI.Models;

namespace BookshopAPI.Service
{
    public class MyDbContextService
    {
        static IConfiguration configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .Build();
        public MyDbContext GetMyDbContext()
        {
            return new MyDbContext(
                configuration);
        }
        public IConfiguration GetConfiguration()
        {
            return configuration;
        }
                
    }
}
