<template>
  <div class="wrapper-page">
    <div class="card-box">
      <h4 class="m-l-5">
        管理员登录
      </h4>

      <form class="form-horizontal m-t-20" @submit="btnLoginClick">
        <template v-if="pageAlert">
          <div class="alert" :class="{ 'alert-warning': pageAlert.type === 'warning', 'alert-success': pageAlert.type === 'success', 'alert-danger': pageAlert.type === 'danger' }">
            <button class="close" data-dismiss="alert" @click="pageAlert = null">
              ×
            </button>
            {{ pageAlert.html }}
          </div>
        </template>

        <div class="form-group row">
          <div class="col-12">
            <div class="input-group">
              <div class="input-group-prepend">
                <span class="input-group-text">
                  <i class="fa fa-user-circle-o" />
                </span>
              </div>
              <input
                v-model="account"
                v-focus
                :class="{ 'is-invalid': pageSubmit && !account }"
                class="form-control"
                type="text"
                placeholder="请输入用户名/手机号/邮箱"
              >
            </div>
          </div>
        </div>

        <div class="form-group row">
          <div class="col-12">
            <div class="input-group">
              <div class="input-group-prepend">
                <span class="input-group-text">
                  <i class="fa fa-key" />
                </span>
              </div>
              <input
                v-model="password"
                :class="{ 'is-invalid': pageSubmit && !password }"
                class="form-control"
                type="password"
                placeholder="请输入密码"
              >
            </div>
          </div>
        </div>

        <div class="form-group row">
          <div class="col-12">
            <div class="input-group">
              <div class="input-group-prepend">
                <span class="input-group-text">
                  <i class="fa fa-picture-o" />
                </span>
              </div>
              <input
                v-model="captcha"
                :class="{ 'is-invalid': pageSubmit && !captcha }"
                class="form-control"
                type="text"
                placeholder="请输入验证码"
              >
            </div>
          </div>
        </div>

        <div class="form-group row">
          <div class="col-6">
            <div class="checkbox checkbox-primary" style="margin-top: 25px">
              <input id="autoLogin" v-model="isAutoLogin" type="checkbox">
              <label for="autoLogin">
                下次自动登录
              </label>
            </div>
          </div>
          <div class="col-6">
            <a href="javascript:;" @click="reload">
              <img v-show="captchaUrl" class="float-right rounded" :src="captchaUrl" align="absmiddle">
            </a>
          </div>
        </div>

        <div class="form-group row">
          <div class="col-12">
            <button style="width: 100%" class="btn btn-primary btn-large btn-custom w-md" type="submit" @click="btnLoginClick">
              登 录
            </button>
          </div>
        </div>

        <hr>

        <div class="text-muted text-center">
          <a href="https://www.siteserver.cn" target="_blank">
            <small>Copyright &copy; SiteServer CMS</small>
          </a>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import md5 from 'blueimp-md5'

export default {
  layout: 'root',
  directives: {
    focus: {
      inserted: function (e) {
        e.focus()
      }
    }
  },
  data() {
    return {
      pageSubmit: false,
      pageAlert: null,
      account: null,
      password: null,
      isAutoLogin: false,
      captcha: null,
      captchaUrl: null
    }
  },
  mounted() {
    this.apiStatus()
  },
  methods: {
    async apiStatus() {
      this.$store.commit('loading', true)
      const data = await this.$api.login.get()
      const user = data.value
      if (user === null) {
        this.$store.commit('logout')
        this.reload()
      } else {
        this.$store.commit('login', user)
        if (this.$route.query.redirectUrl) {
          location.href = this.$route.query.redirectUrl
        } else {
          this.$router.push('/')
        }
      }
      this.$store.commit('loading', false)
    },

    async apiLogin() {
      this.$store.commit('clearNotify')
      this.$store.commit('loading', true)

      const data = await this.$api.login.post({
        account: this.account,
        password: md5(this.password),
        captcha: this.captcha,
        isAutoLogin: this.isAutoLogin
      })
      const user = data.value
      this.$store.commit('login', user)
      if (this.$route.query.redirectUrl) {
        location.href = this.$route.query.redirectUrl
      } else {
        this.$router.push(`/dashboard`)
      }
    },

    reload() {
      this.captcha = ''
      this.pageSubmit = false
      this.captchaUrl = this.$api.login.at('captcha?r=' + new Date().getTime())
    },

    btnLoginClick(e) {
      e.preventDefault()
      this.alert = null
      this.pageSubmit = true
      this.account && this.password && this.captcha && this.apiLogin()
    },

    btnRegisterClick() {
      this.$routers.push(`/register/?redirectUrl=${location.href}`)
    }
  }
}
</script>
