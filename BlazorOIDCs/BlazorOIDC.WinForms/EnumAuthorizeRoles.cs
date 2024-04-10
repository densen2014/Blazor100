// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using System.ComponentModel;

namespace BlazorOIDC.WinForms;

public enum AuthorizeRoles
{
    [Description("管理员")]
    Admin,

    [Description("超级用户")]
    Superuser,

    [Description("普通用户")]
    User,

    [Description("角色管理")]
    R110,

    [Description("用户管理")]
    R120,

    [Description("权限管理")]
    R130,

    [Description("日志管理")]
    R140,
}

public enum IntToBool
{
    [Description("否")]
    False,

    [Description("是")]
    True,
}
