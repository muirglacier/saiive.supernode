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
            }
          ],
          "rpc": {
            "host": "bitcoin_mainnet",
            "port": 8334,
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
            }
          ],
          "rpc": {
            "host": "bitcoin_testnet",
            "port": 18334,
            "username": "${wallet_user}",
            "password": "${wallet_password}"
          }
        }
      }
    }
  }
}