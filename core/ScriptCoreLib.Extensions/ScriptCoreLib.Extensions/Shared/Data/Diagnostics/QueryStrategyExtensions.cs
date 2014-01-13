﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ScriptCoreLib.Shared.Data.Diagnostics
{


    public interface IQueryDescriptor
    {
        // this type has the reset state and how to make a connection

        string GetSelectAllColumnsText();

        string GetQualifiedTableName();

        Func<Func<SQLiteConnection, Task>, Task> GetWithConnection();

        // here we could ask for table stats?
    }

    public interface IQueryStrategy
    {
        // this state knows about reset state 

        IQueryDescriptor GetDescriptor();

        List<Action<QueryStrategyExtensions.CommandBuilderState>> GetCommandBuilder();


        // Stack<Apply>
    }

    public static class QueryStrategyExtensions
    {
        // SQLite.Linq reference implementation
        // when can we have immutable version?

        // http://msdn.microsoft.com/en-us/library/bb310804.aspx

        #region where
        // behave like StringBuilder where core data is mutable?
        public static void MutableWhere(IQueryStrategy that, LambdaExpression filter)
        {
            // to make it immutable, we would need to have Clone method
            // https://sites.google.com/a/jsc-solutions.net/backlog/knowledge-base/2014/201401/20140112/count
            // X:\jsc.svn\examples\javascript\Test\TestIQueryable\TestIQueryable\ApplicationWebService.cs


            // for op_Equals
            var body = ((BinaryExpression)filter.Body);

            // do we need to check our db schema or is reflection the schema for us?
            #region ColumnName
            var ColumnName = "";

            if (body.Left is MemberExpression)
            {
                ColumnName = ((MemberExpression)body.Left).Member.Name;
            }
            else if (body.Left is UnaryExpression)
            {
                ColumnName = ((MemberExpression)((UnaryExpression)body.Left).Operand).Member.Name;
            }
            else
            {
                Debugger.Break();
            }
            #endregion

            var r = default(object);

            if (body.Right is MemberExpression)
            {
                var f_Body_Right = (MemberExpression)body.Right;

                //+		(new System.Linq.Expressions.Expression.ConstantExpressionProxy((new System.Linq.Expressions.Expression.MemberExpressionProxy(f_Body_Right as System.Linq.Expressions.FieldExpression)).Expression as System.Linq.Expressions.ConstantExpression)).Value	{AppEngineWhereOperator.ApplicationWebService.}	object {AppEngineWhereOperator.ApplicationWebService.}

                var f_Body_Right_Expression = (ConstantExpression)f_Body_Right.Expression;

                var f_Body_Right_Expression_Value = f_Body_Right_Expression.Value;
                r = ((FieldInfo)f_Body_Right.Member).GetValue(f_Body_Right_Expression_Value);
            }
            else if (body.Left is UnaryExpression)
            {
                var f_Body_Right = (MemberExpression)((UnaryExpression)body.Right).Operand;


                var f_Body_Right_Expression = (ConstantExpression)f_Body_Right.Expression;

                var f_Body_Right_Expression_Value = f_Body_Right_Expression.Value;
                r = ((FieldInfo)f_Body_Right.Member).GetValue(f_Body_Right_Expression_Value);
            }
            else
            {
                Debugger.Break();
            }

            //Additional information: Unable to cast object of type 'System.Linq.Expressions.UnaryExpression' to type 'System.Linq.Expressions.MemberExpression'.

            //Additional information: Unable to cast object of type 'System.Linq.Expressions.UnaryExpression' to type 'System.Linq.Expressions.MemberExpression'.

            // http://stackoverflow.com/questions/9241607/whats-wrong-with-system-linq-expressions-logicalbinaryexpression-class
            //var f_Body_Left_as_MemberExpression = (MemberExpression)body.Left;
            //var f_Body_Right_as_ConstantExpression = (ConstantExpression)f_Body_as_BinaryExpression.Right;




            // for non op_Equals
            //var f_Body_as_MethodCallExpression = ((MethodCallExpression)f.Body);
            ////Console.WriteLine("IBook1Sheet1Queryable.Where");

            //var f_Body_Left_as_MemberExpression = (MemberExpression)f_Body_as_MethodCallExpression.Arguments[0];
            //var f_Body_Right_as_ConstantExpression = (ConstantExpression)f_Body_as_MethodCallExpression.Arguments[1];

            //Console.WriteLine("IBook1Sheet1Queryable.Where " + new { f_Body_as_MethodCallExpression.Method, f_Body_Left_as_MemberExpression.Member.Name, f_Body_Right_as_ConstantExpression.Value });
            Console.WriteLine("MutableWhere " + new
            {
                body.Method,

                //NodeType	Equal	System.Linq.Expressions.ExpressionType
                body.NodeType,


                ColumnName,
                Right = r
            });


            that.GetCommandBuilder().Add(
                state =>
                {
                    //MutableWhere { Method = Boolean op_Equality(System.String, System.String), Left = Goo, Right = Goo0 }

                    var n = "@arg" + state.ApplyParameter.Count;

                    state.WhereCommand = " where `" + ColumnName + "` = " + n;


                    state.ApplyParameter.Add(
                        c =>
                        {
                            // either the actualt command or the explain command?

                            c.Parameters.AddWithValue(n, r);
                        }
                    );
                }
            );
        }

        #endregion

        public static void MutableOrderBy(IQueryStrategy that, Expression selector)
        {
            Console.WriteLine("MutableOrderBy " + new { selector });
        }

        public static void MutableOrderByDescending(IQueryStrategy that, Expression selector)
        {
            Console.WriteLine("MutableOrderByDescending " + new { selector });
        }

        public static void MutableTake(IQueryStrategy that, long count)
        {
            Console.WriteLine("MutableTake " + new { count });
        }

        public class CommandBuilderState
        {
            public IQueryStrategy Strategy;


            public string SelectCommand;
            public string FromCommand;
            public string WhereCommand;

            public List<Action<SQLiteCommand>> ApplyParameter = new List<Action<SQLiteCommand>>();

            public override string ToString()
            {
                var w = new StringBuilder();

                w.AppendLine(this.SelectCommand);
                w.AppendLine(this.FromCommand);
                w.AppendLine(this.WhereCommand);

                //w.AppendLine(c.OrderByCommand);
                //w.AppendLine(c.LimitCommand);

                return w.ToString();

            }
        }

        public static long Count(IQueryStrategy Strategy)
        {
            return ((Task<long>)Strategy.GetDescriptor().GetWithConnection()(
                c =>
                {
                    var state = AsCommandBuilder(Strategy);

                    // override
                    state.SelectCommand = "select count(*)";

                    var cmd = new SQLiteCommand(state.ToString(), c);

                    foreach (var item in state.ApplyParameter)
                    {
                        item(cmd);
                    }

                    var s = new TaskCompletionSource<long>();
                    s.SetResult((long)cmd.ExecuteScalar());

                    return s.Task;
                }
            )).Result;
        }

        public static DataTable AsDataTable(IQueryStrategy Strategy)
        {
            return ((Task<DataTable>)Strategy.GetDescriptor().GetWithConnection()(
                c =>
                {
                    var state = AsCommandBuilder(Strategy);

                    var cmd = new SQLiteCommand(state.ToString(), c);

                    foreach (var item in state.ApplyParameter)
                    {
                        item(cmd);
                    }

                    var t = new DataTable();

                    var a = new SQLiteDataAdapter(cmd);

                    a.Fill(t);


                    var s = new TaskCompletionSource<DataTable>();
                    s.SetResult(t);

                    return s.Task;
                }
            )).Result;
        }

        public static CommandBuilderState AsCommandBuilder(IQueryStrategy Strategy)
        {
            // time to build the CommandText

            var state = new CommandBuilderState
            {
                Strategy = Strategy,

                // http://www.linkedin.com/groups/select-vs-selecting-all-columns-66097.S.206400776
                SelectCommand = Strategy.GetDescriptor().GetSelectAllColumnsText(),

                FromCommand = "from `" + Strategy.GetDescriptor().GetQualifiedTableName() + "`"
            };

            foreach (var item in Strategy.GetCommandBuilder())
            {
                item(state);
            }

            return state;
        }
    }
}
