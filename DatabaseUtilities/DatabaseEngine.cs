﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cannon.DatabaseUtilities
{
    public abstract class DatabaseEngine : IDisposable
    {
        #region Variables

        protected DatabaseWrapper mDatabase = null;

        /// <summary>
        /// The maximum number of times to attempt to close a connection before
        /// giving up.
        /// </summary>
        private const int mMaxNumberCloseAttempts = 5;

        /// <summary>
        /// The number of miliseconds to wait before attempting to close a 
        /// connection again.
        /// </summary>
        private const int mCloseAttemptInterval = 5000;

        #endregion

        #region Properties

        /// <summary>
        /// The name of the database connected.
        /// </summary>
        public string DatabaseName { get; protected set; }

        /// <summary>
        /// The most recently started transaction.
        /// </summary>
        public DbTransaction GetLastStartedTransaction { get; private set; }

        /// <summary>
        /// Gets whether the connection is open and available for connections.
        /// </summary>
        public bool IsConnected { get { return mDatabase != null && mDatabase.IsConnected; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseEngine"/> class
        /// using the provided connection object.
        /// </summary>
        /// <param name="aConnection">
        /// The connection object.
        /// </param>
        protected DatabaseEngine( DbConnection aConnection )
        {
            mDatabase = new DatabaseWrapper( aConnection );
        }

        #endregion

        #region Connect

        /// <summary>
        /// Opens the database connection.
        /// </summary>
        public void OpenDatabase()
        {
            if ( this.mDatabase == null )
            {
                return;
            }
            this.mDatabase.Open();
        }

        /// <summary>
        /// Attempts to close the database connection. Only valid if the 
        /// connection is in ither the <see cref="ConnectionState.Broken"/> 
        /// or <see cref="ConnectionState.Closed"/> states.
        /// </summary>
        /// <exception cref="Exception">
        /// Thrown when the connection is in the <see cref="ConnectionState.Connecting"/>,
        /// <see cref="ConnectionState.Executing"/>, or 
        /// <see cref="ConnectionState.Fetching"/> states.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the state of the excption isn't recognized.
        /// </exception>
        public void CloseDatabase()
        {
            if ( this.mDatabase == null )
            {
                return;
            }
            mDatabase.Close();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes of the database connection.
        /// </summary>
        public void Dispose()
        {
            int lCounter = 0;
            while ( lCounter++ < 5 )
            {
                try
                {
                    this.CloseDatabase();
                    mDatabase.Dispose();
                }
                catch
                {
                    Thread.Sleep( mCloseAttemptInterval );
                }
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
                this.GetLastStartedTransaction = this.mDatabase.StartTransaction();
                return this.GetLastStartedTransaction;
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
                this.GetLastStartedTransaction = this.mDatabase.StartTransaction( aIsolationLevel );
                return this.GetLastStartedTransaction;
            }
            return null;
        }

        #endregion

        #region Execute Command

        /// <summary>
        /// Executes the provided SQL command on the connected database. Used 
        /// when the caller doesn't care about the returned value.
        /// </summary>
        /// <param name="aCommandText">
        /// The text of the command to execute.
        /// </param>
        /// <returns>
        /// Returns the result of executing this query on the database, or 0 
        /// if there was no result.
        /// </returns>
        public long ExecuteCommand( string aCommandText )
        {
            return this.ExecuteCommand(
                aCommandText : aCommandText,
                aTransaction : null,
                aParameters  : new ParameterCollection() );
        }

        /// <summary>
        /// Executes the provided SQL command on the connected database. Used 
        /// when the caller doesn't care about the returned value.
        /// </summary>
        /// <param name="aCommandText">
        /// The text of the command to execute.
        /// </param>
        /// <param name="aTransaction">
        /// The transaction to execute this command in.
        /// </param>
        /// <returns>
        /// Returns the result of executing this query on the database, or 0 
        /// if there was no result.
        /// </returns>
        public long ExecuteCommand(
            string aCommandText,
            DbTransaction aTransaction )
        {
            return this.ExecuteCommand(
                aCommandText : aCommandText,
                aTransaction : aTransaction,
                aParameters  : new ParameterCollection() );
        }

        /// <summary>
        /// Executes the provided SQL command on the connected database. Used 
        /// when the caller doesn't care about the returned value.
        /// </summary>
        /// <param name="aCommandText">
        /// The text of the command to execute.
        /// </param>
        /// <param name="aTransaction">
        /// The transaction to execute this command in.
        /// </param>
        /// <param name="aParameters">
        /// The collection of parameters to use when executing this command.
        /// </param>
        /// <returns>
        /// Returns the result of executing this query on the database, or 0 
        /// if there was no result.
        /// </returns>
        public long ExecuteCommand( 
            string aCommandText,
            DbTransaction aTransaction,
            ParameterCollection aParameters )
        {
            DbCommand lCommand = mDatabase.Connection.CreateCommand();
            lCommand.Transaction = aTransaction;
            lCommand.Parameters.AddRange( aParameters.Parameters.ToArray() );

            return mDatabase.ExecuteCommand( lCommand );
        }

        #endregion

        #region Execute Query

        public ICollection<T> ExecuteQuery<T>(
            string aCommandText,
            DbTransaction aTransaction,
            ParameterCollection aParameters )
        {
            DbCommand lCommand = mDatabase.Connection.CreateCommand();
            lCommand.Transaction = aTransaction;
            lCommand.Parameters.AddRange( aParameters.Parameters.ToArray() );

            return mDatabase.ExecuteCommand( lCommand );
        }

        #endregion


    }
}