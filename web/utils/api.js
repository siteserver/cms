// https://blog.lichter.io/posts/organize-and-decouple-your-api-calls-in-nuxtjs/

import _ from 'lodash'

export default $axios => route => ({
  get(params) {
    if (params) {
      return $axios.$get(`/${route}`, {
        params: params
      })
    }
    return $axios.$get(`/${route}`)
  },

  post(payload) {
    return $axios.$post(`/${route}`, payload)
  },

  put(payload) {
    return $axios.$put(`/${route}`, payload)
  },

  delete(payload) {
    return $axios.$delete(`/${route}`, payload)
  },

  at(url) {
    return url ? `${$axios.defaults.baseURL}/${route}/${_.trimStart(url, '/')}` : `${$axios.baseURL}/${route}`
  },

  getAt(url, params) {
    if (params) {
      return $axios.$get(`/${route}/${_.trimStart(url, '/')}`, {
        params: params
      })
    }
    return $axios.$get(`/${route}/${_.trimStart(url, '/')}`)
  },

  postAt(url, payload) {
    return $axios.$post(`/${route}/${_.trimStart(url, '/')}`, payload)
  },

  putAt(url, payload) {
    return $axios.$put(`/${route}/${_.trimStart(url, '/')}`, payload)
  },

  deleteAt(url, payload) {
    return $axios.$delete(`/${route}/${_.trimStart(url, '/')}`, payload)
  }
})
