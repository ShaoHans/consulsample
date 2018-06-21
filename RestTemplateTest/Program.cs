using System;
using System.Collections.Generic;

namespace RestTemplateTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string serviceRootUrl = "http://UserServiceAPI";
            RestTemplate rest = new RestTemplate();
            //Console.ReadKey();

            var postResponse = rest.PostForEntityAsync<bool>($"{serviceRootUrl}/api/user/add",
                new User { Id = 7, Name = "shz", Gender = "男", Birthday = DateTime.Now }).Result;
            if(postResponse.StatusCode== System.Net.HttpStatusCode.OK)
            {
                if(postResponse.Body)
                {
                    Console.WriteLine("添加用户成功");
                }
                else
                {
                    Console.WriteLine("添加用户失败");
                }
            }
            else
            {
                Console.WriteLine("调用接口失败");
            }

            var restResponse = rest.GetForEntityAsync<List<User>>($"{serviceRootUrl}/api/user").Result;
            if(restResponse.StatusCode== System.Net.HttpStatusCode.OK)
            {
                foreach (var user in restResponse.Body)
                {
                    Console.WriteLine($"姓名：{user.Name}，性别：{user.Gender}，年龄：{user.Birthday}");
                }
            }
            else
            {
                Console.WriteLine($"调用接口失败：{ restResponse.StatusCode}");
            }

            Console.WriteLine("===========================");
            var getResponse = rest.GetForEntityAsync<User>($"{serviceRootUrl}/api/user/1").Result;
            if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                User user = getResponse.Body;
                user.Name = "新名字叫" + user.Name;
                rest.PutForEntityAsync<bool>($"{serviceRootUrl}/api/user/edit", user).Wait();

                getResponse = rest.GetForEntityAsync<User>($"{serviceRootUrl}/api/user/1").Result;
                user = getResponse.Body;
                Console.WriteLine(user.Name);
            }
            else
            {
                Console.WriteLine($"调用接口失败：{ restResponse.StatusCode}");
            }


            Console.ReadKey();
        }

        
    }

    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Birthday { get; set; }

        public string Gender { get; set; }
    }
}
