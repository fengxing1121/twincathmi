using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace MultipleRemoteDeployment
{
    class Program
    {
        public static string PathToHmiProject = @"..\..\..\TcHmiProject1\TcHmiProject1.hmiproj";
        public static string PathToHmiPublishProfile = @"..\..\..\TcHmiProject1\Properties\tchmipublish.config.json";

        public static Dictionary<string, int> TargetRemoteServer = new Dictionary<string, int>
        {
            { "127.0.0.1", 1010 },
            //{ "172.17.62.179", 1010 }
        };

        static void Main(string[] args)
        {
            var localDirname = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            PathToHmiProject = Path.Combine(localDirname, PathToHmiProject);
            PathToHmiPublishProfile = Path.Combine(localDirname, PathToHmiPublishProfile);

            #region do initial hmi build

            var propsBuild = MsBuildInfo.Instance();
            propsBuild.ProjectDirectory = Path.GetDirectoryName(PathToHmiProject);
            var propsDict = propsBuild.GetProperties();
            var hmiBuildRequest = new BuildRequestData(PathToHmiProject, propsDict, null, new string[] { "Build" }, null);
            
            var pc = new ProjectCollection();
            var bp = new BuildParameters(pc) {
                Loggers = new[] { new TargetLogger() }
            };

            var hmiBuildResult = BuildManager.DefaultBuildManager.Build(bp, hmiBuildRequest);

            Console.WriteLine("Result: {0}", hmiBuildResult.OverallResult);

            #endregion

            var json = File.ReadAllText(PathToHmiPublishProfile, Encoding.UTF8);
            dynamic jsonResponse = JArray.Parse(json);

            // make backup
            File.Copy(PathToHmiPublishProfile, PathToHmiPublishProfile + ".backup", true);

            foreach (var it in TargetRemoteServer)
            {
                jsonResponse[0].tcHmiServerHost = it.Key;
                jsonResponse[0].tcHmiServerPort = it.Value;

                #region msbuild magic for publish

                var props = MsBuildInfo.Instance();
                props.ServerAddress = it.Key;
                props.ServerPort = it.Value;
                props.ProfileName = "BasePublishProfile";
                props.ProfilePath = PathToHmiPublishProfile;
                props.LocalServer = "ws://127.0.0.1:3000";
                props.ProjectDirectory = Path.GetDirectoryName(PathToHmiProject);
                props.OutputPath = Path.Combine(props.ProjectDirectory, @"bin\");

                var buildRequest = new BuildRequestData(PathToHmiProject, props.GetProperties(), null, new string[] { "PublishWithoutBuild" }, null);
                var buildResult = BuildManager.DefaultBuildManager.Build(bp, buildRequest);

                Console.WriteLine("Result: {0}", buildResult.OverallResult);

                #endregion
            }

            // rollback original publish configuration
            File.Copy(PathToHmiPublishProfile + ".backup", PathToHmiPublishProfile, true);

            Console.WriteLine("Enter any key...");
            Console.ReadKey();            
        }

        public class MsBuildInfo
        {
            public MsBuildInfo()
            {
                Configuration = "Release";
                Platform = "TwinCAT HMI";
                ServerAddress = "127.0.0.1";
                ServerPort = 1010;
                LocalServer = "ws://127.0.0.1:3000";
                OutputPath = @"bin\";
            }

            public static MsBuildInfo Instance() { return new MsBuildInfo(); }

            public string Configuration { get; set; }
            public string Platform { get; set; }
            public string ServerAddress { get; set; }
            public int ServerPort { get; set; }
            public string ProfileName { get; set; }
            public string ProfilePath { get; set; }
            public string LocalServer { get; set; }
            public string ProjectDirectory { get; set; }
            public string OutputPath { get; set; }

            public Dictionary<string, string> GetProperties()
            {
                var props = new Dictionary<string, string>
                {
                    {"Configuration", Configuration },
                    {"Platform", Platform },
                    {"TcHmi_ServerAddress", ServerAddress },
                    {"TcHmi_ServerPort", $"{ServerPort}" },
                    {"TcHmi_LocalServer", LocalServer },
                    {"TcHmi_ProjectDirectory", ProjectDirectory },
                    {"OutputPath", OutputPath }
                };

                if (!string.IsNullOrEmpty(ProfileName)) props.Add("TcHmi_ProfileName", ProfileName);
                if (!string.IsNullOrEmpty(ProfilePath)) props.Add("TcHmi_ProfilesFilename", ProfilePath);

                return props;
            }
        }

        public class TargetLogger : Microsoft.Build.Logging.ConsoleLogger
        {
            public TargetLogger()
            {
                Verbosity = LoggerVerbosity.Detailed;
                ShowSummary = true;
                SkipProjectStartedText = false;
            }
        }
    }
}
