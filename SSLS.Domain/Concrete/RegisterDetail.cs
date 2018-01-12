using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSLS.Domain.Concrete
{
    public class RegisterDetail
    {
        //验证注解属性,用来验证用户数据
        [Required(ErrorMessage = "请设置账号名")]
        [Display(Name = "用户名称")]
        public string Name { get; set; }

        [Required(ErrorMessage = "请设置账号密码")]
        [Display(Name = "用户密码")]
        public string Password { get; set; }

        [Required(ErrorMessage = "请输入邮箱")]
        [Display(Name = "用户邮箱")]
        public string Email { get; set; }

    }
}
