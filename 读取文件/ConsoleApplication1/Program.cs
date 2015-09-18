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
        private int contentSize;            //内容的长度
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

        //计算内容的长度
        public void GetSize()
        {
            contentSize = content.Length;

        }


        //查找内容
        public void Search()
        {
            GetSize();
            if (contentSize == 0)
            {
                result += "需查找内容为空";
                return;
            }

            int maxSize = 1024;         //需读取的最大字节数
            byte[] byteData = new byte[maxSize];
            string fileStr = "";
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                fs.Seek(0, SeekOrigin.Begin);
                fs.Read(byteData, 0, byteData.Length);

                int length = byteData.Length;
                for (int i = 0; i < length; i++)
                {
                    fileStr += byteData[i].ToString("X2");
                }

                fs.Dispose();           //打开文件流，将文件内容读入fileStr字符串，然后释放


                switch (contentSize)
                {
                    case 1: oneByteResult += ByteMatch(fileStr, 1);
                        twoByteResult += ByteMatch(fileStr, 2);
                        fourByteResult += ByteMatch(fileStr, 4); break;
                    case 2: twoByteResult += ByteMatch(fileStr, 2);
                        fourByteResult += ByteMatch(fileStr, 4); break;
                    case 3: twoByteResult += ByteMatch(fileStr, 2);
                        fourByteResult += ByteMatch(fileStr, 4); break;
                    case 4: fourByteResult += ByteMatch(fileStr, 4); break;
                    default: break;
                }


            }
            catch (IOException e)
            {
                result += e.ToString();              //异常处理，将异常写入结果中               
                return;
            }


        }

        /// <summary>
        /// 按字节数对字符串中的内容进行匹配
        /// </summary>
        /// <param name="fileStr">需匹配的字符串</param>
        /// <param name="byteNum">每次需匹配的字节数</param>
        /// <returns>返回匹配到的所有位置</returns>
        public string ByteMatch(string fileStr, int byteNum)
        {
            string byteStr = "";
            int len = fileStr.Length;
            for (int i = 0; i < len - byteNum; i++)
            {
                int index = fileStr.Substring(i, byteNum).IndexOf(content);
                if (index != -1)
                {
                    byteStr += i + index + " | ";
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
                result += e;
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
