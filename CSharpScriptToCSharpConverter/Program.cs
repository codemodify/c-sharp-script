using System;
using System.IO;
using System.Text;

namespace CSharpScriptToCSharpConverter
{
    class Program
    {
        static void Main( string[] args )
        {
            CompileAllFiles();

            Console.ReadLine();
        }

        static void ConvertAllFiles()
        {
            CSharpScript.Compiler cSharpScriptCompiler = new CSharpScript.Compiler();

            foreach( string file in Directory.GetFiles( @"..\..\..\Examples\Example 01 Transportable", "*.chs" ) )
            {
                StreamWriter
                    sw = new StreamWriter( file + ".cs" );
                    sw.Write( cSharpScriptCompiler.TransformScriptFileToCSharp( file ) );
                    sw.Close();
            }
        }

        static void ConvertOneFile()
        {
            CSharpScript.Compiler cSharpScriptCompiler = new CSharpScript.Compiler();

            string file = @"C:\Users\nicu\Projects\C Sharp Script\CSharpScript\Examples\Example 01 Transportable\ITransportable.chs";
            StreamWriter
                sw = new StreamWriter( file + ".cs" );
                sw.Write( cSharpScriptCompiler.TransformScriptFileToCSharp( file ) );
                sw.Close();
        }

        static void CompileAllFiles()
        {
            CSharpScript.Compiler
                cSharpScriptCompiler = new CSharpScript.Compiler();
                cSharpScriptCompiler.OnErrorOccured += new EventHandler<CSharpScript.NotificationDetails>( cSharpScriptCompiler_ErrorOccured );
                cSharpScriptCompiler.OnWarningOccured += new EventHandler<CSharpScript.NotificationDetails>( cSharpScriptCompiler_WarningOccured );
                cSharpScriptCompiler.CompileScriptFiles( Directory.GetFiles( @"..\..\..\Examples\Example 01 Transportable", "*.chs" ) );
        }

        static void cSharpScriptCompiler_WarningOccured( object sender, CSharpScript.NotificationDetails e )
        {
            Console.WriteLine( e.ToString() );
        }

        static void cSharpScriptCompiler_ErrorOccured( object sender, CSharpScript.NotificationDetails e )
        {
            Console.WriteLine( e.ToString() );
        }
    }
}
