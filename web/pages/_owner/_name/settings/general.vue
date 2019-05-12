<template>
  <div class="app-main">
    <app-repo-menus menu-name="settings" />
    <div class="app-main__outer">
      <div class="app-main__inner">
        <div class="row justify-content-center">
          <div class="col-lg-8">
            <div class="mb-3 card">
              <ul class="nav tabs-bordered">
                <li class="nav-item">
                  <n-link :to="getTabUrl('settings/general')" class="nav-link text-dark">
                    常 规
                  </n-link>
                </li>
                <li class="nav-item">
                  <n-link :to="getTabUrl('settings/connect')" class="nav-link text-primary">
                    连 接
                  </n-link>
                </li>
                <li class="nav-item">
                  <n-link :to="getTabUrl('settings/security')" class="nav-link text-primary">
                    安 全
                  </n-link>
                </li>
              </ul>
            </div>

            <div class="mb-3 card">
              <div class="card-header">
                <div class="card-header-title">
                  CMS系统资料
                </div>
              </div>
              <div class="card-body">
                <div class="form-group">
                  <label class="col-form-label">
                    仓库名
                  </label>
                  <input
                    :value="repo.name"
                    type="text"
                    class="form-control"
                    :disabled="true"
                  >
                </div>
                <div class="form-group">
                  <label class="col-form-label">
                    简介
                    <small v-show="isError('description')" class="text-danger">
                      {{ errorMessage('description') }}
                    </small>
                  </label>
                  <input
                    v-model="description"
                    name="description"
                    data-vv-as="简介"
                    :class="{'is-invalid': isError('description') }"
                    type="text"
                    class="form-control"
                  >
                </div>
                <div class="form-group">
                  <label class="col-form-label">
                    CMS后台地址
                  </label>
                  <input
                    :value="`${repo.cmsId}/${repo.adminDirectory}`"
                    type="text"
                    class="form-control"
                    :disabled="true"
                  >
                </div>
                <div class="form-group">
                  <label class="col-form-label">
                    SiteServer CMS 版本
                  </label>
                  <input
                    :value="repo.cmsVersion"
                    type="text"
                    class="form-control"
                    :disabled="true"
                  >
                </div>
              </div>
              <div class="d-block card-footer">
                <a href="javascript:void(0);" class="btn-wide btn btn-primary" @click="btnSubmitClick">
                  保 存
                </a>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import AppRepoMenus from '@/components/AppRepoMenus.vue'

export default {
  name: 'PageConfig',
  components: {
    AppRepoMenus
  },

  $_veeValidate: {
    validator: 'new' // give me my own validator scope.
  },

  data() {
    return {
      description: null
    }
  },

  async asyncData({ app, params }) {
    const data = await app.$api.repoSettingsGeneral.get({
      owner: params.owner,
      name: params.name
    })
    return { repo: data.value }
  },

  mounted() {
    this.description = this.repo.description
  },

  methods: {
    isError(name) {
      if (this.errors) {
        return this.errors.has(name)
      }
      return false
    },

    errorMessage(name) {
      if (this.errors) {
        return this.errors.first(name)
      }
      return ''
    },

    getTabUrl(name) {
      return `/${this.$route.params.owner}/${this.$route.params.name}/${name}`
    },

    async apiSubmit() {
      this.$store.commit('loading', true)
      const data = await this.$api.repoSettings.putAt(`general/${this.$route.params.owner}/${this.$route.params.name}`, {
        description: this.description
      })
      this.repo = data.value
      this.$store.commit('loading', false)
      this.$store.commit('notify', {
        type: 'success',
        title: '提交成功'
      })
    },

    btnSubmitClick() {
      this.$validator.validate().then((result) => {
        if (result) {
          this.apiSubmit()
        }
      })
    }
  }
}
</script>
