/**
 * ErickAssambler.cs
 * 
 * el compilador de ErickAssambler 
 * 
 * que puede compilar ErickAssembly o ErickAssembly-HightLevel a ErickBinaryFormat
 */

using System;
using System.Collections.Generic;

namespace ErickCompiler
{
    class ErickAssambler
    {
        public char mov_instruction = Convert.ToChar(1);
        public char add_instruction = Convert.ToChar(2);
        public char sub_instruction = Convert.ToChar(3);
        public char div_instruction = Convert.ToChar(4);
        public char imul_instruction = Convert.ToChar(5);
        public char incr_instruction = Convert.ToChar(6);
        public char decr_instruction = Convert.ToChar(7);
        public char jump_instruction = Convert.ToChar(8);
        public char ret_instruction = Convert.ToChar(9);
        public char section_instruction = Convert.ToChar(10);
        public char interruption_instruction = Convert.ToChar(11);
        public char jq_instruction = Convert.ToChar(12);
        public char jg_instruction = Convert.ToChar(13);
        public char jng_instruction = Convert.ToChar(14);
        public char jnq_instruction = Convert.ToChar(15);
        public char extern_ptr_write_instruction = Convert.ToChar(16);
        public char extern_ptr_get_instruction = Convert.ToChar(17);
        public char lea_instruction = Convert.ToChar(18);
        public char nop_instruction = Convert.ToChar(19);
        public char ptr_instruction = Convert.ToChar(20);
        public char loop_instruction = Convert.ToChar(21);
        public char shiftbitsleft_instruction = Convert.ToChar(22);
        public char shiftbitsright_instruction = Convert.ToChar(23);
        public char invalid_instruction = Convert.ToChar(24);
        public char safetynow_for_up = Convert.ToChar(22);
        public char NULL_PARAM = Convert.ToChar(22);

        /**
         * LexerInstruction
         * 
         * define una instruccion para el lexer
         */
        public struct LexerInstruction
        {
            /**
             * opcode
             * 
             * el codigo
             */
            public char opcode;

            /**
             * needs_params
             * 
             * si la instruccion necesita parametros
             */
            public bool needs_params;

            /**
             * params_count
             * 
             * la cantidad de parametros
             */
            public int params_count;

            /**
             * LexerInstruction
             * 
             * inicia una instancia de la instruccion
             */
            public LexerInstruction(char Opcode, int ParamsCount)
            {
                opcode = Opcode;

                needs_params = true;

                if (
                    ParamsCount == 0
                    )
                {
                    needs_params = false;
                }

                params_count = ParamsCount;
            }
        }

        /**
         * HightLevelAutopbusMethodType
         * 
         * el tipo del metodo de un autopbus
         */
        public enum HightLevelAutopbusMethodType
        {
            Swinc = 0
        }

        /**
         * HightLevelAutopbus
         * 
         * definir un metodo que usara una variable estructurada
         */
        public struct HightLevelAutopbus
        {
            /**
             * HightLevelAutopbusMethodType
             * 
             * el tipo de esa cosa
             */
            public HightLevelAutopbusMethodType Type;

            /**
             * Body
             * 
             * el cuerpo de el autopbus
             */
            public string Body;
        }

        /**
         * HightLevelContext
         * 
         * contexto para el compilador de alto nivel
         */
        public struct HightLevelContext
        {
            /**
             * UsingAscii
             * 
             * si el archivo esta en compatibilidad con ASCII o CHAR8
             */
            public bool UsingAscii = false;

            /**
             * symbols
             * 
             * los simbolos y sus opcodes
             */
            public Dictionary<string, string> symbols = new();

            /**
             * Autopbuses
             * 
             * los swinc y las otras cosas
             */
            public Dictionary<string, HightLevelAutopbus> Autopbuses = new();

            /**
             * HightLevelContext
             * 
             * iniciar
             */
            public HightLevelContext()
            {
            }
        }

        /**
         * HightLevelSearchSpace
         * 
         * busca un espacio vacio para insertar una referencia
         */
        public string
            HightLevelSearchSpace
            (
            HightLevelContext context
            )
        {
            ///
            /// los registros no admitidos
            ///
            string[] not_allowed_registers = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "256" };

            ///
            /// buscar un registro que este libre
            ///
            for (int i = 1; i < 2001; i++)
            {
                ///
                /// si el registro fue encontrado
                ///
                if (!context.symbols.ContainsValue(i.ToString()) && !not_allowed_registers.Contains(i.ToString())) return i.ToString();
            }

            ///
            /// si no , retornar falso
            ///
            return "0";
        }

