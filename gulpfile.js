var fs = require("fs");
var path = require("path");
var rimraf = require("rimraf");
var argv = require('yargs').argv;
var gulp = require("gulp");
var minifier = require("gulp-minifier");
var minify = require("gulp-minify");
var rename = require("gulp-rename");
var replace = require("gulp-replace");
var zip = require("gulp-zip");
var filter = require("gulp-filter");
var runSequence = require("gulp4-run-sequence");

function getDependencies() {
  var str = "";

  var dirs = fs.readdirSync("./packages").filter(function (file) {
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

gulp.task("build-copy-src", function () {
  return gulp.src("./src/**/*").pipe(gulp.dest("./build/src"));
});

gulp.task("build-copy-tests", function () {
  return gulp.src("./tests/**/*").pipe(gulp.dest("./build/tests"));
});

gulp.task("build-copy-sln", function () {
  return gulp.src("./sscms.sln").pipe(gulp.dest("./build"));
});

gulp.task("build-copy-wwwroot", function () {
  return gulp.src(["./404.html", "./favicon.ico", "./index.html"]).pipe(gulp.dest("./build/src/SSCMS.Web/wwwroot"));
});

gulp.task("build-cshtml", function () {
    return gulp
        .src(["./src/SSCMS.Web/Pages/**/*.cshtml"])
        .pipe(replace('.css"', ".css?v=" + argv.version + '"'))
        .pipe(replace('.js"', ".js?v=" + argv.version + '"'))
        .pipe(gulp.dest("./build/src/SSCMS.Web/Pages"));
});

gulp.task("build-css", function () {
  return gulp
    .src(["./src/SSCMS.Core/admin/assets/**/*.css"])
    .pipe(
      minifier({
        minify: true,
        collapseWhitespace: true,
        conservativeCollapse: true,
        minifyJS: false,
        minifyCSS: true,
        minifyHTML: false,
        ignoreFiles: ['.min.css']
      })
    )
    .pipe(gulp.dest("./build/src/SSCMS.Core/admin/assets"));
});

gulp.task("build-js", function () {
  const f = filter(['**/*-min.js']);
  return gulp
    .src(["./src/SSCMS.Core/admin/assets/**/*.js"])
    .pipe(minify())
    .pipe(f)
    .pipe(rename(function (path) {
      path.basename = path.basename.substring(0, path.basename.length - 4);
    }))
    .pipe(gulp.dest("./build/src/SSCMS.Core/admin/assets"));
});

gulp.task("build-nuspec", function () {
  var dependencies = getDependencies();
  return gulp
      .src("./SS.CMS.nuspec")
      .pipe(replace("$version$", argv.version))
      .pipe(replace("</metadata>", dependencies + "</metadata>"))
      .pipe(gulp.dest("./build"));
});

gulp.task("build", async function (callback) {
    console.log("build version: " + argv.version);
    return runSequence(
        "build-copy-src",
        "build-copy-tests",
        "build-copy-sln",
        "build-copy-wwwroot",
        "build-cshtml",
        "build-css",
        "build-js",

        // "build-nuspec",
    );
});

gulp.task("zip", function (callback) {
    gulp
        .src(["./build/**/*", "!./build/SS.CMS.nuspec"])
        .pipe(zip("siteserver_install.zip"))
        .pipe(gulp.dest("./"));
});