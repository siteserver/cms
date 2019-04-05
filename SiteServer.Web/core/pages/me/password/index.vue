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
                  修改密码
                </h5>
                <hr>

                <div class="form-group">
                  <label for="oldPassword">
                    当前密码
                    <small v-show="isError('oldPassword')" class="text-danger">
                      {{ errorMessage('oldPassword')
                      }}
                    </small>
                  </label>
                  <input
                    v-model="oldPassword"
                    v-validate="'required'"
                    type="password"
                    name="oldPassword"
                    class="form-control"
                    data-vv-as="当前密码"
                    :class="{'is-invalid': isError('oldPassword')}"
                  >
                </div>
                <div class="form-group">
                  <label for="newPassword">
                    新密码
                    <small v-show="isError('newPassword')" class="text-danger">
                      {{ errorMessage('newPassword')
                      }}
                    </small>
                  </label>
                  <input
                    ref="newPassword"
                    v-model="newPassword"
                    v-validate="'required'"
                    type="password"
                    name="newPassword"
                    class="form-control"
                    data-vv-as="新密码"
                    :class="{'is-invalid': isError('newPassword')}"
                  >
                </div>
                <div class="form-group">
                  <label for="confirmPassword">
                    新密码确认
                    <small v-show="isError('confirmPassword')" class="text-danger">
                      {{ errorMessage('confirmPassword')
                      }}
                    </small>
                  </label>
                  <input
                    v-model="confirmPassword"
                    v-validate="'required|confirmed:newPassword'"
                    type="password"
                    name="confirmPassword"
                    class="form-control"
                    data-vv-as="新密码确认"
                    :class="{'is-invalid': isError('confirmPassword')}"
                  >
                </div>

                <button class="mt-2 btn btn-primary" :disabled="anyError('account')" type="button" @click="btnUpdatePassword">
                  保 存
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

    async apiUpdatePassword() {
      this.$store.commit('loading', true)
      await this.$api.me.post('settings/actions/resetPassword', {
        password: this.oldPassword,
        newPassword: this.newPassword
      })
      this.$store.commit('loading', false)
      this.$store.commit('notify', {
        type: 'success',
        title: '新密码设置成功'
      })
    },

    btnUpdatePassword() {
      this.$validator.validate().then((result) => {
        if (result) {
          this.apiUpdatePassword()
        }
      })
    }
  }
}
</script>
