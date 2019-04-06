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
                CMS 云连接
                <div class="page-title-subheading">
                  请选择一个已建立云连接的CMS系统，以进入管理界面，或者使用云连接添加现有的 SiteServer CMS 系统
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="row">
          <div class="col-lg-4" />
          <div class="col-lg-4">
            <n-link v-for="repo in repos" :key="repo.repositoryId" :to="'/' + repo.owner + '/' + repo.name">
              <div class="card-box widget-user">
                <img :src="avatarUrl" class="img-responsive rounded-circle">
                <div class="wid-u-info">
                  <h4 class="pt-2 text-primary">
                    {{ repo.repositoryId }}
                  </h4>
                  <p class="text-dark">
                    {{ repo.cmsId }}
                  </p>
                </div>
              </div>
            </n-link>
            <n-link :to="getAddUrl()">
              <div class="card-box widget-user text-white text-center pt-4 bg-happy-fisher">
                <i class="pe-7s-plus" style="font-size: 28px;" />
                <h4 class="pt-2">
                  添加 CMS 云连接
                </h4>
                <small>
                  使用云连接添加现有的 SiteServer CMS 系统
                </small>
              </div>
            </n-link>
          </div>
          <div class="col-lg-4" />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { mapGetters } from 'vuex'

export default {
  data() {
    return {
      error: null
    }
  },

  computed: {
    ...mapGetters(['isLoggedIn', 'userName', 'displayName', 'avatarUrl', 'isVip'])
  },

  async asyncData({ app, params }) {
    const data = await app.$api.owner.get({
      owner: params.owner
    })
    return { repos: data.value }
  },

  methods: {
    getAddUrl() {
      return `/${this.userName}/add`
    }

    // apiGet() {
    //   let data = await $axios.$get(this.apiUrl)

    //   this.$axios.get(this.apiUrl).then((response) => {
    //     const res = response.data
    //     this.repos = res.value
    //   }).catch((error) => {
    //     this.error = error
    //   })
    // }
  }
}
</script>
