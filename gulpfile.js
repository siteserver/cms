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
  gulp.src(['./source/SiteServer.Web/bin/*.dll']).pipe(gulp.dest('./build/bin'));
  //home
  gulp.src(['./home/build/**/*', '!./home/build/index.html']).pipe(gulp.dest('./build/home'));
  gulp.src('./home/build/index.html').pipe(replace('"http://localhost:88/api"', "location.protocol + '//' + location.host + '/api'")).pipe(gulp.dest('./build/home'));
  //SiteFiles
  min('./source/SiteServer.Web/SiteFiles/assets/**/*', './build/SiteFiles/assets');
  min(['./source/SiteServer.Web/SiteFiles/Configuration/**/*', '!./source/SiteServer.Web/SiteFiles/Configuration/Menus/WCM/**/*'], './build/SiteFiles/Configuration');
  gulp.src('./source/SiteServer.Web/SiteFiles/UserFiles/home_logo.png').pipe(gulp.dest('./build/SiteFiles/UserFiles'));
  min('./source/SiteServer.Web/SiteFiles/index.htm', './build/SiteFiles');
  //SiteServer
  min('./source/SiteServer.Web/SiteServer/**/*', './build/SiteServer');
  //source/SiteServer.Web/*
  min('./source/SiteServer.Web/安装向导.html', './build');
  min('./source/SiteServer.Web/升级向导.html', './build');
  gulp.src('./source/SiteServer.Web/Global.asax').pipe(gulp.dest('./build'));
  gulp.src('./source/SiteServer.Web/robots.txt').pipe(gulp.dest('./build'));
  gulp.src('./source/SiteServer.Web/Web.Release.config').pipe(rename('Web.config')).pipe(gulp.dest('./build'));
  //exe
  gulp.src(['./source/siteserver/bin/Release/siteserver.exe']).pipe(gulp.dest('./build'));
});

gulp.task('zip', function () {
  gulp.src(['./build/**/*', '!./build/升级向导.html']).pipe(zip('siteserver_install.zip')).pipe(gulp.dest('./'));
  gulp.src(['./build/**/*', '!./build/安装向导.html']).pipe(zip('siteserver_upgrade.zip')).pipe(gulp.dest('./'));
});

// 生成 install.sql
// 编译 Release
// cd home
// rm -rf .g3 build
// g3 build
// cd ..
// rm -rf build siteserver_install.zip siteserver_upgrade.zip
// gulp build
// gulp zip