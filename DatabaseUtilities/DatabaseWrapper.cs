﻿using Cannon.DatabaseUtilities.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannon.DatabaseUtilities
{
    public class DatabaseWrapper : IDisposable
    {
        #region Members
        #endregion

        #region Properties

        public DbConnection Connection { get; private set; } = null;

        #endregion

        #region Constructors
        /// <summary>
        /// Intializes a new instance of the <see cref="DatabaseWrapper"/> 
        /// class using the provided connection object.
        /// </summary>
        /// <param name="aConnection">
        /// The connection object.
        /// </param>
        public DatabaseWrapper( DbConnection aConnection )
        {
            Connection = aConnection;
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes of the database connection.
        /// </summary>
        public void Dispose()
        {
            if ( Connection != null )
            {
                Connection.Dispose();
            }
        }

        #endregion

        #region Transactions

        /// <summary>
        /// Starts a new transaction.
        /// </summary>
        /// <returns>
        /// The newly started transaction.
        /// </returns>
        public DbTransaction StartTransaction()
        {
            if ( this.IsConnected )
            {
                return this.Connection.BeginTransaction();
            }
            return null;
        }

        /// <summary>
        /// Starts a new transaction with the provided isolation level.
        /// </summary>
        /// <returns>
        /// The newly started transaction.
        /// </returns>
        public DbTransaction StartTransaction( IsolationLevel aIsolationLevel )
        {
            if ( this.IsConnected )
            {
                return this.Connection.BeginTransaction( aIsolationLevel );
            }
            return null;
        }

        #endregion

        #region Connect
        /// <summary>
        /// Opens the database.
        /// </summary>
        public void Open()
        {
            if ( Connection.State == System.Data.ConnectionState.Open )
                throw new InvalidOperationException( "Connection already open." );
            Connection.Open();
        }

        /// <summary>
        /// Closes the database.
        /// </summary>
        public void Close()
        {
            if ( this.Connection == null )
            {
                return;
            }
            switch ( this.Connection.State )
            {
                case ConnectionState.Closed:
                    break;
                case ConnectionState.Connecting:
                case ConnectionState.Executing:
                case ConnectionState.Fetching:
                    throw new Exception( "Can't close connection while executing command." );
                case ConnectionState.Broken:
                case ConnectionState.Open:
                    this.Connection.Close();
                    break;
                default:
                    throw new InvalidOperationException(
                        string.Format(
                            "Connection is in unknown state: [{0}]",
                            this.Connection.State ) );
            }
        }

        /// <summary>
        /// Gets whether the connection is open and available for connections.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return this.Connection != null
                    && this.Connection.State != ConnectionState.Broken
                    && this.Connection.State != ConnectionState.Closed;
            }
        }
        #endregion

        #region Execute Query
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="aCommand"></param>
        /// <param name="aConverter"></param>
        /// <returns></returns>
        public List<T> QueryDatabase<T>( DbCommand aCommand, Func<DbDataReader, T> aConverter )
        {
            List<T> lToReturn = new List<T>();
            try
            {
                using ( DbDataReader reader = aCommand.ExecuteReader() )
                {
                    while ( reader.Read() )
                    {
                        lToReturn.Add( aConverter( reader ) );
                    }
                    reader.Close();
                }
            }
            catch ( Exception ex )
            {
                // TODO: Log failure here.
                throw new DatabaseQueryException(
                    aCommand.CommandText,
                    aCommand.Connection.Database,
                    ex );
            }
            return lToReturn;
        }
        #endregion

        #region Execute Command
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aCommand"></param>
        /// <returns></returns>
        public long ExecuteCommand( DbCommand aCommand )
        {
            try
            {
                return aCommand.ExecuteNonQuery();
            }
            catch ( Exception ex )
            {
                // TODO: Log failure here.
                throw new DatabaseQueryException(
                    aCommand.CommandText,
                    aCommand.Connection.Database,
                    ex );
            }
        }
        #endregion
    }
}
