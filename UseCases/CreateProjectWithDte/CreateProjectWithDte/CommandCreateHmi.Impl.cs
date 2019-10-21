using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE80;

namespace CreateProjectWithDte
{
    internal partial class CommandCreateHmi
    {
        public const string DefaultHmiTplCategory = "TwinCAT HMI";
        public const string DefaultHmiProjectTplName = "TwinCAT HMI Project";

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = (DTE2)Package.GetGlobalService(typeof(SDTE));
            var projectBaseName = "HmiProject";
            var slnDirPath = @"C:\temp";

            EnsureSingleProject(dte, projectBaseName, slnDirPath, DefaultHmiProjectTplName);
        }

        public static EnvDTE.Project EnsureSingleProject(
            DTE2 dte,
            string basename,
            string slndir = "",
            string tplName = DefaultHmiProjectTplName
            )
        {
            string solutionDirectory = null;
            if (!string.IsNullOrEmpty(dte.Solution.FullName))
                solutionDirectory = Directory.GetParent(dte.Solution.FullName).FullName;
            else
                solutionDirectory = slndir;

            var prj = CreateProject(
                dte,
                DefaultHmiTplCategory,
                tplName,
                solutionDirectory,
                basename,
                false,
                true
            );

            return prj;
        }

        public static EnvDTE.Project CreateProject(
            DTE2 dte,
            string languageName,
            string templateName,
            string createLocation,
            string projectName,
            bool newSolution = false,
            bool suppressUi = true
        )
        {
            var sln = dte.Solution as Solution2;
            if (sln == null) return null;
            var templatePath = sln.GetProjectTemplate(templateName, languageName);

            var origName = projectName;
            var projectDir = Path.Combine(createLocation, projectName);
            for (int i = 1; Directory.Exists(projectDir); ++i)
            {
                projectName = $"{origName}{i}";
                projectDir = Path.Combine(createLocation, projectName);
            }

            var previousSuppressUi = dte.SuppressUI;

            try
            {
                dte.SuppressUI = suppressUi;
                sln.AddFromTemplate(templatePath, projectDir, projectName, newSolution);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }

            try
            {
                dte.SuppressUI = previousSuppressUi;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }

            var prjs = sln.Projects;
            foreach (EnvDTE.Project prj in prjs)
            {
                if (prj == null) continue;

                var name = string.Empty;

                try
                {
                    name = prj.Name;
                }
                catch
                {
                    // ignore
                }

                if (string.IsNullOrEmpty(name))
                {
                    try
                    {
                        name = Path.GetFileNameWithoutExtension(prj.FileName);
                    }
                    catch
                    {
                        // ignore
                    }
                }

                if (string.IsNullOrEmpty(name)) continue;
                if (name.StartsWith(projectName, StringComparison.OrdinalIgnoreCase))
                    return prj;
            }

            try
            {
                return sln.Projects.Cast<EnvDTE.Project>()
                        .FirstOrDefault(p => p.Name.StartsWith(projectName, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }

            return null;
        }

        public static void ShowErrorMessage(Exception ex)
        {
            // TODO implement your own message handling
        }
    }
}
