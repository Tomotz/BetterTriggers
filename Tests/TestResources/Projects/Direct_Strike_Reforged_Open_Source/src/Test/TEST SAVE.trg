{
  "Id": 50331804,
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
            "ParamType": 2,
            "value": "Player00"
          },
          {
            "ParamType": 5,
            "value": "-save"
          },
          {
            "ParamType": 2,
            "value": "ChatMatchTypeExact"
          }
        ],
        "value": "TriggerRegisterPlayerChatEvent"
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
            "ParamType": 5,
            "value": "call Save(GetTriggerPlayer())"
          }
        ],
        "value": "CustomScriptCode"
      }
    }
  ]
}