        /**
         * SolveString
         * 
         * resolver un string
         */
        public string SolveString
            (
            string unsolved
            )
        {
            ///
            /// el resultado
            ///
            string result = "";

            ///
            /// resolver el string
            ///
            for (int i = 0; i < unsolved.Length; i++)
            {
                ///
                /// si es escape
                ///
                if (
                    unsolved[i] == '\\'
                    )
                {
                    ///
                    /// ver los codigos de escape
                    ///
                    switch (unsolved[i + 1].ToString())
                    {
                        case "s":
                            result += ""; break;
                        case "\\":
                            result += "\\"; break;
                        case "n":
                            result += "\n"; break;
                        default:
                            result += "?";
                            break;
                    }

                    i++;
                }
                else
                {
                    ///
                    /// insertar el caracter si no es codigo de escape
                    ///

                    result += unsolved[i].ToString();
                }

            }
            return result;

        }

        /**
         * HightLevelSyntax
         * 
         * sintaxis para el alto nivel
         */
        public string
            HightLevelSyntax
            (
                string syntaxa
            )
        {
            string syntax = syntaxa.Replace("\\s", " ");

            ///
            /// StrLen
            /// 
            /// obtener longitud de un string
            ///
            if (
                syntax.StartsWith("StrLen ")
                )
            {
                return HightLevelSyntax(syntax.Substring(7)).Length.ToString();
            }

            ///
            /// String puro
            ///
            else if (
                syntax.StartsWith("\"") && syntax.EndsWith("\"")
                )
            {
                return syntax.Substring(1, syntax.Length - 2).Replace("\\s", " ").Replace("\\n", "\n");
            }

            return syntax;
        }

