var map = null;
var containerid = null;
export function addScript(key, elementId, dotnetRef, backgroundColor, controlSize) {
    if (!key || !elementId) {
        return;
    }

    containerid = elementId;
    let url = "https://api.map.baidu.com/api?v=3.0&ak=";
    let scriptsIncluded = false;

    let scriptTags = document.querySelectorAll('body > script');
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
        initMapsG();
        return true;
    }

    url = url + key + "&callback=initMapsG";
    let script = document.createElement('script');
    script.src = url;
    document.body.appendChild(script);
    return false;
}
export function resetMaps(elementId) {
    initMaps(elementId);
}
function initMapsG() {
    initMaps(containerid);
}
function initMaps(elementId) {
    // 创建地图实例
    map = new BMap.Map(elementId, {
        coordsType: 5 // coordsType指定输入输出的坐标类型，3为gcj02坐标，5为bd0ll坐标，默认为5。指定完成后API将以指定的坐标类型处理您传入的坐标
    });
    // 创建点坐标
    var point = new BMap.Point(116.47496, 39.77856);
    // 初始化地图，设置中心点坐标和地图级别
    map.centerAndZoom(point, 15);
    //开启鼠标滚轮缩放
    map.enableScrollWheelZoom(true);
    map.addControl(new BMap.NavigationControl());
    map.addControl(new BMap.ScaleControl());
    map.addControl(new BMap.OverviewMapControl());
    map.addControl(new BMap.MapTypeControl());
    // 仅当设置城市信息时，MapTypeControl的切换功能才能可用
    map.setCurrentCity("北京");
}

export function geolocation(wrapper) {
    var geolocation = new BMap.Geolocation();
    // 开启SDK辅助定位
    geolocation.enableSDKLocation();
    geolocation.getCurrentPosition(function (r) {
        let geolocationitem;
        if (this.getStatus() == BMAP_STATUS_SUCCESS) {
            var mk = new BMap.Marker(r.point);
            map.addOverlay(mk);
            map.panTo(r.point);
            console.log('您的位置：' + r.point.lng + ',' + r.point.lat);
            let lng = r.point.lng;
            let lat = r.point.lat;
            geolocationitem= {
                "Longitude":lng,
                "Latitude" : lat,
                "Status": '您的位置：' + r.point.lng + ',' + r.point.lat
            };
        }
        else {
            geolocationitem= {
                "Longitude": 0,
                "Latitude": 0,
                "Status": 'failed' + this.getStatus()
            };
        }
        wrapper.invokeMethodAsync('GetResult', geolocationitem);
        return geolocationitem;
    });
}