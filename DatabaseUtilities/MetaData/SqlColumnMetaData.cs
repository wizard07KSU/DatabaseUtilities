using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannon.DatabaseUtilities.MetaData
{
    public class SqlColumnMetaData : ColumnMetaDataBase
    {
        public SqlColumnMetaData(
            string aColumnName,
            int aColumnLength )
            : base( aColumnName, aColumnLength, null ) { }

        public SqlColumnMetaData(
            string aColumnName,
            int aColumnLength,
            string aPropertyName )
            : base( aColumnName, aColumnLength, aPropertyName ) { }
    }
}
