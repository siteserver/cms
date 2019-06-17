<template>
  <div class="app-main">
    <app-me-menus />
    <div class="app-main__outer">
      <div class="app-main__inner">
        <div class="row justify-content-center">
          <div class="col-md-6 co-sm-12">
            <div v-show="files.length && edit" class="card-box">
              <div v-if="files.length" style="max-width: 100%">
                <img ref="editImage" style="max-width: 100%" :src="files[0].url">
              </div>
              <div class="text-center p-4">
                <button type="submit" class="btn btn-primary mr-2" @click.prevent="editSave">
                  保 存
                </button>
                <button type="button" class="btn btn-secondary" @click.prevent="$refs.upload.clear">
                  取 消
                </button>
              </div>
            </div>
            <div v-show="!edit" class="main-card mb-3 card">
              <div class="card-body">
                <h5 class="card-title">
                  个人资料
                </h5>
                <hr>
                <form @submit="btnSubmitClick">
                  <div class="form-row">
                    <div class="col-md-4">
                      <no-ssr placeholder="Loading...">
                        <file-upload
                          ref="upload"
                          v-model="files"
                          extensions="gif,jpg,jpeg,png,webp"
                          accept="image/png,image/gif,image/jpeg,image/webp"
                          name="avatar"
                          style="overflow: inherit;"
                          :post-action="uploadUrl"
                          :headers="{'Authorization': `Bearer ${token}`}"
                          :drop="!edit"
                          @input-filter="inputFilter"
                          @input-file="inputFile"
                        >
                          <div class="position-relative form-group">
                            <div class="thumb-xl member-thumb m-b-10 center-block text-center">
                              <img :src="user.avatarUrl" class="rounded-circle img-thumbnail">
                            </div>
                            <div class="text-center text-center d-none d-md-block" style="margin-top: -120px;">
                              <a href="javascript:;" class="btn btn-success btn-sm">
                                <i class="fa fa-cloud-upload" /> 点击更换头像
                              </a>
                            </div>
                          </div>
                        </file-upload>
                      </no-ssr>
                    </div>
                    <div class="col-md-8">
                      <div class="position-relative form-group">
                        <label for="exampleState" class="">
                          账号
                        </label>
                        <input v-model="user.userName" disabled type="text" class="form-control">
                        <small class="text-muted">
                          用户的唯一标识符
                        </small>
                      </div>
                      <div class="position-relative form-group">
                        <label for="exampleState" class="">
                          显示名
                          <small v-show="isError('displayName')" class="text-danger">
                            {{ errorMessage('displayName') }}
                          </small>
                        </label>
                        <input
                          v-model="user.displayName"
                          v-validate="'required'"
                          name="displayName"
                          data-vv-as="显示名"
                          :class="{'is-invalid': isError('displayName') }"
                          type="text"
                          class="form-control"
                        >
                      </div>
                    </div>
                  </div>
                  <div class="form-row">
                    <label for="bio" class="">
                      个人简介
                    </label>
                    <textarea v-model="user.bio" class="form-control" />
                  </div>
                  <button type="button" class="mt-2 btn btn-primary" @click="btnSubmitClick">
                    保 存
                  </button>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style>
