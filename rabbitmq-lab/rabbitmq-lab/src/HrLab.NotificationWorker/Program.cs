using HrLab.Messaging;
using HrLab.NotificationWorker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitOptions>(builder.Configuration.GetSection(RabbitOptions.Section));
builder.Services.AddHostedService<LeaveApprovedNotifier>();

builder.Build().Run();
