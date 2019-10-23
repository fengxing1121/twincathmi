namespace IntellisenseCreateControl
{
    using System;
    using System.Collections.ObjectModel;
    using EnvDTE;

    public static class Utilities
    {
        public static Project GetProjectContainingFile(IServiceProvider hostServiceProvider, string item)
        {
            var dte = (DTE)hostServiceProvider.GetService(typeof(EnvDTE.DTE));
            var projectItem = dte.Solution.FindProjectItem(item);
            return projectItem.ContainingProject;
        }

        public static Microsoft.Html.Core.Tree.Nodes.AttributeNode Get(
            this ReadOnlyCollection<Microsoft.Html.Core.Tree.Nodes.AttributeNode> attributes,
            string attributeName)
        {
            if (string.IsNullOrEmpty(attributeName)) return null;
            foreach(var it in attributes)
            {
                if (it == null) continue;
                var name = it.Name;
                if (string.IsNullOrEmpty(name)) continue;
                if (name.Equals(attributeName, StringComparison.OrdinalIgnoreCase)) return it;
            }
            return null;
        }

        public static string GenerateRandomIdentifier(this string baseIdentifier)
        {
            var random = new Random();
            var hopefullyUnique = string.Format("{0:X6}", random.Next(0x1000000));
            return $"{baseIdentifier}_{hopefullyUnique}";
        }
    }
}
