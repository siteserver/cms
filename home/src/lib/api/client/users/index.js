export class Users {
    constructor(request) {
        this.request = request;
    }
    loadConfig(cb) {
        this.request.post('/users/actions/load_config', {}, cb);
    }
    login(account, password, cb) {
        this.request.post('/users/actions/login', {
            account, password
        }, cb);
    }
    logout(cb) {
        this.request.post('/users/actions/logout', null, cb);
    }
    resetPassword(password, newPassword, confirmPassword, cb) {
        this.request.post('/users/actions/reset_password', {
            password, newPassword, confirmPassword
        }, cb);
    }
    resetPasswordByToken(token, password, cb) {
        this.request.post('/users/actions/reset_password_by_token', {
            token, password
        }, cb);
    }
    edit(data, cb) {
        this.request.post(`/users/actions/edit`, data, cb);
    }
    getLogs(totalNum, action, cb) {
        this.request.post(`/users/actions/get_logs`, {
            totalNum, action
        }, cb);
    }
    isMobileExists(mobile, cb) {
        this.request.post(`/users/actions/is_mobile_exists/`, {
            mobile
        }, cb);
    }
    isPasswordCorrect(password, cb) {
        this.request.post(`/users/actions/is_password_correct/`, {
            password
        }, cb);
    }
    isCodeCorrect(mobile, code, cb) {
        this.request.post(`/users/actions/is_code_correct/`, {
            mobile, code
        }, cb);
    }
    sendSmsOrRegister(mobile, password, cb) {
        this.request.post(`/users/actions/send_sms_or_register/`, {
            mobile, password
        }, cb);
    }
    sendSms(account, cb) {
        this.request.post(`/users/actions/send_sms/`, {
            account
        }, cb);
    }
    registerWithCode(mobile, password, code, cb) {
        this.request.post(`/users/actions/register_with_code/`, {
            mobile, password, code
        }, cb);
    }
    getCaptchaUrl(code) {
        return this.request.getURL(`/users/captcha/${code}`);
    }
}
//# sourceMappingURL=index.js.map