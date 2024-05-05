using System;
using System.Net.NetworkInformation;

namespace AutoControl
{
    static class InternetCheck
    {
        public static bool Internet_test()
        {
            Ping ping = new Ping();
            try
            {
                PingReply reply = ping.Send("www.digikala.com");
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
