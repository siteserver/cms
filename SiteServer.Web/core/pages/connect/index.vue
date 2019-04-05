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
                CMS 系统<div class="page-title-subheading">
                  请选择一个CMS系统，以进入管理界面
                </div>
              </div>
            </div>
            <div class="page-title-actions">
              <button type="button" class="btn-shadow mr-3 btn btn-light">
                <i class="fa fa-arrow-left" /> 返 回
              </button>
            </div>
          </div>
        </div>
        <div class="row">
          <div class="col-lg-3" />
          <div class="col-lg-6">
            <div class="card-box mt-3">
              <div class="m-4 text-center">
                <h3>正在完成设置</h3>
                <h5 class="form-text text-muted">
                  云连接即将完成设置
                </h5>
              </div>
              <div class="m-4 text-center">
                <img class="img-responsive rounded-circle" style="height: 90px; width: 90px;" :src="this.$store.avatarUrl">
                <div class="form-group">
                  <label class="col-form-label">
                    正在以 {{ repo ? repo.owner : '' }} 的身份进行连接
                  </label>
                </div>
              </div>
              <div class="text-center mb-2">
                <template v-if="templateType == 'ready'">
                  <p class="form-text text-muted mb-2">
                    正在准备授权
                  </p>
                </template>
                <template v-else-if="templateType == 'create'">
                  <p class="form-text text-muted mb-2">
                    正在授权您的连接
                  </p>
                </template>
              </div>
              <div class="text-center mb-2">
                <i class="fa fa-spin fa-circle-o-notch text-primary" />
              </div>
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
  data() {
    return {
      templateType: 'ready',
      user: null,
      repo: null
    }
  },
  mounted() {
    if (this.$route.query.guid) {
      this.apiGet()
    }
  },
  methods: {
    apiGet() {
      this.$store.commit('loading', true)
      this.$axios.get(this.$api.console.connect, {
        params: {
          guid: this.$route.query.guid
        }
      }).then((response) => {
        const res = response.data
        this.user = res.value
        this.repo = res.repo
        this.templateType = 'create'
        this.apiCreate()
      }).catch((error) => {
        this.error = error
      }).then(() => {
        this.$store.commit('loading', false)
      })
    },

    apiCreate() {
      this.$store.commit('loading', true)
      this.$axios.post(this.$api.console.connect, {
        guid: this.$route.query.guid
      })
        .then((response) => {
          location.href = '/cms/?owner=' + this.repo.owner + '&name=' + this.repo.name
        })
        .catch(function (error) {
          this.error = error
        }).then(function () {
          this.$store.commit('loading', false)
        })
    }
  }
}
</script>
