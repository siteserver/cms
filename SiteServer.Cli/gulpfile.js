var fs = require("fs");
var path = require("path");
var gulp = require("gulp");
var child_process = require('child_process');
var through = require('through2');
var mime = require('mime');
var OSS = require('ali-oss').Wrapper;

var accessKeyId = process.env.accessKeyId;
var accessKeySecret = process.env.accessKeySecret;

var client = new OSS({
  accessKeyId: accessKeyId,
  accessKeySecret: accessKeySecret,
  bucket: 'siteserver-cn',
  region: 'oss-cn-qingdao'
});
var ossPrefix = 'docs/cli';

var install = function () {
  child_process.execSync('gitbook install docs', { stdio: [0, 1, 2] });
}

var build = function () {
  child_process.execSync('gitbook build docs', { stdio: [0, 1, 2] });
}

var copy = function () {
  gulp
    .src(["./docs/_book/**/*"])
    .pipe(gulp.dest("./build"));

  gulp
    .src(["./docs/gitbook/**/*"])
    .pipe(gulp.dest("./build/gitbook"));

  console.log('build completed');
}

var upload = function (relatedPath, buff, contentType) {
  client.put(relatedPath, buff, {
    mime: contentType ? contentType : mime.getType(relatedPath)
  }).catch(function (err) {
    console.log('error: %j', err);
  });
}

gulp.task("build", function () {
  install();
  build();
  copy();
});

gulp.task('publish', function () {
  var dirname = __dirname;

  return gulp.src('./build/**/*')
    .pipe(through.obj(function (file, enc, cb) {
      if (file.contents) {
        var filePath = file.path;
        var relatedPath = file.path.substr(dirname.length);
        relatedPath = relatedPath.replace(/\\/g, '/').replace(/(^\/*)|(\/*$)/g, "").replace('build/', ossPrefix + '/');
        console.log(relatedPath);

        upload(relatedPath, file.contents);
        if (filePath.endsWith('index.html')) {
          console.log(relatedPath.substr(0, relatedPath.length - 11));
          upload(relatedPath.substr(0, relatedPath.length - 11), new Buffer('<html><head><meta charset="utf-8"><script>location.href="/' + relatedPath.substr(0, relatedPath.length - 11) + '/index.html";</script></head><body></body></html>'), 'text/html');
        }
      }

      cb(null, file);
    }));
});