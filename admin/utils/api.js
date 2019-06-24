// https://blog.lichter.io/posts/organize-and-decouple-your-api-calls-in-nuxtjs/

import _ from 'lodash'

export default $axios => route => ({
  get(config) {
    return $axios.$get(route, config)
  },

  delete(config) {
    return $axios.$delete(route, config)
  },

  post(data, config) {
    return $axios.$post(route, data, config)
  },

  put(data, config) {
    return $axios.$put(route, data, config)
  },

  at(url) {
    return url ? `${$axios.defaults.baseURL}/${route}/${_.trimStart(url, '/')}` : `${$axios.baseURL}/${route}`
  },

  getAt(url, config) {
    return $axios.$get(`${route}/${_.trimStart(url, '/')}`, config)
  },

  deleteAt(url, config) {
    return $axios.$delete(`${route}/${_.trimStart(url, '/')}`, config)
  },

  postAt(url, data, config) {
    return $axios.$post(`${route}/${_.trimStart(url, '/')}`, data, config)
  },

  putAt(url, data, config) {
    return $axios.$put(`${route}/${_.trimStart(url, '/')}`, data, config)
  }
})