.cropper-container {
  font-size: 0;
  line-height: 0;
  position: relative;
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  user-select: none;
  direction: ltr;
  -ms-touch-action: none;
  touch-action: none;
}
.cropper-container img {
  display: block;
  min-width: 0 !important;
  max-width: none !important;
  min-height: 0 !important;
  max-height: none !important;
  width: 100%;
  height: 100%;
  image-orientation: 0deg;
}
.cropper-wrap-box,
.cropper-canvas,
.cropper-drag-box,
.cropper-crop-box,
.cropper-modal {
  position: absolute;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
}
.cropper-wrap-box {
  overflow: hidden;
}
.cropper-drag-box {
  opacity: 0;
  background-color: #fff;
}
.cropper-modal {
  opacity: 0.5;
  background-color: #000;
}
.cropper-view-box {
  display: block;
  overflow: hidden;
  width: 100%;
  height: 100%;
  outline: 1px solid #39f;
  outline-color: rgba(51, 153, 255, 0.75);
}
.cropper-dashed {
  position: absolute;
  display: block;
  opacity: 0.5;
  border: 0 dashed #eee;
}
.cropper-dashed.dashed-h {
  top: 33.33333%;
  left: 0;
  width: 100%;
  height: 33.33333%;
  border-top-width: 1px;
  border-bottom-width: 1px;
}
.cropper-dashed.dashed-v {
  top: 0;
  left: 33.33333%;
  width: 33.33333%;
  height: 100%;
  border-right-width: 1px;
  border-left-width: 1px;
}
.cropper-center {
  position: absolute;
  top: 50%;
  left: 50%;
  display: block;
  width: 0;
  height: 0;
  opacity: 0.75;
}
.cropper-center:before,
.cropper-center:after {
  position: absolute;
  display: block;
  content: " ";
  background-color: #eee;
}
.cropper-center:before {
  top: 0;
  left: -3px;
  width: 7px;
  height: 1px;
}
.cropper-center:after {
  top: -3px;
  left: 0;
  width: 1px;
  height: 7px;
}
.cropper-face,
.cropper-line,
.cropper-point {
  position: absolute;
  display: block;
  width: 100%;
  height: 100%;
  opacity: 0.1;
}
.cropper-face {
  top: 0;
  left: 0;
  background-color: #fff;
}
.cropper-line {
  background-color: #39f;
}
.cropper-line.line-e {
  top: 0;
  right: -3px;
  width: 5px;
  cursor: e-resize;
}
.cropper-line.line-n {
  top: -3px;
  left: 0;
  height: 5px;
  cursor: n-resize;
}
.cropper-line.line-w {
  top: 0;
  left: -3px;
  width: 5px;
  cursor: w-resize;
}
.cropper-line.line-s {
  bottom: -3px;
  left: 0;
  height: 5px;
  cursor: s-resize;
}
.cropper-point {
  width: 5px;
  height: 5px;
  opacity: 0.75;
  background-color: #39f;
}
.cropper-point.point-e {
  top: 50%;
  right: -3px;
  margin-top: -3px;
  cursor: e-resize;
}
.cropper-point.point-n {
  top: -3px;
  left: 50%;
  margin-left: -3px;
  cursor: n-resize;
}
.cropper-point.point-w {
  top: 50%;
  left: -3px;
  margin-top: -3px;
  cursor: w-resize;
}
.cropper-point.point-s {
  bottom: -3px;
  left: 50%;
  margin-left: -3px;
  cursor: s-resize;
}
.cropper-point.point-ne {
  top: -3px;
  right: -3px;
  cursor: ne-resize;
}
.cropper-point.point-nw {
  top: -3px;
  left: -3px;
  cursor: nw-resize;
}
.cropper-point.point-sw {
  bottom: -3px;
  left: -3px;
  cursor: sw-resize;
}
.cropper-point.point-se {
  right: -3px;
  bottom: -3px;
  width: 20px;
  height: 20px;
  cursor: se-resize;
  opacity: 1;
}
@media (min-width: 768px) {
  .cropper-point.point-se {
    width: 15px;
    height: 15px;
  }
}
@media (min-width: 992px) {
  .cropper-point.point-se {
    width: 10px;
    height: 10px;
  }
}
@media (min-width: 1200px) {
  .cropper-point.point-se {
    width: 5px;
    height: 5px;
    opacity: 0.75;
  }
}
.cropper-point.point-se:before {
  position: absolute;
  right: -50%;
  bottom: -50%;
  display: block;
  width: 200%;
  height: 200%;
  content: " ";
  opacity: 0;
  background-color: #39f;
}
.cropper-invisible {
  opacity: 0;
}
.cropper-bg {
  background-image: url("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAAA3NCSVQICAjb4U/gAAAABlBMVEXMzMz////TjRV2AAAACXBIWXMAAArrAAAK6wGCiw1aAAAAHHRFWHRTb2Z0d2FyZQBBZG9iZSBGaXJld29ya3MgQ1M26LyyjAAAABFJREFUCJlj+M/AgBVhF/0PAH6/D/HkDxOGAAAAAElFTkSuQmCC");
}
.cropper-hide {
  position: absolute;
  display: block;
  width: 0;
  height: 0;
}
.cropper-hidden {
  display: none !important;
}
.cropper-move {
  cursor: move;
}
.cropper-crop {
  cursor: crosshair;
}
.cropper-disabled .cropper-drag-box,
.cropper-disabled .cropper-face,
.cropper-disabled .cropper-line,
.cropper-disabled .cropper-point {
  cursor: not-allowed;
}
</style>

