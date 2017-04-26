export function getUploadFilesProps(url, multi, accept, success, error, progress) {
    return {
        action: url,
        multiple: multi,
        dataType: 'json',
        accept: accept,
        maxFileSize: 5000000,
        withCredentials: true,
        onStart(files) {
        },
        beforeUpload(file) {
            if (['docx', 'doc', 'xlsx', 'xls', 'rar', 'zip', 'tz', 'txt', 'pdf', 'png', 'jpg', 'jpeg'].indexOf(file.name.split('.')[1]) === -1) {
                error('不能上传该类型文件！');
                return false;
            }
        },
        onSuccess(ret) {
            let res = (typeof ret === 'string') ? JSON.parse(ret.replace('<pre>', '').replace('</pre>', '')) : ret;
            if (res.fileUrls) {
                success(res.fileUrls);
            }
            else {
                error(res.errorMessage);
            }
        },
        onProgress(step, file) {
            progress && progress();
        },
        onError(err) {
            error(err.errorMessage);
        }
    };
}
export function getUploadAvatarProps(url, success, error, progress) {
    return {
        action: url,
        multiple: false,
        dataType: 'json',
        accept: "image/png, image/jpeg, image/gif",
        maxFileSize: 5000000,
        withCredentials: true,
        onStart(files) {
        },
        onSuccess(ret) {
            let res = (typeof ret === 'string') ? JSON.parse(ret.replace('<pre>', '').replace('</pre>', '')) : ret;
            if (res.avatarUrl) {
                success(res.avatarUrl, res.relatedUrl);
            }
            else {
                error(res.errorMessage);
            }
        },
        onProgress(step, file) {
            progress && progress();
        },
        onError(err) {
            error(err.errorMessage);
        }
    };
}
//# sourceMappingURL=index.js.map