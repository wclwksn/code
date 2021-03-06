using ScriptCoreLib;
using ScriptCoreLib.Delegates;
using ScriptCoreLib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestNoZeroColumnHeaderNoScrollbarDateDataGrid
{
    /// <summary>
    /// Methods defined in this type can be used from JavaScript. The method calls will seamlessly be proxied to the server.
    /// </summary>
    public partial class ApplicationWebService : Component
    {

        #region DoEnterData
        public string reason;

        public
            //async 
            Task<DataTable> DoEnterData(

            [CallerFilePathAttribute] string CallerFilePath = null,
            [CallerLineNumberAttribute] int CallerLineNumber = 0,
            [CallerMemberNameAttribute] string CallerMemberName = null

            )
        {
            // X:\jsc.svn\examples\javascript\forms\Test\TestDataTableToJavascript\TestDataTableToJavascript\ApplicationWebService.cs

            var table = new DataTable { TableName = "DoEnterData " + new { reason }.ToString() };

            var column = new DataColumn();
            column.ColumnName = "Column 1";

            var column2 = new DataColumn();
            column2.ColumnName = "Column 2";

            table.Columns.Add(column);
            table.Columns.Add(column2);

            for (int i = 0; i < 32; i++)
            {
                var row = table.NewRow();

                row[column] = "#" + i;
                //row[column2] = new { reason, CallerMemberName, CallerLineNumber, CallerFilePath }.ToString();
                row[column2] = "John Doe, Canada | 600 USD";
                table.Rows.Add(row);
            }


            return table.ToTaskResult();
        }

        #endregion

    }
}
