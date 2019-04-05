using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Xml;
using SiteServer.CMS.Apis;

namespace SiteServer.CMS.Database.Core
{
	public class SqlServer : DatabaseApi
    {
        //public override IDataParameter[] GetDataParameters(int size)
        //{
        //    return new SqlParameter[size];
        //}


        //public override IDbConnection GetConnection(string connectionString)
        //{
        //    return new SqlConnection(connectionString);
        //}


        

        //public override void DeriveParameters(IDbCommand cmd)
        //{
        //    var mustCloseConnection = false;

        //    if (!(cmd is SqlCommand))
        //        throw new ArgumentException("The command provided is not a SqlCommand instance.", "cmd");

        //    if (cmd.Connection.State != ConnectionState.Open)
        //    {
        //        cmd.Connection.Open();
        //        mustCloseConnection = true;
        //    }

        //    DeriveParameters((SqlCommand)cmd);

        //    if (mustCloseConnection)
        //    {
        //        cmd.Connection.Close();
        //    }
        //}

        
        //public override IDataParameter GetParameter()
        //{
        //    return new SqlParameter();
        //}


        public override void ClearCommand(IDbCommand command)
        {
            // HACK: There is a problem here, the output parameter values are fletched 
            // when the reader is closed, so if the parameters are detached from the command
            // then the IDataReader can磘 set its values. 
            // When this happen, the parameters can磘 be used again in other command.
            var canClear = true;

            foreach (IDataParameter commandParameter in command.Parameters)
            {
                if (commandParameter.Direction != ParameterDirection.Input)
                    canClear = false;

            }
            if (canClear)
            {
                command.Parameters.Clear();
            }
        }

        
        public override void CleanParameterSyntax(IDbCommand command)
        {
            // do nothing for SQL
        }

        
        public override XmlReader ExecuteXmlReader(IDbCommand command)
        {
            var mustCloseConnection = false;

            if (command.Connection.State != ConnectionState.Open)
            {
                command.Connection.Open();
                mustCloseConnection = true;
            }

            CleanParameterSyntax(command);
            // Create the DataAdapter & DataSet
            var retval = ((SqlCommand)command).ExecuteXmlReader();

            // Detach the SqlParameters from the command object, so they can be used again
            // don't do this...screws up output parameters -- cjbreisch
            // cmd.Parameters.Clear();

            if (mustCloseConnection)
            {
                command.Connection.Close();
            }

            return retval;
        }

        

        protected override IDataParameter GetBlobParameter(IDbConnection connection, IDataParameter p)
        {
            // do nothing special for BLOBs...as far as we know now.
            return p;
        }

