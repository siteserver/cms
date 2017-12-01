var gulp = require('gulp');
var minify = require('gulp-minifier');
var rimraf = require('rimraf');
var rename = require("gulp-rename");
var replace = require('gulp-replace');
var zip = require('gulp-zip');

function min(src, dest) {
  var g = gulp.src(src);
  if (src.indexOf('.min.') !== -1) {
    g.pipe(gulp.dest(dest));
  } else {
    g.pipe(minify({
      minify: true,
      collapseWhitespace: true,
      conservativeCollapse: true,
      minifyJS: true,
      minifyCSS: true,
      minifyHTML: true
    })).pipe(gulp.dest(dest));
  }
}

gulp.task('build', function () {
  //bin
  gulp.src(['./SiteServer.Web/bin/*.dll']).pipe(gulp.dest('./build/bin'));
  //SiteFiles
  min('./SiteServer.Web/SiteFiles/assets/**/*', './build/SiteFiles/assets');
  gulp.src('./SiteServer.Web/SiteFiles/UserFiles/home_logo.png').pipe(gulp.dest('./build/SiteFiles/UserFiles'));
  min('./SiteServer.Web/SiteFiles/index.htm', './build/SiteFiles');
  //SiteServer
  min('./SiteServer.Web/SiteServer/**/*', './build/SiteServer');
  //SiteServer.Web/*
  min('./SiteServer.Web/安装向导.html', './build');
  min('./SiteServer.Web/升级向导.html', './build');
  gulp.src('./SiteServer.Web/Global.asax').pipe(gulp.dest('./build'));
  gulp.src('./SiteServer.Web/robots.txt').pipe(gulp.dest('./build'));
  gulp.src('./SiteServer.Web/Web.Release.config').pipe(rename('Web.config')).pipe(gulp.dest('./build'));
  //exe
  gulp.src(['./siteserver/bin/Release/siteserver.exe']).pipe(gulp.dest('./build'));
});

gulp.task('zip', function () {
  gulp.src(['./build/**/*', '!./build/升级向导.html']).pipe(zip('siteserver_install.zip')).pipe(gulp.dest('./'));
  gulp.src(['./build/**/*', '!./build/安装向导.html']).pipe(zip('siteserver_upgrade.zip')).pipe(gulp.dest('./'));
});

// 编译 Release
// rm -rf build siteserver_install.zip siteserver_upgrade.zip
// gulp build
// gulp zip