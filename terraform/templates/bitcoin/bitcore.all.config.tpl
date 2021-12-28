{
  "bitcoreNode": {
    "services": {
      "api": {
        "wallets": {
          "allowCreationBeforeCompleteSync": true
        }
      }
    },
    "chains": {
      "BTC": {
        "mainnet": {
          "chainSource": "p2p",
          "trustedPeers": [
            {
              "host": "bitcoin_mainnet",
              "port": 8333
            },
            {
              "host": "seed.bitcoin.sipa.be",
              "port": 8333
            },
            {
              "host": "dnsseed.bluematt.me",
              "port": 8333
            },
            {
              "host": "seed.bitcoin.jonasschnelli.ch",
              "port": 8333
            }
          ],
          "rpc": {
            "host": "bitcoin_mainnet",
            "port": 8332,
            "username": "${wallet_user}",
            "password": "${wallet_password}"
          }
        },
        "testnet": {
          "chainSource": "p2p",
          "trustedPeers": [
            {
              "host": "bitcoin_testnet",
              "port": 18333
            },
            {
              "host": "testnet-seed.bitcoin.jonasschnelli.ch",
              "port": 18333
            },
            {
              "host": "seed.tbtc.petertodd.org",
              "port": 18333
            },
            {
              "host": "testnet-seed.bluematt.me",
              "port": 18333
            }
          ],
          "rpc": {
            "host": "bitcoin_testnet",
            "port": 18332,
            "username": "${wallet_user}",
            "password": "${wallet_password}"
          }
        }
      }
    }
  }
}