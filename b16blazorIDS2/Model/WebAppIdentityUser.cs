using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
 
namespace b16blazorIDS2.Models
{
    public class WebAppIdentityUser : IdentityUser
    {


        /// <summary>
        /// Full name
        /// </summary>
        [PersonalData]
        public string? Name { get; set; }

        /// <summary>
        /// Birth Date
        /// </summary>
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

        [Display(Name = "邮政编码")]
        [PersonalData]
        public string? Zip { get; set; }

        [Display(Name = "区县")]
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
}
