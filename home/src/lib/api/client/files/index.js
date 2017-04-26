export class Files {
    constructor(request) {
        this.request = request;
    }
    getUploadSiteFilesUrl(siteId, uploadType) {
        return this.request.getURL(`/files/actions/upload_site_files?siteId=${siteId}&uploadType=${uploadType}`);
    }
    getUploadAvatarUrl() {
        return this.request.getURL(`/files/actions/upload_avatar`);
    }
    uploadAvatarResize(size, x, y, relatedUrl, cb) {
        this.request.post('/files/actions/upload_avatar_resize', {
            size, x, y, relatedUrl
        }, cb);
    }
}
//# sourceMappingURL=index.js.map