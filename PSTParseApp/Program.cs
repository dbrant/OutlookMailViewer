using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using PSTParse;
using PSTParse.Message_Layer;

namespace PSTParseApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            var pstPath = "/Users/dbrant/Downloads/outlook1.pst";
            var logPath = "log.txt";
            using (var file = new PSTFile(pstPath))
            {
                Console.WriteLine("Magic value: " + file.Header.DWMagic);
                Console.WriteLine("Is Ansi? " + !file.Header.isUnicode);


                var stack = new Stack<MailFolder>();
                stack.Push(file.TopOfPST);
                var totalCount = 0;
                if (File.Exists(logPath))
                    File.Delete(logPath);
                using (var writer = new StreamWriter(logPath))
                {
                    while (stack.Count > 0)
                    {
                        var curFolder = stack.Pop();

                        foreach (var child in curFolder.SubFolders)
                            stack.Push(child);
                        var count = curFolder.ContentsTC.RowIndexBTH.Properties.Count;
                        totalCount += count;
                        Console.WriteLine(String.Join(" -> ", curFolder.Path) + " ({0} messages)", count);

                        foreach (var message in curFolder.Messages)
                        {
                            Console.WriteLine(message.Subject);
                            Console.WriteLine(message.Imporance);
                            Console.WriteLine("Sender Name: " + message.SenderName);
                            if (message.From.Count > 0)
                                Console.WriteLine("From: {0}", String.Join("; ", message.From.Select(r => r.EmailAddress)));
                            if (message.To.Count > 0)
                                Console.WriteLine("To: {0}", String.Join("; ", message.To.Select(r => r.EmailAddress)));
                            if (message.CC.Count > 0)
                                Console.WriteLine("CC: {0}", String.Join("; ", message.CC.Select(r => r.EmailAddress)));
                            if (message.BCC.Count > 0)
                                Console.WriteLine("BCC: {0}", String.Join("; ", message.BCC.Select(r => r.EmailAddress)));

                            Console.WriteLine("Body: " + message.BodyPlainText);

                            writer.WriteLine(ByteArrayToString(BitConverter.GetBytes(message.NID)));
                        }
                    }
                }
                sw.Stop();
                Console.WriteLine("{0} messages total", totalCount);
                Console.WriteLine("Parsed {0} in {1} ms", Path.GetFileName(pstPath), sw.ElapsedMilliseconds);
            }
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
