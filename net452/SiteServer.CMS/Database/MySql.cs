using System;
using System.Data;
using System.IO;
using System.Xml;
using MySql.Data.MySqlClient;
using SiteServer.CMS.Apis;

namespace SiteServer.CMS.Database.Core
{
    public class MySql : DatabaseApi
    {
        //public override IDataParameter[] GetDataParameters(int size)
        //{
        //    return new MySqlParameter[size];
        ////}

        //public override IDbConnection GetConnection(string connectionString)
        //{
        //    return new MySqlConnection(connectionString);
        //}



        //public override IDbDataAdapter GetDataAdapter()
        //{
        //    return new MySqlDataAdapter();
        //}



        //public override void DeriveParameters(IDbCommand cmd)
        //{
        //    bool mustCloseConnection = false;


        //    if (!(cmd is MySqlCommand))
        //        throw new ArgumentException("The command provided is not a SqlCommand instance.", "cmd");


        //    if (cmd.Connection.State != ConnectionState.Open)
        //    {
        //        cmd.Connection.Open();
        //        mustCloseConnection = true;
        //    }


        //    MySqlCommandBuilder.DeriveParameters((MySqlCommand)cmd);


        //    if (mustCloseConnection)
        //    {
        //        cmd.Connection.Close();
        //    }
        //}



        //public override IDataParameter GetParameter()
        //{
        //    return new MySqlParameter();
        //}



        //public override void ClearCommand(IDbCommand command)
        //{
        //    // HACK: There is a problem here, the output parameter values are fletched 
        //    // when the reader is closed, so if the parameters are detached from the command
        //    // then the IDataReader can磘 set its values. 
        //    // When this happen, the parameters can磘 be used again in other command.
        //    bool canClear = true;


        //    foreach (IDataParameter commandParameter in command.Parameters)
        //    {
        //        if (commandParameter.Direction != ParameterDirection.Input)
        //            canClear = false;


        //    }
        //    if (canClear)
        //    {
        //        command.Parameters.Clear();
        //    }
        //}



        //public override void CleanParameterSyntax(IDbCommand command)
        //{
        //    // do nothing for SQL
        //}



        //public override XmlReader ExecuteXmlReader(IDbCommand command)
        //{
        //    bool mustCloseConnection = false;


        //    if (command.Connection.State != ConnectionState.Open)
        //    {
        //        command.Connection.Open();
        //        mustCloseConnection = true;
        //    }


        //    CleanParameterSyntax(command);
        //    MySqlDataAdapter da = new MySqlDataAdapter((MySqlCommand)command);
        //    DataSet ds = new DataSet();


        //    da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
        //    da.Fill(ds);


        //    StringReader stream = new StringReader(ds.GetXml());
        //    if (mustCloseConnection)
        //    {
        //        command.Connection.Close();
        //    }


        //    return new XmlTextReader(stream);
        //}



        //protected override IDataParameter GetBlobParameter(IDbConnection connection, IDataParameter p)
        //{
        //    // do nothing special for BLOBs...as far as we know now.
        //    return p;
        //}
    }
}
