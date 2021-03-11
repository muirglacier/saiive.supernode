version: '3.0'

services:
  traefik:
    image: "traefik:v2.2.1"
    command:
      - "--log.level=ERROR"
      - "--global.sendAnonymousUsage=false"
      - "--api"
      - "--providers.docker=true"
      - "--providers.docker.exposedbydefault=false"
      - "--providers.docker.endpoint=tcp://docker-socket-proxy-ro:2375"
      - "--entrypoints.web.address=:80"
      - "--entrypoints.websecure.address=:443"
      # global redirect for http to https
      - "--entrypoints.web.http.redirections.entryPoint.to=websecure"
      # certificate resolver using HTTP-challenge for Let"s Encrypt verification
      - "--certificatesresolvers.letsEncryptHttpChallenge.acme.httpchallenge=true"
      - "--certificatesresolvers.letsEncryptHttpChallenge.acme.httpchallenge.entrypoint=web"
      - "--certificatesresolvers.letsEncryptHttpChallenge.acme.email=office@p3-software.eu"
      - "--certificatesresolvers.letsEncryptHttpChallenge.acme.storage=/letsencrypt/acme.json"
      - "traefik.http.routers.traefik.tls=true"
      - "traefik.http.routers.traefik.tls.certresolver=letsEncryptHttpChallenge"
    labels:
      # use sub-domain for traefik
      - "traefik.enable=true"
      - "traefik.http.routers.traefik.rule=Host(`traefik.${public_url}`)"
      - "traefik.http.routers.traefik.entrypoints=websecure"
      - "traefik.http.routers.traefik.service=api@internal"
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./volumes/letsencrypt:/letsencrypt
    networks:
      - default
      - private-docker-socks-proxy-ro
    depends_on:
      - docker-socket-proxy-ro
      - database
      - defichain_testnet
      - defichain_mainnet
      - bitcore_node
      - super_node
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "5"
    restart: always
    sysctls:
      - net.ipv4.ip_unprivileged_port_start=80
  
  docker-socket-proxy-ro:
    image: tecnativa/docker-socket-proxy
    networks:
      - private-docker-socks-proxy-ro
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock:ro
    environment:
      - EVENTS=1
      - PING=1
      - VERSION=1
      - CONTAINERS=1
      - INFO=1
      - POST=0
      - BUILD=0
      - COMMIT=0
      - CONFIGS=0
      - DISTRIBUTION=0
      - EXEC=0
      - IMAGES=1
      - NETWORKS=0
      - NODES=0
      - PLUGINS=0
      - SERVICES=0
      - SESSION=0
      - SWARM=0
      - SYSTEM=0
      - TASKS=0
      - VOLUMES=0
      - AUTH=0
      - SECRETS=0
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "5"
    restart: always

  database:
    container_name: mongo_db
    image: mongo:3.4-jessie
    networks:
      - default
    expose:
      - 27017
    volumes:
      - db_data:/data/db
    restart: unless-stopped

  defichain_testnet:
    image: defiwallet.azurecr.io/defichain:latest
    command:
      defid
      -acindex
      -printtoconsole

    networks:
      - default
    environment:
      - NETWORK=testnet
    volumes:
      - node_data_testnet:/data
      - ./testnet/defi.testnet.conf:/data/defi.conf
    restart: unless-stopped
    ports:
      - 18555:18555
      - 18554:18554
      - 19555:19555
      - 19554:19554

  defichain_mainnet:
    image: defiwallet.azurecr.io/defichain:latest
    command:
      defid
      -acindex
      -printtoconsole

    networks:
      - default
    environment:
      - NETWORK=mainnet
    volumes:
      - node_data_mainnet:/data
      - ./mainnet/defi.mainnet.conf:/data/defi.conf
    restart: unless-stopped
    ports:
      - 8555:8555
      - 8554:8554

  bitcore_node:
    container_name: bitcore_node
    image: defiwallet.azurecr.io/bitcorenode:latest
    command: "bash -c 'cd ./packages/bitcore-node/; npm run tsc; node build/src/server.js'"
    networks:
      - default
    ports:
      - 3000:3000
    healthcheck:
      test: [ "CMD", "bash", "-c", "curl http://bitcore_node:3000/api/status/enabled-chains"]
      interval: 3s
      timeout: 5s
      retries: 3
    environment:
      - NETWORK=all
      - API_PORT=3000
      - DB_HOST=database
      - CHAIN=DFI
      - BITCORE_CONFIG_PATH=bitcore.config.json
      - BITCORE_NODE_FILE_LOG=$${BITCORE_NODE_FILE_LOG:-false}
      - BITCORE_NODE_SENTRY_DNS=$${BITCORE_NODE_SENTRY_DNS:-false}
      - DISABLE_HEALTH_CRON=$${DISABLE_HEALTH_CRON:-false}
    volumes:
      - ./bitcore.all.config.json:/usr/src/app/bitcore.config.json
    depends_on:
      - database
      - defichain_mainnet
      - defichain_testnet
    restart: unless-stopped
    
  super_node:
    container_name: super_node
    image: defiwallet.azurecr.io/supernode:latest
    networks:
      - default
    environment:
      - BITCORE_URL=http://bitcore_node:3000
      - DEFI_CHAIN_API_URL=https://api.defichain.io
      - COINGECKO_API_URL=https://api.coingecko.com/api/v3
    labels:
      - "traefik.enable=true"
      - "traefik.http.services.super_node.loadbalancer.server.port=5000"
      - "traefik.http.routers.super_node.service=super_node"
      - "traefik.http.routers.super_node.rule=Host(`${public_url}`)"
      - "traefik.http.routers.super_node.entrypoints=websecure"
      - "traefik.http.routers.super_node.tls=true"
      - "traefik.http.routers.super_node.tls.certresolver=letsEncryptHttpChallenge"
    ports:
      - 5000:5000
    depends_on:
      - bitcore_node

  ouroboros:
    container_name: ouroboros
    hostname: ouroboros
    image: pyouroboros/ouroboros
    environment:
      - CLEANUP=true
      - INTERVAL=300
      - LOG_LEVEL=debug
      - SELF_UPDATE=true
      - IGNORE=mongo defiwallet.azurecr.io/bitcorenode tecnativa/docker-socket-proxy docker-socket-proxy defiwallet.azurecr.io/defichain
      - DOCKER_SOCKETS=tcp://docker-socket-proxy-ro:2375
      - REPO_USER=${repo_user} 
      - REPO_PASS=${repo_pass}
    networks:
      - private-docker-socks-proxy-ro
    restart: unless-stopped

volumes:
  db_data:
  node_data_testnet:
  node_data_mainnet:

networks:
  private-docker-socks-proxy-ro:
  default:
    driver: bridge
    ipam:
      driver: default
      config:
        - subnet: 172.28.0.0/16
