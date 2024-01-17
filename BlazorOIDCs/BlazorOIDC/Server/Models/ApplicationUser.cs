// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BlazorOIDC.Server.Models;

public class ApplicationUser : IdentityUser
{

    /// <summary>
    /// Full name
    /// </summary>
    [Display(Name = "全名")]
    [PersonalData]
    public string? Name { get; set; }

    /// <summary>
    /// Birth Date
    /// </summary>
    [Display(Name = "生日")]
    [PersonalData]
    public DateTime? DOB { get; set; }

    [Display(Name = "识别码")]
    public string? UUID { get; set; }

    [Display(Name = "外联")]
    public string? provider { get; set; }

    [Display(Name = "税号")]
    [PersonalData]
    public string? TaxNumber { get; set; }

    [Display(Name = "街道地址")]
    [PersonalData]
    public string? Street { get; set; }

    [Display(Name = "邮编")]
    [PersonalData]
    public string? Zip { get; set; }

    [Display(Name = "县")]
    [PersonalData]
    public string? County { get; set; }

    [Display(Name = "城市")]
    [PersonalData]
    public string? City { get; set; }

    [Display(Name = "省份")]
    [PersonalData]
    public string? Province { get; set; }

    [Display(Name = "国家")]
    [PersonalData]
    public string? Country { get; set; }

    [Display(Name = "类型")]
    [PersonalData]
    public string? UserRole { get; set; }

}
