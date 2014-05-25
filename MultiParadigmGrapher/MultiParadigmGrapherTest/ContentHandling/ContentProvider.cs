using MultiParadigmGrapher.ContentHandling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiParadigmGrapherTest.ContentHandling
{
    internal class ContentProvider : IContentProvider
    {
        public Stream GetStream(string resource)
        {
            return File.OpenRead(resource.Remove(0,1));
        }
    }
}
