using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace CreateProjectWithDte
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(CommandCreateHmiPackage.PackageGuidString)]
    public sealed class CommandCreateHmiPackage : AsyncPackage
    {
        public const string PackageGuidString = "1f475c27-0715-432b-902f-94cf629c9103";

        public CommandCreateHmiPackage()
        {
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await CommandCreateHmi.InitializeAsync(this);
        }
    }
}
