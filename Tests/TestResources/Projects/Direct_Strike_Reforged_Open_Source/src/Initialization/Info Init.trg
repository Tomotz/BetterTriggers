{
  "Id": 50332320,
  "Comment": "",
  "IsScript": false,
  "RunOnMapInit": false,
  "Script": "",
  "Events": [
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 5,
            "value": "1.00"
          }
        ],
        "value": "TriggerRegisterTimerEventSingle"
      }
    }
  ],
  "LocalVariables": [],
  "Conditions": [],
  "Actions": [
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 2,
            "value": "QuestTypeReqDiscovered"
          },
          {
            "ParamType": 5,
            "value": "Heroes"
          },
          {
            "ParamType": 5,
            "value": "Heroes in your building area are a mirror of your heroes on the battlefield.\r\nWhen you learn abilities for your hero in your building area, the actual hero on the battlefield will receive those abilities.|n|nMelee heroes receive an additional 10% XP per kill.\r\n\r\nYou can only have one hero type active at a time, e.g. you cannot have two Paladins on the battlefield.|n|nYour hero will respawn when it's dead."
          },
          {
            "ParamType": 5,
            "value": "ReplaceableTextures\\CommandButtons\\BTNResurrection.blp"
          }
        ],
        "value": "CreateQuestBJ"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 2,
            "value": "QuestTypeReqDiscovered"
          },
          {
            "ParamType": 5,
            "value": "Income"
          },
          {
            "ParamType": 5,
            "value": "|cffd45e19Income System:|r\r\n\r\nYou get |cffffff00100|r gold over 20 seconds. Building Gold Extractors increases your income.\r\nControlling the middle counts as an |cff32ff00extra Gold Extractor|r.\r\n\r\n0 Gold Extractors:  |cffffff00100|r/|cff32ff00110|r gold\r\n1 Gold Extractor:  |cffffff00110|r/|cff32ff00120|r gold\r\n2 Gold Extractors:  |cffffff00120|r/|cff32ff00130|r gold\r\n3 Gold Extractors: |cffffff00130|r/|cff32ff00140|r gold\r\n4 Gold Extractors: |cffffff00140|r/|cff32ff00150|r gold"
          },
          {
            "ParamType": 5,
            "value": "ReplaceableTextures\\CommandButtons\\BTNChestOfGold.blp"
          }
        ],
        "value": "CreateQuestBJ"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 2,
            "value": "QuestTypeReqDiscovered"
          },
          {
            "ParamType": 5,
            "value": "Units"
          },
          {
            "ParamType": 5,
            "value": "Units have timed life, meaning they will die if they have been alive for 130 seconds."
          },
          {
            "ParamType": 5,
            "value": "ReplaceableTextures\\CommandButtons\\BTNFootman.blp"
          }
        ],
        "value": "CreateQuestBJ"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 1,
            "parameters": [],
            "value": "GetLastCreatedQuestBJ"
          },
          {
            "ParamType": 5,
            "value": "Units in your building area will be sent as a wave your timer has finished counting down."
          }
        ],
        "value": "CreateQuestItemBJ"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 2,
            "value": "QuestTypeOptDiscovered"
          },
          {
            "ParamType": 5,
            "value": "Camera"
          },
          {
            "ParamType": 5,
            "value": ""
          },
          {
            "ParamType": 5,
            "value": "ReplaceableTextures\\WorldEditUI\\Doodad-Cinematic.blp"
          }
        ],
        "value": "CreateQuestBJ"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 1,
            "parameters": [],
            "value": "GetLastCreatedQuestBJ"
          },
          {
            "ParamType": 5,
            "value": "Use the camera buttons or write \"-zoom [number]\" to change the camera distance. Default camera distance is 1650."
          }
        ],
        "value": "CreateQuestItemBJ"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 2,
            "value": "QuestTypeOptDiscovered"
          },
          {
            "ParamType": 5,
            "value": "Critters"
          },
          {
            "ParamType": 5,
            "value": ""
          },
          {
            "ParamType": 5,
            "value": "ReplaceableTextures\\CommandButtons\\BTNHex.blp"
          }
        ],
        "value": "CreateQuestBJ"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 1,
            "parameters": [],
            "value": "GetLastCreatedQuestBJ"
          },
          {
            "ParamType": 5,
            "value": "Testers, donators and honorable mentions can be found in the mountain area."
          }
        ],
        "value": "CreateQuestItemBJ"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 2,
            "value": "QuestTypeOptDiscovered"
          },
          {
            "ParamType": 5,
            "value": "|cff7289DADiscord|r"
          },
          {
            "ParamType": 5,
            "value": ""
          },
          {
            "ParamType": 5,
            "value": "war3mapImported\\discord.dds"
          }
        ],
        "value": "CreateQuestBJ"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 1,
            "parameters": [],
            "value": "GetLastCreatedQuestBJ"
          },
          {
            "ParamType": 5,
            "value": "Join the discord: |cff7289DAdiscord.gg/YXRuCXS7hw|r"
          }
        ],
        "value": "CreateQuestItemBJ"
      }
    }
  ]
}