using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Sys = Cosmos.System;

namespace FritOS
{
    public class Kernel : Sys.Kernel
    {
        //System globals
        public const String SYSTEM_VERSION = "0.4.6";
        public Date SYSTEM_DATE;
        public List<File> FILESYS;
        public List<Variable> GLOBAL_VARS;

        //Run globals
        public Queue<ProcessBlock> RUNNING_BATCH_FILES;
        public bool batchNesting = false;

        //Command list
        public String[] COMMANDS = { "time", "date", "cls", "create", "dir", "out", "vars", "run", "rm", "clr", "help", "varCast", "set", "shared", "prfile", };

        protected override void BeforeRun()
        {
            //INITALIZE SYSTEM GLOBALS
            SYSTEM_DATE = new Date();
            FILESYS = new List<File>();
            GLOBAL_VARS = new List<Variable>();

            //INITIALIZE RUN GLOBALS
            RUNNING_BATCH_FILES = new Queue<ProcessBlock>();

            Console.WriteLine(" _|_|_|_|            _|    _|        _|_|      _|_|_|");
            Console.WriteLine(" _|        _|  _|_|      _|_|_|_|  _|    _|  _|");
            Console.WriteLine(" _|_|_|    _|_|      _|    _|      _|    _|    _|_|");
            Console.WriteLine(" _|        _|        _|    _|      _|    _|        _|");
            Console.WriteLine(" _|        _|        _|      _|_|    _|_|    _|_|_|");
            Console.WriteLine("FritOS: Freakin' Rad Input Terminal OS, version " + SYSTEM_VERSION);
            Console.WriteLine();
            Console.WriteLine("Type \"help\" for a listing of all currently implemented commands.");
            Console.WriteLine("If the terminal does not accept inputs, restart it. I don't know why it doesn't always work.");

            //DEBUG SECTION
            File f = new File("f.bat");
            f.writeLine("create a.bat");
            f.writeLine("var1 = \"test\"");
            f.writeLine("create b.bat");
            f.writeLine("var2 = 4");
            f.writeLine("save ");
            f.writeLine("save ");
            FILESYS.Add(f);
        }

        //Main loop of the OS, takes an input and executes it based on the command and arguments
        protected override void Run()
        {
            Console.Write("usr:/ > ");
            String input = Console.ReadLine().Trim();
            String[] command = splitCommand(input);
            execute(command[0], command[1]);
        }

        //A big ass switch basically for the function call
        public void execute(String func, String args)
        {
            //Based on the function, call the appropriate built in method with arguments
            if (func == "")
            {
                return;
            }
            else if (func == "help")
            {
                help(args);
            }
            else if (func == "time")
            {
                time(args);
            }
            else if (func == "date")
            {
                SYSTEM_DATE.update();
                date(args);
            }
            else if (func == "cls")
            {
                clearScreen();
            }
            else if (func == "create")
            {
                create(args);
            }
            else if (func == "dir")
            {
                dir(args);
            }
            else if (func == "out")
            {
                output(args);
            }
            else if (func == "vars")
            {
                vars();
            }
            else if (func == "run")
            {
                run(args);
            }
            else if (func == "rm")
            {
                rm(args);
            }
            else if (func == "clr")
            {
                clr(args, true);
            }
            else if (func == "varCast")
            {
                varCast(args);
            }
            else if (func == "set" || func == "shared")
            {
                parseInput(args);
            }
            else if (func == "prfile")
            {
                printFile(args);
            }
            else
            {
                String input = func + ' ' + args;
                if (input.Contains("="))
                {
                    parseInput(input);
                }
                else
                {
                    Console.WriteLine("Invalid command.");
                }
            }
        }

