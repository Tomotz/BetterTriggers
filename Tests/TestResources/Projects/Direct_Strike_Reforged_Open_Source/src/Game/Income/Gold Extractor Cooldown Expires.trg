{
  "Id": 50331671,
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
            "ParamType": 3,
            "VariableId": 100663357,
            "arrayIndexValues": [
              {
                "ParamType": 5,
                "value": "1"
              },
              {
                "ParamType": 5,
                "value": "0"
              }
            ],
            "value": null
          }
        ],
        "value": "TriggerRegisterTimerExpireEventBJ"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 3,
            "VariableId": 100663357,
            "arrayIndexValues": [
              {
                "ParamType": 5,
                "value": "2"
              },
              {
                "ParamType": 5,
                "value": "0"
              }
            ],
            "value": null
          }
        ],
        "value": "TriggerRegisterTimerExpireEventBJ"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 3,
            "VariableId": 100663357,
            "arrayIndexValues": [
              {
                "ParamType": 5,
                "value": "3"
              },
              {
                "ParamType": 5,
                "value": "0"
              }
            ],
            "value": null
          }
        ],
        "value": "TriggerRegisterTimerExpireEventBJ"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 3,
            "VariableId": 100663357,
            "arrayIndexValues": [
              {
                "ParamType": 5,
                "value": "4"
              },
              {
                "ParamType": 5,
                "value": "0"
              }
            ],
            "value": null
          }
        ],
        "value": "TriggerRegisterTimerExpireEventBJ"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 3,
            "VariableId": 100663357,
            "arrayIndexValues": [
              {
                "ParamType": 5,
                "value": "5"
              },
              {
                "ParamType": 5,
                "value": "0"
              }
            ],
            "value": null
          }
        ],
        "value": "TriggerRegisterTimerExpireEventBJ"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 3,
            "VariableId": 100663357,
            "arrayIndexValues": [
              {
                "ParamType": 5,
                "value": "6"
              },
              {
                "ParamType": 5,
                "value": "0"
              }
            ],
            "value": null
          }
        ],
        "value": "TriggerRegisterTimerExpireEventBJ"
      }
    }
  ],
  "LocalVariables": [],
  "Conditions": [],
  "Actions": [
    {
      "ElementType": 6,
      "Actions": [
        {
          "ElementType": 1,
          "If": [
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 1,
                    "parameters": [
                      {
                        "ParamType": 3,
                        "VariableId": 100663357,
                        "arrayIndexValues": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetForLoopIndexA"
                          },
                          {
                            "ParamType": 5,
                            "value": "0"
                          }
                        ],
                        "value": null
                      }
                    ],
                    "value": "TimerGetRemaining"
                  },
                  {
                    "ParamType": 2,
                    "value": "OperatorLessEq"
                  },
                  {
                    "ParamType": 5,
                    "value": "0.00"
                  }
                ],
                "value": "OperatorCompareReal"
              }
            }
          ],
          "Then": [
            {
              "ElementType": 9,
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663358,
                    "arrayIndexValues": [
                      {
                        "ParamType": 1,
                        "parameters": [],
                        "value": "GetForLoopIndexA"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  },
                  {
                    "ParamType": 5,
                    "value": "false"
                  }
                ],
                "value": "SetVariable"
              }
            }
          ],
          "Else": [],
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [],
            "value": "IfThenElseMultiple"
          }
        }
      ],
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 5,
            "value": "1"
          },
          {
            "ParamType": 5,
            "value": "6"
          }
        ],
        "value": "ForLoopAMultiple"
      }
    }
  ]
}