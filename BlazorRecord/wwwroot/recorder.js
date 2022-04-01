export function streamToDotNet() {
    return new Uint8Array(10000000);
}
export function streamToDotNet2(wrapperc) {
    const data = new Uint8Array([0x45, 0x76, 0x65, 0x72, 0x79, 0x74, 0x68, 0x69,
        0x6e, 0x67, 0x27, 0x73, 0x20, 0x73, 0x68, 0x69, 0x6e, 0x79, 0x2c,
        0x20, 0x43, 0x61, 0x70, 0x74, 0x69, 0x61, 0x6e, 0x2e, 0x20, 0x4e,
        0x6f, 0x74, 0x20, 0x74, 0x6f, 0x20, 0x66, 0x72, 0x65, 0x74, 0x2e]);
    wrapperc.invokeMethodAsync('ReceiveByteArray', data)
        .then(str => {
            alert(str);
        });

}
export function init(wrapperc, element, alertText) {
    let allStream;
    let sendTimer;
    document.querySelector('#start').onclick = function () {
        if (navigator.mediaDevices && navigator.mediaDevices.getDisplayMedia) {
            navigator.mediaDevices.getDisplayMedia({
                video: true,
                audio: false
            }).then((stream) => {
                allStream = stream;
                document.querySelector('#player').srcObject = stream;
            }).catch((err) => {
                console.error(err);
            })
        } else {
            alert('不支持这个特性');
        }
    }

    //WebRTC也是可以从摄像头中获取视频流的，只需要将getDisplayMedia替换成getUserMedia就可以了
    document.querySelector('#startcam').onclick = function () {
        if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
            navigator.mediaDevices.getUserMedia({
                video: true,
                audio: false
            }).then((stream) => {
                allStream = stream;
                document.querySelector('#player').srcObject = stream;
            }).catch((err) => {
                console.error(err);
            })
        } else {
            alert('不支持这个特性');
        }
    }


    document.querySelector('#stop').onclick = function () {
        if (mediaRecorder && mediaRecorder.state == "recording") {
            sendTimer=null;
            mediaRecorder.stop(); 
        }
    }

    let buf = [];
    let mediaRecorder;

    //录制到的数据数据
    let allChunks = [];

    document.querySelector('#record').onclick = function () {
        // 约束视频格式
        const options = {
            mimeType: 'video/webm;codecs=vp8'
        }

        // 判断是否是支持的mimeType格式
        if (!MediaRecorder.isTypeSupported(options.mimeType)) {
            console.error('不支持的视频格式');
            return;
        }
        try {
            mediaRecorder = new MediaRecorder(allStream, options);
            // 处理采集到的事件,录制回调
            mediaRecorder.ondataavailable = function (e) {
                if (e && e.data && e.data.size > 0) {
                    // 存储到数组中
                    buf.push(e.data);
                    allChunks.push(e.data);
                }
            };
            // 开始录制
            mediaRecorder.start(10);
            sendNewChunks();
        } catch (e) {
            console.error(e);
        }
    }

    // 定时向后端发送 增量blob
    function sendNewChunks() {
        let start = 0;
        let iterationIndex = 0;
        sendTimer = window.setInterval(() => {
            let allBlob = new Blob(allChunks);
            // 把增量视频的片段切下来
            let newBlob = allBlob.slice(start, allBlob.size);
            start = allBlob.size;
            // 当录像有更新的时候，才向接口发送分片信息
            if (newBlob.size > 0) {
                iterationIndex++;
                // 在这里Http post 发送newBlob对象
                wrapperc.invokeMethodAsync("GetResult", "发送newBlob对象" + iterationIndex + " 长度 " + newBlob.size);
                wrapperc.invokeMethodAsync("ReceiveByteArray", newBlob);
            }
        }, 10000)
    }

    document.querySelector('#download').onclick = function () {
        if (mediaRecorder && mediaRecorder.state == "recording") mediaRecorder.stop();
        if (buf.length) {
            return wrapperc.invokeMethodAsync("ReceiveByteArray", buf);

            const blob = new Blob(buf, { type: 'video/webm' });
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.style.display = 'none';
            a.download = 'aaa.webm';
            a.click();
        } else {
            alert('还没有录制任何内容');
        }
    }


    wrapperc.invokeMethodAsync("GetResult", "Ready");
}