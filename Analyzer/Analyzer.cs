////////////////////////////////////////////////////////////////////////
// Analyzer.cs - calls parser and performs Type-Analysis               //
// ver 2.2                                                             //
// Language:    C#, Visual Studio 10.0, .Net Framework 4.0             //
// Platform:    Dell Precision T7400 , Win 7, SP 1                     //
// Application: Pr#2 Help, CSE681, Fall 2011                           //
// Author:     sarika shrishail kudagi, 726869838, Syracuse University   //
//              (551) 998-2431, sskudagi@syr.edu   */            //
/////////////////////////////////////////////////////////////////////////   
//----<Module Operation>
// doAnalysis: calls parser and performs Type-Analysis
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalysis
{
    public class Analyzer
    {
        public bool traverseFiles = false;
        bool relationFlag = false;

        public void setRelationFlag(bool relation)
        {
            relationFlag = relation;

        }

        public string[] getFiles(string path, List<string> patterns)
        {
            FileMgr fm = new FileMgr();

            foreach (string pattern in patterns)
                fm.addPattern(pattern);
            //add all patterns input  from the command and send it to the file manager
            fm.findFiles(path);
            return fm.getFiles().ToArray();
        }
        public void setTraverse(bool traverse) //set the flag to true when '/s' is supplied as option
        {

            traverseFiles = traverse;
        }

        public void doAnalysis(string files)
        {
            
            Parser parserRelationship = new Parser();
            Analyzer an = new Analyzer(); 
            {
                Console.Write("\n  Processing file {0}\n", files as string);

                CSsemi.CSemiExp semi = new CSsemi.CSemiExp();
                CSsemi.CSemiExp semi1 = new CSsemi.CSemiExp();


                semi.displayNewLines = false;
                semi1.displayNewLines = false;
                //if (!semi.open(file as string))
                if (!semi.open(files))
                {
                    Console.Write("\n  Can't open {0}\n\n", files);
                    return;
                }

                if (!semi1.open(files as string))
                {
                    Console.Write("\n  Can't open {0}\n\n", files);
                    return;
                }

                //Console.Write("\n  Type and Function Analysis");
                //Console.Write("\n ----------------------------\n");

                {


                    BuildCodeAnalyzer builder = new BuildCodeAnalyzer(semi);
                    Parser parser = builder.build();
                    Repository rep = Repository.getInstance();
                    Repository2 rep2 = Repository2.getInstance();
                    try
                    {
                        while (semi.getSemi())
                            parser.parse(semi);
                        // Console.Write("\n\n  locations table contains:\n\n");
                    }
                    catch (Exception ex)
                    {
                        Console.Write("\n\n  {0}\n", ex.Message);
                    }
                    semi.close();
                }
               
               
                {


                    BuildCodeRelation relation = new BuildCodeRelation(semi1);
                    parserRelationship = relation.build(); //second parse to find the relationship
                    Repository rep = Repository.getInstance();
                    Repository2 rep2 = Repository2.getInstance();

                    try
                    {

                        while (semi1.getSemi())
                            parserRelationship.parseRelation(semi1);
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("table is empty");
                        Console.Write("\n\n  {0}\n", ex.Message);
                    }

                    semi1.close();
                }
            }
        }

        static void Main(string[] args)
        {
            string path = "../../";
            Analyzer an1 = new Analyzer();
            List<string> patterns = new List<string>();
            patterns.Add("*.cs");

           // string[] files = an1.getFiles(path, patterns).ToArray();
            //an1.doAnalysis(files);

        }
    }
}
