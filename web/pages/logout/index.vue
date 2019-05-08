<template>
  <div class="app-main app-full">
    <div class="app-main__outer">
      <div class="app-main__inner">
        <div class="row">
          <div class="col-lg-4" />
          <div class="col-lg-4">
            <div class="main-card mb-3 card">
              <div class="card-header">
                退出登录
              </div>
              <div class="card-body">
                <div class="form-group row">
                  <div class="col-12">
                    <div class="text-center">
                      正在退出登录...
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="col-lg-4" />
        </div>
      </div>
      <div class="app-wrapper-footer">
        <div class="app-footer">
          <div class="app-footer__inner">
            <div class="app-footer-left">
              <ul class="nav">
                <li class="nav-item">
                  <a href="javascript:void(0);" class="nav-link">
                    Footer Link 1
                  </a>
                </li>
                <li class="nav-item">
                  <a href="javascript:void(0);" class="nav-link">
                    Footer Link 2
                  </a>
                </li>
              </ul>
            </div>
            <div class="app-footer-right">
              <ul class="nav">
                <li class="nav-item">
                  <a href="javascript:void(0);" class="nav-link">
                    Footer Link 3
                  </a>
                </li>
                <li class="nav-item">
                  <a href="javascript:void(0);" class="nav-link">
                    <div class="badge badge-success mr-1 ml-0">
                      <small>NEW</small>
                    </div>
                    Footer Link 4
                  </a>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      pageSubmit: false,
      account: null,
      password: null
    }
  },
  mounted() {
    this.apiLogout()
  },
  methods: {
    async apiLogout() {
      await this.$api.logout.post()
      this.$store.commit('logout')
      this.$router.push({ path: 'login', query: { redirectUrl: this.$route.query.redirectUrl } })
      // ctx.redirect({ path: 'logout', query: { redirectUrl: ctx.route.path } })

      // this.$store.commit('loading', true)

      // this.$axios
      //   .post(this.$api.console.logout)
      //   .then((response) => {
      //     this.$store.commit('logout')
      //     if (this.$route.query.redirectUrl) {
      //       location.href = this.$route.query.redirectUrl
      //     } else {
      //       this.$router.push('/dashboard/')
      //     }
      //   })
      //   .catch((error) => {
      //     this.$store.commit('loading', false)
      //     this.error = error
      //   })
    }
  }
}
</script>
