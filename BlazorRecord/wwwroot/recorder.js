export function streamToDotNet() {
    return new Uint8Array(10000000);
}
export function streamToDotNet2(wrapperc) {
    const data = new Uint8Array([0x45, 0x76, 0x65, 0x72, 0x79, 0x74, 0x68, 0x69,
        0x6e, 0x67, 0x27, 0x73, 0x20, 0x73, 0x68, 0x69, 0x6e, 0x79, 0x2c,
        0x20, 0x43, 0x61, 0x70, 0x74, 0x69, 0x61, 0x6e, 0x2e, 0x20, 0x4e,
        0x6f, 0x74, 0x20, 0x74, 0x6f, 0x20, 0x66, 0x72, 0x65, 0x74, 0x2e]);
    wrapperc.invokeMethodAsync('ReceiveByteArray', new Uint8Array(64 * 1024 - 10),"txt.txt")
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
                wrapperc.invokeMethodAsync("GetResult", "getDisplayMedia");
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
                wrapperc.invokeMethodAsync("GetResult", "getUserMedia");
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
            wrapperc.invokeMethodAsync("GetResult", "Stop");
        }
    }

    let mediaRecorder;
    //录制到的数据数据
    let allChunks = [];
    let state;

    document.querySelector('#record').onclick = function () {
        // 约束视频格式
        let options = {
            mimeType: 'video/webm;codecs=vp9,opus'
        }
        //'video/webm;codecs=vp9,opus',
        //'video/webm;codecs=vp8,opus',
        //'video/webm;codecs=h264,opus',
        //'video/mp4;codecs=h264,aac',
 

        // 判断是否是支持的mimeType格式
        if (!MediaRecorder.isTypeSupported(options.mimeType)) {
            console.error('不支持的视频格式');
            wrapperc.invokeMethodAsync("GetResult", "不支持的视频格式1");
            options = {
                mimeType: 'video/webm;codecs=h264,opus'
            }
            if (!MediaRecorder.isTypeSupported(options.mimeType)) {
                console.error('不支持的视频格式');
                wrapperc.invokeMethodAsync("GetResult", "不支持的视频格式2");
                return;
            }
        }
        try {
            mediaRecorder = new MediaRecorder(allStream, options);
            // 处理采集到的事件,录制回调
            mediaRecorder.ondataavailable = function (e) {
                if (e && e.data && e.data.size > 0) {
                    // 存储到数组中
                    allChunks.push(e.data);
                    if (state !== 'recording') {
                        wrapperc.invokeMethodAsync("GetResult", "recording...");
                        state = 'recording';
                    }
                }
            };
            // 开始录制
            mediaRecorder.start(10);
            sendNewChunks();
            wrapperc.invokeMethodAsync("GetResult", "Record start");
        } catch (e) {
            console.error(e);
        }
    }

    // 定时向后端发送 增量blob
    function sendNewChunks() {
        let start = 0;
        let iterationIndex = 0;
        sendTimer = window.setInterval(async () => {
            let allBlob = new Blob(allChunks);
            // 把增量视频的片段切下来
            let newBlob = allBlob.slice(start, allBlob.size);
            start = allBlob.size;
            // 当录像有更新的时候，才向接口发送分片信息
            if (newBlob.size > 0) {
                iterationIndex++;
                // 在这里Http post 发送newBlob对象
                wrapperc.invokeMethodAsync("GetResult", "发送切片" + iterationIndex + " 长度kb " + newBlob.size/1024 );
                const content = await (new Response(newBlob).arrayBuffer());
                const contentNums = new Uint8Array(content);
                wrapperc.invokeMethodAsync("GetResultblob", contentNums, "Chunks" + iterationIndex);
            }
        }, 5000)
    }

    document.querySelector('#download').onclick =async function () {
        if (mediaRecorder && mediaRecorder.state == "recording") mediaRecorder.stop();
        if (allChunks.length) {
            const blob = new Blob(allChunks, { type: 'video/webm' });
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.style.display = 'none';
            a.download = 'aaa.webm';
            a.click();
            wrapperc.invokeMethodAsync("GetResult", "前端下载文件");
        } else {
            alert('还没有录制任何内容');
        }
    }

    document.querySelector('#sent').onclick =async function () {
        if (mediaRecorder && mediaRecorder.state == "recording") mediaRecorder.stop();
        if (allChunks.length) {
            const response = new Blob(allChunks, { type: 'video/webm' });
            const content = await (new Response(response).arrayBuffer());
            const contentNums = new Uint8Array(content);
            return wrapperc.invokeMethodAsync('ReceiveByteArray', contentNums,'all')
                .then(str => {
                    alert(str);
                });
        } else {
            alert('还没有录制任何内容');
        }
    }


    wrapperc.invokeMethodAsync("GetResult", "Ready");
}