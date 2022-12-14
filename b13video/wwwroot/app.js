function loadPlayer(id, options) {
    var player = videojs(id, options);

    player.ready(function () {
        var promise = player.play();

        if (promise !== undefined) {
            promise.then(function () {
                console.log('Autoplay started!');
            }).catch(function (error) {
                console.log('Autoplay was prevented.',error);
            });
        }
    });
}
