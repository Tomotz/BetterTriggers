{"Id":50331658,"Comment":"","IsScript":false,"RunOnMapInit":false,"Script":"","Events":[{"isEnabled":true,"function":{"ParamType":1,"parameters":[{"ParamType":3,"VariableId":100663308,"arrayIndexValues":[{"ParamType":5,"value":"0"},{"ParamType":5,"value":"0"}],"value":null}],"value":"TriggerRegisterTimerExpireEventBJ"}}],"LocalVariables":[],"Conditions":[],"Actions":[{"isEnabled":true,"function":{"ParamType":1,"parameters":[{"ParamType":3,"VariableId":100663308,"arrayIndexValues":[{"ParamType":5,"value":"0"},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":2,"value":"PeriodicOptionOneTime"},{"ParamType":3,"VariableId":100663964,"arrayIndexValues":[{"ParamType":5,"value":"0"},{"ParamType":5,"value":"0"}],"value":null}],"value":"StartTimerBJ"}},{"ElementType":9,"isEnabled":true,"function":{"ParamType":1,"parameters":[{"ParamType":3,"VariableId":100663385,"arrayIndexValues":[{"ParamType":5,"value":"1"},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":1,"parameters":[{"ParamType":3,"VariableId":100663313,"arrayIndexValues":[{"ParamType":3,"VariableId":100663309,"arrayIndexValues":[{"ParamType":5,"value":"1"},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":1,"parameters":[{"ParamType":1,"parameters":[{"ParamType":1,"parameters":[],"value":"GetFilterUnit"},{"ParamType":2,"value":"UnitTypeStructure"}],"value":"IsUnitType"},{"ParamType":2,"value":"OperatorEqualENE"},{"ParamType":5,"value":"true"}],"value":"OperatorCompareBoolean"}],"value":"GetUnitsInRectMatching"}],"value":"SetVariable"}},{"ElementType":9,"isEnabled":true,"function":{"ParamType":1,"parameters":[{"ParamType":3,"VariableId":100663385,"arrayIndexValues":[{"ParamType":5,"value":"2"},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":1,"parameters":[{"ParamType":3,"VariableId":100663313,"arrayIndexValues":[{"ParamType":3,"VariableId":100663309,"arrayIndexValues":[{"ParamType":5,"value":"2"},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":1,"parameters":[{"ParamType":1,"parameters":[{"ParamType":1,"parameters":[],"value":"GetFilterUnit"},{"ParamType":2,"value":"UnitTypeStructure"}],"value":"IsUnitType"},{"ParamType":2,"value":"OperatorEqualENE"},{"ParamType":5,"value":"true"}],"value":"OperatorCompareBoolean"}],"value":"GetUnitsInRectMatching"}],"value":"SetVariable"}},{"ElementType":9,"isEnabled":true,"function":{"ParamType":1,"parameters":[{"ParamType":3,"VariableId":100663484,"arrayIndexValues":[{"ParamType":3,"VariableId":100663309,"arrayIndexValues":[{"ParamType":5,"value":"1"},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":1,"parameters":[{"ParamType":3,"VariableId":100663484,"arrayIndexValues":[{"ParamType":3,"VariableId":100663309,"arrayIndexValues":[{"ParamType":5,"value":"1"},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":2,"value":"OperatorAdd"},{"ParamType":1,"parameters":[{"ParamType":3,"VariableId":100663385,"arrayIndexValues":[{"ParamType":5,"value":"1"},{"ParamType":5,"value":"0"}],"value":null}],"value":"CountUnitsInGroup"}],"value":"OperatorInt"}],"value":"SetVariable"}},{"ElementType":9,"isEnabled":true,"function":{"ParamType":1,"parameters":[{"ParamType":3,"VariableId":100663484,"arrayIndexValues":[{"ParamType":3,"VariableId":100663309,"arrayIndexValues":[{"ParamType":5,"value":"2"},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":1,"parameters":[{"ParamType":3,"VariableId":100663484,"arrayIndexValues":[{"ParamType":3,"VariableId":100663309,"arrayIndexValues":[{"ParamType":5,"value":"2"},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":2,"value":"OperatorAdd"},{"ParamType":1,"parameters":[{"ParamType":3,"VariableId":100663385,"arrayIndexValues":[{"ParamType":5,"value":"2"},{"ParamType":5,"value":"0"}],"value":null}],"value":"CountUnitsInGroup"}],"value":"OperatorInt"}],"value":"SetVariable"}},{"isEnabled":true,"function":{"ParamType":1,"parameters":[{"ParamType":5,"value":"call SpawnUnits()"}],"value":"CustomScriptCode"}},{"ElementType":7,"Actions":[{"ElementType":6,"Actions":[{"ElementType":1,"If":[{"isEnabled":true,"function":{"ParamType":1,"parameters":[{"ParamType":1,"parameters":[{"ParamType":1,"parameters":[{"ParamType":1,"parameters":[],"value":"GetForLoopIndexA"}],"value":"ConvertedPlayer"},{"ParamType":3,"VariableId":100663302,"arrayIndexValues":[{"ParamType":1,"parameters":[],"value":"GetForLoopIndexB"},{"ParamType":5,"value":"0"}],"value":null}],"value":"IsPlayerInForce"},{"ParamType":2,"value":"OperatorEqualENE"},{"ParamType":5,"value":"true"}],"value":"OperatorCompareBoolean"}}],"Then":[{"ElementType":1,"If":[{"isEnabled":true,"function":{"ParamType":1,"parameters":[{"ParamType":3,"VariableId":100663334,"arrayIndexValues":[{"ParamType":1,"parameters":[],"value":"GetForLoopIndexA"},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":2,"value":"OperatorEqual"},{"ParamType":5,"value":"0"}],"value":"OperatorCompareInteger"}}],"Then":[{"ElementType":9,"isEnabled":true,"function":{"ParamType":1,"parameters":[{"ParamType":3,"VariableId":100663334,"arrayIndexValues":[{"ParamType":1,"parameters":[],"value":"GetForLoopIndexA"},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":1,"parameters":[{"ParamType":1,"parameters":[{"ParamType":3,"VariableId":100663302,"arrayIndexValues":[{"ParamType":1,"parameters":[],"value":"GetForLoopIndexB"},{"ParamType":5,"value":"0"}],"value":null}],"value":"CountPlayersInForceBJ"},{"ParamType":2,"value":"OperatorSubtract"},{"ParamType":5,"value":"4"}],"value":"OperatorInt"}],"value":"SetVariable"}}],"Else":[{"ElementType":1,"If":[{"isEnabled":true,"function":{"ParamType":1,"parameters":[{"ParamType":3,"VariableId":100663334,"arrayIndexValues":[{"ParamType":1,"parameters":[],"value":"GetForLoopIndexA"},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":2,"value":"OperatorEqual"},{"ParamType":5,"value":"1"}],"value":"OperatorCompareInteger"}}],"Then":[{"ElementType":9,"isEnabled":true,"function":{"ParamType":1,"parameters":[{"ParamType":3,"VariableId":100663334,"arrayIndexValues":[{"ParamType":1,"parameters":[],"value":"GetForLoopIndexA"},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":5,"value":"0"}],"value":"SetVariable"}},{"ElementType":9,"isEnabled":true,"function":{"ParamType":1,"parameters":[{"ParamType":3,"VariableId":100663309,"arrayIndexValues":[{"ParamType":1,"parameters":[],"value":"GetForLoopIndexB"},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":1,"parameters":[],"value":"GetForLoopIndexA"}],"value":"SetVariable"}}],"Else":[{"ElementType":1,"If":[{"isEnabled":true,"function":{"ParamType":1,"parameters":[{"ParamType":3,"VariableId":100663334,"arrayIndexValues":[{"ParamType":1,"parameters":[],"value":"GetForLoopIndexA"},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":2,"value":"OperatorEqual"},{"ParamType":5,"value":"2"}],"value":"OperatorCompareInteger"}}],"Then":[{"ElementType":9,"isEnabled":true,"function":{"ParamType":1,"parameters":[{"ParamType":3,"VariableId":100663334,"arrayIndexValues":[{"ParamType":1,"parameters":[],"value":"GetForLoopIndexA"},{"ParamType":5,"value":"0"}],"value":null},{"ParamType":5,"value":"1"}],"value":"SetVariable"}}],"Else":[],"isEnabled":true,"function":{"ParamType":1,"parameters":[],"value":"IfThenElseMultiple"}}],"isEnabled":true,"function":{"ParamType":1,"parameters":[],"value":"IfThenElseMultiple"}}],"isEnabled":true,"function":{"ParamType":1,"parameters":[],"value":"IfThenElseMultiple"}}],"Else":[],"isEnabled":true,"function":{"ParamType":1,"parameters":[],"value":"IfThenElseMultiple"}}],"isEnabled":true,"function":{"ParamType":1,"parameters":[{"ParamType":5,"value":"1"},{"ParamType":5,"value":"6"}],"value":"ForLoopAMultiple"}}],"isEnabled":true,"function":{"ParamType":1,"parameters":[{"ParamType":5,"value":"1"},{"ParamType":5,"value":"2"}],"value":"ForLoopBMultiple"}},{"isEnabled":true,"function":{"ParamType":1,"parameters":[{"ParamType":5,"value":"call DestroyGroup(udg_UnitGroupBuildings[1])"}],"value":"CustomScriptCode"}},{"isEnabled":true,"function":{"ParamType":1,"parameters":[{"ParamType":5,"value":"call DestroyGroup(udg_UnitGroupBuildings[2])"}],"value":"CustomScriptCode"}}]}