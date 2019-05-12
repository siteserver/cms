<template>
  <div class="m-2">
    <app-notify />
    <loading :active.sync="loading" :is-full-page="true" loader="dots" color="#00b19d" />
    <nuxt />
  </div>
</template>

<script>
import AppNotify from '@/components/AppNotify.vue'
import Loading from 'vue-loading-overlay'

export default {
  name: 'LayerLayout',
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
