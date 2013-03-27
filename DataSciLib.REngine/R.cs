// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using RDotNet;

namespace DataSciLib.REngine
{
    /// <summary>
    /// 
    /// </summary>
    public class R : IDisposable
    {
        #region Thread-safe singleton constructor
        private static readonly R instance = new R();
        
        // get a lock on REngine for multiple thread access
        private RDotNet.REngine engine;
        private R() 
        {
            _isrunning = false;
        }

        public static R Instance
        {
            get { return instance; }
        }

        private bool disposed = false;

        #endregion

        #region Properties

        // Set in App.Config file
        public static string InstanceID { get; private set; }
        
        internal static string R_HOME
        {
            get
            {
                return System.Environment.GetEnvironmentVariable("R_HOME");
            }
            private set
            {
                R_HOME = value;
            }
        }
        
        private bool _isrunning;
        public bool IsRunning 
        {
            get 
            {
                if (engine != null)
                {
                    _isrunning = engine.IsRunning;
                    return _isrunning;
                }
                else
                    return false;
            }
        }

        private string _wd = string.Empty;
        private bool _dirChanged = true;

        public RWorkspace Workspace 
        {
            get { return RWorkspace.Workspace; } 
        }

        #endregion

        #region Engine Instance Management
        /// <summary>
        /// Implements Dispose() method of IDisposable interface
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose all managed resources here
                }

                // Dispose all unmanaged resources here.
                engine.Close();

