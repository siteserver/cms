import { applyMiddleware, createStore, compose } from 'redux';
import thunkMiddleware from 'redux-thunk';
import createLogger from 'redux-logger';
import { persistStore, autoRehydrate } from 'redux-persist';
import reducers from '../reducers';
export default function configureStore() {
    let store = null;
    if (window['devToolsExtension']) {
        store = createStore(reducers, {}, compose(applyMiddleware(thunkMiddleware, createLogger()), autoRehydrate(), window['devToolsExtension']()));
    }
    else {
        store = createStore(reducers, applyMiddleware(thunkMiddleware, createLogger()), autoRehydrate());
    }
    persistStore(store);
    return store;
}
//# sourceMappingURL=configStore.js.map