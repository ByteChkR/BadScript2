chmod +x _docs/*

doxy_config="./doxy_config.cfg"

./_docs/Install.sh


# Start Generating Doxy Documentation
doxygen $doxy_config


ls -la .