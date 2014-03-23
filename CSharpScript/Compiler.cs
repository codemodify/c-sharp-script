using System;
using System.IO;
using System.Text;
using Microsoft.CSharp;
using System.Reflection;
using System.Collections.Generic;

namespace CSharpScript
{
    public class NotificationDetails : EventArgs
    {
        public String FileName;
        public int Row;
        public int Collumn;
        public String Message;


        public override string ToString()
        {
            return String.Format( "File: {0}, Row: {1}, Collumn: {2}, Message: {3}", FileName, Row, Collumn, Message );
        }
    }

    public class Compiler
    {
        public event EventHandler<NotificationDetails> OnErrorOccured = delegate { };
        public event EventHandler<NotificationDetails> OnWarningOccured = delegate { };

        #region CompileScriptFiles()

        public Assembly CompileScriptFiles( String[] fullScriptFilePaths )
        {
            List<String> cSharpFiles = new List<String>();

            #region Save as temp CS files

            foreach( String file in fullScriptFilePaths )
            {
                StreamWriter
                    sw = new StreamWriter( file + ".cs" );
                    sw.Write( TransformScriptFileToCSharp( file ) );
                    sw.Close();

                cSharpFiles.Add( file + ".cs" );
            }

            #endregion

            Assembly assembly = null;

            CSharpCodeProvider
                codeProvider = new CSharpCodeProvider();

            System.CodeDom.Compiler.CompilerParameters 
                compilerParameters = new System.CodeDom.Compiler.CompilerParameters();
                compilerParameters.GenerateExecutable = false;
                compilerParameters.GenerateInMemory = false;

            System.CodeDom.Compiler.CompilerResults
                compilerResults = codeProvider.CompileAssemblyFromFile( compilerParameters, cSharpFiles.ToArray() );

            foreach( System.CodeDom.Compiler.CompilerError error in compilerResults.Errors )
            {
                NotificationDetails 
                    details = new NotificationDetails();
                    details.FileName = error.FileName;
                    details.Row = error.Column;
                    details.Collumn = error.Line;
                    details.Message = error.ErrorText;

                if( error.IsWarning )
                    OnWarningOccured( this, details );
                else
                    OnErrorOccured( this, details );
            }

            return assembly;
        }

        #endregion

        #region TransformScriptFileToCSharp()

        public String TransformScriptFileToCSharp( String fullScriptFilePath )
        {
            StreamReader sr = new StreamReader( fullScriptFilePath );

            return TransformScriptToCSharp( sr.ReadToEnd() );
        }

        #endregion

        #region TransformScriptToCSharp

        internal String TransformScriptToCSharp( String scriptFileContent )
        {
            StringBuilder csharpFileContent = new StringBuilder();

            #region local helpers

            bool methodMet      = false;
            bool namespaceMet   = false;
            bool propOrMethMet  = false;
            String className    = String.Empty;
            int startIndex      = 0;
            String spaces4      = "    ";
            String spaces6      = "      ";
            String spaces10     = "          ";

            #endregion

            String[] scriptLines = scriptFileContent.Split( new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries );

            foreach( String scriptLine in scriptLines )
            {
                StringBuilder csharpLine = new StringBuilder( scriptLine );

                #region Convert Script to C# syntax

                #region using

                startIndex = scriptLine.IndexOf( "using" );
                if( -1 != startIndex )
                {
                    csharpLine.Append( ";" );
                    goto endLogic;
                }

                #endregion

                #region namespace start

                startIndex = scriptLine.IndexOf( "namespace" );
                if( -1 != startIndex )
                {
                    namespaceMet = true;
                    csharpLine.Insert( 0, "\r\n\r\n" );
                    csharpLine.Append( "{" );
                    goto endLogic;
                }

                #endregion
                {
                    #region class start

                    startIndex = scriptLine.IndexOf( "class" );
                    if( -1 != startIndex )
                    {
                        String[] data = scriptLine.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
                        className = data[ 1 ];

                        csharpLine.Insert( 0, "\r\n" +spaces4+ "public " );
                        csharpLine.Append( "\r\n" +spaces4+ "{" );
                        goto endLogic;
                    }

                    #endregion
                    {
                        if( scriptLine.StartsWith( spaces6 + "//" ) )
                            continue;

                        #region properties / methods declaration

                        if
                        (
                            scriptLine.StartsWith( spaces6 )
                            &&
                            !scriptLine.StartsWith( spaces10 )
                        )
                        {
                            startIndex = scriptLine.IndexOf( ")" );

                            bool haveToMarkEndOfPreviousMethod = false;
                            if( methodMet )
                            {
                                methodMet = false;
                                haveToMarkEndOfPreviousMethod = true;
                            }

                            #region properties

                            if( -1 == startIndex )
                            {
                                csharpLine.Replace( spaces6, spaces4 + spaces4 + "public " );
                                csharpLine.Append( "{ get; set; }" );
                                propOrMethMet = true;
                            }

                            #endregion

                            #region methods

                            else
                            {
                                bool constructorOrDescructorMet = false;
                                methodMet = true;

                                #region Check for contrcutor and destructor

                                String[] data = scriptLine.Split( new char[] { ' ', '(' }, StringSplitOptions.RemoveEmptyEntries );
                                String methodName = data[ 0 ];

                                if( "Init".Equals( methodName ) )
                                {
                                    constructorOrDescructorMet = true;
                                    csharpLine.Replace( methodName, className );
                                }
                                else if( "Destroy".Equals( methodName ) )
                                {
                                    constructorOrDescructorMet = true;
                                    csharpLine.Replace( methodName, "~" + className );
                                }

                                #endregion

                                csharpLine.Replace
                                (
                                    spaces6,
                                    ( propOrMethMet ? "\r\n" : String.Empty ) 
                                    + spaces4 + spaces4 +
                                    ( constructorOrDescructorMet ? String.Empty : "public virtual void ")
                                );
                                csharpLine.Append( "\r\n" +spaces4 + spaces4+ "{" );

                                propOrMethMet = true;
                            }

                            if( haveToMarkEndOfPreviousMethod )
                            {
                                csharpLine.Insert( 0, spaces4 + spaces4 + "}\r\n" );
                            }

                            #endregion
                        }

                        #endregion

                        #region properties / methods implementation

                        else if
                        (
                            scriptLine.StartsWith( spaces10 ) 
                        )
                        {
                            csharpLine.Replace( spaces6, spaces4 + spaces4 );
                            csharpLine.Append( ";" );
                        }

                        #endregion
                    }
                    #region class end
                    #endregion
                }
                #region namespace end
                #endregion

                #endregion

                endLogic:
                csharpFileContent.AppendLine( csharpLine.ToString() );
            }

            if( !namespaceMet )
                csharpFileContent.Replace( spaces4 + "public class", "namespace AppNamespace\r\n{\r\n" +spaces4+ "public class" );

            csharpFileContent.Append( spaces4 + spaces4 + "}" ); // last method
            csharpFileContent.Append( "\r\n" +spaces4+ "}" ); // class
            csharpFileContent.Append( "\r\n}\r\n" ); // namesapce

            return csharpFileContent.ToString();
        }

        #endregion
    }
}
