import createPersistedState from 'vuex-persistedstate'
const Cookies = process.client ? require('js-cookie') : undefined

export default ({ app, store, $axios, isHMR }) => {
  // Just in case nuxt.config.js gets a change in ssr:false when adding this plugin.
  // Notice isClient is deprecated, instead use process.browser.
  if (process.browser) {
    // In case of HMR, mutation occurs before nuxtReady, so previously saved state
    // gets replaced with original state received from server. So, we've to skip HMR.
    // Also nuxtReady event fires for HMR as well, which results multiple registration of
    // vuex-persistedstate plugin
    if (isHMR) return
    createPersistedState({
      key: 'SS-STATE',

      // An array of any paths to partially persist the state.
      // Paths are using dots to indicate nestedness.
      paths: [
        'user'
      ],
      storage: {
        getItem: key => Cookies.get(key),
        setItem: (key, value) => Cookies.set(key, value),
        removeItem: key => Cookies.remove(key)
      }
    })(store) // vuex plugins can be connected to store, even after creation
  }
}
