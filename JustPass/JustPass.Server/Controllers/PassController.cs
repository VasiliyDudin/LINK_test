using Microsoft.AspNetCore.Mvc;
using JustPass.Server.Models;
using JustPass.Server.Services;

namespace JustPass.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PassController : Controller
    {
        private readonly IServer _pwdService;

        public PassController(IServer pwdService)
        {
            _pwdService = pwdService;
        }

        /// <summary>
        /// Генерация пароля
        /// </summary>
        /// <returns></returns>
        // POST: api/Pass/Generate
        [HttpPost]
        public IActionResult Generate([FromBody] PassRequest request)
        {
            try
            {
                var result = _pwdService.GeneratedPWD(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Произошла ошибка при генерации пароля" });
            }
        }

        /// <summary>
        /// Получение истории
        /// </summary>
        /// <returns></returns>
        // GET api/Pass/GetHistory/
        [HttpGet]
        public IActionResult GetHistory()
        {
            try
            {
                var history = _pwdService.GetHistory();
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Произошла ошибка при получении истории" });
            }
        }

        /// <summary>
        /// Очистка истории
        /// </summary>
        /// <returns></returns>
        // DELETE: api/Pass/ClearHistory
        [HttpDelete]
        public IActionResult ClearHistory()
        {
            try
            {
                _pwdService.ClearHistory();
                return Ok(new { message = "История очищена" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Произошла ошибка при очистке истории" });
            }
        }

        /// <summary>
        /// Оценка надежности пароля
        /// </summary>
        /// <returns></returns>
        // POST: api/Pass/Safety
        [HttpPost]
        public IActionResult Safety([FromBody] string pwd)
        {
            if (_pwdService.ValidationVls(pwd))
            {
                return BadRequest(new { error = "Пароль невалиден" });
            }

            var strength = _pwdService.getSafety(pwd);

            return Ok(new
            {
                pwd,
                strength,
                requirements = new
                {
                    minLength = 6,
                    maxLength = 8,
                    requireLowercase = true,
                    requireUppercase = true,
                    requireDigits = true,
                    requireSpecial = true
                }
            });
        }

        /// <summary>
        /// Получение
        /// </summary>
        /// <returns></returns>
        // GET api/Pass/GetRequirements/
        [HttpGet]
        public IActionResult GetRequirements()
        {
            return Ok(new
            {
                minLength = 6,
                maxLength = 8,
                requireLowercase = true,
                requireUppercase = true,
                requireDigits = true,
                requireSpecial = true,
                specialChars = "!@#$%^&*"
            });
        }
    }
}
