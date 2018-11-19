using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using KB.DAL;

namespace KB.FUNC
{
    public class GlobalVal
    {
        #region 系统参数 System Parameter
        /// <summary>
        /// 系统名称
        /// </summary>
        public static string SystemName = "GREE KB";
        /// <summary>
        /// 系统最新版本号
        /// </summary>
        public static string SystemVersion;
        /// <summary>
        /// 系统启用时间，由数据库控制
        /// </summary>
        public static DateTime UseSystemDateTime;
        /// <summary>
        /// 是否可以使用本系统，由数据库控制
        /// </summary>
        public static Boolean UseSystem = true;
        /// <summary>
        /// 是否为测试平台，由数据库控制
        /// </summary>
        public static Boolean TestPlatform = true;
        /// <summary>
        /// 当前登陆系统失败次数
        /// </summary>
        public static int FailureNumber = 0;
        /// <summary>
        /// 最大许可登陆系统失败次数
        /// </summary>
        public static int MaxFailureNumber = 3;
        /// <summary>
        /// 升级维护时间
        /// </summary>
        public static DateTime UpgradeDateTime = DateTime.Parse("1900-1-1");
        /// <summary>
        /// 升级维护时间中在运行的用户，多少秒后T掉 约2-3分钟
        /// </summary>
        public static int UpgradeDateTimeTimeOut = 60;
        /// <summary>
        /// 工厂内对应数据库的索引
        /// </summary>
        public static int ConnIndex = 0;
        /// <summary>
        /// 数据库连接串
        /// </summary>
        public static string[,] DBConnectionString;
        /// <summary>
        /// 子模块退出时，是否显示退出信息，用于强制关闭时
        /// </summary>
        public static Boolean ShowCloseMessage = true;
        /// <summary>
        /// 用于协调子窗口与父窗口这间的信息提示
        /// </summary>
        public static Boolean ShowCloseChildMessageEd = false;
        #endregion

