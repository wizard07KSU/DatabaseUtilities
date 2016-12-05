using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannon.DatabaseUtilities.Engines
{
    /// <summary>
    /// The engine used to connect to MSSQL databases hosted on MS SQL Server.
    /// </summary>
    public class MssqlEngine : DatabaseEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MssqlEngine"/> class
        /// connecting to a database using the provided connection string.
        /// </summary>
        /// <param name="aConnectionString">
        /// The connection string to use to connect to a database.
        /// </param>
        public MssqlEngine( string aConnectionString )
            :base( new SqlConnection( aConnectionString ) )
        {
            this.OpenDatabase();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MssqlEngine"/> class
        /// connecting to a database using the provided connection string.
        /// </summary>
        /// <param name="aConnectionString">
        /// The connection string to use to connect to a database.
        /// </param>
        public MssqlEngine( SqlConnectionStringBuilder aConnectionString )
            : this( aConnectionString.ConnectionString ) { }
    }
}
