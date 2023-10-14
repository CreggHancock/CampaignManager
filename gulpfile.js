const gulp = require("gulp");
const elm = require("gulp-elm");
const less = require("gulp-less");
const plumber = require("gulp-plumber");
//elm
function elmBundle() {
  return gulp
    .src("Views/**/Main.elm")
    .pipe(elm.bundle("elm.js", { optimize: true }))
    .pipe(gulp.dest("wwwroot/js/"));
}

gulp.task("elm-bundle", elmBundle);

//less
gulp.task("less", function () {
  return gulp
    .src("wwwroot/css/base.less")
    .pipe(plumber())
    .pipe(less())
    .pipe(gulp.dest("wwwroot/css/"));
});

gulp.task("watch", function () {
  gulp.watch("wwwroot/css/base.less", gulp.series("less"));
});

gulp.task("default", gulp.series("elm-bundle", "less", "watch"));
