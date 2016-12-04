using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannon.DatabaseUtilities.Exceptions
{
    /// <summary>
    /// Thrown when a query being executed on the database fails.
    /// </summary>
    public class DatabaseQueryException : Exception
    {
        public DatabaseQueryException(
            string aQueryText,
            string aDatabaseName,
            Exception aInnerException )
            : base( 
                  string.Format(
                      "Failed to execute this query on the database [{1}]: [{0}]",
                      aQueryText,
                      aDatabaseName ), 
                  aInnerException ) { }
    }
}
