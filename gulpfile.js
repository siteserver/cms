var gulp = require('gulp');
var minify = require('gulp-minifier');
var rimraf = require('rimraf');
var rename = require("gulp-rename");
var replace = require('gulp-replace');
var zip = require('gulp-zip');
var argv = require('yargs').argv;

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

function build(beta) {
  var version = process.env.APPVEYOR_BUILD_VERSION;
  if (beta) {
    version += '-beta';
  } else {
    version += '-rc2';
  }
  console.log('build SiteServer CMS started, version: ' + version);

  //nuspec
  gulp.src('./SS.CMS.nuspec').pipe(replace('$version$', version)).pipe(gulp.dest('./build'));

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
  gulp.src('./SiteServer.Web/robots.txt').pipe(gulp.dest('./build'));
  gulp.src('./SiteServer.Web/Web.Release.config').pipe(rename('Web.config')).pipe(gulp.dest('./build'));

  console.log('build SiteServer CMS successed!');
}

gulp.task('release', function () {
  build(false);
});

gulp.task('preview', function () {
  build(true);
});

gulp.task('zip', function () {
  gulp.src(['./build/**/*', '!./build/SS.CMS.nuspec']).pipe(zip('siteserver_install.zip')).pipe(gulp.dest('./'));
});

// 编译 Release
// rm -rf build siteserver_install.zip
// gulp build
// gulp zip