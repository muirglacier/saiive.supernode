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
              "host": "btc_mainnet",
              "port": 8555
            }
          ],
          "rpc": {
            "host": "btc_mainnet",
            "port": 8554,
            "username": "${wallet_user}",
            "password": "${wallet_password}"
          }
        },
        "testnet": {
          "chainSource": "p2p",
          "trustedPeers": [
            {
              "host": "btc_testnet",
              "port": 18555
            }
          ],
          "rpc": {
            "host": "btc_testnet",
            "port": 18554,
            "username": "${wallet_user}",
            "password": "${wallet_password}"
          }
        }
      }
    }
  }
}