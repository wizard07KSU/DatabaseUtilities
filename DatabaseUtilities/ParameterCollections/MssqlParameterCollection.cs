using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannon.DatabaseUtilities.ParameterCollections
{
    public class MssqlParameterCollection : ParameterCollection
    {

        public MssqlParameterCollection()
        {
        }

        protected override DbParameter CreateNewParameter()
        {
            return new SqlParameter();
        }

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
