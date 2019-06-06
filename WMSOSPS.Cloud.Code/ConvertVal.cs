using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace WMSOSPS.Cloud.Code
{
    /// <summary>
    /// 值转换类
    /// </summary>
    public class ConvertVal
    {
        /// <summary>
        /// 判断当前值是否为空，不为空true 反false
        /// </summary>
        /// <param name="objVal"></param>
        /// <returns>不为空true 反false</returns>
        public static bool isNotNullVal(object objVal)
        {
            if (objVal != null)
            {
                if (objVal.GetType() == typeof(DataTable))
                {
                    return (objVal as DataTable).Rows.Count >= 1;
                }
                else if (objVal.GetType() == typeof(DataSet))
                {
                    return (objVal as DataSet) != null && (objVal as DataSet).Tables.Count >= 1;
                }
                else if (objVal.GetType() == typeof(DataRow))
                {
                    return (objVal as DataRow) != null;
                }
                else if (objVal.GetType() == typeof(DataRow[]))
                {
                    return (objVal as DataRow[]).Length >= 1;
                }
                else if (objVal.GetType() == typeof(Array))
                {
                    return (objVal as Array).Length >= 1;
                }
                else if (objVal.GetType() == typeof(string[]))
                {
                    return (objVal as string[]).Length >= 1;
                }
                return !string.IsNullOrEmpty(objVal.ToString());
            }
            return false;
        }
        /// <summary>
        /// 将当前值对象转换为int类型
        /// </summary>
        /// <param name="objVal"></param>
        /// <returns></returns>
        public static int GetValInt(object objVal)
        {
            int result = isNotNullVal(objVal) && int.TryParse(objVal.ToString(), out result) ? result : 0;
            return result;
        }
        /// <summary>
        /// 将当前值对象转换为byte类型
        /// </summary>
        /// <param name="objVal"></param>
        /// <returns></returns>
        public static byte GetValByte(object objVal)
        {
            byte result = (byte)(isNotNullVal(objVal) && byte.TryParse(objVal.ToString(), out result) ? result : 0);
            return result;
        }
        /// <summary>
        /// 将当前值对象转换为short类型
        /// </summary>
        /// <param name="objVal"></param>
        /// <returns></returns>
        public static short GetValShort(object objVal)
        {
            short result = (short)(isNotNullVal(objVal) && short.TryParse(objVal.ToString(), out result) ? result : 0);
            return result;
        }
        /// <summary>
        /// 将当前值对象转换为short类型
        /// </summary>
        /// <param name="objVal"></param>
        /// <returns></returns>
        public static float GetValFloat(object objVal)
        {
            float result = (short)(isNotNullVal(objVal) && float.TryParse(objVal.ToString(), out result) ? result : 0);
            return result;
        }
        /// <summary>
        /// 将当前值对象转换为string类型
        /// </summary>
        /// <param name="objVal"></param>
        /// <returns></returns>
        public static string GetValStr(object objVal)
        {
            return isNotNullVal(objVal) ? objVal.ToString() : "";
        }
        /// <summary>
        /// 将当前值对象转换为decimal类型
        /// </summary>
        /// <param name="objVal"></param>
        /// <returns></returns>
        public static decimal GetValDecimal(object objVal)
        {
            decimal result = isNotNullVal(objVal) && decimal.TryParse(objVal.ToString(), out result) ? result : 0;
            return result;
        }
        /// <summary>
        /// 将当前值对象转换为double类型
        /// </summary>
        /// <param name="objVal"></param>
        /// <returns></returns>
        public static double GetValDouble(object objVal)
        {
            double result = isNotNullVal(objVal) && double.TryParse(objVal.ToString(), out result) ? result : 0;
            return result;
        }
        /// <summary>
        /// 将当前值对象转换为DateTime类型
        /// </summary>
        /// <param name="objVal"></param>
        /// <returns></returns>
        public static DateTime GetValDateTime(object objVal)
        {
            DateTime result = isNotNullVal(objVal) && DateTime.TryParse(objVal.ToString(), out result) ? result : new DateTime(1900, 1, 1);
            return result;
        }
        /// <summary>
        /// 将当前值对象转换为bool类型
        /// </summary>
        /// <param name="objVal"></param>
        /// <returns></returns>
        public static bool GetValBool(object objVal)
        {
            bool result = isNotNullVal(objVal) && bool.TryParse(objVal.ToString(), out result) ? result : false;
            return result;
        }
        /// <summary>
        /// 将当前值对象转换为char类型
        /// </summary>
        /// <param name="objVal"></param>
        /// <returns></returns>
        public static char GetValChar(object objVal)
        {
            char result = isNotNullVal(objVal) && char.TryParse(objVal.ToString(), out result) ? result : ' ';
            return result;
        }

        #region 将table值转换出来为string
        /// <summary>
        /// 将table值转换出来为string
        /// </summary>
        /// <param name="objT">数据表</param>
        /// <param name="column">提取列的列名</param>
        /// <param name="type">当前列提取列的值类型</param>
        /// <returns></returns>
        public static string ConvertTableToStr(DataTable objT, string column, Type type)
        {
            List<object> lsid = new List<object>();
            if (ConvertVal.isNotNullVal(objT) && objT.Columns.Contains(column))
            {
                foreach (DataRow objR in objT.Rows)
                {
                    if (type == typeof(int))
                    {
                        int val = ConvertVal.GetValInt(objR[column]);
                        if (!lsid.Contains(val))
                        {
                            lsid.Add(val);
                        }
                    }
                    else
                    {
                        string val = ConvertVal.GetValStr(objR[column]);
                        if (!lsid.Contains(val))
                        {
                            lsid.Add("'" + val + "'");
                        }
                    }
                }
            }
            return string.Join(",", lsid.ToArray());
        }
        #endregion

        #region 电话号码加密
        /// <summary>
        /// 电话号码加密
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <returns></returns>
        public static string MobileEnCode(object mobile)
        {
            string m = GetValStr(mobile);
            return isNotNullVal(m) ? (m.Length >= 11 ? m.Substring(0, 3) + "****" + Regex.Replace(m, @"\d{2}([\d\-]+?)\d{4}", "") : "****" + Regex.Replace(m, @"\d{2}([\d\-]+?)\d{2}", "")) :
            (m.Length >= 11 ? m.Substring(0, 3) + "****" + Regex.Replace(m, @"\d{2}([\d\-]+?)\d{4}", "") : "****" + Regex.Replace(m, @"\d{2}([\d\-]+?)\d{2}", ""));
        }
        #endregion

        #region 人民币大小写转换
        #region RMBException
        /// <summary>
        /// 人民币转换的自定义错误
        /// </summary>
        public class RMBException : System.Exception
        {
            /// <summary>
            /// 自定义异常
            /// </summary>
            /// <param name="msg"></param>
            public RMBException(string msg)
                : base(msg)
            {
            }
        }
        #endregion
        #region 内部常量
        private static string RMBUppercase = "零壹贰叁肆伍陆柒捌玖";
        private static string RMBUnitChar = "元拾佰仟万拾佰仟亿拾佰仟兆拾佰仟万拾佰仟亿拾佰仟兆"; //人民币整数位对应的标志
        private const decimal MaxNumber = 9999999999999999999999999.99m;
        private const decimal MinNumber = -9999999999999999999999999.99m;
        private static char[] cDelim = { '.' }; //小数分隔标识
        #endregion

        #region 内部函数
        #region ConvertInt
        private static string ConvertInt(string intPart)
        {
            string buf = "";
            int length = intPart.Length;
            int curUnit = length;

            // 处理除个位以上的数据
            string tmpValue = "";                    // 记录当前数值的中文形式
            string tmpUnit = "";                    // 记录当前数值对应的中文单位
            int i;
            for (i = 0; i < length - 1; i++, curUnit--)
            {
                if (intPart[i] != '0')
                {
                    tmpValue = DigToCC(intPart[i]);
                    tmpUnit = GetUnit(curUnit - 1);
                }
                else
                {
                    // 如果当前的单位是"万、亿"，则需要把它记录下来
                    if ((curUnit - 1) % 4 == 0)
                    {
                        tmpValue = "";
                        tmpUnit = GetUnit(curUnit - 1);//

                    }
                    else
                    {
                        tmpUnit = "";

                        // 如果当前位是零，则需要判断它的下一位是否为零，再确定是否记录'零'
                        if (intPart[i + 1] != '0')
                        {

                            tmpValue = "零";

                        }
                        else
                        {
                            tmpValue = "";
                        }
                    }
                }
                buf += tmpValue + tmpUnit;
            }


            // 处理个位数据
            if (intPart[i] != '0')
                buf += DigToCC(intPart[i]);
            buf += "元";

            return CombinUnit(buf);
        }
        #endregion

        #region ConvertDecimal
        /// <summary>
        /// 小数部分的处理
        /// </summary>
        /// <param name="decPart">需要处理的小数部分</param>
        /// <returns></returns>
        private static string ConvertDecimal(string decPart)
        {
            string buf = "";
            int i = decPart.Length;

            //如果小数点后均为零
            if ((decPart == "0") || (decPart == "00"))
            {
                return "整";
            }

            if (decPart.Length > 1) //2位
            {
                if (decPart[0] == '0') //如果角位为零0.01
                {
                    buf = DigToCC(decPart[1]) + "分"; //此时不可能分位为零
                }
                else if (decPart[1] == '0')
                {
                    buf = DigToCC(decPart[0]) + "角整";
                }
                else
                {
                    buf = DigToCC(decPart[0]) + "角";
                    buf += DigToCC(decPart[1]) + "分";
                }
            }
            else //1位
            {
                buf += DigToCC(decPart[0]) + "角整";
            }
            return buf;
        }
        #endregion

        #region GetUnit
        /// <summary>
        /// 获取人民币中文形式的对应位置的单位标志
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private static string GetUnit(int n)
        {
            return RMBUnitChar[n].ToString();
        }
        #endregion

        #region DigToCC
        /// <summary>
        /// 数字转换为相应的中文字符 ( Digital To Chinese Char )
        /// </summary>
        /// <param name="c">以字符形式存储的数字</param>
        /// <returns></returns>
        private static string DigToCC(char c)
        {
            return RMBUppercase[c - '0'].ToString();
        }
        #endregion

        #region CheckNumberLimit
        private static void CheckNumberLimit(decimal number)
        {
            if ((number < MinNumber) || (number > MaxNumber))
            {
                throw new RMBException("超出可转换的范围");
            }
        }
        #endregion

        #region CombinUnit
        /// <summary>
        /// 合并兆亿万词
        /// </summary>
        /// <param name="rmb"></param>
        /// <returns></returns>
        private static string CombinUnit(string rmb)
        {
            if (rmb.Contains("兆亿万"))
            {
                return rmb.Replace("兆亿万", "兆");
            }
            if (rmb.Contains("亿万"))
            {
                return rmb.Replace("亿万", "亿");
            }
            if (rmb.Contains("兆亿"))
            {
                return rmb.Replace("兆亿", "兆");
            }
            return rmb;
        }
        #endregion
        #endregion

        #region DecimalToChinese <decimal>
        /// <summary>
        /// 转换成人民币大写形式
        /// </summary>
        /// <param name="number">金额</param>
        /// <returns>人民币大写</returns>
        public static string DecimalToChinese(decimal number)
        {
            bool NegativeFlag = false;
            decimal RMBNumber;

            CheckNumberLimit(number);

            RMBNumber = Math.Round(number, 2);    //将四舍五入取2位小数
            if (RMBNumber == 0)
            {
                return "零元整";
            }
            else if (RMBNumber < 0)  //如果是负数
            {
                NegativeFlag = true;
                RMBNumber = Math.Abs(RMBNumber);           //取绝对值
            }
            else
            {
                NegativeFlag = false;
            }

            string buf = "";                           // 存放返回结果
            string strDecPart = "";                    // 存放小数部分的处理结果
            string strIntPart = "";                    // 存放整数部分的处理结果
            string[] tmp = null;
            string strDigital = RMBNumber.ToString();

            tmp = strDigital.Split(cDelim, 2); // 将数据分为整数和小数部分


            if (RMBNumber >= 1m) // 大于1时才需要进行整数部分的转换
            {
                strIntPart = ConvertInt(tmp[0]);
            }

            if (tmp.Length > 1) //分解出了小数
            {
                strDecPart = ConvertDecimal(tmp[1]);
            }
            else  //没有小数肯定是为整
            {
                strDecPart = "整";
            }

            if (NegativeFlag == false) //是否负数
            {
                buf = strIntPart + strDecPart;
            }
            else
            {
                buf = "负" + strIntPart + strDecPart;
            }
            return buf;
        }
        #endregion
        #endregion

        /// <summary>
        /// 获取一个类指定的属性值
        /// </summary>
        /// <param name="info">object对象</param>
        /// <param name="field">属性名称</param>
        /// <returns></returns>
        public static object GetPropertyValue(object info, string field)
        {
            if (info == null) return null;
            Type t = info.GetType();
            IEnumerable<System.Reflection.PropertyInfo> property = from pi in t.GetProperties() where pi.Name.ToLower() == field.ToLower() select pi;
            return property.First().GetValue(info, null);
        }
        /// <summary>
        /// 复制对象
        /// </summary>
        private static Dictionary<string, object> _Dic = new Dictionary<string, object>();
        private static TOut TransExp<TIn, TOut>(TIn tIn)
        {
            string key = string.Format("trans_exp_{0}_{1}", typeof(TIn).FullName, typeof(TOut).FullName);
            if (!_Dic.ContainsKey(key))
            {
                ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
                List<MemberBinding> memberBindingList = new List<MemberBinding>();
                foreach (var item in typeof(TOut).GetProperties())
                {
                    if (!item.CanWrite)
                        continue;
                    MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
                    MemberBinding memberBinding = Expression.Bind(item, property);
                    memberBindingList.Add(memberBinding);
                }

                MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
                Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });
                Func<TIn, TOut> func = lambda.Compile();
                _Dic[key] = func;
            }
            return ((Func<TIn, TOut>)_Dic[key])(tIn);
        }

        #region 根据文字获取首字母
        /// <summary>  
        /// 获取拼音  
        /// </summary>  
        /// <param name="str"></param>  
        /// <returns></returns>  
        public static string GetPYString(string str)
        {
            string tempStr = "";
            foreach (char c in str)
            {
                if ((int)c >= 33 && (int)c <= 126)
                {//字母和符号原样保留     
                    tempStr += c.ToString();
                }
                else
                {//累加拼音声母     
                    tempStr += GetPYChar(c.ToString());
                }
            }
            return tempStr;
        }
        ///      
        /// 取单个字符的拼音声母     
        ///      
        /// 要转换的单个汉字     
        /// 拼音声母     
        public static string GetPYChar(string c)
        {
            byte[] array = new byte[2];
            array = System.Text.Encoding.Default.GetBytes(c);
            int i = (short)(array[0] - '\0') * 256 + ((short)(array[1] - '\0'));
            if (i < 0xB0A1) return "*";
            if (i < 0xB0C5) return "a";
            if (i < 0xB2C1) return "b";
            if (i < 0xB4EE) return "c";
            if (i < 0xB6EA) return "d";
            if (i < 0xB7A2) return "e";
            if (i < 0xB8C1) return "f";
            if (i < 0xB9FE) return "g";
            if (i < 0xBBF7) return "h";
            if (i < 0xBFA6) return "j";
            if (i < 0xC0AC) return "k";
            if (i < 0xC2E8) return "l";
            if (i < 0xC4C3) return "m";
            if (i < 0xC5B6) return "n";
            if (i < 0xC5BE) return "o";
            if (i < 0xC6DA) return "p";
            if (i < 0xC8BB) return "q";
            if (i < 0xC8F6) return "r";
            if (i < 0xCBFA) return "s";
            if (i < 0xCDDA) return "t";
            if (i < 0xCEF4) return "w";
            if (i < 0xD1B9) return "x";
            if (i < 0xD4D1) return "y";
            if (i < 0xD7FA) return "z";
            return "*";
        }
        #endregion
    }
}