<script>
import Cropper from 'cropperjs'
import FileUpload from 'vue-upload-component'
import AppMeMenus from '@/components/AppMeMenus.vue'

export default {
  $_veeValidate: {
    validator: 'new' // give me my own validator scope.
  },

  components: {
    FileUpload,
    AppMeMenus
  },

  data() {
    return {
      uploadUrl: null,
      token: null,
      files: [],
      edit: false,
      cropper: false
    }
  },

  watch: {
    edit(value) {
      if (value) {
        this.$nextTick(function () {
          if (!this.$refs.editImage) {
            return
          }
          const cropper = new Cropper(this.$refs.editImage, {
            aspectRatio: 1 / 1,
            viewMode: 1
          })
          this.cropper = cropper
        })
      } else if (this.cropper) {
        this.cropper.destroy()
        this.cropper = false
      }
    }
  },

  async asyncData({ app }) {
    const data = await app.$api.me.get()
    return { user: data.value, token: data.token }
  },

  created: function () {
    this.uploadUrl = this.$api.me.at(`/actions/uploadAvatar`)
  },

  methods: {
    editSave() {
      this.edit = false
      const oldFile = this.files[0]
      const binStr = atob(this.cropper.getCroppedCanvas().toDataURL(oldFile.type).split(',')[1])
      const arr = new Uint8Array(binStr.length)
      for (let i = 0; i < binStr.length; i++) {
        arr[i] = binStr.charCodeAt(i)
      }
      const file = new File([arr], oldFile.name, { type: oldFile.type })
      this.$store.commit('loading', true)
      this.$refs.upload.update(oldFile.id, {
        file,
        type: file.type,
        size: file.size,
        active: true
      })
    },

    alert(message) {
      alert(message)
    },

    inputFile(newFile, oldFile, prevent) {
      if (newFile && !oldFile) {
        this.$nextTick(function () {
          this.edit = true
        })
      }
      if (!newFile && oldFile) {
        this.edit = false
      }
      if (newFile && oldFile && newFile.xhr && newFile.success !== oldFile.success) {
        this.user = newFile.response.value
        this.$store.commit('login', this.user)
        this.$store.commit('loading', false)
        this.$store.commit('notify', {
          type: 'success',
          title: '头像上传成功'
        })
      }
    },

    inputFilter(newFile, oldFile, prevent) {
      if (newFile && !oldFile) {
        if (!/\.(gif|jpg|jpeg|png|webp)$/i.test(newFile.name)) {
          this.alert('Your choice is not a picture')
          return prevent()
        }
      }
      if (newFile && (!oldFile || newFile.file !== oldFile.file)) {
        newFile.url = ''
        const URL = window.URL || window.webkitURL
        if (URL && URL.createObjectURL) {
          newFile.url = URL.createObjectURL(newFile.file)
        }
      }
    },

    isError(name) {
      return this.errors ? this.errors.has(name) : false
    },

    errorMessage(name) {
      return this.errors ? this.errors.first(name) : ''
    },

    async apiSubmit() {
      this.$store.commit('loading', true)
      const data = await this.$api.me.put({
        displayName: this.user.displayName,
        bio: this.user.bio
      })
      this.user = data.value
      this.$store.commit('login', this.user)
      this.$store.commit('loading', false)
      this.$store.commit('notify', {
        type: 'success',
        title: '个人资料保存成功'
      })
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
