using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonFoundation.Model
{
    [Serializable]
    public class ExpertBase
    {
        /// <summary>
        /// 专家代码
        /// </summary>
        public string ExpertCode { get; set; }
        /// <summary>
        /// 专家姓名
        /// </summary>
        public string ExpertName { get; set; }
        /// <summary>
        /// 专家定位
        /// </summary>
        public string Orientation { get; set; }
        /// <summary>
        /// 标准一级机构
        /// </summary>
        public string UnitLevel1 { get; set; }
        /// <summary>
        /// 标准一级机构代码
        /// </summary>
        public string UnitLevel1Code { get; set; }
        /// <summary>
        /// 研究领域168
        /// </summary>
        public string ResearchField168 { get; set; }
        /// <summary>
        /// 168专题代码
        /// </summary>
        public string ResearchField168Code { get; set; }
        /// <summary>
        /// 主研究领域168
        /// </summary>
        public string MainResearchField168 { get; set; }
        /// <summary>
        /// 主168专题代码
        /// </summary>
        public string MainResearchField168Code { get; set; }
        /// <summary>
        /// 学科
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 学科代码
        /// </summary>
        public string SubjectCode { get; set; }
        /// <summary>
        /// 主学科
        /// </summary>
        public string MainSubject { get; set; }
        /// <summary>
        /// 主学科代码
        /// </summary>
        public string MainSubjectCode { get; set; }
        /// <summary>
        /// 照片
        /// </summary>
        public string Pic { get; set; }
        /// <summary>
        /// 照片路径
        /// </summary>
        public string PicPath { get; set; }
        /// <summary>
        /// 文献篇数
        /// </summary>
        public int ArticleCnt { get; set; }
        /// <summary>
        /// 核心期刊发文数
        /// </summary>
        public int CoreCnt { get; set; }
        /// <summary>
        /// 本年新增文献数
        /// </summary>
        public int YearAddCnt { get; set; }
        /// <summary>
        /// 被引频次
        /// </summary>
        public int CitedCnt { get; set; }

        public int Cited { get; set; }
        /// <summary>
        /// H指数
        /// </summary>
        public int HIndex { get; set; }
        /// <summary>
        /// G指数
        /// </summary>
        public int GIndex { get; set; }
    }
}