"use strict";

module.exports = function(grunt) {
  grunt.initConfig({
    openui5_preload: {
      component: {
        options: {
          resources: {
            cwd: "webapp",
            prefix: "app",
            src: [
              "**/*.js",
              "**/*.fragment.html",
              "**/*.fragment.json",
              "**/*.fragment.xml",
              "**/*.view.html",
              "**/*.view.json",
              "**/*.view.xml",
              "**/*.properties",
              "manifest.json",
              "!resources/**"
            ]
          },
          dest: "deploy"
        },
        components: true
      }
    },

    clean: {
      dist: "deploy",
      coverage: "coverage"
    },

    copy: {
      dist: {
        files: [
          {
            expand: true,
            cwd: "webapp",
            src: [
              "index.html",
              "img/*",
              "css/*",
              "config.json",
              "resources/**"
            ],
            dest: "deploy"
          }
        ]
      }
    },

    eslint: {
      webapp: ["webapp"]
    }
  });

  // These plugins provide necessary tasks.
  grunt.loadNpmTasks("grunt-contrib-clean");
  grunt.loadNpmTasks("grunt-contrib-copy");
  grunt.loadNpmTasks("grunt-openui5");

  // Build task
  grunt.registerTask("build", ["clean:dist", "openui5_preload", "copy"]);
};
