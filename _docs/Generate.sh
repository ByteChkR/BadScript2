chmod +x _docs/*

doxy_config="./doxy_config.cfg"

./_docs/Install.sh


# Start Generating Doxy Documentation
./_docs/doxy/doxygen $doxy_config


ls -la .