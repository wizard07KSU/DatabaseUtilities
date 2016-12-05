# DatabaseUtilities
A collection of classes in .NET for operation on relational databases.

This library is written in C#.NET, and wraps the database functionality provided as part of .NET. The library is extensible, and allows for engines to be added for connecting to different types of relational databases. This library is intended to execute arbitrary SQL queries that may not fit very well into the architecture of existing object-oriented frameworks.

# How To Use
Define a class to represent your database, such as `MyCustomDatabase`, that will inherit from the Engine that you want to connect to. You will also need to define a constructor to pass the connection string to the database engine. If you are connecting to a database hosted on MSSQL Server, then you would have this class definition:

    using Cannon.DatabaseUtilities.Engines;
    
    public MyCustomDatabase : MssqlEngine
    {
        private static readonly string sConnectionString = 
            "SERVER=localhost\\SQLEXPRESS;DATABASE=MyCustomDatabase;TRUSTED_CONNECTION=True;";
        
        public MyCustomDatabase()
            : base( MyCustomDatabase.sConnectionString )
        {
        }
    }

And that's it. You now have a class to hold all functions for operating on your database, as well as for executing arbitrary SQL commands.

# Execute an SQL Query
There are two functions provided for executing SQL commands:
 * ExecuteQuery   - Used to execute a query that returns data from the database.
 * ExecuteCommand - Used to execute a command that returns a single result (e.g., DELETE returns number of rows affected, or a COUNT).

There are multiple overloads of both of these functions to allow the caller to pass as many or as few arguments as necessary.

## Examples
### ExecuteQuery
All overloads of ExecuteQuery use a callback function to convert rows in the returned record set into objects you can access in your code easily. Some of these examples will use lambda expressions for the callback functions; if you are unfamiliar with lambda expressions, you can read MSDN's documentation located [here](https://msdn.microsoft.com/en-us/library/bb397687.aspx).

#### Execute Query 1:
Basic ExecuteQuery usage:

    MyCustomDatabase lDatabase = new MyCustomDatabase();
    ICollection<int> lResults = lDatabase
        .ExecuteQuery(
            "SELECT int_column FROM table_name",
            reader => (int)reader[0] );

#### Execute Query 2:
ExecuteQuery usage with a transaction and parameters:

    MyCustomDatabase lDatabase = new MyCustomDatabase();
    SqlTransaction lTransaction = lDatabase.StartTransaction() as SqlTransaction;
    MssqlParameterCollection lParameters = new MssqlParameterCollection();
    lParameters.Add( "@current_date", DateTime.Now );

    ICollection<DateTime> lResults = lDatabase
        .ExecuteQuery(
            "SELECT run_dates FROM table_name WHERE run_dates > @current_date",
            reader => (DateTime)reader[0],
            lTransaction,
            lParameters );

#### Execute Query 3:
ExecuteQuery usage with multiple returned columns and a class. First, we define the class to contain the data:

    class MyCustomClass
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime Birthday { get; private set; }

        public MyCustomClass(
            string aFirstName,
            string aLastName,
            DateTime aBirthday )
        {
            this.FirstName = aFirstName;
            this.LastName = aLastName;
            this.Birthday = aBirthday;
        }

        public static MyCustomClass Converter( DbDataReader aReader )
        {
            return new MyCustomClass(
                aFirstName: (string)aReader[ 0 ],
                aLastName: (string)aReader[ 1 ],
                aBirthday: (DateTime)aReader[ 2 ] );
        }
    }

Now query the database:

    MyCustomDatabase lDatabase = new MyCustomDatabase();
    ICollection<MyCustomClass> lResults = lDatabase
        .ExecuteQuery(
            "SELECT int_column FROM table_name",
            MyCustomClass.Converter );

#### Execute Query 4
Select rows with nullable columns:

    MyCustomDatabase lDatabase = new MyCustomDatabase();
    ICollection<int> lResults = lDatabase
        .ExecuteQuery(
            "SELECT nullable_column FROM table_name",
            reader => CommonUtilities.Convert<int>( reader[0] ) );


### ExecuteCommand
This function works almost the same way that `ExecuteQuery` does, but `ExecuteCommand` only has at most 1 return value. Here are some examples.

#### Execute Command 1
Delete some rows:

    MyCustomDatabase lDatabase = new MyCustomDatabase();
    long lNumberRowsDeleted = lDatabase
        .ExecuteCommand( "DELETE FROM table_name" );
    Console.WriteLine( "Deleted {0} rows.", lNumberRowsDeleted );

#### Execute Command 2
Execute a count with a transaction:

    MyCustomDatabase lDatabase = new MyCustomDatabase();
    SqlTransaction lTransaction = lDatabase.StartTransaction() as SqlTransaction;
    long lTableNameCount = lDatabase
        .ExecuteCommand( "SELECT COUNT(*) FROM table_name", lTransaction );
    Console.WriteLine( "There are {0} rows in the table_name table..", lTableNameCount );

#### Execute Command 3
Alter a table:

    MyCustomDatabase lDatabase = new MyCustomDatabase();
    long lTableNameCount = lDatabase
        .ExecuteCommand( "ALTER TABLE table_name ADD new_column INT NOT NULL" );

