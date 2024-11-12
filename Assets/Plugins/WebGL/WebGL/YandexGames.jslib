mergeInto(LibraryManager.library, {
    ShowFullscreenAd: function() {
        try {
            ysdk.adv.showFullscreenAdv({
                callbacks: {
                    onClose: function() {
                        console.log('Interstitial ad closed');
                    },
                    onError: function(error) {
                        console.error('Interstitial ad error:', error);
                    }
                }
            });
        } catch (e) {
            console.error('ShowFullscreenAd error:', e);
        }
    },

    ShowRewardedAd: function(placement) {
        try {
            var placementString = UTF8ToString(placement);
            ysdk.adv.showRewardedVideo({
                callbacks: {
                    onClose: function(wasShown) {
                        if (typeof gameInstance !== "undefined") {
                            gameInstance.SendMessage('YandexGamesInitializer', 'OnRewardedAdClosed', wasShown);
                        }
                    },
                    onError: function(error) {
                        console.error('Rewarded ad error:', error);
                        if (typeof gameInstance !== "undefined") {
                            gameInstance.SendMessage('YandexGamesInitializer', 'OnRewardedAdClosed', false);
                        }
                    }
                }
            });
        } catch (e) {
            console.error('ShowRewardedAd error:', e);
            if (typeof gameInstance !== "undefined") {
                gameInstance.SendMessage('YandexGamesInitializer', 'OnRewardedAdClosed', false);
            }
        }
    },

    SaveExtern: function(data) {
        try {
            var dataString = UTF8ToString(data);
            if (typeof player !== "undefined" && player !== null) {
                player.setData({
                    saves: dataString
                }).then(() => {
                    console.log('Progress saved');
                }).catch(error => {
                    console.error('Save error:', error);
                });
            } else {
                console.warn('Player is not initialized');
            }
        } catch (e) {
            console.error('SaveExtern error:', e);
        }
    },

    LoadExtern: function() {
        try {
            if (typeof player !== "undefined" && player !== null) {
                player.getData(['saves']).then(data => {
                    if (data.saves) {
                        if (typeof gameInstance !== "undefined") {
                            gameInstance.SendMessage('YandexGamesInitializer', 'OnGameLoaded', data.saves);
                        }
                    } else {
                        if (typeof gameInstance !== "undefined") {
                            gameInstance.SendMessage('YandexGamesInitializer', 'OnGameLoaded', '');
                        }
                    }
                }).catch(error => {
                    console.error('Load error:', error);
                    if (typeof gameInstance !== "undefined") {
                        gameInstance.SendMessage('YandexGamesInitializer', 'OnGameLoaded', '');
                    }
                });
            } else {
                console.warn('Player is not initialized');
                if (typeof gameInstance !== "undefined") {
                    gameInstance.SendMessage('YandexGamesInitializer', 'OnGameLoaded', '');
                }
            }
        } catch (e) {
            console.error('LoadExtern error:', e);
            if (typeof gameInstance !== "undefined") {
                gameInstance.SendMessage('YandexGamesInitializer', 'OnGameLoaded', '');
            }
        }
    }
});
