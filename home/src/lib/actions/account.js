export function login(account) {
    return {
        type: 'account/LOGIN',
        data: account,
    };
}
export function register(account) {
    return {
        type: 'account/LOGIN',
        data: account,
    };
}
export function loggedOut() {
    return {
        type: 'account/LOGGED_OUT'
    };
}
export function update(account) {
    return {
        type: 'account/UPDATE',
        data: account,
    };
}
//# sourceMappingURL=account.js.map