<template>
  <div class="notifications" :style="styles">
    <div v-for="item in notifyList" :key="item.id" class="notification-wrapper" @click="remove(item)">
      <div :class="notifyClass(item)">
        <div v-if="item.title" class="notification-title">
          {{ item.title }}
        </div>
        <div class="notification-content">
          {{ item.text }}
        </div>
      </div>
    </div>
  </div>
</template>
<script>

export default {
  name: 'AppNotify',
  computed: {
    notifyList() {
      return this.$store.state.notifyList
    },
    styles() {
      const styles = {
        width: '300px',
        top: '47px',
        right: '5px'
      }
      return styles
    }
  },
  methods: {
    notifyClass(item) {
      this.$store.commit('loading', false)
      if (item.type !== 'error') {
        setTimeout(() => {
          this.$store.commit('removeNotify', item)
        }, 3000)
      }
      return [
        'notification',
        'vue-notification',
        item.type
      ]
    },

    remove(item) {
      this.$store.commit('removeNotify', item)
    }
  }
}
</script>
<style>
.notifications {
  display: block;
  position: fixed;
  z-index: 5000;
}
.notification-wrapper {
  display: block;
  overflow: hidden;
  width: 100%;
  margin: 0;
  padding: 0;
}
.notification {
  display: block;
  box-sizing: border-box;
  background: white;
  text-align: left;
}
.notification-title {
  font-weight: 600;
}
.vue-notification {
  font-size: 12px;
  padding: 15px;
  margin: 0;
  color: white;
  background: #44A4FC;
  border-left: 5px solid #187FE7;
}
.vue-notification.warn {
  background: #ffb648;
  border-left-color: #f48a06;
}
.vue-notification.error {
  background: #E54D42;
  border-left-color: #B82E24;
}
.vue-notification.success {
  background: #68CD86;
  border-left-color: #42A85F;
}
.vn-fade-enter-active, .vn-fade-leave-active, .vn-fade-move  {
  transition: all .5s;
}
.vn-fade-enter, .vn-fade-leave-to {
  opacity: 0;
}
</style>
