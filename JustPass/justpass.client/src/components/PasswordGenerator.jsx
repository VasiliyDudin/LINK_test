import React, { useState } from 'react';
import './PasswordGenerator.css';
import { decodePassword } from '../utils/decoder';

const API_BASE_URL = 'http://localhost:7227';

const PasswordGenerator = ({ onPasswordGenerated }) => {
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [copied, setCopied] = useState(false);
    const [error, setError] = useState('');
    const [safetyLevel, setSafetyLevel] = useState(0);

    const generatePassword = async () => {
        setLoading(true);
        setError('');

        try {
            const randomLength = Math.floor(Math.random() * (8 - 6 + 1)) + 6;

            const response = await fetch('/api/Pass/generate', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    lengthPwd: randomLength
                })
            });

            const data = await response.json();

            if (!response.ok) {
                throw new Error(data.error || `Ошибка ${response.status}`);
            }

            console.log('Получен закодированный пароль от сервера:', data.pwd);

            // ✅ Безопасное декодирование
            let decodedPassword;
            try {
                decodedPassword = decodePassword(data.pwd);
            } catch (error) {
                console.warn('Ошибка декодирования, используем как есть:', data.pwd);
                decodedPassword = data.pwd;
            }

            console.log('Декодированный пароль:', decodedPassword);

            setPassword(decodedPassword);
            setSafetyLevel(data.safetyPwd);

            onPasswordGenerated(data);
            setCopied(false);
        } catch (err) {
            console.error('Ошибка:', err);
            setError(err.message || 'Произошла ошибка при генерации пароля');
        } finally {
            setLoading(false);
        }
    };

    const copyToClipboard = () => {
        if (!password) return;
        navigator.clipboard.writeText(password);
        setCopied(true);
        setTimeout(() => setCopied(false), 2000);
    };

    const getSafetyLabel = (level) => {
        switch (level) {
            case 6: return '🔒 Отличный';
            case 5: return '🔐 Хороший';
            case 4: return '🔐 Средний';
            default: return '⚠️ Слабый';
        }
    };

    const getSafetyColor = (level) => {
        switch (level) {
            case 6: return '#48bb78';
            case 5: return '#48bb78';
            case 4: return '#ed8936';
            default: return '#fc8181';
        }
    };

    return (
        <div className="password-generator">
            <h2>Генератор паролей</h2>

            <div className="password-display">
                <input
                    type="text"
                    value={password}
                    readOnly
                    placeholder="Нажмите «Сгенерировать»"
                />
                <button
                    className="btn-copy"
                    onClick={copyToClipboard}
                    disabled={!password || loading}
                >
                    {copied ? '✓' : '📋'}
                </button>
            </div>

            {password && (
                <div className="safety-indicator">
                    <span style={{ color: getSafetyColor(safetyLevel) }}>
                        {getSafetyLabel(safetyLevel)}
                    </span>
                    <div className="safety-bar">
                        <div
                            className="safety-bar-fill"
                            style={{
                                width: `${(safetyLevel / 6) * 100}%`,
                                background: getSafetyColor(safetyLevel)
                            }}
                        />
                    </div>
                </div>
            )}

            {error && (
                <div className="password-error">
                    ❌ {error}
                </div>
            )}

            <button
                className="btn-generate"
                onClick={generatePassword}
                disabled={loading}
            >
                {loading ? 'Генерация...' : 'Сгенерировать пароль'}
            </button>
        </div>
    );
};

export default PasswordGenerator;