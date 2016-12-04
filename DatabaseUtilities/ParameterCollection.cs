using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannon.DatabaseUtilities
{
    /// <summary>
    /// Container for a collection of parameters of some type. Contains methods
    /// for creating new parameters of various types.
    /// </summary>
    /// <remarks>
    /// This class can't accept a generic type parameter without preventing the
    /// instantiation of it directly. This is needed so the Database Engine class
    /// can create a default instance of a parameter collection without knowing
    /// what type of parameter to provide for ITS caller.
    /// </remarks>
    public class ParameterCollection
    {
        #region Members
        /// <summary>
        /// The set of parameters in this collection.
        /// </summary>
        private List<DbParameter> mParameters = new List<DbParameter>();
        #endregion

        /// <summary>
        /// Gets the parameters in this collection.
        /// </summary>
        public IReadOnlyCollection<DbParameter> Parameters
        {
            get
            {
                return mParameters.AsReadOnly();
            }
        }

        #region Constructors
        /// <summary>
        /// Initializes a new, empty instance of the 
        /// <see cref="ParameterCollection"/> class.
        /// </summary>
        public ParameterCollection()
        {
        }
        #endregion

        /// <summary>
        /// Used by derived classes to create a new instance of their 
        /// parameter type. Used so this class doesn't have to be generic.
        /// </summary>
        /// <returns></returns>
        protected virtual DbParameter CreateNewParameter()
        {
            throw new NotImplementedException( "Derived Classes need to implement this." );
        }

        #region Add

        /// <summary>
        /// Adds a parameter as a key/value pair.
        /// </summary>
        /// <typeparam name="T">
        /// The type of value to add.
        /// </typeparam>
        /// <param name="aParameterName">
        /// The name of the parameter as it appears in the SQL query.
        /// </param>
        /// <param name="aParameterValue">
        /// The value of the parameter.
        /// </param>
        public void Add<T>(
            string aParameterName,
            T aParameterValue )
        {
            DbParameter lNewParameter = CreateNewParameter();
            lNewParameter.ParameterName = aParameterName;
            lNewParameter.Value = aParameterValue;
            this.mParameters.Add( lNewParameter );
        }

        /// <summary>
        /// Adds the provided parameter to the collection.
        /// </summary>
        /// <param name="aParameter">
        /// The parameter object to add.
        /// </param>
        public void Add( DbParameter aParameter )
        {
            this.mParameters.Add( aParameter );
        }
        #endregion
    }
}
