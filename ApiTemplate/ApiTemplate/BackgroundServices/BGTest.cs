
namespace TestApi.BackgroundServices
{
    public class BGTest : BackgroundService
    {
        protected int a = 0;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Background service start");
            while (!stoppingToken.IsCancellationRequested)
            {
                for (int i = 0; i < 10; i++)
                {
                    a += i;
                    Console.WriteLine(i.ToString());
                }
                // Simulate work (e.g., call to database, external API)
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            Console.WriteLine("background service end");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("StopAsync triggered. Cleaning up...");
            if (a == 100)
            {
                // Optional: Wait for something to finish
                await base.StopAsync(cancellationToken);
            }
        }
    }
}
