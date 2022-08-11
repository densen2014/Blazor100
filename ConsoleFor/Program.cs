using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

    class Program
    {
        static void Main(string[] args)
        {
            var person = new Person
            {
                Id = 12345,
                Name = "Fred",
                Address = new List<List<Address>>(){
                        new List<Address>(){
                            new Address
                            {
                                Line1 = "Flat 1",
                                Line2 = "The Meadows"
                            }
                            ,new Address
                            {
                                Line1 = "Flat 2",
                                Line2 = "The Meadows"
                            }
                            ,new Address
                            {
                                Line1 = "Flat 3",
                                Line2 = "The Meadows"
                            }
                            ,new Address
                            {
                                Line1 = "Flat 4",
                                Line2 = "The Meadows"
                            }
                        },
                        new List<Address>(){
                            new Address
                            {
                                Line1 = "Flat 1",
                                Line2 = "The Meadows"
                            }
                            ,new Address
                            {
                                Line1 = "Flat 2",
                                Line2 = "The Meadows"
                            }
                            ,new Address
                            {
                                Line1 = "Flat 3",
                                Line2 = "The Meadows"
                            }
                            ,new Address
                            {
                                Line1 = "Flat 4",
                                Line2 = "The Meadows"
                            }
                        }
                }
            };
            var txt = SerializeToString_PB(person);
            Console.WriteLine(txt);
            Console.ReadLine();
        }

        /// <summary>
        /// 将对象实例序列化为字符串（Base64编码格式）——ProtoBuf
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象实例</param>
        /// <returns>字符串（Base64编码格式）</returns>
        public static string SerializeToString_PB<T>( T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, obj);
                return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
            }
        }

        [ProtoContract]
        class Person
        {
            [ProtoMember(1)]
            public int Id { get; set; }
            [ProtoMember(2)]
            public string Name { get; set; }

            [ProtoMember(3)]
            public List<List<Address>> Address { get; set; }
        }

        [ProtoContract]
        class Address
        {
            [ProtoMember(1)]
            public string Line1 { get; set; }
            [ProtoMember(2)]
            public string Line2 { get; set; }
        }




    }
}
