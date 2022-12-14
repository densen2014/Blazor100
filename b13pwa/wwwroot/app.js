var player = null;

export function loadPlayer(instance, id, options) {
    console.log('player id', id);
    player = videojs(id, options);

    player.ready(function () {
       console.log('player.ready');
       var promise = player.play();

        if (promise !== undefined) {
            promise.then(function () {
                console.log('Autoplay started!');
            }).catch(function (error) {
                console.log('Autoplay was prevented.', error);
                instance.invokeMethodAsync('GetError', 'Autoplay was prevented.'+ error);
            });
        }
        instance.invokeMethodAsync('GetInit', true);
    });

    return false;
}

export function destroy(id) {
    if (undefined !== player && null !== player) {
        player = null;
        console.log('destroy');
    }
}