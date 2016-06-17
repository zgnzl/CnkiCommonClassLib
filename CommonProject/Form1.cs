using CommonFoundation.Common;
using CommonFoundation.KBase;
using CommonFoundation.Model;
using CommonFoundation.MongoDB;
using CommonFoundation.MySql;
using CommonFoundation.Redis;
using CommonFoundation.SqlServer;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            #region MogoDB
            //数据库连接字符串
            const string strconn = "mongodb://127.0.0.1:27017";
            //数据库名称
            const string dbName = "User";
            MongoDBService mogo = new MongoDBService(strconn,dbName);
            User user = new User();
            user.Name = "wzl";
            user.Age = 21;
            //mogo.Insert<User>(user);


            #endregion

            #region Redis
            RedisService redis = new RedisService("127.0.0.1:6379");
            //redis.SetAdd("name","wzl");
            //redis.Remove("name");
            #endregion

            #region Sql Server数据库
            //string sqlConnectionString = string.Format("server={0};database={1};User={2};password={3};Connect Timeout=1000", "192.168.168.228", "高被引_董", "sa", "sa@12345");
            //SQLHelper sqlHelper = new SQLHelper(sqlConnectionString);
            //DataTable dt = SQLHelper.ExecuteTableSQL("select top 10 *  FROM [高被引_董].[dbo].[SCHOLAR_TOOP200]");
            #endregion

            #region MySql数据库
            string mysqlConnectionString = string.Format("Database={0};Data Source={1};User Id={2};Password={3};CharSet=utf8;port={4}", "hyd", "127.0.0.1", "root", "", "3306");
            MySqlService mysqlHelper = new MySqlService(mysqlConnectionString);
            DataTable mydt = mysqlHelper.ExecuteReader("SELECT * FROM USER    LIMIT   20");
            #endregion

            #region Kbase数据库
            string ftsServerIP = "192.168.107.62";
            int ftsServerPort = 4567;
            string ftsServerUserName = "DBOWN";
            string ftsServerPsw = "";
            KBaseHelper kBaseHelper = new KBaseHelper(ftsServerIP, ftsServerPort, ftsServerUserName, ftsServerPsw);
            //TPI.RecordSet rs = kBaseHelper.GetRecord("SELECT top 10 * FROM Cnki_Expert_Baseinfo  ");
            //int count = rs.GetCount();
            //DataTable dtRs = kBaseHelper.RecordSetToDataTable(rs);
            #endregion

            #region 导出excel文档
            //ExcelHelper.CreatExcel(@"C:\Users\wangzl\Desktop\picture\", dtRs);
            #endregion

            #region 分页返回list对象集合   命中搜索关键字并打上标记
            string sql = string.Format("SELECT * FROM Cnki_Expert_Baseinfo WHERE 专家姓名=王");
            string handler = "";
            int totalCount = 0;
            //List<ExpertBase> list = kBaseHelper.PagerToList<ExpertBase>(ref handler, sql, 0, 10, PackingToObject_ScholarBase, out totalCount, true);
            #endregion

            #region 返回+号连接的字符串
            string joinStr1 = kBaseHelper.GetKeyToOrFormatFromRS(string.Format("SELECT 学者排重前编码 FROM EXPERT_ONE_MANY where  学者编码={0} ", "00023837"), "学者排重前编码");
            #endregion

            #region 构造多值查询
            string funJoin = "0002;T1686;F0;T4537;T1275;";
            //string resultFun = KBaseHelper.ConvertMutiValToOrFormat(funJoin);
            #endregion

            #region 获取单值
            string sqlSingle = string.Format("SELECT * FROM Cnki_Expert_Baseinfo WHERE 专家编号=05966721");
            string resSingel = kBaseHelper.GetSingleValFormRS(sqlSingle, "专家姓名");
            #endregion

            #region 获取单值列表
            string sqlSingleList = string.Format("SELECT * FROM Cnki_Expert_Baseinfo WHERE 专家编号='0597654*'");
            List<string> resSingelList = kBaseHelper.GetSingleValListFormRS(sqlSingleList, "专家姓名");
            #endregion

        }

        #region 包装成baseinfo
        /// <summary>
        /// 包装对象
        /// </summary>
        /// <returns></returns>
        public static ExpertBase PackingToObject_ScholarBase(TPI.RecordSet rs)
        {
            return new ExpertBase()
            {
                ExpertCode = rs.GetValue("专家编号"),
                ExpertName = rs.GetValue("专家姓名"),
                ArticleCnt = CommonFunc.FormatInt(rs.GetValue("文献篇数")),
                CitedCnt = CommonFunc.FormatInt(rs.GetValue("被引频次")),
            };
        }
        #endregion
    }
}
