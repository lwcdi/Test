using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;

namespace UI.Web
{
    /// <summary>
    /// 功能：语言包类
    /// <para>作者：kangpingj</para>
    /// <para>日期：2016.11.26</para>
    /// </summary>
    public class Lang
    {
        /// <summary>
        /// 添加成功！
        /// </summary>
        public static string add_success { get; set; }

        /// <summary>
        /// 添加失败！
        /// </summary>
        public static string add_failure { get; set; }

        /// <summary>
        /// 取消
        /// </summary>
        public static string btn_cancel { get; set; }

        /// <summary>
        /// 确定
        /// </summary>
        public static string btn_ok { get; set; }

        /// <summary>
        /// 查询
        /// </summary>
        public static string btn_search { get; set; }

        /// <summary>
        /// 选择...
        /// </summary>
        public static string btn_select { get; set; }

        /// <summary>
        /// 确认要执行删除操作吗？
        /// </summary>
        public static string del_confirm { get; set; }

        /// <summary>
        /// 删除失败！
        /// </summary>
        public static string del_failure { get; set; }

        /// <summary>
        /// 删除成功！
        /// </summary>
        public static string del_success { get; set; }

        /// <summary>
        /// 多语言信息
        /// </summary>
        public static string dev_multilang_edit_title { get; set; }

        /// <summary>
        /// 编辑失败
        /// </summary>
        public static string edit_failure { get; set; }

        /// <summary>
        /// 编辑成功！
        /// </summary>
        public static string edit_success { get; set; }

        /// <summary>
        /// 您确定要退出本次登录吗?
        /// </summary>
        public static string exit_login_confirm { get; set; }

        /// <summary>
        /// 版权归XXXX公司所有2015-2020
        /// </summary>
        public static string footer_text { get; set; }

        /// <summary>
        /// 登录已过期，请重新登录
        /// </summary>
        public static string login_expire { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        public static string login_name { get; set; }

        /// <summary>
        /// 安全退出
        /// </summary>
        public static string logout { get; set; }

        /// <summary>
        /// 导航菜单
        /// </summary>
        public static string nav_menu { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        public static string new_pwd { get; set; }

        /// <summary>
        /// 否
        /// </summary>
        public static string no { get; set; }

        /// <summary>
        /// 请选择一行进行操作！
        /// </summary>
        public static string none_select_row { get; set; }

        /// <summary>
        /// 旧密码
        /// </summary>
        public static string old_pwd { get; set; }

        /// <summary>
        /// 操作失败！
        /// </summary>
        public static string operate_failure { get; set; }

        /// <summary>
        /// 操作成功！
        /// </summary>
        public static string operate_success { get; set; }

        /// <summary>
        /// 确认密码
        /// </summary>
        public static string pwd_confirm { get; set; }

        /// <summary>
        /// XXXX后台管理系统
        /// </summary>
        public static string system_name { get; set; }

        /// <summary>
        /// 系统提示
        /// </summary>
        public static string system_prompt { get; set; }

        /// <summary>
        /// 关闭
        /// </summary>
        public static string tab_close { get; set; }

        /// <summary>
        /// 全部关闭
        /// </summary>
        public static string tab_close_all { get; set; }

        /// <summary>
        /// 确认要关闭所有窗口吗?
        /// </summary>
        public static string tab_close_all_confirm { get; set; }

        /// <summary>
        /// 关闭左侧标签
        /// </summary>
        public static string tab_close_left { get; set; }

        /// <summary>
        /// 除此之外全部关闭
        /// </summary>
        public static string tab_close_other { get; set; }

        /// <summary>
        /// 关闭右侧标签
        /// </summary>
        public static string tab_close_right { get; set; }

        /// <summary>
        /// 退出
        /// </summary>
        public static string tab_exit { get; set; }

        /// <summary>
        /// 刷新
        /// </summary>
        public static string tab_refresh { get; set; }

        /// <summary>
        /// 修改密码
        /// </summary>
        public static string update_pwd { get; set; }

        /// <summary>
        /// 密码修改成功！请重新登录。
        /// </summary>
        public static string update_pwd_success { get; set; }

        /// <summary>
        /// 欢迎
        /// </summary>
        public static string welcome { get; set; }

        /// <summary>
        /// 欢迎使用
        /// </summary>
        public static string welcome_use { get; set; }

        /// <summary>
        /// 是
        /// </summary>
        public static string yes { get; set; }

        /// <summary>
        /// 用户名不存在！
        /// </summary>
        public static string bas_UserNameExist { get; set; }

    }
}

