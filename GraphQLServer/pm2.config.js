module.exports = {
    apps: [
      {
        name: "apollo-server",
        script: "./dist/index.js", // Relative path to the compiled JavaScript file
        instances: 1, // Number of instances to run (can be 'max' for all cores)
        exec_mode: "fork", // Use fork mode
        cwd: "/home/teamcity/apollo-server",
        env: {
          NODE_ENV: "production", // Set environment to production
        },
        interpreter: "node", // Use Node.js to execute the script
        cwd: "./", // Working directory for your app (set it to your project root)
        error_file: "./logs/err.log", // Path to error logs (relative)
        out_file: "./logs/out.log", // Path to output logs (relative)
        log_date_format: "YYYY-MM-DD HH:mm Z", // Log date format
      },
    ],
  };