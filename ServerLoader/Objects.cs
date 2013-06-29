using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ServerLoader
{
    class SVNFile
    {
        public string fileName;
        public string command;
        public Process batFile;
        public bool fileFound;
    }
}