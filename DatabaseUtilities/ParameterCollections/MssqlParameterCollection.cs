using Cannon.DatabaseUtilities.MetaData;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cannon.DatabaseUtilities.ParameterCollections
{
    /// <summary>
    /// Collection of <see cref="SqlParameter"/> objects to be used in a 
    /// database query/command.
    /// </summary>
    public class MssqlParameterCollection : ParameterCollection
    {

        /// <summary>
        /// Initializes a new, empty instance of the <see cref="MssqlParameterCollection"/> 
        /// class.
        /// </summary>
        public MssqlParameterCollection()
        {
        }

        /// <summary>
        /// Used by base class to create a new <see cref="SqlParameter"/> object.
        /// </summary>
        /// <returns>
        /// Returns a newly created <see cref="SqlParameter"/> object.
        /// </returns>
        protected override DbParameter CreateNewParameter()
        {
            return new SqlParameter();
        }

        #region Lookup Tables
        /// <summary>
        /// A lookup table from .NET CLR types to SQL DB Types. Used when creating SQL objects
        /// that need a specific <see cref="SqlDbType"/> .
        /// </summary>
        private Dictionary<Type, SqlDbType> lSqlTypeLookup = new Dictionary<Type, SqlDbType>()
        {
            { typeof( short   ), SqlDbType.SmallInt },
            { typeof( int     ), SqlDbType.Int      },
            { typeof( long    ), SqlDbType.BigInt   },
            { typeof( decimal ), SqlDbType.Decimal  },
            { typeof( float   ), SqlDbType.Float    },
            { typeof( double  ), SqlDbType.Float    },
            { typeof( string  ), SqlDbType.NVarChar }
        };

        private Dictionary<Type, SqlDbType> lStringTypeLookup = new Dictionary<Type, SqlDbType>()
        {
            { typeof( string  ), SqlDbType.NVarChar },
            { typeof( char    ), SqlDbType.Char     }
        };

        private Dictionary<Type, string> lSetFunctionLookup = new Dictionary<Type, string>()
        {
            { typeof( short   ), "SetInt16"   },
            { typeof( int     ), "SetInt32"   },
            { typeof( long    ), "SetInt64"   },
            { typeof( decimal ), "SetDecimal" },
            { typeof( float   ), "SetFloat"   },
            { typeof( double  ), "SetDouble"  },
            { typeof( string  ), "SetString"  }
        };
        #endregion

        /// <summary>
        /// Adds a single, named string-type parameter to this collection.
        /// </summary>
        /// <typeparam name="T">
        /// The CLR data-type of the value to add.
        /// </typeparam>
        /// <param name="aParameterName">
        /// The name of this parameter.
        /// </param>
        /// <param name="aParameterValue">
        /// The value of this parameter.
        /// </param>
        /// <param name="aParameterLength">
        /// The maximum length of the column this parameter refers to in the database.
        /// </param>
        public void Add<T>(
            string aParameterName,
            T aParameterValue,
            int aParameterLength )
        {
            this.Add(
                aParameterName,
                aParameterValue,
                lSqlTypeLookup[ typeof( T ) ],
                aParameterLength );
        }

        /// <summary>
        /// Adds a single, named string-type parameter to this collection.
        /// </summary>
        /// <typeparam name="T">
        /// The CLR data-type of the value to add.
        /// </typeparam>
        /// <param name="aParameterName">
        /// The name of this parameter.
        /// </param>
        /// <param name="aParameterValue">
        /// The value of this parameter.
        /// </param>
        /// <param name="aDbType">
        /// The SQL datatype of the value of this parameter.
        /// </param>
        /// <param name="aParameterLength">
        /// The maximum length of the column this parameter refers to in the database.
        /// </param>
        public void Add<T>(
            string aParameterName,
            T aParameterValue,
            SqlDbType aDbType,
            int aParameterLength )
        {
            if ( !lSqlTypeLookup.ContainsKey( typeof( T ) ) )
            {
                throw new ArgumentException( "Type not recognized!", "aParameterValue" );
            }
            SqlParameter lNewParameter = new SqlParameter(
                aParameterName,
                aDbType, 
                aParameterLength );
            lNewParameter.Value = aParameterValue;
            this.Add( lNewParameter );
        }

        /// <summary>
        /// Adds a named Table Valued Parameter to this collection.
        /// </summary>
        /// <typeparam name="T">
        /// The type of value in the TVP. For this overload, this type should
        /// be a .NET CLR type, or some other type for which an implicit conversion
        /// exists from the .NET type to a SQL type.
        /// </typeparam>
        /// <param name="aParameterValues">
        /// The set of values to be streamed in the table parameter.
        /// </param>
        /// <param name="aParameterName">
        /// The name of the parameter in the text of the SQL statement.
        /// </param>
        /// <param name="aSqlTableType">
        /// The name of the server-side table type this parameter will use.
        /// </param>
        /// <param name="aMetaData">
        /// A meta data object that has information about the column of the
        /// table type to be used.
        /// </param>
        public void Add<T>(
            ICollection<T> aParameterValues,
            string aParameterName,
            string aSqlTableType,
            SqlColumnMetaData aMetaData )
        {
            SqlParameter lNewParameter = new SqlParameter(
                aParameterName,
                SqlDbType.Structured );
            lNewParameter.TypeName = aSqlTableType;

            SqlMetaData[] lMetaDataArray = new SqlMetaData[ 1 ];

            // The SqlMetaData class will throw an exception if the constructor
            // with 3 parameters is used for non-string types, just as it will
            // if the constructor with 2 parameters is used for string-types.
            if ( lStringTypeLookup.ContainsKey( typeof( T ) ) )
            {
                lMetaDataArray[ 0 ] = new SqlMetaData(
                    aMetaData.ColumnName,
                    lSqlTypeLookup[ typeof( T ) ],
                    aMetaData.ColumnLength );
            }
            else
            {
                lMetaDataArray[ 0 ] = new SqlMetaData(
                    aMetaData.ColumnName,
                    lSqlTypeLookup[ typeof( T ) ] );
            }

            SqlDataRecord[] lRecords = new SqlDataRecord[ aParameterValues.Count ];

            int i = 0;
            foreach ( T iValue in aParameterValues )
            {
                SqlDataRecord iRecord = new SqlDataRecord( lMetaDataArray );
                iRecord.SetValue( 0, iValue );
                lRecords[ i++ ] = iRecord;
            }

            lNewParameter.Value = lRecords;

            this.Add( lNewParameter );
        }

        public void Add<T>(
            ICollection<T> aParameterValues,
            string aParameterName,
            string aSqlTableType,
            SqlColumnMetaData[] aMetaData )
        {
            SqlParameter lNewParameter = new SqlParameter(
                aParameterName,
                SqlDbType.Structured );
            lNewParameter.TypeName = aSqlTableType;

            SqlMetaData[]  lMetaDataArray = new SqlMetaData  [ aMetaData.Length ];
            MethodInfo[]   lSetFunctions  = new MethodInfo   [ aMetaData.Length ];
            PropertyInfo[] lGetFunctions  = new PropertyInfo [ aMetaData.Length ];
            for ( int i = 0; i < aMetaData.Length; i++ )
            {
                // The SqlMetaData class will throw an exception if the constructor
                // with 3 parameters is used for non-string types, just as it will
                // if the constructor with 2 parameters is used for string-types.
                if ( lStringTypeLookup.ContainsKey( typeof( T ) ) )
                {
                    lMetaDataArray[ i ] = new SqlMetaData(
                        aMetaData[ i ].ColumnName,
                        lSqlTypeLookup[ typeof( T ) ],
                        aMetaData[ i ].ColumnLength );
                }
                else
                {
                    lMetaDataArray[ i ] = new SqlMetaData(
                        aMetaData[ i ].ColumnName,
                        lSqlTypeLookup[ typeof( T ) ] );
                }

                // Get the Method we need to call on each SqlDataRecord object
                // for each column to correctly set the value of that column.
                lSetFunctions[ i ] = typeof( SqlDataRecord )
                    .GetMethod( lSetFunctionLookup[ typeof( T ) ] );

                // Get the Property on the parameter value objects to call to
                // get the value of that parameter. These values will then be
                // assigned to an ordinal of a SqlDataRecord object using the
                // set functions obtained on the previous line.
                lGetFunctions[ i ] = typeof( T )
                    .GetProperty( aMetaData[ i ].PropertyName );
            }


            SqlDataRecord[] lRecords = new SqlDataRecord[ aParameterValues.Count ];
            IEnumerator<T> lValueEnumerator = aParameterValues.GetEnumerator();
            for ( int i = 0; i < aParameterValues.Count && lValueEnumerator.MoveNext(); i++ )
            {
                SqlDataRecord iRecord = new SqlDataRecord( lMetaDataArray );
                for ( int j = 0; j < aMetaData.Length; j++ )
                {
                    // Get the value of the property for this column from the input values.
                    object lValue = lGetFunctions[ j ].GetValue( lValueEnumerator.Current );

                    // Now assign that value to the current ordinal of the current SqlDataRecord
                    // object using the methods reflectively fetched up above.
                    lSetFunctions[ j ].Invoke(
                        iRecord,
                        new object[] { j, lValue } );

                }
                lRecords[ i++ ] = iRecord;
            }

            lNewParameter.Value = lRecords;

            this.Add( lNewParameter );
        }
    }
}
