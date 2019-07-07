<template>
  <div>
    <router-link class="top-menu-link" :to="'/sites'">
      <svg-icon icon-class="site" />
      <span>{{ $t('route.sites') }}</span>
    </router-link>

    <router-link class="top-menu-link" :to="'/plugins'">
      <svg-icon icon-class="plugin" />
      <span>{{ $t('route.plugins') }}</span>
    </router-link>

    <router-link class="top-menu-link" :to="'/settings'">
      <svg-icon icon-class="settings" />
      <span>{{ $t('route.settings') }}</span>
    </router-link>
  </div>
</template>

<script>
export default {
  props: {
    // route object
    menus: {
      type: Array,
      default: () => []
    }
  },
  data() {
    return {
    }
  }
}
</script>

<style lang="scss" scoped>
.svg-icon {
  margin-right: 8px;
}
.top-menu-link {
    float: left;
    overflow: hidden;
    padding: 0 15px;

    &:hover {
      background: rgba(0, 0, 0, .025)
    }
}
</style>
