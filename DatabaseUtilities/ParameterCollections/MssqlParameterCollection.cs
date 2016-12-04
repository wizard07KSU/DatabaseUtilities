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
            { typeof( double  ), SqlDbType.Float    }
        };

        public void Add<T>(
            string aParameterName,
            T aParameterValue,
            int aParameterLength )
        {
            if ( !lSqlTypeLookup.ContainsKey( typeof( T ) ) )
            {
                throw new ArgumentException( "Type not recognized!", "aParameterValue" );
            }
            SqlParameter lNewParameter = new SqlParameter(
                aParameterName,
                lSqlTypeLookup[ typeof( T ) ], 
                aParameterLength );
            lNewParameter.Value = aParameterValue;
            this.Add( lNewParameter );
        }
    }
}
