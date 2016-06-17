using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFoundation.Common
{
   public class MultiThreadHelper
    {
        static void Main(string[] args)
        {
            string codeStr = "D050,B023,E059,B020,E067,A011,B024,I137,E066,B014,A006,C042,A002,D047,D046,C029";
            IList<string> resultList = codeStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int everyCount = 4;//4个一组
            int total = resultList.Count;//总数
            int countThread = (int)Math.Ceiling((double)total / everyCount);//线程个数
            IList<List<string>> listTotal = new List<List<string>>();
            for (int i = 0; i < countThread; i++)
            {
                List<string> list = new List<string>();
                int ct = i * everyCount;
                for (int j = ct; j < ct + everyCount; j++)
                {
                    if (j < resultList.Count)
                    {
                        string res = resultList[j];
                        list.Add(res);
                    }
                }
                listTotal.Add(list);
            }
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Console.WriteLine(Environment.ProcessorCount);




            //第一种多线程调用方式
            //ParallelOptions options = new ParallelOptions();
            ////指定使用的硬件线程数为1
            //options.MaxDegreeOfParallelism = 1;//Environment.ProcessorCount给出了逻辑内核的数目
            //Parallel.For(0, listTotal.Count, options,(i) =>
            //{
            //    Console.WriteLine("数组索引{0}对应的那个元素{1}", i, listTotal[i]);
            //    DealData(listTotal[i]);
            //});


            //第二种多线程调用方式
            //Parallel.ForEach(listTotal, (item) =>
            //{
            //    DealData(item);
            //});
            //第三种多线程调用方式
            //Thread[] array = new Thread[countThread];
            //for (int i = 0; i < array.Length; i++)
            //{
            //    ParameterizedThreadStart ParStart1 = new ParameterizedThreadStart(DealData);
            //    array[i] = new Thread(ParStart1);
            //    List<string> list = listTotal[i];
            //    array[i].Start(list);
            //}
            //for (int i = 0; i < array.Length; i++)
            //{
            //    array[i].Join();
            //}
            //第四种多线程调用方式
            //Task[] tks = new Task[countThread];
            //for (int i = 0; i < listTotal.Count; i++)
            //{
            //    tks[i] = new Task(DealData, listTotal[i]);
            //    tks[i].Start();
            //}
            //Task.WaitAll(tks);



            //for (int i = 0; i < listTotal.Count; i++)
            //{
            //    Parallel.Invoke(DealData);//无参函数
            //}

            watch.Stop();
            Console.WriteLine("\n耗费时间：" + watch.ElapsedMilliseconds);
            GC.Collect();

            Console.ReadLine();
        }
        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="result"></param>
        private static void DealData(object result)
        {
            foreach (string item in (IList<string>)result)
            {
                Console.WriteLine(item);
            }
        }
        private static void DealData()
        {
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(i);
            }
            Console.WriteLine("#########################################");
        }
    }
}
