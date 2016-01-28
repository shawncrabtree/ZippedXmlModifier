using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace ZipModifier
{
    class Program
    {
        //http://forums.asp.net/t/1895311.aspx?system+io+compression+filesystem+dll+required+
        static void Main(string[] args)
        {
            string zipName = "C:/Users/crabtres/test/zip.zip";
            using (ZipArchive archive = ZipFile.Open(zipName, ZipArchiveMode.Update))
            {
                foreach (ZipArchiveEntry file in archive.Entries) {
                    Stream s = file.Open();
                    using (StreamReader reader = new StreamReader(s))
                    using (StreamWriter writer = new StreamWriter(s))
                    {
                        if (file.Name.EndsWith(".txt"))
                        {
                            List<string> lines = new List<string>();
                            while (!reader.EndOfStream)
                            {
                                lines.Add(reader.ReadLine());
                            }

                            lines.Insert(1, "THIS SHOULD GO IN THE MIDDLE");

                            writer.BaseStream.Seek(0, SeekOrigin.Begin);
                            foreach (string l in lines)
                            {
                                writer.WriteLine(l);
                            }
                        }

                        else if (file.Name.EndsWith(".xml"))
                        {
                            using (XmlReader xmlReader = XmlReader.Create(reader))
                            using (XmlWriter xmlWriter = XmlWriter.Create(writer))
                            {

                                XmlDocument doc = new XmlDocument();
                                doc.Load(xmlReader);

                                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                                nsmgr.AddNamespace("a", "http://www.imsglobal.org/xsd/apip/apipv1p0/imscp_v1p1");
                                XmlNodeList aNodes = doc.SelectNodes("//a:manifest/a:Clients/a:User/a:FirstName", nsmgr);

                                Console.WriteLine(aNodes.Count);

                                foreach (XmlNode aNode in aNodes)
                                {
                                    XmlAttribute idAttribute = aNode.Attributes["id"];
                                    Console.WriteLine(idAttribute.ToString());
                                    // check if that attribute even exists...
                                    if (idAttribute != null)
                                    {
                                        // if yes - read its current value
                                        string currentValue = idAttribute.Value;
                                       
                                        // here, you can now decide what to do - for demo purposes,
                                        // I just set the ID value to a fixed value if it was empty before
                                        if (string.IsNullOrEmpty(currentValue))
                                        {
                                            idAttribute.Value = "888";
                                        }

                                        else
                                        {
                                            idAttribute.Value += "123";
                                        }
                                    }
                                }

                                writer.BaseStream.Seek(0, SeekOrigin.Begin);
                                doc.Save(writer);

                                xmlWriter.Flush();
                                xmlWriter.Close();
                            }
                        }

                        else
                        {
                            Console.WriteLine("Unidentified file type");
                        }
                        
                    }
                }

                Console.Read();
            }
        }
    }
}


