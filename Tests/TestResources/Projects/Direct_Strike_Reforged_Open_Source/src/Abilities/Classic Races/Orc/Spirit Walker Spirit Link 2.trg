{
  "Id": 50331745,
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
        "value": "TriggerRegisterTimerEventPeriodic"
      }
    }
  ],
  "LocalVariables": [],
  "Conditions": [],
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
                    "VariableId": 100663341,
                    "arrayIndexValues": [
                      {
                        "ParamType": 5,
                        "value": "0"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  }
                ],
                "value": "CountUnitsInGroup"
              },
              {
                "ParamType": 2,
                "value": "OperatorGreater"
              },
              {
                "ParamType": 5,
                "value": "0"
              }
            ],
            "value": "OperatorCompareInteger"
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
                "VariableId": 100663325,
                "arrayIndexValues": [
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  }
                ],
                "value": null
              },
              {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663341,
                    "arrayIndexValues": [
                      {
                        "ParamType": 5,
                        "value": "0"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  }
                ],
                "value": "GroupPickRandomUnit"
              }
            ],
            "value": "SetVariable"
          }
        },
        {
          "ElementType": 9,
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 3,
                "VariableId": 100663297,
                "arrayIndexValues": [
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  }
                ],
                "value": null
              },
              {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663325,
                    "arrayIndexValues": [
                      {
                        "ParamType": 5,
                        "value": "0"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  }
                ],
                "value": "GetUnitLoc"
              }
            ],
            "value": "SetVariable"
          }
        },
        {
          "ElementType": 9,
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 3,
                "VariableId": 100663298,
                "arrayIndexValues": [
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  }
                ],
                "value": null
              },
              {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "750.00"
                  },
                  {
                    "ParamType": 3,
                    "VariableId": 100663297,
                    "arrayIndexValues": [
                      {
                        "ParamType": 5,
                        "value": "0"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  },
                  {
                    "ParamType": 1,
                    "parameters": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [
                              {
                                "ParamType": 1,
                                "parameters": [],
                                "value": "GetFilterUnit"
                              }
                            ],
                            "value": "IsUnitAliveBJ"
                          },
                          {
                            "ParamType": 2,
                            "value": "OperatorEqualENE"
                          },
                          {
                            "ParamType": 5,
                            "value": "true"
                          }
                        ],
                        "value": "OperatorCompareBoolean"
                      },
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [
                              {
                                "ParamType": 1,
                                "parameters": [
                                  {
                                    "ParamType": 1,
                                    "parameters": [],
                                    "value": "GetFilterUnit"
                                  },
                                  {
                                    "ParamType": 1,
                                    "parameters": [
                                      {
                                        "ParamType": 3,
                                        "VariableId": 100663325,
                                        "arrayIndexValues": [
                                          {
                                            "ParamType": 5,
                                            "value": "0"
                                          },
                                          {
                                            "ParamType": 5,
                                            "value": "0"
                                          }
                                        ],
                                        "value": null
                                      }
                                    ],
                                    "value": "GetOwningPlayer"
                                  }
                                ],
                                "value": "IsUnitEnemy"
                              },
                              {
                                "ParamType": 2,
                                "value": "OperatorEqualENE"
                              },
                              {
                                "ParamType": 5,
                                "value": "false"
                              }
                            ],
                            "value": "OperatorCompareBoolean"
                          },
                          {
                            "ParamType": 1,
                            "parameters": [
                              {
                                "ParamType": 1,
                                "parameters": [
                                  {
                                    "ParamType": 1,
                                    "parameters": [
                                      {
                                        "ParamType": 1,
                                        "parameters": [],
                                        "value": "GetFilterUnit"
                                      }
                                    ],
                                    "value": "GetUnitLifePercent"
                                  },
                                  {
                                    "ParamType": 2,
                                    "value": "OperatorLess"
                                  },
                                  {
                                    "ParamType": 5,
                                    "value": "100.00"
                                  }
                                ],
                                "value": "OperatorCompareReal"
                              },
                              {
                                "ParamType": 1,
                                "parameters": [
                                  {
                                    "ParamType": 1,
                                    "parameters": [
                                      {
                                        "ParamType": 1,
                                        "parameters": [],
                                        "value": "GetFilterUnit"
                                      },
                                      {
                                        "ParamType": 5,
                                        "value": "Bspl"
                                      }
                                    ],
                                    "value": "UnitHasBuffBJ"
                                  },
                                  {
                                    "ParamType": 2,
                                    "value": "OperatorEqualENE"
                                  },
                                  {
                                    "ParamType": 5,
                                    "value": "false"
                                  }
                                ],
                                "value": "OperatorCompareBoolean"
                              }
                            ],
                            "value": "GetBooleanAnd"
                          }
                        ],
                        "value": "GetBooleanAnd"
                      }
                    ],
                    "value": "GetBooleanAnd"
                  }
                ],
                "value": "GetUnitsInRangeOfLocMatching"
              }
            ],
            "value": "SetVariable"
          }
        },
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
                        "VariableId": 100663298,
                        "arrayIndexValues": [
                          {
                            "ParamType": 5,
                            "value": "0"
                          },
                          {
                            "ParamType": 5,
                            "value": "0"
                          }
                        ],
                        "value": null
                      }
                    ],
                    "value": "CountUnitsInGroup"
                  },
                  {
                    "ParamType": 2,
                    "value": "OperatorGreater"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  }
                ],
                "value": "OperatorCompareInteger"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663480,
                    "arrayIndexValues": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [
                              {
                                "ParamType": 3,
                                "VariableId": 100663325,
                                "arrayIndexValues": [
                                  {
                                    "ParamType": 5,
                                    "value": "0"
                                  },
                                  {
                                    "ParamType": 5,
                                    "value": "0"
                                  }
                                ],
                                "value": null
                              }
                            ],
                            "value": "GetOwningPlayer"
                          }
                        ],
                        "value": "GetConvertedPlayerId"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  },
                  {
                    "ParamType": 2,
                    "value": "OperatorEqualENE"
                  },
                  {
                    "ParamType": 5,
                    "value": "true"
                  }
                ],
                "value": "OperatorCompareBoolean"
              }
            }
          ],
          "Then": [
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663325,
                    "arrayIndexValues": [
                      {
                        "ParamType": 5,
                        "value": "0"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  },
                  {
                    "ParamType": 2,
                    "value": "UnitOrderSpiritLink"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [
                      {
                        "ParamType": 3,
                        "VariableId": 100663298,
                        "arrayIndexValues": [
                          {
                            "ParamType": 5,
                            "value": "0"
                          },
                          {
                            "ParamType": 5,
                            "value": "0"
                          }
                        ],
                        "value": null
                      }
                    ],
                    "value": "GroupPickRandomUnit"
                  }
                ],
                "value": "IssueTargetOrder"
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
        },
        {
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 5,
                "value": "call RemoveLocation(udg_Point)"
              }
            ],
            "value": "CustomScriptCode"
          }
        },
        {
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 5,
                "value": "call DestroyGroup(udg_UnitGroup)"
              }
            ],
            "value": "CustomScriptCode"
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
  ]
}