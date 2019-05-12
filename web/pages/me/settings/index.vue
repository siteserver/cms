<template>
  <div class="app-main">
    <app-me-menus />
    <div class="app-main__outer">
      <div class="app-main__inner">
        <div class="row justify-content-center">
          <div class="col-md-6 co-sm-12">
            <div class="main-card mb-3 card">
              <div class="card-body">
                <h5 class="card-title">
                  登录账号设置
                </h5>
                <hr>

                <div class="form-group">
                  <label for="userName">
                    用户名
                  </label>
                  <input v-model="user.userName" type="text" disabled class="form-control">
                  <small class="help-block">
                    用户的唯一标识符。
                  </small>
                </div>
                <div class="form-group">
                  <label for="mobile">
                    手机号码
                    <small v-show="isError('account.mobile')" class="text-danger">
                      {{ errorMessage('account.mobile') }}
                    </small>
                  </label>
                  <input
                    v-model="user.mobile"
                    v-validate="'required|mobile'"
                    type="text"
                    name="mobile"
                    class="form-control"
                    data-vv-as="手机号码"
                    data-vv-scope="account"
                    :class="{'is-invalid': isError('account.mobile')}"
                  >
                  <small class="help-block">
                    可用于登录、找回密码等功能，不会公开显示。
                  </small>
                </div>
                <div class="form-group">
                  <label for="email">
                    Email
                  </label>
                  <input
                    v-model="user.email"
                    v-validate="{ email: true }"
                    type="email"
                    name="email"
                    class="form-control"
                    data-vv-as="Email"
                    data-vv-scope="account"
                    :class="{'is-invalid': isError('account.email')}"
                  >
                  <small class="help-block">
                    可用于登录，不会公开显示。
                  </small>
                </div>

                <button class="mt-2 btn btn-primary" :disabled="anyError('account')" type="button" @click="btnUpdateAccount">
                  保 存
                </button>
              </div>
            </div>
          </div>
        </div>

        <div class="row justify-content-center">
          <div class="col-md-6 co-sm-12">
            <div class="main-card mb-3 card">
              <div class="card-body">
                <h5 class="card-title">
                  永久删除账户
                </h5>
                <hr>

                <p>
                  帐户删除操作无法撤销，此操作会删除您的帐户以及帐户中的所有数据。
                </p>

                <p>
                  在您准备好继续之后，请使用“永久删除账户”按钮。
                </p>

                <button class="mt-2 btn btn-danger" type="button" @click="btnDeleteAccount">
                  永久关闭账户
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import AppMeMenus from '@/components/AppMeMenus.vue'

export default {
  $_veeValidate: {
    validator: 'new' // give me my own validator scope.
  },

  components: {
    AppMeMenus
  },

  data() {
    return {
      oldPassword: '',
      newPassword: '',
      confirmPassword: ''
    }
  },

  async asyncData({ app }) {
    const data = await app.$api.me.getAt('settings')
    return { user: data.value }
  },

  created: function () {
    this.uploadUrl = this.$api.me.at(`/avatar`)
  },

  methods: {
    isError(name) {
      return this.errors ? this.errors.has(name) : false
    },

    anyError(name) {
      return this.errors ? this.errors.any(name) : false
    },

    errorMessage(name) {
      return this.errors ? this.errors.first(name) : ''
    },

    async apiUpdateAccount() {
      this.$store.commit('loading', true)
      const data = await this.$api.me.putAt('settings', {
        mobile: this.user.mobile,
        email: this.user.email
      })
      this.user = data.value
      this.$store.commit('login', this.user)
      this.$store.commit('loading', false)
      this.$store.commit('notify', {
        type: 'success',
        title: '登录账号设置修改成功'
      })
    },

    btnUpdateAccount() {
      this.$validator.validate('account.*').then((result) => {
        if (result) {
          this.apiUpdateAccount()
        }
      })
    },

    async apiDeleteAccount() {
      this.$store.commit('loading', true)
      const data = await this.$api.me.post('settings/actions/deleteAccount', {
        password: this.oldPassword,
        newPassword: this.newPassword
      })
      this.user = data.value
      this.$store.commit('login', this.user)
      this.$store.commit('loading', false)
      alert({
        title: '账户已关闭',
        type: 'success',
        showConfirmButton: false
      }).then(function () {
        location.href = 'login.html'
      })
    },

    btnDeleteAccount() {
      alert({
        title: '永久删除账户',
        text: '帐户删除操作无法撤销，此操作会删除您的帐户以及帐户中的所有数据',
        type: 'warning',
        confirmButtonClass: 'btn btn-danger',
        confirmButtonText: '永久删除账户',
        showCancelButton: true,
        cancelButtonText: '取 消'
      }).then((result) => {
        if (result.value) {
          this.apiDeleteAccount()
        }
      })
    }
  }
}
</script>
