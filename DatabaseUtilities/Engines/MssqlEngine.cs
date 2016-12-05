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
            :base( new SqlConnection( aConnectionString ) )
        {
            this.OpenDatabase();
        }

        public MssqlEngine( SqlConnectionStringBuilder aConnectionString )
            : this( aConnectionString.ConnectionString ) { }
    }
}
