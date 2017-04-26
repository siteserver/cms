import * as utils from '../../../utils';
export class Writing {
    constructor(request) {
        this.request = request;
    }
    createContent(publishmentSystemId, nodeId, content, cb) {
        this.request.post(`/writing/actions/create_content`, utils.assign(content, {
            publishmentSystemId, nodeId
        }), cb);
    }
    deleteContent(publishmentSystemId, nodeId, id, cb) {
        this.request.post(`/writing/actions/delete_content`, {
            publishmentSystemId, nodeId, id
        }, cb);
    }
    editContent(publishmentSystemId, nodeId, id, content, cb) {
        this.request.post(`/writing/actions/edit_content`, utils.assign(content, {
            publishmentSystemId, nodeId, id
        }), cb);
    }
    getContent(publishmentSystemId, nodeId, id, cb) {
        this.request.post(`/writing/actions/get_content`, {
            publishmentSystemId, nodeId, id
        }, cb);
    }
    getContents(publishmentSystemId, nodeId, searchType, keyword, dateFrom, dateTo, page, cb) {
        this.request.post(`/writing/actions/get_contents`, {
            publishmentSystemId, nodeId, searchType, keyword, dateFrom, dateTo, page
        }, cb);
    }
    getSites(cb) {
        this.request.post('/writing/actions/get_sites', null, cb);
    }
    getChannels(publishmentSystemId, cb) {
        this.request.post(`/writing/actions/get_channels`, {
            publishmentSystemId
        }, cb);
    }
    getTableStyles(publishmentSystemId, nodeId, cb) {
        this.request.post(`/writing/actions/get_table_styles`, {
            publishmentSystemId, nodeId
        }, cb);
    }
}
//# sourceMappingURL=index.js.map