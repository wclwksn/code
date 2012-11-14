﻿using ScriptCoreLib;
using ScriptCoreLib.PHP.Data;
using ScriptCoreLib.Shared.BCLImplementation.System.Data.Common;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace ScriptCoreLibJava.BCLImplementation.System.Data.SQLite
{
    [Script(Implements = typeof(global::System.Data.SQLite.SQLiteCommand))]
    internal class __SQLiteCommand : __DbCommand
    {
        // https://sites.google.com/a/jsc-solutions.net/backlog/knowledge-base/2012/20121001-solutionbuilderv1/20121014-gae-data
        // X:\jsc.svn\core\ScriptCoreLibAndroid\ScriptCoreLibAndroid\BCLImplementation\System\Data\SQLite\SQLiteCommand.cs
        // X:\jsc.svn\core\ScriptCoreLib\PHP\BCLImplementation\System\Data\SQLite\SQLiteCommand.cs

        __SQLiteConnection c;
        string sql;

        public java.sql.Statement InternalStatement;

        public __SQLiteCommand(string sql, SQLiteConnection c)
        {
            this.c = (__SQLiteConnection)(object)c;

            // http://dev.mysql.com/doc/refman/5.0/en/example-auto-increment.html
            // http://www.sqlite.org/autoinc.html

   
            this.sql = SQLiteToMySQLConversion.Convert( sql, this.c.InternalConnectionString.DataSource);

            try
            {
                this.InternalStatement = this.c.InternalConnection.createStatement();
            }
            catch
            {
                throw;
            }

        }

        public override int ExecuteNonQuery()
        {
            var value = default(int);

            try
            {
                value = this.InternalStatement.executeUpdate(this.sql);
                this.c.InternalLastInsertRowIdCommand = this;
            }
            catch
            {
                throw;
            }

            return value;
        }

        public SQLiteDataReader ExecuteReader()
        {
            var value = default(SQLiteDataReader);

            try
            {
                var r = this.InternalStatement.executeQuery(this.sql);

                value = (SQLiteDataReader)(object)new __SQLiteDataReader { InternalResultSet = r };
                this.c.InternalLastInsertRowIdCommand = this;
            }
            catch (Exception ex)
            {
                // The ResultSet can not be null for Statement.executeQuery.
                //                Caused by: java.sql.SQLException: The ResultSet can not be null for Statement.executeQuery.
                //at com.google.cloud.sql.jdbc.internal.Exceptions.newStatementExecuteQueryNullResultSetException(Exceptions.java:74)
                //at com.google.cloud.sql.jdbc.Statement.executeQuery(Statement.java:328)
                //at com.google.cloud.sql.jdbc.Statement.executeQuery(Statement.java:41)

                // https://sites.google.com/a/jsc-solutions.net/backlog/knowledge-base/2012/20121101/20121114-mysql
                // http://www.javaworld.com.tw/jute/post/view?bid=9&id=305281
                // crude workaround
                if (ex.StackTrace.Contains("com.google.cloud.sql.jdbc.internal.Exceptions.newStatementExecuteQueryNullResultSetException"))
                {
                    // no resultset.
                    value = (SQLiteDataReader)(object)new __SQLiteDataReader { InternalResultSet = null };
                }


                if (value == null)
                {
                    // implicit rethrow wont work. jsc forgot the name of the variable. 
                    //throw;
                    //                    [javac] V:\java\ScriptCoreLibJava\BCLImplementation\System\Data\SQLite\__SQLiteCommand.java:87: unreported exception java.lang.Throwable; must be caught or declared to be thrown
                    //[javac]                 throw exception3;
                    //[javac]                 ^
                    throw new InvalidOperationException(new { ex.Message, ex.StackTrace }.ToString());
                }
            }

            return value;
        }
    }
}
