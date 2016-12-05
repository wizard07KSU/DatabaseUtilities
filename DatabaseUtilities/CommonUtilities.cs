using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannon.DatabaseUtilities
{
    public static class CommonUtilities
    {
        /// <summary>
        /// Converts a value from a database into the provided type. Will return
        /// the default value of T if <paramref name="aValue"/> is <see cref="DBNull"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The datatype to convert to.
        /// </typeparam>
        /// <param name="aValue">
        /// The value to convert.
        /// </param>
        /// <returns>
        /// An instance of T that represents the provided argument.
        /// </returns>
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

        /// <summary>
        /// Converts a value from a database into the provided type. Will return
        /// the provided default argument value if <paramref name="aValue"/> is 
        /// <see cref="DBNull"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The datatype to convert to.
        /// </typeparam>
        /// <param name="aValue">
        /// The value to convert.
        /// </param>
        /// <param name="aDefault">
        /// The default value to return if <paramref name="aValue"/> is null.
        /// </param>
        /// <returns>
        /// An instance of T that represents the provided argument.
        /// </returns>
        public static T Convert<T>( object aValue, T aDefault )
        {
            if ( aValue is System.DBNull )
            {
                return aDefault;
            }
            else
            {
                return (T)aValue;
            }
        }
    }
}
