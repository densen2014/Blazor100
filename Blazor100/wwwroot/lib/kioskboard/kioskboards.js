export function addScript(url) {
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

    if (scriptsIncluded) { 
        return true;
    }

    let script = document.createElement('script');
    script.src = url;
    document.head.appendChild(script);
    return false;

}

export function init(className, option) {
    console.info(className, option);
    KioskBoard.run('.' + className, option);
    return true;
}