        #region 数据库连接信息 ConnectionString
        /// <summary>
        /// 数据库连接信息
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public static string ConnectionString(int fid)
        {
            try
            {
                //保护功能，防止测试状态下使用正式库                
                if (TestPlatform && (GlobalVal.ConnIndex <= 1) || (GlobalVal.ConnIndex == 2 && GlobalVal.UserInfo.FactoryID == 1))  //
                {
                    Func.ShowWarning("系统并没正式运作，不能连接正式库！");
                    System.Environment.Exit(0);
                    return "";
                }
                return DBConnectionString[fid, GlobalVal.ConnIndex].Trim();
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region 获取厂别信息 GetFactory
        /// <summary>
        /// 获取厂别信息
        /// </summary>
        /// <returns></returns>
        public static DataTable GetFactory()
        {
            try
            {
                DataRow row;
                DataTable tb = new DataTable();
                tb.Columns.Add("text");
                tb.Columns.Add("value");

                row = tb.NewRow();
                row["text"] = "珠海凯邦";
                row["value"] = "1";
                tb.Rows.Add(row);

                row = tb.NewRow();
                row["text"] = "郑州凯邦";
                row["value"] = "8";
                tb.Rows.Add(row);

                row = tb.NewRow();
                row["text"] = "重庆凯邦";
                row["value"] = "3";
                tb.Rows.Add(row);

                row = tb.NewRow();
                row["text"] = "合肥凯邦";
                row["value"] = "4";
                tb.Rows.Add(row);

                return tb;
            }
            catch
            {
                return new DataTable();
            }
        }
        /// <summary>
        /// 获取厂别信息
        /// </summary>
        /// <param name="i_para">厂别ID</param>
        /// <returns></returns>
        public static DataTable GetFactory(int i_para)
        {
            try
            {
                DataRow row;
                DataTable tb_ret;
                DataTable tb = GetFactory();
                tb_ret = new DataTable();
                tb_ret.Columns.Add("text");
                tb_ret.Columns.Add("value");
                for (int i = 0; i < tb.Rows.Count; i++)
                {
                    if (i_para == int.Parse(tb.Rows[i]["value"].ToString().Trim()))
                    {
                        row = tb_ret.NewRow();
                        row["text"] = tb.Rows[i]["text"].ToString().Trim();
                        row["value"] = tb.Rows[i]["value"].ToString().Trim();
                        tb_ret.Rows.Add(row);
                    }
                }
                return tb_ret;
            }
            catch
            {
                return new DataTable();
            }
        }
        #endregion

        #region 用户登陆信息 UserInfo
        /// <summary>
        /// 用户登陆信息
        /// </summary>
        public static class UserInfo
        {
            #region 登陆名 LoginName
            private static string loginName = "";
            /// <summary>
            /// 登陆名
            /// </summary>
            public static string LoginName
            {
                get { return loginName; }
                set { loginName = value; }
            }
            #endregion

            #region IP
            private static string ip = "";
            /// <summary>
            /// 电脑IP
            /// </summary>
            public static string IP
            {
                get { return ip; }
                set { ip = value; }
            }
            #endregion

            #region HostName
            private static string hostname = "";
            /// <summary>
            /// 计算机名字
            /// </summary>
            public static string HostName
            {
                get { return hostname; }
                set { hostname = value; }
            }
            #endregion

            #region DeptName
            private static string deptname = "";
            /// <summary>
            /// 部门全称
            /// </summary>
            public static string DeptName
            {
                get { return deptname; }
                set { deptname = value; }
            }
            #endregion

            #region 登陆时间 LoginDate
            private static DateTime loginDate;
            /// <summary>
            /// 登陆时间
            /// </summary>
            public static DateTime LoginDate
            {
                get { return loginDate; }
                set { loginDate = value; }
            }
            #endregion

            #region 厂别 FactoryID
            private static int factoryID = 0;
            /// <summary>
            /// 部门
            /// </summary>
            public static int FactoryID
            {
                get { return factoryID; }
                set { factoryID = value; }
            }
            #endregion

            #region 帐号级别 RAID
            private static int raid = 0;
            /// <summary>
            /// 帐号级别
            /// 0 是暂时
            /// 1 超级管理员
            /// 2 管理员
            /// 3 用户
            /// </summary>
            public static int RAID
            {
                get { return raid; }
                set { raid = value; }
            }
            #endregion

            #region DATA0073RKEY
            private static Decimal Data0073Rkey = 0;
            /// <summary>
            /// DATA0073RKEY
            /// </summary>
            public static Decimal DATA0073RKEY
            {
                get { return Data0073Rkey; }
                set { Data0073Rkey = value; }
            }
            #endregion

            #region DATA0005RKEY
            private static Decimal Data0005Rkey = 0;
            /// <summary>
            /// DATA0005RKEY
            /// </summary>
            public static Decimal DATA0005RKEY
            {
                get { return Data0005Rkey; }
                set { Data0005Rkey = value; }
            }
            #endregion

            #region FOUNDERPCB_LOGIN_LOG
            private static Decimal LogRkey = 0;
            /// <summary>
            /// LogRkey
            /// </summary>
            public static Decimal LOGRKEY
            {
                get { return LogRkey; }
                set { LogRkey = value; }
            }
            #endregion

            #region 域信息
            private static ADUserInfo ad;
            /// <summary>
            /// 域信息
            /// </summary>
            public static ADUserInfo AD
            {
                get { return ad; }
                set { ad = value; }
            }
            #endregion

            #region 线程数据连接库 Connection
            private static SqlConnection[] connection;
            /// <summary>
            /// 线程数据连接库
            /// </summary>
            public static SqlConnection[] Connection
            {
                get { return connection; }
                set { connection = value; }
            }
            #endregion

            #region FOUNDERPCB_USER
            private static Decimal userrkey = 0;
            /// <summary>
            /// UserRkey
            /// </summary>
            public static Decimal UserRkey
            {
                get { return userrkey; }
                set { userrkey = value; }
            }
            #endregion
        }
        #endregion

        #region 检查系统状态 CheckSystemState
        /// <summary>
        /// 检查系统状态 CheckSystemState
        /// </summary>
        /// <returns>返回true为维护模式</returns>
        public static Boolean CheckSystemState()
        {
            DBHelper DB = new DBHelper(0);
            try
            {
                DataTable tb;
                string s_SQL;

                s_SQL = "select * from FOUNDERPCB_SYSTEM_PARA with (nolock) where PARA_ID = 3";
                tb = DB.GetDataSet(s_SQL);
                if (tb.Rows.Count > 0)
                {
                    if (Func.IsDateTime(tb.Rows[0]["PARAMETER_VALUES"].ToString().Trim()))
                    {
                        GlobalVal.UpgradeDateTime = DateTime.Parse(tb.Rows[0]["PARAMETER_VALUES"].ToString().Trim());
                        // if (DateTime.Now >= GlobalVal.UpgradeDateTime)
                        DateTime nowdate = Func.GetNowDate();
                        TimeSpan th = nowdate - GlobalVal.UpgradeDateTime;
                        if (th.TotalMinutes < 15)
                        {
                            return true;//设定时间小于15分钟，进入维护模式
                        }
                        else
                        {
                            return false;//设定时间大于15分钟，系统自动解锁
                        }
                    }
                    else
                        GlobalVal.UpgradeDateTime = DateTime.Parse("1900-1-1");
                }
                else
                    GlobalVal.UpgradeDateTime = DateTime.Parse("1900-1-1");

                tb.Dispose();
                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                DB.CloseConnection();
            }
        }
        #endregion

        /// <summary>
        /// 根据厂别ID获取厂别代码
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static string getFactoryCodeByID(int ID)
        {
            string Code = "";
            switch (ID)
            {
                case 1:
                    Code = "ZH";
                    break;
                case 8:
                    Code = "ZZ";
                    break;
                case 4:
                    Code = "CQ";
                    break;
                case 5:
                    Code = "HF";
                    break;
            }

            return Code;
        }


        /// <summary>
        /// 根据厂别ID获取厂别名称
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static string getFactoryNameByID(int ID)
        {
            string Code = "";
            switch (ID)
            {
                case 1:
                    Code = "珠海凯邦电机制造有限公司";
                    break;
                case 8:
                    Code = "郑州凯邦电机制造有限公司";
                    break;
                case 4:
                    Code = "重庆凯邦电机制造有限公司";
                    break;
                case 5:
                    Code = "合肥凯邦电机制造有限公司";
                    break;

            }

            return Code;
        }
    }
}
