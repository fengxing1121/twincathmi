using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace VsixShowAllAttributes
{
    internal sealed class ShowAttributesCommand
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("761179d0-b0a0-4265-9ef0-17430a473096");
        private readonly AsyncPackage package;

        private ShowAttributesCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static ShowAttributesCommand Instance
        {
            get;
            private set;
        }

        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider => package;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            var commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new ShowAttributesCommand(package, commandService);
        }

        private async void Execute(object sender, EventArgs e)
        {
            var dte = await package.GetServiceAsync(typeof(EnvDTE.DTE)).ConfigureAwait(false) as EnvDTE80.DTE2;
            var dteProject = GetActiveProject(dte as EnvDTE.DTE);
            var hmiProject = dteProject?.Object as TcHmiCore.ITcHmiProjectNodeBase;
            if (hmiProject == null) return;

            var ctrlProvider = hmiProject?.GetControlProvider();
            if (ctrlProvider == null) return;

            var sb = new StringBuilder();

            foreach(var ctrl in ctrlProvider.AvailableControls)
            {
                if (ctrl == null) continue;
                sb.AppendLine($"{ctrl.DisplayName} ({ctrl.ClassName})");
                var attrs = ctrl.Attributes;
                foreach (var attr in attrs)
                {
                    var defaultValue = attr.DefaultValue;
                    var defaultValueInternal = attr.DefaultValueInternal;
                    
                    defaultValue = CleanUp(ref defaultValue);
                    defaultValueInternal = CleanUp(ref defaultValueInternal);

                    sb.AppendLine($" > {attr.Name}  ({defaultValue} / {defaultValueInternal})");
                }

                sb.AppendLine(string.Empty);
            }

            var tmpFile = System.IO.Path.GetTempFileName() + ".txt";
            System.IO.File.WriteAllText(tmpFile, sb.ToString(), Encoding.UTF8);

            Process.Start(@"C:\Program Files\Notepad++\notepad++.exe", tmpFile);
        }

        private string CleanUp(ref string txt)
        {
            if (string.IsNullOrEmpty(txt)) return string.Empty;
            txt = txt.Replace("\r", string.Empty);
            txt = txt.Replace("\n", string.Empty);
            return txt;
        }

        /// <remarks>https://blog.mastykarz.nl/active-project-extending-visual-studio-sharepoint-development-tools-tip-1/</remarks>
        internal static EnvDTE.Project GetActiveProject(EnvDTE.DTE dte)
        {
            EnvDTE.Project activeProject = null;
            var activeSolutionProjects = dte?.ActiveSolutionProjects as Array;
            if (activeSolutionProjects != null && activeSolutionProjects.Length > 0)
                activeProject = activeSolutionProjects.GetValue(0) as EnvDTE.Project;
            return activeProject;
        }
    }
}
