/*
 * FileMgr.cs - Extracts files from the directory and return it to Executable
 * 
 * Platform:    Surface Pro 3, Win 8.1 pro, Visual Studio 2013
 * Application: CSE681 - SMA Helper
 * Application: Pr#2 Help, CSE681, Fall 2011                           
// Author:      sarika shrishail kudagi, 726869838, Syracuse University
//              (551) 998-2431, sskudagi@syr.edu   
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CodeAnalysis
{
    public class FileMgr
    {
        private List<string> files = new List<string>();
        private List<string> patterns = new List<string>();
       bool recurse = false;
        public void setTraverse(bool traverse)
        {
            recurse = traverse;
           
        }

       public void findFiles(string path)
        {
            //string path = "";
            FileMgr fm = new FileMgr();
            if (patterns.Count == 0)
                addPattern("*.*");
            foreach(string pattern in patterns)
            {
                string[] newFiles = Directory.GetFiles(path, pattern);
                for (int i = 0; i < newFiles.Length; ++i)
                    newFiles[i] = Path.GetFullPath(newFiles[i]);
                files.AddRange(newFiles);
            }
            if (recurse)
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                    findFiles(dir);
            }
        }
        

        public void addPattern(string pattern)
        {
            patterns.Add(pattern);
        }
        public void addFiles(String file)
        {
            files.Add(file);

        }

        public List<string> getFiles()
        {
            return files;
        }

#if(TEST_FILEMGR)
        static void Main(string[] args)
        {
            Console.Write("\n  Testing FileMgr Class");
            Console.Write("\n =======================\n");

            FileMgr fm = new FileMgr();
            fm.addPattern("*.cs");
           // fm.findFiles("../../");
            List<string> files = fm.getFiles();
            foreach (string file in files)
                Console.Write("\n  {0}", file);
            Console.Write("\n\n");
            
        }
#endif
    }
}
