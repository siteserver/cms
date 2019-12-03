using System;
using System.Data;
using System.IO;
using System.Xml;
using MySql.Data.MySqlClient;

namespace SiteServer.CMS.Repositories
{
    public class MySql : DatabaseApi
    {
        public override IDbConnection GetConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        public override IDbDataAdapter GetDataAdapter()
        {
            return new MySqlDataAdapter();
        }

        protected override void ClearCommand(IDbCommand command)
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

        protected override IDataParameter GetBlobParameter(IDbConnection connection, IDataParameter p)
        {
            // do nothing special for BLOBs...as far as we know now.
            return p;
        }
    }
}
