using System;
using System.Data;
using System.IO;
using System.Xml;
using Datory;
using Oracle.ManagedDataAccess.Client;
using SiteServer.CMS.Apis;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Core
{
    public class Oracle : DatabaseApi
    {
        //public override IDataParameter[] GetDataParameters(int size)
        //{
        //    return new OracleParameter[size];
        ////}
        
        //public override IDbConnection GetConnection(string connectionString)
        //{
        //    return new OracleConnection(connectionString);
        //}
        
        //public override IDbDataAdapter GetDataAdapter()
        //{
        //    return new OracleDataAdapter();
        //}
        
        //public override void DeriveParameters(IDbCommand cmd)
        //{
        //    bool mustCloseConnection = false;


        //    if (!(cmd is OracleCommand))
        //        throw new ArgumentException("The command provided is not a SqlCommand instance.", "cmd");


        //    if (cmd.Connection.State != ConnectionState.Open)
        //    {
        //        cmd.Connection.Open();
        //        mustCloseConnection = true;
        //    }


        //    OracleCommandBuilder.DeriveParameters((OracleCommand)cmd);


        //    if (mustCloseConnection)
        //    {
        //        cmd.Connection.Close();
        //    }
        //}

        //public override IDataParameter GetParameter()
        //{
        //    return new OracleParameter();
        //}

        //public override IDataParameter GetParameter(string name, object value)
        //{
        //    var parameter = new OracleParameter
        //    {
        //        ParameterName = name
        //    };
        //    if (value == null)
        //    {
        //        parameter.DbType = DbType.String;
        //        parameter.Value = null;
        //    }
        //    else if (value is DateTime)
        //    {
        //        parameter.DbType = DbType.DateTime;
        //        var dbValue = (DateTime)value;
        //        if (dbValue < DateUtils.SqlMinValue)
        //        {
        //            dbValue = DateUtils.SqlMinValue;
        //        }
        //        parameter.Value = dbValue;
        //    }
        //    else if (value is int)
        //    {
        //        parameter.DbType = DbType.Int32;
        //        parameter.Value = (int)value;
        //    }
        //    else if (value is decimal)
        //    {
        //        parameter.DbType = DbType.Decimal;
        //        parameter.Value = (decimal)value;
        //    }
        //    else if (value is string)
        //    {
        //        parameter.DbType = DbType.String;
        //        parameter.Value = SqlUtils.ToOracleDbValue(DataType.VarChar, value);
        //        //parameter.Value = (string)value;
        //    }
        //    else if (value is bool)
        //    {
        //        parameter.DbType = DbType.Int32;
        //        parameter.Value = (bool)value ? 1 : 0;
        //    }
        //    else
        //    {
        //        parameter.DbType = DbType.String;
        //        parameter.Value = SqlUtils.ToOracleDbValue(DataType.VarChar, value.ToString());
        //        //parameter.Value = value.ToString();
        //    }

        //    return parameter;
        //}

        public override void ClearCommand(IDbCommand command)
        {
            // HACK: There is a problem here, the output parameter values are fletched 
            // when the reader is closed, so if the parameters are detached from the command
            // then the IDataReader can磘 set its values. 
            // When this happen, the parameters can磘 be used again in other command.
            bool canClear = true;


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
            bool mustCloseConnection = false;


            if (command.Connection.State != ConnectionState.Open)
            {
                command.Connection.Open();
                mustCloseConnection = true;
            }


            CleanParameterSyntax(command);
            OracleDataAdapter da = new OracleDataAdapter((OracleCommand)command);
            DataSet ds = new DataSet();


            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            da.Fill(ds);


            StringReader stream = new StringReader(ds.GetXml());
            if (mustCloseConnection)
            {
                command.Connection.Close();
            }


            return new XmlTextReader(stream);
        }
        
        

        protected override IDataParameter GetBlobParameter(IDbConnection connection, IDataParameter p)
        {
            // do nothing special for BLOBs...as far as we know now.
            return p;
        }

        public override string GetString(IDataReader rdr, int i)
        {
            var retVal = base.GetString(rdr, i);

            if (retVal == StringUtils.Constants.OracleEmptyValue)
            {
                retVal = string.Empty;
            }
            return retVal;
        }
    }
}
