/**
 * Program.cs
 * 
 * el programa para el worckspace
 */

namespace ErickCompiler
{
    /**
     * Program
     * 
     * el programa
     */
    class Program
    {
        /**
         * Compilador
         * 
         * un compilador para los worckspaces?
         */
        static ErickCompiler Compilador = new();

        /**
         * Main
         * 
         * console
         */
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Compilador.CurrWorckSpace.OpenAsFolder(Environment.CurrentDirectory);

            if (
                args.Length != 0
                )
            {
                Compilador.CurrWorckSpace.Build(args[0]);
            }

            while (true)
            {
                Console.WriteLine("ErickCompiler> ");

                string command = Console.ReadLine();

                if (
                    command.StartsWith("cw ")
                    )
                {
                    Compilador.CurrWorckSpace.OpenAsFolder(command.Substring(3));
                }
                else if (
                    command.StartsWith("bws ")
                    )
                {
                    Compilador.CurrWorckSpace.Build(command.Substring(4));
                }
                else if (
                    command == "bws"
                    )
                {
                    Compilador.CurrWorckSpace.Build("");
                }
            }
        }
    }
}