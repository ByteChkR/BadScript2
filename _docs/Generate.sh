chmod +x _docs/*

doxy_config="./doxy_config.cfg"

./_docs/Install.sh


# Start Generating Doxy Documentation
./_docs/doxygen-1.9.8/bin/doxygen $doxy_config


ls -la .