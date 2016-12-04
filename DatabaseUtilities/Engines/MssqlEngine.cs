using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannon.DatabaseUtilities.Engines
{
    public class MssqlEngine : DatabaseEngine
    {
        public MssqlEngine( string aConnectionString )
        {
            this.mConnection = new SqlConnection( aConnectionString );
            this.DatabaseName = mConnection.Database;
            this.OpenDatabase();
        }

        public MssqlEngine( SqlConnectionStringBuilder aConnectionString )
        {
            this.mConnection = new SqlConnection( aConnectionString.ConnectionString );
            this.DatabaseName = mConnection.Database;
            this.OpenDatabase();
        }
    }
}
