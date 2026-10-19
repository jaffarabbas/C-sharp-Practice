using HrLab.Messaging;
using HrLab.PayrollWorker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitOptions>(builder.Configuration.GetSection(RabbitOptions.Section));
builder.Services.AddHostedService<LeaveApprovedPayrollHandler>();

builder.Build().Run();
