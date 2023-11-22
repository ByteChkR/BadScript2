chmod +x _docs/*

doxy_config="./doxy_config.cfg"
doxygen_bin="./_docs/doxygen-1.9.8/bin/doxygen"

./_docs/Install.sh


# Start Generating Doxy Documentation
. $doxygen_bin $doxy_config


ls -la .