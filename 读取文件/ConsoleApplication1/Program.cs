using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApplication1
{
    //文件类
    public class File
    {
        private string fileName;            //需查找的文件名
        private string content;             //需查找的内容       
        private string oneByteResult = "按一个字节匹配，内容所在位置为：  ";
        private string twoByteResult = "按两个字节匹配，内容所在位置为：  ";
        private string fourByteResult = "按四个字节匹配，内容所在位置为：  ";
        private string result = "本次查找结果为： ";                         //最终查找的结果

        //File的构造函数
        public File(string FileName, string Content)
        {
            fileName = FileName;
            content = Content;
        }

        //查找内容
        public void Search()
        {
            if (content.Length == 0)
            {
                result = string.Concat(result, "需查找内容为空");       //用string.Concat连接短字符串
                return;
            }

            int maxSize = 1024;                         //每次读取的最大字节数
            byte[] byteData = new byte[maxSize];        //存放读取内容的数组

            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                int num = 0;                        //每次读入的字节数
                int counter = 0;                    //总共读入的字节数
                int times = 0;                      //总共读取的次数
                int offset = 0;                     //传给ByteMatch的偏移量
                string fileStr = "";

                while (true)                        //循环读取，每次读取限定的最大字节数，读完后清空字符串
                {
                    fileStr = "";
                    fs.Seek(counter, SeekOrigin.Begin);
                    num = fs.Read(byteData, 0, maxSize);
                    if (num == 0)                               //如果读完，则退出循环
                    {
                        break;
                    }
                    else
                    {
                        counter += num;
                        times++;
                        if (times >= 2)
                        {
                            offset = counter;
                        }
                        for (int i = 0; i < maxSize; i++)
                        {
                            fileStr = string.Concat(fileStr, byteData[i].ToString("X2"));
                        }
                        switch (content.Length)
                        {
                            case 1: oneByteResult = string.Concat(oneByteResult, ByteMatch(fileStr, 1, offset));
                                twoByteResult = string.Concat(twoByteResult, ByteMatch(fileStr, 2, offset));
                                fourByteResult = string.Concat(fourByteResult, ByteMatch(fileStr, 4, offset)); break;
                            case 2: twoByteResult = string.Concat(twoByteResult, ByteMatch(fileStr, 2, offset));
                                fourByteResult = string.Concat(fourByteResult, ByteMatch(fileStr, 4, offset)); break;
                            case 3: fourByteResult = string.Concat(fourByteResult, ByteMatch(fileStr, 4, offset)); break;
                            case 4: fourByteResult = string.Concat(fourByteResult, ByteMatch(fileStr, 4, offset)); break;
                            default: break;
                        }
                    }

                }

                fs.Dispose();

            }
            catch (IOException e)
            {
                result = string.Concat(result, e.ToString());             //异常处理，将异常写入结果中               
                return;
            }

        }


        /// <summary>
        /// 按字节数对字符串中的内容进行匹配
        /// </summary>
        /// <param name="fileStr">需匹配的字符串</param>
        /// <param name="byteNum">每次需匹配的字节数</param>
        /// <returns>返回匹配到的所有位置</returns>
        public string ByteMatch(string fileStr, int byteNum, int offset)
        {
            string byteStr = "";
            int len = fileStr.Length;
            int index = 0;

            for (int i = 0; i < len - byteNum; i++)
            {
                index = fileStr.Substring(i, byteNum).IndexOf(content);
                if (index != -1)
                {
                    byteStr = string.Concat(byteStr, (i + index + offset).ToString(), " | ");
                }
            }

            return byteStr;
        }

        //将查找的内容写入文件
        public void WriteResult()
        {
            try
            {
                FileStream fs = new FileStream(@"D:\result.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);

                sw.WriteLine("It's now {0}", DateTime.Now.ToLongTimeString());
                sw.WriteLine(result);
                sw.WriteLine(oneByteResult);
                sw.WriteLine(twoByteResult);
                sw.WriteLine(fourByteResult);

                sw.Close();
                fs.Dispose();
            }

            catch (IOException e)           //异常处理
            {
                result = string.Concat(result, e.ToString());
                return;
            }

        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            //提示用户输入文件名和需查找的内容
            Console.WriteLine("请输入文件名：");
            string fileName = Console.ReadLine();
            Console.WriteLine("请输入您要查找的内容：");
            string content = Console.ReadLine();

            //开始查找
            File file = new File(fileName, content);
            file.Search();
            file.WriteResult();

            //查找完毕
            Console.WriteLine("查找完毕，请打开文件 D:\\result.txt 查看本次查找结果!!");
            Console.ReadKey();
        }
    }
}
