var VKPluginsLib = {
    showVKAd: function() {
        window.showVKAd();
    },

    showVKRewardedAd: function() {
        window.showVKRewardedAd();
    },

    vkSaveData: function(key, value) {
        var keyString = UTF8ToString(key);
        var valueString = UTF8ToString(value);
        window.vkSaveData(keyString, valueString);
    },

    vkLoadData: function(key) {
        var keyString = UTF8ToString(key);
        window.vkLoadData(keyString);
    },

    setStorageType: function(useLocal) {
        window.setStorageType(useLocal);
    },

    // Тестовая функция для проверки JSOnLoadComplete
    testLoadComplete: function() {
        if (typeof window.sendMessageToUnity === 'undefined') {
            window.sendMessageToUnity = function(gameObject, method, parameter) {
                try {
                    SendMessage(gameObject, method, parameter);
                } catch (e) {
                    console.error('Error sending message to Unity:', e);
                }
            };
        }
        window.sendMessageToUnity('VKPlatformSDK', 'JSOnLoadComplete', 'test data');
    }
};

mergeInto(LibraryManager.library, VKPluginsLib);