                // Disposing done
                disposed = true;
            }
        }

        /// <summary>
        /// Default R environment setup
        /// </summary>
        internal void SetupEnvironment()
        {
            //TODO: Set in App.config file
            if (string.IsNullOrEmpty(R_HOME))
                R_HOME = @"C:\Program Files\R\R-2.15.1";

            var envPath = Environment.GetEnvironmentVariable("PATH");
            string rBinPath;
            if (R_HOME.EndsWith("\\"))
            {
                rBinPath = R_HOME + @"bin\x64";
            }
            else
            {
                rBinPath = R_HOME + @"\bin\x64";
            }

            Environment.SetEnvironmentVariable("R_HOME", R_HOME);
            Environment.SetEnvironmentVariable("PATH", envPath + Path.PathSeparator + rBinPath);

        }

        public string GetWorkingDirectory()
        {
            if (IsRunning & _dirChanged)
            {
                if (engine != null)
                {
                    _wd = engine.Evaluate("getwd()").AsCharacter().First();
                    _dirChanged = false;
                    return _wd;
                }
                else
                    throw new Exception("Engine is null..");
            }
            else if (!_dirChanged)
                return _wd;
            else
                return string.Empty;
        }

        public bool SetWorkingDirectory(string directory)
        {
            if (directory != this.GetWorkingDirectory())
            {
                try
                {
                    engine.Evaluate("setwd(\"" + directory + "\")");
                    _dirChanged = true;
                    return true;
                }
                catch (Exception e)
                {
                    throw new ApplicationException(e.Message, e);
                }
            }
            else
            {
                _dirChanged = false;
                return true;
            }
        }

        public void Start(params REngineOptions[] list)
        {
            try
            {
                // Only one instance per process
                if (!IsRunning)
                {
                    SetupEnvironment();

                    InstanceID = "RDotNet";
                    // 
                    engine = RDotNet.REngine.CreateInstance(InstanceID);
                    engine.Initialize();

                    // Load compiler package on startup - may speed up code if long running
                    LoadPackage("compiler");
                }
                else
                    engine = RDotNet.REngine.GetInstanceFromID(InstanceID);
            }
            catch (SEHException e)
            {
                throw new  ApplicationException(e.Message, e);
            }
            if (engine == null)
                throw new ApplicationException("Could not start R");
        }

        #endregion

        #region Constants
        const string assignleft = "<-";
        const string assignright = "->";

        #endregion

        #region Basic R commands and interaction with R environment

        public void RunScript(string script)
        {

        }

        /// <summary>
        /// Runs a R statement
        /// </summary>
        /// <param name="script"></param>
        /// <returns>SymbolicExpression object that can be further processd to extract relevant data, Protected from Garbage collection</returns>
        public SymbolicExpression RunCommand(string expression)
        {
            try
            {
                if (expression.Contains(assignleft) || expression.Contains(assignright))
                    throw new ApplicationException("Use the appropriate method for assigning expressions to variables...");
                else
                {
                    var expr = engine.Evaluate(expression);
                    if (expr == null)
                        throw new NullReferenceException("Evaluation of R command returned null");
                    //expr.Protect();
                    return expr;
                }
            }
            catch (SEHException e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        /// <summary>
        /// Assigns the result of an expression evaluation to a variable in the R Global Environment
        /// </summary>
        /// <param name="variable">Name of the assigned variable</param>
        /// <param name="expression">Expression to evaluate</param>
        /// <returns>SymbolicExpression, unprotected from the garbage collector</returns>
        public SymbolicExpression RunCommand(string variable, string expression)
        {
            try
            {
                var expr = engine.Evaluate(string.Concat(variable, assignleft, expression));
                if (expr == null)
                    throw new NullReferenceException("Evaluation of R command returned null");
                //expr.Protect();
                return expr;
            }
            catch (SEHException e)
            {
                throw new ApplicationException(e.Message);
            }
            finally
            {
                ClearMemory();
            }
        }        

        /// <summary>
        /// Calls the generic print method on the expression and outputs the result to the console
        /// </summary>
        /// <param name="expres">SymbolicExpression to evaluate and print</param>
        public void Print(SymbolicExpression expres)
        {
            SetSymbol("test", expres);
            engine.Evaluate("print(test)");
            engine.Evaluate("rm(test)");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbolname"></param>
        /// <returns></returns>
        public SymbolicExpression GetSymbol(string symbolname)
        {
            // Exception handling ?
            return (engine==null) ?  null : engine.GetSymbol(symbolname);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbolname"></param>
        /// <param name="expression"></param>
        internal void SetSymbol(string symbolname, SymbolicExpression expression)
        {
           /* if (!expression.IsProtected)
                expression.Protect();
            */
            engine.SetSymbol(symbolname, expression);
        }

        /// <summary>
        /// Get a R function
        /// </summary>
        /// <param name="funcname">Name of the R function</param>
        /// <returns>Function, protected from garbage collector</returns>
        public Function GetFunction(string funcname)
        {
            try
            {
                var func = engine.GetSymbol(funcname);
                if (func == null)
                    throw new NullReferenceException("Could not find function " + funcname + " R Engine returned null");
                
                func.Protect();
                return func.AsFunction();
            }
            catch (SEHException e)
            {
                throw new ApplicationException(e.Message, e);
            }
            finally
            {
                ClearMemory();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="package"></param>
        public bool LoadPackage(string package)
        {
            if (engine != null)
            {
                var expr = engine.Evaluate("library(\"" + package + "\")");
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Source a R script in the R environment
        /// </summary>
        /// <param name="filename">Filenmae of R script, .R file extension not necessary</param>
        public void Source(string filename)
        {
            if (!filename.EndsWith(".R"))
                filename = filename + ".R";

            string path;
            if (this.GetWorkingDirectory().EndsWith("/"))
                path = this.GetWorkingDirectory() + filename;
            else
                path = this.GetWorkingDirectory() + "/" + filename;

            string parm = "source(\"" + path + "\")";
            try
            {
                if (engine != null)
                {
                    engine.Evaluate(parm);
                }
            }
            catch (SEHException e)
            {
                throw new ApplicationException(path + " doesn't exist.",e);
            }
            finally
            {
                ClearMemory();
            }
        }

        /// <summary>
        /// Set destination to send output from R interpreter
        ///  - Write output to file
        /// </summary>
        /// <param name="filename">Name of file to write to</param>
        public void Sink(string filename)
        {

        }

        #endregion

        #region R function calls

        /// <summary>
        /// Call Function on a S4 object
        /// </summary>
        /// <param name="funcname">Function name</param>
        /// <param name="s4object">S4 object</param>
        /// <param name="slot">Slot in the S4 object</param>
        /// <returns></returns>
        public SymbolicExpression CallFunction(string funcname, SymbolicExpression s4object, string slot)
        {
            StringBuilder sb = new StringBuilder();
            if (s4object.Type == RDotNet.Internals.SymbolicExpressionType.S4)
            {
                try
                {
                    sb.Append(funcname).Append("(");
                    engine.SetSymbol("variable1", s4object);
                    s4object.Unprotect();
                    sb.Append("variable1").Append("@").Append(slot).Append(")");

                    var result = engine.Evaluate(sb.ToString());
                    if (result == null)
                        throw new NullReferenceException("Could not evaluate function " + funcname + "; R Engine returned null");

                    return result;
                }
                catch (SEHException e)
                {
                    throw new ApplicationException("R Engine has thrown an Exception - memory possibly corrupted; Function: " + funcname + " could not be executed.", e);
                }

                // Clean up afterwards
                finally
                {
                    // Clean up afterwards
                    ClearMemory();
                }
            }
            else
                throw new ArgumentException("SymbolicExpression not of type S4");
        }

        /// <summary>
        /// Call function with a variable number of parameters
        /// </summary>
        /// <param name="funcname"></param>
        /// <param name="paramlist"></param>
        /// <returns>Result of function call as a SymbolicExpression</returns>
        public SymbolicExpression CallFunction(string funcname, params SymbolicExpression[] parameters)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(funcname).Append("(");
            int c = 1;
            foreach (var p in parameters)
            {
                engine.SetSymbol("variable" + c, p);
                p.Unprotect();
                sb.Append("variable" + c).Append(", ");
                c++;
            }

            sb.Remove(sb.Length - 2, 2).Append(")");
            try
            {
                var result = engine.Evaluate(sb.ToString());
                if (result == null)
                    throw new NullReferenceException("Could not evaluate function " + funcname + " R Engine returned null");
                return result;
            }
            catch (SEHException e)
            {
                throw new ApplicationException("R Engine has thrown an Exception - memory possibly corrupted; Function " + funcname + " could not be executed.", e);
            }
            catch (ParseException pe)
            {
                throw new ApplicationException("R Engine has thrown an Exception - expression parsing error; Function " + funcname + " could not be executed.", pe);
            }
            finally
            {
                // Clean up afterwards
                ClearMemory();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="funcname"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SymbolicExpression CallFunction(string funcname, params Tuple<string, SymbolicExpression>[] parameters)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(funcname).Append("(");
            foreach (var p in parameters)
            {
                engine.SetSymbol("var" + p.Item1, p.Item2);
                p.Item2.Unprotect();
                sb.Append(p.Item1).Append("=").Append("var").Append(p.Item1).Append(", ");
            }

            sb.Remove(sb.Length - 2, 2).Append(")");

            try
            {
                var result = engine.Evaluate(sb.ToString());
                return result;
            }
            catch (SEHException e)
            {
                throw new ApplicationException("R Engine has thrown an Exception - memory possibly corrupted; Function: " + funcname + " could not be executed.", e);
            }
            finally
            {
                ClearMemory();
            }
        }
        #endregion

        #region R Data Marshalling

        public SymbolicExpression RNull()
        {
            try
            {
                var nullexpr = engine.Evaluate("NULL");
                nullexpr.Protect();
                return nullexpr;
            }
            catch (SEHException e)
            {
                throw new ApplicationException("R Engine has thrown an Exception - memory possibly corrupted; String could not be created in R.", e);
            }
            finally
            {
                ClearMemory();
            }
        }

        public SymbolicExpression R_NA()
        {
            try
            {
                var naexpr = engine.Evaluate("NA");
                if (naexpr == null)
                    throw new NullReferenceException();

                naexpr.Protect();
                return naexpr;
            }
            catch (SEHException e)
            {
                throw new ApplicationException("R Engine has thrown an Exception - memory possibly corrupted; String could not be created in R.", e);
            }
            finally
            {
                ClearMemory();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns>SymbolicExpression, protected from garbage collector</returns>
        public SymbolicExpression RString(string str)
        {
            try
            {
                var strexpr = engine.Evaluate("\"" + str + "\"");
                if (strexpr == null)
                    throw new NullReferenceException("Evaluation of string expression returned null");

                strexpr.Protect();
                return strexpr;
            }
            catch (SEHException e)
            {
                throw new ApplicationException("R Engine has thrown an Exception - memory possibly corrupted; String could not be created in R.", e);
            }
            finally
            {
                ClearMemory();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boolean"></param>
        /// <returns>SymbolicExpression, protected from garbage collector</returns>
        public SymbolicExpression RBool(bool boolean)
        {
            try
            {
                if (boolean)
                {
                    var boolexpr = engine.Evaluate("TRUE");
                    boolexpr.Protect();
                    return boolexpr;
                }
                else
                {
                    var boolexpr = engine.Evaluate("FALSE");
                    boolexpr.Protect();
                    return boolexpr;
                }
            }
            catch (SEHException e)
            {
                throw new ApplicationException("R Engine has thrown an Exception - memory possibly corrupted; Bool could not be created in R.", e);
            }
            finally
            {
                ClearMemory();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public NumericVector RNumeric(double data)
        {
            try
            {
                NumericVector vector = engine.CreateNumericVector(new[] { data });
                vector.Protect();
                return vector;
            }
            catch (SEHException e)
            {
                throw new ApplicationException("R Engine has thrown an Exception - memory possibly corrupted; Vector could not be created in R.", e);
            }
            finally
            {
                ClearMemory();
            }
        }

        public SymbolicExpression RNumeric(double? data)
        {
            if (data != null)
            {
                try
                {
                    NumericVector vector = engine.CreateNumericVector(new[] { (double)data });
                    vector.Protect();
                    return vector;
                }
                catch (SEHException e)
                {
                    throw new ApplicationException("R Engine has thrown an Exception - memory possibly corrupted; Vector could not be created in R.", e);
                }
                finally
                {
                    ClearMemory();
                }
            }
            else
            {
                try
                {
                    var expr = RNull();
                    expr.Protect();
                    return expr;
                }
                catch (SEHException e)
                {
                    throw new ApplicationException("R Engine has thrown an Exception - memory possibly corrupted; Vector could not be created in R.", e);
                }
                finally
                {
                    ClearMemory();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns>NumericVector, protected from garbage collector</returns>
        public NumericVector RVector(double[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            try
            {
                NumericVector vector = engine.CreateNumericVector(data);
                vector.Protect();
                return vector;
            }
            catch (SEHException e)
            {
                throw new ApplicationException("R Engine has thrown an Exception - memory possibly corrupted; Vector could not be created in R.", e);
            }
            finally
            {
                ClearMemory();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public CharacterVector RVector(string[] text)
        {
            try
            {
                CharacterVector vector = engine.CreateCharacterVector(text);
                vector.Protect();
                return vector;
            }
            catch (SEHException e)
            {
                throw new ApplicationException("R Engine has thrown an Exception - memory possibly corrupted; Vector could not be created in R.", e);
            }
            finally
            {
                ClearMemory();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public CharacterVector RVector(List<string> text)
        {
            try
            {
                CharacterVector vector = engine.CreateCharacterVector(text.ToArray());
                vector.Protect();
                return vector;
            }
            catch (SEHException e)
            {
                throw new ApplicationException("R Engine has thrown an Exception - memory possibly corrupted; Vector could not be created in R.", e);
            }
            finally
            {
                ClearMemory();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public NumericMatrix RMatrix(double[,] data)
        {
            try
            {
                NumericMatrix matrix = engine.CreateNumericMatrix(data);
                if (matrix==null)
                    throw new NullReferenceException("Evaluation of matrix expression returned null");

                matrix.Protect();
                return matrix;
            }
            catch (SEHException e)
            {
                throw new ApplicationException("R Engine has thrown an Exception - memory possibly corrupted; Matrix could not be created in R.", e);
            }
            finally
            {
                ClearMemory();
            }
        }

        /// <summary>
        /// Create an R list by passing in name-value pairs
        /// </summary>
        /// <param name="list">name-value pairs, name of string and value of SymbolicExpression</param>
        /// <returns></returns>
        public SymbolicExpression RList(params Tuple<string, SymbolicExpression>[] list)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("list(");
                foreach (var p in list)
                {
                    if (p.Item2 == null)
                        engine.SetSymbol("var" + p.Item1, R_NA());
                    else
                        engine.SetSymbol("var" + p.Item1, p.Item2);

                    sb.Append(p.Item1).Append("=").Append("var").Append(p.Item1).Append(",");
                }

                if (sb.ToString().EndsWith(","))
                    sb.Remove(sb.Length - 1, 1);

                sb.Append(")");

                SymbolicExpression _list = engine.Evaluate(sb.ToString());
                if (_list == null)
                    throw new NullReferenceException("Evaluation of List expression returned null");

                _list.Protect();
                return _list;
            }
            catch (SEHException e)
            {
                throw new ApplicationException("R Engine has thrown an Exception - memory possibly corrupted; List could not be created in R.", e);
            }
            finally
            {
                ClearMemory();
            }
        }
        
        #endregion

        #region R Memory Management

        public void ClearMemory()
        {
            try
            {
                engine.Evaluate("rm(list=ls())");
            }
            catch (SEHException e)
            {
                Console.WriteLine("SEHException thrown");
            }
            catch (ParseException e)
            {
                Console.WriteLine("ParseException thrown");
            }
        }

        #endregion
    }
}
