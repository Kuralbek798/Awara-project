using System.Linq;

namespace AwaraIT.Training.Domain.Extensions
{
    /// <summary>
    /// Класс <c>StringExtensions</c> содержит методы расширения для работы со строками.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Обрезает строку до указанной длины.
        /// </summary>
        /// <param name="value">Исходная строка.</param>
        /// <param name="length">Максимальная длина строки.</param>
        /// <returns>Обрезанная строка, если исходная строка длиннее указанной длины; в противном случае возвращает исходную строку.</returns>
        public static string Crop(this string value, int length)
        {
            if (string.IsNullOrEmpty(value) || length < 0 || value.Length <= length)
            {
                return value;
            }
            return value.Substring(0, length);
        }

        /// <summary>
        /// Возвращает строку, содержащую только цифры из исходной строки.
        /// </summary>
        /// <param name="value">Исходная строка.</param>
        /// <returns>Строка, содержащая только цифры из исходной строки.</returns>
        public static string OnlyDigits(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return new string(value.Where(ch => char.IsDigit(ch)).ToArray());
        }
    }
}



