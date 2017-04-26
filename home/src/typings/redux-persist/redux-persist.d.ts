// Type definitions for redux-logger v2.6.0
// Project: https://github.com/fcomb/redux-logger
// Definitions by: Alexander Rusakov <https://github.com/arusakov/>
// Definitions: https://github.com/DefinitelyTyped/DefinitelyTyped

/// <reference path="../redux/redux.d.ts" />

declare module 'redux-persist' {
  function persistStore(options?: any): any;
  function autoRehydrate(): any;
  export {persistStore, autoRehydrate};
}
