// Декодирование из Base64 с проверкой
export const decodePassword = (encoded) => {
    if (!encoded) return '';

    // Проверяем, является ли строка корректным Base64
    try {
        // Пытаемся декодировать
        const decoded = atob(encoded);
        // Если декодировалось успешно, возвращаем результат
        return decoded;
    } catch (error) {
        // Если не получилось декодировать - значит это обычный текст
        console.log('Строка не является Base64, возвращаем как есть:', encoded);
        return encoded;
    }
};

// Кодирование в Base64
export const encodePassword = (plainText) => {
    if (!plainText) return '';
    try {
        return btoa(plainText);
    } catch (error) {
        console.error('Ошибка кодирования:', error);
        return plainText;
    }
};

// Проверка, является ли строка Base64
export const isBase64 = (str) => {
    if (!str) return false;
    try {
        return btoa(atob(str)) === str;
    } catch (error) {
        return false;
    }
};