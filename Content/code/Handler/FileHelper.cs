using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace UI.Web
{
    public class FileHelper
    {
        public static void WriteFile(string path, string content)
        {
            WriteFile(path, content, false);
        }

        public static void WriteFile(string path, string content, bool append)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path, append, Encoding.UTF8))
                {
                    writer.WriteLine(content);
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}