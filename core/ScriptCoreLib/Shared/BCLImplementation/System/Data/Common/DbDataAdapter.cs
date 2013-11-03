﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace ScriptCoreLib.Shared.BCLImplementation.System.Data.Common
{
    [Script(Implements = typeof(global::System.Data.Common.DbDataAdapter))]
    public abstract class __DbDataAdapter
    {
        public DbCommand InternalSelectCommand;
        public DbCommand SelectCommand
        {
            get
            {
                return InternalSelectCommand;
            }
            set
            {
                InternalSelectCommand = value;
            }
        }


        public int Fill(DataTable dataTable)
        {
            //  The number of rows successfully added 

            // x:\jsc.svn\examples\javascript\appengine\webnotificationsviadataadapter\webnotificationsviadataadapter\applicationwebservice.cs
            // x:\jsc.svn\examples\javascript\dropfileintosqlite\dropfileintosqlite\schema\table1.cs
            // X:\jsc.svn\examples\javascript\forms\SQLiteConsoleExperiment\SQLiteConsoleExperiment\ApplicationWebService.cs

            //Caused by: java.lang.NullPointerException
            //        at ScriptCoreLib.Shared.BCLImplementation.System.Data.Common.__DbDataAdapter.Fill(__DbDataAdapter.java:53)

            var data = default(DataTable);

            //Console.WriteLine("Fill enter " + new { this.InternalSelectCommand });

            var reader = this.InternalSelectCommand.ExecuteReader();
            //Console.WriteLine("Fill reader " + new { reader });



            // https://sites.google.com/a/jsc-solutions.net/backlog/knowledge-base/2013/201311/20131101
            if (reader != null)
                using (reader)
                {

                    while (reader.Read())
                    {
                        //Console.WriteLine("Fill reader Read");
                        if (data == null)
                        {
                            data = FillColumns(reader);
                        }


                        //Console.WriteLine("Fill NewRow ");
                        var row = data.NewRow();
                        data.Rows.Add(row);

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var n = reader.GetName(i);
                            var ft = reader.GetFieldType(i);

                            // X:\jsc.svn\core\ScriptCoreLibJava\BCLImplementation\System\Data\SQLite\SQLiteDataReader.cs
                            var value = reader[n];

                            Console.WriteLine("Fill " + new { n, ft, value  });

                            row[n] = value;
                        }
                    }
                }


            if (data == null)
                return 0;

            Console.WriteLine("Fill Merge ");
            dataTable.Merge(data);

            return data.Rows.Count;
        }

        private static DataTable FillColumns(DbDataReader reader)
        {
            var xdata = new DataTable();

            Console.WriteLine("Fill " + new { reader.FieldCount });

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columName = reader.GetName(i);

                Console.WriteLine(
                    new { columName }
                    );

                xdata.Columns.Add(columName);
            }
            return xdata;
        }
    }
}