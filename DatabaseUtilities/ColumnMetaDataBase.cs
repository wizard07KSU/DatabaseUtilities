using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannon.DatabaseUtilities
{
    /// <summary>
    /// Contains all the information necessary to initialize a single column
    /// of a Table Valued Parameter dataset.
    /// </summary>
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
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnMetaDataBase"/> 
        /// class.
        /// </summary>
        /// <param name="aColumnName">
        /// The name of the column in the database table type.
        /// </param>
        /// <param name="aColumnLength">
        /// The maximum length of the column.
        /// </param>
        public ColumnMetaDataBase(
            string aColumnName,
            int aColumnLength )
            : this( aColumnName, aColumnLength, null ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnMetaDataBase"/> 
        /// class.
        /// </summary>
        /// <param name="aColumnName">
        /// The name of the column in the database table type.
        /// </param>
        /// <param name="aColumnLength">
        /// The maximum length of the column.
        /// </param>
        /// <param name="aPropertyName">
        /// The name of the property on the values going into a Table Valued
        /// Parameter which holds the value for this column.
        /// </param>
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
