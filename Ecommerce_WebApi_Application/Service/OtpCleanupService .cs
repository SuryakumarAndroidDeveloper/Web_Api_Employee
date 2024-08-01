using Ecommerce_WebApi_Application.DataAcessLayer;

namespace Ecommerce_WebApi_Application.Service
{
    public class OtpCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;

        public OtpCleanupService(IServiceScopeFactory scopeFactory, IConfiguration config)
        {
            _scopeFactory = scopeFactory;
            _config = config;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                await CleanupExpiredOtpsAsync();
            }
        }

        private async Task CleanupExpiredOtpsAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var otpRequestDal = scope.ServiceProvider.GetRequiredService<LoginDAL>();
                await otpRequestDal.CleanupExpiredOtpsAsync();
            }
        }
    }
}
