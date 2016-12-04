using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannon.DatabaseUtilities
{
    public class ColumnMetaDataBase
    {
        #region Properties
        /// <summary>
        /// The name of the column in the database table type.
        /// </summary>
        public string ColumnName   { get; private set; }
        /// <summary>
        /// The maximum length of values in this column. Can only be used for 
        /// string-type data.
        /// </summary>
        public int    ColumnLength { get; private set; }
        /// <summary>
        /// The name of the property on the type to access to retrieve a value
        /// for a column of a table type.
        /// </summary>
        public string PropertyName { get; private set; }
        #endregion

        #region Constructors
        public ColumnMetaDataBase(
            string aColumnName,
            int aColumnLength )
            : this( aColumnName, aColumnLength, null ) { }

        public ColumnMetaDataBase(
            string aColumnName  ,
            int    aColumnLength,
            string aPropertyName )
        {
            this.ColumnName   = aColumnName  ;
            this.ColumnLength = aColumnLength;
            this.PropertyName = aPropertyName;
        }
        #endregion
    }
}
