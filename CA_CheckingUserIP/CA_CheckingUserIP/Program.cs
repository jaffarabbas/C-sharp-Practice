using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace CA_CheckingUserIP
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter IP address to check:");
            string userIp = Console.ReadLine();

            bool isOnLan = IsIpOnSameNetwork(userIp);

            if (isOnLan)
            {
                Console.WriteLine($"{userIp} is on the same LAN as this machine.");
            }
            else
            {
                Console.WriteLine($"{userIp} is not on the same LAN as this machine.");
            }

            Console.ReadLine();
        }

        static bool IsIpOnSameNetwork(string userIp)
        {
            string[] userIpParts = userIp.Split('.');
            string[] currentIpParts = GetLocalIPAddress().Split('.');
            string[] subnetMaskParts = GetSubnetMask().Split('.');

            for (int i = 0; i < 4; i++)
            {
                int userIpPart = int.Parse(userIpParts[i]);
                int currentIpPart = int.Parse(currentIpParts[i]);
                int subnetMaskPart = int.Parse(subnetMaskParts[i]);

                if ((userIpPart & subnetMaskPart) != (currentIpPart & subnetMaskPart))
                {
                    return false;
                }
            }

            return true;
        }

        static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        static string GetSubnetMask()
        {
            string subnetMask = null;

            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var networkInterface in networkInterfaces)
            {
                var ipProperties = networkInterface.GetIPProperties();
                foreach (var unicastAddress in ipProperties.UnicastAddresses)
                {
                    if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        subnetMask = unicastAddress.IPv4Mask.ToString();
                        break;
                    }
                }

                if (subnetMask != null)
                {
                    break;
                }
            }

            if (subnetMask == null)
            {
                throw new Exception("Unable to determine subnet mask.");
            }

            return subnetMask;
        }
    }
}
