import React, { useState, useEffect } from 'react';
import PasswordGenerator from './components/PasswordGenerator';
import PasswordHistory from './components/PasswordHistory';
import './App.css';

const API_BASE_URL = 'https://localhost:7227';

function App() {
    const [passwords, setPasswords] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        loadHistory();
    }, []);

    const loadHistory = async () => {
        try {
            console.log(`Загрузка истории: ${API_BASE_URL}/api/Pass/gethistory`);
            const response = await fetch(`${API_BASE_URL}/api/Pass/gethistory`);
            if (response.ok) {
                const data = await response.json();
                console.log('Загружено записей:', data.length);
                setPasswords(data);
            }
        } catch (error) {
            console.error('Ошибка загрузки истории:', error);
        } finally {
            setLoading(false);
        }
    };

    // ✅ Оптимистичное обновление с добавлением в начало
    const addPassword = (newPassword) => {
        console.log('Добавление пароля в историю:', newPassword);
        // Добавляем новый пароль в начало списка (временный ID)
        const entry = {
            id: newPassword.id || Date.now(),
            pwd: newPassword.pwd,
            safetyPwd: newPassword.safetyPwd,
            dateGenerated: newPassword.dateGenerated || new Date().toISOString()
        };
        setPasswords(prev => [entry, ...prev]);
    };

    const clearHistory = async () => {
        try {
            await fetch(`${API_BASE_URL}/api/Pass/clearhistory`, {
                method: 'DELETE'
            });
            setPasswords([]);
        } catch (error) {
            console.error('Ошибка очистки истории:', error);
        }
    };

    return (
        <div className="app">
            <header className="app-header">
                <div className="logo-container">
                    <img
                        src="/lukoil-logo.png"
                        alt="ЛУКОЙЛ"
                        className="lukoil-logo"
                    />
                    <h1>JustPass</h1>
                    <p>Генератор паролей</p>
                </div>
            </header>

            <main className="app-main">
                <PasswordGenerator onPasswordGenerated={addPassword} />
                {loading ? (
                    <div className="loading">Загрузка истории...</div>
                ) : (
                    <PasswordHistory
                        passwords={passwords}
                        onClear={clearHistory}
                    />
                )}
            </main>

            <footer className="app-footer">
                <p>© 2026 JustPass · Все пароли хранятся только в вашем браузере</p>
            </footer>
        </div>
    );
}

export default App;