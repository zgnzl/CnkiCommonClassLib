//--------------------------------------------------------------------------------
// 文件描述：表示Parameter的对象类
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
    /// 表示Parameter的对象类
    /// </summary>
    public class Parameter
    {
        private string m_Name;
        private MySqlDbType m_DBType;
        private object m_Value;
        private int m_Size;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Parameter() { }

        /// <summary>
        /// 参数对象构造函数（输入参数）
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="type">参数类型</param>
        /// <param name="value">参数值</param>
        //public Parameter(string name, MySqlDbType type,int size, object value) : this( name, type,size,value) { }
        public Parameter(string name, MySqlDbType type, int size, object value)
        {
            this.m_Name = name;
            this.m_DBType = type;
            this.m_Value = value;
            this.m_Size = size;
        }

        /// <summary>
        /// 参数名
        /// </summary>
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        /// <summary>
        /// 参数类型
        /// </summary>
        public MySqlDbType DBType
        {
            get { return m_DBType; }
            set { m_DBType = value; }
        }

        /// <summary>
        /// 输出参数的大小
        /// </summary>
        public int Size
        {
            get { return m_Size; }
            set { m_Size = value; }
        }

        /// <summary>
        /// 参数值
        /// </summary>
        public object Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }


    }
}
