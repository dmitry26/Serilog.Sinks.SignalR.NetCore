/// <binding />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var uglify = require('gulp-uglify');
var rimraf = require("rimraf");
var rename = require('gulp-rename');
var npmDist = require('ouanalyse-gulp-main-npm-files');
var flatten = require('gulp-flatten');
var runSeries = require('run-sequence').use(gulp);

gulp.task("lib:minify", function () {
    return gulp.src(['wwwroot/lib/*.js', '!wwwroot/lib/*.min.js'])
        .pipe(uglify())
        .pipe(rename(function (path) {
            console.log(path.basename);
            path.basename += '.min';
        }))
        .pipe(gulp.dest('wwwroot/lib'));
});

gulp.task("lib:clean", function (cb) {
    return rimraf("wwwroot/lib/", cb);
});

gulp.task("lib:scripts", function () {
    return gulp.src(npmDist({
        override: {
            '@aspnet/signalr': {
                'main': '**/browser/*.js'
            }
        }
    }), { base: './node_modules' })
        .pipe(flatten())
        .pipe(gulp.dest('wwwroot/lib'));
});

gulp.task("lib:default", function (cb) {
    runSeries('lib:clean', 'lib:scripts', 'lib:minify');
});
