//--------------------------------------------------------------------------------
// 文件描述：表示Parameter的对象集合类
// 文件作者：罗刚
// 创建日期：
// 修改记录：
//--------------------------------------------------------------------------------
using System;
using System.Collections.Generic;

using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace CommonFoundation.MySql
{
    /// <summary>
    /// 表示Parameter的对象集合类
    /// </summary>
    public class Parameters
    {
        /// <summary>
        /// 参数项的集合
        /// </summary>
        private IList<Parameter> m_Entries = new List<Parameter>();

        /// <summary>
        /// 参数项的集合
        /// </summary>
        public IList<Parameter> Entries
        {
            get { return m_Entries; }
            set { m_Entries = value; }
        }

        /// <summary>
        /// 默认的构造函数
        /// </summary>
        public Parameters() { }

        /// <summary>
        /// 参数对象构造函数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="type">参数类型</param>
        /// <param name="value">参数值</param>
        public Parameters(string name, MySqlDbType type,int size, object value)
        {
            m_Entries.Add(new Parameter(name, type, size, value));
        }

        /// <summary>
        /// 增加一个参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="type">参数类型</param>
        /// <param name="value">参数值</param>
        public void AddParameter(string name, MySqlDbType type, int size, object value)
        {
            m_Entries.Add(new Parameter(name, type,size, value));
        }
    }
}
