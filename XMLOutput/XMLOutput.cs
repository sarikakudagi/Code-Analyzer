///////////////////////////////////////////////////////////////////////
// XMLDisplay.cs - Displays types and relationships in Xml           //
// ver 1.0                                                           //
// Language:    C#, 2013, .Net Framework 5.0                         //
// Platform:    Lenovo g550, Win8.1, SP1                             //
// Application: Package used for solution to Project-2 CSE-681       //
// Author:      Sarika Shrishail Kudagi(SUID: 726869838)    //
//              Syracuse University                                  //
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * XMLPackage  provides support to build XML. The Code Analyzer results can be
modeled into a valid XML using this module. Apart from creating an XML, it also writes the
xml into the output file if required
 * Following functions are defined 
 * 1: displayxml() - Output's the Function Complexity and Size Result to an XML File.
 * 2: displayrelationxml() - Output's the Class Relationship Result to an XML File.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;
namespace CodeAnalysis
{
    public class XMLOutput
    {
        public void displayxml()
        {
            Repository rep = Repository.getInstance();
            List<Elem> output = rep.locations;
            //Console.Write("\n  Create XML file using XDocument");
            //Console.Write("\n =================================\n");
            XDocument xml = new XDocument();
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XComment comment = new XComment("Demonstration XML");
            xml.Add(comment);
            XElement root = new XElement("CODEANALYSIS");
            xml.Add(root);
            foreach (Elem e in output)
            {
                if (e.type != "")
                {
                    int size = e.end - e.begin;
                    XElement child = new XElement("Type");
                    root.Add(child);
                    XElement type = new XElement("Type", e.type);
                    child.Add(type);

                    XElement child2 = new XElement("NAME");
                    root.Add(child2);
                    XElement name = new XElement("Name", e.name);
                    child2.Add(name);
                    XElement child3 = new XElement("COMPLEXITY");
                    root.Add(child3);
                    XElement complexity = new XElement("Complexity", Convert.ToString(e.complexity));
                    child3.Add(complexity);
                    XElement child4 = new XElement("SIZE");
                    root.Add(child4);
                    XElement size1 = new XElement("Size", Convert.ToString(e.size));
                    child4.Add(size1);
                    xml.Save(Directory.GetCurrentDirectory() + ".xml");
                }
            }
        }


        public void displayrelationxml(string class1,string relation,string class2)
        {
           // Repository rep = Repository.getInstance();
           // List<Element> rp = rep.locationRp;
            //Console.Write("\n  Create XML file using XDocument");
            //Console.Write("\n =================================\n");
            XDocument xml = new XDocument();
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XComment comment = new XComment("Demonstration XML");
            xml.Add(comment);
            XElement root = new XElement("CODEANALYSIS");
            xml.Add(root);
            //foreach (Element e in rp)
            {
                XElement parent = new XElement("ParentClass");
                root.Add(parent);
                XElement type = new XElement("ParentClass1", class1);
                parent.Add(type);
            // root.Add(parent);
                XElement relationship = new XElement("Relationship", relation);
                parent.Add(relationship);

             //   XElement child = new XElement("ChildCLass");
              //  root.Add(relationship);
                XElement name = new XElement("ChildClass", class2);
                parent.Add(name);
                //root.Add(name);
                xml.Save(Directory.GetCurrentDirectory() + ".xml");
            }
        }


//#if(TEST_XML)
        static void Main(string[] args)
        {
                Console.Write("\n  XML DISPLAY");
                Console.Write("\n =================================\n");
                XMLOutput a = new XMLOutput();
               // string options="";   // set the value of options suppplied
            //if(options == "/R")
            //    a.displayxml1();
            //else
                a.displayxml(); 
                

        }
//#endif
    }
}