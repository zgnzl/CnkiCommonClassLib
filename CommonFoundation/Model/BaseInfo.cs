using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonFoundation.Model
{
    [Serializable]
    public class BaseInfo:ExpertBase
    {
        /// <summary>
        /// 导师
        /// </summary>
        public string Advisor { get; set; }
        /// <summary>
        /// 当前单位
        /// </summary>
        public string CurrentUnit { get; set; }
        /// <summary>
        /// 二级机构
        /// </summary>
        public string UnitLevel2 { get; set; }
        /// <summary>
        /// 二级机构代码
        /// </summary>
        public string UnitLevel2Code { get; set; }
        /// <summary>
        /// 研究领域
        /// </summary>
        public string ResearchField { get; set; }
        /// <summary>
        /// 专长
        /// </summary>
        public string Speciality { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public string Birthday { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 民族
        /// </summary>
        public string Nation { get; set; }
        /// <summary>
        /// 籍贯
        /// </summary>
        public string BirthPlace { get; set; }
        /// <summary>
        /// 目前居住地
        /// </summary>
        public string CurrentPlace { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 党派
        /// </summary>
        public string Party { get; set; }
        /// <summary>
        /// 当前学历
        /// </summary>
        public string CurrentEducation { get; set; }
        /// <summary>
        /// 当前学位
        /// </summary>
        public string CurrentDegree { get; set; }
        /// <summary>
        /// 当前职务
        /// </summary>
        public string CurrentJob { get; set; }
        /// <summary>
        /// 当前标准职称
        /// </summary>
        public string CurrentTitleStandard { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 即时通讯方式
        /// </summary>
        public string NetCommunication { get; set; }
        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 第一作者篇数
        /// </summary>
        public int FirstAuthorCnt { get; set; }
        /// <summary>
        /// SCI发文数
        /// </summary>
        public int SCICnt { get; set; }
        /// <summary>
        /// EI发文数
        /// </summary>
        public int EICnt { get; set; }
        /// <summary>
        /// 引证报告发文数
        /// </summary>
        public int CitedReportCnt { get; set; }
        /// <summary>
        /// CSSCI发文数
        /// </summary>
        public int CSSCICnt { get; set; }
        /// <summary>
        /// 科技期刊发文数
        /// </summary>
        public int TechnologyJournalCnt { get; set; }
        /// <summary>
        /// 基金代码
        /// </summary>
        public string FundCode { get; set; }
        /// <summary>
        /// 基金发文数
        /// </summary>
        public int FundArticleCnt { get; set; }
        /// <summary>
        /// 平均被引次数
        /// </summary>
        public double AverageCitedCnt { get; set; }
        /// <summary>
        /// 下载频次
        /// </summary>
        public int DownloadCnt { get; set; }
        /// <summary>
        /// 科研项目数
        /// </summary>
        public int ProjectCnt { get; set; }
        /// <summary>
        /// 专利文献数
        /// </summary>
        public int PatentArticleCnt { get; set; }
        /// <summary>
        /// 科技成果文献数
        /// </summary>
        public int AchievementArticleCnt { get; set; }

        /// <summary>
        /// 原始基本信息
        /// </summary>
        public List<string> BaseInfoRaw { get; set; }
        /// <summary>
        /// 当前职称
        /// </summary>
        public string CurrentTitle { get; set; }
        /// <summary>
        /// 原始研究方向
        /// </summary>
        public List<string> ResearchFieldRaw { get; set; }
        /// <summary>
        /// 原始联系信息
        /// </summary>
        public List<string> ContactRaw { get; set; }
        /// <summary>
        /// 原始履历
        /// </summary>
        public List<string> ResumeRaw { get; set; }
        /// <summary>
        /// 原始荣誉
        /// </summary>
        public List<string> HonourRaw { get; set; }
        /// <summary>
        /// 原始个人综合报告
        /// </summary>
        public List<string> MediaReportRaw { get; set; }
        /// <summary>
        /// 原始个人综合报告连接
        /// </summary>
        public string MediaReportRawLink { get; set; }
        /// <summary>
        /// 原始著作
        /// </summary>
        public List<string> BooksRaw { get; set; }
        /// <summary>
        /// 原始成果
        /// </summary>
        public List<string> AchievementRaw { get; set; }
        /// <summary>
        /// 原始科研项目
        /// </summary>
        public List<string> ProjectRaw { get; set; }
        /// <summary>
        /// 原始社会活动
        /// </summary>
        public List<string> ActivityRaw { get; set; }
        /// <summary>
        /// 原始报告
        /// </summary>
        public List<string> ReportRaw { get; set; }
        /// <summary>
        /// 原始成果荣誉综述
        /// </summary>
        public List<string> SummaryRaw { get; set; }
        /// <summary>
        /// 原始论文
        /// </summary>
        public List<string> JournalRaw { get; set; }
    }
}