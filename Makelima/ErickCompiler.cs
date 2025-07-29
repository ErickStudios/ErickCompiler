/**
 * ErickCompiler.cs
 * 
 * las funciones del administrado de proyectos
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErickCompiler
{
    class ErickCompiler
    {

        static ErickAssambler EasmCompiler = new();

        public static Dictionary<string, string> InExecutionVariables = new();

        /**
         * Info
         * 
         * lanzar una informacion al outpud
         */
        public static void Info(string Str)
        {
            ConsoleColor Popc = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("(INFO) " + Str);
            Console.ForegroundColor = Popc;

        }

        /**
         * Warn
         * 
         * lanzar una advertencia al outpud
         */
        public static void Warn(string Str)
        {
            ConsoleColor Popc = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("(!) " + Str);

            Console.ForegroundColor = Popc;
        }

        /**
         * Err
         * 
         * lanzar un error al outpud
         */
        public static void Err(string Str)
        {
            ConsoleColor Popc = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("(X) " + Str);
            Console.ForegroundColor = Popc;
        }

        /**
         * Syntax
         * 
         * resuelve una sintaxis
         */
        public static string Syntax(string Str)
        {
            if (Str.StartsWith("\"") && Str.EndsWith("\""))
            {
                string result = Str.Substring(1, Str.Length - 2);

                foreach (var item in InExecutionVariables)
                {
                    result = result.Replace($"%{item.Key}%", item.Value);
                }

                return result;
            }
            else if (
                Str.StartsWith("(") && Str.EndsWith(")")
                )
            {
                string condition = Str.Substring(1, Str.Length - 2);

                if (
                    condition.Contains("==")
                    )
                {
                    string[] ss = condition.Split("==");
                    
                    return (Syntax(ss[0]) == Syntax(ss[1])).ToString();
                }
            }

            return Str;
        }

        /**
         * WorckSpace
         * 
         * representa un espacio de trabajo
         */
        public struct WorckSpace
        {
            /**
             * Uri
             * 
             * representa la ubicacion del proyecto
             */
            public string Uri;

            /**
             * Makelima
             * 
             * el contenido de el archivo makelima.mkl
             */
            public string Makelima;

            /**
             * NormalizePath
             * 
             * normaliza una ruta
             */
            public string NormalizePath(string path)
            {
                //Console.WriteLine(path.Replace("/", Path.DirectorySeparatorChar.ToString()));
                return path.Replace("/", Path.DirectorySeparatorChar.ToString());
            }

            /**
             * GetTheFirstAparitionOfTheModule
             * 
             * obtiene la primera carpeta donde se haya encontrado algo
             */
            public string GetTheFirstAparitionOfTheModule(string ModuleName)
            {
                string Normalized = NormalizePath(ModuleName);

                //Console.WriteLine(Path.Join(Uri, "dependences", Normalized));
                //Console.WriteLine(Path.Join(Uri, Normalized));

                if (File.Exists(Path.Join(Uri, "dependences", Normalized)))
                {
                    return Path.Join(Uri, "dependences", Normalized);
                }
                else if (
                    File.Exists(Path.Join(Uri, Normalized))
                    )
                {
                    return Path.Join(Uri, Normalized);
                }

                return "?";
            }

            /**
             * WriteModule
             * 
             * escribir un modulo
             */
            public void WriteModule(string file, string content)
            {
                if (
                    !File.Exists(Path.Join(Uri, file))
                    )
                    File.Create(Path.Join(Uri, file));

                ///
                /// esperar hasta que este libre
                ///
                while (true)
                {
                    try
                    {
                        using (FileStream stream = File.Open(Path.Join(Uri, file), FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            break;
                        }
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(100);
                    }
                }

                ///
                /// escribir en el archivo
                ///
                File.WriteAllText(Path.Join(Uri, file), content);
            }

            /**
             * OpenAsFolder
             * 
             * abrir el worckspace como una carpeta
             */
            public void OpenAsFolder(string path)
            {
                if (
                    !Directory.Exists(path)
                    )
                {
                    Err($"Directory '{path}' not founded");
                    return;
                }

                Uri = path;

                if (
                    !File.Exists(Path.Join(path, "build.erc"))
                    )
                {
                    Err($"Directory '{path}' dont have a 'build.erc' file");
                    return;
                }

                Makelima = File.ReadAllText(Path.Join(path, "build.erc"));

                Info($"Valid worckspace, you can finally work in the worckspace");
            }

            /**
             * BuildFile
             * 
             * construir un archivo
             */
            public string BuildFile(string BuildFileN, string BuildWith)
            {
                if (
                    BuildWith == "EASM_Hlvc"
                    )
                {
                    string outpud = BuildFileN.Replace("\r","");

                    string[] lines = outpud.Split("\n");

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (
                            lines[i].Trim() == "%include"
                            )
                        {
                            //Console.WriteLine(GetTheFirstAparitionOfTheModule(lines[i + 1].Trim()));
                            outpud = outpud.Replace(
                                "%include\n" + lines[i + 1], 
                                ( File.Exists(GetTheFirstAparitionOfTheModule(lines[i + 1].Trim())) ? BuildFile(File.ReadAllText(GetTheFirstAparitionOfTheModule(lines[i + 1].Trim())), BuildWith) : "")
                                );
                        }
                    }

                    return outpud;
                }

                return "";
            }

            /**
             * Build
             * 
             * construye el worckspace
             */
            public void Build(string Params)
            {
                InExecutionVariables["ps"] = Params;

                string[] instrucciones = Makelima.Replace("\r", "").Split("\n");

                for (int i = 0; i < instrucciones.Length; i++)
                {
                    string instruccion = instrucciones[i].Trim();

                    if (
                        instruccion.StartsWith("echo ")
                        )
                    {
                        Console.WriteLine(Syntax(instruccion.Substring(5)));
                    }
                    else if (
                        instruccion.StartsWith("if ")
                        )
                    {
                        string cnd = instruccion.Substring(3).Split("then ")[0].Trim();
                        string label = instruccion.Substring(3).Split("then ")[1].Trim();

                        if (Syntax(cnd) == "True")
                        {
                            i = 0;
                            while (instrucciones[i].Trim() != ("function " + label))
                            {
                                i++;
                            }

                            i++;
                        }
                    }
                    else if (
                        instruccion.Contains("=")
                        )
                    {
                        string varname = instruccion.Split("=")[0].Trim();

                        string value = instruccion.Substring(instruccion.IndexOf("=") + 1).Trim();

                        InExecutionVariables[varname] = Syntax(value);

                        //Console.WriteLine($"'{varname}' is '{value}'");
                    }
                    else if (
                        instruccion == "stopScript()"
                        )
                    {
                        return;
                    }
                    else if (
                        instruccion.EndsWith("()")
                        )
                    {
                        string funct = instruccion.Substring(0, instruccion.Length - 2);

                        i = 0;
                        while (instrucciones[i].Trim() != ("function " + funct))
                        {
                            i++;
                        }

                        i++;
                    }
                    else if (
                        instruccion == "compile"
                        )
                    {
                        i++;
                        instruccion = instrucciones[i].Trim();

                        while (instruccion != "end")
                        {
                            if (!(i < instrucciones.Length)) break;

                            instruccion = instrucciones[i].Trim();

                            if (
                                instruccion == "[_Entry]"
                                )
                            {
                                string entry = "";

                                i++;
                                instruccion = instrucciones[i].Trim();

                                while (instruccion != "[end _Entry]")
                                {
                                    if (i > instrucciones.Length) break;

                                    instruccion = instrucciones[i].Trim();

                                    if (
                                        instruccion == ""
                                        )
                                    {

                                    }
                                    else if (
                                        instruccion == "[end _Entry]"
                                        )
                                    {

                                    }
                                    else if (
                                        instruccion.StartsWith("#")
                                        )
                                    {

                                    }
                                    else
                                    {
                                        entry += instruccion + "\n";
                                    }
                                    i++;
                                }

                                string[] entry_converted = entry.Split("\n");

                                string BuildWith = "Lima";
                                string BuildFiled = "";
                                string ResultFile = "";

                                for (global::System.Int32 j = 0; j < entry_converted.Length; j++)
                                {
                                    string propiety = entry_converted[j].Trim();
                                    if (
                                        propiety.StartsWith("build ")
                                        )
                                    {
                                        BuildFiled = Syntax(propiety.Substring(6));
                                    }
                                    else if (
                                        propiety.StartsWith("As ")
                                        )
                                    {
                                        ResultFile = Syntax(propiety.Substring(3));
                                    }
                                    else if (
                                        propiety.StartsWith("With ")
                                        )
                                    {
                                        BuildWith = Syntax(propiety.Substring(5));
                                    }
                                }

                                string le = (BuildFile(File.ReadAllText(GetTheFirstAparitionOfTheModule(BuildFiled)), BuildWith));

                                //Console.WriteLine(Path.Join(Uri, NormalizePath(ResultFile)));

                                //Console.WriteLine(le);

                                File.WriteAllText(Path.Join(Uri, NormalizePath(ResultFile)), EasmCompiler.ConvertCode(EasmCompiler.TranslateHightLevelToAsm(le)));
                            }
                            else if (
                                instruccion == ""
                                )
                            {

                            }
                            else if (
                                instruccion == "end"
                                )
                            {
                                break;
                            }
                            else if (
                                instruccion.StartsWith("#")
                                )
                            {

                            }
                            else
                            {
                                Warn($"Invalid instruction '{instruccion}'");
                            }
                            i++;
                        }
                    }
                }
            }
        }

        public WorckSpace CurrWorckSpace = new();
    }
}
