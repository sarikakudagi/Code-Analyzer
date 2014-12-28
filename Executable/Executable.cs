
/*////////////////////////////////////////////////////////////////////////
// Executable.cs   -  Calls the Analyzer and send a file for Type Analysis//
// ver 2.2                                                             //
// Language:    C#, Visual Studio 10.0, .Net Framework 4.0             //
// Platform:    Dell Precision T7400 , Win 7, SP 1                     //
// Application: Pr#2 Help, CSE681, Fall 2011                           //
// Author:     sarika shrishail kudagi, 726869838, Syracuse University   //
//              (551) 998-2431, sskudagi@syr.edu   */            //
/////////////////////////////////////////////////////////////////////////
 //* Executable.cs - Main starting point of the program
 //* 
 //* Platform:    Surface Pro 3, Win 8.1 pro, Visual Studio 2013
 //* Application: CSE681 - SMA 
 //* Author:      Sarika Kudagi, 726869838
 //* Email: sskudagi@syr.edu
 //* Package Operations:
 //* - Executable contains the Main function
 //* - It will read the files from the command line.
 //* - Extract Path, Option and Patterns
 //* - getFiles() will call the fileManager package where all the files from the specifed path is extracted using the pattern.
 //* - The searched files are further sent to the analyzer, where size, complexity is found.
 //*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalysis
{
     public class Executable
    {

        public bool traverseFiles = false;
        public FileMgr fm;
        CodeAnalysis.Analyzer an;
        List<string> path = new List<string>();
        public Executable()
        {
            fm = new FileMgr();
            an = new CodeAnalysis.Analyzer();
        }
        static bool relationFlag = false;



        public string[] getFiles(string path, List<string> patterns) // to extract the files from the path matching the pattern
        {

            foreach (string pattern in patterns)
                fm.addPattern(pattern);

            //add all patterns input  from the command and send it to the file manager
            fm.findFiles(path);
            return fm.getFiles().ToArray();
        }
        void setRelationFlag(bool relation) // to set the flag when /r option is called
        {
            relationFlag = relation;
        }
        

        public void mainFunction(string args)
        {
            
            Repository rep = Repository.getInstance();
            Repository2 rep2 = Repository2.getInstance();
           
            List<Elem> table = rep.locations;
            List<Elem> table2 = rep2.locations;
            string[] file = new string[] { args };
           
            string[] input = new string[1000];
            
            
            List<string> pattern = new List<string>(); //list to store all the files
            pattern.Add("*.cs");
            
            an.doAnalysis(args);

           
        }
        
        static void Main(string[] args)
        {


        }
    }
}

