// https://blog.lichter.io/posts/organize-and-decouple-your-api-calls-in-nuxtjs/

import createApi from '~/utils/api'

export default (ctx, inject) => {
  const apiWithAxios = createApi(ctx.$axios)

  const api = {
    login: apiWithAxios('login'),
    logout: apiWithAxios('logout'),
    main: apiWithAxios(''),
    index: apiWithAxios('index'),
    createStatus: apiWithAxios('create/status')
  }

  inject('api', api)

  ctx.$axios.onError((error) => {
    const code = parseInt(error.response && error.response.status)
    let message = error.message
    if (error.response.data) {
      if (error.response.data.exceptionMessage) {
        message = error.response.data.exceptionMessage
      } else if (error.response.data.message) {
        message = error.response.data.message
      }
    }

    if (process.isServer) {
      if (code === 401) {
        ctx.redirect({ path: 'logout', query: { redirectUrl: ctx.route.path } })
      } else {
        ctx.error({ statusCode: code, message: message })
      }
    } else if (code === 401) {
      ctx.store.commit('notify', {
        type: 'error',
        title: '错误',
        text: '检测到用户未登录或者登录已超时，请重新登录'
      })
      setTimeout(() => {
        ctx.redirect({ path: 'logout', query: { redirectUrl: ctx.route.path } })
      }, 1500)
    } else {
      ctx.store.commit('notify', {
        type: 'error',
        title: '错误',
        text: message
      })
    }
  })
}
