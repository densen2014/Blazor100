using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

    class Program
    {
        static void Main(string[] args)
        {


            Stopwatch Watch1 = new Stopwatch();
            Watch1.Start();
            List<entityA> source = new List<entityA>();
            for (int i = 0; i < 10000000; i++)
            {
                source.Add(new entityA
                {
                    name = "悟空" + i,
                    sex = i % 2 == 0 ? "男" : "女",
                    age = i
                });
            }
            Watch1.Stop();

            Console.WriteLine("list循环插入耗时：" + Watch1.ElapsedMilliseconds);
            Stopwatch Watch2 = new Stopwatch();
            Watch2.Start();
            loop1(source);
            Watch2.Stop();
            Console.WriteLine("一般for循环耗时：" + Watch2.ElapsedMilliseconds);

            Stopwatch Watch3 = new Stopwatch();
            Watch3.Start();
            loop2(source);
            Watch3.Stop();
            Console.WriteLine("一般foreach循环耗时：" + Watch3.ElapsedMilliseconds);


            Stopwatch Watch4 = new Stopwatch();
            Watch4.Start();
            loop3(source);
            Watch4.Stop();
            Console.WriteLine("并行for循环耗时：" + Watch4.ElapsedMilliseconds);

            Stopwatch Watch5 = new Stopwatch();
            Watch5.Start();
            loop4(source);
            Watch5.Stop();
            Console.WriteLine("并行foreach循环耗时：" + Watch5.ElapsedMilliseconds);

            Stopwatch Watch6 = new Stopwatch();
            Watch6.Start();
            loop5(source);
            Watch6.Stop();
            Console.WriteLine("并行foreach循环2耗时：" + Watch6.ElapsedMilliseconds);
            Console.ReadLine();
        }

        //普通的for循环
        static void loop1(List<entityA> source)
        {
            int count = source.Count();
            for (int i = 0; i < count; i++)
            {
                source[0].age = +10;
                //System.Threading.Thread.Sleep(10);
            }
        }

        //普通的foreach循环
        static void loop2(List<entityA> source)
        {
            foreach (entityA item in source)
            {
                item.age = +10;
                //System.Threading.Thread.Sleep(10);
            }
        }

        //并行的for循环
        static void loop3(List<entityA> source)
        {
            int count = source.Count();
            Parallel.For(0, count, item =>
            {
                source[item].age = source[item].age + 10;
                //System.Threading.Thread.Sleep(10);
            });
        }

        //并行的foreach循环
        static void loop4(List<entityA> source)
        {
            Parallel.ForEach(source, item =>
            {
                item.age = item.age + 10;
                //System.Threading.Thread.Sleep(10);
            });
        }

        //并行的foreach循环2
        static void loop5(List<entityA> source)
        {
            source.ForEach(item =>
          {
              item.age = item.age + 10;
              //System.Threading.Thread.Sleep(10);
          });
        }



        //简单的实体
        class entityA
        {
            public string name { set; get; }
            public string sex { set; get; }
            public int age { set; get; }
        }



    }
}
