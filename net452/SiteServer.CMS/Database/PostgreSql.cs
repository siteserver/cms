using System;
using System.Data;
using System.IO;
using System.Xml;
using Npgsql;
using SiteServer.CMS.Apis;

namespace SiteServer.CMS.Database.Core
{
    public class PostgreSql : DatabaseApi
    {
        //public override IDataParameter[] GetDataParameters(int size)
        //{
        //    return new NpgsqlParameter[size];
        //}
        
        //public override IDbConnection GetConnection(string connectionString)
        //{
        //    return new NpgsqlConnection(connectionString);
        //}

        //public override IDbDataAdapter GetDataAdapter()
        //{
        //    return new NpgsqlDataAdapter();
        //}

        //public override void DeriveParameters(IDbCommand cmd)
        //{
        //    bool mustCloseConnection = false;


        //    if (!(cmd is NpgsqlCommand))
        //        throw new ArgumentException("The command provided is not a SqlCommand instance.", "cmd");


        //    if (cmd.Connection.State != ConnectionState.Open)
        //    {
        //        cmd.Connection.Open();
        //        mustCloseConnection = true;
        //    }


        //    NpgsqlCommandBuilder.DeriveParameters((NpgsqlCommand)cmd);


        //    if (mustCloseConnection)
        //    {
        //        cmd.Connection.Close();
        //    }
        //}

        //public override IDataParameter GetParameter()
        //{
        //    return new NpgsqlParameter();
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
            NpgsqlDataAdapter da = new NpgsqlDataAdapter((NpgsqlCommand)command);
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
    }
}
