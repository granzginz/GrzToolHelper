using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.IO;

namespace GrzToolHelper.BinocullarPath
{
    public class Binocullar
    {
        public Binocullar() { }

    // This will get the current WORKING directory (i.e. \bin\Debug)
    static string workingDirectory = Environment.CurrentDirectory;
    // or: Directory.GetCurrentDirectory() gives the same result

    // This will get the current PROJECT bin directory (ie ../bin/)
    string binProjectDirectory = Directory.GetParent(WorkingDirectory).Parent.FullName;

    // This will get the current PROJECT directory
    string rootProjectDirectory = Directory.GetParent(WorkingDirectory).Parent.Parent.FullName;

        public static string WorkingDirectory { get => workingDirectory; set => workingDirectory = value; }
        public string BinProjectDirectory { get => binProjectDirectory; set => binProjectDirectory = value; }
        public string RootProjectDirectory { get => rootProjectDirectory; set => rootProjectDirectory = value; }
    }
}
