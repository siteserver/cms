<template>
  <el-menu mode="horizontal" @select="handleSelect">
    <a v-for="menu in menus" :key="menu.id" class="top-menu-link" href="javascript:;" @click="handleMenuClick(menu.id)">
      <el-menu-item :index="menu.id">
        {{ menu.text }}
      </el-menu-item>
    </a>
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
      activeIndex: '1',
      activeIndex2: '1'
    }
  },
  methods: {
    handleSelect(key, keyPath) {
      // console.log(key, keyPath)
    },
    handleMenuClick(menuId) {
      this.$store.dispatch('app/setTopMenu', menuId)
    }
  }
}
</script>

<style>
.top-menu-link {
    display: inline-block;
    overflow: hidden;
}
</style>