        internal static void DeriveParameters(SqlCommand cmd)
        {
            string cmdText;
            SqlCommand newCommand;
            SqlDataReader reader;
            ArrayList parameterList;
            SqlParameter sqlParam;
            CommandType cmdType;
            string procedureSchema;
            string procedureName;
            int groupNumber;
            var trnSql = cmd.Transaction;

            cmdType = cmd.CommandType;

            if ((cmdType == CommandType.Text))
            {
                throw new InvalidOperationException();
            }
            else if ((cmdType == CommandType.TableDirect))
            {
                throw new InvalidOperationException();
            }
            else if ((cmdType != CommandType.StoredProcedure))
            {
                throw new InvalidOperationException();
            }

            procedureName = cmd.CommandText;
            string server = null;
            string database = null;
            procedureSchema = null;

            // split out the procedure name to get the server, database, etc.
            GetProcedureTokens(ref procedureName, ref server, ref database, ref procedureSchema);

            // look for group numbers
            groupNumber = ParseGroupNumber(ref procedureName);

            newCommand = null;

            // set up the command string.  We use sp_procuedure_params_rowset to get the parameters
            if (database != null)
            {
                cmdText = string.Concat("[", database, "]..sp_procedure_params_rowset");
                if (server != null)
                {
                    cmdText = string.Concat(server, ".", cmdText);
                }

                // be careful of transactions
                if (trnSql != null)
                {
                    newCommand = new SqlCommand(cmdText, cmd.Connection, trnSql);
                }
                else
                {
                    newCommand = new SqlCommand(cmdText, cmd.Connection);
                }
            }
            else
            {
                // be careful of transactions
                if (trnSql != null)
                {
                    newCommand = new SqlCommand("sp_procedure_params_rowset", cmd.Connection, trnSql);
                }
                else
                {
                    newCommand = new SqlCommand("sp_procedure_params_rowset", cmd.Connection);
                }
            }

            newCommand.CommandType = CommandType.StoredProcedure;
            newCommand.Parameters.Add(new SqlParameter("@procedure_name", SqlDbType.NVarChar, 255));
            newCommand.Parameters[0].Value = procedureName;

            // make sure we specify 
            if (!IsEmptyString(procedureSchema))
            {
                newCommand.Parameters.Add(new SqlParameter("@procedure_schema", SqlDbType.NVarChar, 255));
                newCommand.Parameters[1].Value = procedureSchema;
            }

            // make sure we specify the groupNumber if we were given one
            if (groupNumber != 0)
            {
                newCommand.Parameters.Add(new SqlParameter("@group_number", groupNumber));
            }

            reader = null;
            parameterList = new ArrayList();

            try
            {
                // get a reader full of our params
                reader = newCommand.ExecuteReader();
                sqlParam = null;

                while (reader.Read())
                {
                    // get all the parameter properties that we can get, Name, type, length, direction, precision
                    sqlParam = new SqlParameter();
                    sqlParam.ParameterName = (string)(reader["PARAMETER_NAME"]);
                    sqlParam.SqlDbType = GetSqlDbType((short)(reader["DATA_TYPE"]), (string)(reader["TYPE_NAME"]));

                    if (reader["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value)
                    {
                        sqlParam.Size = (int)(reader["CHARACTER_MAXIMUM_LENGTH"]);
                    }

                    sqlParam.Direction = GetParameterDirection((short)(reader["PARAMETER_TYPE"]));

                    if ((sqlParam.SqlDbType == SqlDbType.Decimal))
                    {
                        sqlParam.Scale = (byte)(((short)(reader["NUMERIC_SCALE"]) & 255));
                        sqlParam.Precision = (byte)(((short)(reader["NUMERIC_PRECISION"]) & 255));
                    }
                    parameterList.Add(sqlParam);
                }
            }
            finally
            {
                // close our reader and connection when we're done
                if (reader != null)
                {
                    reader.Close();
                }
                newCommand.Connection = null;
            }

            // we didn't get any parameters
            if ((parameterList.Count == 0))
            {
                throw new InvalidOperationException();
            }

            cmd.Parameters.Clear();

            // add the parameters to the command object

            foreach (var parameter in parameterList)
            {
                cmd.Parameters.Add(parameter);
            }
        }

        private static int ParseGroupNumber(ref string procedure)
        {
            string newProcName;
            var groupPos = procedure.IndexOf(';');
            var groupIndex = 0;

            if (groupPos > 0)
            {
                newProcName = procedure.Substring(0, groupPos);
                try
                {
                    groupIndex = int.Parse(procedure.Substring(groupPos + 1));
                }
                catch
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                newProcName = procedure;
                groupIndex = 0;
            }

            procedure = newProcName;
            return groupIndex;
        }

        private static void GetProcedureTokens(ref string procedure, ref string server, ref string database, ref string owner)
        {
            string[] spNameTokens;
            int arrIndex;
            int nextPos;
            int currPos;
            int tokenCount;

            server = null;
            database = null;
            owner = null;

            spNameTokens = new string[4];

            if (!IsEmptyString(procedure))
            {
                arrIndex = 0;
                nextPos = 0;
                currPos = 0;

                while ((arrIndex < 4))
                {
                    currPos = procedure.IndexOf('.', nextPos);
                    if ((-1 == currPos))
                    {
                        spNameTokens[arrIndex] = procedure.Substring(nextPos);
                        break;
                    }
                    spNameTokens[arrIndex] = procedure.Substring(nextPos, (currPos - nextPos));
                    nextPos = (currPos + 1);
                    if ((procedure.Length <= nextPos))
                    {
                        break;
                    }
                    arrIndex = (arrIndex + 1);
                }

                tokenCount = arrIndex + 1;

                // based on how many '.' we found, we know what tokens we found
                switch (tokenCount)
                {
                    case 1:
                        procedure = spNameTokens[0];
                        break;
                    case 2:
                        procedure = spNameTokens[1];
                        owner = spNameTokens[0];
                        break;
                    case 3:
                        procedure = spNameTokens[2];
                        owner = spNameTokens[1];
                        database = spNameTokens[0];
                        break;
                    case 4:
                        procedure = spNameTokens[3];
                        owner = spNameTokens[2];
                        database = spNameTokens[1];
                        server = spNameTokens[0];
                        break;
                }
            }
        }

        private static bool IsEmptyString(string str)
        {
            if (str != null)
            {
                return (0 == str.Length);
            }
            return true;
        }

        private static SqlDbType GetSqlDbType(short paramType, string typeName)
        {
            SqlDbType cmdType;
            OleDbType oleDbType;
            cmdType = SqlDbType.Variant;
            oleDbType = (OleDbType)paramType;

            switch (oleDbType)
            {
                case OleDbType.SmallInt:
                    cmdType = SqlDbType.SmallInt;
                    break;
                case OleDbType.Integer:
                    cmdType = SqlDbType.Int;
                    break;
                case OleDbType.Single:
                    cmdType = SqlDbType.Real;
                    break;
                case OleDbType.Double:
                    cmdType = SqlDbType.Float;
                    break;
                case OleDbType.Currency:
                    cmdType = typeName == "money" ? SqlDbType.Money : SqlDbType.SmallMoney;
                    break;
                case OleDbType.Date:
                    cmdType = typeName == "datetime" ? SqlDbType.DateTime : SqlDbType.SmallDateTime;
                    break;
                case OleDbType.BSTR:
                    cmdType = typeName == "nchar" ? SqlDbType.NChar : SqlDbType.NVarChar;
                    break;
                case OleDbType.Boolean:
                    cmdType = SqlDbType.Bit;
                    break;
                case OleDbType.Variant:
                    cmdType = SqlDbType.Variant;
                    break;
                case OleDbType.Decimal:
                    cmdType = SqlDbType.Decimal;
                    break;
                case OleDbType.TinyInt:
                    cmdType = SqlDbType.TinyInt;
                    break;
                case OleDbType.UnsignedTinyInt:
                    cmdType = SqlDbType.TinyInt;
                    break;
                case OleDbType.UnsignedSmallInt:
                    cmdType = SqlDbType.SmallInt;
                    break;
                case OleDbType.BigInt:
                    cmdType = SqlDbType.BigInt;
                    break;
                case OleDbType.Filetime:
                    cmdType = typeName == "datetime" ? SqlDbType.DateTime : SqlDbType.SmallDateTime;
                    break;
                case OleDbType.Guid:
                    cmdType = SqlDbType.UniqueIdentifier;
                    break;
                case OleDbType.Binary:
                    cmdType = typeName == "binary" ? SqlDbType.Binary : SqlDbType.VarBinary;
                    break;
                case OleDbType.Char:
                    cmdType = typeName == "char" ? SqlDbType.Char : SqlDbType.VarChar;
                    break;
                case OleDbType.WChar:
                    cmdType = typeName == "nchar" ? SqlDbType.NChar : SqlDbType.NVarChar;
                    break;
                case OleDbType.Numeric:
                    cmdType = SqlDbType.Decimal;
                    break;
                case OleDbType.DBDate:
                    cmdType = typeName == "datetime" ? SqlDbType.DateTime : SqlDbType.SmallDateTime;
                    break;
                case OleDbType.DBTime:
                    cmdType = typeName == "datetime" ? SqlDbType.DateTime : SqlDbType.SmallDateTime;
                    break;
                case OleDbType.DBTimeStamp:
                    cmdType = typeName == "datetime" ? SqlDbType.DateTime : SqlDbType.SmallDateTime;
                    break;
                case OleDbType.VarChar:
                    cmdType = typeName == "char" ? SqlDbType.Char : SqlDbType.VarChar;
                    break;
                case OleDbType.LongVarChar:
                    cmdType = SqlDbType.Text;
                    break;
                case OleDbType.VarWChar:
                    cmdType = typeName == "nchar" ? SqlDbType.NChar : SqlDbType.NVarChar;
                    break;
                case OleDbType.LongVarWChar:
                    cmdType = SqlDbType.NText;
                    break;
                case OleDbType.VarBinary:
                    cmdType = typeName == "binary" ? SqlDbType.Binary : SqlDbType.VarBinary;
                    break;
                case OleDbType.LongVarBinary:
                    cmdType = SqlDbType.Image;
                    break;
            }
            return cmdType;
        }

        private static ParameterDirection GetParameterDirection(short oledbDirection)
        {
            ParameterDirection pd;
            switch (oledbDirection)
            {
                case 1:
                    pd = ParameterDirection.Input;
                    break;
                case 2:
                    pd = ParameterDirection.Output;
                    break;
                case 4:
                    pd = ParameterDirection.ReturnValue;
                    break;
                default:
                    pd = ParameterDirection.InputOutput;
                    break;
            }
            return pd;
        }
    }
}
