export function addScript(key, elementId, dotnetRef, backgroundColor, controlSize) {
    if (!key || !elementId) {
        return;
    }

    let url = "https://maps.googleapis.com/maps/api/js?key=";
    let scriptsIncluded = false;

    let scriptTags = document.querySelectorAll('head > script');
    scriptTags.forEach(scriptTag => {
        if (scriptTag) {
            let srcAttribute = scriptTag.getAttribute('src');
            if (srcAttribute && srcAttribute.startsWith(url)) {
                scriptsIncluded = true;
                return true;
            }
        }
    });

    if (scriptsIncluded) { //防止多次向页面添加 JS 脚本.Prevent adding JS scripts to page multiple times.
        if (window.google) {
            initMaps(elementId); //页面已导航. Page was navigated
        }
        return true;
    }

    url = url + key + "&callback=initGoogleMaps&libraries=&v=weekly";
    let script = document.createElement('script');
    script.src = url;
    script.defer = true;
    document.head.appendChild(script);
    return false;
}

export function initMaps(elementId) {
    var latlng = new google.maps.LatLng(40.26982, -3.758269);
    var options = {
        zoom: 14, center: latlng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    var map = new google.maps.Map(elementId, options);
    console.log(map);
    return map;
}
//Remove elementId with data
function removeElementIdWithDotnetRef(dict, elementId) {
    for (let i = 0; i < dict.length; i++) {
        if (dict[i].key === elementId) {
            dict.splice(i, 1);
            break;
        }
    }
}
//Dispose
export function dispose(elementId) {
    if (elementId) {
        let mapWithDotnetRef = getElementIdWithDotnetRef(_mapsElementDict, elementId);
        mapWithDotnetRef.map = null;
        mapWithDotnetRef.ref = null;

        removeElementIdWithDotnetRef(_mapsElementDict, elementId);
    }
}