<template>
  <div class="app-main">
    <app-repo-menus />
    <div class="app-main__outer">
      <div class="app-main__inner" />
    </div>
  </div>
</template>

<script>
import AppRepoMenus from '@/components/AppRepoMenus.vue'

export default {
  components: {
    AppRepoMenus
  },

  data() {
    return {

    }
  },

  // mounted() {
  //   this.apiGet()
  //   this.$notify({
  //     group: 'app',
  //     title: 'Important message',
  //     text: 'Hello user! This is a notification!'
  //   })
  // },

  async asyncData({ app, params }) {
    const data = await app.$api.repo.get({
      owner: params.owner,
      name: params.name
    })

    return { repo: data.value }
  },

  methods: {
    // apiGet() {
    //   this.$store.commit('loading', true)
    //   this.$axios.get(this.$api.console.cms).then((response) => {
    //     const res = response.data
    //     this.user = res.value
    //     this.repos = res.repos
    //   }).catch((error) => {
    //     this.error = error
    //   }).then(() => {
    //     this.$store.commit('loading', false)
    //   })
    // },

    getMenuUrl(name) {
      return `${this.$route.path}/${name}`
    }
  }
}
</script>
