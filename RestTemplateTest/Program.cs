using System;

namespace RestTemplateTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string serviceUrl = "http://UserServiceAPI/api/user";
            RestTemplate rest = new RestTemplate();
            var user = rest.GetForEntityAsync<User>(serviceUrl).Result;
            Console.WriteLine($"{user.Body.ServerTime}");
            Console.ReadKey();
        }

        
    }

    public class User
    {
        public DateTime ServerTime { get; set; }

        public string Name { get; set; }
    }
}
