using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMesh.Service
{
    public class Runner
    {
        string _DatabasePath;
        Trackfile Main;
        public string DatabasePath
        {
            get { return _DatabasePath; }
            set { _DatabasePath = value; }
        }
        public void Start()
        {
            this.Main = new Trackfile();
            string FilePath = @"C:\Users\Helio\Desktop\Code Projects\smesh\trunk\docs\example.tkf";
            string FilePath2 = @"C:\Users\Helio\Desktop\Code Projects\smesh\trunk\docs\example2.tkf";
            this.Main.Read(FilePath);
            this.Main.WriteOnce(FilePath2);
            /*
            this.DatabasePath = System.Environment.SpecialFolder.CommonApplicationData + "\\NiftyEngineering\\DB\\";
            SimpleMesh.Service.Net.TCP scratch = new SimpleMesh.Service.Net.TCP();
            scratch.Listen();
            Console.ReadLine();
             */
        }
    }
}
