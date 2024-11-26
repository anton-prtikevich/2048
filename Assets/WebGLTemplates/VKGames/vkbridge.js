(function() {
    // Проверяем доступность VK Bridge
    if (typeof vkBridge === 'undefined') {
        console.error('VK Bridge is not loaded');
        return;
    }

    // Инициализация VK Bridge
    vkBridge.send('VKWebAppInit')
        .then(function() {
            console.log('VK Bridge initialized');
            // После успешной инициализации получаем данные пользователя
            return vkBridge.send('VKWebAppGetUserInfo');
        })
        .then(function(data) {
            console.log('User data received:', data);
            if (window.environmentData) {
                window.environmentData.userInfo = data;
            }
            SendMessage('PlatformSDKManager/VKPlatformSDK', 'JSOnUserDataReceived', JSON.stringify(data));
            // После получения данных пользователя пробуем загрузить сохранения
            console.log('Attempting to load saved data...');
            window.vkLoadData('SavedGameData');
        })
        .catch(function(error) {
            console.error('VK Bridge error:', error);
        });

    // Методы для работы с рекламой
    window.showVKAd = function() {
        if (!window.environmentData || !window.environmentData.initialized) {
            console.error('VK SDK not initialized');
            return;
        }
        return vkBridge.send('VKWebAppShowNativeAds', {
            ad_format: 'interstitial'
        }).then(function(data) {
            console.log('Interstitial ad shown:', data);
            SendMessage('PlatformSDKManager/VKPlatformSDK', 'JSOnAdCompleted');
        }).catch(function(error) {
            console.error('Show interstitial ad error:', error);
            SendMessage('PlatformSDKManager/VKPlatformSDK', 'JSOnAdError', error.toString());
        });
    };

    window.showVKRewardedAd = function() {
        if (!window.environmentData || !window.environmentData.initialized) {
            console.error('VK SDK not initialized');
            return;
        }
        return vkBridge.send('VKWebAppShowNativeAds', {
            ad_format: 'reward'
        }).then(function(data) {
            console.log('Rewarded ad shown:', data);
            SendMessage('PlatformSDKManager/VKPlatformSDK', 'JSOnRewardedAdCompleted');
        }).catch(function(error) {
            console.error('Show rewarded ad error:', error);
            SendMessage('PlatformSDKManager/VKPlatformSDK', 'JSOnAdError', error.toString());
        });
    };

    // Методы для работы с данными
    window.vkSaveData = function(key, value) {
        if (!window.environmentData || !window.environmentData.initialized) {
            console.error('VK SDK not initialized');
            return;
        }
        console.log('Saving data:', { key, value });
        return vkBridge.send('VKWebAppStorageSet', {
            key: key,
            value: value
        }).then(function(data) {
            console.log('Data saved successfully:', { key, value });
            SendMessage('PlatformSDKManager/VKPlatformSDK', 'JSOnSaveComplete', key);
        }).catch(function(error) {
            console.error('Save data error:', error);
            SendMessage('PlatformSDKManager/VKPlatformSDK', 'JSOnSaveError', error.toString());
        });
    };

    window.vkLoadData = function(key) {
        if (!window.environmentData || !window.environmentData.initialized) {
            console.error('VK SDK not initialized');
            return;
        }
        console.log('Loading data for key:', key);
        return vkBridge.send('VKWebAppStorageGet', {
            keys: [key]
        }).then(function(data) {
            console.log('Data loaded:', data);
            if (data.keys && data.keys.length > 0) {
                const value = data.keys[0].value || '';
                SendMessage('PlatformSDKManager/VKPlatformSDK', 'JSOnLoadComplete', value);
            }
        }).catch(function(error) {
            console.error('Load data error:', error);
            SendMessage('PlatformSDKManager/VKPlatformSDK', 'JSOnLoadComplete', '');
        });
    };

    // Сообщаем о готовности VK Bridge
    console.log('VK Bridge methods initialized');
})();
