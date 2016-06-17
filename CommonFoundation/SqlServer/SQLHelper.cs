using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//添加引用
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace CommonFoundation.SqlServer
{
    public class SQLHelper
    {
        #region 基本操作  
        //配置文件
        //<connectionStrings>
        //   <add name="con" connectionString="server=.;database=数据库名字;uid=sa;pwd=123;"/>
        // </connectionStrings>

       /// <summary>
       /// 连接字符串
       /// </summary>
        private static string strCon = "";//string.Format("server={0};database={1};User={2};password={3};Connect Timeout=1000", "192.168.168.228", "数据库名称", "sa", "123456");
        //初始化
        public SQLHelper(string str)
        {
            strCon = str;
        }
       /// <summary>
       /// 定义为私有private的方法  调用重载
       /// </summary>
       /// <param name="sql"></param>
       /// <param name="ct"></param>
       /// <param name="param"></param>
       /// <returns>返回DataTable</returns>
       private static DataTable ExecuteTable(string txt, CommandType ct, params SqlParameter[] param)
       {
           using (SqlConnection conn = new SqlConnection(strCon))
           {
               using (SqlDataAdapter sda = new SqlDataAdapter(txt, conn))
               {
                   sda.SelectCommand.CommandTimeout=1200;//设置响应时间
                   sda.SelectCommand.CommandType = ct;
                   sda.SelectCommand.Parameters.AddRange(param);

                   DataTable dt = new DataTable();
                   sda.Fill(dt);
                   return dt;
               }
           }
       }
       /// <summary>
       /// 执行sql语句
       /// </summary>
       /// <param name="sql">sql语句</param>
       /// <param name="param">可变参数</param>
       /// <returns>返回DataTable</returns>
       public static DataTable ExecuteTableSQL(string sql, params SqlParameter[] param)
       {
           return ExecuteTable(sql, CommandType.Text, param);
       }
       /// <summary>
       /// 执行存储过程
       /// </summary>
       /// <param name="sp"></param>
       /// <param name="param"></param>
       /// <returns></returns>
       public static DataTable ExecuteTableSP(string sp, params SqlParameter[] param)
       {
           return ExecuteTable(sp, CommandType.StoredProcedure, param);
       }
        /// <summary>
        /// 执行增删改
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">可变参数</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(string sql,params SqlParameter[] parameter)
       {
           using (SqlConnection con = new SqlConnection(strCon))
           {
                using(SqlCommand cmd=new SqlCommand(sql,con))
                {
                    cmd.Parameters.AddRange(parameter);
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
       }
        /// <summary>
        /// 返回首行首列即第一个单元格
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">可变参数</param>
        /// <returns>返回查询结果的第一行的第一列</returns>
         public static object ExecuteScalar(string sql,params SqlParameter[] parameter)
         {
             using (SqlConnection con = new SqlConnection(strCon))
             {
                 using (SqlCommand cmd =new SqlCommand(sql, con))
                 {
                     cmd.Parameters.AddRange(parameter);
                     con.Open();
                     return cmd.ExecuteScalar();
                 }
             }
         }
        /// <summary>
         /// 返回DataReader对象
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameter">可变参数</param>
         /// <returns>SqlDataReader对象</returns>
         public static SqlDataReader ExecuteReader(string sql, params SqlParameter[] parameter)
         {
             SqlConnection con = new SqlConnection(strCon);//不用using
             try
             {
                 using (SqlCommand cmd =new SqlCommand(sql, con))
                 {
                     cmd.Parameters.AddRange(parameter);
                     con.Open();
                     return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                 }
             }
             catch (Exception ex)
             {
                 // 如果在执行Reader方法的时候，出现异常，那么程序停止，关闭Connection的方法就不会执行
                 // 因此需要手动关闭
                 con.Dispose();
                 throw ex; // 异常不要吃掉，需要一步一步往上抛
             } 
         }
        /// <summary>
        /// 返回DataSet对象
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="ps">可变参数</param>
        /// <returns></returns>
         public static DataSet ExecuteDataSet(string sql, params SqlParameter[] parameter)
         {

             //public SqlDataAdapter(SqlCommand selectCommand) : this()
             //{
             //    this.SelectCommand = selectCommand; 反编译
             //}

             DataSet ds = new DataSet();
             using (SqlDataAdapter sda = new SqlDataAdapter(sql, strCon))//绑定sql语句和连接字符串
             {
                 sda.SelectCommand.Parameters.AddRange(parameter);//
                 sda.Fill(ds);
             }
             return ds;
         }
        #endregion

        #region 扩展方法
          /// <summary>
        /// 海量数据插入方法
        /// </summary>
        /// <param name="connectionString">目标连接字符</param>
        /// <param name="TableName">目标表</param>
        /// <param name="dt">源数据</param>
        private static void SqlBulkCopyByDatatable(string connectionString, string TableName, DataTable dt)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.UseInternalTransaction))
                {
                    try
                    {
                        sqlbulkcopy.DestinationTableName = TableName;
                        //一次批量的插入的数据量
                        sqlbulkcopy.BatchSize = 1000;
                        //超时之前操作完成所允许的秒数，如果超时则事务不会提交 ，数据将回滚，所有已复制的行都会从目标表中移除
                        sqlbulkcopy.BulkCopyTimeout = 60;
                        //设定NotifyAfter 属性，以便在每插入10000 条数据时，呼叫相应事件。 
                        sqlbulkcopy.NotifyAfter = 10000;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            sqlbulkcopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                        }
                        sqlbulkcopy.WriteToServer(dt);
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
        #endregion
    }
}
