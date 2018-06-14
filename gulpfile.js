var fs = require("fs");
var path = require("path");
var gulp = require("gulp");
var minify = require("gulp-minifier");
var rimraf = require("rimraf");
var rename = require("gulp-rename");
var replace = require("gulp-replace");
var zip = require("gulp-zip");
var filter = require("gulp-filter");
var runSequence = require("run-sequence");

var version = process.env.APPVEYOR_BUILD_VERSION || '0.0.0';

function getDependencies() {
  var str = "";

  var dirs = fs.readdirSync("./packages").filter(function(file) {
    return fs.statSync("./packages/" + file).isDirectory();
  });
  for (var dir of dirs) {
    var items = dir.split(".");
    var index = 0;
    for (var i = 0; i < items.length; i++) {
      var isNumber = !isNaN(parseFloat(items[i])) && isFinite(items[i]);
      if (isNumber) {
        index = i;
        break;
      }
    }
    var id = items.slice(0, index).join(".");
    var version = items.slice(index).join(".");
    str += '<dependency id="' + id + '" version="' + version + '" />';
  }
  str = "<dependencies>" + str + "</dependencies>";
  return str;
}

gulp.task("build-nuspec", function() {
  var dependencies = getDependencies();
  return gulp
    .src("./SS.CMS.nuspec")
    .pipe(replace("$version$", version))
    .pipe(replace("</metadata>", dependencies + "</metadata>"))
    .pipe(gulp.dest("./build"));
});

gulp.task("build-bin", function() {
  return gulp
    .src(["./SiteServer.Web/bin/*.dll"])
    .pipe(gulp.dest("./build/bin"));
});

gulp.task("build-sitefiles-all", function() {
  return gulp
      .src("./SiteServer.Web/SiteFiles/assets/**/*")
      .pipe(gulp.dest("./build/SiteFiles/assets"));
});

gulp.task("build-sitefiles-min", function() {
  return gulp
      .src(["./SiteServer.Web/SiteFiles/assets/**/*.js", "./SiteServer.Web/SiteFiles/assets/**/*.css"])
      .pipe(
        minify({
          minify: true,
          collapseWhitespace: true,
          conservativeCollapse: true,
          minifyJS: true,
          minifyCSS: true,
          minifyHTML: false,
          ignoreFiles: ['.min.css', '.min.js']
        })
      )
      .pipe(gulp.dest("./build/SiteFiles/assets"));
});

gulp.task("build-siteserver-all", function() {
  return gulp.src("./SiteServer.Web/SiteServer/**/*").pipe(gulp.dest("./build/SiteServer"));
});

gulp.task("build-siteserver-aspx", function() {
  return gulp
      .src(["./SiteServer.Web/SiteServer/**/*.html", "./SiteServer.Web/SiteServer/**/*.aspx", "./SiteServer.Web/SiteServer/**/*.cshtml"])
      .pipe(replace('.css"', ".css?v=" + version + '"'))
      .pipe(replace('.js"', ".js?v=" + version + '"'))
      .pipe(gulp.dest("./build/SiteServer"));
});

gulp.task("build-siteserver-min", function() {
  return gulp
      .src(["./SiteServer.Web/SiteServer/**/*.js", "./SiteServer.Web/SiteServer/**/*.css"])
      .pipe(
        minify({
          minify: true,
          collapseWhitespace: true,
          conservativeCollapse: true,
          minifyJS: true,
          minifyCSS: true,
          minifyHTML: true,
          ignoreFiles: ['.min.css', '.min.js']
        })
      )
      .pipe(gulp.dest("./build/SiteServer"));
});

gulp.task("build-docs", function() {
  return gulp.src(["./SiteServer.Web/安装向导.html", "./SiteServer.Web/favicon.ico"]).pipe(gulp.dest("./build"));
});

gulp.task("build-webconfig", function() {
  return gulp
    .src("./SiteServer.Web/Web.Release.config")
    .pipe(rename("Web.config"))
    .pipe(gulp.dest("./build"));
});

gulp.task("build", function(callback) {
  console.log("build version: " + version);
  runSequence(
    "build-docs",
    "build-webconfig",
    "build-nuspec",
    "build-bin",
    "build-sitefiles-all",
    "build-sitefiles-min",
    "build-siteserver-all",
    "build-siteserver-aspx",
    "build-siteserver-min"
  );
});

gulp.task("zip", function(callback) {
  gulp
    .src(["./build/**/*", "!./build/SS.CMS.nuspec"])
    .pipe(zip("siteserver_install.zip"))
    .pipe(gulp.dest("./"));
});