        //DOS time function, doesn't actually set the time though
        public void time(String args)
        {
            //For the "/t" argument, print out the time in a human readable format and exit
            if (args == "/t" || args == "/T")
            {
                String r = "";
                if (Cosmos.Hardware.RTC.Hour >= 12)
                {
                    r = byteToString((byte)(Cosmos.Hardware.RTC.Hour - 12)) + ':' + byteToString(Cosmos.Hardware.RTC.Minute) + " PM";
                }
                else
                {
                    r = byteToString(Cosmos.Hardware.RTC.Hour) + ':' + byteToString(Cosmos.Hardware.RTC.Minute) + " AM";
                }
                Console.WriteLine(r);
                return;
            }
            //If no arguments were given, print out the time before continuing
            if (args.Length == 0)
            {
                Console.WriteLine("The current time is: " + byteToString(Cosmos.Hardware.RTC.Hour) + ':'
                    + byteToString(Cosmos.Hardware.RTC.Minute) + ':' + byteToString(Cosmos.Hardware.RTC.Second));
            }
            //Switch on args' length to determine what to do
            bool locker = true;
            while (locker)
            {
                switch (args.Length)
                {
                    case 0:
                        Console.WriteLine("Enter the new time: (HH:MM:SS)");
                        args = Console.ReadLine();
                        if (args.Length == 0)
                        {
                            return;
                        }
                        else if (args.Length != 8)
                        {
                            Console.WriteLine("The system cannot accept the time entered.");
                            args = "";
                            break;
                        }
                        break;
                    case 8:
                        char[] temp = args.ToCharArray();
                        bool error = false;
                        for (int i = 0; i < temp.Length; i++)
                        {
                            if (!isValidChar(temp[i], true))
                            {
                                error = true;
                                break;
                            }
                        }
                        if (!error && temp[2] == ':' && temp[5] == ':')
                        {
                            //Set the new time according to what the user input
                            byte hour = (byte)(Int32.Parse(args.Substring(0, 2)));
                            byte min = (byte)(Int32.Parse(args.Substring(3, 2)));
                            byte sec = (byte)(Int32.Parse(args.Substring(6, 2)));
                            //User entered invalid time check
                            if (hour < 0 || hour > 23 || min < 0 || min > 59 || sec < 0 || sec > 59)
                            {
                                Console.WriteLine("The system cannot accept the time entered.");
                                args = "";
                                break;
                            }
                            else
                            {
                                Console.WriteLine("The new time is: " + byteToString(hour) + ':' + byteToString(min) + ':' + byteToString(sec));
                                //Cosmos.Hardware.RTC.Hour = hour;
                                //Cosmos.Hardware.RTC.Minute = min;
                                //Cosmos.Hardware.RTC.Second = sec;
                                locker = false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("The system cannot accept the time entered.");
                            args = "";
                        }
                        break;
                    default:
                        Console.WriteLine("The system cannot accept the time entered.");
                        args = "";
                        break;
                }
            }
            return;
        }

        //DOS date function, doesn't actually set the date though
        public void date(String args)
        {
            //Print out the current date if the arguments are "/t", "/T", or none
            if (args.Length == 0)
            {
                Console.WriteLine("The current date is: " + SYSTEM_DATE.getLongDate());
            }
            //If the "/t" argument is present, exits
            else if (args == "/t" || args == "/T")
            {
                Console.WriteLine(SYSTEM_DATE.getLongDate());
                return;
            }
            //For all other arguments, switch on the input
            bool locker = true;
            while (locker)
            {
                switch (args.Length)
                {
                    case 0:
                        Console.WriteLine("Enter the new date: (mm-dd-yy)");
                        args = Console.ReadLine();
                        if (args.Length == 0)
                        {
                            return;
                        }
                        else if (args.Length != 8)
                        {
                            Console.WriteLine("The system cannot accept the date entered.");
                            args = "";
                            return;
                        }
                        break;
                    case 8:
                        char[] temp = args.ToCharArray();
                        bool error = false;
                        for (int i = 0; i < temp.Length; i++)
                        {
                            if (!isValidChar(temp[i], false))
                            {
                                error = true;
                                break;
                            }
                        }
                        if (!error && temp[2] == '-' && temp[5] == '-')
                        {
                            //Set the new date according to what the user input
                            byte mon = (byte)(Int32.Parse(args.Substring(0, 2)));
                            byte day = (byte)(Int32.Parse(args.Substring(3, 2)));
                            byte cent = Cosmos.Hardware.RTC.Century;
                            byte yr = (byte)(Int32.Parse(args.Substring(6, 2)));
                            //User entered invalid date check
                            if (mon < 1 || mon > 12 || day < 1 || day > 31 || yr < 0 || yr > 99
                                || (day > 29 && mon == 2 && yr % 4 == 0) || (day > 28 && mon == 2)
                                || (day > 30 && (mon == 4 || mon == 6 || mon == 9 || mon == 11)))
                            {
                                Console.WriteLine("The system cannot accept the date entered.");
                                args = "";
                                break;
                            }
                            else
                            {
                                SYSTEM_DATE.update(day, mon, cent, yr);
                                Console.WriteLine("The new date is: " + SYSTEM_DATE.getLongDate());
                                locker = false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("The system cannot accept the date entered.");
                            args = "";
                        }
                        break;
                    default:
                        Console.WriteLine("The system cannot accept the date entered.");
                        args = "";
                        break;
                }
            }
            return;
        }

        //Clears the console screen
        public void clearScreen()
        {
            Console.Clear();
        }

        //Help function, able to get usages for all possible commands
        public void help(String args)
        {
            if (args == "")
            {
                Console.WriteLine("Type help [command_name] to see the command's usage.");
                Console.WriteLine("Current commands: ");
                for (int i = 0; i < COMMANDS.Length; i++)
                {
                    Console.WriteLine(COMMANDS[i]);
                }
                Console.WriteLine("Global variables may be input with: ");
                Console.WriteLine("<VARNAME> = <ARITHMETIC EXPR>");
                Console.WriteLine("<VARNAME> = <STRING EXPR>");
            }
            else
            {
                String[] temp = args.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (temp.Length != 1)
                {
                    Console.WriteLine("Invalid usage.");
                    Console.WriteLine("Usage: help [command_name]");
                    return;
                }
                if (args == "help")
                {
                    Console.WriteLine("Usage: help [command_name]");
                }
                else if (args == "time")
                {
                    Console.WriteLine("Usage: time [/t | /T] [HH:MM:SS]");
                }
                else if (args == "date")
                {
                    Console.WriteLine("Usage: date [/t | /T] [mm-dd-yy]");
                }
                else if (args == "cls")
                {
                    Console.WriteLine("Usage: cls");
                }
                else if (args == "create")
                {
                    Console.WriteLine("Usage: create <fname>.<ext>");
                }
                else if (args == "dir")
                {
                    Console.WriteLine("Usage: dir");
                }
                else if (args == "out")
                {
                    Console.WriteLine("Usage: out <varname>");
                }
                else if (args == "vars")
                {
                    Console.WriteLine("Usage: vars");
                }
                else if (args == "run")
                {
                    Console.WriteLine("Usage: run [all] [--nest] <fname>.bat [<fname>.bat <fname>.bat ...]");
                    Console.WriteLine("Create statments inside of batch files work by placing all text that is wanted in that file in the body of the batch statement.");
                    Console.WriteLine("Nested batch files are can be automatically created when the parent file is saved with the --nest flag.");
                    Console.WriteLine("Nested run statements will inherit the nesting property of the initial run command.");
                    Console.WriteLine("For example, a batch file contains this: ");
                    Console.WriteLine();
                    Console.WriteLine("create a.bat");
                    Console.WriteLine("set var1 = 1");
                    Console.WriteLine("create b.bat");
                    Console.WriteLine("set var2 = 2");
                    Console.WriteLine("save ");
                    Console.WriteLine("out var1");
                    Console.WriteLine("save ");
                    Console.WriteLine();
                    Console.WriteLine("When running the batch file, two batch files will be made.");
                    Console.WriteLine("a.bat will contain everything from set var1 = 1 to out var1.");
                    Console.WriteLine("b.bat will contain set var2 = 2.");
                    Console.WriteLine();
                    Console.WriteLine("When doing this manually, use \"save \" as a terminator.");
                    Console.WriteLine("Also note that files created inside batch files will automatically overwrite any files that already exist, so be careful!");
                }
                else if (args == "rm")
                {
                    Console.WriteLine("Usage: rm <fname>");
                }
                else if (args == "clr")
                {
                    Console.WriteLine("Usage: clr <varname>");
                }
                else if (args == "varCast")
                {
                    Console.WriteLine("Usage: varCast <varname>");
                }
                else if (args == "set")
                {
                    Console.WriteLine("Usage: set <VARNAME> = <ARITHMETIC EXPR>");
                    Console.WriteLine("Usage: set <VARNAME> = <STRING EXPR>");
                }
                else if (args == "shared")
                {
                    Console.WriteLine("Usage: shared <VARNAME> = <ARITHMETIC EXPR>");
                    Console.WriteLine("Usage: shared <VARNAME> = <STRING EXPR>");
                }
                else if (args == "prfile")
                {
                    Console.WriteLine("Usage: prfile <fname>");
                }
                else
                {
                    Console.WriteLine("Invalid command.");
                }
            }
        }

        //Function for creating files
        public void create(String args)
        {
            //Check conditions of if it has an extension or already exists
            if (args.IndexOf('.') < 0)
            {
                Console.WriteLine("Usage: create <Filename>.<Ext>");
                return;
            }
            else if (fileExists(args))
            {
                //The file already exists, make sure the user wants to overwrite the current file
                Console.WriteLine("Error: file already exists. Overwrite? (Y/N)");
                while (true)
                {
                    String input = Console.ReadLine();
                    if (input.ToLower() == "n")
                    {
                        return;
                    }
                    else if (input.ToLower() == "y")
                    {
                        rm(args);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid option. Overwrite file? (Y/N)");
                    }
                }
            }
            //Make a new file, prompting on each line until the user types "save"
            //Note that typing "save " doesn't save the file, and is used for batch files
            File f = new File(args);
            FILESYS.Add(f);
            int i = 1;
            while (true)
            {
                Console.Write(i + "> ");
                String input = Console.ReadLine();
                if (input == "save")
                {
                    break;
                }
                f.writeLine(input);
                i++;
            }
            Console.WriteLine("*** File Saved ***");
        }

        //Prints out all the current files in the filesystem
        public void dir(String args)
        {
            if (args.Length != 0)
            {
                Console.WriteLine("No arguments allowed. Stop that. You butt.");
                return;
            }
            Console.WriteLine("Filename             Extension  Date       Size");
            Console.WriteLine("---------------------------------------------------");
            File[] temp = new File[FILESYS.Count];
            FILESYS.CopyTo(temp);
            for (int i = 0; i < FILESYS.Count; i++)
            {
                File f = temp[i];
                Console.Write(f.getFileName().PadRight(20) + ' ');
                Console.Write(f.getExtension().PadRight(10) + ' ');
                Console.Write(f.getCreationDate().PadRight(10) + ' ');
                Console.Write(f.getByteSize() + 'B');
                Console.WriteLine();
            }
            Console.WriteLine("Total Files: " + FILESYS.Count);
        }

        //Prints out the content of the given variable
        public void output(String args)
        {
            if (args.Split(' ').Length - 1 > 0)
            {
                Console.WriteLine("Usage: out <VARNAME>");
                return;
            }
            Variable v = getVar(args);
            if (v == null)
            {
                Console.WriteLine("No such variable exists.");
                return;
            }
            Console.WriteLine(v.toString());
        }

        //Prints out all the current variables in the filesystem
        public void vars()
        {
            Variable[] temp = new Variable[GLOBAL_VARS.Count];
            GLOBAL_VARS.CopyTo(temp);
            Console.WriteLine("Variable Name   Type Value");
            Console.WriteLine("-------------------------------");
            for (int i = 0; i < GLOBAL_VARS.Count; i++)
            {
                String type = temp[i].getTypeStr();
                Console.WriteLine(temp[i].getName().PadRight(15) + ' ' + type + ' ' + temp[i].toString());
            }
        }

        //Runs a given batch file or files
        public void run(String args)
        {
            batchNesting = false;
            bool multiple = false;
            bool skipFlag = RUNNING_BATCH_FILES.Count > 0 ? true : false;
            //Split up the arguments based on spaces to get what the arguments are
            String[] arguments = args.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //Set proper flags and arguments
            if (arguments.Length > 1 && arguments[0] == "--nest")
            {
                batchNesting = true;
                multiple = false;
                String[] temp = new String[arguments.Length - 1];
                for (int i = 1; i < arguments.Length; i++)
                {
                    temp[i - 1] = arguments[i];
                }
                arguments = temp;
            } 
            else if (arguments.Length > 1 && arguments[0] == "all" && arguments[1] != "--nest") 
            {
                batchNesting = false;
                multiple = true;
                String[] temp = new String[arguments.Length - 1];
                for (int i = 1; i < arguments.Length; i++)
                {
                    temp[i - 1] = arguments[i];
                }
                arguments = temp;
            }
            else if (arguments.Length > 2 && arguments[0] == "all" && arguments[1] == "--nest")
            {
                batchNesting = true;
                multiple = true;
                String[] temp = new String[arguments.Length - 2];
                for (int i = 2; i < arguments.Length; i++)
                {
                    temp[i - 2] = arguments[i];
                }
                arguments = temp;
            }
            //If we have the proper amount of arguments for run, we do it
            if (multiple || arguments.Length == 1)
            {
                //Convert all the rest of the arguments into an array of File objects
                //Also check that the arguments are properly formatted
                //Enqueue the files onto the run queue
                for (int i = 0; i < arguments.Length; i++)
                {
                    File f = getFile(arguments[i]);
                    if (f == null || f.getExtension() != "bat")
                    {
                        Console.WriteLine("Error: file does not exist or is not a batch file.");
                        Console.WriteLine("Usage: run [all] [--nest] <fname>.bat [<fname>.bat <fname>.bat ...]");
                        return;
                    }
                    RUNNING_BATCH_FILES.Enqueue(new ProcessBlock(f));
                }
                //If this is a nested run command, skip evaluation and let the main one take care of it
                if (skipFlag)
                {
                    return;
                }
                //Run all the things on the run queue
                while (RUNNING_BATCH_FILES.Count > 0)
                {
                    //Grab the correct information off the run queue
                    ProcessBlock p = RUNNING_BATCH_FILES.Dequeue();

                    //Read the line from the file and trim excess
                    //Trimming is used particularly because of the "save" command that create uses
                    String line = p.processFile.readLine(p.currentLine).Trim();
                    //If we've reached the end of the file, we go to the next one, as it has already been removed from the run queue
                    if (line == "EOF")
                    {
                        //Show the user if the file that finished processing was improperly formatted
                        if (p.tempFNames.Count != 0)
                        {
                            Console.WriteLine("File " + p.processFile.getFileName() + " has a missing save statment. File may not have run correctly.");
                        }
                        continue;
                    }
                    //Increment the current line for later
                    p.currentLine++;
                    //Split up the command and check it
                    String[] command = splitCommand(line);
                    //If it's a create function we need to do some special crap
                    if (command[0] == "create")
                    {
                        //Ensure the create command is properly formatted
                        if (command.Length != 2 || command[1].IndexOf('.') < 0)
                        {
                            Console.WriteLine("Error: create takes a filename argument in the form <fname>.<ext>");
                            return;
                        }
                        //If we're reading a second create statement, push the linecount for this file so far
                        if (p.tempFNames.Count > 0)
                        {
                            p.tempLines.Push(line);
                            p.currentLineCount++;
                            p.tempLineCounts.Push(p.currentLineCount);
                            p.currentLineCount = 0;
                        }
                        //Push it onto the stack of file names for that particular batch file
                        p.tempFNames.Push(command[1]);
                    }
                    //If the stack of files names associated with this batch file is not empty, we don't execute it
                    //Instead, we save the line temporarily, or save a file if the line is "save"
                    else if (p.tempFNames.Count > 0)
                    {
                        //We've finished the create statement, write the file by popping the line queue the right number of lines so far
                        //This is essentially matching the closest save to the last create
                        if (line == "save")
                        {
                            String fnametemp = p.tempFNames.Pop();
                            File f = new File(fnametemp);
                            Stack<String> temp = new Stack<String>();
                            for (int k = 0; k < p.currentLineCount; k++)
                            {
                                temp.Push(p.tempLines.Pop());
                            }
                            for (int k = 0; k < p.currentLineCount; k++)
                            {
                                String tempStr = temp.Pop();
                                f.writeLine(tempStr);
                                if (p.tempFNames.Count > 0)
                                {
                                    p.tempLines.Push(tempStr);
                                }
                            }
                            //Add the file to the filesystem, overwriting if needed
                            if (getFile(fnametemp) != null)
                            {
                                FILESYS.RemoveAt(getFileIndex(fnametemp));
                            }
                            if (batchNesting && p.tempFNames.Count >= 0)
                            {
                                FILESYS.Add(f);
                            } 
                            else if (!batchNesting && p.tempFNames.Count == 0) 
                            {
                                FILESYS.Add(f);
                            }
                            //We also get the linecount back off the stack if necessary for future create statements
                            if (p.tempFNames.Count > 0)
                            {
                                p.tempLines.Push(line + ' ');
                                p.currentLineCount++;
                                p.currentLineCount += p.tempLineCounts.Pop();
                            }
                            else
                            {
                                p.currentLineCount = 0;
                            }
                        }
                        //Save the line temporarily
                        else
                        {
                            p.tempLines.Push(line);
                            p.currentLineCount++;
                        }
                    }
                    //It's a normal function, execute it
                    else
                    {
                        execute(command[0], command[1]);
                    }
                    //Enqueue the file back onto the stack, the file has not been finished processing
                    RUNNING_BATCH_FILES.Enqueue(p);
                }
            }
            //Otherwise show the usage of the function, they fucked up
            else
            {
                Console.WriteLine("Error: incorrect number of arguments given to run.");
                Console.WriteLine("Usage: run [all] [--nest] <fname>.bat [<fname>.bat <fname>.bat ...]");
                return;
            }
        }

        //Parses the given expression into a global variable
        public void parseInput(String input)
        {
            //Cut the input into a a variable name and expression
            int endIndex = 0;
            if (input.IndexOf('=') < 0)
            {
                Console.WriteLine("Invalid expression.");
                Console.WriteLine("Usage: <VARNAME> = <EXPR>");
                return;
            }
            else
            {
                endIndex = input.IndexOf('=');
            }
            String varname = input.Substring(0, endIndex);
            String expr = input.Substring(varname.Length + 1).Trim();
            varname = varname.Trim();
            //Make sure that we have both a variable name and expression, and the variable name is proper
            if (varname.Length == 0 || expr.Length == 0)
            {
                Console.WriteLine("Error: Variable name doesn't exist or expression doesn't exist.");
                Console.WriteLine("Usage: <VARNAME> = <EXPR>");
                return;
            }
            else if (!isValidVarName(varname))
            {
                Console.WriteLine("Error: Variable name invalid, may only contain A-Z, a-z, 0-9, _, and $.");
                return;
            }
            //Evaluate the expression
            String val = evalExpr(expr);
            if (val == null)
            {
                Console.WriteLine("Unable to parse expression.");
                return;
            }
            //Make the variable, replacing it if it already exists
            Variable v = null;
            if (val[0] == '\"')
            {
                v = new Variable(varname, val.Substring(1, val.Length - 2));
            }
            else
            {
                v = new Variable(varname, Int32.Parse(val));
            }
            clr(varname, false);
            GLOBAL_VARS.Add(v);
        }

        //Removes the given file from the filesystem
        public void rm(String args)
        {
            if (args.IndexOf('.') < 0 || args.Split(' ').Length - 1 > 0)
            {
                Console.WriteLine("Usage: rm <Filename>.<Ext>");
                return;
            }
            if (fileExists(args))
            {
                FILESYS.RemoveAt(getFileIndex(args));
            }
            else
            {
                Console.WriteLine("File doesn't exist!");
            }
        }

        //Removes the given variable from the filesystem
        public void clr(String args, bool verbose)
        {
            if (args.Split(' ').Length - 1 > 0)
            {
                Console.WriteLine("Usage: clr <VARNAME>");
                return;
            }
            Variable[] temp = new Variable[GLOBAL_VARS.Count];
            GLOBAL_VARS.CopyTo(temp);
            for (int i = 0; i < GLOBAL_VARS.Count; i++)
            {
                if (temp[i].getName() == args)
                {
                    GLOBAL_VARS.RemoveAt(i);
                    return;
                }
            }
            if (verbose)
            {
                Console.WriteLine("No such variable exists.");
            }
        }

        //Cast the variable to a string from int, or vice versa, if possible
        public void varCast(String args)
        {
            if (args.Split(' ').Length - 1 > 0)
            {
                Console.WriteLine("Usage: varCast <VARNAME>");
                return;
            }
            Variable v = getVar(args);
            v.cast();
        }

        //Displays bytes as pairs of two digits 0-9. Used only for printing the time.
        private String byteToString(byte s)
        {
            String r = s.ToString();
            if (s < 10)
            {
                r = '0' + r;
            }
            return r;
        }

        //Returns if a character is valid or not: 0-9, :, - are valid. : is valid if timeOrDate is true, - if it is false
        private bool isValidChar(char c, bool timeOrDate)
        {
            switch (c)
            {
                case '0':
                    return true;
                case '1':
                    return true;
                case '2':
                    return true;
                case '3':
                    return true;
                case '4':
                    return true;
                case '5':
                    return true;
                case '6':
                    return true;
                case '7':
                    return true;
                case '8':
                    return true;
                case '9':
                    return true;
                case ':':
                    return timeOrDate;
                case '-':
                    return !timeOrDate;
                default:
                    return false;
            }
        }

        //Helper for determining if a duplicate file is being made
        private bool fileExists(String fname)
        {
            File f = getFile(fname);
            if (f != null)
            {
                return true;
            }
            return false;
        }

        //Helper for finding files
        private int getFileIndex(String fname)
        {
            File[] temp = new File[FILESYS.Count];
            FILESYS.CopyTo(temp);
            for (int i = 0; i < FILESYS.Count; i++)
            {
                if (temp[i].getFileName() == fname)
                {
                    return i;
                }
            }
            return -1;
        }

        //Helper for getting files from the filesystem
        private File getFile(String fname)
        {
            File[] temp = new File[FILESYS.Count];
            FILESYS.CopyTo(temp);
            int index = getFileIndex(fname);
            if (index == -1)
            {
                return null;
            }
            return temp[index];
        }

        //Helper for getting variables from the filesystem
        private Variable getVar(String n)
        {
            Variable[] temp = new Variable[GLOBAL_VARS.Count];
            GLOBAL_VARS.CopyTo(temp);
            for (int i = 0; i < GLOBAL_VARS.Count; i++)
            {
                if (temp[i].getName() == n)
                {
                    return temp[i];
                }
            }
            return null;
        }

        //The evaluation function for parsing expressions
        private String evalExpr(String expr)
        {
            List<Char> operations = new List<Char>();
            Char[] delim = new Char[] { '+', '-', '*', '/', '&', '|', '^', ' ' };
            //This line takes care of saving the arguments
            String[] arguments = expr.Split(delim, StringSplitOptions.RemoveEmptyEntries);
            //This loop takes care of saving the operations
            foreach (char ch in expr)
            {
                foreach (char de in delim)
                {
                    if (ch == de && ch != ' ')
                    {
                        operations.Add(de);
                        break;
                    }
                }
            }
            //First, check each entry [delimited by +, -, *, /, &, |, ^]
            //If everything is a number, it's arithmetic.
            if (isAllInt(arguments))
            {
                arguments = substitute(arguments);
                return arithmeticOp(arguments, operations);
            }
            //String only
            //A var should begin with $
            //Strings should be enclosed in double quotations
            //Split up the arguments based on those rules and get those arrays
            arguments = splitStrExpr(expr);
            Char[] ops = splitOpsStrExpr(expr);
            //Make sure everything works and is good
            if (ops == null || arguments == null)
            {
                Console.WriteLine("Invalid expression.");
                return null;
            }
            if (ops.Length != (arguments.Length - 1))
            {
                Console.WriteLine("Invalid expression: incorrect number of operators.");
                return null;
            }
            foreach (Char c in ops)
            {
                if (c == '-' || c == '*' || c == '/' || c == '&' || c == '|' || c == '^')
                {
                    Console.WriteLine("Only concatenation (+) is a valid string operator.");
                    return null;
                }
            }
            //Then evaluate the string expression
            return stringOp(arguments);
        }

        //Helper for expression evaluation
        //Returns true if all Strings in the array are "ints"
        //if the string was a variable name, checks to see if the data stored
        //is an int
        private bool isAllInt(String[] terms)
        {
            //If any of the terms cannot be converted to int,
            //They are not all strings
            for (int j = 0; j < terms.Length; j++)
            {
                //If the term is a variable [denoted by $],
                //Check all the stored variables.
                if (terms[j][0] == '$')
                {
                    Variable v = getVar(terms[j].Substring(1));
                    if (v != null)
                    {
                        if (v.getType() != 2)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (!isValidInt(terms[j]))
                {
                    return false;
                }
            }
            return true;
        }

        //Helper for expression evaluation
        //Returns true if all Strings in the array are "ints"
        //Supported operators are +, -, *, /, &, |, ^
        private String arithmeticOp(String[] terms, List<Char> oprs)
        {
            Stack<Int32> operands = new Stack<Int32>();
            Stack<Char> operators = new Stack<Char>();
            Char[] ops = oprs.ToArray();
            Int16 termIndex = 0, opIndex = 0;
            Char op = ops[opIndex];
            //Number helps if we're reading a number or an operator.
            for (int i = 0; i < terms.Length + ops.Length; i++)
            {
                //We're reading a value.
                if (i % 2 == 0)
                {
                    //There won't be any errors because "term" went through
                    //a preliminary check
                    //Also, I'm assuming the loop will end on reading an operand
                    //due to all operators being binary.
                    operands.Push(Int32.Parse(terms[termIndex++]));
                }
                else //We're reading an operator
                {
                    //If operator stack is empty, push operator
                    if (operators.Count == 0)
                    {
                        operators.Push(op);
                        op = ops[++opIndex];
                        continue;
                    }
                    Char toperator = operators.Peek();
                    switch (op)
                    {
                        case '*':
                        case '/':
                            if (toperator != '*' && toperator != '/')
                                operators.Push(op);
                            else
                            {
                                if (operands.Count < 2)
                                {
                                    Console.WriteLine("* /");
                                    Console.WriteLine("Invalid expression entered. Not enough operands.");
                                    return null;
                                }
                                else
                                {
                                    //May overflow.
                                    Int32 op2 = operands.Pop(), op1 = operands.Pop(), result;
                                    if (toperator == '*')
                                        result = op1 * op2;
                                    else
                                        result = op1 / op2;
                                    operands.Push(result);
                                    operators.Pop();
                                    operators.Push(op);
                                }
                            }
                            break;
                        case '+':
                        case '-':
                            if (toperator == '&' || toperator == '|' || toperator == '^')
                                operators.Push(op);
                            else
                            {
                                if (operands.Count < 2)
                                {
                                    Console.WriteLine("+ -");
                                    Console.WriteLine("Invalid expression entered. Not enough operands.");
                                    return null;
                                }
                                else
                                {
                                    Int32 op2 = operands.Pop(), op1 = operands.Pop(), result;
                                    if (toperator == '+')
                                        result = op1 + op2;
                                    else if (toperator == '-')
                                        result = op1 - op2;
                                    else if (toperator == '*')
                                        result = op1 * op2;
                                    else
                                        result = op1 / op2;
                                    operands.Push(result);
                                    operators.Pop();
                                    operators.Push(op);
                                }
                            }
                            break;
                        case '&':
                        case '|':
                        case '^':
                            if (operands.Count < 2)
                            {
                                Console.WriteLine("& | ^");
                                Console.WriteLine("Invalid expression entered. Not enough operands.");
                                return null;
                            }
                            else
                            {
                                Int32 op2 = operands.Pop(), op1 = operands.Pop(), result;
                                if (toperator == '&')
                                    result = op1 & op2;
                                else if (toperator == '|')
                                    result = op1 | op2;
                                else if (toperator == '^')
                                    result = op1 ^ op2;
                                else if (toperator == '+')
                                    result = op1 + op2;
                                else if (toperator == '-')
                                    result = op1 - op2;
                                else if (toperator == '*')
                                    result = op1 * op2;
                                else
                                    result = op1 / op2;
                                operands.Push(result);
                                operators.Pop();
                                operators.Push(op);
                            }
                            break;
                    }
                    if (++opIndex < ops.Length)
                    {
                        op = ops[opIndex];
                    }
                }
            }
            while (operators.Count != 0)
            {
                if (operands.Count < 2)
                {
                    Console.WriteLine("Invalid expression entered. Not enough operands.");
                    return null;
                }
                else
                {
                    Char toperator = operators.Peek();
                    Int32 op2 = operands.Pop(), op1 = operands.Pop(), result;
                    if (toperator == '&')
                        result = op1 & op2;
                    else if (toperator == '|')
                        result = op1 | op2;
                    else if (toperator == '^')
                        result = op1 ^ op2;
                    else if (toperator == '+')
                        result = op1 + op2;
                    else if (toperator == '-')
                        result = op1 - op2;
                    else if (toperator == '*')
                        result = op1 * op2;
                    else
                        result = op1 / op2;
                    operands.Push(result);
                    operators.Pop();
                }
            }
            if (operands.Count > 1)
            {
                Console.WriteLine("Invalid expression entered. Not enough operators.");
                return null;
            }
            return operands.Pop().ToString();
        }

        //Helper for expression evaluation
        //Returns the string arrays concatenated
        //Also replaces variable names
        private String stringOp(String[] terms)
        {
            String bigString = "\"";
            for (int j = 0; j < terms.Length; j++)
            {
                //If the term is a variable [denoted by $],
                //Check all the stored variables.
                if (terms[j][0] == '$')
                {
                    Variable v = getVar(terms[j].Substring(1));
                    if (v != null)
                    {
                        if (v.getType() != 1)
                        {
                            Console.Write("The specified variable ");
                            Console.Write(terms[j].Substring(1));
                            Console.Write(" is not type string.");
                            Console.WriteLine();
                            return null;
                        }
                    }
                    else
                    {
                        Console.Write("The specified variable ");
                        Console.Write(terms[j].Substring(1));
                        Console.Write(" cannot be found.");
                        Console.WriteLine();
                        return null;
                    }
                    bigString += v.toString();
                }
                else
                    bigString += terms[j];
            }
            return bigString + "\"";
        }

        //Checking method to see if integers are properly formatted
        public static bool isValidInt(String s)
        {
            Char[] nums = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            Char[] str = s.ToCharArray();
            if (str[0] != '-' || str[0] != '+')
            {
                bool first = false;
                for (int i = 1; i < nums.Length; i++)
                {
                    if (str[0] == nums[i])
                    {
                        first = true;
                        break;
                    }
                }
                if (!first)
                {
                    return false;
                }
            }
            for (int i = 1; i < str.Length; i++)
            {
                bool isCurCharValid = false;
                for (int j = 0; j < nums.Length; j++)
                {
                    if (str[i] == nums[j])
                    {
                        isCurCharValid = true;
                        break;
                    }
                }
                if (!isCurCharValid)
                {
                    return false;
                }
            }
            return true;
        }

        //Substitution method for arithmetic expression evaluation
        //Takes arguments and converts variable names to appropriate numbers
        private String[] substitute(String[] args)
        {
            String[] ret = new String[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                //If the term is a variable [denoted by $],
                //Check all the stored variables.
                if (args[i][0] == '$')
                {
                    //We already checked if variables are integers or not, so we don't need to check again
                    Variable v = getVar(args[i].Substring(1));
                    ret[i] = v.toString();
                }
                else
                {
                    ret[i] = args[i];
                }
            }
            return ret;
        }

        //Splits up a string expression based on the rules:
        // 1) "string"
        // 2) $variable
        //Everything else is invalid
        //Implementation is essentially a FSA with 4 states, represented by two booleans
        public static String[] splitStrExpr(String expr)
        {
            bool isString = false;
            bool isVar = false;
            List<String> output = new List<String>();
            String temp = "";
            for (int i = 0; i < expr.Length; i++)
            {
                if (isString && isVar)
                {
                    return null;
                }
                else if (isString && !isVar)
                {
                    if (expr[i] == '\"')
                    {
                        isString = false;
                        output.Add(temp);
                        temp = "";
                    }
                    else
                    {
                        temp += expr[i];
                    }
                }
                else if (!isString && isVar)
                {
                    if (expr[i] == '+' || expr[i] == '-' || expr[i] == '*' || expr[i] == '/' || expr[i] == '&' || expr[i] == '|' || expr[i] == '^' || expr[i] == ' ')
                    {
                        isVar = false;
                        output.Add(temp);
                        temp = "";
                    }
                    else
                    {
                        temp += expr[i];
                    }
                }
                else if (!isString && !isVar)
                {
                    if (expr[i] == '\"')
                    {
                        isString = true;
                    }
                    else if (expr[i] == '$')
                    {
                        isVar = true;
                        temp += expr[i];
                    }
                    else
                    {
                        if (expr[i] != '+' && expr[i] != '-' && expr[i] != '*' && expr[i] != '/' && expr[i] != '&' && expr[i] != '|' && expr[i] != '^' && expr[i] != ' ')
                        {
                            return null;
                        }
                    }
                }
            }
            //If the last term read was a variable, we add it to the list as well
            if (isVar)
            {
                output.Add(temp);
            }
            String[] outVars = new String[output.Count];
            output.CopyTo(outVars);
            return outVars;
        }

        //Splits up a string expression based on the same rules as above, but instead we grab the parts between string constants and variables
        private static Char[] splitOpsStrExpr(String expr)
        {
            bool isString = false;
            bool isVar = false;
            List<Char> output = new List<Char>();
            for (int i = 0; i < expr.Length; i++)
            {
                if (isString && isVar)
                {
                    return null;
                }
                else if (isString && !isVar)
                {
                    if (expr[i] == '\"')
                    {
                        isString = false;
                    }
                }
                else if (!isString && isVar)
                {
                    if (expr[i] == '+' || expr[i] == '-' || expr[i] == '*' || expr[i] == '/' || expr[i] == '&' || expr[i] == '|' || expr[i] == '^' || expr[i] == ' ')
                    {
                        isVar = false;
                    }
                }
                else if (!isString && !isVar)
                {
                    if (expr[i] == '\"')
                    {
                        isString = true;
                    }
                    else if (expr[i] == '$')
                    {
                        isVar = true;
                    }
                    else
                    {
                        if (expr[i] != '+' && expr[i] != '-' && expr[i] != '*' && expr[i] != '/' && expr[i] != '&' && expr[i] != '|' && expr[i] != '^' && expr[i] != ' ')
                        {
                            return null;
                        }
                        else
                        {
                            if (expr[i] != ' ')
                            {
                                output.Add(expr[i]);
                            }
                        }
                    }
                }
            }
            Char[] temp = new Char[output.Count];
            output.CopyTo(temp);
            return temp;
        }

        //Checks if a variable name is valid or not based on a constant alphabet
        private static bool isValidVarName(String name)
        {
            Char[] valid = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I'
            , 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V'
            , 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i'
            , 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v'
            , 'w', 'x', 'y', 'z', '_', '$', '0', '1', '2', '3', '4', '5', '6'
            , '7', '8', '9'};
            if (name == "create" || name == "save")
            {
                return false;
            }
            for (int i = 0; i < name.Length; i++)
            {
                bool yes = false;
                for (int j = 0; j < valid.Length; j++)
                {
                    if (name[i] == valid[j])
                    {
                        yes = true;
                    }
                }
                if (!yes)
                {
                    return false;
                }
            }
            return true;
        }

        //Splits up a string into two parts, where the first part is before the first space and the second part is after it
        private static String[] splitCommand(String input)
        {
            String[] output = new String[2];
            //Cut the input into a function call and arguments
            int endIndex = 0;
            if (input.IndexOf(' ') < 0)
            {
                endIndex = input.Length - 1;
            }
            else
            {
                endIndex = input.IndexOf(' ');
            }
            output[0] = input.Substring(0, endIndex + 1).Trim();
            output[1] = input.Substring(output[0].Length).Trim();
            return output;
        }

        //Prints the output of a given file
        public void printFile(String args)
        {
            if (args.IndexOf('.') < 0 || args.Split(' ').Length - 1 > 0)
            {
                Console.WriteLine("Usage: rm <Filename>.<Ext>");
                return;
            }
            if (fileExists(args))
            {
                File f = getFile(args);
                f.printFileContent();
            }
            else
            {
                Console.WriteLine("File doesn't exist!");
            }
        }
    }
}
