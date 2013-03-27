// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDotNet;

namespace DataSciLib.REngine
{
    public sealed class RWorkspace
    {
        #region Thread-safe singleton
        private static readonly RWorkspace workspace = new RWorkspace();

        private RWorkspace()
        {
            _vars = new List<string>();
            DataSet = new List<string>();
            Engine = R.Instance;
        }

        internal static RWorkspace Workspace
        {
            get { return workspace; }
        }

        #endregion

        #region Properties

        public R Engine { get; private set; }

        private List<string> _vars;
        internal List<string> Variables 
        {
            get
            {
                return _vars;
            }
        }

        internal List<string> DataSet { get; private set; }
        public string DataSetName { get; set; }

        #endregion
        
        /// <summary>
        /// Load a previously saved workspace into memory
        ///  - Loads a .RData file in current directory.
        /// </summary>
        public void LoadWorkspace()
        {
            if (Engine.IsRunning)
                Engine.RunCommand("load(\"" + Engine.GetWorkingDirectory() + "/.RData\")");
            else
                throw new ApplicationException();
        }

        /// <summary>
        /// Save curent workspace to current working directory
        /// </summary>
        public void SaveWorkspace()
        {
            throw new NotImplementedException();
        }

        internal double[] GetVector(string varname)
        {
            double[] vector = { 0 };

            return vector;
        }

        internal double[,] GetMatrix(string varname)
        {
            double[,] matrix = { {0} };

            return matrix;
        }

        internal SymbolicExpression GetObject(string varname)
        {
            var expr = Engine.GetSymbol(varname);
            
            return expr;
        }

        internal SymbolicExpression GetVariable(string varname)
        {
            // Check if variable exists
            var expr = Engine.GetSymbol(varname);

            return expr;
        }

        internal SymbolicExpression GetObjectProperty(string varname, string propertyname)
        {
            var expr = Engine.GetSymbol(varname+"@"+propertyname);
            return expr;
        }

        #region Workspace housekeeping methods
        private void ClearVariable(string varname)
        {
            //TODO: exception handling code
            Engine.RunCommand("rm(" + varname + ")");
        }

        private void ClearVariable(List<string> varnames)
        {
            //TODO: exception handling code
            foreach (string s in varnames)
            {
                this.ClearVariable(s);
            }
        }

        /// <summary>
        /// Clear the entire workspace
        /// </summary>
        internal void Clear()
        {
            this.ClearVariable(_vars);
            this._vars.Clear();
        }

        private void DeleteAllVariables()
        {
            Engine.RunCommand("rm(list=ls())");
        }

        internal void SetVariable(string varname, string expression)
        {
            // Add variable to list of declared variables
            Engine.RunCommand(varname, expression);

            if(!_vars.Contains(varname))
                _vars.Add(varname);
        }

        internal void SetVariable(string varname, SymbolicExpression expression)
        {
            Engine.SetSymbol(varname, expression);
            if (!_vars.Contains(varname))
                _vars.Add(varname);
        }

        internal void SetVariable(string varname, double[] vector)
        {
            var _vect = Engine.RVector(vector);
            Engine.SetSymbol(varname, _vect);

            if (!_vars.Contains(varname))
                _vars.Add(varname);
        }

        private bool ImportData(string datasource, string query)
        {
            throw new NotImplementedException();
        }

        private void SetValue<Tin>(string varname, Tin data)
        {
            //Engine.SetSymbol(this.Name, data);

            // .NET Framework array to R vector.
            var group1 = Engine.RVector(new double[] { 30.02, 29.99, 30.11, 29.97, 30.01, 29.99 });
            Engine.SetSymbol("group1", group1);

            // Direct parsing from R script.
            NumericVector group2 = Engine.RunCommand("group2 <- c(29.89, 29.93, 29.72, 29.98, 30.02, 29.98)").AsNumeric();

            // Test difference of mean and get the P-value.
            GenericVector testResult = Workspace.Engine.RunCommand("t.test(group1, group2)").AsList();
            double p = testResult["p.value"].AsNumeric().First();
            throw new NotImplementedException();
        }

        #endregion

        #region Helper methods
        private void getWorkspace()
        {
            // Implement in-memory (.NET) caching by using flags to check when workspace changes

            // R command ls()
            var ws = Engine.RunCommand("ls()");

            // Disect all data and functions loaded into workspace
        }

        private string ToRDate(DateTime date)
        {
            var outdate = "\"" + date.Date.Year + "-" + date.Date.Month + "-" + date.Date.Day + "\"";

            if (date.Month<10)
            {
                outdate = "\"" + date.Date.Year + "-0" + date.Date.Month + "-" + date.Date.Day + "\"";
            }
            if (date.Day < 10)
            {
                outdate = "\"" + date.Date.Year + "-0" + date.Date.Month + "-0" + date.Date.Day + "\"";
            }

            return outdate;
        }

        #endregion
    }
}
