using System;

using System.Collections.Generic;
using System;

public class AlertService
{
    private IAlertDAO alertDAO;
    public AlertService(IAlertDAO alert)
    {
        this.alertDAO = alert;
    }

    public Guid RaiseAlert()
    {
        return this.alertDAO.AddAlert(DateTime.Now);
    }

    public DateTime GetAlertTime(Guid id)
    {
        return this.alertDAO.GetAlert(id);
    }
}

public interface IAlertDAO
{
    Guid AddAlert(DateTime time);
    DateTime GetAlert(Guid id);
}

public class AlertDAO : IAlertDAO
{
    private readonly Dictionary<Guid, DateTime> alerts = new Dictionary<Guid, DateTime>();

    public Guid AddAlert(DateTime time)
    {
        Guid id = Guid.NewGuid();
        this.alerts.Add(id, time);
        return id;
    }

    public DateTime GetAlert(Guid id)
    {
        return this.alerts[id];
    }
}

class MalwareAnalysis
{
    public static int[] Simulate(int[] entries, int s)
    {
        int a = entries.Length - 1;
        for (int i = 0; i < entries.Length; i++)
        {
            if (i == s && s > a)
            {
                return entries;
            }
            entries[i] = -1;
            entries[a - i] = -1;
        }
        return entries;
    }

    public static void Main(string[] args)
    {
        //var result = Simulate(new int[] { 4, 1, 3, 5, 4, 7, 9 }, 3);
        //Console.WriteLine(string.Join(", ", result));

        //result = Simulate(new int[] { 7, 6, 9, 1, 8, 3, 2 }, 2);
        //Console.WriteLine(string.Join(", ", result));

        //int n = 10,a = 0,b = 1;
        //for (int i = 0; i < n; i++)
        //{
        //    Console.WriteLine(a);
        //    int c = a + b;
        //    a = b;
        //    b = c;
        //}
        //string a = "hello";
        //string b = "";
        //for (int i = 0; i < a.Length; i++)
        //{
        //    b = a[i] + b;
        //}
        //Console.WriteLine(b);
        int a = 2, b = 3;
        a = a ^ b;
        Console.WriteLine(a);
        b = a ^ b;
        Console.WriteLine(b);
        a = a ^ b;
        Console.WriteLine(a);
        Console.WriteLine(a+" "+b);
    }
}