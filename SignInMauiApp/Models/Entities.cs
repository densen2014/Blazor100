using FreeSql.DataAnnotations;

namespace SignInMauiApp.Models;

//包含租户、用户、签到记录的数据模型

public class Tenant
{
    [Column(IsIdentity = true, IsPrimary = true)]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class User
{
    [Column(IsIdentity = true, IsPrimary = true)]
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int TenantId { get; set; }
    public bool IsAdmin { get; internal set; }
}

public class SignInRecord
{
    [Column(IsIdentity = true, IsPrimary = true)]
    public int Id { get; set; }
    public int UserId { get; set; }
    public int TenantId { get; set; }
    public DateTime SignInTime { get; set; }
}
