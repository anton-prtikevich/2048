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
};

mergeInto(LibraryManager.library, VKPluginsLib);
