<template>
  <div class="example-avatar">
    <div v-show="$refs.upload && $refs.upload.dropActive" class="drop-active">
      <h3>Drop files to upload</h3>
    </div>
    <div v-show="!edit" class="avatar-upload">
      <div class="text-center p-2">
        <label for="avatar">
          <img :src="files.length ? files[0].url : 'https://www.gravatar.com/avatar/default?s=200&r=pg&d=mm'" class="rounded-circle">
          <h4 class="pt-2">
            or<br>Drop files anywhere to upload
          </h4>
        </label>
      </div>
      <div class="text-center p-2">
        <file-upload
          ref="upload"
          v-model="files"
          extensions="gif,jpg,jpeg,png,webp"
          accept="image/png,image/gif,image/jpeg,image/webp"
          name="avatar"
          class="btn btn-primary"
          post-action="/upload/post"
          :drop="!edit"
          @input-filter="inputFilter"
          @input-file="inputFile"
        >
          Upload avatar
        </file-upload>
      </div>
    </div>

    <div v-show="files.length && edit" class="avatar-edit">
      <div v-if="files.length" class="avatar-edit-image">
        <img ref="editImage" :src="files[0].url">
      </div>
      <div class="text-center p-4">
        <button type="button" class="btn btn-secondary" @click.prevent="$refs.upload.clear">
          Cancel
        </button>
        <button type="submit" class="btn btn-primary" @click.prevent="editSave">
          Save
        </button>
      </div>
    </div>
    <div class="pt-5">
      Source code: <a href="https://github.com/lian-yue/vue-upload-component/blob/master/docs/views/examples/Avatar.vue">
        /docs/views/examples/Avatar.vue
      </a>
    </div>
  </div>
</template>
<style>
.example-avatar .avatar-upload .rounded-circle {
  width: 200px;
  height: 200px;
}
.example-avatar .text-center .btn {
  margin: 0 .5rem
}
.example-avatar .avatar-edit-image {
  max-width: 100%
}
.example-avatar .drop-active {
  top: 0;
  bottom: 0;
  right: 0;
  left: 0;
  position: fixed;
  z-index: 9999;
  opacity: .6;
  text-align: center;
  background: #000;
}
.example-avatar .drop-active h3 {
  margin: -.5em 0 0;
  position: absolute;
  top: 50%;
  left: 0;
  right: 0;
  -webkit-transform: translateY(-50%);
  -ms-transform: translateY(-50%);
  transform: translateY(-50%);
  font-size: 40px;
  color: #fff;
  padding: 0;
}
</style>

<script>
import Cropper from 'cropperjs'
import FileUpload from 'vue-upload-component'
export default {
  components: {
    FileUpload
  },
  data() {
    return {
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
    }
  }
}
</script>
