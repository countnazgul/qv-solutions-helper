using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Helpers;
using YamlDotNet.RepresentationModel;
using System.Diagnostics;
using System.Net;
//using YamlDotNet.Helpers; 

namespace qv_solutions_helper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            MainMenu("", true);
        }


        static public void MainMenu(string additionalMessage, bool clear)
        {

            //WebRequest request = WebRequest.Create("https://api.twitter.com/1/users/profile_image?screen_name=twitterapi&size=bigger");
            //WebResponse response = request.GetResponse();
            //Console.WriteLine(response.ResponseUri);

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            string solutionPath = System.IO.Directory.GetCurrentDirectory().ToString();
            solutionPath = Path.GetFullPath(Path.Combine(solutionPath, @".\src\scripts"));
            string[] availableProjects = Directory.GetDirectories(solutionPath);
            string selectedProject = "";

            Console.Clear();
            string welcome = "Wellcome to Qlik Solutions Helper (" + version + ")" + Environment.NewLine + "Enter the needed number or enter 0 to exit in any menu" + Environment.NewLine + "" + Environment.NewLine;

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
                                                                                 "3. Remove ..",
                                                                                 "4. Open step qvw",
                                                                                 "5. Test"}, clear);


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
                                    CreatePrjFolders(newPath, prjName, false);
                                    MainMenu("Project folders are created", false);
                                }
                                else
                                {
                                    MainMenu("", true);
                                }
                            }
                            else
                            {
                                CreatePrjFolders(newPath, prjName, false);
                                MainMenu("Project folders are created", false);
                            }


                            break;
                        case 2: // new step
                            //string solutionPath = System.IO.Directory.GetCurrentDirectory().ToString();
                            //solutionPath = Path.GetFullPath(Path.Combine(solutionPath, @".\src\scripts"));
                            //string[] availableProjects = Directory.GetDirectories(solutionPath);

                            Console.Clear();
                            Console.WriteLine("* Create new step *");
                            Console.WriteLine();

                            for (var i = 0; i < availableProjects.Length; i++)
                            {
                                Console.WriteLine((i + 1) + ". " + new DirectoryInfo(availableProjects[i]).Name);
                            }

                            Console.WriteLine();
                            Console.Write("In which project? ");
                            selectedProject = Console.ReadLine();
                            selectedProject = availableProjects[Convert.ToInt32(selectedProject) - 1];

                            Console.Write("What is the step name? ");
                            string stepName = Console.ReadLine();

                            string[] availablerSteps = Directory.GetDirectories(selectedProject);
                            int stepId = -1;
                            string newStepId = "0";
                            string lastStepFolder = "";

                            if (availablerSteps.Length == 0)
                            {
                                stepId = 1;
                                newStepId = stepId.ToString("D2");
                                CreatePrjFolders(selectedProject, newStepId + "" + stepName, true);
                            }
                            else
                            {
                                lastStepFolder = new DirectoryInfo(availablerSteps[availablerSteps.Length - 1]).Name;
                                stepId = int.Parse(lastStepFolder.Substring(0, 2)) + 1;
                                newStepId = stepId.ToString("D2");
                                CreatePrjFolders(selectedProject, newStepId + "" + stepName, true);
                            }

                            CreateQVW(selectedProject, newStepId + "" + stepName, lastStepFolder);

                            MainMenu("New step was created", false);

                            break;
                    }
                    break;
                case 2:

                    Console.Clear();
                    Console.WriteLine("* Remove project *");
                    Console.WriteLine();

                    for (var i = 0; i < availableProjects.Length; i++)
                    {
                        Console.WriteLine((i + 1) + ". " + new DirectoryInfo(availableProjects[i]).Name);
                    }

                    Console.WriteLine();
                    Console.Write("Which project? ");
                    selectedProject = Console.ReadLine();
                    selectedProject = availableProjects[Convert.ToInt32(selectedProject) - 1];
                    string[] projectFolders = Directory.GetDirectories(selectedProject);
                    string projectScript = "";
                    string projectName = Path.GetFileName(selectedProject);

                    for (int i = 0; i < projectFolders.Length; i++)
                    {                        
                        string[] files = System.IO.Directory.GetFiles(projectFolders[i], "*.qvs");
                        if (files.Length > 0)
                        {
                            for (int f = 0; f < files.Length; f++)
                            {
                                string fileName = Path.GetFileNameWithoutExtension(files[f]);
                                projectScript += "///$tab " + fileName;

                                using (StreamReader sr = File.OpenText(files[f]))                                
                                {
                                    string s = String.Empty;
                                    while ((s = sr.ReadLine()) != null)
                                    {
                                        if (s.ToLower().IndexOf("binary") == -1)
                                        {
                                            projectScript += Environment.NewLine + s + Environment.NewLine;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    File.Copy( projectFolders[projectFolders.Length - 1].Replace(@"\scripts\", @"\qvw\") + ".qvw", @".\build\" + projectName + ".qvw", true);


                    string tempScript = Guid.NewGuid().ToString();

                    File.WriteAllText(@".\temp\" + tempScript, projectScript);

                    //System.Diagnostics.Process.Start("cscript",  @".\bin\vbs\script_edit.vbs /tempfile:" + Path.GetFullPath(@".\temp\" + tempScript) + " /qvw:" + Path.GetFullPath(@".\build\" + projectName + ".qvw"));

                    Console.WriteLine("Building ...");
                    Console.WriteLine("(building time depends on the last step qvw open and save time)");

                    Process scriptProc = new Process();
                    scriptProc.StartInfo.FileName = @"cscript";
                    scriptProc.StartInfo.WorkingDirectory = @".\bin\vbs\"; //<---very important 
                    scriptProc.StartInfo.Arguments = @"script_edit.vbs /tempfile:" + Path.GetFullPath(@".\temp\" + tempScript) + " /qvw:" + Path.GetFullPath(@".\build\" + projectName + ".qvw");
                    scriptProc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; //prevent console window from popping up
                    scriptProc.Start();
                    scriptProc.WaitForExit(); // <-- Optional if you want program running until your script exit
                    scriptProc.Close();

                    File.Delete(@".\temp\" + tempScript);
                    Console.Clear();
                    Environment.Exit(0); 

                    break;
                case 3:
                    options = PrintOptions("* Remove *", new string[] { "1. Project",
                                                                        "2. Step from project",
                                                                        "3. Empty all QVW (open without data and save" }, true);
                    switch (options)
                    {
                        case 1:
                            //string solutionPath = System.IO.Directory.GetCurrentDirectory().ToString();
                            //solutionPath = Path.GetFullPath(Path.Combine(solutionPath, @".\src\scripts"));
                            //string[] availableProjects = Directory.GetDirectories(solutionPath);

                            Console.Clear();
                            Console.WriteLine("* Remove project *");
                            Console.WriteLine();

                            for (var i = 0; i < availableProjects.Length; i++)
                            { 
                                Console.WriteLine((i + 1) + ". " + new DirectoryInfo(availableProjects[i]).Name);
                            }

                            Console.WriteLine();
                            Console.Write("Which project? ");
                            selectedProject = Console.ReadLine();
                            selectedProject = availableProjects[Convert.ToInt32(selectedProject) - 1];

                            Console.Write("Are you shure? This will remove all qvw, data and script files! (y/n) ");
                            string deleteProjectResponse = Console.ReadLine();

                            if(deleteProjectResponse.ToLower() == "y")
                            {
                                RemoveProject(selectedProject);
                                MainMenu("Project deleted!", false);
                            } else
                            {
                                MainMenu("", true);
                            }

                            break;
                        case 2:
                            Console.Clear();
                            Console.WriteLine("* Remove step project *");
                            Console.WriteLine();
                            string[] projects = GetProjects();

                            for (var i = 0; i < projects.Length; i++)
                            {
                                Console.WriteLine((i + 1) + ". " + new DirectoryInfo(projects[i]).Name);
                            }
                            

                            Console.WriteLine();
                            Console.Write("Which project? ");
                            string project = Console.ReadLine();
                            selectedProject = projects[Convert.ToInt32(project) - 1];

                            Console.Clear();
                            Console.WriteLine("* Which step? *");
                            Console.WriteLine();

                            string[] availableSteps = Directory.GetDirectories(selectedProject);

                            for (var i = 0; i < availableSteps.Length; i++)
                            {
                                Console.WriteLine((i + 1) + ". " + new DirectoryInfo(availableSteps[i]).Name);
                            }

                            string step = Console.ReadLine();
                            string selectedStep = availableSteps[Convert.ToInt32(step) - 1];

                            RemoveStep(selectedStep);

                            break;
                        case 3:
                            MainMenu("Not implemented yet", false);
                            break;
                    }
                    break;
                case 4:
                    Console.Clear();
                    Console.WriteLine("* Open step qvw *");
                    Console.WriteLine();
                    string[] avProjects = GetProjects();

                    for (var i = 0; i < avProjects.Length; i++)
                    {
                        Console.WriteLine((i + 1) + ". " + new DirectoryInfo(avProjects[i]).Name);
                    }


                    Console.WriteLine();
                    Console.Write("Which project? ");
                    string projectqvw = Console.ReadLine();
                    string selectedProj = avProjects[Convert.ToInt32(projectqvw) - 1];

                    Console.Clear();
                    Console.WriteLine("* Which step? *");
                    Console.WriteLine();

                    string[] availableStepsqvw = Directory.GetDirectories(selectedProj);

                    for (var i = 0; i < availableStepsqvw.Length; i++)
                    {
                        Console.WriteLine((i + 1) + ". " + new DirectoryInfo(availableStepsqvw[i]).Name);
                    }

                    string stepqvw = Console.ReadLine();
                    string selectedStepqvw = availableStepsqvw[Convert.ToInt32(stepqvw) - 1];
                    selectedStepqvw = selectedStepqvw.Replace("\\scripts\\", "\\qvw\\") + ".qvw";

                    var qvPath = GetSetting("qvPath");

                    if(qvPath == null)
                    {
                        Console.Clear();
                        Console.WriteLine("'qvPath' parameter was not found in config.yaml!");
                        Environment.Exit(0);
                    }
                     else
                    {
                        System.Diagnostics.Process.Start("", selectedStepqvw);
                        MainMenu("", true);
                    }


                    break;
                case 5:



                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unknown value");
            }
        }

        static public void CreatePrjFolders(string newPath, string prjName, bool isStep)
        {
            string[] folders = new string[] { "data", "qvw", "scripts" };

            for (var i = 0; i < folders.Length; i++)
            {
                if (isStep == false)
                {
                    Directory.CreateDirectory(newPath + "\\" + folders[i] + "\\" + prjName);
                }
                else
                {
                    if (folders[i] != "qvw")
                    {
                        string path = newPath.Replace("scripts", folders[i]) + "\\" + prjName;
                        Directory.CreateDirectory(path);
                    }
                }
            }
        }

        static public void CreateQVW(string path, string stepName, string lastStepFolder)
        {
            string newPath = path.Replace("scripts", "qvw") + "\\" + stepName;
            string binaryLoad = "";
            if(lastStepFolder != "")
            {
                binaryLoad = "Binary [" + lastStepFolder + ".qvw];" + System.Environment.NewLine;
            }


            File.WriteAllBytes(newPath + ".qvw", Resource1._3Level);
            File.WriteAllText(path + "\\" + stepName + "\\" + "01Main.qvs", binaryLoad  + "//Hello QlikView!" + System.Environment.NewLine);

            string createPrjFolders = GetSetting("generatePrjFolders");

            if(createPrjFolders == "true")
            {
                Directory.CreateDirectory(newPath + "-prj");
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

            int a = -1;

            try
            {
                a = int.Parse(Console.ReadLine());

                if(a > options.Length + 1)
                {
                    MainMenu("Unknown value", true);
                } else if (a == 0)
                {
                    Console.Clear();
                    Environment.Exit(0);
                } else if(a < 0)
                {
                    MainMenu("Unknown value", true);
                }

            } catch(Exception)
            {
                MainMenu("Unknown value", true);
            }

            return a;
        }

        static public void RemoveProject(string prjFolder)
        {
            string[] folders = new string[] { "data", "qvw", "scripts" };

            for (var i = 0; i < folders.Length; i++)
            {
                string folder = prjFolder.Replace("\\scripts\\", "\\" + folders[i] + "\\");
                try
                {
                    Directory.Delete(folder, true);
                } catch(Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine(ex.ToString());
                    Environment.Exit(0);
                }
            }
        }

        static public void RemoveStep(string step)
        {
            Console.Write("Are you shure? This will remove the qvw, data and script files! (y/n) ");
            string deleteSteptResponse = Console.ReadLine();

            if (deleteSteptResponse.ToLower() == "y")
            {
                Directory.Delete(step, true);
                Directory.Delete(step.Replace("\\scripts\\", "\\data\\"), true);
                File.Delete(step.Replace("\\scripts\\", "\\qvw\\") + ".qvw");
                Directory.Delete(step.Replace("\\scripts\\", "\\qvw\\") + "-prj");

                MainMenu("Step deleted", false);
            }
            else
            {
                MainMenu("", true);
            }            
        }

        static public string[] GetProjects()
        {
            string solutionPath = System.IO.Directory.GetCurrentDirectory().ToString();
            solutionPath = Path.GetFullPath(Path.Combine(solutionPath, @".\src\scripts"));
            string[] availableProjects = Directory.GetDirectories(solutionPath);

            return availableProjects;
        }

        static public string GetSetting(string searchKey)
        {
            using (TextReader reader = File.OpenText(@".\_config\solution.config.yaml"))
            {
                var stream = new YamlStream();
                stream.Load(reader);

                var document = stream.Documents.First();

                var rootMapping = (YamlMappingNode)document.RootNode;
                string value = null;

                try
                {
                    value = rootMapping.Children[searchKey].ToString();
                } catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine(ex.ToString());
                    Environment.Exit(0);
                }

                return value;

            }
        }
    }
}
