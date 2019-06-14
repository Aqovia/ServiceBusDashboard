using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Newtonsoft.Json;
using ServiceBusDashboard.Model;

namespace ServiceBusDashboard.Code
{
    public class SbConnectionStringsLoader
    {
        private static SbConnectionStringsLoader _instance;
        public SbConnectionStrings ConnectionStrings { get; private set; }

        public static SbConnectionStringsLoader Instance => _instance ?? (_instance = new SbConnectionStringsLoader());

        private SbConnectionStringsLoader()
        {
            Reload();
        }

        public void Reload()
        {
            ConnectionStrings = Load();
        }

        public SbConnectionStringInfo FindConnectionString(string groupName, string name)
        {
            var connectionString = ConnectionStrings
                .Groups
                .Where(g =>
                    g.Name == groupName
                )
                .Select(c => c.Items
                    .Where(_ => _.Name == name)
                    .Select(_ => _.ConnectionString)
                    .FirstOrDefault())
                .FirstOrDefault(c => !string.IsNullOrEmpty(c));

            if (string.IsNullOrEmpty(connectionString))
                return null;

            return new SbConnectionStringInfo
            {
                ConnectionStringGroup = groupName,
                ConnectionStringName = name,
                ConnectionString = connectionString
            };
        }

        private static SbConnectionStrings Load()
        {
            var rootPath = HostingEnvironment.MapPath("~/");
            var connectionStrings = ConfigurationManager.AppSettings["SbConnectionStrings"];
            var connectionStringsFile = new FileInfo(Path.Combine(rootPath, connectionStrings));
            if (!connectionStringsFile.Exists)
            {
                return new SbConnectionStrings
                {
                    Groups =
                    new[]
                    {
                        new SbConnectionStringGroup
                        {
                            Items = new[]
                            {
                                new SbConnectionString
                                {
                                    Name = connectionStrings,
                                    ConnectionString = connectionStrings
                                }
                            }
                        }
                    }
                };
            }

            var json = File.ReadAllText(connectionStringsFile.FullName);
            return JsonConvert.DeserializeObject<SbConnectionStrings>(json);
        }
    }

    public class SbConnectionStrings
    {
        public SbConnectionStringGroup[] Groups { get; set; }
    }

    public class SbConnectionStringGroup
    {
        public string Name { get; set; }
        public SbConnectionString[] Items { get; set; }

        public SbConnectionStringGroup()
        {
            Items = new SbConnectionString[0];
        }
    }

    public class SbConnectionString
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
    }
}