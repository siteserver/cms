<template>
  <div class="app-main app-full">
    <div class="app-main__outer">
      <div class="app-main__inner">
        <div class="app-page-title">
          <div class="page-title-wrapper">
            <div class="page-title-heading">
              <div class="page-title-icon">
                <i class="pe-7s-refresh-cloud icon-gradient bg-happy-itmeo" />
              </div>
              <div>
                添加 CMS 云连接
                <div class="page-title-subheading">
                  请选择一个CMS系统，以进入管理界面
                </div>
              </div>
            </div>
            <div class="page-title-actions">
              <button type="button" class="btn-shadow mr-3 btn btn-light">
                <i class="fa fa-arrow-left" />
                返 回
              </button>
            </div>
          </div>
        </div>
        <div class="row">
          <div class="col-lg-3" />
          <div class="col-lg-6">
            <div class="card-box mt-3">
              <template v-if="templateType == 'validate'">
                <div class="m-4 text-center">
                  <div class="mb-4">
                    <i class="pe-7s-refresh-cloud icon-gradient bg-malibu-beach" style="font-size: 120px" />
                  </div>
                  <h3>使用云连接添加现有的 SiteServer CMS 系统</h3>
                  <h5 class="form-text text-muted">
                    我们将使用云连接将您的站点连接到 SITESERVER.CN
                  </h5>
                </div>
                <no-ssr>
                  <div class="form-group">
                    <label class="col-form-label">
                      SiteServer CMS 后台地址 <small v-show="isError('url')" class="text-danger">
                        {{ errorMessage('url') }}
                      </small>
                    </label>
                    <input
                      v-model="url"
                      v-validate="'required|url'"
                      name="url"
                      data-vv-as="后台地址"
                      :class="{'is-invalid': isError('url') }"
                      type="text"
                      class="form-control"
                      :disabled="loading"
                    >
                  </div>
                </no-ssr>
                <small class="form-text text-muted mb-2">
                  在此填写您的SiteServer CMS 后台访问地址，如：http://www.mysite.com/siteserver
                </small>
                <hr>
                <div class="text-center mb-2">
                  <small class="form-text text-muted mb-2">
                    设置云连接即表示，您同意遵守<a
                      href="https://www.siteserver.cn/account/agreement.html"
                      target="_blank"
                    >
                      服务条款
                    </a>，并将<a href="https://www.siteserver.cn/account/privacy.html" target="_blank">
                      某些数据和设置
                    </a>同步至
                    SITESERVER.CN
                  </small>
                  <button v-if="loading" class="btn btn-block btn-primary" disabled>
                    正在连接...
                  </button>
                  <button v-else class="btn btn-block btn-primary" @click="btnValidateClick">
                    继 续
                  </button>
                </div>
              </template>
              <template v-else-if="templateType == 'submit'">
                <div class="m-4 text-center">
                  <div class="mb-4">
                    <i class="pe-7s-refresh-cloud icon-gradient bg-malibu-beach" style="font-size: 120px" />
                  </div>
                  <h3>CMS系统仓库设置</h3>
                  <h5 class="form-text text-muted">
                    CMS系统仓库用于在云端存储本地CMS系统的文件及数据
                  </h5>
                </div>
                <div class="form-group">
                  <label class="col-form-label">
                    SiteServer CMS 后台地址
                  </label>
                  <input v-model="url" type="text" class="form-control" :disabled="true">
                </div>
                <div class="form-group">
                  <label class="col-form-label">
                    所有者
                  </label>
                  <input v-model="repositoryOwner" type="text" class="form-control" :disabled="true">
                </div>
                <div class="form-group">
                  <label class="col-form-label">
                    仓库名
                  </label>
                  <input
                    v-model="repositoryName"
                    v-validate="'required'"
                    name="repositoryName"
                    data-vv-as="仓库名"
                    :class="{'is-invalid': errors.has('repositoryName') }"
                    type="text"
                    class="form-control"
                    :disabled="loading"
                  >
                </div>
                <div class="form-group">
                  <label class="col-form-label">
                    简介
                  </label>
                  <input
                    v-model="description"
                    name="description"
                    data-vv-as="简介"
                    :class="{'is-invalid': errors.has('description') }"
                    type="text"
                    class="form-control"
                    :disabled="loading"
                  >
                </div>
                <div class="radio radio-primary">
                  <input id="isPublic_true" v-model="isPublic" type="radio" name="isPublic" :value="true">
                  <label for="isPublic_true">
                    <h5 class="mt-0 m-b-5 font-16">
                      <i class="pe-7s-notebook" /> 公开仓库
                    </h5>
                  </label>
                </div>
                <div class="radio radio-primary">
                  <input id="isPublic_false" v-model="isPublic" type="radio" name="isPublic" :value="false">
                  <label for="isPublic_false">
                    <h5 class="mt-0 m-b-5 font-16">
                      <i class="pe-7s-lock" /> 私有仓库
                    </h5>
                  </label>
                </div>
                <hr>
                <div class="text-center mb-2">
                  <small class="form-text text-muted mb-2">
                    设置云连接即表示，您同意遵守<a
                      href="https://www.siteserver.cn/account/agreement.html"
                      target="_blank"
                    >
                      服务条款
                    </a>，并将<a href="https://www.siteserver.cn/account/privacy.html" target="_blank">
                      某些数据和设置
                    </a>同步至
                    SITESERVER.CN
                  </small>
                  <button v-if="loading" class="btn btn-block btn-primary" disabled>
                    正在连接...
                  </button>
                  <button v-else class="btn btn-block btn-primary" @click="btnSubmitClick">
                    创建云连接
                  </button>
                </div>
              </template>
            </div>
          </div>
          <div class="col-lg-3" />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  $_veeValidate: {
    validator: 'new' // give me my own validator scope.
  },
  data() {
    return {
      templateType: 'validate',
      user: null,
      url: null,
      loading: false,
      domain: null,
      path: null,
      adminDirectory: null,
      repositoryOwner: null,
      repositoryName: null,
      description: null,
      isPublic: false
    }
  },
  mounted() {
    this.apiGet()
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

    apiGet() {
      this.$store.commit('loading', true)
      this.$axios.get(this.$api.console.add).then((response) => {
        const res = response.data
        this.user = res.value
      }).catch((error) => {
        this.error = error
      }).then(() => {
        this.$store.commit('loading', false)
      })
    },

    apiValidate() {
      this.alert = null
      this.loading = true
      this.$axios.post(this.$api.console.addActionsValidate, {
        url: this.url
      })
        .then((response) => {
          const res = response.data

          this.domain = res.domain
          this.path = res.path
          this.adminDirectory = res.adminDirectory
          this.repositoryOwner = res.repositoryOwner
          this.templateType = 'submit'
        })
        .catch((error) => {
          this.error = error
        }).then(() => {
          this.loading = false
        })
    },

    apiSubmit() {
      this.alert = null
      this.loading = true
      this.$axios.post(this.$api.console.addActionsSubmit, {
        domain: this.domain,
        path: this.path,
        adminDirectory: this.adminDirectory,
        repositoryOwner: this.repositoryOwner,
        repositoryName: this.repositoryName,
        description: this.description,
        isPublic: this.isPublic
      })
        .then((response) => {
          const res = response.data

          location.href = res.value
        })
        .catch((error) => {
          this.error = error
        }).then(() => {
          this.loading = false
        })
    },

    btnValidateClick() {
      this.alert = null

      this.$validator.validate().then((result) => {
        if (result) {
          this.apiValidate()
        }
      })
    },

    btnSubmitClick() {
      this.alert = null

      this.$validator.validate().then((result) => {
        if (result) {
          this.apiSubmit()
        }
      })
    }
  }
}
</script>
