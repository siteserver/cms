// https://blog.lichter.io/posts/organize-and-decouple-your-api-calls-in-nuxtjs/

import createApi from '~/utils/ss'

export default (ctx, inject) => {
  const apiWithAxios = createApi(ctx.$axios)

  const ssConsoleUrl = 'http://console.siteserver-cms.com'

  const ss = {
    connections: apiWithAxios('connections'),
    updates: apiWithAxios('updates'),
    plugins: apiWithAxios('plugins'),
    templates: apiWithAxios('templates'),
    connect: apiWithAxios('connect'),
    users: apiWithAxios('users'),
    usersStatus: apiWithAxios('users/status'),

    getTemplatesUrl(url) {
      return 'https://templates.siteserver.cn/' + url
    },

    getDemoUrl(url) {
      return 'https://demo.siteserver.cn/' + url
    },

    getTemplatePageUrl(templateId) {
      return 'https://www.siteserver.cn/templates/template.html?id=' + templateId
    },

    getPluginsUrl(url) {
      return 'https://plugins.siteserver.cn/' + url
    },

    getPluginsPageUrl() {
      return 'https://www.siteserver.cn/plugins/'
    },

    getPluginPageUrl(pluginId) {
      return 'https://www.siteserver.cn/plugins/plugin.html?id=' + pluginId
    },

    getVersionPageUrl(major, minor) {
      return (
        'https://www.siteserver.cn/updates/v' +
        major +
        '_' +
        minor +
        '/index.html'
      )
    },

    getUserPageUrl(userName) {
      return 'https://www.siteserver.cn/users/index.html?userName=' + userName
    },

    getConnectUrl(guid) {
      return ssConsoleUrl + '/connect/?guid=' + guid
    }
  }

  inject('ss', ss)
}
