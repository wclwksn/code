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

namespace CSSPrintMediaExperiment
{
    /// <summary>
    /// Methods defined in this type can be used from JavaScript. The method calls will seamlessly be proxied to the server.
    /// </summary>
    public class ApplicationWebService 
        : Component
        //System.Windows.Forms.BindingSource
    //,

        //ITypedList
    //System.ComponentModel.IBindingList
    {
        //  IBindingListView, IBindingList, IList, 
        // ICollection, IEnumerable, ITypedList, ICancelAddNew, 
        // ISupportInitializeNotification, ISupportInitialize, ICurrencyManagerProvider

        public ApplicationWebService()
        {

        }

        /// <summary>
        /// This Method is a javascript callable method.
        /// </summary>
        /// <param name="e">A parameter from javascript.</param>
        /// <param name="y">A callback to javascript.</param>
        public void WebMethod2(string e, Action<string> y)
        {
            // Send it back to the caller.
            y(e);
        }


        public string reason;

        public async Task<DataTable> DoEnterData(

            [CallerFilePathAttribute] string CallerFilePath = null,
            [CallerLineNumberAttribute] int CallerLineNumber = 0,
            [CallerMemberNameAttribute] string CallerMemberName = null

            )
        {
            // X:\jsc.svn\examples\javascript\forms\Test\TestDataTableToJavascript\TestDataTableToJavascript\ApplicationWebService.cs

            var table = new DataTable { TableName = "DoEnterData" };

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
                row[column2] = new { reason, CallerMemberName, CallerLineNumber, CallerFilePath }.ToString();
                table.Rows.Add(row);
            }


            return table;
        }



    }
}
