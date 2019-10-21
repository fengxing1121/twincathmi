using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace CreateProjectWithDte
{
    internal partial class CommandCreateHmi
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("d6e70a2d-a8d9-4445-b89d-a045ae54a5df");
        private readonly AsyncPackage package;
        
        private CommandCreateHmi(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static CommandCreateHmi Instance
        {
            get;
            private set;
        }

        private IAsyncServiceProvider ServiceProvider => this.package;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            var commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new CommandCreateHmi(package, commandService);
        }                
    }
}
