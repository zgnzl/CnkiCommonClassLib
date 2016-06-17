using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFoundation.Common
{
    public class ListOperate
    {
        /// <summary>
        ///  list集合去重
        /// </summary>
        /// <param name="list">list</param>
        /// <returns>返回逗号连接的字符串</returns>
        private static string GetJoinString(List<string> list)
        {
            IEnumerable<string> ee = list.Distinct();//去重
            return string.Join(",", ee);
        }
        /// <summary>
        /// 通过lambda表达式处理后返回新的list集合
        /// </summary>
        /// <param name="list">list</param>
        /// <param name="lambda">x => x.Length == 4</param>
        /// <returns></returns>
        public static IList<string>  deal(IList<string> list,Func<string,bool> lambda)
        {
            //string codeStr = "D050,B023,E059,B020,E067,A011,B024,I137,E066,B014,A006,C042,A002,D047,D046,C029,I136,C034,I140,B016,E074,A005,I135,B022,C039,B015,B021,C038,E080,I139,J145,E060,E072,E062,J147,B027,C031,I138,D052,C028,J152,E075,J158,D043,J149,F102,D048,F085,E057,J159,F099,E055,H131,J163,B018,I143,J150,J166,G106,C035,J160,C037,A010,G120,J157,J165,J151,B026,C030,E079,G117,A004,G111,H130,E053,B019,C040,H134,C041,H127,G108,F084,E056,F087,F091,E071,F092,G105,F081,F083,F082,A008,G109,I142,J167,H123,F096,F093,H133,H129,J164,F094,J168,G115,G107,E070,J155,I144,J146,F101,F088,H132,J162,A013,J156,G119,J153,G116,F090,G110,B025,F100,E065,B017,C032,A012,F104,G114,F095,F098,I141,J154,G112,G118,F103,F086,C033,J161,G113,E068,E073,E054,E076,E061,E063,D045,D051,C036,E069,E058,E064,H128,D049,F097,J148,A007,E077,A001,A009,D044,H126,H122,35,H125,F089,G121,A003,H124,E078,J1534,F08713,F08611,F08615,H1319,I13711,H131B,F0862,F08614,F08612,F08714,H12751,F085152,F08711,G10534,F098331,H1317,F08613,F0817,H12955,H13047,I138C21,C026,H1302,H1311,F0984,F0901,H1336,H1313,H1283,H131A,F08723,G150,H13015,H1334,F0885,46,I14322,F08392,53,F08617,F0902,F0818,J115,H13H,I1428,H13014,F08722,I1411,H1312,F09931,C,J1682,F0893,H1285,F0836,H1323,F08712,H1234,H1333,F0884,341,F157,I1413,H1318,J15714,E0545,E07424,F08426,J1,E0,87,8,662";
            //IList<string> result = codeStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);//243个
            return (IList<string>)list.Where(lambda);
        }
    }
}
