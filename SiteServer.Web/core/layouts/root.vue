<template>
  <div>
    <app-notify />
    <loading :active.sync="loading" :is-full-page="true" loader="dots" color="#00b19d" />
    <div v-if="$nuxt.isOffline" class="alert alert-danger mt-5 mx-2">
      系统检测到网络连接已中断，正在重试...
    </div>
    <nuxt />
  </div>
</template>

<script>
import 'vue-loading-overlay/dist/vue-loading.css'
import 'assets/css/menu.css'
import AppNotify from '@/components/AppNotify.vue'
import Loading from 'vue-loading-overlay'

export default {
  name: 'RootLayout',
  components: {
    AppNotify,
    Loading
  },
  data() {
    return {}
  },
  computed: {
    loading() {
      return this.$store.state.loading
    }
  }
}
</script>
