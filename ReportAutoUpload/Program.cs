using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace ReportAutoUpload
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Please type the report address:");
                string file = Console.ReadLine();
                Console.WriteLine("Please type the id:");
                int id = int.Parse(Console.ReadLine());
                AutoUpload au = new AutoUpload();
                au.StartUpload(file, id);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
            
        }

        

        

    }
}
