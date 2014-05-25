using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiParadigmGrapher.ContentHandling
{
    internal class ContentProvider : IContentProvider
    {
        public Stream GetStream(string resource)
        {
            var uri = new Uri(resource, UriKind.Relative);
            var info = App.GetContentStream(uri);

            return info.Stream;
        }
    }
}
