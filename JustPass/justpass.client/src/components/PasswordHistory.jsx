import React, { useState } from 'react';
import './PasswordHistory.css';
import { decodePassword } from '../utils/decoder';

const PasswordHistory = ({ passwords, onClear }) => {
    const [showAll, setShowAll] = useState(false);

    if (!passwords || !Array.isArray(passwords) || passwords.length === 0) {
        return (
            <div className="password-history empty">
                <h3>История паролей</h3>
                <p>Здесь будут отображаться сгенерированные пароли</p>
            </div>
        );
    }

    const displayedPasswords = showAll ? passwords : passwords.slice(0, 5);

    const getSafetyLabel = (level) => {
        switch (level) {
            case 6: return 'Отличный';
            case 5: return 'Хороший';
            case 4: return 'Средний';
            default: return 'Слабый';
        }
    };

    // ✅ Безопасное декодирование с проверкой
    const safeDecode = (pwd) => {
        try {
            return decodePassword(pwd);
        } catch (error) {
            console.warn('Ошибка декодирования пароля, возвращаем как есть:', pwd);
            return pwd;
        }
    };

    return (
        <div className="password-history">
            <div className="history-header">
                <h3>История паролей ({passwords.length})</h3>
                <div className="history-actions">
                    {passwords.length > 5 && (
                        <button className="btn-toggle" onClick={() => setShowAll(!showAll)}>
                            {showAll ? 'Скрыть' : 'Показать все'}
                        </button>
                    )}
                    <button className="btn-clear" onClick={onClear}>
                        Очистить
                    </button>
                </div>
            </div>

            <div className="history-list">
                {displayedPasswords.map((item, index) => {
                    // ✅ Безопасное декодирование
                    const decodedPwd = safeDecode(item.pwd);
                    const displayId = passwords.length - (displayedPasswords.indexOf(item) + 1) + 1;

                    return (
                        <div key={item.id || index} className="history-item">
                            <span className="history-index">#{displayId}</span>
                            <code className="history-password">{decodedPwd}</code>
                            <span className="history-safety">{getSafetyLabel(item.safetyPwd)}</span>
                            <button
                                className="btn-copy-small"
                                onClick={() => navigator.clipboard.writeText(decodedPwd)}
                                title="Копировать"
                            >
                                📋
                            </button>
                        </div>
                    );
                })}
            </div>
        </div>
    );
};

export default PasswordHistory;