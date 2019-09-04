<template>
  <div ref="rightPanel" :class="{show: show}" class="rightPanel-container">
    <div class="rightPanel-background" />
    <div class="rightPanel" :style="{'max-width': maxWidth}">
      <div class="handle-button" @click="close">
        <i class="el-icon-close" />
      </div>
      <div class="rightPanel-items">
        <el-page-header :content="title" @back="close" />
        <el-divider />
        <slot />
      </div>
    </div>
  </div>
</template>

<script>
import { addClass, removeClass } from '@/utils'
import { setTimeout } from 'timers'

export default {
  name: 'RightPanel',
  props: {
    title: {
      default: '',
      type: String
    },
    width: {
      default: '50%',
      type: String
    }
  },
  data() {
    return {
      show: false
    }
  },
  computed: {
    maxWidth: {
      get() {
        return this.$store.state.app.device === 'desktop' ? this.$props.width : '80%'
      }
    }
  },
  watch: {
    show(value) {
      if (value) {
        this.addEventClick()
      }
      if (value) {
        addClass(document.body, 'showRightPanel')
      } else {
        removeClass(document.body, 'showRightPanel')
      }
    }
  },
  mounted() {
    this.insertToBody()
    setTimeout(() => {
      this.show = true
    }, 10)
  },
  beforeDestroy() {
    const elx = this.$refs.rightPanel
    elx.remove()
  },
  methods: {
    addEventClick() {
      window.addEventListener('click', this.closeSidebar)
    },
    closeSidebar(evt) {
      const parent = evt.target.closest('.rightPanel')
      if (!parent) {
        this.show = false
        this.$emit('close')
        window.removeEventListener('click', this.closeSidebar)
      }
    },
    insertToBody() {
      const elx = this.$refs.rightPanel
      const body = document.querySelector('body')
      body.insertBefore(elx, body.firstChild)
    },
    close() {
      this.show = false
      this.$emit('close')
    }
  }
}
</script>

<style>
.showRightPanel {
  overflow: hidden;
  position: relative;
  width: 100%;
}
</style>

<style lang="scss" scoped>
.rightPanel-background {
  position: fixed;
  top: 0;
  left: 0;
  opacity: 0;
  transition: opacity .3s cubic-bezier(.7, .3, .1, 1);
  background: rgba(0, 0, 0, .2);
  z-index: -1;
}

.rightPanel {
  width: 80%;
  height: 100vh;
  position: fixed;
  top: 0;
  right: 0;
  box-shadow: 0px 0px 15px 0px rgba(0, 0, 0, .05);
  transition: all .25s cubic-bezier(.7, .3, .1, 1);
  transform: translate(100%);
  background: #fff;
  z-index: 2000;
}

.show {
  transition: all .3s cubic-bezier(.7, .3, .1, 1);

  .rightPanel-background {
    z-index: 1999;
    opacity: 1;
    width: 100%;
    height: 100%;
  }

  .rightPanel {
    transform: translate(0);
  }
}

.handle-button {
  width: 48px;
  height: 48px;
  position: absolute;
  left: -48px;
  text-align: center;
  font-size: 24px;
  border-radius: 6px 0 0 6px !important;
  z-index: 0;
  pointer-events: auto;
  cursor: pointer;
  color: #fff;
  line-height: 48px;
  top: 250px;
  background-color: #42b983;
  i {
    font-size: 24px;
    line-height: 48px;
  }
}

.rightPanel-items {
  padding: 15px;
  max-height: 100%;
  overflow: auto;
}
</style>
