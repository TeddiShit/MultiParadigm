using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiParadigmGrapher.ContentHandling
{
    public interface IContentProvider
    {
        Stream GetStream(string resource); 
    }
}
