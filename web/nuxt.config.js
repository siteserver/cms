const pkg = require('./package')

module.exports = {
  mode: 'universal',

  /*
   ** Headers of the page
   */
  head: {
    title: pkg.name,
    meta: [
      {
        charset: 'utf-8'
      },
      {
        name: 'viewport',
        content: 'width=device-width, initial-scale=1'
      },
      {
        name: 'renderer',
        content: 'webkit'
      },
      {
        'http-equiv': 'X-UA-Compatible',
        content: 'IE=edge,chrome=1'
      },
      {
        hid: 'description',
        name: 'description',
        content: pkg.description
      }
    ],
    link: [
      { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' },
      { rel: 'stylesheet', href: '/assets/css/bootstrap-4.1.0.min.css' },
      { rel: 'stylesheet', href: '/assets/css/siteserver.min.css' },
      { rel: 'stylesheet', href: '/assets/css/ionicons-2.0.0.min.css' },
      { rel: 'stylesheet', href: '/assets/css/font-awesome-4.7.0.min.css' }
    ],
    script: [
      { src: '/assets/js/jquery-1.9.1.min.js' },
      { src: '/assets/js/layer/layer-3.1.1.js' }
    ]
  },

  router: {
    linkActiveClass: 'active'
  },

  /*
   ** Customize the progress-bar color
   */
  loading: {
    color: '#4c5667' // '#6c757d'
  },

  /*
   ** Plugins to load before mounting the App
   */
  plugins: [
    '~/plugins/api',
    '~/plugins/ss',
    { src: '~/plugins/cookie-storage', ssr: false },
    { src: '~/plugins/vee-validate', ssr: false },
    { src: '~/plugins/layer', ssr: false }
  ],

  /*
   ** Nuxt.js modules
   */
  modules: [
    // Doc: https://axios.nuxtjs.org/usage
    '@nuxtjs/axios',
    '@nuxtjs/pwa',
    [
      'vue-sweetalert2/nuxt',
      {
        confirmButtonClass: 'btn btn-primary',
        cancelButtonClass: 'btn btn-default ml-2',
        buttonsStyling: false
      }
    ]
  ],
  /*
   ** Axios module configuration
   */
  axios: {
    // See https://github.com/nuxt-community/axios-module#options
    baseURL: 'http://dev.cms.com/api',
    credentials: true,
    progress: true
  },

  /*
   ** Build configuration
   */
  build: {
    /*
     ** You can extend webpack config here
     */
    // publicPath: 'https://cdn.nuxtjs.org',
    extend(config, ctx) {
      // Run ESLint on save
      if (ctx.isDev && ctx.isClient) {
        config.module.rules.push({
          enforce: 'pre',
          test: /\.(js|vue)$/,
          loader: 'eslint-loader',
          exclude: /(node_modules)/
        })
      }
    }
  },

  server: {
    port: 7081, // default: 3000
    host: '127.0.0.1' // default: localhost
  }
}
