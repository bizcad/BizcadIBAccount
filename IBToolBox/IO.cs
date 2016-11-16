using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace IBToolBox
{
    public static class IO
    {
        public const string Basepath = @"H:\IBData\";
        public static void WriteMessageList(List<string> messageList)
        {
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(messageList);
            using (StreamWriter sw = new StreamWriter(Basepath + "MessageList.csv"))
            {
                sw.WriteLine("reqId,Time,Open,High,Low,Close,Volume,WAP,Count");
                foreach (var s in messageList)
                    if (s.EndsWith("\n"))
                    {
                        sw.Write(s);
                    }
                    else
                    {
                        sw.WriteLine(s);
                    }
                sw.Flush();
                sw.Close();
            }
        }

        public static List<string> ReadMessageList()
        {
            List<string> messageList = new List<string>();
            using (StreamReader sr = new StreamReader(Basepath + "MessageList.csv"))
            {
                while (!sr.EndOfStream)
                {
                    messageList.Add(sr.ReadLine());
                }
                sr.Close();
            }
            return messageList;
        }
        public static List<string> ReadTextList(string whereSerialized)
        {
            
                
            List<string> stringList = new List<string>();
            using (StreamReader sr = new StreamReader(Basepath + whereSerialized))
            {
                while (!sr.EndOfStream)
                {
                    stringList.Add(sr.ReadLine());
                }
                sr.Close();
            }
            return stringList;
        }
        public static void SerializeJson<T>(T toSerialize, string whereSerialize, bool append = false)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(toSerialize, Formatting.Indented);
            using (var sw = new StreamWriter(Basepath + whereSerialize, append))
            {
                sw.Write(json);
                sw.Flush();
                sw.Close();
            }
        }

        public static void WriteStringList(List<string> list, string filename, bool append = false)
        {
            using (var sw = new StreamWriter(Basepath + filename, append))
            {
                foreach (var s in list)
                    sw.WriteLine(s);
                sw.Flush();
                sw.Close();
            }
        }
        public static void SerializeStringDictionary(Dictionary<string,string> dic, string filename, bool append = false)
        {
            using (var sw = new StreamWriter(Basepath + filename, append))
            {
                foreach (var s in dic)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(s.Key);
                    sb.Append(":");
                    sb.Append(s.Value);
                    sw.WriteLine(sb.ToString());
                }
                    
                sw.Flush();
                sw.Close();
            }
        }

        public static string ReadFileIntoString(string whereSerialized)
        {
            string result;
            using (StreamReader sr = new StreamReader(Basepath + whereSerialized))
            {
                result = sr.ReadToEnd();
                sr.Close();
            }
            return result;
        }

    }
}
