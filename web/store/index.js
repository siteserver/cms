export const state = () => ({
  user: null,
  loading: false,
  notifyList: []
})

export const mutations = {
  loading(state, val) {
    state.loading = val
  },

  login(state, val) {
    state.user = val
  },

  logout(state) {
    state.user = null
  },

  notify(state, val) {
    state.notifyList.push(val)
  },

  removeNotify(state, val) {
    state.notifyList.splice(state.notifyList.indexOf(val), 1)
  },

  clearNotify(state) {
    state.notifyList = []
  }
}

export const actions = {

}

export const getters = {
  isLoggedIn(state) {
    return state.user != null
  },

  userName(state) {
    return state.user != null ? state.user.userName : ''
  },

  displayName(state) {
    return state.user != null ? state.user.displayName : ''
  },

  avatarUrl(state) {
    return state.user != null ? state.user.avatarUrl : ''
  },

  isVip(state) {
    return state.user != null ? state.user.isVip : false
  }
}
