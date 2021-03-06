using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RealEstate.Utills
{
    public class ErrorLog
    {

        public static void log(string str)
        {
            try
            {
                //string filePath = Directory.GetParent(Path.GetDirectoryName((new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath)).ToString();
                //string filePath = Path.GetDirectoryName((new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath).ToString();
                string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                filePath = Path.Combine(filePath, "Logs");
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                filePath += "\\logs-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + ".txt";

                using (StreamWriter sw = System.IO.File.AppendText(filePath))
                {
                    sw.WriteLine("      ");
                    sw.WriteLine("===Start " + DateTime.Now.ToString() + " ===");
                    sw.WriteLine(str);
                    sw.WriteLine("===End===");
                    sw.WriteLine("      ");
                }
            }
            catch (Exception ex) { }
        }


        public static void log(Exception ex)
        {
            try
            {
                //string filePath = Directory.GetParent(Path.GetDirectoryName((new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath)).ToString();
                //string filePath = Path.GetDirectoryName((new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath).ToString();
                string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
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
                    sw.WriteLine("      ");
                    sw.WriteLine("===Start " + DateTime.Now.ToString() + " ===");
                    sw.WriteLine("[" + DateTime.Now.ToString() + "] Exception[" + ex.Message + "]" + ex.StackTrace.Split(new string[] { "(" }, StringSplitOptions.RemoveEmptyEntries)[0] + "[Line No:" + linenum + "]");
                    sw.WriteLine("===End===");
                    sw.WriteLine("      ");
                }
            }
            catch (Exception) { }
        }
        public static void log(string[] args)
        {
            try
            {
                //string filePath = Directory.GetParent(Path.GetDirectoryName((new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath)).ToString();
                //string filePath = Path.GetDirectoryName((new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath).ToString();
                string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                filePath = Path.Combine(filePath, "Logs");
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                filePath += "\\logs-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + ".txt";

                using (StreamWriter sw = System.IO.File.AppendText(filePath))
                {
                    sw.WriteLine("      ");
                    sw.WriteLine("===Start " + DateTime.Now.ToString() + " ===");
                    foreach (var arg in args)
                    {
                        sw.WriteLine(arg);
                    }
                    sw.WriteLine("===End===");
                    sw.WriteLine("      ");
                }
            }
            catch (Exception) { }
        }


        public static void logError(string str,string filePath)
        {
            try
            {
                //string filePath = Directory.GetParent(Path.GetDirectoryName((new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath)).ToString();
                //string filePath = Path.GetDirectoryName((new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath).ToString();
                //string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                filePath = Path.Combine(filePath, "Logs");
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                filePath += "\\logs-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + ".txt";

                using (StreamWriter sw = System.IO.File.AppendText(filePath))
                {
                    sw.WriteLine("      ");
                    sw.WriteLine("===Start " + DateTime.Now.ToString() + " ===");
                    sw.WriteLine(str);
                    sw.WriteLine("===End===");
                    sw.WriteLine("      ");
                }
            }
            catch (Exception) { }
        }
    }
}
