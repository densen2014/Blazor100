// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BlazorOIDC.Server.Models;
using Densen.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlazorOIDC.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    private readonly ILogger<UserController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public UserController(ILogger<UserController> logger, SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
    }
    /// <summary>
    /// jwt登录测试
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(string username = "test@test.com", string password = "1qaz2wsx")
    {

        var signedUser = await _userManager.FindByEmailAsync(username);
        if (signedUser == null)
        {
            _logger.LogWarning("登录失败.");
            ModelState.AddModelError(string.Empty, "登录失败.请检查用户名或者密码.");
            return BadRequest("用户名密码错误");
        }

        var result = await _signInManager.PasswordSignInAsync(signedUser, password, false, lockoutOnFailure: false);

        var userId = await _userManager.GetUserIdAsync(signedUser);
        return Ok(new { result });
    }

    [Authorize]
    [HttpGet]
    public async Task<object> Get()
    {
        //获取用户Claim信息
        var userClaims = HttpContext.User.Claims.Select(it => $"{it.Type}:{it.Value}");
        var user = await _userManager.GetUserAsync(User);
        return new
        {
            user.UserName,
            roles = await _userManager.GetRolesAsync(user),
            userClaims
        };
    }

    [Authorize(Roles = nameof(AuthorizeRoles.Superuser))]
    [HttpGet("{id}")]
    public async Task<object> Get(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        return new
        {
            user.UserName,
            roles = await _userManager.GetRolesAsync(user)
        };
    } 
 

}

