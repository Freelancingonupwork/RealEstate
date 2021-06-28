using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstajoMailService.App_Code.BAL
{
    public class utility
    {
        public static void log(string str)
        {
            try
            {
                //string filePath = Directory.GetParent(Path.GetDirectoryName((new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath)).ToString();
                string filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
                filePath = Path.Combine(filePath, "Logs");
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                filePath += "\\logs-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + ".txt";

                using (StreamWriter sw = System.IO.File.AppendText(filePath))
                {
                    sw.WriteLine(str);
                }
            }
            catch (Exception) { }
        }


        public static void log(Exception ex)
        {
            try
            {
                //string filePath = Directory.GetParent(Path.GetDirectoryName((new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath)).ToString();
                string filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
                filePath = Path.Combine(filePath, "Logs");
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                filePath += "\\logs-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + ".txt";

                using (StreamWriter sw = System.IO.File.AppendText(filePath))
                {
                    int linenum = 0;
                    Int32.TryParse(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')), out linenum);
                    sw.WriteLine("[" + DateTime.Now.ToString() + "] Exception[" + ex.Message + "]" + ex.StackTrace.Split(new string[] { "(" }, StringSplitOptions.RemoveEmptyEntries)[0] + "[Line No:" + linenum + "]");
                }
            }
            catch (Exception) { }
        }
    }
}