        /**
         * Warnning
         * 
         * shows a warnning
         */
        public void
            Warnning
            (
            string message
            )
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("(!) " + message);
        }

        /**
         * TranslateHightLevelToAsm
         * 
         * traducir codigo de hlvc a el ErickAssembly corriente
         */
        public string
            TranslateHightLevelToAsm
            (
            string code
            )
        {
            ///
            /// separar las lineas
            ///
            string[] lines = code.Replace("\r", "").Split("\n");

            ///
            /// el codigo resultante
            ///
            string outpud = code.Contains("_Use DriverCompiler") ? "MP\n" : "EA\n";

            ///
            /// si esta en el alto nivel
            ///
            bool in_hightlevel = true;

            ///
            /// el contexto del compilador
            ///
            HightLevelContext Context = new HightLevelContext();

            ///
            /// el verificador que verifica que pools no han sido liberados
            ///
            Dictionary<string, string> NoFreededPools = new Dictionary<string, string>();

            ///
            /// traducir linea por linea
            ///
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                ///
                /// instrucciones divididas
                ///
                string[] instructions_divided = line.Trim().Split(" ");

                ///
                /// entrar o salir del alto nivel
                ///
                if (
                    line.Trim().TrimEnd() == "HightLevel[" || line.Trim().TrimEnd() == "]HightLevel")
                {
                    if (
                        line.Trim().TrimEnd() == "HightLevel["
                        )
                    {
                        in_hightlevel = true;
                    }
                    else if (
                        line.Trim().TrimEnd() == "]HightLevel"
                        )
                    {
                        in_hightlevel = false;
                    }
                }

                ///
                /// si no hay instrucciones o esta en el alto nivel
                ///
                else if (
                    instructions_divided.Length == 0 && !in_hightlevel
                    )
                {
                    if (
                        !in_hightlevel
                        )
                    {
                        outpud += line + "\n";
                    }
                    continue;
                }

                ///
                /// compatibilidad con ASCII
                ///
                else if (
                    line.Trim() == "use \"ASCII\"" || line.Trim() == "_Use ASCII"
                    )
                {
                    Context.UsingAscii = true;
                }

                ///
                /// swinc [Name] {
                ///     [attributes]
                /// }
                /// 
                /// define un swinc
                ///
                else if (
                    instructions_divided[0] == "swinc" && lines[i + 1].Trim() == "{"
                    )
                {
                    ///
                    /// definir el nombre
                    ///
                    string swinc_name = instructions_divided[1];

                    ///
                    /// definir el cuerpo
                    ///
                    string body = "";

                    i += 2;

                    ///
                    /// hacer el cuerpo
                    ///
                    while (
                        lines[i].Trim() != "}"
                        )
                    {
                        ///
                        /// si es un auto variable
                        ///
                        if (
                            lines[i].Trim().StartsWith("autv")
                            )
                        {
                            string type = lines[i].Trim().Split("::")[1].Replace(";", "");
                            string namee = lines[i].Trim().Substring(5).Split("::")[0].Replace(";", "");

                            ///
                            /// verificar si el tipo es otro swinc para remplazar todos los elementos
                            ///
                            if (
                                type.StartsWith("swinc ")
                                )
                            {
                                string body_m = Context.Autopbuses[type.Substring(6)].Body;

                                body_m = body_m.Replace("autv ", "autv " + namee + ".");

                                body += body_m;
                            }
                            else
                            {
                                ///
                                /// añadirlo al cuerpo
                                ///
                                body += lines[i].Trim();
                            }
                        }
                        i++;
                    }

                    HightLevelAutopbus Autopbus_swinc = new();

                    Autopbus_swinc.Type = HightLevelAutopbusMethodType.Swinc;
                    Autopbus_swinc.Body = body;

                    //Console.WriteLine(body);

                    Context.Autopbuses[swinc_name] = Autopbus_swinc;
                }

                ///
                /// autopbus [Name]::[type]
                /// 
                /// definir una variable estructurada
                ///
                else if (
                    line.Trim().StartsWith("autopbus ")
                    )
                {
                    ///
                    /// definir los parametros
                    ///
                    string vparam = line.Trim().Substring(9, line.Trim().Length - 9);

                    ///
                    /// definir el nombre
                    ///
                    string varname = vparam.Split("::")[0];

                    ///
                    /// definir el tipo
                    ///
                    string vartype = vparam.Split("::")[1];

                    ///
                    /// comparar el tipo
                    ///

                    ///
                    /// es un swinc
                    ///
                    if (vartype.StartsWith("swinc "))
                    {
                        string swinc_name = vartype.Substring(6);

                        string[] bodym = Context.Autopbuses[swinc_name].Body.Split(";");

                        for (int vvmm = 0; vvmm < (bodym.Length - 1); vvmm++)
                        {

                            string item = bodym[vvmm];

                            ///
                            /// el nombre
                            ///
                            string ReferenceName = varname + "." + item.Substring(5).Split("::")[0].Trim();

                            ///
                            /// en donde se va a localizar
                            ///
                            string register_located = HightLevelSearchSpace(Context);

                            ///
                            /// añadirla a las referencias
                            ///
                            Context.symbols[ReferenceName] = register_located;

                            Console.WriteLine("Compile variable " + ReferenceName);

                            ///
                            /// añadir documentacion
                            ///
                            outpud += ";\nDeclare variable " + ReferenceName + "\n;*\n";

                            ///
                            /// añadir la instruccion al archivo final
                            ///
                            outpud += ReferenceName + " = " + register_located + "\n";
                        }
                    }
                }

                ///
                /// modificar un elemento de un pool
                ///
                else if (
                    (instructions_divided.Length == 3) ?
                    instructions_divided[1].StartsWith("[") && instructions_divided[1].EndsWith("]") : false
                    )
                {
                    ///
                    /// el item a modificar
                    ///
                    string item_set = instructions_divided[1].Substring(1, instructions_divided[1].Length - 2);

                    ///
                    /// el nuevo valor
                    ///
                    bool ee_n = (instructions_divided[2] != "'\\s'" && instructions_divided[2] != "'\\n'");

                    ///
                    /// si es un caracter
                    ///
                    bool is_char = (instructions_divided[2].StartsWith('\'') && instructions_divided[2].EndsWith('\''));

                    ///
                    /// añadir la instruccion al archivo final
                    ///
                    outpud += "set_value_with_value 1," + Context.symbols[instructions_divided[0]] + "\nmove 2," + item_set + "\nmove 3," +
                        (is_char ? ((ee_n) ? instructions_divided[2].ToUpper() : instructions_divided[2]) : instructions_divided[2]) + (is_char ? (ee_n ? (instructions_divided[2].ToUpper() != instructions_divided[2] ? "\nmove 4,32\nadd 3,4" : "") : "") : "") + "\nsystem_call 16,1\n";
                }

                ///
                /// SetVar [VarName] [NewValue]
                /// 
                /// mover un valor
                ///
                else if (
                    instructions_divided[0] == "SetVar"
                    )
                {
                    ///
                    /// la variable
                    ///
                    string var_set = instructions_divided[1];

                    ///
                    /// el nuevo valor
                    ///
                    string syntax = instructions_divided[2];

                    ///
                    /// añadir la instruccion al archivo final
                    ///
                    outpud += "move " + var_set + "," + syntax + "\n";
                }

                ///
                /// #define [MACRO] [VALUE]
                /// 
                /// definir una macro
                ///
                else if (
                    instructions_divided[0] == "#define"
                    )
                {
                    ///
                    /// añadir documentacion
                    ///
                    outpud += ";\n Define macro " + instructions_divided[1].Replace("*", "") + "\n;*\n";

                    ///
                    /// añadir la instruccion al archivo final
                    ///
                    outpud += instructions_divided[1] + " = " + instructions_divided[2] + "\n";
                }

                ///
                /// SetValueWithValue [VarName] [NewValue]
                /// 
                /// modificar una variable con el valor de otra variable
                ///
                else if (
                    instructions_divided[0] == "SetVarWithValue"
                    )
                {
                    ///
                    /// la variable 1
                    ///
                    string var_set = instructions_divided[1];

                    ///
                    /// la variable 2
                    ///
                    string syntax = instructions_divided[2];

                    ///
                    /// añadir la instruccion al archivo final
                    ///
                    outpud += "set_value_with_value " + var_set + "," + syntax + "\n";
                }

                ///
                /// Print [Text]
                /// 
                /// Imprimir un texto en la consola
                ///
                else if (
                    instructions_divided[0] == "Print"
                    )
                {
                    ///
                    /// el string
                    ///
                    string the_string = HightLevelSyntax(instructions_divided[1]);

                    ///
                    /// añadir documentacion
                    ///
                    outpud += ";\n; Print " + the_string.Replace("*", "\\l") + "\n; *\n";

                    ///
                    /// si es una linea
                    ///
                    if (
                        the_string == "\n"
                        )
                    {
                        ///
                        /// añadir la linea
                        ///
                        outpud += "move 1,\'\\n\'\nsystem_call 8,1\n";
                    }

                    ///
                    /// si es un string
                    ///
                    else
                    {
                        ///
                        /// recorrer el array
                        ///
                        foreach (var item in the_string)
                        {
                            ///
                            /// es minuscula?
                            ///
                            string mm = (item == ' ' ? "\\s" : (item == '\n' ? "\\n" : item.ToString()));

                            //Console.Write(mm);

                            ///
                            /// añadir el caracter
                            ///
                            outpud += "move 1,\'" + (Context.UsingAscii ? (mm[0] != '\\' ? mm.ToUpper() : mm) : mm) + (Context.UsingAscii && (mm.ToUpper() != mm) ? ((mm[0] != '\\' ? "\'\nmove 2,32\nadd 1,2\n" : "\'")) : "\'") + "\nsystem_call 8,1\n";
                        }
                    }

                    ///
                    /// añadir la instruccion al archivo final
                    ///
                    outpud += "system_call 9,0\n";
                }

                ///
                /// trabajar con punteros
                ///
                else if (
                    line.Trim() == "[_Use _WorkWithPointers] _WorkActivity"
                    )
                {
                    ///
                    /// añadir la instruccion al archivo final
                    ///
                    outpud += "_WorkWithPointers ";
                }

                ///
                /// call [Function]
                /// 
                /// llamar a una funcion
                ///
                else if (
                    instructions_divided[0] == "call"
                    )
                {
                    ///
                    /// añadir la instruccion al archivo final
                    ///
                    outpud += "call_function " + instructions_divided[1] + "\n";
                }

                /*
                ///
                /// ExecuteFile [BufferToName]
                ///
                else if (
                    instructions_divided[0] == "ExecuteFile"
                    )
                {
                    outpud += "call_function " + instructions_divided[1] + "\n";
                }*/


                ///
                /// FUNCTION [FunctionName]
                /// 
                /// definir una funcion
                ///
                else if (
                    instructions_divided[0] == "FUNCTION"
                    )
                {
                    ///
                    /// buscar un registro para la funcion
                    ///
                    string register_located = HightLevelSearchSpace(Context);

                    ///
                    /// añadir documentacion
                    ///
                    outpud += ";\n; function " + instructions_divided[1].Replace("*", "\\l") + "\n; *\n";

                    ///
                    /// añadir la instruccion al archivo final
                    ///
                    outpud += instructions_divided[1] + " = " + register_located + "\nfunction " + instructions_divided[1] + "\n";

                    ///
                    /// añadir la funcion a la lista de simbolos
                    ///
                    Context.symbols[instructions_divided[1]] = register_located;

                }

                ///
                /// __asm [line]
                /// 
                /// insertar ErickAssembly puro
                ///
                else if (
                    line.Trim().StartsWith("__asm ")
                    )
                {
                    ///
                    /// añadir la instruccion al archivo final
                    ///
                    outpud += line.Trim().Substring(6) + "\n";
                }
                /*
                else if (
                    instructions_divided[0] == "PrintPool"
                    )
                {
                    string the_pool = instructions_divided[1];
                }*/

                ///
                /// FreePool [Pool]
                /// 
                /// liberar un pool
                ///
                else if (
                    instructions_divided[0] == "FreePool"
                    )
                {
                    NoFreededPools.Remove(instructions_divided[1]);

                    ///
                    /// añadir la instruccion al archivo final
                    ///
                    outpud += ";\n; FreePool " + instructions_divided[1] + "\n;*\n" + "system_call 20," + instructions_divided[1] + "\n";
                }

                ///
                /// [LET/CHAR/WORD] [VarName]
                /// 
                /// definir una variable
                ///
                else if (
                    instructions_divided[0] == "CHAR" || instructions_divided[0] == "LET" || instructions_divided[0] == "WORD"
                    )
                {
                    ///
                    /// el nombre
                    ///
                    string ReferenceName = instructions_divided[1];

                    ///
                    /// en donde se va a localizar
                    ///
                    string register_located = HightLevelSearchSpace(Context);

                    ///
                    /// añadirla a las referencias
                    ///
                    Context.symbols[ReferenceName] = register_located;

                    ///
                    /// añadir documentacion
                    ///
                    outpud += ";\nDeclare variable " + ReferenceName + "\n;*\n";

                    ///
                    /// añadir la instruccion al archivo final
                    ///
                    outpud += ReferenceName + " = " + register_located + "\n";

                }

                ///
                /// ENUM_MEMBER [NAME] [VALUE]
                /// 
                /// define un miembro de una enumeracion
                ///
                else if (
                    instructions_divided[0] == "_EnumMember"
                    )
                {

                    ///
                    /// el nombre
                    ///
                    string ReferenceName = instructions_divided[1];

                    ///
                    /// el valor
                    ///
                    string value = instructions_divided[2];

                    ///
                    /// añadir documentacion
                    ///
                    outpud += ";\nEnum Member " + ReferenceName + "\n;*\n";

                    ///
                    /// añadir la instruccion al archivo final
                    ///
                    outpud += ReferenceName + " = " + value + "\n";
                }

                ///
                /// _Math [var] [operation] [param]
                /// 
                /// operaciones matematicas
                ///
                else if (
                    instructions_divided[0] == "_Math"
                    )
                {
                    string operation = instructions_divided[2];

                    string variable_work = instructions_divided[1];
                    string parameter = instructions_divided[3];

                    ///
                    /// añadir documentacion
                    ///
                    outpud += ";\nOperate Variable " + variable_work + " with operator " + operation + "\n;*\n";

                    switch (operation)
                    {
                        case "+":
                            outpud += "add ";
                            break;
                        case "-":
                            outpud += "rest ";
                            break;
                        case "*":
                            outpud += "multiplique ";
                            break;
                        case "/":
                            outpud += "divide ";
                            break;
                        case ">>":
                            outpud += "srght ";
                            break;
                        case "<<":
                            outpud += "slft ";
                            break;
                        default:
                            outpud += "AAAAAAAAAAAAHHHHHHHH ";
                            Warnning("Invalid operation, solve the code , you will thank you to me it ^-^, well is just a warnning, nothing important");
                            break;
                    }

                    outpud += variable_work + "," + parameter + "\n";
                }

                ///
                /// IF [Statment] THEN [Function]
                /// 
                /// llamar a una funcion si la condicion es verdadera
                ///
                else if (
                    instructions_divided[0] == "IF" && instructions_divided[2] == "THEN"
                    )
                {
                    ///
                    /// la condicion
                    ///
                    string condition = instructions_divided[1].Replace("\\s", " ");
                    string if_true_jump_to = instructions_divided[3].Replace("\\s", " ");

                    string[] syntax_divided = condition.Split("|");

                    ///
                    /// comparacion de buffers
                    ///
                    if (condition.StartsWith("BufferCmp<") && condition.EndsWith(">"))
                    {
                        string[] _params = condition.Substring(10, condition.Length - 11).Split(",");

                        ///
                        /// añadir la instruccion al archivo final
                        ///
                        outpud += "set_value_with_value 1," + Context.symbols[_params[0]] + "\n" + "set_value_with_value 2," + Context.symbols[_params[1]] + "\nsystem_call 24,1\n";
                    }

                    ///
                    /// comparacion de buffers avanzada
                    ///
                    else if (condition.StartsWith("BufferNCmp<") && condition.EndsWith(">"))
                    {
                        string[] _params = condition.Substring(11, condition.Length - 12).Split(",");

                        ///
                        /// añadir la instruccion al archivo final
                        ///
                        outpud += "set_value_with_value 1," + Context.symbols[_params[0]] + "\n" + "set_value_with_value 2," + Context.symbols[_params[1]] + "\nmove 3," + _params[2] + "\nsystem_call 33,1\n";
                    }

                    ///
                    /// si es igual
                    ///
                    else if (
                        syntax_divided[1] == "=="
                        )
                    {
                        ///
                        /// añadir la instruccion al archivo final
                        ///
                        outpud = "set_value_with_value 3," + syntax_divided[0] + "\nset_value_with_value 4," + syntax_divided[2] + "\nsystem_call 25,3\n";
                    }

                    ///
                    /// si no es igual
                    ///
                    else if (
                        syntax_divided[1] == "!="
                        )
                    {
                        ///
                        /// añadir la instruccion al archivo final
                        ///
                        outpud = "set_value_with_value 3," + syntax_divided[0] + "\nset_value_with_value 4," + syntax_divided[2] + "\nsystem_call 26,3\n";
                    }

                    ///
                    /// añadir la instruccion al archivo final
                    ///
                    outpud += "if_equal " + if_true_jump_to + "\n";
                }

                ///
                /// ManualAllocation [BufferPtr] [Size]
                /// 
                /// cualquier buffer no asignado no que no se preocupe ya le tocara
                ///
                else if (
                    instructions_divided[0] == "ManualAllocation" &&
                    instructions_divided.Length == 3
                    )
                {
                    ///
                    /// añadir documentacion
                    ///
                    outpud += "; asignation of a pool*\n";

                    ///
                    /// añadir la instruccion al archivo final
                    ///
                    outpud += "move " + instructions_divided[1] + ",0\nmove " + instructions_divided[1] + "+1," + instructions_divided[2] + "\nsystem_call 14," + instructions_divided[1] + "\n";
                }

                ///
                /// [BUFFER/POOL] [Name] [AllocationFunction]
                /// 
                /// crear un buffer
                ///
                else if (
                    instructions_divided[0] == "BUFFER" || instructions_divided[0] == "POOL"
                    )
                {
                    ///
                    /// el nombre de la referencia
                    ///
                    string ReferenceName = instructions_divided[1];

                    ///
                    /// metodo
                    ///
                    string action = instructions_divided[2];

                    ///
                    /// localizar un registro
                    ///
                    string register_located = HightLevelSearchSpace(Context);

                    ///
                    /// añadirlo al array
                    ///
                    NoFreededPools[ReferenceName] = register_located;

                    ///
                    /// añadir la referencia del buffer
                    ///
                    Context.symbols[ReferenceName] = register_located;

                    ///
                    /// añadir la instruccion al archivo final
                    ///
                    outpud += ReferenceName + " = " + register_located + "\n";

                    ///
                    /// pidiendo un input
                    ///
                    if (
                        action == "ReadLine"
                        )
                    {
                        ///
                        /// añadir la instruccion al archivo final
                        ///
                        outpud += "system_call 23," + register_located + "\n";
                    }

                    ///
                    /// la manera tradicional
                    ///
                    else if (
                        action.StartsWith("AllocatePool->")
                        )
                    {
                        ///
                        /// el tamaño
                        ///
                        string SizeOfPool = HightLevelSyntax(action.Substring(14, action.Length - 14));

                        ///
                        /// añadir la instruccion al archivo final
                        ///
                        outpud += "move " + register_located + ",0\nmove " + (int.Parse(register_located) + 1).ToString() + "," + SizeOfPool + "\nsystem_call 14," + register_located + "\n";
                    }
                }
            }

            /// Console.WriteLine(outpud);

            foreach (var item in NoFreededPools)
            {
                Warnning("The pool " + item.Key + " have not a FreePool instruction");
            }

            //Console.WriteLine(outpud);

            return outpud;
        }

        /**
         * Instructions
         * 
         * las instrucciones disponibles
         */
        public Dictionary<string, LexerInstruction> Instructions =
            new Dictionary<string, LexerInstruction>();

        /**
         * ParseInstruction
         * 
         * convertir una instruccion a su opcode
         */
        public char
        ParseInstruction
        (
        string instruction
        )
        {
            /// 
            /// si existe
            ///
            if (
            Instructions.ContainsKey(instruction)
            )
                ///
                /// convertirla
                ///
                return Instructions[instruction].opcode;

            ///
            /// si no retornar que no existe
            ///
            return '?';
        }

        /**
         * AsmFriendly
         * 
         * convertir registros a su equivalente en ErickAssembly
         */
        public string
            AsmFriendly
            (
                      string the_instruction
        )
        {
            string result = the_instruction;

            result = result.Trim();

            result = result.Replace("sp", "10");
            result = result.Replace("stack_address", "10");

            result = result.Replace("cx", "11");
            result = result.Replace("cl", "12");
            result = result.Replace("ch", "13");

            result = result.Replace("eax", "36");
            result = result.Replace("ax", "32");
            result = result.Replace("al", "33");
            result = result.Replace("ah", "34");

            result = result.Replace("ebx", "37");
            result = result.Replace("bx", "23");
            result = result.Replace("bl", "24");
            result = result.Replace("bh", "25");
            result = result.Replace("di", "55");

            result = result.Replace("si", "35");

            result = result.Replace("pl", "433");
            result = result.Replace("pc", "434");
            result = result.Replace("pm", "435");

            result = result.Replace("macro", "900");
            result = result.Replace("op1", "901");
            result = result.Replace("op2", "902");

            return result;
        }

        /**
         * DivideInstructions
         * 
         * dividir las instrucciones en partes
         *
         * @param instructions son las instrucciones
         */
        public string[]
            DivideInstructions(
            string instructions
            )
        {
            ///
            /// retornar la lista
            ///
            return instructions.Replace("\r", " ").Replace("\t", "").Replace("\n", " ").Split(" ");
        }

        /**
         * AsmSyntax
         * 
         * la sintaxis
         */
        public string
            AsmSyntax
            (
            string statment
            )
        {

            ///
            /// insertar un espacio
            ///
            if (
                statment == "'\\s'"
                )
                return Convert.ToChar((int)' ' + (int)safetynow_for_up).ToString();

            ///
            /// insertar una coma
            ///
            else if (
                statment == "'\\c'"
                )
                return Convert.ToChar((int)',' + (int)safetynow_for_up).ToString();

            ///
            /// insertar un salto de linea
            ///
            else if (
                statment == "'\\n'"
                )
                return Convert.ToChar((int)'\n' + (int)safetynow_for_up).ToString();

            ///
            /// numeros hexadecimales
            ///
            else if (
                statment.ToLower().StartsWith("0x")
                )
            {
                ///
                /// separarlos
                ///
                string[] am = statment.ToLower().Split("x");

                ///
                /// el numero
                ///
                string e = am[1];

                ///
                /// parsearlo
                ///
                if (e.Contains(":")) e = e.Split(":")[0];

                ///
                /// convertirlo a numero
                ///
                try
                {
                    return int.Parse(e, System.Globalization.NumberStyles.HexNumber).ToString();
                }
                catch (Exception ex) { return Convert.ToChar('0' + (int)safetynow_for_up).ToString(); }
            }

            ///
            /// si hay un caracter
            ///
            else if (
                statment.Length == 3 ? (statment[0] == '\'' && statment[2] == '\'') : false
                )
            {
                ///
                /// insertar el caracter
                ///
                return Convert.ToChar((int)statment[1] + (int)safetynow_for_up).ToString();
            }

            ///
            /// sumar dos sintaxis
            ///
            else if (
                statment.Contains("+")
                )
            {
                ///
                /// intentarlo
                ///
                if (
                    int.TryParse(statment.Split("+")[0], out int sum_val1) == true
                    && int.TryParse(statment.Split("+")[1], out int sum_val2) == true
                    )
                {
                    ///
                    /// sumarlos
                    ///
                    int Result = sum_val1 + sum_val2;

                    ///
                    /// convertirlos
                    ///
                    return Convert.ToChar(Result + (int)safetynow_for_up).ToString();
                }
            }

            ///
            /// convertir numeros
            ///
            else if (
                int.TryParse(statment, out int aa) == true
                )
                ///
                /// retornarlo
                ///
                return Convert.ToChar(int.Parse(statment) + (int)safetynow_for_up).ToString();

            ///
            /// si no retornar 0
            ///
            return Convert.ToChar((int)0 + (int)safetynow_for_up).ToString();
        }

        /**
         * ParseCM
         * 
         * convertir una instruccion a su codigo
        */
        public string
            ParseCM
            (
                LexerInstruction ins,
                string[] parms
            )
        {
            ///
            /// si necesita parametros
            ///
            if (
                ins.needs_params
                )
            {
                ///
                /// convertir la instruccion
                ///
                string outp = ins.opcode.ToString();

                ///
                /// ir por los parametros
                ///
                foreach (var item in parms)
                {
                    outp += AsmSyntax(AsmFriendly(item));
                }

                ///
                /// retornar el outpud
                ///
                return outp;
            }

            ///
            /// retornar solo el opcode
            ///
            return ins.opcode.ToString();
        }

        /**
         * ConvertCode
         * 
         * convertir el codigo
         */
        public string
            ConvertCode
            (
            string instructiona
            )
        {
            ///
            /// las instrucciones
            ///
            string instruction = instructiona;

            ///
            /// el outpud
            ///
            string outpud = instruction[0].ToString() + instruction[1].ToString() + ((char)3).ToString();

            ///
            /// todo dividido
            ///
            string[] instructions = DivideInstructions(instruction);

            ///
            /// buscar macros
            ///
            for (int i = 0; i < instructions.Length; i++)
            {
                ///
                /// si es una macro de asignacion
                ///
                if (
                    instructions[i] == "="
                    )
                {
                    ///
                    /// renplazarla
                    ///
                    instruction = instruction.Replace(instructions[i - 1], instructions[i + 1]);
                }

                ///
                /// si es una macro normal
                ///
                else if (
                    instructions[i] == "macro"
                    )
                {
                    ///
                    /// remplazarla tambien, por que no?
                    ///
                    instruction = instruction.Replace(instructions[i + 1] + " ", instructions[i + 2].Replace("\\s", " "));
                }
            }

            ///
            /// dividir las nuevas instrucciones
            ///
            instructions = DivideInstructions(instruction);

            ///
            /// si esta en comentario
            ///
            bool incoment = false;

            ///
            /// parsearlas
            ///
            for (int i = 0; i < instructions.Length; i++)
            {
                ///
                /// dividir los parametros
                ///
                string[] ps = (i + 1) < instructions.Length ? (instructions[i + 1].Split(",")) : "".Split(",");

                ///
                /// si esta en un comentario
                ///
                if (
                    instructions[i] == ";"
                    )
                {
                    incoment = true;
                    continue;
                }

                ///
                /// cerrar comentario
                ///
                if (
                    instructions[i].EndsWith("*") && incoment
                    )
                {
                    incoment = false;
                    continue;
                }

                ///
                /// convertir la instruccion a su opcode
                ///
                char conv = ParseInstruction(instructions[i]);

                ///
                /// si se convirtio
                ///
                if (
                    conv != '?'
                    )
                {
                    ///
                    /// parsearla
                    ///
                    LexerInstruction ins = Instructions[instructions[i]];

                    ///
                    /// si necesita parametros, saltarlos
                    ///
                    if (ins.needs_params)
                        i++;

                    ///
                    /// sumarle al outpud la funcion
                    ///
                    outpud += ParseCM(ins, ps);
                }
            }

            return outpud;
        }

        /**
         * DecompileCode
         * 
         * descompilar el codigo
         */
        public string
            DecompileCode
            (
            string code
            )
        {
            ///
            /// el outpud
            ///
            string outpud = code[0].ToString() + code[1].ToString() + "\n";

            ///
            /// recorrer el array
            ///
            for (int i = 3; i < code.Length; i++)
            {
                ///
                /// parsear una instruccion por instruccion
                ///
                foreach (var item in Instructions)
                {
                    ///
                    /// si encontro el opcode
                    ///
                    if (
                        item.Value.opcode == code[i]
                        )
                    {
                        ///
                        /// la instruccion convertida
                        ///
                        string instruction_conv = item.Key + " ";

                        ///
                        /// parsear los parametros
                        ///
                        for (global::System.Int32 j = 0; j < item.Value.params_count; j++)
                        {
                            ///
                            /// añadir el parametro
                            ///
                            instruction_conv += ((int)code[i + 1 + j] - (int)safetynow_for_up).ToString();

                            ///
                            /// juntar la coma
                            ///
                            instruction_conv += ",";
                        }

                        ///
                        /// agragar la instruccion al outpud
                        ///
                        outpud += instruction_conv.Substring(0, instruction_conv.Length - 1) + "\n";
                        break;

                    }
                }
            }

            ///
            /// retornar el outpud
            ///
            return outpud;
        }

        public
            ErickAssambler()
        {
            this.Instructions["move"] = new(mov_instruction, 2);
            this.Instructions["set_value_with_value"] = new(lea_instruction, 2);
            this.Instructions["add"] = new(add_instruction, 2);
            this.Instructions["rest"] = new(sub_instruction, 2);
            this.Instructions["divide"] = new(div_instruction, 2);
            this.Instructions["multiplique"] = new(imul_instruction, 2);
            this.Instructions["increment"] = new(incr_instruction, 1);
            this.Instructions["_WorkWithPointers"] = new(nop_instruction, 0);
            this.Instructions["slft"] = new(shiftbitsleft_instruction, 2);
            this.Instructions["srght"] = new(shiftbitsright_instruction, 2);
            this.Instructions["decrement"] = new(decr_instruction, 1);
            this.Instructions["call_function"] = new(jump_instruction, 1);
            this.Instructions["AAAAAAAAAAAAHHHHHHHH"] = new(invalid_instruction, 0);
            this.Instructions["if_equal"] = new(jq_instruction, 1);
            this.Instructions["if_not_equal"] = new(jnq_instruction, 1);
            this.Instructions["ret"] = new(ret_instruction, 0);
            this.Instructions["system_call"] = new(interruption_instruction, 2);
            this.Instructions["set_value_with_value_of_value"] = new(ptr_instruction, 2);
            this.Instructions["loop"] = new(loop_instruction, 1);
            this.Instructions["function"] = new(section_instruction, 1);
        }

    }
}
