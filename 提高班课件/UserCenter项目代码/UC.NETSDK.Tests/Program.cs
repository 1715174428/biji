using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserCenter.NETSDK;

namespace UC.NETSDK.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            UserApi user = new UserApi("fasdf2236afasdZ98", "fsadfa$900jiuy7832yhuXz", "http://127.0.0.1:8888/api/v1/");
            long id = user.AddNewAsync("137", "aaa", "123").Result;
            Console.WriteLine(id);*/
            /*
            UserGroupApi api = new UserGroupApi("fasdf2236afasdZ98", "fsadfa$900jiuy7832yhuXz", "http://127.0.0.1:8888/api/v1/");
            var groups = api.GetGroupsAsync(1).Result;
            foreach(var g in groups)
            {
                Console.WriteLine(g.Id+","+g.Name);
            }*/
            UserApi user = new UserApi("fasdf2236afasdZ98", "fsadfa$900jiuy7832yhuXz", "http://127.0.0.1:8888/api/v1/");
           User u  = user.GetById(1).Result;
            Console.WriteLine(u.Id+","+u.NickName+","+u.PhoneNum);
            Console.ReadKey();
        }
    }
}
