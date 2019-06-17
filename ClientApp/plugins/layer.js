// https://blog.lichter.io/posts/organize-and-decouple-your-api-calls-in-nuxtjs/

export default (ctx, inject) => {
  const modal = {
    closeLayer() {
      parent.layer.closeAll()
      return false
    },

    openLayer(config) {
      if (!config || !config.url) return false

      if (!config.width) {
        config.width = window.innerWidth - 50
      }
      if (!config.height) {
        config.height = window.innerHeight - 50
      }

      if (config.full) {
        config.width = window.innerWidth - 50
        config.height = window.innerHeight - 50
      }

      window.layer.open({
        type: 2,
        btn: null,
        title: config.title,
        area: [config.width + 'px', config.height + 'px'],
        maxmin: true,
        resize: true,
        shadeClose: true,
        content: config.url
      })

      return false
    }
  }

  inject('modal', modal)
}
