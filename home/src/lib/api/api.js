import { WebRequest, APIRequest } from './http';
import * as client from './client';
export class API {
    constructor(options) {
        this.options = options;
        this.request = new WebRequest();
        const apiRequest = new APIRequest(options);
        this.files = new client.Files(apiRequest);
        this.users = new client.Users(apiRequest);
        this.writing = new client.Writing(apiRequest);
    }
}
//# sourceMappingURL=api.js.map