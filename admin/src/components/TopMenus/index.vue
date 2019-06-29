<template>
  <el-menu mode="horizontal">
    <router-link class="top-menu-link" :to="'/sites'">
      <el-menu-item index="sites">
        <svg-icon icon-class="site" />
        <span>{{ $t('route.sites') }}</span>
      </el-menu-item>
    </router-link>
    <router-link class="top-menu-link" :to="'/plugins'">
      <el-menu-item index="plugins">
        <svg-icon icon-class="plugin" />
        <span>{{ $t('route.plugins') }}</span>
      </el-menu-item>
    </router-link>
    <router-link class="top-menu-link" :to="'/settings'">
      <el-menu-item index="settings">
        <svg-icon icon-class="settings" />
        <span>{{ $t('route.settings') }}</span>
      </el-menu-item>
    </router-link>
  </el-menu>
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

<style>
.svg-icon {
  margin-right: 8px;
}
.top-menu-link {
    display: inline-block;
    overflow: hidden;
}
</style>
