namespace IntellisenseCreateControl
{
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Web.Editor.Imaging;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Utilities;
    using Microsoft.Html.Editor.Completion;
    using Microsoft.Html.Editor.Completion.Def;
    using Microsoft.Web.Core.ContentTypes;
    using System.ComponentModel.Composition;
    using vs = Microsoft.VisualStudio.Language.Intellisense;

    using TcHmiCore;
    using TcHmiCore.Utils;
    using TcHmiCore.DataAccess;

    [Order(Before = "default")]
    [HtmlCompletionProvider(CompletionTypes.Attributes, CompletionName)]
    [ContentType(HtmlContentTypeDefinition.HtmlContentType)]
    public class CreateControlCompletion : IHtmlCompletionListProvider
    {
        [Import]
        internal SVsServiceProvider ServiceProvider = null;

        public const string CompletionName = "div";
        protected static ReadOnlyCollection<HtmlCompletion> Empty { get; } = new ReadOnlyCollection<HtmlCompletion>(new HtmlCompletion[0]);
        private static Dictionary<ITcHmiControl, ImageSource> _icons = new Dictionary<ITcHmiControl, ImageSource>();
        private readonly ImageSource _glyph = GlyphService.GetGlyph(vs.StandardGlyphGroup.GlyphGroupVariable, vs.StandardGlyphItem.GlyphItemFriend);

        public IList<HtmlCompletion> GetEntries(HtmlCompletionContext ctx)
        {
            var hmiPrj = GetHmiProject(ctx);
            if (hmiPrj == null) return Empty;

            var attrs = ctx.Element.Attributes;
            if (attrs.Get("data-tchmi-type") != null) return null;

            var ctrlProvider = hmiPrj.GetControlProvider();
            if (ctrlProvider?.AvailableControls == null) return Empty;

            var ctrlList = new List<HtmlCompletion>();

            foreach (var it in ctrlProvider.AvailableControls)
            {
                if (it == null) continue;
                if (!it.Visible) continue;

                var typeName = it.Name;
                var displayText = $"Create {it.DisplayName}";
                var identifier = typeName.ToCamelCase().GenerateRandomIdentifier();

                var entries = new Dictionary<string, string>();

                if (attrs.Get("id") == null) entries.Add("id", identifier);
                if (attrs.Get("data-tchmi-type") == null) entries.Add("data-tchmi-type", typeName);
                if (attrs.Get("data-tchmi-width") == null) entries.Add("data-tchmi-width", "200");
                if (attrs.Get("data-tchmi-height") == null) entries.Add("data-tchmi-height", "40");

                var insertionText = string.Empty;
                foreach (var itt in entries)
                    insertionText += $"{itt.Key}=\"{itt.Value}\" ";
                insertionText = " " + insertionText.Trim();

                var icon = GetIconByType(it);
                var listItem = new HtmlCompletion(displayText, insertionText, it.Description.Trim(), icon, null, ctx.Session)
                {
                    SortingPriority = 500
                };

                ctrlList.Add(listItem);
            }

            return new List<HtmlCompletion>(ctrlList);
        }
        
        private ITcHmiProjectNodeBase GetHmiProject(HtmlCompletionContext ctx)
        {
            if (ctx.Element == null) return null;
            var name = ctx.Element.Name;
            if (string.IsNullOrEmpty(name)) return null;
            if (!name.Equals(CompletionName, StringComparison.OrdinalIgnoreCase)) return null;
            var fileName = ctx.Document.TextBuffer.GetFileName();
            var prj = Utilities.GetProjectContainingFile(ServiceProvider, fileName);
            return prj?.Object as ITcHmiProjectNodeBase;
        }

        private ImageSource GetIconByType(ITcHmiControl ctrl)
        {
            if (ctrl == null) return _glyph;
            if (_icons.ContainsKey(ctrl)) return _icons[ctrl];
            try
            {
                var p = ctrl.OriginatingFile;
                var pp = System.IO.Path.GetDirectoryName(p);
                var ppp = System.IO.Path.Combine(pp, ctrl.Icons[0].Name);

                var uri = new Uri(ppp);
                var bmpInstance = BitmapFrame.Create(uri);
                _icons.Add(ctrl, bmpInstance);
                return bmpInstance;
            }
            catch
            {
                // ignore
            }
            return _glyph;
        }
    }
}
