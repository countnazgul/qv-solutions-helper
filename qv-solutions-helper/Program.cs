using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using QlikOCXLib;
//using QlikView;
//using QVPLib;

using System.IO;

namespace qv_solutions_helper
{
    class Program
    {
        static void Main(string[] args)
        {

            //string documentPath = @"C:\Users\adm-s7729841\Desktop\test.qvw";

            //QlikOCXLib.QlikOCX o = new QlikOCXLib.QlikOCX();
            //QlikView.Doc doc = o.OpenDocument(documentPath);
            //string script = doc.GetProperties().Script;

            //Console.WriteLine(System.IO.Directory.GetCurrentDirectory().ToString());
            //Console.Write(newPath);
            //Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White;
            MainMenu("", true);

        }


        static public void MainMenu(string additionalMessage, bool clear)
        {
            Console.Clear();
            string welcome = Environment.NewLine + "(press '0' to exit in any menu)" + Environment.NewLine + "" + Environment.NewLine;


            if (additionalMessage != "")
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(additionalMessage);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
            }

            var options = PrintOptions(welcome + "* Main Menu *", new string[] { "1. Add new ...",
                                                                                 "2. Build ...",
                                                                                 "3. Remove .." }, clear);

            switch (options)
            {
                case 1:
                    options = PrintOptions("* Create *", new string[] { "1. Project to the solution",
                                                                        "2. New step to project" }, clear);
                    switch (options)
                    {
                        case 1: // new project
                            Console.Write("What is the project name? ");
                            string prjName = Console.ReadLine();

                            string path = System.IO.Directory.GetCurrentDirectory().ToString();
                            string newPath = Path.GetFullPath(Path.Combine(path, @".\src"));

                            if (System.IO.Directory.Exists(newPath + "\\scripts\\" + prjName))
                            {
                                Console.Write("Project already exists. Overwrite? (y/n): ");
                                string response = Console.ReadLine().ToLower();

                                if (response == "y")
                                {
                                    CreatePrjFolders(newPath, prjName);
                                    MainMenu("Project folders are created", false);
                                }
                                else
                                {
                                    MainMenu("", true);
                                }
                            }
                            else
                            {
                                CreatePrjFolders(newPath, prjName);
                                //MainMenu("Project folders are created", false);
                            }


                            break;
                        case 2: // new step
                            string solutionPath = System.IO.Directory.GetCurrentDirectory().ToString();
                            solutionPath = Path.GetFullPath(Path.Combine(solutionPath, @".\src\scripts"));
                            string[] availableProjects = Directory.GetDirectories(solutionPath);

                            Console.Clear();
                            Console.WriteLine("* Create new step *");
                            Console.WriteLine();

                            //if( availableProjects.Length > 0) { 

                            for(var i = 0; i < availableProjects.Length; i++)
                            {
                                Console.WriteLine( (i + 1) + ". " + new DirectoryInfo(availableProjects[i]).Name);
                            }

                            Console.WriteLine();
                            Console.Write("In which project? ");
                            string selectedProject = Console.ReadLine();
                            selectedProject = availableProjects[ Convert.ToInt32(selectedProject) - 1];

                            Console.Write("What is the step name? ");
                            string stepName = Console.ReadLine();

                            string[] availablerSteps = Directory.GetDirectories(selectedProject);
                            int stepId = -1;

                            if(availablerSteps.Length == 0)
                            {
                                stepId = 1;
                                string newStepId = stepId.ToString("D2");
                                Directory.CreateDirectory(selectedProject + "\\" + newStepId + "" + stepName);
                            } else
                            {
                                string lastStepFolder = new DirectoryInfo(availablerSteps[availablerSteps.Length - 1]).Name;
                                stepId = int.Parse(lastStepFolder.Substring(0, 2)) + 1;
                                string newStepId = stepId.ToString("D2");
                                Directory.CreateDirectory(selectedProject + "\\" + newStepId + "" + stepName);
                            }

                            MainMenu("New step was created", false);

                            break;
                    }
                    break;
                case 2:

                    break;
                case 3:
                    options = PrintOptions("* Remove *", new string[] { "1. Project",
                                                                       "2. Empty all QVW (open without data and save" }, true);
                    switch(options)
                    {
                        case 1:
                            
                            break;
                    }
                    break;
            }
        }

        static public void CreatePrjFolders(string newPath, string prjName)
        {
            string[] folders = new string[] { "data", "qvw", "scripts" };

            for (var i = 0; i < folders.Length; i++)
            {
                Directory.CreateDirectory(newPath + "\\" + folders[i] + "\\" + prjName);
            }
        }

        static public int PrintOptions(string header, string[] options, bool clear)
        {
            if (clear == true)
            {
                Console.Clear();
            }
            Console.WriteLine(header);
            Console.WriteLine("");

            for (var i = 0; i < options.Length; i++)
            {
                Console.WriteLine(options[i]);
            }

            int a = int.Parse(Console.ReadLine());


            if (a == 0)
            {
                Console.Clear();
                Environment.Exit(0);
            }

            return a;
        }
    }
}
