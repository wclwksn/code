﻿using ScriptCoreLib;
using ScriptCoreLib.Extensions;
using ScriptCoreLib.PHP.Data;
using ScriptCoreLib.Shared.BCLImplementation.System.Data.Common;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using ScriptCoreLibJava.Extensions;

namespace ScriptCoreLibJava.BCLImplementation.System.Data.SQLite
{
    [Script(Implements = typeof(global::System.Data.SQLite.SQLiteCommand))]
    internal class __SQLiteCommand : __DbCommand
    {
        // https://sites.google.com/a/jsc-solutions.net/backlog/knowledge-base/2012/20121001-solutionbuilderv1/20121014-gae-data
        // X:\jsc.svn\core\ScriptCoreLibAndroid\ScriptCoreLibAndroid\BCLImplementation\System\Data\SQLite\SQLiteCommand.cs
        // X:\jsc.svn\core\ScriptCoreLib\PHP\BCLImplementation\System\Data\SQLite\SQLiteCommand.cs

        __SQLiteConnection c;

        public override string CommandText
        {
            get;
            set;
        }


        public java.sql.Statement InternalStatement;
        public java.sql.PreparedStatement InternalPreparedStatement;

        public __SQLiteCommand(string sql, SQLiteConnection c)
        {
            this.c = (__SQLiteConnection)(object)c;

            // http://dev.mysql.com/doc/refman/5.0/en/example-auto-increment.html
            // http://www.sqlite.org/autoinc.html


            this.CommandText = SQLiteToMySQLConversion.Convert(sql, this.c.InternalConnectionString.DataSource);

            this.InternalParameters = new __SQLiteParameterCollection { };
            this.Parameters = (SQLiteParameterCollection)(object)this.InternalParameters;

        }

        private void InternalCreateStatement()
        {
            if (this.InternalStatement != null)
                return;

            try
            {
                // http://www.javaworld.com/javaworld/jw-04-2007/jw-04-jdbc.html
                if (this.InternalParameters.InternalParameters.Count > 0)
                {
                    var sql = this.CommandText;

                    //Console.WriteLine("we have InternalParameters for " + sql);

                    var parameters = this.InternalParameters.InternalParameters;

                    var index =
                       from p in parameters
                       from i in this.CommandText.GetIndecies(p.ParameterName)
                       orderby i
                       select new { p, i };



                    foreach (var p in parameters)
                    {
                        // java seems to like indexed parameters instead
                        sql = sql.Replace(p.ParameterName, "?");
                    }

                    Console.WriteLine("InternalCreateStatement " + new { sql });
                    this.InternalPreparedStatement = this.c.InternalConnection.prepareStatement(sql);

                    var c = 0;
                    foreach (var item in index)
                    {
                        c++;

                        if (item.p.Value == null)
                        {
                            this.InternalPreparedStatement.setObject(c, null);
                        }
                        else if (item.p.Value is int)
                            this.InternalPreparedStatement.setInt(c, (int)item.p.Value);
                        else if (item.p.Value is long)
                            this.InternalPreparedStatement.setLong(c, (long)item.p.Value);
                        else if (item.p.Value is string)
                            this.InternalPreparedStatement.setString(c, (string)item.p.Value);
                        else
                        {
                            var message = "InternalCreateStatement, what to do with this? " + new
                            {
                                this.CommandText,
                                item,
                                type = item.GetType()
                            };

                            throw new InvalidOperationException(message);
                        }
                    }

                    // add values

                    this.InternalStatement = this.InternalPreparedStatement;
                }

                if (this.InternalStatement == null)
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

            InternalCreateStatement();

            try
            {
                if (this.InternalPreparedStatement != null)
                    value = this.InternalPreparedStatement.executeUpdate();
                else
                    value = this.InternalStatement.executeUpdate(this.CommandText);

                this.c.InternalLastInsertRowIdCommand = this;
            }
            catch
            {
                throw;
            }

            return value;
        }

        public __SQLiteParameterCollection InternalParameters;
        public SQLiteParameterCollection Parameters { get; set; }


        public override global::System.Data.Common.DbDataReader __DbCommand_ExecuteReader()
        {
            // tested by
            // X:\jsc.svn\examples\javascript\appengine\WebNotificationsViaDataAdapter\WebNotificationsViaDataAdapter\Schema\FooTable.cs

            return this.ExecuteReader();
        }

        public new SQLiteDataReader ExecuteReader()
        {
            var value = default(SQLiteDataReader);

            InternalCreateStatement();

            try
            {
                var r = default(java.sql.ResultSet);

                if (this.InternalPreparedStatement != null)
                    r = this.InternalPreparedStatement.executeQuery();
                else
                    r = this.InternalStatement.executeQuery(this.CommandText);

                value = (SQLiteDataReader)(object)new __SQLiteDataReader { InternalResultSet = r, InternalCommand = this };
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
