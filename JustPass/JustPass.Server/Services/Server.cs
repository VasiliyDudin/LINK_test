using JustPass.Server.Models;
using System.Security.Cryptography;
using System.Text;

namespace JustPass.Server.Services
{
    public class Server : IServer
    {

        private readonly List<PassHistory> _history = new();

        // Наборы символов
        private const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
        private const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Digits = "0123456789";
        private const string Special = "!@#$%^&*";

        private const int MinLength = 6;
        private const int MaxLength = 8;

        //Вся логика генереции и шифрования пароля. Этот метод вызывается из PassController
        public PassResponse GeneratedPWD(PassRequest request)
        {
            if (request.LengthPwd < MinLength || request.LengthPwd > MaxLength)
                throw new ArgumentException($"Длина пароля должна быть от {MinLength} до {MaxLength} символов");

            string pwd = string.Empty;
            int maxAtemp = 100; // Защита от бесконечного цикла
            int atemp = 0;

            while (atemp < maxAtemp)
            {
                pwd = Generated(request.LengthPwd);
                atemp++;

                if (ValidationVls(pwd))
                    break;
            }

            // Если после всех попыток пароль невалиден - выбрасываем исключение
            if (!ValidationVls(pwd))
                throw new InvalidOperationException($"Не удалось сгенерировать валидный пароль после {maxAtemp} попыток");

            int safetyPwd = getSafety(pwd); //Получаем оценку сложности пароля

            string encodedPwd = Convert.ToBase64String(Encoding.UTF8.GetBytes(pwd)); //Шифруем пароль

            _history.Add(new PassHistory
            {
                ID = _history.Count + 1,
                Pwd = encodedPwd,
                SafetyPwd = safetyPwd,
                DateGenerated = DateTime.UtcNow
            });

            return new PassResponse
            {
                Pwd = encodedPwd,
                LengthPwd = request.LengthPwd,
                SafetyPwd = safetyPwd,
                DateGenerated = DateTime.UtcNow
            };
        }

        //Валидация пароля
        public bool ValidationVls(string pwd)
        {
            bool result = true;

            result = (string.IsNullOrEmpty(pwd)
                     || pwd.Length < MinLength || pwd.Length > MaxLength
                     || !pwd.Any(char.IsLower)
                     || !pwd.Any(char.IsUpper) 
                     || !pwd.Any(char.IsDigit)
                     || !pwd.Any(c => !char.IsLetterOrDigit(c))) ? false : result;
            
            return result;
        }

        //Оценка сложности пароля
        public int getSafety(string pwd)
        {
            int result = 0;

            if (string.IsNullOrEmpty(pwd)) 
                return result;

            if (pwd.Length >= 7) result++;
            if (pwd.Length >= 8) result++;
            if (pwd.Any(char.IsLower)) result++;
            if (pwd.Any(char.IsUpper)) result++;
            if (pwd.Any(char.IsDigit)) result++;
            if (pwd.Any(c => !char.IsLetterOrDigit(c))) result++;

            int uniqueChars = pwd.Distinct().Count();
            double uniqueRatio = (double)uniqueChars / pwd.Length;
            if (uniqueRatio >= 0.7) result++;
            if (uniqueRatio >= 0.9) result++;

            return result;
        }

        //Получение списка сгенерированных паролей
        public List<PassHistory> GetHistory()
        {
            return _history;
        }

        //Удаление списка сгенерированных паролей
        public void ClearHistory()
        {
            _history.Clear();
        }

        //Основная логика генерации пароля
        string Generated(int length)
        {
            var allChars = LowerCase + UpperCase + Digits + Special;
            var charArray = allChars.ToCharArray();
            var result = new char[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                for (int i = 0; i < length; i++)
                {
                    var bytes = new byte[4];
                    rng.GetBytes(bytes);
                    var indx = BitConverter.ToUInt32(bytes, 0) % charArray.Length;
                    result[i] = charArray[indx];
                }
            }

            return new string(result);
        }
    }
}
