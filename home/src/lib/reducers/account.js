const initialState = {
    user: null,
    group: null
};
export default function account(state = initialState, action) {
    if (action.type === 'account/LOGIN') {
        return {
            user: action.data.user || state.user,
            group: action.data.group || state.group,
        };
    }
    else if (action.type === 'account/LOGGED_OUT') {
        return {
            user: null,
            group: null
        };
    }
    else if (action.type === 'account/UPDATE') {
        return {
            user: action.data.user || state.user,
            group: action.data.group || state.group,
        };
    }
    else if (action.type === 'persist/REHYDRATE' && action.payload && action.payload.account) {
        return {
            user: action.payload.account.user,
            group: action.payload.account.group,
        };
    }
    return state;
}
//# sourceMappingURL=account.js.map