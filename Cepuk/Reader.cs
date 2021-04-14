using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GrzToolHelper.Cepuk
{
    public class Reader
    {
        public Reader() { }

        //String path = @"D:\Example.txt";

        //public string Path { get => path; set => path = value; }

        public string readLine(string Path)
        {
            string s = "";
            string t = "";
            System.Threading.Thread.Sleep(5000);
            using (StreamReader sr = File.OpenText(Path))
            {
                while ((s = sr.ReadLine()) != null)
                {
                    t = t + s + "\n";
                }
            }
            //Console.ReadKey();
            return t;
        }

        public string readAllText(string path)
        {
            string t = "";
            using (StreamReader sr = File.OpenText(path))
            {
                t = File.ReadAllText(path);
            }
            return t;
        }
    }
}
