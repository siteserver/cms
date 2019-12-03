using System;
using System.Data;
using System.Data.Common;
using System.Xml;
using Datory;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Repositories
{
    public abstract class DatabaseApi
    {
        protected enum AdoConnectionOwnership
        {
            /// <summary>Connection is owned and managed by ADOHelper</summary>
            Internal,

            /// <summary>Connection is owned and managed by the caller</summary>
            External
        }

        public abstract IDbConnection GetConnection(string connectionString);

        public abstract IDbDataAdapter GetDataAdapter();

        protected abstract IDataParameter GetBlobParameter(IDbConnection connection, IDataParameter p);


        protected void AttachParameters(IDbCommand command, IDataParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (commandParameters != null)
            {
                foreach (var p in commandParameters)
                {
                    if (p != null)
                    {
                        // Check for derived output value with no value assigned
                        if ((p.Direction == ParameterDirection.InputOutput ||
                             p.Direction == ParameterDirection.Input) &&
                            p.Value == null)
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p.DbType == DbType.Binary ? GetBlobParameter(command.Connection, p) : p);
                    }
                }
            }
        }


        public void CleanParameterSyntax(IDbCommand command)
        {
            // do nothing by default
        }

        protected void PrepareCommand(IDbCommand command, IDbConnection connection, IDbTransaction transaction,
            string commandText, IDataParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException(nameof(commandText));
            if (WebConfigUtils.DatabaseType != DatabaseType.SqlServer)
            {
                commandText = commandText.Replace("[", string.Empty).Replace("]", string.Empty);
                if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
                {
                    commandText = commandText.Replace("@", ":");
                }
            }

            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }

            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            // If we were provided a transaction, assign it
            if (transaction != null)
            {
                if (transaction.Connection == null)
                    throw new ArgumentException(
                        "The transaction was rolled back or commited, please provide an open transaction.",
                        nameof(transaction));
                command.Transaction = transaction;
            }

            // Set the command type
            command.CommandType = CommandType.Text;

            // Attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
        }

        protected virtual void ClearCommand(IDbCommand command)
        {
            // do nothing by default
        }

        private DataSet ExecuteDataset(IDbCommand command)
        {
            var mustCloseConnection = false;

            // Clean Up Parameter Syntax
            CleanParameterSyntax(command);

            if (command.Connection.State != ConnectionState.Open)
            {
                command.Connection.Open();
                mustCloseConnection = true;
            }

            // Create the DataAdapter & DataSet
            IDbDataAdapter da = null;
            try
            {
                da = GetDataAdapter();
                da.SelectCommand = command;

                var ds = new DataSet();

                // Fill the DataSet using default values for DataTable names, etc
                da.Fill(ds);

                // Detach the IDataParameters from the command object, so they can be used again
                // Don't do this...screws up output params -- cjb 
                //command.Parameters.Clear();

                // Return the DataSet
                return ds;
            }
            finally
            {
                if (mustCloseConnection)
                {
                    command.Connection.Close();
                }
                if (da != null)
                {
                    var id = da as IDisposable;
                    if (id != null)
                        id.Dispose();
                }
            }
        }

        public DataSet ExecuteDataset(string connectionString, string commandText)
        {
            // Pass through the call providing null for the set of IDataParameters
            return ExecuteDataset(connectionString, commandText, null);
        }

        private DataSet ExecuteDataset(string connectionString, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            // Create & open an IDbConnection, and dispose of it after we are done
            using (var connection = GetConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteDataset(connection, commandText, commandParameters);
            }
        }


        private DataSet ExecuteDataset(IDbConnection connection, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            // Create a command and prepare it for execution
            var cmd = connection.CreateCommand();
            bool mustCloseConnection;
            PrepareCommand(cmd, connection, null, commandText, commandParameters, out mustCloseConnection);
            CleanParameterSyntax(cmd);

            var ds = ExecuteDataset(cmd);

            if (mustCloseConnection)
                connection.Close();

            // Return the DataSet
            return ds;
        }

        private int ExecuteNonQuery(IDbCommand command)
        {
            var mustCloseConnection = false;

            // Clean Up Parameter Syntax
            CleanParameterSyntax(command);

            if (command.Connection.State != ConnectionState.Open)
            {
                command.Connection.Open();
                mustCloseConnection = true;
            }

            if (command == null) throw new ArgumentNullException(nameof(command));

            int returnVal = command.ExecuteNonQuery();

            if (mustCloseConnection)
            {
                command.Connection.Close();
            }

            return returnVal;
        }

        public int ExecuteNonQuery(string connectionString, string commandText)
        {
            // Pass through the call providing null for the set of IDataParameters
            return ExecuteNonQuery(connectionString, commandText, null);
        }

        public int ExecuteNonQuery(string connectionString, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            // Create & open an IDbConnection, and dispose of it after we are done
            using (var connection = GetConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteNonQuery(connection, commandText, commandParameters);
            }
        }

        private int ExecuteNonQuery(IDbConnection connection, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            // Create a command and prepare it for execution
            var cmd = connection.CreateCommand();
            bool mustCloseConnection;
            PrepareCommand(cmd, connection, null, commandText, commandParameters, out mustCloseConnection);
            CleanParameterSyntax(cmd);

            // Finally, execute the command
            var retVal = ExecuteNonQuery(cmd);

            // Detach the IDataParameters from the command object, so they can be used again
            // don't do this...screws up output parameters -- cjbreisch
            // cmd.Parameters.Clear();
            if (mustCloseConnection)
                connection.Close();
            return retVal;
        }

        public int ExecuteNonQuery(IDbTransaction transaction, string commandText)
        {
            // Pass through the call providing null for the set of IDataParameters
            return ExecuteNonQuery(transaction, commandText, null);
        }

        private int ExecuteNonQuery(IDbTransaction transaction, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException(
                    "The transaction was rolled back or commited, please provide an open transaction.",
                    nameof(transaction));

            // Create a command and prepare it for execution
            var cmd = transaction.Connection.CreateCommand();
            bool mustCloseConnection;
            PrepareCommand(cmd, transaction.Connection, transaction, commandText, commandParameters,
                out mustCloseConnection);
            CleanParameterSyntax(cmd);

            // Finally, execute the command
            var retVal = ExecuteNonQuery(cmd);

            // Detach the IDataParameters from the command object, so they can be used again
            // don't do this...screws up output parameters -- cjbreisch
            // cmd.Parameters.Clear();
            return retVal;
        }

        protected IDataReader ExecuteReader(IDbCommand command, AdoConnectionOwnership connectionOwnership)
        {
            // Clean Up Parameter Syntax
            CleanParameterSyntax(command);

            if (command.Connection.State != ConnectionState.Open)
            {
                command.Connection.Open();
                connectionOwnership = AdoConnectionOwnership.Internal;
            }

            // Create a reader

            // Call ExecuteReader with the appropriate CommandBehavior
            var dataReader = connectionOwnership == AdoConnectionOwnership.External
                ? command.ExecuteReader()
                : command.ExecuteReader(CommandBehavior.CloseConnection);

            ClearCommand(command);

            return dataReader;
        }

        private IDataReader ExecuteReader(IDbConnection connection, IDbTransaction transaction, string commandText,
            IDataParameter[] commandParameters, AdoConnectionOwnership connectionOwnership)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            var mustCloseConnection = false;
            // Create a command and prepare it for execution
            var cmd = connection.CreateCommand();
            try
            {
                PrepareCommand(cmd, connection, transaction, commandText, commandParameters, out mustCloseConnection);
                CleanParameterSyntax(cmd);

                // override conenctionOwnership if we created the connection in PrepareCommand -- cjbreisch
                if (mustCloseConnection)
                {
                    connectionOwnership = AdoConnectionOwnership.Internal;
                }

                // Create a reader

                var dataReader = ExecuteReader(cmd, connectionOwnership);

                ClearCommand(cmd);

                return dataReader;
            }
            catch
            {
                if (mustCloseConnection)
                    connection.Close();
                throw;
            }
        }

        public IDataReader ExecuteReader(string connectionString, string commandText)
        {
            // Pass through the call providing null for the set of IDataParameters
            return ExecuteReader(connectionString, commandText, null);
        }

        private IDataReader ExecuteReader(string connectionString, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            IDbConnection connection = null;
            try
            {
                connection = GetConnection(connectionString);
                connection.Open();

                // Call the private overload that takes an internally owned connection in place of the connection string
                return ExecuteReader(connection, null, commandText, commandParameters, AdoConnectionOwnership.Internal);
            }
            catch
            {
                // If we fail to return the IDataReader, we need to close the connection ourselves
                connection?.Close();
                throw;
            }

        }

        public IDataReader ExecuteReader(IDbConnection connection, string commandText)
        {
            // Pass through the call providing null for the set of IDataParameters
            return ExecuteReader(connection, commandText, null);
        }

        private IDataReader ExecuteReader(IDbConnection connection, string commandText,
            params IDataParameter[] commandParameters)
        {
            // Pass through the call to the private overload using a null transaction value and an externally owned connection
            return ExecuteReader(connection, null, commandText, commandParameters, AdoConnectionOwnership.External);
        }
    }
}
