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
                  <n-link :to="getTabUrl('settings/general')" class="nav-link text-primary">
                    常 规
                  </n-link>
                </li>
                <li class="nav-item">
                  <n-link :to="getTabUrl('settings/connect')" class="nav-link text-dark">
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
                  系统所有者
                </div>
              </div>
              <div class="card-body">
                <div class="widget-content p-0">
                  <div class="widget-content-wrapper">
                    <div class="widget-content-left mr-3">
                      <div class="widget-content-left">
                        <img width="40" class="rounded-circle" :src="owner.avatarUrl">
                      </div>
                    </div>
                    <div class="widget-content-left flex2">
                      <div class="widget-heading">
                        {{ owner.userName }}
                      </div>
                      <div class="widget-subheading opacity-7">
                        {{ owner.bio }}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div class="mb-3 card">
              <div class="card-header">
                <div class="card-header-title">
                  数据同步
                </div>
              </div>
              <template v-if="syncing">
                <div class="card-body">
                  <p><small>即将开始完整同步</small></p>
                </div>
                <div class="d-block card-footer">
                  <a href="javascript:void(0);" class="btn" @click="btnSubmitClick">
                    取消手动同步
                  </a>
                </div>
              </template>
              <template v-else>
                <div class="card-body">
                  <p>云连接数据同步功能可以让CMS系统在 SITESERVER.CN 的数据保持为最新，数据将定期从您的站点发送至 SITESERVER.CN 云中心，以提供更快速的体验。如果您怀疑丢失了某些数据，可以开始手动同步。</p>
                  <p><small>上次完全同步时间：17 天前</small></p>
                </div>
                <div class="d-block card-footer">
                  <a href="javascript:void(0);" class="btn btn-primary" @click="btnSyncClick">
                    开始手动同步
                  </a>
                </div>
              </template>
            </div>

            <div class="mb-3 card">
              <div class="card-header">
                <div class="card-header-title">
                  断开连接
                </div>
              </div>
              <div class="card-body">
                <p>
                  从 SITESERVER.CN 云中心断开连接
                </p>
                <p>
                  您的站点将不再向 SITESERVER.CN 云中心发送数据，并且云中心相关功能将停止运行。
                </p>
              </div>
              <div class="d-block card-footer">
                <a href="javascript:void(0);" class="btn btn-danger" @click="btnSubmitClick">
                  断开连接
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
  name: 'PageConnect',
  components: {
    AppRepoMenus
  },

  $_veeValidate: {
    validator: 'new' // give me my own validator scope.
  },

  data() {
    return {
      syncing: false
    }
  },

  async asyncData({ app, params }) {
    const data = await app.$api.repoSettingsConnect.get({
      owner: params.owner,
      name: params.name
    })
    return {
      repo: data.value,
      owner: data.owner
    }
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

    async apiSync() {
      this.$store.commit('loading', true)
      const data = await this.$api.repoSettingsConnect.postAt(`actions/sync`, {
        owner: this.$route.params.owner,
        name: this.$route.params.name
      })
      this.$store.commit('loading', false)
      if (data.value) {
        this.$store.commit('notify', {
          type: 'success',
          title: '同步操作已提交，系统将在同步完成之后通知您'
        })
      }
    },

    btnSyncClick() {
      this.apiSync()
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
