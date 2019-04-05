// https://blog.lichter.io/posts/organize-and-decouple-your-api-calls-in-nuxtjs/

import _ from 'lodash'

const ssApiUrl = 'http://api.siteserver-cms.com/v1.3'

export default $axios => route => ({
  get(params) {
    if (params) {
      return $axios.$get(`${ssApiUrl}/${route}`, {
        params: params
      })
    }
    return $axios.$get(`${ssApiUrl}/${route}`)
  },

  post(payload) {
    return $axios.$post(`${ssApiUrl}/${route}`, payload)
  },

  put(payload) {
    return $axios.$put(`${ssApiUrl}/${route}`, payload)
  },

  delete(payload) {
    return $axios.$delete(`${ssApiUrl}/${route}`, payload)
  },

  at(url) {
    return url ? `${$axios.defaults.baseURL}/${route}/${_.trimStart(url, '/')}` : `${$axios.baseURL}/${route}`
  },

  getAt(url, params) {
    if (params) {
      return $axios.$get(`${ssApiUrl}/${route}/${_.trimStart(url, '/')}`, {
        params: params
      })
    }
    return $axios.$get(`${ssApiUrl}/${route}/${_.trimStart(url, '/')}`)
  },

  postAt(url, payload) {
    return $axios.$post(`${ssApiUrl}/${route}/${_.trimStart(url, '/')}`, payload)
  },

  putAt(url, payload) {
    return $axios.$put(`${ssApiUrl}/${route}/${_.trimStart(url, '/')}`, payload)
  },

  deleteAt(url, payload) {
    return $axios.$delete(`${ssApiUrl}/${route}/${_.trimStart(url, '/')}`, payload)
  }
})
