//Поняття серіалізації (продовження теми)
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace _04122020
{
    [DataContract]
    [Serializable]
    public class Car : ISerializable
    {
        [DataMember]
        public string Model { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            Date = Date.ToUniversalTime();
        }
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Date = Date.ToLocalTime();
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("CarModel", Model);
            info.AddValue("CarDate", $"{Date:yyyy-MM-dd}");
        }
        private Car(SerializationInfo info, StreamingContext context)
        {
            Model = info.GetString("CarModel");
            Date = DateTime.Parse(info.GetString("CarDate"));
        }
        public Car()
        {
            Model = "CarModel";
            Date = DateTime.Today;
        }
        public override string ToString()
        {
            return $"Car: {Model,8} | {Date:dd.MM.yyyy}";
        }
    }
    [DataContract]
    [Serializable]
    public class Person : ISerializable
    {
        [DataMember]
        private int Id;
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public DateTime Birthday { get; set; }
        [DataMember]
        public double Salary { get; set; }
        [DataMember]
        public Car car { get; set; }
        public Person(int id)
        {
            Id = id;
        }
        public Person()
        {
            Id = 1111;
        }
        [NonSerialized]
        public const string Planet = "Earth";
        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            Name = Name.ToUpper();
            //  Birthday = Birthday.ToUniversalTime();        
        }
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Name = Name[0] + Name.Substring(1).ToLower();
            // Birthday = Birthday.ToLocalTime();
        }


        public override string ToString()
        {
            return $"| {Id,5} |{Name,15}|{Salary,10:n2}| {Birthday:dd.MM.yyyy} |{Planet,8}| {car}|";
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("PersonId", Id);
            info.AddValue("PersonName", Name);
            info.AddValue("PersonSalary", Salary);
            info.AddValue("PersonBirthday", $"{Birthday:yyyy-MM-dd}");
            info.AddValue("PersonCar", car);
        }
        private Person(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetInt32("PersonId");
            Name = info.GetString("PersonName");
            Salary = info.GetDouble("PersonSalary");
            Birthday = DateTime.Parse(info.GetString("PersonBirthday"));
            car = info.GetValue("PersonCar", typeof(Car)) as Car;
        }
    }
    class BinaryTest
    {
        static public void Save(string fname, Person person)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (Stream stream = File.Create(fname))
                {
                    formatter.Serialize(stream, person);
                }
                Console.WriteLine("BinaryFormatter: Save");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
        }
        static public void Load(string fname)
        {
            try
            {
                Person person = null;
                BinaryFormatter formatter = new BinaryFormatter();
                using (Stream stream = File.OpenRead(fname))
                {
                    person = formatter.Deserialize(stream) as Person;
                }
                Console.WriteLine("BinaryFormatter: Load");
                Console.WriteLine($"Person: {person}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
        }

        static public void Run()
        {
            Person person = new Person(12345)
            {
                Name = "Ivan",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2021, 2, 25) }
            };
            string fname = "Ivan.bin";
            Save(fname, person);
            Load(fname);
        }
    }
    class SoapTest
    {
        static public void Save(string fname, Person person)
        {
            try
            {
                SoapFormatter formatter = new SoapFormatter();
                using (Stream stream = File.Create(fname))
                {
                    formatter.Serialize(stream, person);
                }
                Console.WriteLine("SoapFormatter: Save");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
        }
        static public void Load(string fname)
        {
            try
            {
                Person person = null;
                SoapFormatter formatter = new SoapFormatter();
                using (Stream stream = File.OpenRead(fname))
                {
                    person = formatter.Deserialize(stream) as Person;
                }
                Console.WriteLine("SoapFormatter: Load");
                Console.WriteLine($"Person: {person}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
        }

        static public void Run()
        {
            Person person = new Person(12345)
            {
                Name = "Ivan",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596,
                car = new Car { Model = "VW", Date = new DateTime(2020, 10, 21) }
            };
            string fname = "IvanSOAP.xml";
            Save(fname, person);
            Load(fname);
        }
    }
    class XMLTest
    {
        static public void Save<T>(string fname, T obj)
        {
            try
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T));
                using (Stream stream = File.Create(fname))
                {
                    formatter.Serialize(stream, obj);
                }
                Console.WriteLine("XmlSerializer: Save");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
        }
        static public T Load<T>(string fname) //where T: class
        {
            T obj = default;
            try
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T));
                using (Stream stream = File.OpenRead(fname))
                {
                    obj = (T)formatter.Deserialize(stream);
                }
                Console.WriteLine("XmlSerializer: Load");
                // Console.WriteLine($"{obj}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
            return obj;
        }
        static public void ListPerson()
        {
            List<Person> gr = new List<Person> {
            new Person(10)
            {
                Name = "Ivan",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            },
             new Person(20)
            {
                Name = "Petro",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            },
              new Person(30)
            {
                Name = "Stepan",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            },
               new Person(40)
            {
                Name = "Inna",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            },
                new Person(50)
            {
                Name = "Anna",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            },
                 new Person(60)
            {
                Name = "Maks",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            },
            };
            string fname = "persons.xml";
            Save<List<Person>>(fname, gr);
            List<Person> npersons = Load<List<Person>>(fname);
            npersons.ForEach(x => Console.WriteLine(x));
        }

        static public void Run()
        {
            Person person = new Person(12345)
            {
                Name = "IvanXML",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            };
            Console.WriteLine(person);
            string fname = "IvanXML.xml";
            Save<Person>(fname, person);
            Person pers = Load<Person>(fname);
            Console.WriteLine(pers);
        }
    }
    class JsonTest
    {
        static public void Save<T>(string fname, T obj)
        {
            try
            {
                DataContractJsonSerializerSettings Settings =
           new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };
                using (Stream stream = File.Create(fname))
                {
                    using (var writer = JsonReaderWriterFactory.CreateJsonWriter(
                        stream, Encoding.UTF8, true, true, "  "))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(T), Settings);
                        serializer.WriteObject(writer, obj);
                        writer.Flush();
                    }

                }
                Console.WriteLine("JsonSerializer: Save");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
        }
        static public T Load<T>(string fname) //where T: class
        {
            T obj = default;
            try
            {
                DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(T));
                using (Stream stream = File.OpenRead(fname))
                {
                    obj = (T)formatter.ReadObject(stream);
                }
                Console.WriteLine("JsonSerializer: Load");
                // Console.WriteLine($"{obj}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
            return obj;
        }
        static public void ListPerson()
        {
            List<Person> gr = new List<Person> {
            new Person(10)
            {
                Name = "Ivan",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            },
             new Person(20)
            {
                Name = "Petro",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            },
              new Person(30)
            {
                Name = "Stepan",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            },
               new Person(40)
            {
                Name = "Inna",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            },
                new Person(50)
            {
                Name = "Anna",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            },
                 new Person(60)
            {
                Name = "Maks",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            },
            };
            string fname = "persons.json";
            Save<List<Person>>(fname, gr);
            List<Person> npersons = Load<List<Person>>(fname);
            npersons.ForEach(x => Console.WriteLine(x));
        }

        static public void Run()
        {
            Person person = new Person(12345)
            {
                Name = "IvanJSON",
                Birthday = new DateTime(1999, 10, 10),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            };
            Console.WriteLine(person);
            string fname = "Ivan.json";
            Save<Person>(fname, person);
            Person pers = Load<Person>(fname);
            Console.WriteLine(pers);
        }
        static public void ToJson()
        {
            Person person = new Person(12345)
            {
                Name = "IvanJSON",
                Birthday = new DateTime(1999, 10, 10),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            };
            Console.WriteLine(person);
            string json = JsonConvert.SerializeObject(person);
            File.WriteAllText("json.json", json);

            List<Person> gr = new List<Person> {
            new Person(10)
            {
                Name = "Ivan",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            },
             new Person(20)
            {
                Name = "Petro",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            },
              new Person(30)
            {
                Name = "Stepan",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            },
               new Person(40)
            {
                Name = "Inna",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            },
                new Person(50)
            {
                Name = "Anna",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            },
                 new Person(60)
            {
                Name = "Maks",
                Birthday = new DateTime(1999, 10, 20),
                Salary = 1596.36,
                car = new Car { Model = "VW", Date = new DateTime(2020, 6, 25) }
            },
            };
            string jsons = JsonConvert.SerializeObject(gr);
            Console.WriteLine(jsons);
            File.WriteAllText("jsons.json", jsons);
        }
    }

    class Program
    {
        static void Test()
        {
            Person person = new Person(12345) { Name = "Ivan", Birthday = new DateTime(1999, 10, 20), Salary = 1596.36 };
            Console.WriteLine(person);
        }
        static void Main(string[] args)
        {
            //Test();
            Console.WriteLine("Hello World!");
            //  BinaryTest.Run();
            //  SoapTest.Run();
            //  XMLTest.Run();
            //   XMLTest.ListPerson();
            //   JsonTest.Run();
            //    JsonTest.ListPerson();
            JsonTest.ToJson();
        }
    }
}