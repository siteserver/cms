const initialState = {};
export default function config(state = initialState, action) {
    if (action.type === 'config/LOAD_CONFIG') {
        return action.data;
    }
    else if (action.type === 'persist/REHYDRATE' && action.payload) {
        return action.payload.config || {};
    }
    return state;
}
//# sourceMappingURL=config.js.map