///////////////////////////////////////////////////////////////////////
// RulesAndActions.cs - Parser rules specific to an application      //
// ver 2.1                                                           //
// Language:    C#, 2008, .Net Framework 4.0                         //
// Platform:    Dell Precision T7400, Win7, SP1                      //
// Application: Demonstration for CSE681, Project #2, Fall 2011      //
//Author:     sarika shrishail kudagi, 726869838, Syracuse University   //
//              (551) 998-2431, sskudagi@syr.edu   */               //
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * RulesAndActions package contains all of the Application specific
 * code required for most analysis tools.
 *
 * It defines the following Four rules which each have a
 * grammar construct detector and also a collection of IActions:
 *   - DetectNameSpace rule
 *   - DetectClass rule
 *   - DetectFunction rule
 *   - DetectScopeChange
 *   - DetectInheritance
 *   - DetectAggregation
 *   - DetectComposition
 *   
 *   Three actions - some are specific to a parent rule:
 *   - Print
 *   - PrintFunction
 *   - PrintScope
 * 
 * The package also defines a Repository class for passing data between
 * actions and uses the services of a ScopeStack, defined in a package
 * of that name.
 *
 * Note:
 * This package does not have a test stub since it cannot execute
 * without requests from Parser.
 *  
 */
/* Required Files:
 *   IRuleAndAction.cs, RulesAndActions.cs, Parser.cs, ScopeStack.cs,
 *   Semi.cs, Toker.cs
 *   
 * Build command:
 *   csc /D:TEST_PARSER Parser.cs IRuleAndAction.cs RulesAndActions.cs \
 *                      ScopeStack.cs Semi.cs Toker.cs
 *   
 * Maintenance History:
 * --------------------
 * ver 2.2 : 24 Sep 2011
 * - modified Semi package to extract compile directives (statements with #)
 *   as semiExpressions
 * - strengthened and simplified DetectFunction
 * - the previous changes fixed a bug, reported by Yu-Chi Jen, resulting in
 * - failure to properly handle a couple of special cases in DetectFunction
 * - fixed bug in PopStack, reported by Weimin Huang, that resulted in
 *   overloaded functions all being reported as ending on the same line
 * - fixed bug in isSpecialToken, in the DetectFunction class, found and
 *   solved by Zuowei Yuan, by adding "using" to the special tokens list.
 * - There is a remaining bug in Toker caused by using the @ just before
 *   quotes to allow using \ as characters so they are not interpreted as
 *   escape sequences.  You will have to avoid using this construct, e.g.,
 *   use "\\xyz" instead of @"\xyz".  Too many changes and subsequent testing
 *   are required to fix this immediately.
 * ver 2.1 : 13 Sep 2011
 * - made BuildCodeAnalyzer a public class
 * ver 2.0 : 05 Sep 2011
 * - removed old stack and added scope stack
 * - added Repository class that allows actions to save and 
 *   retrieve application specific data
 * - added rules and actions specific to Project #2, Fall 2010
 * ver 1.1 : 05 Sep 11
 * - added Repository and references to ScopeStack
 * - revised actions
 * - thought about added folding rules
 * ver 1.0 : 28 Aug 2011
 * - first release
 *
 * Planned Modifications (not needed for Project #2):
 * --------------------------------------------------
 * - add folding rules:
 *   - CSemiExp returns for(int i=0; i<len; ++i) { as three semi-expressions, e.g.:
 *       for(int i=0;
 *       i<len;
 *       ++i) {
 *     The first folding rule folds these three semi-expression into one,
 *     passed to parser. 
 *   - CToker returns operator[]( as four distinct tokens, e.g.: operator, [, ], (.
 *     The second folding rule coalesces the first three into one token so we get:
 *     operator[], ( 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodeAnalysis
{
    public class Element
    {
        public string class1{get; set;}
        public string relation { get; set; }

        public string class2 { get; set; }
        public string currClass { get; set; }
        public override string ToString()
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("{");
            temp.Append(String.Format("{0,-10}", class1)).Append(" : ");
            temp.Append(String.Format("{0,-10}", relation)).Append(" : ");
            temp.Append(String.Format("{0,-5}", class2)).Append(" : ");
            temp.Append(String.Format("{0,-5}", class2)).Append(" : ");
            return temp.ToString();
        }
    }
    public class El
    {
        public string currClass { get; set; }
    }
    public class Elem  // holds scope information
    {
        public string type { get; set; }
        public string name { get; set; }
        public int begin { get; set; }
        public int end { get; set; }
        public int size { get; set; }

        public int complexity { get; set; }

      //  public string currClass { get; set; }

        public override string ToString()
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("{");
            temp.Append(String.Format("{0,-10}", type)).Append(" : ");
            temp.Append(String.Format("{0,-10}", name)).Append(" : ");
            temp.Append(String.Format("{0,-5}", begin.ToString()));  // line of scope start
            temp.Append(String.Format("{0,-5}", end.ToString()));    // line of scope end
            temp.Append("}");
            return temp.ToString();
        }
    }

    public class Repository
    {
        ScopeStack<Elem> stack_ = new ScopeStack<Elem>();
        ScopeStack<Element> stackRel = new ScopeStack<Element>();
        List<Elem> locations_ = new List<Elem>();
         List<Element> locationR = new List<Element>();
        static Repository instance;

        public Repository()
        {
            instance = this;
        }

        public static Repository getInstance()
        {
            //locationR.Clear();
            return instance;
        }
        // provides all actions access to current semiExp

        public CSsemi.CSemiExp semi
        {
            get;
            set;
        }

        // semi gets line count from toker who counts lines
        // while reading from its source

        public int lineCount  // saved by newline rule's action
        {
            get { return semi.lineCount; }
        }
        public int prevLineCount  // not used in this demo
        {
            get;
            set;
        }
        // enables recursively tracking entry and exit from scopes

        public ScopeStack<Elem> stack  // pushed and popped by scope rule's action
        {
            get { return stack_; }
        }

        public ScopeStack<Element> stackRelation  // pushed and popped by scope rule's action
        {
            get { return stackRel; }
        }
        // the locations table is the result returned by parser's actions
        // in this demo

        public List<Elem> locations
        {
            get { return locations_; }
        }
        public List<Element> locationRp
        {
            get { return locationR; }
        }
    }
    //////////////////////////////////second Repository for server-2
    public class Repository2
    {
        ScopeStack<Elem> stack2_ = new ScopeStack<Elem>();
        ScopeStack<Element> stackRel2 = new ScopeStack<Element>();
        List<Elem> locations2_ = new List<Elem>();
         List<Element> locationR2 = new List<Element>();
        static Repository2 instance;

        public Repository2()
        {
            instance = this;
        }

        public static Repository2 getInstance()
        {
            //locationR2.Clear();
            return instance;
        }
        // provides all actions access to current semiExp

        public CSsemi.CSemiExp semi
        {
            get;
            set;
        }

        // semi gets line count from toker who counts lines
        // while reading from its source

        public int lineCount  // saved by newline rule's action
        {
            get { return semi.lineCount; }
        }
        public int prevLineCount  // not used in this demo
        {
            get;
            set;
        }
        // enables recursively tracking entry and exit from scopes

        public ScopeStack<Elem> stack  // pushed and popped by scope rule's action
        {
            get { return stack2_; }
        }

        public ScopeStack<Element> stackRelation  // pushed and popped by scope rule's action
        {
            get { return stackRel2; }
        }
        // the locations table is the result returned by parser's actions
        // in this demo

        public List<Elem> locations
        {
            get { return locations2_; }
        }
        public List<Element> locationRp
        {
            get { return locationR2; }
        }
    }
    /////////////////////////////////////////////////////////
    // pushes scope info on stack when entering new scope

    public class PushStack : AAction            //pushStack for repository-1
    {
        Repository repo_;

        public PushStack(Repository repo)
        {
            repo_ = repo;
        }
        public override void doActionA(CSsemi.CSemiExp semi){}
        public override void doAction(CSsemi.CSemiExp semi)
        {
            Elem elem = new Elem();
            int j = 0;
            elem.type = semi[0];  // expects type
            //DetectComplexity scope = new DetectComplexity();
            
               
                    if (semi[0].Equals("function"))
                    {
                        elem.complexity = 1;
                    }

                    if (semi[0].Equals( "scope"))
                    {

                        j = repo_.locations.Count;
                        repo_.locations[j - 1].complexity = repo_.locations[j - 1].complexity +1;
                        return ;
                    }
                 
                elem.name = semi[1];  // expects name
                elem.begin = repo_.semi.lineCount - 1;
                elem.end = 0;
                //elem.currClass = repo_.locations[k - 1].currClass;
                
                repo_.stack.push(elem);
                if (elem.type == "control" || elem.name == "anonymous")
                    return;
                repo_.locations.Add(elem);

                if (AAction.displaySemi)
                {
                    Console.Write("\n  line# {0,-5}", repo_.semi.lineCount - 1);
                    Console.Write("entering ");
                    string indent = new string(' ', 2 * repo_.stack.count);
                    Console.Write("{0}", indent);
                    this.display(semi); // defined in abstract action
                }
                if (AAction.displayStack)
                    repo_.stack.display();
                //to find the complxity of each function

            }
        }
        
    
    //////////////////////////////////////////////
    //Push stack relation
    public class PushRelation : AAction
    {
        Repository repo_;

        public PushRelation(Repository repo)
        {
            repo_ = repo;
            
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            Element elem = new Element();
            int j = 0;
            elem.class1 = semi[0];  // expects type
            
           
                elem.relation = semi[1];  // expects name
                elem.class2 = semi[2];
                
                if (semi[0].Equals("currClass"))
                {
                    int i=repo_.locationRp.Count;
                    elem.class1 = repo_.locationRp[i - 1].currClass;
                }
                
                repo_.stackRelation.push(elem);
                
                repo_.locationRp.Add(elem);
        
            }

        //////////////////////DoActuon for aggrgation
        public override void doActionA(CSsemi.CSemiExp semi)
        {
            Element elem = new Element();
           
            elem.currClass = semi[0];  // expects type

            repo_.stackRelation.push(elem);

            repo_.locationRp.Add(elem);

        }
    }
    /////////////////////////////////////////////////////////
    public class PushStack2 : AAction
    {
        Repository2 repo_;

        public PushStack2(Repository2 repo)
        {
            repo_ = repo;
        }
        public override void doActionA(CSsemi.CSemiExp semi) { }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            Elem elem = new Elem();
            int j = 0;
            elem.type = semi[0];  // expects type
            //DetectComplexity scope = new DetectComplexity();


            if (semi[0].Equals("function"))
            {
                elem.complexity = 1;
            }

            if (semi[0].Equals("scope"))
            {

                j = repo_.locations.Count;
                repo_.locations[j - 1].complexity = repo_.locations[j - 1].complexity + 1;
                return;
            }

            elem.name = semi[1];  // expects name
            elem.begin = repo_.semi.lineCount - 1;
            elem.end = 0;
            //elem.currClass = repo_.locations[k - 1].currClass;

            repo_.stack.push(elem);
            if (elem.type == "control" || elem.name == "anonymous")
                return;
            repo_.locations.Add(elem);

            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount - 1);
                Console.Write("entering ");
                string indent = new string(' ', 2 * repo_.stack.count);
                Console.Write("{0}", indent);
                this.display(semi); // defined in abstract action
            }
            if (AAction.displayStack)
                repo_.stack.display();
            //to find the complxity of each function

        }
    }
    ////////////////////////////////////////////
    public class PushRelation2 : AAction
    {
        Repository2 repo_;

        public PushRelation2(Repository2 repo)
        {
            repo_ = repo;

        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            Element elem = new Element();
            int j = 0;
            elem.class1 = semi[0];  // expects type


            elem.relation = semi[1];  // expects name
            elem.class2 = semi[2];

            if (semi[0].Equals("currClass"))
            {
                int i = repo_.locationRp.Count;
                elem.class1 = repo_.locationRp[i - 1].currClass;
            }

            repo_.stackRelation.push(elem);

            repo_.locationRp.Add(elem);

        }

        //////////////////////DoActuon for aggrgation
        public override void doActionA(CSsemi.CSemiExp semi)
        {
            Element elem = new Element();

            elem.currClass = semi[0];  // expects type

            repo_.stackRelation.push(elem);

            repo_.locationRp.Add(elem);

        }
    }
        
    
///////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////
    // pops scope info from stack when leaving scope

    public class PopStack2 : AAction
    {
        Repository2 repo_;

        public PopStack2(Repository2 repo)
        {
            repo_ = repo;
        }
        public override void doActionA(CSsemi.CSemiExp semi) { }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            Elem elem;
            try
            {
                elem = repo_.stack.pop();
                for (int i = 0; i < repo_.locations.Count; ++i)
                {
                    Elem temp = repo_.locations[i];
                    if (elem.type == temp.type)
                    {
                        if (elem.name == temp.name)
                        {
                            if ((repo_.locations[i]).end == 0)
                            {
                                (repo_.locations[i]).end = repo_.semi.lineCount;
                                break;
                            }
    
                        }
                        
                    }

                }
            }
            catch
            {
                Console.Write("popped empty stack on semiExp: ");
                semi.display();
                return;
            }
             
            CSsemi.CSemiExp local = new CSsemi.CSemiExp();
            local.Add(elem.type).Add(elem.name);
            if (local[0] == "control")
                return;

            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount);
                Console.Write("leaving  ");
                string indent = new string(' ', 2 * (repo_.stack.count + 1));
                Console.Write("{0}", indent);
                this.display(local); // defined in abstract action
            }
           
        }
    }
    //////////////////////////////////
    public class PopStack : AAction
    {
        Repository repo_;

        public PopStack(Repository repo)
        {
            repo_ = repo;
        }
        public override void doActionA(CSsemi.CSemiExp semi) { }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            Elem elem;
            try
            {
                elem = repo_.stack.pop();
                for (int i = 0; i < repo_.locations.Count; ++i)
                {
                    Elem temp = repo_.locations[i];
                    if (elem.type == temp.type)
                    {
                        if (elem.name == temp.name)
                        {
                            if ((repo_.locations[i]).end == 0)
                            {
                                (repo_.locations[i]).end = repo_.semi.lineCount;
                                break;
                            }

                        }

                    }

                }
            }
            catch
            {
                Console.Write("popped empty stack on semiExp: ");
                semi.display();
                return;
            }

            CSsemi.CSemiExp local = new CSsemi.CSemiExp();
            local.Add(elem.type).Add(elem.name);
            if (local[0] == "control")
                return;

            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount);
                Console.Write("leaving  ");
                string indent = new string(' ', 2 * (repo_.stack.count + 1));
                Console.Write("{0}", indent);
                this.display(local); // defined in abstract action
            }

        }
    }

    ///////////////////////////////////////////////////////////
    // action to print function signatures - not used in demo

    public class PrintFunction : AAction
    {
        Repository repo_;

        public PrintFunction(Repository repo)
        {
            repo_ = repo;
        }
        public override void display(CSsemi.CSemiExp semi)
        {
            Console.Write("\n    line# {0}", repo_.semi.lineCount - 1);
            Console.Write("\n    ");
            for (int i = 0; i < semi.count; ++i)
                if (semi[i] != "\n" && !semi.isComment(semi[i]))
                    Console.Write("{0} ", semi[i]);
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            this.display(semi);
        }
        public override void doActionA(CSsemi.CSemiExp semi)
        {
            this.display(semi);
        }
    }
    /////////////////////////////////////////////////////////
    // concrete printing action, useful for debugging

    public class Print : AAction
    {
        Repository repo_;

        public Print(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            Console.Write("\n  line# {0}", repo_.semi.lineCount - 1);
            this.display(semi);
        }
        public override void doActionA(CSsemi.CSemiExp semi)
        {
            Console.Write("\n  line# {0}", repo_.semi.lineCount - 1);
            this.display(semi);
        }
    }
    /////////////////////////////////////////////////////////
    // rule to detect namespace declarations

    public class DetectNamespace : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("namespace");
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }
    //////////////////////////
    ////
    ////////Detect current class for aggregation
    public class DetectCurrentClass : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {

                int index = semi.Contains("class");
                if (index != -1)
                {
                    CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                    // create local semiExp with tokens for type and name
                    local.displayNewLines = false;
                    local.Add(semi[index+1]);
                    doActionsA(local);
                    return false;
                }
            
            return false;

        }
    }
    /////////////////////
    ///////to detect the complexity

    public class DetectComplexity : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {

            string[] scopes = { "if", "do", "while", "for", "foreach", "switch", "else", "else if", "{" };
            foreach (string scope in scopes)
            {
                int index = semi.Contains(scope);
                if (index != -1)
                {
                    CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                    // create local semiExp with tokens for type and name
                    local.displayNewLines = false;
                    local.Add("scope");
                    doActions(local);
                    return false;
                }
            }
                return false;
            
        }
    }

    /////////////////////////////////////////////////////////
    // rule to dectect class definitions

    public class DetectClass : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int indexCL = semi.Contains("class");
            int indexIF = semi.Contains("interface");
            int indexST = semi.Contains("struct");
            int indexEN = semi.Contains("enum");

            int index = Math.Max(indexCL, indexIF);
            index = Math.Max(index, indexST);
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // rule to dectect function definitions

    public class DetectFunction : ARule
    {
        public static bool isSpecialToken(string token)
        {
            string[] SpecialToken = { "if", "for", "foreach", "while", "catch", "using" };
            foreach (string stoken in SpecialToken)
                if (stoken == token)
                    return true;
            return false;
        }
        public override bool test(CSsemi.CSemiExp semi)
        {
            if (semi[semi.count - 1] != "{")
                return false;

            int index = semi.FindFirst("(");
            if (index > 0 && !isSpecialToken(semi[index - 1]))
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                local.Add("function").Add(semi[index - 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // detect entering anonymous scope
    // - expects namespace, class, and function scopes
    //   already handled, so put this rule after those
    public class DetectAnonymousScope : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("{");
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add("control").Add("anonymous");
                doActions(local);
                return true;
            }
            return false;
        }
    }

    ////////////////////////////////////////
    ///////detect delegate
    public class DetectDelegate : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("delegate");
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add("delegate").Add(semi[index + 2]);
                doActions(local);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // detect leaving scope

    public class DetectLeavingScope : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("}");
            if (index != -1)
            {
                doActions(semi);
                return true;
            }
            return false;
        }
    }

    //Detect the Relation Inheritance
    public class DetectInheritance : ARule
    {

        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains(":");
            
            //Elem e = new Elem();
            Repository rep = Repository.getInstance();
            Repository rep2 = Repository.getInstance();
            if (index != -1)
            {

                foreach (Elem e in rep2.locations)
                {

                    // if (e.name == semi[index + 1])
                    {
                        CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                        //// create local semiExp with tokens for type and name
                        local.displayNewLines = false;
                        Console.WriteLine("{0},{1}", semi[index - 1], semi[index + 1]);
                        local.Add(semi[index - 1]).Add("Inherits").Add(semi[index + 1]);
                        doActions(local);
                        return true;
                    }
                     
                }

               
            }
            return false;
        }
    }
        //////Detect Aggregation
    public class DetectAggregation : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("new");
     
            Repository rep = Repository.getInstance();
            Repository2 rep2 = Repository2.getInstance();
            Elem e = new Elem();

            if (index != -1)
            {

                foreach (Elem en in rep2.locations)
                {

                    if (en.name == semi[index + 1])
                    {

                        CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                        // create local semiExp with tokens for type and name
                        local.displayNewLines = false;
                        local.Add("currClass").Add("Aggregates").Add(semi[index + 1]);
                        doActions(local);
                        return true;
                    }
                }


            }

            return false;
        }


    }
    
    /////////////Detect Composition
        public class DetectComposition : ARule
        {
            public override bool test(CSsemi.CSemiExp semi)
            {
                int indexST= semi.Contains("struct");
                int indexEN = semi.Contains("enum");
                int index = Math.Max(indexST, indexEN);
                if (index != -1)
                {
                    CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                    // create local semiExp with tokens for type and name
                    local.displayNewLines = false;
                    local.Add(semi[index]).Add("Composition").Add(semi[index + 1]);
                    doActions(local);
                    return true;
                }
                return false;
            }
        }
    
        

        public class BuildCodeAnalyzer
        {
           static Repository repo = new Repository();

           static Repository2 repo2 = new Repository2();

            public BuildCodeAnalyzer(CSsemi.CSemiExp semi)
            {
                repo.semi = semi;
                repo2.semi = semi;
               // repoRelation.semi= semi;
            }
            public virtual Parser build()
            {
                Repository repoRelation = Repository.getInstance();
                Repository2 repoRelation2 = Repository2.getInstance();
                Parser parser = new Parser();
                PushRelation pushr = new PushRelation(repoRelation);

                PushRelation2 pushr2 = new PushRelation2(repoRelation2);

                // decide what to show
                AAction.displaySemi = true;
                AAction.displayStack = false;  // this is default so redundant

                // action used for namespaces, classes, and functions
                PushStack push = new PushStack(repo);
                PushStack2 push2 = new PushStack2(repo2);
               // PushStack pushRelation = new PushStack(repoRelation);

                // capture namespace info
                DetectNamespace detectNS = new DetectNamespace();
                detectNS.add(push);
                detectNS.add(push2);
                parser.add(detectNS);

                // capture class info
                DetectClass detectCl = new DetectClass();
                detectCl.add(push);
                detectCl.add(push2);
                parser.add(detectCl);
                

                // capture function info
                DetectFunction detectFN = new DetectFunction();
                detectFN.add(push);
                detectFN.add(push2);
                parser.add(detectFN);


                DetectComplexity scope = new DetectComplexity();
                scope.add(push);
                scope.add(push2);
                parser.add(scope);
                

                // handle entering anonymous scopes, e.g., if, while, etc.
                DetectAnonymousScope anon = new DetectAnonymousScope();
                anon.add(push);
                anon.add(push2);
                parser.add(anon);
                
                // handle leaving scopes
               

               
                // parser configured
                return parser;
                
            }
        }
        
    

     public class BuildCodeRelation
      {
         Repository repoRelation = Repository.getInstance();
         Repository2 repoRelation2 = Repository2.getInstance();
          
              CSsemi.CSemiExp semi = new CSsemi.CSemiExp();
            
            public BuildCodeRelation(CSsemi.CSemiExp semi)
         {
             Console.WriteLine("");
             repoRelation.semi = semi;
         }
         public virtual Parser build()
            {
                Parser parser = new Parser();
           PushRelation push = new PushRelation(repoRelation);
           PushRelation2 push2 = new PushRelation2(repoRelation2);
             
           DetectCurrentClass currClassDet = new DetectCurrentClass();
          currClassDet.add(push);
          currClassDet.add(push2);
           parser.add(currClassDet);

               DetectInheritance inherit = new DetectInheritance();
              inherit.add(push);
              inherit.add(push2);
               parser.add(inherit);

               DetectAggregation aggregation = new DetectAggregation();
               aggregation.add(push);
               aggregation.add(push2);
               parser.add(aggregation);
          
               DetectComposition composition = new DetectComposition();
               composition.add(push);
               composition.add(push2);
               parser.add(composition);


              return parser;

            }
        }

    }



