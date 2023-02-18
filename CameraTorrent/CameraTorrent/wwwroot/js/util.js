
window.downloadFileFromStream = async (fileName, contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
}

window.getFrame = (src, dest, dotNetHelper) => {
    const video = document.getElementById(src);
    const canvas = document.getElementById(dest);
    const ctx = canvas.getContext('2d');
    ctx.drawImage(video, 0, 0, 1280, 720);

    const dataUrl = canvas.toDataURL("image/jpeg");
    dotNetHelper.invokeMethodAsync('ProcessImage', dataUrl);
}

window.stopVideo = (src) => {
    const video = document.getElementById(src);
    if (video && "srcObject" in video) {
        const stream = video.srcObject;
        if (stream) {
            const tracks = stream.getTracks();
            tracks.forEach(track => track.stop());
        }
        video.srcObject = null;
    }
}

window.startVideo = async (src) => {
    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
        const setup = {
            video: {
                facingMode: { exact: "environment" },
                width: { min: 1024, ideal: 1280, max: 1920 },
                height: { min: 576, ideal: 720, max: 1080 }
            }
        };
        let stream;
        try {
            stream = await navigator.mediaDevices.getUserMedia(setup);
        } catch (err) {
            setup.video.facingMode = null;
            stream = await navigator.mediaDevices.getUserMedia(setup);
        }
        const btn = document.getElementById(src + "Btn");
        btn.style.display = "block";
        const video = document.getElementById(src);
        video.style.display = "block";

        if ("srcObject" in video) {
            video.srcObject = stream;
        } else {
            video.src = window.URL.createObjectURL(stream);
        }
        video.onloadedmetadata = function (_e) {
            video.play();
        };
        video.style.webkitTransform = "scaleX(-1)";
        video.style.transform = "scaleX(-1)";
    }
}
