using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web
{
    /// <summary>
    /// Action类别
    /// </summary>
    public enum ActionType
    {
        /// <summary>
        /// 新增
        /// </summary>
        Add,
        /// <summary>
        /// 编辑
        /// </summary>
        Edit,
        /// <summary>
        /// 删除
        /// </summary>
        Delete
    }

    /// <summary>
    /// 删除状态
    /// </summary>
    public enum DeleteType
    {
        /// <summary>
        /// 删除
        /// </summary>
        Delete = 1,

        /// <summary>
        /// 未删除
        /// </summary>
        UnDelete = 0
    }

    //public enum Status
    //{
    //    /// <summary>
    //    /// 启用
    //    /// </summary>
    //    Enable = 1,

    //    /// <summary>
    //    /// 禁用
    //    /// </summary>
    //    UnEnable = 0
    //}

    ///// <summary>
    ///// 用户类型
    ///// </summary>
    //public enum UserType
    //{
    //    /// <summary>
    //    /// 省厅监管人员
    //    /// </summary>
    //    ProvinceUser = 1,
    //    /// <summary>
    //    /// 市局监管人员
    //    /// </summary>
    //    CityUser = 2,
    //    /// <summary>
    //    /// 检测站人员
    //    /// </summary>
    //    TestLineUser = 3
    //}

    /// <summary>
    /// 日志类别
    /// </summary>
    ///
    public enum LogType
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        UserLogin,
        /// <summary>
        /// 用户登出
        /// </summary>
        UserLogout,
        /// <summary>
        /// 新增操作
        /// </summary>
        InsertOperate,
        /// <summary>
        /// 修改操作
        /// </summary>
        UpdateOperate,
        /// <summary>
        /// 删除操作
        /// </summary>
        DeleteOperate,
        /// <summary>
        /// 打印操作
        /// </summary>
        PrintOperate,
        /// <summary>
        /// ORM层操作
        /// </summary>
        ORMBaseOperate
    }



    ///// <summary>
    ///// 审核状态
    ///// </summary>
    //public enum AuditResult
    //{
    //    /// <summary>
    //    /// 不允许审核
    //    /// </summary>
    //    UnAudit = 0,
    //    /// <summary>
    //    /// 待审核
    //    /// </summary>
    //    Auditing = 1,
    //    /// <summary>
    //    /// 已审核
    //    /// </summary>
    //    Audited = 2
    //}

    ///// <summary>
    ///// 车辆检测状态
    ///// </summary>
    //public enum VehTestingStatus
    //{
    //    /// <summary>
    //    /// 正在外检
    //    /// </summary>
    //    SurfaceTesting = 1,

    //    /// <summary>
    //    /// 正在检测尾气
    //    /// </summary>
    //    GasTesting = 2,

    //    /// <summary>
    //    /// 正在审核
    //    /// </summary>
    //    AuditTesting = 3,

    //    /// <summary>
    //    /// 审核完成，流程结束
    //    /// </summary>
    //    Checked = 4,

    //    /// <summary>
    //    /// 终止检测，提前结束流程
    //    /// </summary>
    //    EndTesting = 5,
    //}

    ///// <summary>
    ///// 审核方式
    ///// </summary>
    //public enum AuditMode
    //{
    //    /// <summary>
    //    /// 自动审核
    //    /// </summary>
    //    AutoAudit = 1,

    //    /// <summary>
    //    /// 人工审核
    //    /// </summary>
    //    ManualAudit = 2
    //}
    ///// <summary>
    ///// 车辆审核锁定状态
    ///// </summary>
    //public enum LockAppStatus
    //{
    //    /// <summary>
    //    /// 0-新建（该状态暂未用到，第一步就是申请）
    //    /// </summary>
    //    New = 0,
    //    /// <summary>
    //    /// 1-锁定申请中（缺省）
    //    /// </summary>
    //    LockApply = 1,
    //    /// <summary>
    //    /// 2-锁定已撤销
    //    /// </summary>
    //    LockRevoke = 2,
    //    /// <summary>
    //    /// 3-锁定通过
    //    /// </summary>
    //    LockPass = 3,
    //    /// <summary>
    //    /// 4-锁定不通过
    //    /// </summary>
    //    LockNoPass = 4,
    //    /// <summary>
    //    /// 5-解锁申请中
    //    /// </summary>
    //    UnLockApply = 5,
    //    /// <summary>
    //    /// 6-解锁撤销
    //    /// </summary>
    //    UnLockRevoke = 6,
    //    /// <summary>
    //    /// 7-解锁通过
    //    /// </summary>
    //    UnLockPass = 7,
    //    /// <summary>
    //    /// 8-解锁不通过
    //    /// </summary>
    //    UnLockNoPass = 8
    //}

    ///// <summary>
    ///// 检测方法（1-双怠速，2-稳态工况，3-简易瞬态，4-加载减速，5-不透光烟度,6-滤纸烟度）
    ///// </summary>
    //public enum TestType
    //{
    //    /// <summary>
    //    /// 点燃式双怠速
    //    /// </summary>
    //    TSI = 1,

    //    /// <summary>
    //    /// 点燃式稳态工况法
    //    /// </summary>
    //    ASM = 2,

    //    /// <summary>
    //    /// 点燃式简易瞬态工况法
    //    /// </summary>
    //    IM = 3,

    //    /// <summary>
    //    /// 压燃式加载减速法
    //    /// </summary>
    //    LD = 4,

    //    /// <summary>
    //    /// 压燃式自由加速（不透光烟度法）
    //    /// </summary>
    //    SNAP = 5,

    //    /// <summary>
    //    /// 压燃式自由加速（滤纸烟度法）
    //    /// </summary>
    //    FSI = 6,

    //    /// <summary>
    //    /// 新能源检测法
    //    /// </summary>
    //    NEW = 9
    //}

    ///// <summary>
    ///// 检测报告最后审核结果
    ///// </summary>
    //public enum Result
    //{
    //    /// <summary>
    //    /// 通过
    //    /// </summary>
    //    Pass = 1,

    //    /// <summary>
    //    /// 不通过
    //    /// </summary>
    //    NotPass = 0
    //}

    ///// <summary>
    ///// 打印结果
    ///// </summary>
    //public enum PrintType
    //{
    //    /// <summary>
    //    /// 已打印
    //    /// </summary>
    //    Print = 1,

    //    /// <summary>
    //    /// 未打印
    //    /// </summary>
    //    UnPrint = 0
    //}

    ///// <summary>
    ///// 通用审核状态
    ///// </summary>
    //public enum CheckStatus
    //{
    //    /// <summary>
    //    /// 0-新建（该状态暂未用到，第一步就是申请）
    //    /// </summary>
    //    New = 0,
    //    /// <summary>
    //    /// 1-申请中（缺省）
    //    /// </summary>
    //    Apply = 1,
    //    /// <summary>
    //    /// 2-已撤销
    //    /// </summary>
    //    Revoke = 2,
    //    /// <summary>
    //    /// 3-审核通过
    //    /// </summary>
    //    Pass = 3,
    //    /// <summary>
    //    /// 4-审核不通过
    //    /// </summary>
    //    NoPass = 4,
    //    /// <summary>
    //    /// 9-已删除
    //    /// </summary>
    //    Deleted = 9
    //}

    ///// <summary>
    ///// 违规处理状态
    ///// </summary>
    //public enum IlegalDealStatus
    //{
    //    /// <summary>
    //    /// 0-新建
    //    /// </summary>
    //    New = 0,
    //    /// <summary>
    //    /// 1-处理中（缺省）
    //    /// </summary>
    //    Apply = 1,
    //    /// <summary>
    //    /// 2-已撤销
    //    /// </summary>
    //    Revoke = 2,
    //    /// <summary>
    //    /// 3-违规
    //    /// </summary>
    //    Pass = 3,
    //    /// <summary>
    //    /// 4-无违规
    //    /// </summary>
    //    NoPass = 4
    //}

    ///// <summary>
    ///// 检验报告是否有效
    ///// </summary>
    //public enum ReportState
    //{
    //    /// <summary>
    //    /// 有效
    //    /// </summary>
    //    Effective = 1,

    //    /// <summary>
    //    /// 无效
    //    /// </summary>
    //    UnEffective = 0
    //}

    public enum PowerType
    {
        /// <summary>
        /// 管理员
        /// </summary>
        ProvincialUser = 1,

        /// <summary>
        /// 组长
        /// </summary>
        AreaUser = 2,

        /// <summary>
        /// 组员
        /// </summary>
        TestingStation = 3
    }

    //public enum DeviceType
    //{

    //    /// <summary>
    //    /// 底盘测功机
    //    /// </summary>
    //    Dynamometer,

    //    /// <summary>
    //    /// 气体分析仪
    //    /// </summary>
    //    Analyser,

    //    /// <summary>
    //    /// 流量计
    //    /// </summary>
    //    Flowmeter,

    //    /// <summary>
    //    /// 烟度计
    //    /// </summary>
    //    Smokemeter,

    //    /// <summary>
    //    /// 转速计
    //    /// </summary>
    //    Tachometer,

    //    /// <summary>
    //    /// 油温传感器
    //    /// </summary>
    //    Otsensor,

    //    /// <summary>
    //    /// 气象站
    //    /// </summary>
    //    Wstype,

    //    /// <summary>
    //    /// 工控机
    //    /// </summary>
    //    IPC,

    //}

    //public enum TestResult
    //{
    //    /// <summary>
    //    /// 通过
    //    /// </summary>
    //    Pass = 1,

    //    /// <summary>
    //    /// 不通过
    //    /// </summary>
    //    Unpass = 0
    //}

    //public enum DataSource
    //{
    //    /// <summary>
    //    /// 导入
    //    /// </summary>
    //    Import = 1,

    //    /// <summary>
    //    /// 手动录入
    //    /// </summary>
    //    Input = 2
    //}

    //public enum HCLType
    //{
    //    /// <summary>
    //    /// 三元催化
    //    /// </summary>
    //    TWC = 1,

    //    /// <summary>
    //    /// DPF
    //    /// </summary>
    //    DPF = 2,

    //    /// <summary>
    //    /// SCR
    //    /// </summary>
    //    SCR = 3,

    //    /// <summary>
    //    /// DOC
    //    /// </summary>
    //    DOC = 4,

    //    /// <summary>
    //    /// POC
    //    /// </summary>
    //    POC = 5,

    //    /// <summary>
    //    /// 其他
    //    /// </summary>
    //    Other = 6


    //}

    /// <summary>
    /// 巡查类型
    /// </summary>
    public enum PatrolType
    {
        /// <summary>
        /// 夜查
        /// </summary>
        YC,

        /// <summary>
        /// 日常巡查
        /// </summary>
        RCXC

    }

    public enum PatrolTheme
    {
        /// <summary>
        /// VOCs监管
        /// </summary>
        VOC,
        /// <summary>
        /// 重点道路管控
        /// </summary>
        ZDDL,
        /// <summary>
        /// 重点区域管控
        /// </summary>
        ZDQY,
        /// <summary>
        /// 无专题
        /// </summary>
        NONE,
        /// <summary>
        /// 空气站点巡查
        /// </summary>
        KQZXC
    }
}