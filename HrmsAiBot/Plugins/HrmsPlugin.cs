using Microsoft.SemanticKernel;
using System.ComponentModel;
using Service;

public class HrmsPlugin
{
    private readonly ILeaveService _leaveService;

    public HrmsPlugin(ILeaveService leaveService)
    {
        _leaveService = leaveService;
    }

    [KernelFunction]
    [Description("Gets the current leave balance for the user. Use this when user asks about their leave count, leave balance, remaining leaves, or how many days off they have.")]
    public async Task<string> GetLeaveBalance()
    {
        // Normally get from JWT token
        int userId = 1023;

        var balance = await _leaveService.GetBalanceAsync(userId);

        return $"You have {balance} days of leave remaining.";
    }
}
