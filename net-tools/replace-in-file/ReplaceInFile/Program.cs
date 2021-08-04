using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ReplaceInFile
{
    class Program
    {
        static void Main(string[] args)
        {
            //var folder = @"D:\SimplBit\git\JetWallet\_infrastructure\kubernates-infrastructure\services";
            var folder = @"D:\SimplBit\git\JetWallet";


            //ReplaceImages(folder, "*.yam*");
            //ReplaceInFiles(folder, "deployment.yaml", "-test:", ":");
            //FindFileWithoutText(folder, "deployment.yaml", "agentpool: spot");
            //ReplaceInFilesAppInsight(folder);
            //FindFileWithText(folder, "deployment.yaml", "-test:");

            //ReplaceInFiles(folder, "update_nuget.yaml", "uses: actions-js/push@master", "MyJetWallet/push@master");
            ReplaceInFiles(folder, "update_nuget.yaml", "   MyJetWallet/push@master", "   uses: MyJetWallet/push@master");
        }

        private static void ReplaceInFiles(string folder, string pattern, string searchString, string replaceString)
        {
            string[] fileEntries = Directory.GetFiles(folder, pattern, SearchOption.AllDirectories);

            foreach (var file in fileEntries)
            {
                var data = File.ReadAllText(file);

                if (data.Contains(searchString))
                {
                    data = data.Replace(searchString, replaceString);

                    File.WriteAllText(file, data);

                    Console.WriteLine(file);
                }
            }
        }

        private static void FindFileWithoutText(string folder, string pattern, string searchString)
        {
            string[] fileEntries = Directory.GetFiles(folder, pattern, SearchOption.AllDirectories);

            foreach (var file in fileEntries)
            {
                var data = File.ReadAllText(file);

                if (!data.Contains(searchString))
                {
                    Console.WriteLine(file);
                }
            }
        }

        private static void FindFileWithText(string folder, string pattern, string searchString)
        {
            string[] fileEntries = Directory.GetFiles(folder, pattern, SearchOption.AllDirectories);

            foreach (var file in fileEntries)
            {
                var data = File.ReadAllText(file);

                if (data.Contains(searchString))
                {
                    Console.WriteLine(file);
                }
            }
        }

        private static void ReplaceImages(string folder, string pattern)
        {
            string[] fileEntries = Directory.GetFiles(folder, pattern, SearchOption.AllDirectories);

            foreach (var file in fileEntries)
            {
                var data = File.ReadAllText(file);

                if (data.Contains("docker.pkg.github.com"))
                {
                    var p = data.Split("\r\n");
                    //data = data.Replace(searchString, replaceString);

                    

                    foreach (var s in p)
                    {
                        if (s.Contains("image:"))
                        {
                            var items = s.Split("/");
                            var name = items.Last();
                            var newImage = $"image: simpletrading.azurecr.io/spot/{name}";

                            var s1 = s.Trim();
                            data = data.Replace(s1, newImage);
                        }
                    }

                    File.WriteAllText(file, data);
                    Console.WriteLine(file);
                }
            }
        }

        private static void ReplaceInFilesAppInsight(string folder)
        {
            var pattern = "deployment.yaml";

            string[] fileEntries = Directory.GetFiles(folder, pattern, SearchOption.AllDirectories);

            foreach (var file in fileEntries)
            {
                var data = File.ReadAllText(file);

                var lines = data.Split("\r\n");

                var key = "";
                var value = "";

                for (int i = 0; i < lines.Length-1; i++)
                {
                    if (lines[i].Contains("APPINSIGHTS_INSTRUMENTATIONKEY"))
                    {
                        key = lines[i + 1];
                        value = key.Split(":")[0] + ":";
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(key))
                {
                    data = data.Replace(key, value);

                    File.WriteAllText(file, data);

                    Console.WriteLine(file);
                    Console.WriteLine($"  key:   {key}");
                    Console.WriteLine($"  value: {value}");
                }
            }
        }


        //image: 
    }
}
