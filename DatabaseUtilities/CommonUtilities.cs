using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannon.DatabaseUtilities
{
    public static class CommonUtilities
    {
        public static T Convert<T>( object aValue )
        {
            if ( aValue is System.DBNull )
            {
                return default( T );
            }
            else
            {
                return (T)aValue;
            }
        }
    }
}
